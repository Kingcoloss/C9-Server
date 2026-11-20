using System.Text.Json.Serialization;

namespace C9.Contracts;

/// <summary>
/// Player-facing account API used by the L9 launcher (service c9-auth, docs/LAUNCHER_ACCOUNTS_TH.md).
/// Passwords travel only in <see cref="LauncherCredentials"/> to c9-auth and are never stored by the launcher.
/// </summary>
public static class LauncherAuthRoutes
{
    public const string Register = "/launcher/v1/register";
    public const string Login = "/launcher/v1/login";
    public const string Ticket = "/launcher/v1/ticket";
    public const string Logout = "/launcher/v1/logout";
    public const string ConsumeTicket = "/internal/v1/tickets/consume";
    public const string ResolveHandoff = "/internal/v1/handoffs/resolve";
    /// <summary>GET: stat sheets of the signed-in account's characters.</summary>
    public const string CharacterStats = "/launcher/v1/characters/stats";
    /// <summary>POST <see cref="LauncherStatSpend"/>: spend stat upgrade points of one character.</summary>
    public const string SpendStatsPattern = "/launcher/v1/characters/{id:long}/stats";
    public static string SpendStats(long characterId) => $"/launcher/v1/characters/{characterId}/stats";
    public const int DefaultPort = 14890;
    public const int TicketBytes = 16;
}

public sealed record LauncherCredentials(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("password")] string Password);

public sealed record LauncherSession(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("account_id")] long AccountId,
    [property: JsonPropertyName("session_token")] string SessionToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>One-time game ticket handed to C9.exe; <see cref="Ticket"/> is base64 of 16 random bytes.</summary>
public sealed record LauncherTicket(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("account_id")] long AccountId,
    [property: JsonPropertyName("ticket")] string Ticket,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

public sealed record TicketConsumeRequest([property: JsonPropertyName("ticket")] string Ticket);

public sealed record TicketOwner(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("account_id")] long AccountId);

/// <summary>
/// Result of redeeming a ticket (c9-login). <see cref="HandoffKey"/> (16 hex digits) is the 64-bit key the login
/// server puts in 0x2EE2; the client echoes it with the account number in the lobby's 0x1771, where c9-lobby
/// resolves it again through <see cref="LauncherAuthRoutes.ResolveHandoff"/>.
/// </summary>
public sealed record TicketRedemption(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("account_id")] long AccountId,
    [property: JsonPropertyName("handoff_key")] string HandoffKey);

public sealed record HandoffResolveRequest(
    [property: JsonPropertyName("account_id")] long AccountId,
    [property: JsonPropertyName("handoff_key")] string HandoffKey);

/// <summary>One stat of a character: class base + level bonus (per-level growth × (level − 1)) + spent points × per-point gain = total.</summary>
public sealed record LauncherStatLine(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("upgradable")] bool Upgradable,
    [property: JsonPropertyName("per_point")] int PerPoint,
    [property: JsonPropertyName("base")] int Base,
    [property: JsonPropertyName("points")] int Points,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("level_bonus")] int LevelBonus = 0);

public sealed record LauncherCharacterStats(
    [property: JsonPropertyName("character_id")] long CharacterId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("class_name")] string ClassName,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("earned_points")] int EarnedPoints,
    [property: JsonPropertyName("spent_points")] int SpentPoints,
    [property: JsonPropertyName("available_points")] int AvailablePoints,
    [property: JsonPropertyName("stats")] IReadOnlyList<LauncherStatLine> Stats);

public sealed record LauncherStatSheets(
    [property: JsonPropertyName("characters")] IReadOnlyList<LauncherCharacterStats> Characters);

/// <summary>
/// Points to add per stat key. <see cref="RequestId"/> makes a retry (for example after a timeout) apply only once;
/// changes take effect the next time the character enters the world.
/// </summary>
public sealed record LauncherStatSpend(
    [property: JsonPropertyName("request_id")] Guid RequestId,
    [property: JsonPropertyName("add")] IReadOnlyDictionary<string, int> Add);

/// <summary>Error body: <see cref="Code"/> is stable for programs, <see cref="Message"/> is Thai text for people.</summary>
public sealed record LauncherAuthError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message);
