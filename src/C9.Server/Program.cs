using System.Text.Json;
using C9.Protocol;
using C9.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

try
{
    string? configPath = null, hostOverride = null;
    bool checkRuntime = false;
    int? portOverride = null;
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] is "--help" or "-h")
        {
            Console.WriteLine("C9 .NET server\nUsage: dotnet C9.Server.dll [--config FILE] [--host LOOPBACK] [--port PORT] [--check-runtime]\nOriginal-Client compatibility is unverified; fixture replies are synthetic.");
            return 0;
        }
        if (args[i] == "--check-runtime") { checkRuntime = true; continue; }
        if (i + 1 >= args.Length) throw new ArgumentException("Missing command-line value");
        switch (args[i])
        {
            case "--config": configPath = args[++i]; break;
            case "--host": hostOverride = args[++i]; break;
            case "--port": portOverride = int.Parse(args[++i], System.Globalization.CultureInfo.InvariantCulture); break;
            default: throw new ArgumentException("Unknown command-line option");
        }
    }
    ServerOptions options = configPath is null ? new() : ServerOptions.FromJson(await File.ReadAllTextAsync(configPath));
    options = options with { Host = hostOverride ?? options.Host, Port = portOverride ?? options.Port };
    options.Validate();
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
    builder.Services.AddSingleton<IServerEvents, JsonServerEvents>();
    builder.Services.AddSingleton<TcpGameServer>();
    builder.Services.AddHostedService<ServerHostedService>();
    using IHost host = builder.Build();
    await host.RunAsync();
    return 0;
}
catch (Exception error) when (error is ArgumentException or FormatException or JsonException or IOException or InvalidOperationException or KeyNotFoundException or OverflowException or UnauthorizedAccessException or System.Net.Sockets.SocketException)
{
    Console.Error.WriteLine($"c9server: {error.GetType().Name}: {error.Message}");
    return 2;
}
