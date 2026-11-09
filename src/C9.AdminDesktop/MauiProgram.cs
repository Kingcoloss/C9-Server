using C9.Admin;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace C9.AdminDesktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<ManagerApp>();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton(_ => DesktopWorkspace.Locate());
        return builder.Build();
    }
}

public sealed class ManagerApp : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        BlazorWebView view = new() { HostPage = "wwwroot/index.html" };
        view.RootComponents.Add(new RootComponent { Selector = "#app", ComponentType = typeof(ConsoleHome) });
        return new Window(new ContentPage { Content = view })
        {
            Title = "C9 Server Manager",
            Width = 1240,
            Height = 880,
            MinimumWidth = 650,
            MinimumHeight = 460
        };
    }
}

public sealed class DesktopWorkspace
{
    public ServerOperations Operations { get; }
    public AdminSettings Settings { get; }
    public PlayerAdministration Players { get; }

    private DesktopWorkspace(string workspace, string dotnet)
    {
        string state = Path.Combine(workspace, "artifacts", "admin-console");
        string backend = Path.Combine(AppContext.BaseDirectory, "game", "C9.Server.dll");
        string? clientDirectory = Environment.GetEnvironmentVariable("C9_CLIENT_DIRECTORY");
        Operations = new(workspace, state, backend, dotnet, clientDirectory);
        Settings = new(workspace, state);
        Players = new(Settings, state);
    }

    internal static DesktopWorkspace Locate()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "C9.slnx")))
            directory = directory.Parent;
        if (directory is null)
            throw new InvalidOperationException("เปิด C9 Server Manager จากโฟลเดอร์ที่ build ไว้ใน C9-Server");

        string? dotnet = new[]
        {
            Environment.GetEnvironmentVariable("DOTNET_ROOT"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dotnet"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet")
        }.OfType<string>().Where(Path.IsPathFullyQualified)
            .Select(root => Path.Combine(root, "dotnet.exe")).FirstOrDefault(File.Exists);
        return new(directory.FullName, dotnet ?? throw new InvalidOperationException("กรุณาติดตั้ง .NET 10 SDK"));
    }
}
