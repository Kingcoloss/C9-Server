using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using C9.Contracts;
using C9.GameData;
using C9.Portal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>Stand-in for the C9.Portal host: same discovery contract (ConfigureServices, MapApi on /api/v1/{Id}), test auth.</summary>
internal sealed class PortalTestHost : IAsyncDisposable
{
    public const string RolesHeader = "X-Test-Roles";
    private readonly WebApplication app;
    public HttpClient Client { get; }
    public MemoryAudit Audit { get; } = new();
    public FakeGameServices Game { get; } = new();
    public IServiceProvider Services => app.Services;

    private PortalTestHost(WebApplication app, HttpClient client, MemoryAudit audit, FakeGameServices game)
    {
        this.app = app; Client = client; Audit = audit; Game = game;
    }

    public static async Task<PortalTestHost> StartAsync(string? connection)
    {
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        if (Environment.GetEnvironmentVariable("C9_VALIDATION_VERBOSE") is not null) builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Error);
        MemoryAudit audit = new();
        FakeGameServices game = new();
        builder.Services.AddSingleton<IAuditLog>(audit).AddSingleton<IGameServices>(game).AddSingleton<ISessionFeed>(game);
        builder.Services.AddAuthentication("test").AddScheme<AuthenticationSchemeOptions, HeaderAuth>("test", null);
        builder.Services.AddAuthorization(options =>
        {
            foreach (var (policy, roles) in PortalPolicies.Roles) options.AddPolicy(policy, p => p.RequireRole(roles));
        });
        if (connection is not null) builder.Services.AddC9GameData(connection);
        IPortalModule[] modules = [new C9.Portal.Accounts.AccountsModule(), new C9.Portal.World.WorldModule()];
        foreach (IPortalModule module in modules) module.ConfigureServices(builder.Services, builder.Configuration);
        // As in the portal host: Blazor Web App with interactive server components (pages are prerendered on GET).
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddCascadingAuthenticationState();
        WebApplication app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.MapRazorComponents<TestApp>().AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(C9.Portal.Accounts.AccountsModule).Assembly, typeof(C9.Portal.World.WorldModule).Assembly);
        foreach (IPortalModule module in modules) module.MapApi(app.MapGroup($"/api/v1/{module.Id}").RequireAuthorization());
        if (connection is not null) await app.Services.MigrateGameDataAsync();
        await app.StartAsync();
        string address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
        return new PortalTestHost(app, new HttpClient { BaseAddress = new Uri(address) }, audit, game);
    }

    public async Task<(int Status, JsonElement Body)> SendAsync(HttpMethod method, string path, string? roles, object? body = null, Guid? key = null)
    {
        using HttpRequestMessage request = new(method, path);
        if (roles is not null) request.Headers.Add(RolesHeader, roles);
        if (key is not null) request.Headers.Add("Idempotency-Key", key.Value.ToString());
        if (body is not null) request.Content = JsonContent.Create(body, options: ContractJson.Options);
        using HttpResponseMessage response = await Client.SendAsync(request);
        string text = await response.Content.ReadAsStringAsync();
        JsonElement json = text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone();
        return ((int)response.StatusCode, json);
    }

    /// <summary>GET a page and return the prerendered HTML.</summary>
    public async Task<(int Status, string Html)> Page(string path, string role)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, path);
        request.Headers.Add(RolesHeader, role);
        using HttpResponseMessage response = await Client.SendAsync(request);
        return ((int)response.StatusCode, System.Net.WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
    }

    public Task<(int Status, JsonElement Body)> Get(string path, string roles = "Owner") => SendAsync(HttpMethod.Get, path, roles);
    public Task<(int Status, JsonElement Body)> Post(string path, object body, string roles = "Owner", Guid? key = null) =>
        SendAsync(HttpMethod.Post, path, roles, body, key ?? Guid.NewGuid());
    public Task<(int Status, JsonElement Body)> Put(string path, object body, string roles = "Owner") =>
        SendAsync(HttpMethod.Put, path, roles, body, Guid.NewGuid());
    public Task<(int Status, JsonElement Body)> Patch(string path, object body, string roles = "Owner") =>
        SendAsync(HttpMethod.Patch, path, roles, body, Guid.NewGuid());

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await app.StopAsync();
        await app.DisposeAsync();
    }
}

/// <summary>Authenticates "X-Test-Roles: Role1,Role2" as user "tester"; no header = anonymous.</summary>
internal sealed class HeaderAuth(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(PortalTestHost.RolesHeader, out var header)) return Task.FromResult(AuthenticateResult.NoResult());
        List<Claim> claims = [new(ClaimTypes.NameIdentifier, "tester")];
        claims.AddRange(header.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(role => new Claim(ClaimTypes.Role, role)));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, "test")), "test")));
    }
}

internal sealed class MemoryAudit : IAuditLog
{
    public ConcurrentQueue<AuditEntry> Entries { get; } = new();
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default) { Entries.Enqueue(entry); return Task.CompletedTask; }
    public Task<IReadOnlyList<AuditEntry>> QueryAsync(AuditQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AuditEntry>>(Entries.ToArray());
}

internal sealed class FakeGameServices : IGameServices, ISessionFeed
{
    public List<SessionInfo> Sessions { get; } = [];
    public List<string> Disconnected { get; } = [];
    public IReadOnlyList<GameServiceEndpoint> Endpoints => [];
    public Task<IReadOnlyList<ServiceSnapshot>> SnapshotAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ServiceSnapshot>>([]);
    public Task<ServiceSnapshot> SnapshotAsync(string serviceId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<IReadOnlyList<SessionInfo>> SessionsAsync(bool includeClosed = false, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<SessionInfo>>(Sessions.Where(s => includeClosed || s.ClosedUtc is null).ToArray());
    public Task<ControlResult> ControlAsync(string serviceId, string action, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<bool> DisconnectAsync(string sessionKey, CancellationToken cancellationToken = default)
    {
        bool found = Sessions.Any(s => s.Key == sessionKey && s.ClosedUtc is null);
        if (found) Disconnected.Add(sessionKey);
        return Task.FromResult(found);
    }
    public async IAsyncEnumerable<SessionLifecycleEvent> SubscribeAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken).ContinueWith(_ => { });
        yield break;
    }

    public static SessionInfo Session(string key, int? map, int? submap = 0, ulong? character = null, DateTimeOffset? closed = null) =>
        new(key, 1, "lobby", "127.0.0.1:50000", 14001, DateTimeOffset.UtcNow.AddMinutes(-5), closed, map is null ? "character_lobby" : "world_loading",
            character, map, submap, 10, 12, 100, 120, DateTimeOffset.UtcNow, null, null);
}
