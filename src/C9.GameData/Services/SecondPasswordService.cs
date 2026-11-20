using System.Globalization;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

public enum SecondPasswordOutcome
{
    Ok,
    /// <summary>Wrong password; the account is not locked yet (see <see cref="SecondPasswordStatus.FailCount"/>).</summary>
    WrongPassword,
    /// <summary>Wrong security answer; the account is not locked yet.</summary>
    WrongAnswer,
    /// <summary>This attempt reached <see cref="SecondPasswordPolicy.MaxFailures"/> and locked the account.</summary>
    LockedNow,
    /// <summary>The account was already locked; nothing was checked.</summary>
    Locked,
    AlreadySet,
    NotSet
}

/// <summary>What the client needs to know about an account's secondary password (select-ack 0x0BC0 fields).</summary>
public sealed record SecondPasswordStatus(bool IsSet, short FailCount, DateTimeOffset? LockedAt)
{
    public static readonly SecondPasswordStatus None = new(false, 0, null);
    public bool IsLocked => LockedAt is not null;
}

public sealed record SecondPasswordResult(SecondPasswordOutcome Outcome, SecondPasswordStatus Status);

/// <summary>
/// Rules of the in-game secondary password, shared by <see cref="SecondPasswordService"/> and the lobby's in-memory lab store.
/// Every method works on the row in place and never sees a raw password: the client sends CRC32(UTF-16LE(password)).
/// Constants come from the current client (docs/EXIT_AND_2ND_PASSWORD_TH.md).
/// </summary>
public static class SecondPasswordPolicy
{
    /// <summary>Client "FailCntMax" (008D8902) and the select-ack lock threshold (008CBD06).</summary>
    public const int MaxFailures = 5;
    /// <summary>C9GAMESTR_SECONDARYPASSWORD_QUESTION_00..09.</summary>
    public const int Questions = 10;
    /// <summary>The answer is a wchar[25] in 0x2744 and 0x2746.</summary>
    public const int MaxAnswerLength = 25;
    /// <summary>The client shows the unlock time as lock start + 259,200 s (00B30052).</summary>
    public static readonly TimeSpan LockDuration = TimeSpan.FromHours(72);

    public static string PasswordSecret(uint crc) => crc.ToString("x8", CultureInfo.InvariantCulture);

    /// <summary>Answers compare case-insensitively and ignore surrounding spaces; null when the answer is not acceptable.</summary>
    public static string? AnswerSecret(string? answer)
    {
        string? trimmed = answer?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaxAnswerLength || trimmed.Any(char.IsControl)) return null;
        return trimmed.ToUpperInvariant();
    }

    public static bool ValidQuestion(int question) => question is >= 0 and < Questions;

    /// <summary>Ends a lock that is older than <see cref="LockDuration"/>; true when the row changed.</summary>
    public static bool Expire(AccountSecondPassword row, DateTimeOffset now)
    {
        if (row.LockedAt is not { } at || at + LockDuration > now) return false;
        row.LockedAt = null;
        row.FailCount = 0;
        row.UpdatedAt = now;
        return true;
    }

    public static SecondPasswordStatus StatusOf(AccountSecondPassword? row, DateTimeOffset now)
    {
        if (row is null) return SecondPasswordStatus.None;
        bool expired = row.LockedAt is { } at && at + LockDuration <= now;
        return expired ? new(true, 0, null) : new(true, row.FailCount, row.LockedAt);
    }

    public static AccountSecondPassword Create(long accountId, uint crc, byte question, string answer, DateTimeOffset now)
    {
        string secret = AnswerSecret(answer) ?? throw new GameDataException(GameDataError.Invalid, "Security answer must be 1–25 characters");
        if (!ValidQuestion(question)) throw new GameDataException(GameDataError.Invalid, "Unknown security question");
        return new AccountSecondPassword
        {
            AccountId = accountId, PasswordHash = PasswordHasher.HashSecret(PasswordSecret(crc)), Question = question,
            AnswerHash = PasswordHasher.HashSecret(secret), CreatedAt = now, UpdatedAt = now
        };
    }

    public static SecondPasswordOutcome Verify(AccountSecondPassword row, uint crc, DateTimeOffset now)
    {
        Expire(row, now);
        if (row.LockedAt is not null) return SecondPasswordOutcome.Locked;
        if (!PasswordHasher.Verify(PasswordSecret(crc), row.PasswordHash)) return Fail(row, now, SecondPasswordOutcome.WrongPassword);
        Succeed(row, now);
        return SecondPasswordOutcome.Ok;
    }

    public static SecondPasswordOutcome Change(AccountSecondPassword row, uint current, uint next, DateTimeOffset now)
    {
        SecondPasswordOutcome outcome = Verify(row, current, now);
        if (outcome != SecondPasswordOutcome.Ok) return outcome;
        row.PasswordHash = PasswordHasher.HashSecret(PasswordSecret(next));
        return SecondPasswordOutcome.Ok;
    }

    /// <summary>Checks the security answer of a reset; the caller removes the password on <see cref="SecondPasswordOutcome.Ok"/>.</summary>
    public static SecondPasswordOutcome CheckAnswer(AccountSecondPassword row, byte question, string? answer, DateTimeOffset now)
    {
        Expire(row, now);
        if (row.LockedAt is not null) return SecondPasswordOutcome.Locked;
        string? secret = AnswerSecret(answer);
        if (secret is null || question != row.Question || !PasswordHasher.Verify(secret, row.AnswerHash))
            return Fail(row, now, SecondPasswordOutcome.WrongAnswer);
        Succeed(row, now);
        return SecondPasswordOutcome.Ok;
    }

    private static SecondPasswordOutcome Fail(AccountSecondPassword row, DateTimeOffset now, SecondPasswordOutcome wrong)
    {
        row.FailCount = (short)Math.Min(row.FailCount + 1, MaxFailures);
        row.UpdatedAt = now;
        if (row.FailCount < MaxFailures) return wrong;
        row.LockedAt = now;
        return SecondPasswordOutcome.LockedNow;
    }

    private static void Succeed(AccountSecondPassword row, DateTimeOffset now)
    {
        if (row.FailCount == 0) return;
        row.FailCount = 0;
        row.UpdatedAt = now;
    }
}

/// <summary>
/// Secondary passwords in game.account_second_passwords. Setting, changing, resetting, clearing and every lock are
/// written to the audit log (ISO/IEC 27001 A.8.15); the audit rows never contain hashes or answers.
/// Writes are optimistic (xmin) and retried a few times when the game server and the Portal race.
/// </summary>
public sealed class SecondPasswordService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    private const int Attempts = 3;

    public async Task<SecondPasswordStatus> StatusAsync(long accountId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        AccountSecondPassword? row = await db.AccountSecondPasswords.AsNoTracking().FirstOrDefaultAsync(p => p.AccountId == accountId, token);
        return SecondPasswordPolicy.StatusOf(row, clock.GetUtcNow());
    }

    public Task<SecondPasswordResult> SetAsync(long accountId, uint crc, byte question, string answer, OperatorContext context,
        CancellationToken token = default)
    {
        context.Validate();
        return WriteAsync(accountId, context, token, (db, row, now) =>
        {
            if (row is not null) return (SecondPasswordOutcome.AlreadySet, row, null);
            AccountSecondPassword created = SecondPasswordPolicy.Create(accountId, crc, question, answer, now);
            db.AccountSecondPasswords.Add(created);
            return (SecondPasswordOutcome.Ok, created, "account.second_password.set");
        });
    }

    /// <summary>Checks the password (certify); wrong attempts count towards the lock.</summary>
    public Task<SecondPasswordResult> VerifyAsync(long accountId, uint crc, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        return WriteAsync(accountId, context, token, (_, row, now) =>
            row is null ? (SecondPasswordOutcome.NotSet, null, null) : (SecondPasswordPolicy.Verify(row, crc, now), row, null));
    }

    public Task<SecondPasswordResult> ChangeAsync(long accountId, uint current, uint next, OperatorContext context,
        CancellationToken token = default)
    {
        context.Validate();
        return WriteAsync(accountId, context, token, (_, row, now) =>
        {
            if (row is null) return (SecondPasswordOutcome.NotSet, null, null);
            SecondPasswordOutcome outcome = SecondPasswordPolicy.Change(row, current, next, now);
            return (outcome, row, outcome == SecondPasswordOutcome.Ok ? "account.second_password.change" : null);
        });
    }

    /// <summary>Removes the password when the security answer matches (client "forgot password").</summary>
    public Task<SecondPasswordResult> ResetAsync(long accountId, byte question, string? answer, OperatorContext context,
        CancellationToken token = default)
    {
        context.Validate();
        return WriteAsync(accountId, context, token, (db, row, now) =>
        {
            if (row is null) return (SecondPasswordOutcome.NotSet, null, null);
            SecondPasswordOutcome outcome = SecondPasswordPolicy.CheckAnswer(row, question, answer, now);
            if (outcome != SecondPasswordOutcome.Ok) return (outcome, row, null);
            db.AccountSecondPasswords.Remove(row);
            return (outcome, null, "account.second_password.reset");
        });
    }

    /// <summary>Operator removal (e.g. a GM unlocking a player who forgot both password and answer).</summary>
    public Task<SecondPasswordResult> ClearAsync(long accountId, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        return WriteAsync(accountId, context, token, (db, row, _) =>
        {
            if (row is null) return (SecondPasswordOutcome.NotSet, null, null);
            db.AccountSecondPasswords.Remove(row);
            return (SecondPasswordOutcome.Ok, null, "account.second_password.clear");
        });
    }

    private async Task<SecondPasswordResult> WriteAsync(long accountId, OperatorContext context, CancellationToken token,
        Func<GameDbContext, AccountSecondPassword?, DateTimeOffset, (SecondPasswordOutcome Outcome, AccountSecondPassword? Row, string? Audit)> apply)
    {
        for (int attempt = 1; ; attempt++)
        {
            await using GameDbContext db = await factory.CreateDbContextAsync(token);
            AccountSecondPassword? row = await db.AccountSecondPasswords.FirstOrDefaultAsync(p => p.AccountId == accountId, token);
            DateTimeOffset now = clock.GetUtcNow();
            SecondPasswordStatus before = SecondPasswordPolicy.StatusOf(row, now);
            var (outcome, after, audit) = apply(db, row, now);
            SecondPasswordStatus status = SecondPasswordPolicy.StatusOf(after, now);
            audit ??= outcome == SecondPasswordOutcome.LockedNow ? "account.second_password.lock" : null;
            if (audit is not null)
                Audited.Record(db, context with { RequestId = Guid.NewGuid() }, audit, "account", accountId.ToString(CultureInfo.InvariantCulture),
                    Summary(before), Summary(status), clock);
            if (!db.ChangeTracker.HasChanges()) return new(outcome, status);
            try
            {
                await Audited.SaveAsync(db, token);
                return new(outcome, status);
            }
            catch (GameDataException error) when (error.Code is GameDataError.Conflict or GameDataError.Duplicate && attempt < Attempts)
            {
                // Another session wrote the same row first (or created it): reload and decide again.
            }
        }
    }

    private static object Summary(SecondPasswordStatus status) => new { status.IsSet, status.FailCount, status.LockedAt };
}
