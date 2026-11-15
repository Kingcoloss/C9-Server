using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using C9.Admin;
using C9.LocalLauncher;
using C9.Server;

namespace C9.AdminDesktop.WinForms;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            string? hint = typeof(Program).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(value => value.Key == "C9WorkspaceRoot")?.Value;
            string? backend = null;
            bool check = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--check") { check = true; continue; }
                if (i + 1 >= args.Length) throw new ArgumentException("Missing option value");
                switch (args[i])
                {
                    case "--workspace": hint = args[++i]; break;
                    case "--backend": backend = args[++i]; break;
                    default: throw new ArgumentException("Use [--check] [--workspace PATH] [--backend PATH]");
                }
            }
            string workspace = LocalLauncherConfiguration.FindWorkspace(AppContext.BaseDirectory, hint);
            backend ??= Path.Combine(AppContext.BaseDirectory, "game", "C9.Server.dll");
            if (!File.Exists(backend)) throw new FileNotFoundException("Publish the game backend into game/ before opening Server Manager.");
            string dotnet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "dotnet.exe");
            if (!File.Exists(dotnet)) throw new FileNotFoundException("Install .NET 10 to run the backend.");
            foreach (string profile in new[] { "lab-official-login-fixture.json", "lab-official-character-lobby.json" })
                ServerOptions.FromJson(File.ReadAllText(Path.Combine(workspace, "config", profile))).Validate();
            if (check)
            {
                Console.WriteLine(JsonSerializer.Serialize(new { status = "passed", ui = "windows_desktop", workspace, backend }));
                return 0;
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new ManagerForm(workspace, Path.GetFullPath(backend), dotnet));
            return 0;
        }
        catch (Exception error) when (error is IOException or InvalidDataException or ArgumentException or InvalidOperationException or JsonException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine(error.Message);
            if (!args.Contains("--check")) MessageBox.Show(error.Message, "C9 Server Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 2;
        }
    }
}

internal sealed class ManagerForm : Form
{
    private readonly ServerOperations operations;
    private readonly AdminSettings settings;
    private readonly string workspace;
    private readonly DataGridView services = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false, RowHeadersVisible = false, AutoGenerateColumns = true };
    private readonly TextBox activity = new() { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
    private readonly Label notice = new() { Dock = DockStyle.Fill, AutoSize = true };
    private readonly TextBox clientDirectory = new() { Width = 460, Text = "C:\\VFUN\\C9-GL" };
    private readonly NumericUpDown hp = new() { Minimum = 1, Maximum = 1000000, Width = 90 };
    private readonly NumericUpDown mp = new() { Minimum = 0, Maximum = 1000000, Width = 90 };
    private readonly System.Windows.Forms.Timer timer = new() { Interval = 3000 };
    private readonly List<Button> operationButtons = [];
    private bool busy, closing, mayClose;

    internal ManagerForm(string workspace, string backend, string dotnet)
    {
        this.workspace = workspace;
        string state = Path.Combine(workspace, "artifacts", "admin-console");
        RestoreMissingState(workspace);
        settings = new(workspace, state);
        operations = new(workspace, state, backend, dotnet, clientDirectory.Text);
        Text = "C9 Server Manager";
        ClientSize = new Size(1100, 670);
        MinimumSize = new Size(850, 560);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);
        TableLayoutPanel layout = new() { Dock = DockStyle.Fill, RowCount = 6, ColumnCount = 1, Padding = new Padding(16) };
        foreach (SizeType type in new[] { SizeType.Absolute, SizeType.Absolute, SizeType.Percent, SizeType.Absolute, SizeType.Percent, SizeType.Absolute })
            layout.RowStyles.Add(new RowStyle(type));
        layout.RowStyles[0].Height = 45; layout.RowStyles[1].Height = 45; layout.RowStyles[2].Height = 45;
        layout.RowStyles[3].Height = 45; layout.RowStyles[4].Height = 55; layout.RowStyles[5].Height = 36;
        layout.Controls.Add(new Label { Text = "C9 SERVER MANAGER  |  Local services", AutoSize = true,
            Font = new Font(Font.FontFamily, 18, FontStyle.Bold) }, 0, 0);
        FlowLayoutPanel controls = new() { Dock = DockStyle.Fill };
        foreach (var action in new[] { ("Start all", "start"), ("Stop all", "stop"), ("Restart all", "restart") })
        {
            Button button = new() { Text = action.Item1, AutoSize = true };
            button.Click += async (_, _) => await OperateAsync(action.Item2);
            operationButtons.Add(button); controls.Controls.Add(button);
        }
        Button refresh = new() { Text = "Refresh", AutoSize = true };
        refresh.Click += (_, _) => RefreshStatus(); controls.Controls.Add(refresh);
        layout.Controls.Add(controls, 0, 1); layout.Controls.Add(services, 0, 2);
        FlowLayoutPanel parameters = new() { Dock = DockStyle.Fill };
        parameters.Controls.Add(new Label { Text = "Initial HP", AutoSize = true }); parameters.Controls.Add(hp);
        parameters.Controls.Add(new Label { Text = "MP", AutoSize = true }); parameters.Controls.Add(mp);
        AdminParameters saved = settings.Read(); hp.Value = Math.Clamp(saved.InitialHp, 1, 1000000); mp.Value = Math.Clamp(saved.InitialMp, 0, 1000000);
        Button save = new() { Text = "Save (restart to apply)", AutoSize = true };
        save.Click += (_, _) =>
        {
            try { settings.Save(settings.Read() with { InitialHp = (int)hp.Value, InitialMp = (int)mp.Value }); notice.Text = "Settings saved; restart services to apply."; }
            catch (Exception error) { notice.Text = error.Message; }
        };
        parameters.Controls.Add(save); layout.Controls.Add(parameters, 0, 3); layout.Controls.Add(activity, 0, 4);
        layout.Controls.Add(notice, 0, 5); Controls.Add(layout);
        timer.Tick += (_, _) => { if (!busy) RefreshStatus(); };
        Shown += (_, _) => { RefreshStatus(); timer.Start(); };
        FormClosing += async (_, args) =>
        {
            if (mayClose) return;
            args.Cancel = true;
            if (closing) return;
            closing = true; timer.Stop(); Enabled = false;
            try { await operations.StopAsync(CancellationToken.None); }
            finally { mayClose = true; Close(); }
        };
    }

    private static void RestoreMissingState(string workspace)
    {
        foreach (var item in new[] { ("official-55350-characters.json", "official-55350-characters.local.json"),
            ("lab-characters.json", "lab-characters.local.json"), ("parameters.json", "admin-console/parameters.local.json") })
        {
            string destination = Path.Combine(workspace, "artifacts", item.Item2);
            if (File.Exists(destination)) continue;
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(Path.Combine(workspace, "config", "migration", "local-state", item.Item1), destination, overwrite: false);
        }
    }

    private async Task OperateAsync(string action)
    {
        if (busy) return;
        busy = true; operationButtons.ForEach(button => button.Enabled = false);
        try
        {
            IReadOnlyList<OperationRecord> results = await operations.ExecuteAsync(action, "all");
            notice.Text = string.Join(" | ", results.Select(result => result.Service + ": " + result.Message));
        }
        catch (Exception error) { notice.Text = error.Message; }
        finally { busy = false; operationButtons.ForEach(button => button.Enabled = true); RefreshStatus(); }
    }

    private void RefreshStatus()
    {
        try
        {
            ConsoleStatus snapshot = operations.Snapshot();
            services.DataSource = snapshot.Services.Select(service => new { Service = service.Name, service.Port,
                Status = service.State, PID = service.Pid, TCP = service.Connections, service.MemoryMiB, service.Uptime }).ToArray();
            activity.Text = string.Join(Environment.NewLine, snapshot.Services.SelectMany(service => service.Events
                .Select(entry => JsonSerializer.Serialize(entry))));
        }
        catch (Exception error) { notice.Text = error.Message; }
    }

    protected override void Dispose(bool disposing) { if (disposing) timer.Dispose(); base.Dispose(disposing); }
}
