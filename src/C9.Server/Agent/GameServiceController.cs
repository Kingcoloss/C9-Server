using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using C9.Contracts;

namespace C9.Server;

/// <summary>
/// Owns the game listener of one service process so the portal can start, stop and restart it
/// (restart re-reads configuration) while the process and its agent keep running.
/// </summary>
public sealed class GameServiceController(ConfigSource config, IServerEvents events, ServiceRuntime runtime) : IAsyncDisposable
{
    public static string Version { get; } = typeof(GameServiceController).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "0.0.0";

    private readonly SemaphoreSlim gate = new(1, 1);
    private readonly object state = new();
    private TcpGameServer? server;
    private ConfigSource.Loaded? loaded;
    private ListenerState listener = ListenerState.Stopped;
    private string? lastError;
    private DateTimeOffset? listenerStarted;

    public ConfigSource Config => config;

    public async Task<ControlResult> ExecuteAsync(string action, CancellationToken cancellationToken = default)
    {
        if (!ControlActions.IsValid(action)) throw new ArgumentException("Unknown control action");
        await gate.WaitAsync(cancellationToken);
        try
        {
            if (action is ControlActions.Stop or ControlActions.Restart) await StopCoreAsync();
            if (action is ControlActions.Start or ControlActions.Restart) await StartCoreAsync(cancellationToken);
            AgentStatus status = Status();
            bool success = action == ControlActions.Stop ? status.Listener == ListenerState.Stopped : status.Listener == ListenerState.Running;
            string message = success
                ? action == ControlActions.Stop ? "Listener stopped" : $"Listening on {status.Host}:{status.Port}"
                : status.LastError ?? "Operation failed";
            events.Emit(new ServerEvent("control") { State = action, Reason = success ? null : message });
            return new ControlResult(success, message, status);
        }
        finally { gate.Release(); }
    }

    private async Task StartCoreAsync(CancellationToken cancellationToken)
    {
        if (server is not null) return;
        lock (state) { listener = ListenerState.Starting; lastError = null; }
        TcpGameServer? candidate = null;
        try
        {
            ConfigSource.Loaded next = config.Load();
            candidate = new TcpGameServer(next.Options, events, runtime);
            await candidate.StartAsync(cancellationToken);
            lock (state)
            {
                server = candidate;
                loaded = next;
                listener = ListenerState.Running;
                listenerStarted = DateTimeOffset.UtcNow;
            }
        }
        catch (Exception error) when (error is ArgumentException or IOException or JsonException or InvalidDataException or
            InvalidOperationException or KeyNotFoundException or FormatException or OverflowException or
            UnauthorizedAccessException or System.Net.Sockets.SocketException)
        {
            if (candidate is not null) await candidate.DisposeAsync();
            lock (state) { listener = ListenerState.Failed; lastError = $"{error.GetType().Name}: {error.Message}"; }
            events.Emit(new ServerEvent("listener_failed") { Reason = lastError });
        }
    }

    private async Task StopCoreAsync()
    {
        TcpGameServer? current;
        lock (state)
        {
            current = server;
            if (current is null)
            {
                if (listener == ListenerState.Failed) listener = ListenerState.Stopped;
                return;
            }
            listener = ListenerState.Stopping;
        }
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(15));
        try { await current.StopAsync(timeout.Token); }
        catch (OperationCanceledException) { }
        await current.DisposeAsync();
        lock (state)
        {
            server = null;
            listener = ListenerState.Stopped;
            listenerStarted = null;
        }
    }

    public bool Disconnect(string key)
    {
        SessionContext? session = runtime.FindLive(key);
        TcpGameServer? current;
        lock (state) current = server;
        return session is not null && current is not null && current.Disconnect(session.Number);
    }

    public string EffectiveConfigJson()
    {
        string path;
        lock (state) path = loaded?.Path ?? Path.GetFullPath(config.OverlayPath is { } overlay && File.Exists(overlay) ? overlay : config.BasePath);
        return File.ReadAllText(path);
    }

    public AgentStatus Status()
    {
        ServerOptions? options;
        string path;
        bool overlay;
        ListenerState current;
        string? error;
        DateTimeOffset? started;
        int port;
        lock (state)
        {
            options = loaded?.Options;
            path = loaded?.Path ?? config.BasePath;
            overlay = loaded?.OverlayActive ?? false;
            current = listener;
            error = lastError;
            started = listenerStarted;
            port = server?.Port ?? options?.Port ?? 0;
        }
        if (options is null)
        {
            try { options = config.Load().Options; }
            catch (Exception failure) when (failure is ArgumentException or IOException or JsonException or InvalidDataException or KeyNotFoundException or FormatException)
            {
                error ??= failure.Message;
            }
            port = options?.Port ?? config.PortOverride ?? 0;
        }
        IReadOnlyCollection<SessionInfo> live = runtime.LiveSessions();
        using Process process = Process.GetCurrentProcess();
        return new AgentStatus(runtime.ServiceId, runtime.BootId, Version, options?.Profile ?? "unknown",
            options?.Host ?? config.HostOverride ?? "", port, current, error, runtime.ProcessStartedUtc, started,
            live.Count, options?.MaxConnections ?? 0, runtime.Metrics.SessionsTotal, runtime.Metrics.PacketsReceived,
            runtime.Metrics.PacketsSent, runtime.Metrics.BytesReceived, runtime.Metrics.BytesSent, runtime.Metrics.SessionErrors,
            process.WorkingSet64, path, overlay,
            runtime.Tracer?.Status ?? new TraceStatus(false, TracePayloadPolicies.HeadersOnly, false, 0, 0, 0, null));
    }

    public async ValueTask DisposeAsync()
    {
        await gate.WaitAsync();
        try { await StopCoreAsync(); }
        finally { gate.Release(); }
    }
}
