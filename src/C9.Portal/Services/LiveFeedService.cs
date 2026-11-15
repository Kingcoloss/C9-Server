using System.Text;
using System.Text.Json;
using C9.Contracts;

namespace C9.Portal.Services;

/// <summary>
/// Keeps one server-sent-event connection open to every agent and republishes frames, session
/// lifecycle and events on the <see cref="PortalBus"/>. Reconnects with capped backoff.
/// </summary>
public sealed class LiveFeedService(GameServices services, PortalBus bus, ILogger<LiveFeedService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.WhenAll(services.Clients.Select(client => FollowAsync(client, stoppingToken)));

    private async Task FollowAsync(AgentClient client, CancellationToken stoppingToken)
    {
        TimeSpan delay = TimeSpan.FromSeconds(1);
        string id = client.Endpoint.Id;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using Stream stream = await client.OpenStreamAsync(frames: true, stoppingToken);
                bus.Publish(new LiveLink(id, true, null));
                delay = TimeSpan.FromSeconds(1);
                await ReadAsync(id, stream, stoppingToken);
                bus.Publish(new LiveLink(id, false, "stream ended"));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
            catch (Exception error) when (error is HttpRequestException or IOException or AgentException or OperationCanceledException)
            {
                bus.Publish(new LiveLink(id, false, GameServices.Describe(error)));
                logger.LogDebug("Live stream from {Service} unavailable: {Message}", id, error.Message);
            }
            try { await Task.Delay(delay, stoppingToken); }
            catch (OperationCanceledException) { return; }
            delay = TimeSpan.FromSeconds(Math.Min(15, delay.TotalSeconds * 2));
        }
    }

    private async Task ReadAsync(string serviceId, Stream stream, CancellationToken cancellationToken)
    {
        using StreamReader reader = new(stream, Encoding.UTF8);
        string? name = null;
        StringBuilder data = new();
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (line.Length == 0)
            {
                if (name is not null && data.Length > 0) Dispatch(serviceId, name, data.ToString());
                name = null;
                data.Clear();
            }
            else if (line.StartsWith("event: ", StringComparison.Ordinal)) name = line[7..];
            else if (line.StartsWith("data: ", StringComparison.Ordinal)) data.Append(line.AsSpan(6));
        }
    }

    private void Dispatch(string serviceId, string name, string json)
    {
        try
        {
            switch (name)
            {
                case StreamEvents.Frame when JsonSerializer.Deserialize<TraceFrame>(json, ContractJson.Options) is { } frame:
                    bus.Publish(new LiveFrame(serviceId, frame));
                    break;
                case StreamEvents.Session when JsonSerializer.Deserialize<SessionLifecycleEvent>(json, ContractJson.Options) is { } lifecycle:
                    bus.Publish(new LiveSession(serviceId, lifecycle));
                    break;
                case StreamEvents.Event when JsonSerializer.Deserialize<ServiceEvent>(json, ContractJson.Options) is { } value:
                    bus.Publish(new LiveEvent(serviceId, value));
                    break;
            }
        }
        catch (JsonException error) { logger.LogWarning("Ignored malformed {Event} from {Service}: {Message}", name, serviceId, error.Message); }
    }
}
