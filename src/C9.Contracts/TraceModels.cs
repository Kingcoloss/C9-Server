namespace C9.Contracts;

public static class TraceDirections
{
    /// <summary>Client to server.</summary>
    public const string Inbound = "c2s";
    /// <summary>Server to client.</summary>
    public const string Outbound = "s2c";
}

public static class TracePayloadPolicies
{
    /// <summary>Keep decoded payloads except packets marked sensitive (login credentials). Default.</summary>
    public const string RedactCredentials = "redact-credentials";
    /// <summary>Keep every byte, including credentials. Development only.</summary>
    public const string Full = "full";
    /// <summary>Keep headers, lengths and hashes only.</summary>
    public const string HeadersOnly = "headers";
    public static bool IsValid(string? value) => value is RedactCredentials or Full or HeadersOnly;
}

public static class TraceTransports
{
    public const string Tcp = "tcp";
    public const string Udp = "udp";
}

/// <summary>
/// One application frame on a connection. <see cref="Wire"/> holds the exact bytes on the socket
/// (encrypted when <see cref="Flag"/> is non-zero) and <see cref="Payload"/> the decoded payload,
/// both base64. When decoding fails <see cref="Error"/> is set and <see cref="Wire"/> still holds
/// the bytes that failed, so the failure can be analysed offline.
/// </summary>
public sealed record TraceFrame(
    long Id,
    DateTimeOffset TimeUtc,
    string ServiceId,
    string Session,
    string Direction,
    string Transport,
    long StreamOffset,
    int WireLength,
    byte? Flag,
    ushort? Type,
    uint? Sequence,
    uint? Checksum,
    int? PayloadLength,
    string? State,
    string? Wire,
    string? Payload,
    bool Redacted,
    string? PayloadSha256,
    string? Error,
    int? CipherFrames,
    int? Selector);

public sealed record TraceSessionSummary(
    string Key,
    string ServiceId,
    DateTimeOffset StartedUtc,
    DateTimeOffset? EndedUtc,
    string? RemoteEndpoint,
    int LocalPort,
    int Frames,
    int Errors,
    string? LastState,
    bool Live,
    string? CloseReason);

/// <summary>Filter for trace queries. All fields are optional; results are ordered by <see cref="TraceFrame.Id"/>.</summary>
public sealed record TraceQuery
{
    public string? Session { get; init; }
    public string? Direction { get; init; }
    public IReadOnlyList<ushort>? Types { get; init; }
    public bool ErrorsOnly { get; init; }
    public long? AfterId { get; init; }
    public long? BeforeId { get; init; }
    public DateTimeOffset? SinceUtc { get; init; }
    public int Limit { get; init; } = 200;
    public const int MaxLimit = 5000;
}

public sealed record TracePage(IReadOnlyList<TraceFrame> Frames, long? NextAfterId, bool Truncated);

/// <summary>Record kinds stored in trace JSONL files and pushed on the live stream.</summary>
public static class TraceRecordKinds
{
    public const string SessionOpened = "session_opened";
    public const string Frame = "frame";
    public const string SessionClosed = "session_closed";
}

/// <summary>One line of a trace JSONL file. Closing records carry the frame and error totals of the session.</summary>
public sealed record TraceRecord(string Kind, TraceFrame? Frame = null, SessionInfo? Session = null,
    int? FrameCount = null, int? ErrorCount = null);

/// <summary>Server-sent event names on <c>GET /agent/v1/stream</c>.</summary>
public static class StreamEvents
{
    public const string Frame = "frame";
    public const string Session = "session";
    public const string Event = "event";
    public const string Status = "status";
}
