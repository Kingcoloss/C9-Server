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

/// <summary>Error body: <see cref="Code"/> is stable for programs, <see cref="Message"/> is Thai text for people.</summary>
public sealed record LauncherAuthError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message);
