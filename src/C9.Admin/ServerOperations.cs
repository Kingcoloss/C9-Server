using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.Json;
using C9.Server;

namespace C9.Admin;

public sealed record ServiceStatus(string Id, string Name, int Port, string Profile, string State,
    int? Pid, string Uptime, int Connections, long? MemoryMiB, bool CanStart, bool CanStop,
    string? Message, string LogSource, IReadOnlyList<ServerEvent> Events);

public sealed record OperationRecord(DateTimeOffset TimeUtc, string Action, string Service, bool Success, string Message);
public sealed record ConsoleStatus(IReadOnlyList<ServiceStatus> Services, IReadOnlyList<OperationRecord> Operations);

public sealed class ServerOperations : IHostedService
{
    private sealed class Endpoint(string id, string name, int port, string profile, string config, string clientLog)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public int Port { get; } = port;
        public string Profile { get; } = profile;
        public string Config { get; } = config;
        public string ClientLog { get; } = clientLog;
        public Process? Process { get; set; }
        public string? LogPath { get; set; }
        public DateTimeOffset? Started { get; set; }
        public bool Ready { get; set; }
        public string? Error { get; set; }
    }

    private readonly Endpoint[] endpoints =
    [
        new("lobby", "Character & World", 14001, "character_lobby", "lab-official-character-lobby.json", "L9.handoff.log"),
        new("login", "Login & World List", 14000, "fixture", "lab-official-login-fixture.json", "L9.server.log")
    ];
    private readonly SemaphoreSlim operations = new(1);
    private readonly object state = new();
    private readonly string workspace;
    private readonly string backend;
    private readonly string dotnet;
    private readonly string? clientDirectory;
    private readonly string logDirectory;
    private readonly string auditPath;

    public ServerOperations(string workspace, string stateDirectory, string backend, string dotnet, string? clientDirectory)
    {
        this.workspace = workspace;
        this.backend = backend;
        this.dotnet = dotnet;
        this.clientDirectory = clientDirectory;
        logDirectory = Path.Combine(stateDirectory, "logs");
        auditPath = Path.Combine(stateDirectory, "operations.jsonl");
        Directory.CreateDirectory(logDirectory);
    }

    public ConsoleStatus Snapshot()
    {
        IPGlobalProperties network = IPGlobalProperties.GetIPGlobalProperties();
        HashSet<int> listening = network.GetActiveTcpListeners().Select(e => e.Port).ToHashSet();
        TcpConnectionInformation[] connections = network.GetActiveTcpConnections();
        lock (state)
        {
            ServiceStatus[] services = endpoints.Reverse().Select(endpoint =>
            {
                bool occupied = listening.Contains(endpoint.Port);
                Process? process = endpoint.Process;
                bool alive = process is { HasExited: false };
                bool external = occupied && !alive;
                string status = external ? "External" : alive ? (endpoint.Ready && occupied ? "Running" : "Starting") :
                    endpoint.Error is not null || process is { HasExited: true } ? "Failed" : "Stopped";
                string? log = endpoint.LogPath;
                string source = "Admin Console";
                if (external && clientDirectory is not null)
                {
                    log = Path.Combine(clientDirectory, endpoint.ClientLog);
                    source = "Local Launcher";
                }
                else if (log is null)
                    log = Directory.EnumerateFiles(logDirectory, endpoint.Id + "-*.jsonl")
                        .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
                int active = connections.Count(connection => connection.LocalEndPoint.Port == endpoint.Port &&
                    connection.State == TcpState.Established);
                return new ServiceStatus(endpoint.Id, endpoint.Name, endpoint.Port, endpoint.Profile, status,
                    alive ? process!.Id : null,
                    alive && endpoint.Started is { } started ? (DateTimeOffset.UtcNow - started).ToString(@"d\.hh\:mm\:ss") : "—",
                    active, alive ? WorkingSetMiB(process!) : null, !occupied && !alive, alive,
                    external ? "พอร์ตนี้เปิดจากโปรแกรมอื่น" : endpoint.Error ?? (process is { HasExited: true } ? $"Exit code {process.ExitCode}" : null),
                    source, log is null ? [] : ReadRecent<ServerEvent>(log, 24));
            }).ToArray();
            return new ConsoleStatus(services, ReadRecent<OperationRecord>(auditPath, 18).Reverse().ToArray());
        }
    }

    public async Task<IReadOnlyList<OperationRecord>> ExecuteAsync(string action, string target)
    {
        if (action is not ("start" or "stop" or "restart")) throw new ArgumentException("Unknown operation.");
        Endpoint[] selected = target == "all"
            ? (action == "stop" ? endpoints.Reverse().ToArray() : endpoints)
            : endpoints.Where(endpoint => endpoint.Id == target).ToArray();
        if (selected.Length == 0) throw new ArgumentException("Unknown service.");

        await operations.WaitAsync();
        try
        {
            List<OperationRecord> results = [];
            foreach (Endpoint endpoint in selected)
            {
                OperationRecord entry;
                try
                {
                    if (action is "stop" or "restart") await StopEndpointAsync(endpoint);
                    if (action is "start" or "restart") await StartEndpointAsync(endpoint);
                    entry = new(DateTimeOffset.UtcNow, action, endpoint.Name, true,
                        action == "stop" ? "ปิดบริการแล้ว" : "บริการพร้อมรับการเชื่อมต่อ");
                }
                catch (Exception error) when (error is IOException or InvalidOperationException or ArgumentException or
                    TimeoutException or JsonException or KeyNotFoundException or FormatException or OverflowException or
                    Win32Exception or UnauthorizedAccessException)
                {
                    entry = new(DateTimeOffset.UtcNow, action, endpoint.Name, false, error.Message);
                }
                File.AppendAllText(auditPath, JsonSerializer.Serialize(entry) + Environment.NewLine);
                results.Add(entry);
            }
            return results;
        }
        finally { operations.Release(); }
    }

    private async Task StartEndpointAsync(Endpoint endpoint)
    {
        if (endpoint.Process is { HasExited: false } && endpoint.Ready) return;
        if (PortInUse(endpoint.Port)) throw new InvalidOperationException($"พอร์ต {endpoint.Port} ถูกใช้งานอยู่");
        string configPath = Path.Combine(workspace, "config", endpoint.Config);
        ServerOptions config = ServerOptions.FromJson(File.ReadAllText(configPath));
        config.Validate();
        if (config.Host != "127.0.0.1" || config.Port != endpoint.Port || config.Profile != endpoint.Profile)
            throw new InvalidOperationException("ค่าบริการไม่ตรงกับพอร์ตและโปรไฟล์ของ Local Server");
        if (!File.Exists(backend) || !File.Exists(dotnet))
            throw new FileNotFoundException("กรุณาเปิด Console ผ่าน scripts/Start-C9Admin.ps1 เพื่อเตรียมตัวเซิร์ฟเวอร์");

        string log = Path.Combine(logDirectory, $"{endpoint.Id}-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.jsonl");
        ProcessStartInfo start = new(dotnet)
        {
            WorkingDirectory = workspace, UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardInput = true, RedirectStandardError = true
        };
        foreach (string argument in new[] { backend, "--config", configPath, "--event-log", log, "--managed" })
            start.ArgumentList.Add(argument);

        Process process = Process.Start(start) ?? throw new IOException("เปิดโปรเซสเซิร์ฟเวอร์ไม่สำเร็จ");
        lock (state)
        {
            endpoint.Process?.Dispose();
            endpoint.Process = process;
            endpoint.Ready = false;
            endpoint.Error = null;
            endpoint.LogPath = log;
            endpoint.Started = DateTimeOffset.UtcNow;
        }
        process.ErrorDataReceived += (_, eventArgs) =>
        {
            if (string.IsNullOrWhiteSpace(eventArgs.Data)) return;
            lock (state)
                if (ReferenceEquals(endpoint.Process, process)) endpoint.Error = eventArgs.Data[..Math.Min(320, eventArgs.Data.Length)];
        };
        process.BeginErrorReadLine();

        for (int attempt = 0; attempt < 40; attempt++)
        {
            if (process.HasExited) break;
            if (ReadRecent<ServerEvent>(log, 8).Any(entry => entry.Event == "listening" && entry.Port == endpoint.Port))
            {
                lock (state) endpoint.Ready = true;
                return;
            }
            await Task.Delay(150);
        }
        string message;
        lock (state) message = endpoint.Error ?? $"พอร์ต {endpoint.Port} ยังไม่พร้อมหลังเปิดบริการ";
        await StopProcessAsync(endpoint);
        lock (state) endpoint.Error = message;
        throw new InvalidOperationException(message);
    }

    private async Task StopEndpointAsync(Endpoint endpoint)
    {
        if (endpoint.Process is not { HasExited: false } && PortInUse(endpoint.Port))
            throw new InvalidOperationException($"พอร์ต {endpoint.Port} เปิดจากโปรแกรมอื่น ให้ปิดจากโปรแกรมที่เปิดบริการ");
        await StopProcessAsync(endpoint);
    }

    private async Task StopProcessAsync(Endpoint endpoint)
    {
        Process? process = endpoint.Process;
        if (process is null) return;
        if (!process.HasExited)
        {
            try
            {
                await process.StandardInput.WriteLineAsync("stop");
                process.StandardInput.Close();
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (IOException) when (process.HasExited) { }
            catch (TimeoutException)
            {
                if (!process.HasExited) process.Kill();
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(2));
            }
        }
        lock (state)
        {
            endpoint.Process = null;
            endpoint.Ready = false;
            endpoint.Error = null;
            process.Dispose();
        }
    }

    private static bool PortInUse(int port) => IPGlobalProperties.GetIPGlobalProperties()
        .GetActiveTcpListeners().Any(listener => listener.Port == port);

    private static long? WorkingSetMiB(Process process)
    {
        try { process.Refresh(); return process.WorkingSet64 / (1024 * 1024); }
        catch (Exception error) when (error is InvalidOperationException or Win32Exception) { return null; }
    }

    private static IReadOnlyList<T> ReadRecent<T>(string path, int limit)
    {
        try
        {
            using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            if (stream.Length > 128 * 1024) stream.Seek(-128 * 1024, SeekOrigin.End);
            using StreamReader reader = new(stream);
            if (stream.Position != 0) _ = reader.ReadLine();
            Queue<T> entries = new();
            while (reader.ReadLine() is { } line)
            {
                if (line.Length > 4096) continue;
                try
                {
                    if (JsonSerializer.Deserialize<T>(line, typeof(T) == typeof(ServerEvent) ? JsonServerEvents.Options : null) is not { } entry)
                        continue;
                    if (entries.Count == limit) _ = entries.Dequeue();
                    entries.Enqueue(entry);
                }
                catch (JsonException) { }
            }
            return entries.ToArray();
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { return []; }
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await operations.WaitAsync(cancellationToken);
        try
        {
            foreach (Endpoint endpoint in endpoints.Reverse()) await StopProcessAsync(endpoint);
        }
        finally { operations.Release(); }
    }
}
