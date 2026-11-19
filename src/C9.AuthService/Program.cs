using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using C9.AuthService;
using C9.Contracts;
using C9.GameData;
using C9.GameData.Services;
using Microsoft.AspNetCore.RateLimiting;

// c9-auth: player accounts for the L9 launcher (register, login, one-time game tickets).
// Separate from the Admin Portal on purpose: players never reach operator endpoints (ISO/IEC 27001 A.8.22).
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string url = builder.Configuration["Auth:Url"] ?? $"http://127.0.0.1:{LauncherAuthRoutes.DefaultPort}";
builder.WebHost.UseUrls(url);
bool allowRegistration = builder.Configuration.GetValue("Auth:AllowRegistration", true);
string? internalTokenFile = builder.Configuration["Auth:InternalTokenFile"];
string? connection = GameDataConnection.ResolveWithEnvironmentPassword(
    builder.Configuration[GameDataServiceCollectionExtensions.ConnectionVariable] ?? GameDataConnection.FromEnvironment());
if (connection is not null) builder.Services.AddC9GameData(connection);
else builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<LauncherSessionStore>();
builder.Services.AddRateLimiter(limiter =>
{
    limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    limiter.OnRejected = async (context, token) =>
        await context.HttpContext.Response.WriteAsJsonAsync(
            new LauncherAuthError("rate_limited", "ลองหลายครั้งเกินไป กรุณารอ 1 นาทีแล้วลองใหม่"), token);
    // Password guessing: 10 attempts per minute per address (register + login share the budget).
    limiter.AddPolicy("credentials", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
    limiter.AddPolicy("tickets", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});

WebApplication app = builder.Build();
app.UseRateLimiter();
ILogger log = app.Logger;

app.MapGet("/healthz", () => Results.Text(connection is null ? "accounts-disabled" : "ok"));

app.MapPost(LauncherAuthRoutes.Register, async (LauncherCredentials body, HttpContext http, IServiceProvider services,
    LauncherSessionStore store, CancellationToken token) =>
{
    if (services.GetService<AccountService>() is not { } accounts) return Disabled();
    if (!allowRegistration) return Error(StatusCodes.Status403Forbidden, "registration_closed", "ปิดรับสมัครสมาชิกใหม่อยู่");
    if (Problem(body) is { } invalid) return invalid;
    try
    {
        PasswordHasher.ValidatePolicy(body.Password);
        var created = await accounts.CreateAsync(body.Login, body.Password,
            new OperatorContext("launcher", "self-registration from L9", Guid.NewGuid()), token);
        log.LogInformation("auth.register ok login={Login} account={Account} remote={Remote}", created.LoginName, created.Id, Remote(http));
        return Results.Json(store.CreateSession(created.LoginName, created.Id), statusCode: StatusCodes.Status201Created);
    }
    catch (GameDataException error) when (error.Code == GameDataError.Duplicate)
    {
        return Error(StatusCodes.Status409Conflict, "login_taken", "ชื่อผู้ใช้นี้ถูกใช้แล้ว");
    }
    catch (GameDataException error) when (error.Code == GameDataError.Invalid)
    {
        return Error(StatusCodes.Status400BadRequest, "invalid", error.Message.StartsWith("Password", StringComparison.Ordinal)
            ? "รหัสผ่านต้องยาว 8–128 ตัวอักษร" : "ชื่อผู้ใช้ต้องยาว 3–32 ตัว ใช้ได้เฉพาะ A–Z a–z 0–9 _ . -");
    }
}).RequireRateLimiting("credentials");

app.MapPost(LauncherAuthRoutes.Login, async (LauncherCredentials body, HttpContext http, IServiceProvider services,
    LauncherSessionStore store, CancellationToken token) =>
{
    if (services.GetService<AccountService>() is not { } accounts) return Disabled();
    if (Problem(body) is { } invalid) return invalid;
    (LoginResult result, var account) = await accounts.VerifyLoginAsync(body.Login, body.Password, token);
    log.LogInformation("auth.login {Result} login={Login} remote={Remote}", result, body.Login, Remote(http));
    return result switch
    {
        LoginResult.Ok when account is not null => Results.Json(store.CreateSession(account.LoginName, account.Id)),
        LoginResult.Banned => Error(StatusCodes.Status403Forbidden, "banned", "บัญชีนี้ถูกแบน"),
        LoginResult.Suspended => Error(StatusCodes.Status403Forbidden, "suspended", "บัญชีนี้ถูกระงับชั่วคราว"),
        // Unknown name, wrong password and accounts without password login look the same to the caller.
        _ => Error(StatusCodes.Status401Unauthorized, "invalid_credentials", "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง"),
    };
}).RequireRateLimiting("credentials");

app.MapPost(LauncherAuthRoutes.Ticket, (HttpContext http, LauncherSessionStore store) =>
{
    if (Bearer(http) is not { } session || store.IssueTicket(session) is not { } ticket)
        return Error(StatusCodes.Status401Unauthorized, "session_expired", "หมดเวลาเข้าสู่ระบบ กรุณาเข้าสู่ระบบใหม่");
    log.LogInformation("auth.ticket issued login={Login} account={Account}", ticket.Login, ticket.AccountId);
    return Results.Json(ticket);
}).RequireRateLimiting("tickets");

app.MapPost(LauncherAuthRoutes.Logout, (HttpContext http, LauncherSessionStore store) =>
{
    if (Bearer(http) is { } session) store.RevokeSession(session);
    return Results.NoContent();
});

// Called by the login service (C9.Server) when C9.exe presents a ticket. Protected by the shared agent token.
app.MapPost(LauncherAuthRoutes.ConsumeTicket, (TicketConsumeRequest body, HttpContext http, LauncherSessionStore store) =>
{
    if (!InternalCaller(http)) return Results.StatusCode(StatusCodes.Status401Unauthorized);
    if (store.ConsumeTicket(body.Ticket ?? "") is not { } owner)
    {
        log.LogWarning("auth.ticket rejected (unknown, reused or expired)");
        return Results.NotFound();
    }
    ulong handoff = store.CreateHandoff(owner);
    log.LogInformation("auth.ticket consumed login={Login} account={Account}", owner.Login, owner.AccountId);
    return Results.Json(new TicketRedemption(owner.Login, owner.AccountId, handoff.ToString("x16")));
});

app.MapPost(LauncherAuthRoutes.ResolveHandoff, (HandoffResolveRequest body, HttpContext http, LauncherSessionStore store) =>
{
    if (!InternalCaller(http)) return Results.StatusCode(StatusCodes.Status401Unauthorized);
    if (!ulong.TryParse(body.HandoffKey, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ulong key) ||
        store.ResolveHandoff(body.AccountId, key) is not { } owner)
    {
        log.LogWarning("auth.handoff rejected account={Account}", body.AccountId);
        return Results.NotFound();
    }
    log.LogInformation("auth.handoff resolved login={Login} account={Account}", owner.Login, owner.AccountId);
    return Results.Json(owner);
});

log.LogInformation("c9-auth on {Url}; game database {Database}; registration {Registration}",
    url, connection is null ? "not configured (accounts disabled)" : "configured", allowRegistration ? "open" : "closed");
app.Run();

IResult Disabled() => Error(StatusCodes.Status503ServiceUnavailable, "accounts_disabled",
    "ระบบบัญชียังไม่ได้เปิด (เริ่ม platform ด้วย -GameData)");

static IResult Error(int status, string code, string message) => Results.Json(new LauncherAuthError(code, message), statusCode: status);

static IResult? Problem(LauncherCredentials? body) =>
    body is null || string.IsNullOrWhiteSpace(body.Login) || string.IsNullOrEmpty(body.Password) || body.Login.Length > 64 || body.Password.Length > 128
        ? Error(StatusCodes.Status400BadRequest, "invalid", "กรุณากรอกชื่อผู้ใช้และรหัสผ่าน")
        : null;

static string? Bearer(HttpContext http) =>
    http.Request.Headers.Authorization.ToString() is { } header && header.StartsWith("Bearer ", StringComparison.Ordinal) && header.Length > 7
        ? header[7..].Trim() : null;

static string Remote(HttpContext http) => http.Connection.RemoteIpAddress?.ToString() ?? "unknown";

bool InternalCaller(HttpContext http)
{
    if (string.IsNullOrWhiteSpace(internalTokenFile) || !File.Exists(internalTokenFile) || Bearer(http) is not { } presented) return false;
    byte[] expected = Encoding.UTF8.GetBytes(File.ReadAllText(internalTokenFile).Trim());
    return expected.Length > 0 && CryptographicOperations.FixedTimeEquals(expected, Encoding.UTF8.GetBytes(presented));
}

