using System.Reflection;

namespace C9.LocalLauncher.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<LauncherApp>();
        return builder.Build();
    }
}

/// <summary>Start-up options: launcher folder, project workspace and optional command-line overrides.</summary>
public sealed record LauncherStartup(string LauncherDirectory, string? Workspace, string? ClientDirectoryOverride)
{
    public static LauncherStartup FromEnvironment()
    {
        string? workspace = typeof(LauncherStartup).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(value => value.Key == "C9WorkspaceRoot")?.Value;
        if (workspace is not null && !File.Exists(Path.Combine(workspace, "C9.slnx"))) workspace = null;
        string? client = null;
        string[] args = Environment.GetCommandLineArgs();
        for (int index = 1; index < args.Length; index++)
        {
            if (args[index] == "--client-dir" && index + 1 < args.Length) client = Path.GetFullPath(args[++index]);
            else if (args[index] == "--workspace" && index + 1 < args.Length) workspace = Path.GetFullPath(args[++index]);
        }
        return new(LocalLauncherConfiguration.LauncherHome(AppContext.BaseDirectory), workspace, client);
    }
}

public sealed class LauncherApp : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Set before the page is built: its controls take their text from LauncherText when created.
        LauncherText.Language = LauncherText.Parse(LauncherUserSettings.Load().Language);
        return new(new LauncherPage(LauncherStartup.FromEnvironment()))
        {
            Title = "C9 Local Launcher",
            Width = 980,
            Height = 700,
            MinimumWidth = 420,
            MinimumHeight = 560,
        };
    }
}
