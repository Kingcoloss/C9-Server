using System.Buffers.Binary;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using C9.Contracts;
using C9.Protocol;

namespace C9.Server;

/// <summary>A redeemed launcher ticket: who logged in to L9 and the handoff key for the lobby.</summary>
public sealed record LauncherHandoff(string Login, long AccountId, ulong Key);

/// <summary>c9-auth as seen by the game services (docs/LAUNCHER_ACCOUNTS_TH.md).</summary>
public interface ILauncherAuthority
{
    /// <summary>Redeems a one-time ticket; null when c9-auth does not know it (unknown, reused or expired).</summary>
    ValueTask<LauncherHandoff?> RedeemTicketAsync(string ticket, CancellationToken token);

    /// <summary>Resolves the handoff the client echoes to the lobby; null when it is unknown or expired.</summary>
    ValueTask<TicketOwner?> ResolveHandoffAsync(long accountId, ulong key, CancellationToken token);
}

/// <summary>
/// Layouts that carry the launcher identity, verified with the official build on 2026-11-16 (trace login-ced44f-7,
/// lobby-c53478-7) and the 2EE2/2EED/1771 readers (client 00A621F0; packet+0x0D is payload offset 0).
/// </summary>
public static class LauncherLoginPacket
{
    public const int PayloadBytes = 1092;
    /// <summary>UTF-16LE, NUL terminated: the ticket exactly as L9 put it in launch field 3 (base64, %-escaped).</summary>
    public const int TicketOffset = 0x000;
    /// <summary>UTF-16LE, NUL terminated: "{service}:{13-digit account}" from launch fields 4 and 5.</summary>
    public const int AccountOffset = 0x200;
    public const int FieldBytes = 0x200;
    public const int LoginReplyBytes = 20;
    public const int LobbyRequestBytes = 49;
    public const int ReturnRequestBytes = 28;

    /// <summary>Reads ticket (canonical base64 of 16 bytes) and account number from a 0x2F04 payload.</summary>
    public static bool TryParse(ReadOnlySpan<byte> payload, out string ticket, out long accountId)
    {
        ticket = "";
        accountId = 0;
        if (payload.Length != PayloadBytes) return false;
        string? escaped = Field(payload.Slice(TicketOffset, FieldBytes));
        string? account = Field(payload.Slice(AccountOffset, FieldBytes));
        if (escaped is null || account is null) return false;
        byte[] raw;
        try { raw = Convert.FromBase64String(Uri.UnescapeDataString(escaped)); }
        catch (Exception error) when (error is FormatException or UriFormatException) { return false; }
        if (raw.Length != LauncherAuthRoutes.TicketBytes || raw.AsSpan().IndexOfAnyExcept((byte)0) < 0) return false;
        int separator = account.LastIndexOf(':');
        string digits = account[(separator + 1)..];
        if (digits.Length is 0 or > 13 || !digits.All(char.IsAsciiDigit) ||
            !long.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out accountId) || accountId <= 0)
            return false;
        ticket = Convert.ToBase64String(raw);
        return true;
    }

    /// <summary>0x2EE2 payload: account number (u64), handoff key (u64), then the observed constant 1 (u32).</summary>
    public static byte[] LoginReply(long accountId, ulong key)
    {
        byte[] payload = new byte[LoginReplyBytes];
        BinaryPrimitives.WriteInt64LittleEndian(payload, accountId);
        BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(8), key);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(16), 1);
        return payload;
    }

    /// <summary>The client echoes the 2EE2 account number and key at the start of the lobby's 0x1771.</summary>
    public static bool TryReadLobbyIdentity(ReadOnlySpan<byte> payload, out long accountId, out ulong key)
    {
        accountId = payload.Length == LobbyRequestBytes ? BinaryPrimitives.ReadInt64LittleEndian(payload) : 0;
        key = payload.Length == LobbyRequestBytes ? BinaryPrimitives.ReadUInt64LittleEndian(payload[8..]) : 0;
        return accountId > 0 && key != 0;
    }

    /// <summary>
    /// 0x2EE8 (return to character select, login reconnect after 0x2EEB): the client copies the 2EE2 block back in
    /// stored order, key first: [key u64][account u64][12 zero bytes]. Verified 2026-11-16 (client 008e6fb3 stores
    /// 2EE2 account at +8 and key at +0; trace login-ced44f-8 with account 1, key 0).
    /// </summary>
    public static bool TryReadReturnIdentity(ReadOnlySpan<byte> payload, out long accountId, out ulong key)
    {
        key = payload.Length == ReturnRequestBytes ? BinaryPrimitives.ReadUInt64LittleEndian(payload) : 0;
        accountId = payload.Length == ReturnRequestBytes ? BinaryPrimitives.ReadInt64LittleEndian(payload[8..]) : 0;
        return accountId > 0 && key != 0;
    }

    private static string? Field(ReadOnlySpan<byte> field)
    {
        int end = 0;
        while (end + 1 < field.Length && (field[end] | field[end + 1]) != 0) end += 2;
        if (end == 0 || end + 1 >= field.Length) return null;
        string text = Encoding.Unicode.GetString(field[..end]);
        return text.All(c => c is > ' ' and < (char)0x7F) ? text : null;
    }
}

/// <summary>Thrown when c9-auth cannot be reached; the session is closed instead of letting the player in.</summary>
public sealed class LauncherAuthUnavailableException(string message) : ProtocolException(ProtocolError.InvalidPacket, message);

/// <summary>HTTP client for the internal c9-auth endpoints (Bearer = the shared agent token).</summary>
public sealed class HttpLauncherAuthority : ILauncherAuthority, IDisposable
{
    private readonly HttpClient http;
    private readonly string tokenFile;

    public HttpLauncherAuthority(Uri baseUrl, string tokenFile, HttpMessageHandler? handler = null)
    {
        this.tokenFile = tokenFile;
        http = handler is null ? new HttpClient() : new HttpClient(handler, disposeHandler: false);
        http.BaseAddress = baseUrl;
        http.Timeout = TimeSpan.FromSeconds(5);
    }

    public static ILauncherAuthority? FromOptions(ServerOptions options) =>
        options.LauncherAuth is { } url && options.LauncherAuthTokenFile is { } file ? new HttpLauncherAuthority(url, file) : null;

    public async ValueTask<LauncherHandoff?> RedeemTicketAsync(string ticket, CancellationToken token)
    {
        TicketRedemption? result = await PostAsync<TicketRedemption>(LauncherAuthRoutes.ConsumeTicket, new TicketConsumeRequest(ticket), token);
        if (result is null) return null;
        if (!ulong.TryParse(result.HandoffKey, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ulong key) || key == 0)
            throw new LauncherAuthUnavailableException("c9-auth returned an invalid handoff key");
        return new(result.Login, result.AccountId, key);
    }

    public async ValueTask<TicketOwner?> ResolveHandoffAsync(long accountId, ulong key, CancellationToken token) =>
        await PostAsync<TicketOwner>(LauncherAuthRoutes.ResolveHandoff, new HandoffResolveRequest(accountId, key.ToString("x16")), token);

    private async Task<T?> PostAsync<T>(string route, object body, CancellationToken token) where T : class
    {
        try
        {
            using HttpRequestMessage request = new(HttpMethod.Post, route) { Content = JsonContent.Create(body) };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", (await File.ReadAllTextAsync(tokenFile, token)).Trim());
            using HttpResponseMessage response = await http.SendAsync(request, token);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            if (!response.IsSuccessStatusCode)
                throw new LauncherAuthUnavailableException($"c9-auth answered HTTP {(int)response.StatusCode}");
            return await response.Content.ReadFromJsonAsync<T>(token)
                ?? throw new LauncherAuthUnavailableException("c9-auth returned an empty body");
        }
        catch (Exception error) when (error is HttpRequestException or IOException or System.Text.Json.JsonException ||
            (error is TaskCanceledException && !token.IsCancellationRequested))
        {
            throw new LauncherAuthUnavailableException("c9-auth is unreachable: " + error.GetType().Name);
        }
    }

    public void Dispose() => http.Dispose();
}
