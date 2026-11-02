using Microsoft.Extensions.Hosting;

namespace C9.Server;

public sealed class ServerHostedService(TcpGameServer server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await server.StartAsync(stoppingToken);
        await server.Completion;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await server.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
