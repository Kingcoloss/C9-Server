using C9.Contracts;
using C9.Protocol;

namespace C9.Server;

/// <summary>Receives session lifecycle and per-packet notifications from <see cref="SessionProcessor"/>.</summary>
public interface ISessionObserver
{
    void Opened(SessionContext session);
    void Packet(SessionContext session, string direction, ushort? type, int wireLength);
    void StateChanged(SessionContext session);
    void Error(SessionContext session, string reason);
    void Closed(SessionContext session);
}

/// <summary>Receives every framed packet (or framing failure) for packet trace.</summary>
public interface IPacketTraceSink
{
    /// <param name="wire">Exact socket bytes of the frame, or the unframed preview on failure.</param>
    /// <param name="packet">Decoded packet; null when decoding failed.</param>
    void Frame(SessionContext session, string direction, long streamOffset, ReadOnlySpan<byte> wire,
        Packet? packet, string? state, string? error, int? cipherFrames, int? selector);
}

/// <summary>
/// Identity and live counters of one connection. The legacy path (no agent) uses
/// <see cref="Legacy"/>, which has no observer and no trace, so behaviour is unchanged.
/// </summary>
public sealed class SessionContext
{
    private long packetsReceived, packetsSent, bytesReceived, bytesSent;
    private readonly object gate = new();
    private string state = "connected";
    private string? lastError, closeReason;
    private DateTimeOffset lastActivity;
    private DateTimeOffset? closed;
    private bool disconnectRequested;

    public SessionContext(long number, string key, string serviceId, string? remote = null, int localPort = 0,
        ISessionObserver? observer = null, IPacketTraceSink? trace = null)
    {
        Number = number;
        Key = key;
        ServiceId = serviceId;
        Remote = remote;
        LocalPort = localPort;
        Observer = observer;
        Trace = trace;
        ConnectedUtc = lastActivity = DateTimeOffset.UtcNow;
    }

    public static SessionContext Legacy(long id) => new(id, id.ToString(System.Globalization.CultureInfo.InvariantCulture), "local");

    public long Number { get; }
    public string Key { get; }
    public string ServiceId { get; }
    public string? Remote { get; }
    public int LocalPort { get; }
    public DateTimeOffset ConnectedUtc { get; }
    public ISessionObserver? Observer { get; }
    public IPacketTraceSink? Trace { get; }
    /// <summary>True when events should carry the global key (agent runtime active).</summary>
    public bool Keyed => Observer is not null;
    public ulong? CharacterId { get; private set; }
    /// <summary>Game database account id; null when the lobby uses the JSON character store.</summary>
    public long? AccountId { get; private set; }
    public int? MapId { get; private set; }
    public int? Submap { get; private set; }
    public string State { get { lock (gate) return state; } }

    internal void RecordPacket(string direction, ushort? type, int wireLength)
    {
        if (direction == TraceDirections.Inbound)
        {
            Interlocked.Increment(ref packetsReceived);
            Interlocked.Add(ref bytesReceived, wireLength);
        }
        else
        {
            Interlocked.Increment(ref packetsSent);
            Interlocked.Add(ref bytesSent, wireLength);
        }
        lock (gate) lastActivity = DateTimeOffset.UtcNow;
        Observer?.Packet(this, direction, type, wireLength);
    }

    internal void InitState(string value)
    {
        lock (gate) state = value;
    }

    internal void SetState(string value, ulong? characterId = null, int? mapId = null, int? submap = null, long? accountId = null)
    {
        lock (gate)
        {
            state = value;
            if (accountId is not null) AccountId = accountId;
            if (characterId is not null) CharacterId = characterId;
            if (mapId is not null) { MapId = mapId; Submap = submap; }
        }
        Observer?.StateChanged(this);
    }

    internal void RecordError(string reason)
    {
        lock (gate) lastError = reason;
        Observer?.Error(this, reason);
    }

    /// <summary>Marks an operator-initiated disconnect so the close reason is reported as such.</summary>
    public void MarkDisconnectRequested()
    {
        lock (gate) disconnectRequested = true;
    }

    internal void Close(string? reason)
    {
        lock (gate)
        {
            if (closed is not null) return;
            closed = DateTimeOffset.UtcNow;
            closeReason = disconnectRequested ? "disconnected_by_operator" : reason ?? lastError;
        }
        Observer?.Closed(this);
    }

    public SessionInfo Snapshot()
    {
        lock (gate)
            return new SessionInfo(Key, Number, ServiceId, Remote, LocalPort, ConnectedUtc, closed, state,
                CharacterId, MapId, Submap, Interlocked.Read(ref packetsReceived), Interlocked.Read(ref packetsSent),
                Interlocked.Read(ref bytesReceived), Interlocked.Read(ref bytesSent), lastActivity, closeReason, lastError, AccountId);
    }
}
