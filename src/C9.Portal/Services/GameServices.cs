using C9.Contracts;

namespace C9.Portal.Services;

/// <summary>Default <see cref="IGameServices"/>: one <see cref="AgentClient"/> per configured service.</summary>
public sealed class GameServices : IGameServices
{
    private readonly Dictionary<string, AgentClient> clients;

    public GameServices(IEnumerable<AgentClient> agents)
    {
        clients = agents.ToDictionary(a => a.Endpoint.Id, StringComparer.Ordinal);
        Endpoints = clients.Values.Select(c => c.Endpoint).ToArray();
    }

    public IReadOnlyList<GameServiceEndpoint> Endpoints { get; }
    public IEnumerable<AgentClient> Clients => clients.Values;

    public AgentClient Client(string serviceId) =>
        clients.TryGetValue(serviceId, out AgentClient? client) ? client : throw new KeyNotFoundException($"Unknown service '{serviceId}'");

    public bool TryClientForSession(string sessionKey, out AgentClient? client)
    {
        client = null;
        return SessionKeys.TryParse(sessionKey, out string service, out _, out _) && clients.TryGetValue(service, out client);
    }

    public async Task<IReadOnlyList<ServiceSnapshot>> SnapshotAsync(CancellationToken cancellationToken = default) =>
        await Task.WhenAll(Endpoints.Select(e => SnapshotAsync(e.Id, cancellationToken)));

    public async Task<ServiceSnapshot> SnapshotAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        AgentClient client = Client(serviceId);
        try { return new ServiceSnapshot(client.Endpoint, await client.StatusAsync(cancellationToken), null); }
        catch (Exception error) when (IsUnavailable(error, cancellationToken))
        {
            return new ServiceSnapshot(client.Endpoint, null, Describe(error));
        }
    }

    public async Task<IReadOnlyList<SessionInfo>> SessionsAsync(bool includeClosed = false, CancellationToken cancellationToken = default)
    {
        IEnumerable<Task<SessionInfo[]>> requests = clients.Values.Select(async client =>
        {
            try { return await client.SessionsAsync(includeClosed, cancellationToken); }
            catch (Exception error) when (IsUnavailable(error, cancellationToken)) { return Array.Empty<SessionInfo>(); }
        });
        return (await Task.WhenAll(requests)).SelectMany(s => s).OrderByDescending(s => s.ConnectedUtc).ToArray();
    }

    public Task<ControlResult> ControlAsync(string serviceId, string action, CancellationToken cancellationToken = default)
    {
        if (!ControlActions.IsValid(action)) throw new ArgumentException("Unknown control action");
        return Client(serviceId).ControlAsync(action, cancellationToken);
    }

    public Task<bool> DisconnectAsync(string sessionKey, CancellationToken cancellationToken = default) =>
        TryClientForSession(sessionKey, out AgentClient? client) ? client!.DisconnectAsync(sessionKey, cancellationToken) : Task.FromResult(false);

    public static bool IsUnavailable(Exception error, CancellationToken cancellationToken) =>
        error is HttpRequestException or AgentException or System.Text.Json.JsonException ||
        (error is OperationCanceledException && !cancellationToken.IsCancellationRequested);

    public static string Describe(Exception error) => error switch
    {
        OperationCanceledException => "Agent did not answer in time",
        HttpRequestException http => "Agent unreachable: " + (http.InnerException?.Message ?? http.Message),
        _ => error.Message
    };
}
