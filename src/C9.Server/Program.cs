using System.Text.Json;
using C9.Protocol;
using C9.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

try
{
    string? configPath = null, hostOverride = null, clientDirectory = null, eventLog = null;
    bool checkRuntime = false, launcherUpdate = false, managed = false;
    int updatePort = 18080;
    int? portOverride = null, loginPort = null;
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] is "--help" or "-h")
        {
            Console.WriteLine("C9 .NET server\nUsage: dotnet C9.Server.dll [--config FILE] [--host LOOPBACK] [--port PORT] [--event-log FILE] [--check-runtime] [--managed]\n       dotnet C9.Server.dll --launcher-update --client-dir DIR [--update-port PORT] [--login-port PORT]\n--managed stops the game server on a stop command or EOF from redirected standard input.");
            return 0;
        }
        if (args[i] == "--check-runtime") { checkRuntime = true; continue; }
        if (args[i] == "--launcher-update") { launcherUpdate = true; continue; }
        if (args[i] == "--managed") { managed = true; continue; }
        if (i + 1 >= args.Length) throw new ArgumentException("Missing command-line value");
        switch (args[i])
        {
            case "--config": configPath = args[++i]; break;
            case "--event-log": eventLog = args[++i]; break;
            case "--host": hostOverride = args[++i]; break;
            case "--port": portOverride = int.Parse(args[++i], System.Globalization.CultureInfo.InvariantCulture); break;
            case "--client-dir": clientDirectory = args[++i]; break;
            case "--update-port": updatePort = int.Parse(args[++i], System.Globalization.CultureInfo.InvariantCulture); break;
            case "--login-port": loginPort = int.Parse(args[++i], System.Globalization.CultureInfo.InvariantCulture); break;
            default: throw new ArgumentException("Unknown command-line option");
        }
    }
    if (launcherUpdate)
    {
        if (clientDirectory is null) throw new ArgumentException("Launcher update mode requires --client-dir DIR");
        if (checkRuntime || managed || configPath is not null || hostOverride is not null || portOverride is not null || eventLog is not null)
            throw new ArgumentException("Launcher update mode cannot be combined with game server options");
        await LauncherUpdateServer.RunAsync(clientDirectory, updatePort, loginPort ?? 14000);
        return 0;
    }
    if (clientDirectory is not null || updatePort != 18080 || loginPort is not null)
        throw new ArgumentException("Client directory, update port and login port require --launcher-update");
    if (managed && (checkRuntime || !Console.IsInputRedirected))
        throw new ArgumentException("Managed mode requires redirected standard input and a running game endpoint");
    ServerOptions options = configPath is null ? new() : ServerOptions.FromJson(await File.ReadAllTextAsync(configPath));
    options = options with { Host = hostOverride ?? options.Host, Port = portOverride ?? options.Port };
    options.Validate();
    if (options.CharacterStore is { } store)
        options = options with { CharacterStore = Path.GetFullPath(store, Path.GetDirectoryName(Path.GetFullPath(configPath!))!) };
    if (checkRuntime)
    {
        _ = new ConnectionCodec(options.MaxFrame);
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            @event = "runtime_check", status = "passed", runtime = "dotnet", profile = options.Profile,
            cipher_epoch_limit = InitialCipherDirection.FrameLimit, python_required = false, client_files_required = false
        }));
        return 0;
    }
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { Args = [] });
    builder.Logging.ClearProviders();
    builder.Services.AddSingleton(options);
    builder.Services.AddSingleton<IServerEvents>(new JsonServerEvents(eventLog));
    builder.Services.AddSingleton<TcpGameServer>();
    builder.Services.AddHostedService<ServerHostedService>();
    using IHost host = builder.Build();
    if (managed)
    {
        IHostApplicationLifetime lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        _ = Task.Run(() =>
        {
            try
            {
                if (Console.ReadLine() is null or "stop") lifetime.StopApplication();
            }
            catch (IOException) { lifetime.StopApplication(); }
        });
    }
    await host.RunAsync();
    return 0;
}
catch (Exception error) when (error is ArgumentException or FormatException or JsonException or IOException or InvalidOperationException or KeyNotFoundException or OverflowException or UnauthorizedAccessException or System.Net.Sockets.SocketException)
{
    Console.Error.WriteLine($"c9server: {error.GetType().Name}: {error.Message}");
    return 2;
}
