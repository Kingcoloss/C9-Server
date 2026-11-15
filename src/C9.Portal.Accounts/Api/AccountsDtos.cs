using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Portal.GameData;

namespace C9.Portal.Accounts.Api;

// Response DTOs never expose password hashes. Version is the PostgreSQL xmin to send back on edits.

public sealed record AccountDto(long Id, string LoginName, AccountStatus Status, bool IsGm, short SlotCount, string? Note,
    IReadOnlyList<string> Tags, DateTimeOffset CreatedAt, DateTimeOffset? LastLoginAt, bool PasswordLogin, uint Version)
{
    public static AccountDto From(Account a) => new(a.Id, a.LoginName, a.Status, a.IsGm, a.SlotCount, a.Note, a.Tags,
        a.CreatedAt, a.LastLoginAt, a.PasswordHash != PasswordHasher.Disabled, a.Version);
}

public sealed record SanctionDto(long Id, SanctionKind Kind, string Reason, DateTimeOffset StartsAt, DateTimeOffset? EndsAt,
    string IssuedBy, DateTimeOffset? RevokedAt, string? RevokedBy, bool Active)
{
    public static SanctionDto From(AccountSanction s, DateTimeOffset now) =>
        new(s.Id, s.Kind, s.Reason, s.StartsAt, s.EndsAt, s.IssuedBy, s.RevokedAt, s.RevokedBy, s.IsActiveAt(now));
}

public sealed record CharacterDto(long Id, long AccountId, short Slot, string Name, short Job, string Appearance, short Level,
    long Exp, long Gold, int? MapId, short? Submap, float? PosX, float? PosY, float? PosZ, long PlaySeconds,
    DateTimeOffset CreatedAt, DateTimeOffset? LastPlayedAt, DateTimeOffset? DeletedAt, uint Version)
{
    public static CharacterDto From(Character c) => new(c.Id, c.AccountId, c.Slot, c.Name, c.Job, Convert.ToBase64String(c.Appearance),
        c.Level, c.Exp, c.Gold, c.MapId, c.Submap, c.PosX, c.PosY, c.PosZ, c.PlaySeconds, c.CreatedAt, c.LastPlayedAt, c.DeletedAt, c.Version);
}

public sealed record ItemDto(Guid Serial, ItemLocation Location, short Slot, int ItemId, int Quantity, short Enchant, string Options,
    bool Bound, DateTimeOffset? ExpiresAt);

public sealed record CharacterDetailDto(CharacterDto Character, IReadOnlyList<ItemDto> Items);

public sealed record AccountDetailDto(AccountDto Account, IReadOnlyList<CharacterDto> Characters, IReadOnlyList<SanctionDto> Sanctions);

public sealed record TimelineEntry(DateTimeOffset At, string Operator, string Action, string? TargetType, string? TargetId,
    string Reason, string? Before, string? After);

public sealed record SessionHistoryDto(string Key, string Service, long? CharacterId, string? RemoteAddress, DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt, string? EndReason, string? LastState, int? MapId);

// Requests. Every write carries a reason (audit) and an Idempotency-Key header.

public sealed record CreateAccountRequest(string? LoginName, string? Password, string? Reason) : IWriteRequest;
public sealed record UpdateAccountRequest(uint Version, bool? IsGm, short? SlotCount, string? Note, IReadOnlyList<string>? Tags, string? Reason) : IWriteRequest;
public sealed record PasswordRequest(string? Password, string? Reason) : IWriteRequest;
public sealed record SanctionRequest(SanctionKind Kind, DateTimeOffset? EndsAt, string? Reason) : IWriteRequest;
public sealed record ReasonRequest(string? Reason) : IWriteRequest;
public sealed record VersionedRequest(uint Version, string? Reason) : IWriteRequest;
public sealed record RenameRequest(uint Version, string? Name, string? Reason) : IWriteRequest;
public sealed record MoveRequest(uint Version, short Slot, string? Reason) : IWriteRequest;
public sealed record UpdateCharacterRequest(uint Version, short? Level, int? MapId, short? Submap, float? PosX, float? PosY, float? PosZ,
    string? Appearance, string? Reason) : IWriteRequest;
public sealed record ImportCharacterRequest(short Slot, CharacterExport? Data, string? Reason) : IWriteRequest;
public sealed record AccountRatesRequest(RateSet? Rates, DateTimeOffset? ExpiresAt, string? Reason) : IWriteRequest;
/// <summary>Legacy CharacterLobby JSON content (not a path: the portal never reads arbitrary server files).</summary>
public sealed record LegacyImportRequest(System.Text.Json.JsonElement Characters, string? Reason) : IWriteRequest;
