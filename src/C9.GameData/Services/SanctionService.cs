using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

public sealed class SanctionService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    public static AccountStatus StatusAt(IEnumerable<AccountSanction> sanctions, DateTimeOffset at)
    {
        AccountStatus status = AccountStatus.Active;
        foreach (AccountSanction sanction in sanctions.Where(s => s.IsActiveAt(at)))
        {
            if (sanction.Kind == SanctionKind.Ban) return AccountStatus.Banned;
            if (sanction.Kind == SanctionKind.Suspend) status = AccountStatus.Suspended;
        }
        return status;
    }

    public async Task<AccountSanction> IssueAsync(long accountId, SanctionKind kind, DateTimeOffset? endsAt,
        OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        DateTimeOffset now = clock.GetUtcNow();
        if (endsAt is { } end && end <= now)
            throw new GameDataException(GameDataError.Invalid, "End time must be in the future");
        if (kind == SanctionKind.Suspend && endsAt is null)
            throw new GameDataException(GameDataError.Invalid, "A suspension needs an end time; use a ban for permanent sanctions");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return await db.AccountSanctions.AsNoTracking().Where(s => s.AccountId == accountId)
                .OrderByDescending(s => s.Id).FirstAsync(token);
        Account account = await db.Accounts.Include(a => a.Sanctions).FirstOrDefaultAsync(a => a.Id == accountId, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Account not found");
        AccountSanction sanction = new()
        {
            AccountId = accountId, Kind = kind, Reason = context.Reason, StartsAt = now, EndsAt = endsAt, IssuedBy = context.Operator
        };
        account.Sanctions.Add(sanction);
        AccountStatus before = account.Status;
        account.Status = StatusAt(account.Sanctions, now);
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        await Audited.SaveAsync(db, token);
        Audited.Record(db, context, "account.sanction", "account", accountId.ToString(), new { Status = before },
            new { Status = account.Status, SanctionId = sanction.Id, Kind = kind, EndsAt = endsAt }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return sanction;
    }

    public async Task RevokeAsync(long accountId, long sanctionId, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        DateTimeOffset now = clock.GetUtcNow();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return;
        Account account = await db.Accounts.Include(a => a.Sanctions).FirstOrDefaultAsync(a => a.Id == accountId, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Account not found");
        AccountSanction sanction = account.Sanctions.FirstOrDefault(s => s.Id == sanctionId)
            ?? throw new GameDataException(GameDataError.NotFound, "Sanction not found");
        if (sanction.RevokedAt is not null) return;
        AccountStatus before = account.Status;
        sanction.RevokedAt = now;
        sanction.RevokedBy = context.Operator;
        account.Status = StatusAt(account.Sanctions, now);
        Audited.Record(db, context, "account.sanction_revoke", "account", accountId.ToString(),
            new { Status = before, SanctionId = sanctionId }, new { account.Status }, clock);
        await Audited.SaveAsync(db, token);
    }

    /// <summary>Recomputes cached statuses whose sanctions expired; run periodically by the Portal host.</summary>
    public async Task<int> RefreshExpiredAsync(CancellationToken token = default)
    {
        DateTimeOffset now = clock.GetUtcNow();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        List<Account> accounts = await db.Accounts.Include(a => a.Sanctions)
            .Where(a => a.Status != AccountStatus.Active).ToListAsync(token);
        int changed = 0;
        foreach (Account account in accounts)
        {
            AccountStatus status = StatusAt(account.Sanctions, now);
            if (status != account.Status) { account.Status = status; changed++; }
        }
        await db.SaveChangesAsync(token);
        return changed;
    }
}
