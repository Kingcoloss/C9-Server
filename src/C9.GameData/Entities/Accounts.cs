namespace C9.GameData.Entities;

public enum AccountStatus { Active, Suspended, Banned }

public enum SanctionKind { Ban, Suspend, Mute }

public sealed class Account
{
    public long Id { get; set; }
    public required string LoginName { get; set; }
    /// <summary>PHC-style PBKDF2 string from <see cref="Services.PasswordHasher"/>; "!" disables password login.</summary>
    public required string PasswordHash { get; set; }
    /// <summary>Cached result of active sanctions; recomputed by <see cref="Services.SanctionService"/>.</summary>
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public short SlotCount { get; set; } = 17;
    public bool IsGm { get; set; }
    public string? Note { get; set; }
    public List<string> Tags { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    /// <summary>PostgreSQL xmin; changes on every write by any process.</summary>
    public uint Version { get; set; }

    public List<Character> Characters { get; set; } = [];
    public List<AccountSanction> Sanctions { get; set; } = [];
}

public sealed class AccountSanction
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public SanctionKind Kind { get; set; }
    public required string Reason { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    /// <summary>Null means permanent.</summary>
    public DateTimeOffset? EndsAt { get; set; }
    public required string IssuedBy { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? RevokedBy { get; set; }

    public bool IsActiveAt(DateTimeOffset at) =>
        RevokedAt is null && StartsAt <= at && (EndsAt is null || EndsAt > at);
}

/// <summary>
/// In-game secondary password of an account (client panel C9UISecondaryPasswordPanel, docs/EXIT_AND_2ND_PASSWORD_TH.md).
/// The client never sends the password itself, only CRC32(UTF-16LE(password)); that value and the security answer are
/// stored as PBKDF2 strings from <see cref="Services.PasswordHasher.HashSecret"/>, never raw.
/// </summary>
public sealed class AccountSecondPassword
{
    public long AccountId { get; set; }
    public required string PasswordHash { get; set; }
    /// <summary>Security question index shown by the client (C9GAMESTR_SECONDARYPASSWORD_QUESTION_00..09).</summary>
    public short Question { get; set; }
    public required string AnswerHash { get; set; }
    /// <summary>Wrong passwords or answers since the last success; the account locks at <see cref="Services.SecondPasswordPolicy.MaxFailures"/>.</summary>
    public short FailCount { get; set; }
    /// <summary>Start of the current lock; the client shows the unlock time as this + 72 hours.</summary>
    public DateTimeOffset? LockedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    /// <summary>PostgreSQL xmin.</summary>
    public uint Version { get; set; }
}
