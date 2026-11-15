using System.Text.RegularExpressions;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

public enum AccountSort { LoginName, CreatedAt, LastLoginAt }

public sealed record AccountQuery(string? Text = null, AccountStatus? Status = null, string? Tag = null,
    AccountSort Sort = AccountSort.LoginName, bool Descending = false, int Skip = 0, int Take = 50);

public sealed record AccountSummary(long Id, string LoginName, AccountStatus Status, bool IsGm, short SlotCount,
    int CharacterCount, IReadOnlyList<string> Tags, DateTimeOffset CreatedAt, DateTimeOffset? LastLoginAt, uint Version);

public sealed record AccountChanges(bool? IsGm = null, short? SlotCount = null, string? Note = null, IReadOnlyList<string>? Tags = null);

public enum LoginResult { Ok, UnknownOrWrongPassword, Banned, Suspended, PasswordLoginDisabled }

public sealed partial class AccountService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    public const string LocalAccount = "local";

    [GeneratedRegex("^[A-Za-z0-9_.-]{3,32}$")]
    private static partial Regex LoginPattern();

    public async Task<Page<AccountSummary>> SearchAsync(AccountQuery query, CancellationToken token = default)
    {
        if (query.Take is < 1 or > 200 || query.Skip < 0)
            throw new GameDataException(GameDataError.Invalid, "Take must be 1–200 and Skip non-negative");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        IQueryable<Account> accounts = db.Accounts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Text))
        {
            // EF.Functions.ILike sends the pattern as a parameter; escape LIKE wildcards from user input.
            string pattern = "%" + EscapeLike(query.Text.Trim()) + "%";
            accounts = accounts.Where(a => EF.Functions.ILike(a.LoginName, pattern, "\\") ||
                a.Characters.Any(c => c.DeletedAt == null && EF.Functions.ILike(c.Name, pattern, "\\")));
        }
        if (query.Status is { } status) accounts = accounts.Where(a => a.Status == status);
        if (!string.IsNullOrWhiteSpace(query.Tag)) accounts = accounts.Where(a => a.Tags.Contains(query.Tag));
        // Sorting is a whitelist of expressions, never a column name from the caller.
        accounts = (query.Sort, query.Descending) switch
        {
            (AccountSort.CreatedAt, false) => accounts.OrderBy(a => a.CreatedAt).ThenBy(a => a.Id),
            (AccountSort.CreatedAt, true) => accounts.OrderByDescending(a => a.CreatedAt).ThenBy(a => a.Id),
            (AccountSort.LastLoginAt, false) => accounts.OrderBy(a => a.LastLoginAt).ThenBy(a => a.Id),
            (AccountSort.LastLoginAt, true) => accounts.OrderByDescending(a => a.LastLoginAt).ThenBy(a => a.Id),
            (_, true) => accounts.OrderByDescending(a => a.LoginName),
            _ => accounts.OrderBy(a => a.LoginName)
        };
        int total = await accounts.CountAsync(token);
        List<AccountSummary> items = await accounts.Skip(query.Skip).Take(query.Take)
            .Select(a => new AccountSummary(a.Id, a.LoginName, a.Status, a.IsGm, a.SlotCount,
                a.Characters.Count(c => c.DeletedAt == null), a.Tags, a.CreatedAt, a.LastLoginAt, a.Version))
            .ToListAsync(token);
        return new(items, total, query.Skip, query.Take);
    }

    public async Task<Account?> GetAsync(long id, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.Accounts.AsNoTracking()
            .Include(a => a.Characters.OrderBy(c => c.Slot))
            .Include(a => a.Sanctions.OrderByDescending(s => s.StartsAt))
            .AsSplitQuery()
            .FirstOrDefaultAsync(a => a.Id == id, token);
    }

    public async Task<Account?> FindByLoginAsync(string loginName, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.LoginName == loginName, token);
    }

    /// <summary>Creates an account. A null password creates an account without password login (e.g. the local lab account).</summary>
    public async Task<Account> CreateAsync(string loginName, string? password, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (loginName is null || !LoginPattern().IsMatch(loginName))
            throw new GameDataException(GameDataError.Invalid, "Login name must be 3–32 characters: letters, digits, _ . -");
        string hash = password is null ? PasswordHasher.Disabled : PasswordHasher.Hash(password);
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return await db.Accounts.AsNoTracking().FirstAsync(a => a.LoginName == loginName, token);
        if (await db.Accounts.AnyAsync(a => a.LoginName == loginName, token))
            throw new GameDataException(GameDataError.Duplicate, "Login name is already in use");
        Account account = new() { LoginName = loginName, PasswordHash = hash, CreatedAt = clock.GetUtcNow() };
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        db.Accounts.Add(account);
        await Audited.SaveAsync(db, token); // Assigns the identity used by the audit row.
        Audited.Record(db, context, "account.create", "account", account.Id.ToString(), null,
            new { account.LoginName, PasswordLogin = password is not null }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return account;
    }

    public async Task<Account> UpdateAsync(long id, uint expectedVersion, AccountChanges changes, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (changes.SlotCount is < 1 or > 17)
            throw new GameDataException(GameDataError.Invalid, "Slot count must be 1–17");
        if (changes.Note is { Length: > 2000 })
            throw new GameDataException(GameDataError.Invalid, "Note is limited to 2000 characters");
        if (changes.Tags is { } tags && (tags.Count > 20 || tags.Any(t => string.IsNullOrWhiteSpace(t) || t.Length > 32)))
            throw new GameDataException(GameDataError.Invalid, "Up to 20 tags of 1–32 characters");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        Account account = await Tracked(db, id, expectedVersion, token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return account;
        var before = new { account.IsGm, account.SlotCount, account.Note, Tags = account.Tags.ToArray() };
        if (changes.SlotCount is { } slots)
        {
            short highest = await db.Characters.Where(c => c.AccountId == id && c.DeletedAt == null)
                .Select(c => (short?)c.Slot).MaxAsync(token) ?? -1;
            if (slots <= highest)
                throw new GameDataException(GameDataError.Invalid, $"Slot {highest} is in use; move or delete that character first");
            account.SlotCount = slots;
        }
        if (changes.IsGm is { } gm) account.IsGm = gm;
        if (changes.Note is not null) account.Note = changes.Note.Length == 0 ? null : changes.Note;
        if (changes.Tags is not null) account.Tags = changes.Tags.Select(t => t.Trim()).Distinct().ToList();
        Audited.Record(db, context, "account.update", "account", id.ToString(), before,
            new { account.IsGm, account.SlotCount, account.Note, Tags = account.Tags.ToArray() }, clock);
        await Audited.SaveAsync(db, token);
        return account;
    }

    public async Task ResetPasswordAsync(long id, string newPassword, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        string hash = PasswordHasher.Hash(newPassword);
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return;
        Account account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == id, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Account not found");
        account.PasswordHash = hash;
        // Never store password material in the audit log.
        Audited.Record(db, context, "account.password_reset", "account", id.ToString(), null, null, clock);
        await Audited.SaveAsync(db, token);
    }

    /// <summary>For the login service: checks credentials and sanctions, and stamps LastLoginAt on success.</summary>
    public async Task<(LoginResult Result, Account? Account)> VerifyLoginAsync(string loginName, string password, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        Account? account = await db.Accounts.Include(a => a.Sanctions)
            .FirstOrDefaultAsync(a => a.LoginName == loginName, token);
        if (account is null)
        {
            PasswordHasher.Verify(password, DummyHash); // Equalise timing for unknown names.
            return (LoginResult.UnknownOrWrongPassword, null);
        }
        if (account.PasswordHash == PasswordHasher.Disabled) return (LoginResult.PasswordLoginDisabled, null);
        if (!PasswordHasher.Verify(password, account.PasswordHash)) return (LoginResult.UnknownOrWrongPassword, null);
        DateTimeOffset now = clock.GetUtcNow();
        AccountStatus status = SanctionService.StatusAt(account.Sanctions, now);
        if (status == AccountStatus.Banned) return (LoginResult.Banned, null);
        if (status == AccountStatus.Suspended) return (LoginResult.Suspended, null);
        account.LastLoginAt = now;
        await db.SaveChangesAsync(token);
        return (LoginResult.Ok, account);
    }

    private static readonly string DummyHash = PasswordHasher.Hash("timing-equaliser-only");

    internal static async Task<Account> Tracked(GameDbContext db, long id, uint expectedVersion, CancellationToken token)
    {
        Account account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == id, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Account not found");
        db.Entry(account).Property(a => a.Version).OriginalValue = expectedVersion;
        return account;
    }

    internal static string EscapeLike(string text) =>
        text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
