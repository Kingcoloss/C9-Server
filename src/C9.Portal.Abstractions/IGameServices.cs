using C9.Contracts;

namespace C9.Portal;

/// <summary>A game service process the portal manages through its agent.</summary>
public sealed record GameServiceEndpoint(string Id, string Name, Uri AgentUrl, int PublicPort, string? Description = null);

/// <summary>Status of one service, or why it could not be read.</summary>
public sealed record ServiceSnapshot(GameServiceEndpoint Endpoint, AgentStatus? Status, string? Error)
{
    public bool Reachable => Status is not null;
}

/// <summary>Typed access to every game-service agent (status, sessions, control). Registered by the host.</summary>
public interface IGameServices
{
    IReadOnlyList<GameServiceEndpoint> Endpoints { get; }
    Task<IReadOnlyList<ServiceSnapshot>> SnapshotAsync(CancellationToken cancellationToken = default);
    Task<ServiceSnapshot> SnapshotAsync(string serviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SessionInfo>> SessionsAsync(bool includeClosed = false, CancellationToken cancellationToken = default);
    Task<ControlResult> ControlAsync(string serviceId, string action, CancellationToken cancellationToken = default);
    /// <summary>Disconnects a live session by its global key ("{service}-{boot}-{n}").</summary>
    Task<bool> DisconnectAsync(string sessionKey, CancellationToken cancellationToken = default);
}

/// <summary>
/// Live session lifecycle from all services (opened, state changes with character/map, closed).
/// Each subscriber gets its own stream; slow subscribers lose the oldest events.
/// Use it to persist sessions (for example ops.sessions) without touching the game server.
/// </summary>
public interface ISessionFeed
{
    IAsyncEnumerable<SessionLifecycleEvent> SubscribeAsync(CancellationToken cancellationToken);
}
