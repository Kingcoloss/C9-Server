using System.Text.Json;
using System.Threading.Channels;
using C9.Contracts;

namespace C9.Server;

/// <summary>One server-sent event: name, JSON data and the session it belongs to (if any).</summary>
public sealed record StreamMessage(string Event, string Json, string? Session);

/// <summary>Fans live frames, session lifecycle and events out to SSE subscribers. Slow readers lose the oldest messages.</summary>
public sealed class AgentStreamHub
{
    private readonly object gate = new();
    private readonly List<Subscription> subscribers = [];

    public sealed class Subscription : IDisposable
    {
        private readonly AgentStreamHub hub;
        internal Subscription(AgentStreamHub hub, string? session, bool frames)
        {
            this.hub = hub;
            Session = session;
            Frames = frames;
        }
        internal Channel<StreamMessage> Channel { get; } = System.Threading.Channels.Channel.CreateBounded<StreamMessage>(
            new BoundedChannelOptions(4096) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true });
        public string? Session { get; }
        public bool Frames { get; }
        public ChannelReader<StreamMessage> Reader => Channel.Reader;
        public void Dispose() => hub.Remove(this);
    }

    public int SubscriberCount { get { lock (gate) return subscribers.Count; } }

    public Subscription Subscribe(string? session, bool frames)
    {
        Subscription subscription = new(this, session, frames);
        lock (gate) subscribers.Add(subscription);
        return subscription;
    }

    private void Remove(Subscription subscription)
    {
        lock (gate) subscribers.Remove(subscription);
        subscription.Channel.Writer.TryComplete();
    }

    public void Publish<T>(string name, T value, string? session)
    {
        Subscription[] targets;
        lock (gate)
        {
            if (subscribers.Count == 0) return;
            targets = subscribers.ToArray();
        }
        StreamMessage? message = null;
        foreach (Subscription target in targets)
        {
            if (name == StreamEvents.Frame && !target.Frames) continue;
            if (target.Session is not null && session is not null && target.Session != session) continue;
            message ??= new StreamMessage(name, JsonSerializer.Serialize(value, ContractJson.Options), session);
            target.Channel.Writer.TryWrite(message);
        }
    }
}
