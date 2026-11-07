using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;

namespace C9.LocalLauncher;

internal sealed class LauncherForm : Form
{
    private readonly string folder = AppContext.BaseDirectory;
    private readonly Label status = new();
    private readonly Label detail = new();
    private readonly Button play = new();
    private readonly Button check = new();
    private readonly Color gold = Color.FromArgb(232, 190, 104);
    private bool busy;
    private Process? activeGame;

    internal LauncherForm()
    {
        Text = "C9 Local Launcher";
        ClientSize = new Size(680, 520);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(17, 23, 33);
        ForeColor = Color.WhiteSmoke;
        Font = new Font("Segoe UI", 10);
        AddLabel("C9  LOCAL", new Rectangle(32, 26, 610, 47), 27, gold);
        AddLabel("เปิดเกมบนเซิร์ฟเวอร์ในเครื่อง", new Rectangle(34, 80, 610, 30), 13);
        Panel card = new() { Bounds = new Rectangle(32, 128, 616, 112), BackColor = Color.FromArgb(26, 35, 49) };
        Controls.Add(card);
        Label name = new() { Text = "CLIENT", Bounds = new Rectangle(18, 14, 570, 24), ForeColor = gold };
        Label location = new() { Text = folder.TrimEnd(Path.DirectorySeparatorChar), AutoEllipsis = true,
            Bounds = new Rectangle(18, 42, 570, 25) };
        Label note = new() { Text = "อัปเดตเกมผ่าน VFUN Launcher", Bounds = new Rectangle(18, 75, 570, 25),
            ForeColor = Color.Silver };
        card.Controls.AddRange([name, location, note]);
        status.Bounds = new Rectangle(34, 260, 612, 31);
        status.Font = new Font(Font.FontFamily, 13, FontStyle.Bold);
        detail.Bounds = new Rectangle(34, 300, 612, 48);
        detail.ForeColor = Color.Silver;
        Controls.AddRange([status, detail]);
        ConfigureButton(play, "เปิดเกม local", new Rectangle(32, 365, 300, 52), gold, Color.FromArgb(17, 23, 33));
        ConfigureButton(check, "ตรวจความพร้อม", new Rectangle(348, 365, 300, 52), Color.FromArgb(42, 54, 72), Color.WhiteSmoke);
        play.Click += async (_, _) => await StartGameAsync();
        check.Click += async (_, _) => await CheckAsync();
        Button openFolder = new(), official = new();
        ConfigureButton(openFolder, "โฟลเดอร์เกม", new Rectangle(32, 432, 200, 36), BackColor, Color.Silver);
        ConfigureButton(official, "เปิด VFUN Launcher", new Rectangle(250, 432, 244, 36), BackColor, Color.Silver);
        openFolder.Click += (_, _) => RunShell(folder);
        official.Click += (_, _) => OpenOfficial();
        AddLabel("บัญชีทดสอบ: test  •  โหมดพัฒนา local server", new Rectangle(34, 484, 612, 24), 9, Color.Gray);
        Shown += async (_, _) => await CheckAsync();
    }

    private void AddLabel(string text, Rectangle bounds, float size, Color? color = null) =>
        Controls.Add(new Label { Text = text, Bounds = bounds,
            Font = new Font(Font.FontFamily, size), ForeColor = color ?? ForeColor });

    private void ConfigureButton(Button button, string text, Rectangle bounds, Color background, Color foreground)
    {
        button.Text = text;
        button.Bounds = bounds;
        button.BackColor = background;
        button.ForeColor = foreground;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Cursor = Cursors.Hand;
        Controls.Add(button);
    }

    private void SetBusy(bool value)
    {
        busy = value;
        play.Enabled = !value && activeGame is null;
        check.Enabled = !value;
    }

    private async Task CheckAsync()
    {
        if (busy) return;
        SetBusy(true);
        try
        {
            LocalLaunchPlan plan = LocalLaunchPlan.Read(folder);
            bool ready = await ServerReadyAsync();
            status.Text = ready ? "พร้อมเปิดเกม local" : "Client พร้อม • เซิร์ฟเวอร์ยังไม่เปิด";
            status.ForeColor = ready ? Color.LightGreen : gold;
            detail.Text = ready ? $"ตรวจพบ client {plan.DisplayName} และเซิร์ฟเวอร์ในเครื่อง" :
                "กดเปิดเกมเพื่อเริ่มเซิร์ฟเวอร์ในเครื่องโดยอัตโนมัติ";
        }
        catch (Exception error) when (IsExpected(error)) { ShowError(error); }
        finally { SetBusy(false); }
    }

    private async Task StartGameAsync()
    {
        if (busy) return;
        SetBusy(true);
        try
        {
            LocalLaunchPlan plan = LocalLaunchPlan.Read(folder);
            if (Process.GetProcessesByName("C9").Length > 0)
                throw new InvalidOperationException("มีเกม C9 เปิดอยู่ กรุณาปิดเกมก่อนเปิดชุด local");
            status.Text = "กำลังเตรียมเซิร์ฟเวอร์…";
            using (JsonDocument config = ReadConfiguration())
            {
                JsonElement settings = config.RootElement;
                string loginHost = settings.GetProperty("host").GetString()!;
                int loginPort = settings.GetProperty("port").GetInt32();
                if (settings.TryGetProperty("handoff_backend_config", out _))
                    await EnsureBackendAsync(settings, "handoff_backend_config", "127.0.0.1", 14001, 1, "L9.handoff.log");
                await EnsureBackendAsync(settings, "backend_config", loginHost, loginPort, 0, "L9.server.log");
            }
            ProcessStartInfo start = new(plan.Executable) { WorkingDirectory = plan.Directory, UseShellExecute = false };
            start.ArgumentList.Add(plan.Arguments);
            Process game = Process.Start(start) ?? throw new IOException("Cannot start the selected Client");
            activeGame = game;
            Log($"started pid={game.Id} client={plan.ClientHash} endpoint=loopback");
            status.Text = "กำลังเริ่มเกม…";
            status.ForeColor = Color.LightGreen;
            detail.Text = "รอเกมเตรียมระบบ หากเกมปิดตัวลง สามารถเปิดจากหน้านี้อีกครั้งได้";
            _ = MonitorGameAsync(game);
        }
        catch (Exception error) when (IsExpected(error)) { ShowError(error); }
        finally { SetBusy(false); }
    }

    private async Task MonitorGameAsync(Process game)
    {
        using (game)
        {
            Task exited = game.WaitForExitAsync();
            if (await Task.WhenAny(exited, Task.Delay(TimeSpan.FromSeconds(30))) != exited && !IsDisposed && !Disposing)
            {
                status.Text = "กำลังรอเกมเตรียมระบบ…";
                status.ForeColor = gold;
                detail.Text = "หากรอนาน กรุณาตรวจข้อความในหน้าต่าง C9 รวมถึงข้อผิดพลาดจาก GameGuard";
            }
            await exited;
            Log($"game exited pid={game.Id} code={game.ExitCode}");
            activeGame = null;
            if (IsDisposed || Disposing) return;
            status.Text = "เกมปิดตัวลงแล้ว";
            status.ForeColor = gold;
            detail.Text = "ดูรายละเอียดการเริ่มเกมใน L9.local.log และการเชื่อมต่อใน L9.server.log";
            SetBusy(busy);
        }
    }

    private async Task<bool> ServerReadyAsync()
    {
        using JsonDocument config = ReadConfiguration();
        JsonElement settings = config.RootElement;
        return await EndpointReadyAsync(settings.GetProperty("host").GetString()!, settings.GetProperty("port").GetInt32(), 0) &&
            (!settings.TryGetProperty("handoff_backend_config", out _) || await EndpointReadyAsync("127.0.0.1", 14001, 1));
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

    private JsonDocument ReadConfiguration() => JsonDocument.Parse(File.ReadAllBytes(Path.Combine(folder, "L9.local.json")));

    private async Task EnsureBackendAsync(JsonElement config, string configKey, string host, int port, byte mode, string logName)
    {
        if (await EndpointReadyAsync(host, port, mode)) return;
        string dotnet = RequiredPath(config, "dotnet_path", "dotnet.exe");
        string assembly = RequiredPath(config, "backend_assembly", "C9.Server.dll");
        string settings = RequiredPath(config, configKey, null);
        ProcessStartInfo start = new(dotnet) { WorkingDirectory = Path.GetDirectoryName(settings)!,
            UseShellExecute = false, CreateNoWindow = true };
        start.ArgumentList.Add(assembly);
        start.ArgumentList.Add("--config");
        start.ArgumentList.Add(settings);
        start.ArgumentList.Add("--event-log");
        start.ArgumentList.Add(Path.Combine(folder, logName));
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

    private static string RequiredPath(JsonElement config, string name, string? expectedName)
    {
        string value = config.GetProperty(name).GetString() ?? "";
        if (!Path.IsPathFullyQualified(value) || !File.Exists(value) ||
            (expectedName is not null && !Path.GetFileName(value).Equals(expectedName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException("Invalid local server installation");
        return value;
    }

    private void OpenOfficial()
    {
        try
        {
            using JsonDocument config = ReadConfiguration();
            if (!config.RootElement.TryGetProperty("official_launcher", out _))
            {
                RunShell("https://vfun.valofe.com/game/download");
                return;
            }
            if (Process.GetProcessesByName("C9").Length > 0)
                throw new InvalidOperationException("กรุณาปิดเกมก่อนเปิด VFUN Launcher");
            string launcher = RequiredPath(config.RootElement, "official_launcher", null);
            using Process? process = Process.Start(new ProcessStartInfo(launcher)
                { WorkingDirectory = Path.GetDirectoryName(launcher)!, UseShellExecute = true });
        }
        catch (Exception error) when (IsExpected(error)) { ShowError(error); }
    }

    private void RunShell(string path)
    {
        try { using Process? process = Process.Start(new ProcessStartInfo(path) { WorkingDirectory = folder, UseShellExecute = true }); }
        catch (Exception error) when (IsExpected(error)) { ShowError(error); }
    }

    private void Log(string message)
    {
        try { File.AppendAllText(Path.Combine(folder, "L9.local.log"), $"{DateTimeOffset.UtcNow:O} {message}{Environment.NewLine}"); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { }
    }

    private void ShowError(Exception error)
    {
        Log($"launcher error: {error.GetType().Name}: {error.Message}");
        status.Text = "ยังเปิดเกม local ไม่ได้";
        status.ForeColor = Color.Salmon;
        detail.Text = error is InvalidDataException ? "Client มีการเปลี่ยนแปลง ต้องตรวจความเข้ากันได้ก่อนเปิด local" :
            error is InvalidOperationException ? error.Message : "กรุณาตรวจไฟล์ติดตั้งและ L9.local.log";
    }

    internal static bool IsExpected(Exception error) => error is IOException or ArgumentException or JsonException or
        InvalidOperationException or KeyNotFoundException or UnauthorizedAccessException or FormatException or System.ComponentModel.Win32Exception;
}
