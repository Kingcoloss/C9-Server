using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;

namespace C9.LocalLauncher;

public enum LauncherTone { Neutral, Good, Warning, Error }

public sealed record LauncherStatus(LauncherTone Tone, string Title, string Detail, string? ClientBuild = null, string? ClientHash = null);

/// <summary>
/// UI-independent launch workflow for L9 front ends: verifies the local configuration and Client hash,
/// starts the loopback backends when needed and starts C9.exe, asking Windows for elevation because the
/// official Client manifest is requireAdministrator.
/// </summary>
public sealed class LocalLauncherRuntime
{
    private const int ElevationCancelled = 1223; // ERROR_CANCELLED from ShellExecuteEx when UAC is declined
    private const int HandoffPort = 14001;
    private readonly string? workspace;
    private Process? activeGame;

    public LocalLauncherRuntime(string configurationDirectory, string? workspace)
    {
        ConfigurationDirectory = Path.GetFullPath(configurationDirectory);
        this.workspace = workspace;
    }

    public string ConfigurationDirectory { get; }
    public string LogPath => Path.Combine(ConfigurationDirectory, "L9.local.log");
    public bool GameRunning => activeGame is not null;
    public static bool IsElevated => Environment.IsPrivilegedProcess;

    /// <summary>Raised when the game process reports progress after <see cref="StartGameAsync"/> returned.</summary>
    public event Action<LauncherStatus>? StatusChanged;

    public async Task<LauncherStatus> CheckAsync(string gameDirectory, bool compatibilityTest)
    {
        try
        {
            LocalLaunchPlan plan = Prepare(gameDirectory, compatibilityTest);
            if (activeGame is not null)
                return new(LauncherTone.Good, "เกมกำลังทำงาน", "ปิดเกมก่อนหากต้องการเปลี่ยนโฟลเดอร์หรือโหมด", plan.DisplayName, plan.ClientHash);
            bool ready = await ServerReadyAsync();
            return ready
                ? new(LauncherTone.Good, "พร้อมเปิดเกม local", $"ตรวจพบ client {plan.DisplayName} และเซิร์ฟเวอร์ในเครื่อง", plan.DisplayName, plan.ClientHash)
                : new(LauncherTone.Warning, "Client พร้อม • เซิร์ฟเวอร์ยังไม่เปิด", "กดเปิดเกมเพื่อเริ่มเซิร์ฟเวอร์ในเครื่องโดยอัตโนมัติ", plan.DisplayName, plan.ClientHash);
        }
        catch (Exception error) when (IsExpected(error)) { return Failure(error); }
    }

    /// <param name="progress">Receives intermediate steps (server start, UAC prompt).</param>
    public async Task<LauncherStatus> StartGameAsync(string gameDirectory, bool compatibilityTest, IProgress<LauncherStatus>? progress = null,
        LaunchIdentity? identity = null)
    {
        if (activeGame is not null)
            return new(LauncherTone.Warning, "เกมกำลังทำงานอยู่", "ปิดเกมเดิมก่อนเปิดใหม่");
        try
        {
            LocalLaunchPlan plan = Prepare(gameDirectory, compatibilityTest, identity);
            if (ClientRunning())
                throw new InvalidOperationException("มีเกม C9 เปิดอยู่ กรุณาปิดเกมก่อนเปิดชุด local");
            progress?.Report(new(LauncherTone.Neutral, "กำลังเตรียมเซิร์ฟเวอร์…", "เริ่ม login และ character lobby ในเครื่อง"));
            using (JsonDocument config = ReadConfiguration())
            {
                JsonElement settings = config.RootElement;
                string loginHost = settings.GetProperty("host").GetString()!;
                int loginPort = settings.GetProperty("port").GetInt32();
                if (settings.TryGetProperty("handoff_backend_config", out _))
                    await EnsureBackendAsync(settings, "handoff_backend_config", "127.0.0.1", HandoffPort, 1, "L9.handoff.log");
                await EnsureBackendAsync(settings, "backend_config", loginHost, loginPort, 0, "L9.server.log");
            }
            if (!IsElevated)
                progress?.Report(new(LauncherTone.Neutral, "รอการอนุญาตสิทธิ์ผู้ดูแลระบบ…",
                    "C9.exe ต้องใช้สิทธิ์ Administrator กรุณากด Yes ในหน้าต่าง UAC"));
            Process game = StartClient(plan);
            activeGame = game;
            // The ticket itself is never logged; it is single-use and expires within two minutes.
            Log($"started pid={game.Id} client={plan.ClientHash} endpoint=loopback elevated_launcher={IsElevated} compatibility_test={plan.CompatibilityTest} login={identity?.Login ?? "(local lab)"}");
            _ = MonitorGameAsync(game);
            return new(LauncherTone.Good, "กำลังเริ่มเกม…", "รอเกมเตรียมระบบ หากเกมปิดตัวลง สามารถเปิดจากหน้านี้อีกครั้งได้",
                plan.DisplayName, plan.ClientHash);
        }
        catch (Win32Exception error) when (error.NativeErrorCode == ElevationCancelled)
        {
            Log("game start cancelled at the elevation prompt");
            return new(LauncherTone.Warning, "ยังไม่ได้รับสิทธิ์ผู้ดูแลระบบ",
                "C9.exe ต้องใช้สิทธิ์ Administrator กดเปิดเกมอีกครั้งแล้วเลือก Yes ในหน้าต่าง UAC");
        }
        catch (Exception error) when (IsExpected(error)) { return Failure(error); }
    }

    public LauncherStatus? OpenOfficialLauncher()
    {
        try
        {
            if (ClientRunning()) throw new InvalidOperationException("กรุณาปิดเกมก่อนเปิด VFUN Launcher");
            string? launcher = LocalLauncherConfiguration.FindOfficialLauncher(ConfigurationDirectory);
            if (launcher is null) return OpenShell("https://vfun.valofe.com/game/download");
            using Process? process = Process.Start(new ProcessStartInfo(launcher)
                { WorkingDirectory = Path.GetDirectoryName(launcher)!, UseShellExecute = true });
            return null;
        }
        catch (Exception error) when (IsExpected(error)) { return Failure(error); }
    }

    public LauncherStatus? OpenShell(string path, string? workingDirectory = null)
    {
        try
        {
            using Process? process = Process.Start(new ProcessStartInfo(path)
                { WorkingDirectory = workingDirectory ?? ConfigurationDirectory, UseShellExecute = true });
            return null;
        }
        catch (Exception error) when (IsExpected(error)) { return Failure(error); }
    }

    private LocalLaunchPlan Prepare(string gameDirectory, bool compatibilityTest, LaunchIdentity? identity = null)
    {
        string? problem = LauncherUserSettings.ValidateGameDirectory(gameDirectory);
        if (problem is not null) throw new DirectoryNotFoundException(problem);
        string folder = LauncherUserSettings.NormalizeDirectory(gameDirectory);
        LocalConfigurationResult result = LocalLauncherConfiguration.Ensure(folder, workspace,
            Path.Combine(ConfigurationDirectory, LocalLauncherConfiguration.FileName));
        using (JsonDocument config = ReadConfiguration()) LocalLauncherConfiguration.Verify(config.RootElement);
        if (result.Created) Log("created and verified local configuration from project defaults");
        return LocalLaunchPlan.Read(folder, ConfigurationDirectory, compatibilityTest, identity);
    }

    private static Process StartClient(LocalLaunchPlan plan)
    {
        // The launch argument is a single token without spaces or quotes, so Arguments matches the
        // previous ArgumentList form exactly. ShellExecute with "runas" shows the UAC prompt; an already
        // elevated launcher starts the Client directly.
        ProcessStartInfo start = new(plan.Executable) { WorkingDirectory = plan.Directory, Arguments = plan.Arguments };
        if (IsElevated) start.UseShellExecute = false;
        else
        {
            start.UseShellExecute = true;
            start.Verb = "runas";
        }
        return Process.Start(start) ?? throw new IOException("Cannot start the selected Client");
    }

    private async Task MonitorGameAsync(Process game)
    {
        using (game)
        {
            int id = game.Id;
            Task exited = game.WaitForExitAsync();
            if (await Task.WhenAny(exited, Task.Delay(TimeSpan.FromSeconds(30))) != exited)
                StatusChanged?.Invoke(new(LauncherTone.Warning, "กำลังรอเกมเตรียมระบบ…",
                    "หากรอนาน กรุณาตรวจข้อความในหน้าต่าง C9 รวมถึงข้อผิดพลาดจาก GameGuard"));
            await exited;
            string code;
            try { code = game.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture); }
            catch (Exception error) when (error is InvalidOperationException or Win32Exception) { code = "unknown"; }
            Log($"game exited pid={id} code={code}");
            activeGame = null;
            StatusChanged?.Invoke(new(LauncherTone.Warning, "เกมปิดตัวลงแล้ว",
                "ดูรายละเอียดการเริ่มเกมใน L9.local.log และการเชื่อมต่อใน L9.server.log"));
        }
    }

    private static bool ClientRunning()
    {
        Process[] running = Process.GetProcessesByName("C9");
        foreach (Process process in running) process.Dispose();
        return running.Length > 0;
    }

    private async Task<bool> ServerReadyAsync()
    {
        using JsonDocument config = ReadConfiguration();
        JsonElement settings = config.RootElement;
        return await EndpointReadyAsync(settings.GetProperty("host").GetString()!, settings.GetProperty("port").GetInt32(), 0) &&
            (!settings.TryGetProperty("handoff_backend_config", out _) || await EndpointReadyAsync("127.0.0.1", HandoffPort, 1));
    }

    private static async Task<bool> EndpointReadyAsync(string host, int port, byte mode)
    {
        try
        {
            using CancellationTokenSource limit = new(TimeSpan.FromMilliseconds(700));
            using TcpClient client = new();
            await client.ConnectAsync(host, port, limit.Token);
            byte[] greeting = new byte[14];
            await client.GetStream().ReadExactlyAsync(greeting, limit.Token);
            return greeting.AsSpan().SequenceEqual(new byte[] { 14, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, mode });
        }
        catch (Exception error) when (error is SocketException or IOException or OperationCanceledException) { return false; }
    }

    private JsonDocument ReadConfiguration() => LocalLauncherConfiguration.Read(ConfigurationDirectory);

    private async Task EnsureBackendAsync(JsonElement config, string configKey, string host, int port, byte mode, string logName)
    {
        if (await EndpointReadyAsync(host, port, mode)) return;
        string dotnet = LocalLauncherConfiguration.RequiredFile(config, "dotnet_path", "dotnet.exe");
        string assembly = LocalLauncherConfiguration.RequiredFile(config, "backend_assembly", "C9.Server.dll");
        string settings = LocalLauncherConfiguration.RequiredFile(config, configKey, null);
        ProcessStartInfo start = new(dotnet) { WorkingDirectory = Path.GetDirectoryName(settings)!,
            UseShellExecute = false, CreateNoWindow = true };
        start.ArgumentList.Add(assembly);
        start.ArgumentList.Add("--config");
        start.ArgumentList.Add(settings);
        start.ArgumentList.Add("--event-log");
        start.ArgumentList.Add(Path.Combine(ConfigurationDirectory, logName));
        using Process server = Process.Start(start) ?? throw new IOException("Cannot start local server");
        Log($"server started pid={server.Id} port={port}");
        for (int attempt = 0; attempt < 20; attempt++)
        {
            await Task.Delay(250);
            if (await EndpointReadyAsync(host, port, mode)) return;
            if (server.HasExited) break;
        }
        throw new InvalidOperationException($"เปิดเซิร์ฟเวอร์พอร์ต {port} ไม่สำเร็จ กรุณาตรวจ {logName}");
    }

    public void Log(string message)
    {
        try { File.AppendAllText(LogPath, $"{DateTimeOffset.UtcNow:O} {message}{Environment.NewLine}"); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { }
    }

    private LauncherStatus Failure(Exception error)
    {
        Log($"launcher error: {error.GetType().Name}: {error.Message}");
        return new(LauncherTone.Error, "ยังเปิดเกม local ไม่ได้", Describe(error));
    }

    public static string Describe(Exception error) => error switch
    {
        DirectoryNotFoundException => error.Message,
        InvalidDataException when error.Message.Contains("compatibility profile", StringComparison.Ordinal) =>
            "C9.exe รุ่นนี้ยังไม่มี profile ที่รองรับ หากเป็น Client รุ่น 14 พ.ย. ให้เปิด \"ทดสอบ Client รุ่นใหม่ (Local)\"",
        InvalidDataException or InvalidOperationException => error.Message,
        FileNotFoundException => error.Message,
        Win32Exception => $"Windows ไม่อนุญาตให้เปิดโปรแกรม: {error.Message}",
        _ => "กรุณาตรวจไฟล์ติดตั้งและ L9.local.log",
    };

    public static bool IsExpected(Exception error) => error is IOException or InvalidDataException or ArgumentException or JsonException or
        InvalidOperationException or KeyNotFoundException or UnauthorizedAccessException or FormatException or Win32Exception;
}
