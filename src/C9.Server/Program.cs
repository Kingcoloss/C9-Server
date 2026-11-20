using System.Text.Json;
using C9.Contracts;
using C9.Protocol;
using C9.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

try
{
    string? configPath = null, hostOverride = null, clientDirectory = null, eventLog = null;
    string? serviceId = null, agentUrl = null, agentTokenFile = null, traceDirectory = null, tracePayloads = null;
    string? configOverlay = null, characterStore = null, probeUrl = null;
    int? traceRetentionDays = null;
    bool checkRuntime = false, launcherUpdate = false, managed = false, allowContainerBind = false;
    int updatePort = 18080;
    int? portOverride = null, loginPort = null;
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] is "--help" or "-h")
        {
            Console.WriteLine("""
                C9 .NET server
                Usage: dotnet C9.Server.dll [--config FILE] [--host LOOPBACK] [--port PORT] [--event-log FILE] [--check-runtime] [--managed]
                       dotnet C9.Server.dll --launcher-update --client-dir DIR [--update-port PORT] [--login-port PORT]
                --managed stops the game server on a stop command or EOF from redirected standard input.

                Agent mode (Docker / Admin Portal), enabled by --agent-url:
                  --agent-url http://0.0.0.0:9100   internal HTTP API (/healthz, /metrics, /agent/v1/*)
                  --agent-token-file FILE           shared secret with the portal (created when missing)
                  --service-id ID                   login, lobby, ... (default from profile)
                  --config-overlay FILE             portal-written profile used instead of --config when it exists
                  --character-store FILE            overrides character_store of the profile
                  --trace-dir DIR                   persist packet trace as JSONL (memory ring otherwise)
                  --trace-payloads POLICY           redact-credentials (default) | full | headers
                  --trace-retention-days N          default 7
                  --allow-container-bind            permit --host 0.0.0.0 inside a container
                       dotnet C9.Server.dll --probe URL   exit 0 when URL answers 200 (container healthcheck)
                """);
            return 0;
        }
        if (args[i] == "--check-runtime") { checkRuntime = true; continue; }
        if (args[i] == "--launcher-update") { launcherUpdate = true; continue; }
        if (args[i] == "--managed") { managed = true; continue; }
        if (args[i] == "--allow-container-bind") { allowContainerBind = true; continue; }
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
            case "--service-id": serviceId = args[++i]; break;
            case "--agent-url": agentUrl = args[++i]; break;
            case "--agent-token-file": agentTokenFile = args[++i]; break;
            case "--trace-dir": traceDirectory = args[++i]; break;
            case "--trace-payloads": tracePayloads = args[++i]; break;
            case "--trace-retention-days": traceRetentionDays = int.Parse(args[++i], System.Globalization.CultureInfo.InvariantCulture); break;
            case "--config-overlay": configOverlay = args[++i]; break;
            case "--character-store": characterStore = args[++i]; break;
            case "--probe": probeUrl = args[++i]; break;
            default: throw new ArgumentException("Unknown command-line option");
        }
    }
    if (probeUrl is not null)
    {
        if (args.Length != 2) throw new ArgumentException("--probe cannot be combined with other options");
        using HttpClient probe = new() { Timeout = TimeSpan.FromSeconds(3) };
        try { return (await probe.GetAsync(probeUrl)).IsSuccessStatusCode ? 0 : 1; }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException) { return 1; }
    }
    bool agentOptions = serviceId is not null || agentTokenFile is not null || traceDirectory is not null || tracePayloads is not null ||
        traceRetentionDays is not null || configOverlay is not null || characterStore is not null || allowContainerBind;
    if (agentOptions && agentUrl is null)
        throw new ArgumentException("Agent, trace and container options require --agent-url");
    if (launcherUpdate)
    {
        if (clientDirectory is null) throw new ArgumentException("Launcher update mode requires --client-dir DIR");
        if (checkRuntime || managed || configPath is not null || hostOverride is not null || portOverride is not null || eventLog is not null || agentUrl is not null)
            throw new ArgumentException("Launcher update mode cannot be combined with game server options");
        await LauncherUpdateServer.RunAsync(clientDirectory, updatePort, loginPort ?? 14000);
        return 0;
    }
    if (clientDirectory is not null || updatePort != 18080 || loginPort is not null)
        throw new ArgumentException("Client directory, update port and login port require --launcher-update");
    if (managed && (checkRuntime || !Console.IsInputRedirected))
        throw new ArgumentException("Managed mode requires redirected standard input and a running game endpoint");
    if (agentUrl is not null)
    {
        if (checkRuntime) throw new ArgumentException("--check-runtime cannot be combined with --agent-url");
        return await RunAgentAsync();
    }
    ServerOptions options = configPath is null ? new() : ServerOptions.FromJson(await File.ReadAllTextAsync(configPath));
    options = options with { Host = hostOverride ?? options.Host, Port = portOverride ?? options.Port };
    options.Validate();
    if (options.CharacterStore is { } store)
        options = options with { CharacterStore = Path.GetFullPath(store, Path.GetDirectoryName(Path.GetFullPath(configPath!))!) };
    if (options.NpcSpawnFile is { } npcFile)
        options = options with { NpcSpawnFile = Path.GetFullPath(npcFile, Path.GetDirectoryName(Path.GetFullPath(configPath!))!) };
    options = options.WithGameDatabaseFromEnvironment().WithLauncherAuthFromEnvironment();
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
        WatchStandardInput(lifetime.StopApplication);
    }
    await host.RunAsync();
    return 0;

    async Task<int> RunAgentAsync()
    {
        if (configPath is null) throw new ArgumentException("Agent mode requires --config FILE");
        if (agentTokenFile is null) throw new ArgumentException("Agent mode requires --agent-token-file FILE");
        if (!Uri.TryCreate(agentUrl, UriKind.Absolute, out Uri? agentAddress) ||
            !(agentAddress.IsLoopback || (allowContainerBind && agentAddress.Host is "0.0.0.0" or "[::]")))
            throw new ArgumentException(allowContainerBind
                ? "--agent-url must use a loopback address, or 0.0.0.0/[::] inside a container"
                : "--agent-url must use a loopback address (use --allow-container-bind only inside a container)");
        ConfigSource source = new(configPath, configOverlay, hostOverride, portOverride, characterStore, allowContainerBind);
        ServerOptions initial = ServerOptions.FromJson(await File.ReadAllTextAsync(configPath));
        string id = serviceId ?? initial.Profile switch { "fixture" => "login", "character_lobby" => "lobby", string other => other };
        TraceOptions traceOptions = new()
        {
            PayloadPolicy = tracePayloads ?? TracePayloadPolicies.RedactCredentials,
            Directory = traceDirectory,
            RetentionDays = traceRetentionDays ?? 7
        };
        string bootId = Convert.ToHexStringLower(System.Security.Cryptography.RandomNumberGenerator.GetBytes(3));
        await using PacketTracer tracer = new(id, bootId, traceOptions);
        ServiceRuntime runtime = new(id, tracer, bootId);
        tracer.Stream = runtime.Stream;
        IServerEvents events = new FanoutServerEvents(new JsonServerEvents(eventLog), runtime);
        AgentToken token = AgentToken.LoadOrCreate(agentTokenFile);
        await using GameServiceController controller = new(source, events, runtime);
        await using WebApplication agent = AgentHost.Create(controller, runtime, agentUrl!, token);
        await agent.StartAsync();
        events.Emit(new ServerEvent("agent_listening")
        {
            Host = string.Join(' ', agent.Urls), Profile = initial.Profile, Runtime = "dotnet",
            Reason = traceOptions.PayloadPolicy == TracePayloadPolicies.Full ? "trace payload policy FULL records credentials" : null
        });
        await controller.ExecuteAsync(ControlActions.Start);
        if (managed) WatchStandardInput(() => agent.Lifetime.StopApplication());
        await agent.WaitForShutdownAsync();
        return 0;
    }

    static void WatchStandardInput(Action stop) => _ = Task.Run(() =>
    {
        try
        {
            if (Console.ReadLine() is null or "stop") stop();
        }
        catch (IOException) { stop(); }
    });
}
catch (Exception error) when (error is ArgumentException or FormatException or JsonException or IOException or InvalidOperationException or KeyNotFoundException or OverflowException or UnauthorizedAccessException or System.Net.Sockets.SocketException or InvalidDataException)
{
    Console.Error.WriteLine($"c9server: {error.GetType().Name}: {error.Message}");
    return 2;
}
