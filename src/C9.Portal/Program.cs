using System.Security.Claims;
using System.Text.Encodings.Web;
using C9.Contracts;
using C9.Portal;
using C9.Portal.Api;
using C9.Portal.Components;
using C9.Portal.Services;
using C9.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
PortalOptions options = builder.Configuration.GetSection("Portal").Get<PortalOptions>() ?? new PortalOptions();
options.Resolve(builder.Environment.ContentRootPath);
Directory.CreateDirectory(options.StateDirectory);
builder.WebHost.UseUrls(options.Url);
builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.AddServerHeader = false;
    kestrel.Limits.MaxRequestBodySize = 256 * 1024;
});

ModuleCatalog modules = ModuleCatalog.Discover();
AgentToken agentToken = AgentToken.LoadOrCreate(options.AgentTokenFile, TimeSpan.FromSeconds(30));
GameServices gameServices = new(options.Services.Select(s =>
    new AgentClient(new GameServiceEndpoint(s.Id, string.IsNullOrWhiteSpace(s.Name) ? s.Id : s.Name, new Uri(s.AgentUrl), s.PublicPort, s.Description), agentToken.Value)));

// Cookie/antiforgery keys survive restarts (container recreation) so sign-ins stay valid.
builder.Services.AddDataProtection()
    .SetApplicationName("C9.Portal")
    .PersistKeysToFileSystem(Directory.CreateDirectory(Path.Combine(options.StateDirectory, "keys")));
builder.Services.AddSingleton(options);
builder.Services.AddSingleton(modules);
builder.Services.AddSingleton(gameServices);
builder.Services.AddSingleton<IGameServices>(gameServices);
builder.Services.AddSingleton<PortalBus>();
builder.Services.AddSingleton<ISessionFeed>(services => services.GetRequiredService<PortalBus>());
builder.Services.AddHostedService<LiveFeedService>();
builder.Services.AddSingleton<IAuditLog>(new JsonlAuditLog(Path.Combine(options.StateDirectory, "audit")));
OperatorKeyUserStore operatorKeys = new(options.StateDirectory);
builder.Services.AddSingleton(operatorKeys);
builder.Services.AddSingleton<IPortalUserStore>(operatorKeys);
builder.Services.AddSingleton<ParametersService>();
builder.Services.AddSingleton<PortalActions>();
builder.Services.AddSingleton<PortalClock>();
builder.Services.AddSingleton<SignInThrottle>();

const string Smart = "c9-smart";
builder.Services.AddAuthentication(Smart)
    .AddPolicyScheme(Smart, "Cookie or bearer", scheme => scheme.ForwardDefaultSelector = context =>
        context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.Ordinal)
            ? BearerKeyHandler.SchemeName : CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookie =>
    {
        cookie.LoginPath = "/signin";
        cookie.LogoutPath = "/signout";
        cookie.AccessDeniedPath = "/denied";
        cookie.Cookie.Name = "C9.Portal";
        cookie.Cookie.HttpOnly = true;
        cookie.Cookie.SameSite = SameSiteMode.Strict;
        cookie.ExpireTimeSpan = TimeSpan.FromHours(8);
        cookie.SlidingExpiration = false;
        cookie.Events.OnRedirectToLogin = context => ApiStatus(context, StatusCodes.Status401Unauthorized);
        cookie.Events.OnRedirectToAccessDenied = context => ApiStatus(context, StatusCodes.Status403Forbidden);
    })
    .AddScheme<AuthenticationSchemeOptions, BearerKeyHandler>(BearerKeyHandler.SchemeName, null);
builder.Services.AddAuthorization(authorization =>
{
    foreach ((string policy, string[] roles) in PortalPolicies.Roles)
        authorization.AddPolicy(policy, p => p.RequireAuthenticatedUser().RequireRole(roles));
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
string? gameDataConnection = GameDataConnection.Resolve(builder.Configuration);
#if C9_GAMEDATA
if (gameDataConnection is not null) C9.GameData.GameDataServiceCollectionExtensions.AddC9GameData(builder.Services, gameDataConnection);
#endif
foreach (IPortalModule module in modules.Modules) module.ConfigureServices(builder.Services, builder.Configuration);

WebApplication app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers.XFrameOptions = "DENY";
    context.Response.Headers.ContentSecurityPolicy =
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'; frame-ancestors 'none'; form-action 'self'; base-uri 'self'";
    await next();
});
app.UseExceptionHandler("/error", createScopeForErrors: true);
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"),
    pages => pages.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true));
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapGet("/healthz", () => Results.Text("ok")).AllowAnonymous();
app.MapGet("/metrics", async (GameServices services, PortalBus bus, CancellationToken ct) =>
{
    IReadOnlyList<ServiceSnapshot> snapshots = await services.SnapshotAsync(ct);
    PrometheusText text = new PrometheusText()
        .Family("c9_portal_up", "gauge", "Admin Portal is serving").Sample("c9_portal_up", 1)
        .Family("c9_portal_agent_up", "gauge", "1 when the portal can read the service agent");
    foreach (ServiceSnapshot snapshot in snapshots) text.Sample("c9_portal_agent_up", snapshot.Reachable ? 1 : 0, ("service", snapshot.Endpoint.Id));
    text.Family("c9_portal_stream_up", "gauge", "1 when the portal follows the agent live stream");
    foreach ((string id, LiveLink link) in bus.Links) text.Sample("c9_portal_stream_up", link.Connected ? 1 : 0, ("service", id));
    return Results.Text(text.ToString(), PrometheusText.ContentType);
}).AllowAnonymous();
app.MapPost("/signout", async (HttpContext context, Microsoft.AspNetCore.Antiforgery.IAntiforgery antiforgery, IAuditLog audit) =>
{
    if (!await antiforgery.IsRequestValidAsync(context)) return Results.BadRequest();
    await audit.WriteAsync(new AuditEntry(Guid.NewGuid(), DateTimeOffset.UtcNow, context.User.ActorName(), "portal.signout", "portal", true,
        null, null, null, null, context.Connection.RemoteIpAddress?.ToString()), context.RequestAborted);
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/signin");
}).RequireAuthorization().DisableAntiforgery();

RouteGroupBuilder api = app.MapGroup("/api/v1").RequireAuthorization();
api.MapCoreApi();
foreach (IPortalModule module in modules.Modules) module.MapApi(api.MapGroup("/" + module.Id));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. modules.Assemblies]);

app.Logger.LogInformation("C9 Admin Portal on {Url}; operator key file {KeyPath}; {Modules} module(s): {Names}; game database {Database}",
    options.Url, operatorKeys.KeyPath, modules.Modules.Count, string.Join(", ", modules.Modules.Select(m => m.Id)),
    gameDataConnection is null ? "not configured" : "configured");
#if C9_GAMEDATA
if (gameDataConnection is not null) await GameDataConnection.MigrateWithRetryAsync(app, TimeSpan.FromSeconds(90));
#endif
await app.RunAsync();

static Task ApiStatus(RedirectContext<CookieAuthenticationOptions> context, int status)
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.StatusCode = status;
        return Task.CompletedTask;
    }
    context.Response.Redirect(context.RedirectUri);
    return Task.CompletedTask;
}

/// <summary>"Authorization: Bearer &lt;operator key&gt;" for scripts and automation.</summary>
internal sealed class BearerKeyHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
    IPortalUserStore users) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "c9-bearer";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.Ordinal)) return AuthenticateResult.NoResult();
        PortalUser? user = await users.ValidateAsync(new PortalCredentials(null, header[7..]), Context.RequestAborted);
        return user is null ? AuthenticateResult.Fail("Invalid key") : AuthenticateResult.Success(new AuthenticationTicket(PortalPrincipal.Create(user, SchemeName), SchemeName));
    }
}

internal static class PortalPrincipal
{
    public static ClaimsPrincipal Create(PortalUser user, string scheme)
    {
        List<Claim> claims = [new(ClaimTypes.NameIdentifier, user.Id), new(ClaimTypes.Name, user.DisplayName)];
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, scheme));
    }
}

/// <summary>C9_DB_CONNECTION (+ optional C9_DB_PASSWORD_FILE) for the game database of the Accounts/World modules.</summary>
internal static class GameDataConnection
{
    public static string? Resolve(IConfiguration configuration)
    {
        string? connection = configuration["C9_DB_CONNECTION"];
        if (string.IsNullOrWhiteSpace(connection)) return null;
        System.Data.Common.DbConnectionStringBuilder parts = new() { ConnectionString = connection };
        if (!parts.ContainsKey("Password") && configuration["C9_DB_PASSWORD_FILE"] is { Length: > 0 } file && File.Exists(file))
            parts["Password"] = File.ReadAllText(file).Trim();
        return parts.ConnectionString;
    }

#if C9_GAMEDATA
    /// <summary>The database container may still be starting; retry migrations, then keep serving (module pages report the error).</summary>
    public static async Task MigrateWithRetryAsync(WebApplication app, TimeSpan budget)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow + budget;
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                await C9.GameData.GameDataServiceCollectionExtensions.MigrateGameDataAsync(app.Services);
                app.Logger.LogInformation("Game database migrations applied");
                return;
            }
            catch (Exception error) when (error is not OperationCanceledException && DateTimeOffset.UtcNow < deadline)
            {
                app.Logger.LogWarning("Game database not ready (attempt {Attempt}): {Message}", attempt, error.Message);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                app.Logger.LogError(error, "Game database migrations failed; Accounts/World pages will report the database as unavailable");
                return;
            }
        }
    }
#endif
}
