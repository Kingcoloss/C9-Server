namespace C9.Contracts;

/// <summary>Version of the agent HTTP contract under <c>/agent/v1</c>.</summary>
public static class AgentApi
{
    public const string Version = "1";
    public const string Prefix = "/agent/v1";
    public const string HealthPath = "/healthz";
    public const string MetricsPath = "/metrics";
    public const string TokenHeader = "Authorization";
}

public enum ListenerState { Stopped, Starting, Running, Stopping, Failed }

public static class ControlActions
{
    public const string Start = "start";
    public const string Stop = "stop";
    public const string Restart = "restart";
    public static bool IsValid(string? action) => action is Start or Stop or Restart;
}

public sealed record TraceStatus(bool Enabled, string PayloadPolicy, bool Persisted, long FramesCaptured,
    long FramesDropped, int RingCapacity, string? Directory);

/// <summary>Snapshot of one game service process (login, lobby, world ...).</summary>
public sealed record AgentStatus(
    string ServiceId,
    string BootId,
    string Version,
    string Profile,
    string Host,
    int Port,
    ListenerState Listener,
    string? LastError,
    DateTimeOffset ProcessStartedUtc,
    DateTimeOffset? ListenerStartedUtc,
    int ActiveSessions,
    int MaxConnections,
    long TotalSessions,
    long PacketsReceived,
    long PacketsSent,
    long BytesReceived,
    long BytesSent,
    long SessionErrors,
    long WorkingSetBytes,
    string ConfigPath,
    bool ConfigOverlayActive,
    TraceStatus Trace);

public sealed record ControlRequest(string Action);

public sealed record ControlResult(bool Success, string Message, AgentStatus Status);

/// <summary>
/// One client connection. <see cref="Key"/> is globally unique ("{service}-{bootId}-{number}")
/// and is the correlation id shared by sessions, server events, packet trace and metrics.
/// </summary>
public sealed record SessionInfo(
    string Key,
    long Number,
    string ServiceId,
    string? RemoteEndpoint,
    int LocalPort,
    DateTimeOffset ConnectedUtc,
    DateTimeOffset? ClosedUtc,
    string State,
    ulong? CharacterId,
    int? MapId,
    int? Submap,
    long PacketsReceived,
    long PacketsSent,
    long BytesReceived,
    long BytesSent,
    DateTimeOffset LastActivityUtc,
    string? CloseReason,
    string? LastError,
    long? AccountId = null);

/// <summary>Global session keys have the form "{service}-{boot}-{number}"; the service id may contain '-'.</summary>
public static class SessionKeys
{
    public static string Format(string serviceId, string bootId, long number) => $"{serviceId}-{bootId}-{number}";

    public static bool TryParse(string? key, out string serviceId, out string bootId, out long number)
    {
        serviceId = bootId = "";
        number = 0;
        if (string.IsNullOrEmpty(key) || key.Length > 80) return false;
        int last = key.LastIndexOf('-');
        if (last <= 0 || !long.TryParse(key.AsSpan(last + 1), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out number)) return false;
        int middle = key.LastIndexOf('-', last - 1);
        if (middle <= 0 || middle + 1 == last) return false;
        serviceId = key[..middle];
        bootId = key[(middle + 1)..last];
        return bootId.All(Uri.IsHexDigit);
    }
}

public static class SessionLifecycleKinds
{
    public const string Opened = "opened";
    public const string State = "state";
    public const string Closed = "closed";
}

/// <summary>Pushed on the agent stream whenever a session opens, changes state or closes.</summary>
public sealed record SessionLifecycleEvent(long Id, DateTimeOffset TimeUtc, string Kind, SessionInfo Session);

/// <summary>Mirror of the game server's structured event line (see C9.Server ServerEvent).</summary>
public sealed record ServiceEvent
{
    public string Event { get; init; } = "";
    public DateTimeOffset RecordedAtUtc { get; init; }
    public long? Session { get; init; }
    public string? SessionKey { get; init; }
    public ushort? Type { get; init; }
    public int? WireLength { get; init; }
    public int? PayloadLength { get; init; }
    public byte? Flag { get; init; }
    public uint? Sequence { get; init; }
    public string? State { get; init; }
    public string? Reason { get; init; }
    public string? Host { get; init; }
    public int? Port { get; init; }
    public string? Profile { get; init; }
    public string? Remote { get; init; }
    public ulong? CharacterId { get; init; }
    public int? MapId { get; init; }
}
