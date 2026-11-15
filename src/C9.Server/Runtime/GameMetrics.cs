using System.Collections.Concurrent;
using System.Diagnostics;
using C9.Contracts;

namespace C9.Server;

/// <summary>Process-wide counters for one game service, exported as Prometheus text by the agent.</summary>
public sealed class GameMetrics
{
    private sealed class Cell { public long Value; }
    private static readonly double[] DurationBuckets = [1, 5, 15, 30, 60, 300, 900, 1800, 3600, 14400];
    private readonly ConcurrentDictionary<(string Direction, ushort? Type), Cell> packets = new();
    private readonly ConcurrentDictionary<string, Cell> errors = new();
    private readonly long[] durationCounts = new long[DurationBuckets.Length + 1];
    private readonly object durationGate = new();
    private double durationSum;
    private long durationCount;
    private long bytesIn, bytesOut, sessionsTotal, rejected;

    public long SessionsTotal => Interlocked.Read(ref sessionsTotal);
    public long BytesReceived => Interlocked.Read(ref bytesIn);
    public long BytesSent => Interlocked.Read(ref bytesOut);
    public long PacketsReceived => packets.Where(p => p.Key.Direction == TraceDirections.Inbound).Sum(p => Interlocked.Read(ref p.Value.Value));
    public long PacketsSent => packets.Where(p => p.Key.Direction == TraceDirections.Outbound).Sum(p => Interlocked.Read(ref p.Value.Value));
    public long SessionErrors => errors.Values.Sum(c => Interlocked.Read(ref c.Value));

    public void SessionOpened() => Interlocked.Increment(ref sessionsTotal);
    public void ConnectionRejected() => Interlocked.Increment(ref rejected);

    public void Packet(string direction, ushort? type, int wireLength)
    {
        Interlocked.Increment(ref packets.GetOrAdd((direction, type), _ => new Cell()).Value);
        if (direction == TraceDirections.Inbound) Interlocked.Add(ref bytesIn, wireLength);
        else Interlocked.Add(ref bytesOut, wireLength);
    }

    public void Error(string reason) => Interlocked.Increment(ref errors.GetOrAdd(NormalizeReason(reason), _ => new Cell()).Value);

    public void SessionClosed(TimeSpan duration)
    {
        double seconds = Math.Max(0, duration.TotalSeconds);
        lock (durationGate)
        {
            int index = Array.FindIndex(DurationBuckets, bound => seconds <= bound);
            durationCounts[index < 0 ? DurationBuckets.Length : index]++;
            durationSum += seconds;
            durationCount++;
        }
    }

    /// <summary>Maps free-text failure reasons to a small, bounded label set.</summary>
    public static string NormalizeReason(string reason) => reason switch
    {
        "TimeoutException" => "timeout",
        "ConnectionError" => "connection",
        "Disconnected" => "disconnected",
        "truncated_frame" => "truncated_frame",
        _ when reason.Contains("cipher epoch", StringComparison.OrdinalIgnoreCase) => "cipher_epoch",
        _ when reason.Contains("checksum", StringComparison.OrdinalIgnoreCase) => "checksum",
        _ when reason.Contains("sequence", StringComparison.OrdinalIgnoreCase) => "sequence",
        _ when reason.Contains("length", StringComparison.OrdinalIgnoreCase) => "length",
        _ when reason.Contains("disabled", StringComparison.OrdinalIgnoreCase) => "account_disabled",
        _ => "protocol"
    };

    public string Render(AgentStatus status, IReadOnlyCollection<SessionInfo> live)
    {
        (string, string) service = ("service", status.ServiceId);
        PrometheusText text = new();
        text.Family("c9_build_info", "gauge", "Agent build and contract version")
            .Sample("c9_build_info", 1, service, ("version", status.Version), ("profile", status.Profile), ("boot_id", status.BootId));
        text.Family("c9_listener_up", "gauge", "1 when the game TCP listener accepts connections")
            .Sample("c9_listener_up", status.Listener == ListenerState.Running ? 1 : 0, service, ("port", status.Port.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        text.Family("c9_sessions_active", "gauge", "Open client connections")
            .Sample("c9_sessions_active", live.Count, service);
        text.Family("c9_sessions_max", "gauge", "Configured connection limit")
            .Sample("c9_sessions_max", status.MaxConnections, service);
        text.Family("c9_sessions_total", "counter", "Accepted client connections since process start")
            .Sample("c9_sessions_total", SessionsTotal, service);
        text.Family("c9_connection_rejections_total", "counter", "Connections refused by the connection limit")
            .Sample("c9_connection_rejections_total", Interlocked.Read(ref rejected), service);
        text.Family("c9_online_players", "gauge", "Sessions that have entered a world map, by map id");
        foreach (var group in live.Where(s => s.MapId is not null).GroupBy(s => s.MapId!.Value))
            text.Sample("c9_online_players", group.Count(), service, ("map", group.Key.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        text.Family("c9_sessions_by_state", "gauge", "Open sessions by protocol state");
        foreach (var group in live.GroupBy(s => s.State))
            text.Sample("c9_sessions_by_state", group.Count(), service, ("state", group.Key));
        text.Family("c9_packets_total", "counter", "Application frames by direction and packet type");
        foreach (var pair in packets.OrderBy(p => p.Key.Direction).ThenBy(p => p.Key.Type))
            text.Sample("c9_packets_total", Interlocked.Read(ref pair.Value.Value), service, ("direction", pair.Key.Direction),
                ("type", pair.Key.Type is { } type ? $"0x{type:X4}" : "unknown"),
                ("name", pair.Key.Type is { } named ? KnownPackets.NameOf(named) : "unknown"));
        text.Family("c9_bytes_total", "counter", "Wire bytes by direction")
            .Sample("c9_bytes_total", BytesReceived, service, ("direction", TraceDirections.Inbound))
            .Sample("c9_bytes_total", BytesSent, service, ("direction", TraceDirections.Outbound));
        text.Family("c9_session_errors_total", "counter", "Session failures by normalized reason");
        foreach (var pair in errors.OrderBy(p => p.Key))
            text.Sample("c9_session_errors_total", Interlocked.Read(ref pair.Value.Value), service, ("reason", pair.Key));
        text.Family("c9_session_duration_seconds", "histogram", "Duration of closed sessions");
        lock (durationGate)
        {
            long cumulative = 0;
            for (int i = 0; i < DurationBuckets.Length; i++)
            {
                cumulative += durationCounts[i];
                text.Sample("c9_session_duration_seconds_bucket", cumulative, service, ("le", DurationBuckets[i].ToString(System.Globalization.CultureInfo.InvariantCulture)));
            }
            cumulative += durationCounts[^1];
            text.Sample("c9_session_duration_seconds_bucket", cumulative, service, ("le", "+Inf"));
            text.Sample("c9_session_duration_seconds_sum", durationSum, service);
            text.Sample("c9_session_duration_seconds_count", durationCount, service);
        }
        text.Family("c9_trace_frames_total", "counter", "Frames captured by packet trace")
            .Sample("c9_trace_frames_total", status.Trace.FramesCaptured, service);
        text.Family("c9_trace_dropped_total", "counter", "Trace records dropped because the writer queue was full")
            .Sample("c9_trace_dropped_total", status.Trace.FramesDropped, service);
        using Process process = Process.GetCurrentProcess();
        text.Family("process_resident_memory_bytes", "gauge", "Resident memory size in bytes")
            .Sample("process_resident_memory_bytes", process.WorkingSet64, service);
        text.Family("process_cpu_seconds_total", "counter", "Total user and system CPU time in seconds")
            .Sample("process_cpu_seconds_total", process.TotalProcessorTime.TotalSeconds, service);
        text.Family("process_start_time_seconds", "gauge", "Start time of the process since unix epoch in seconds")
            .Sample("process_start_time_seconds", status.ProcessStartedUtc.ToUnixTimeMilliseconds() / 1000.0, service);
        text.Family("dotnet_gc_heap_bytes", "gauge", "Managed heap size in bytes")
            .Sample("dotnet_gc_heap_bytes", GC.GetTotalMemory(false), service);
        text.Family("dotnet_threadpool_threads", "gauge", "Thread pool threads")
            .Sample("dotnet_threadpool_threads", ThreadPool.ThreadCount, service);
        return text.ToString();
    }
}
