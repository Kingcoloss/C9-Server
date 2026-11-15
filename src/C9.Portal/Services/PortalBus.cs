using System.Runtime.CompilerServices;
using System.Threading.Channels;
using C9.Contracts;

namespace C9.Portal.Services;

/// <summary>A message from an agent's live stream, tagged with the service it came from.</summary>
public abstract record LiveMessage(string ServiceId);
public sealed record LiveFrame(string ServiceId, TraceFrame Frame) : LiveMessage(ServiceId);
public sealed record LiveSession(string ServiceId, SessionLifecycleEvent Lifecycle) : LiveMessage(ServiceId);
public sealed record LiveEvent(string ServiceId, ServiceEvent Event) : LiveMessage(ServiceId);
public sealed record LiveLink(string ServiceId, bool Connected, string? Error) : LiveMessage(ServiceId);

/// <summary>In-process broadcast of live agent messages to portal pages and modules.</summary>
public sealed class PortalBus : ISessionFeed
{
    private readonly object gate = new();
    private readonly List<Channel<LiveMessage>> subscribers = [];
    private readonly Dictionary<string, LiveLink> links = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, LiveLink> Links { get { lock (gate) return new Dictionary<string, LiveLink>(links); } }

    public void Publish(LiveMessage message)
    {
        Channel<LiveMessage>[] targets;
        lock (gate)
        {
            if (message is LiveLink link) links[link.ServiceId] = link;
            targets = subscribers.ToArray();
        }
        foreach (Channel<LiveMessage> target in targets) target.Writer.TryWrite(message);
    }

    public async IAsyncEnumerable<LiveMessage> SubscribeAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Channel<LiveMessage> channel = Channel.CreateBounded<LiveMessage>(new BoundedChannelOptions(8192)
        { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true });
        lock (gate) subscribers.Add(channel);
        try
        {
            await foreach (LiveMessage message in channel.Reader.ReadAllAsync(cancellationToken))
                yield return message;
        }
        finally
        {
            lock (gate) subscribers.Remove(channel);
        }
    }

    public async IAsyncEnumerable<SessionLifecycleEvent> SubscribeAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (LiveMessage message in SubscribeAllAsync(cancellationToken))
            if (message is LiveSession session) yield return session.Lifecycle;
    }
}
