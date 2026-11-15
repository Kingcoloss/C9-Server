using System.Collections.Concurrent;
using System.Security.Cryptography;
using C9.Contracts;
using C9.Protocol;

namespace C9.Server;

/// <summary>
/// Captures every frame of every session with its exact wire bytes and decoded payload,
/// applies the payload policy (credential redaction by default), keeps a ring for live
/// queries, persists JSONL when a directory is configured and pushes frames to the stream.
/// </summary>
public sealed class PacketTracer : IPacketTraceSink, IAsyncDisposable
{
    private sealed class SessionIndex(SessionInfo info)
    {
        public SessionInfo Info = info;
        public int Frames;
        public int Errors;
        public readonly HashSet<string> Files = [];
        public bool Live = true;
    }

    private readonly ConcurrentDictionary<string, SessionIndex> sessions = new();
    private readonly TraceRing ring;
    private readonly TraceFileStore? store;
    private readonly string serviceId;
    private long nextId, captured;

    public PacketTracer(string serviceId, string bootId, TraceOptions options)
    {
        options.Validate();
        this.serviceId = serviceId;
        Options = options;
        ring = new TraceRing(options.RingCapacity);
        if (options.Directory is not null)
        {
            store = new TraceFileStore(options.Directory, serviceId, bootId, options)
            {
                SessionWritten = (key, path) =>
                {
                    if (sessions.TryGetValue(key, out SessionIndex? index)) lock (index) index.Files.Add(path);
                }
            };
            LoadHistory();
        }
    }

    public TraceOptions Options { get; }
    public AgentStreamHub? Stream { get; set; }

    public TraceStatus Status => new(true, Options.PayloadPolicy, store is not null, Interlocked.Read(ref captured),
        store?.Dropped ?? 0, ring.Capacity, store?.Root);

    public void SessionOpened(SessionInfo info)
    {
        sessions[info.Key] = new SessionIndex(info);
        store?.Enqueue(new TraceRecord(TraceRecordKinds.SessionOpened, Session: info));
    }

    public void SessionUpdated(SessionInfo info)
    {
        if (sessions.TryGetValue(info.Key, out SessionIndex? index)) lock (index) index.Info = info;
    }

    public void SessionClosed(SessionInfo info)
    {
        int frames = 0, errors = 0;
        if (sessions.TryGetValue(info.Key, out SessionIndex? index))
            lock (index)
            {
                index.Info = info;
                index.Live = false;
                frames = index.Frames;
                errors = index.Errors;
            }
        store?.Enqueue(new TraceRecord(TraceRecordKinds.SessionClosed, Session: info, FrameCount: frames, ErrorCount: errors));
    }

    public void Frame(SessionContext session, string direction, long streamOffset, ReadOnlySpan<byte> wire,
        Packet? packet, string? state, string? error, int? cipherFrames, int? selector)
    {
        ushort? type = packet?.Type;
        byte? flag = wire.Length > 2 ? wire[2] : null;
        // A plain frame that failed to decode still exposes its type at +3.
        if (type is null && flag == 0 && wire.Length >= 5) type = BitConverter.ToUInt16(wire.Slice(3, 2));
        bool sensitive = type is { } t && KnownPackets.IsSensitive(t);
        string policy = Options.PayloadPolicy;
        string? wireText, payloadText, hash = null;
        bool redacted = false;
        ReadOnlySpan<byte> payload = packet is null ? default : packet.Payload.Span;
        if (policy == TracePayloadPolicies.Full)
        {
            wireText = Convert.ToBase64String(wire);
            payloadText = packet is null ? null : Convert.ToBase64String(payload);
        }
        else if (policy == TracePayloadPolicies.HeadersOnly || sensitive)
        {
            wireText = null;
            payloadText = null;
            redacted = true;
            if (!sensitive && packet is not null) hash = Convert.ToHexStringLower(SHA256.HashData(payload))[..16];
        }
        else if (packet is null && direction == TraceDirections.Inbound && state == "connected")
        {
            // An undecodable first request may be an encrypted login under the static initial key.
            wireText = Convert.ToBase64String(wire[..Math.Min(wire.Length, TraceOptions.RedactedFailurePrefix)]);
            payloadText = null;
            redacted = wire.Length > TraceOptions.RedactedFailurePrefix;
        }
        else
        {
            wireText = Convert.ToBase64String(wire);
            payloadText = packet is null ? null : Convert.ToBase64String(payload);
        }

        TraceFrame frame = new(Interlocked.Increment(ref nextId), DateTimeOffset.UtcNow, serviceId, session.Key, direction,
            TraceTransports.Tcp, streamOffset, wire.Length, flag, type, packet?.Sequence,
            packet is { Flag: not 0 } ? packet.Checksum : null, packet?.Payload.Length, state,
            wireText, payloadText, redacted, hash, error, cipherFrames, selector);
        Interlocked.Increment(ref captured);
        ring.Add(frame);
        if (sessions.TryGetValue(session.Key, out SessionIndex? index))
            lock (index)
            {
                index.Frames++;
                if (error is not null) index.Errors++;
            }
        store?.Enqueue(new TraceRecord(TraceRecordKinds.Frame, Frame: frame));
        Stream?.Publish(StreamEvents.Frame, frame, session.Key);
    }

    public IReadOnlyList<TraceSessionSummary> Sessions(int limit) =>
        sessions.Values.Select(Summary).OrderByDescending(s => s.StartedUtc).Take(Math.Clamp(limit, 1, 5000)).ToArray();

    public SessionInfo? SessionInfoOf(string key)
    {
        if (!sessions.TryGetValue(key, out SessionIndex? index)) return null;
        lock (index) return index.Info;
    }

    public TraceSessionSummary? Session(string key) => sessions.TryGetValue(key, out SessionIndex? index) ? Summary(index) : null;

    private static TraceSessionSummary Summary(SessionIndex index)
    {
        lock (index)
            return new TraceSessionSummary(index.Info.Key, index.Info.ServiceId, index.Info.ConnectedUtc, index.Info.ClosedUtc,
                index.Info.RemoteEndpoint, index.Info.LocalPort, index.Frames, index.Errors, index.Info.State, index.Live,
                index.Info.CloseReason);
    }

    public TracePage Query(TraceQuery query)
    {
        IEnumerable<TraceFrame> source = ring.Snapshot();
        // A session that started before the ring's oldest frame is read from disk instead.
        if (query.Session is { } key && sessions.TryGetValue(key, out SessionIndex? index) && store is not null)
        {
            TraceFrame[] memory = (TraceFrame[])source;
            bool complete;
            lock (index) complete = index.Frames == memory.Count(f => f.Session == key) && (index.Live || index.Frames > 0);
            if (!complete) source = DiskFrames(key);
        }
        return TraceRing.Page(source, query);
    }

    /// <summary>All frames of one session in id order (memory when complete, otherwise disk).</summary>
    public IReadOnlyList<TraceFrame> SessionFrames(string key) =>
        Query(new TraceQuery { Session = key, Limit = TraceQuery.MaxLimit, AfterId = 0 }) is { Truncated: false } page
            ? page.Frames
            : DiskFrames(key).ToArray();

    private IEnumerable<TraceFrame> DiskFrames(string key)
    {
        string[] files;
        if (sessions.TryGetValue(key, out SessionIndex? index)) lock (index) files = index.Files.Order(StringComparer.Ordinal).ToArray();
        else files = [];
        return files.SelectMany(path => TraceFileStore.Read(path, key))
            .Where(r => r.Kind == TraceRecordKinds.Frame && r.Frame?.Session == key)
            .Select(r => r.Frame!).OrderBy(f => f.Id);
    }

    /// <summary>Rebuilds the session index of earlier boots from the side index (closed or interrupted sessions).</summary>
    private void LoadHistory()
    {
        string[] allFiles = store!.Files().Select(f => f.FullName).Order(StringComparer.Ordinal).ToArray();
        foreach (IGrouping<string, TraceIndexEntry> group in store.ReadIndex().GroupBy(e => e.Session!.Key))
        {
            TraceIndexEntry last = group.Last();
            string[] files = group.Select(e => store.FullPath(e.File!)).Where(File.Exists).Distinct().Order(StringComparer.Ordinal).ToArray();
            if (files.Length == 0) continue;
            // Frames written between the opening and closing records may sit in rotated files of the same boot.
            string first = files[0], final = files[^1];
            string bootPrefix = Path.GetFileName(first).Split('-')[0];
            IEnumerable<string> span = allFiles.Where(f => string.CompareOrdinal(f, first) >= 0 && string.CompareOrdinal(f, final) <= 0 &&
                Path.GetFileName(f).StartsWith(bootPrefix + "-", StringComparison.Ordinal));
            SessionIndex index = new(last.Session!) { Live = false, Frames = last.FrameCount ?? 0, Errors = last.ErrorCount ?? 0 };
            foreach (string file in files.Concat(span)) index.Files.Add(file);
            sessions.TryAdd(index.Info.Key, index);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (store is not null) await store.DisposeAsync();
    }
}
