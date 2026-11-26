using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using C9.Contracts;
using static C9.LocalLauncher.LauncherText;

namespace C9.LocalLauncher;

public sealed record AuthResult<T>(T? Value, string? ErrorCode, string? ErrorMessage) where T : class
{
    public bool Ok => Value is not null;
    public static AuthResult<T> Success(T value) => new(value, null, null);
    public static AuthResult<T> Failure(string code, string message) => new(null, code, message);
}

/// <summary>Identity passed to C9.exe in the launch argument (login name, account number, one-time ticket).</summary>
public sealed record LaunchIdentity(string Login, long AccountId, byte[] Ticket)
{
    public static LaunchIdentity From(LauncherTicket ticket) =>
        new(ticket.Login, ticket.AccountId, Convert.FromBase64String(ticket.Ticket));
}

/// <summary>
/// Client for the c9-auth player account service. The password is sent once per login/register request and
/// never stored; plain HTTP is allowed only to a loopback address so credentials never cross a network unencrypted.
/// </summary>
public sealed class LauncherAuthClient : IDisposable
{
    public const string DefaultUrl = "http://127.0.0.1:14890";
    private readonly HttpClient http;

    public LauncherAuthClient(string baseUrl, HttpMessageHandler? handler = null)
    {
        BaseUrl = ValidateUrl(baseUrl);
        http = handler is null ? new HttpClient() : new HttpClient(handler, disposeHandler: false);
        http.BaseAddress = BaseUrl;
        http.Timeout = TimeSpan.FromSeconds(10);
    }

    public Uri BaseUrl { get; }

    /// <summary>Throws <see cref="ArgumentException"/> (Thai message) for URLs that would expose passwords.</summary>
    public static Uri ValidateUrl(string? value)
    {
        if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out Uri? url) || url.Scheme is not ("http" or "https") ||
            !string.IsNullOrEmpty(url.UserInfo) || url.PathAndQuery != "/")
            throw new ArgumentException(T("URL เซิร์ฟเวอร์บัญชีไม่ถูกต้อง ตัวอย่าง: http://127.0.0.1:14890"));
        bool loopback = url.IsLoopback || (IPAddress.TryParse(url.Host, out IPAddress? address) && IPAddress.IsLoopback(address));
        if (url.Scheme == "http" && !loopback)
            throw new ArgumentException(T("เซิร์ฟเวอร์บัญชีที่อยู่นอกเครื่องต้องใช้ https เพื่อไม่ให้รหัสผ่านถูกส่งแบบไม่เข้ารหัส"));
        return url;
    }

    public Task<AuthResult<LauncherSession>> LoginAsync(string login, string password, CancellationToken token = default) =>
        SendAsync<LauncherSession>(LauncherAuthRoutes.Login, new LauncherCredentials(login.Trim(), password), null, token);

    public Task<AuthResult<LauncherSession>> RegisterAsync(string login, string password, CancellationToken token = default) =>
        SendAsync<LauncherSession>(LauncherAuthRoutes.Register, new LauncherCredentials(login.Trim(), password), null, token);

    public Task<AuthResult<LauncherTicket>> TicketAsync(string sessionToken, CancellationToken token = default) =>
        SendAsync<LauncherTicket>(LauncherAuthRoutes.Ticket, null, sessionToken, token);

    public async Task LogoutAsync(string sessionToken, CancellationToken token = default) =>
        await SendAsync<LauncherAuthError>(LauncherAuthRoutes.Logout, null, sessionToken, token);

    public Task<AuthResult<LauncherStatSheets>> StatsAsync(string sessionToken, CancellationToken token = default) =>
        SendAsync<LauncherStatSheets>(LauncherAuthRoutes.CharacterStats, null, sessionToken, token, HttpMethod.Get);

    /// <summary>Spends points; reuse the same <see cref="LauncherStatSpend.RequestId"/> when retrying one click.</summary>
    public Task<AuthResult<LauncherCharacterStats>> SpendStatsAsync(string sessionToken, long characterId, LauncherStatSpend spend,
        CancellationToken token = default) =>
        SendAsync<LauncherCharacterStats>(LauncherAuthRoutes.SpendStats(characterId), spend, sessionToken, token);

    private async Task<AuthResult<T>> SendAsync<T>(string route, object? body, string? bearer, CancellationToken token,
        HttpMethod? method = null) where T : class
    {
        using HttpRequestMessage request = new(method ?? HttpMethod.Post, route)
            { Content = body is null ? null : JsonContent.Create(body) };
        if (bearer is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        try
        {
            using HttpResponseMessage response = await http.SendAsync(request, token);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return AuthResult<T>.Failure("no_content", "");
                T? value = await response.Content.ReadFromJsonAsync<T>(token);
                return value is null ? AuthResult<T>.Failure("invalid_response", LauncherText.T("เซิร์ฟเวอร์บัญชีตอบกลับไม่ถูกต้อง")) : AuthResult<T>.Success(value);
            }
            LauncherAuthError? error = null;
            try { error = await response.Content.ReadFromJsonAsync<LauncherAuthError>(token); }
            catch (Exception parse) when (parse is JsonException or NotSupportedException) { }
            return AuthResult<T>.Failure(error?.Code ?? $"http_{(int)response.StatusCode}",
                error?.Message is { } message ? LauncherText.T(message) : F("เซิร์ฟเวอร์บัญชีตอบกลับ HTTP {0}", (int)response.StatusCode));
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or JsonException)
        {
            return AuthResult<T>.Failure("unreachable",
                F("ติดต่อเซิร์ฟเวอร์บัญชีไม่ได้ ({0}) ตรวจว่าเริ่ม platform ด้วย -GameData แล้ว", BaseUrl.GetLeftPart(UriPartial.Authority)));
        }
    }

    public void Dispose() => http.Dispose();
}
