using System.Collections.Concurrent;
using C9.Contracts;

namespace C9.Server;

/// <summary>
/// Agent-mode state shared by every listener instance of one game service process:
/// session numbering, the live session registry, metrics, trace and the live stream.
/// </summary>
public sealed class ServiceRuntime : ISessionObserver, IServerEvents
{
    private const int RecentClosedCapacity = 256;
    private const int RecentEventCapacity = 512;
    private readonly ConcurrentDictionary<string, SessionContext> live = new();
    private readonly ConcurrentQueue<SessionInfo> closed = new();
    private readonly ConcurrentQueue<ServerEvent> recentEvents = new();
    private long sessionCounter, lifecycleCounter;

    public ServiceRuntime(string serviceId, PacketTracer? tracer = null, string? bootId = null)
    {
        if (string.IsNullOrWhiteSpace(serviceId) || serviceId.Length > 32 ||
            !serviceId.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_'))
            throw new ArgumentException("Service id must be 1-32 ASCII letters, digits, '-' or '_'");
        ServiceId = serviceId;
        BootId = bootId ?? Convert.ToHexStringLower(System.Security.Cryptography.RandomNumberGenerator.GetBytes(3));
        Tracer = tracer;
    }

    public string ServiceId { get; }
    public string BootId { get; }
    public DateTimeOffset ProcessStartedUtc { get; } = DateTimeOffset.UtcNow;
    public GameMetrics Metrics { get; } = new();
    public AgentStreamHub Stream { get; } = new();
    public PacketTracer? Tracer { get; }

    public SessionContext CreateSession(string? remote, int localPort)
    {
        long number = Interlocked.Increment(ref sessionCounter);
        return new SessionContext(number, $"{ServiceId}-{BootId}-{number}", ServiceId, remote, localPort, this, Tracer);
    }

    public void ConnectionRejected() => Metrics.ConnectionRejected();

    public IReadOnlyCollection<SessionInfo> LiveSessions() => live.Values.Select(s => s.Snapshot()).OrderBy(s => s.Number).ToArray();
    public IReadOnlyCollection<SessionInfo> RecentClosedSessions() => closed.Reverse().ToArray();
    public SessionContext? FindLive(string key) => live.GetValueOrDefault(key);
    public IReadOnlyList<ServerEvent> RecentEvents(int limit) => recentEvents.Reverse().Take(limit).ToArray();

    public void Opened(SessionContext session)
    {
        live[session.Key] = session;
        Metrics.SessionOpened();
        SessionInfo info = session.Snapshot();
        Tracer?.SessionOpened(info);
        Publish(SessionLifecycleKinds.Opened, info);
    }

    public void Packet(SessionContext session, string direction, ushort? type, int wireLength) =>
        Metrics.Packet(direction, type, wireLength);

    public void StateChanged(SessionContext session)
    {
        SessionInfo info = session.Snapshot();
        Tracer?.SessionUpdated(info);
        Publish(SessionLifecycleKinds.State, info);
    }

    public void Error(SessionContext session, string reason) => Metrics.Error(reason);

    public void Closed(SessionContext session)
    {
        live.TryRemove(session.Key, out _);
        SessionInfo info = session.Snapshot();
        closed.Enqueue(info);
        while (closed.Count > RecentClosedCapacity && closed.TryDequeue(out _)) { }
        Metrics.SessionClosed((info.ClosedUtc ?? DateTimeOffset.UtcNow) - info.ConnectedUtc);
        Tracer?.SessionClosed(info);
        Publish(SessionLifecycleKinds.Closed, info);
    }

    private void Publish(string kind, SessionInfo info) =>
        Stream.Publish(StreamEvents.Session,
            new SessionLifecycleEvent(Interlocked.Increment(ref lifecycleCounter), DateTimeOffset.UtcNow, kind, info), info.Key);

    /// <summary>Keeps recent structured events; non-packet events are also pushed on the live stream.</summary>
    public void Emit(ServerEvent value)
    {
        recentEvents.Enqueue(value);
        while (recentEvents.Count > RecentEventCapacity && recentEvents.TryDequeue(out _)) { }
        if (value.Event is not ("received" or "sent"))
            Stream.Publish(StreamEvents.Event, value, value.SessionKey);
    }
}
