using C9.Contracts;

namespace C9.Server;

/// <summary>Fixed-size, thread-safe ring of the most recent frames, ordered by id.</summary>
public sealed class TraceRing(int capacity)
{
    private readonly TraceFrame?[] items = new TraceFrame?[capacity];
    private readonly object gate = new();
    private long count;

    public int Capacity => items.Length;

    public void Add(TraceFrame frame)
    {
        lock (gate)
        {
            items[count % items.Length] = frame;
            count++;
        }
    }

    public long OldestId
    {
        get
        {
            lock (gate)
            {
                if (count == 0) return long.MaxValue;
                return items[count <= items.Length ? 0 : count % items.Length]!.Id;
            }
        }
    }

    /// <summary>Copies frames in id order, oldest first.</summary>
    public TraceFrame[] Snapshot()
    {
        lock (gate)
        {
            int length = (int)Math.Min(count, items.Length);
            TraceFrame[] result = new TraceFrame[length];
            long start = count - length;
            for (int i = 0; i < length; i++) result[i] = items[(start + i) % items.Length]!;
            return result;
        }
    }

    public static bool Matches(TraceFrame frame, TraceQuery query) =>
        (query.Session is null || frame.Session == query.Session) &&
        (query.Direction is null || frame.Direction == query.Direction) &&
        (query.Types is not { Count: > 0 } types || (frame.Type is { } type && types.Contains(type))) &&
        (!query.ErrorsOnly || frame.Error is not null) &&
        (query.SinceUtc is null || frame.TimeUtc >= query.SinceUtc) &&
        (query.AfterId is null || frame.Id > query.AfterId) &&
        (query.BeforeId is null || frame.Id < query.BeforeId);

    /// <summary>
    /// With <see cref="TraceQuery.AfterId"/> returns the first matches after the cursor (live tail);
    /// otherwise the newest matches before <see cref="TraceQuery.BeforeId"/>. Always id ascending.
    /// </summary>
    public static TracePage Page(IEnumerable<TraceFrame> ordered, TraceQuery query)
    {
        int limit = Math.Clamp(query.Limit, 1, TraceQuery.MaxLimit);
        if (query.AfterId is not null)
        {
            List<TraceFrame> forward = ordered.Where(f => Matches(f, query)).Take(limit + 1).ToList();
            bool more = forward.Count > limit;
            if (more) forward.RemoveAt(forward.Count - 1);
            return new TracePage(forward, forward.Count > 0 ? forward[^1].Id : query.AfterId, more);
        }
        Queue<TraceFrame> tail = new();
        bool truncated = false;
        foreach (TraceFrame frame in ordered)
        {
            if (!Matches(frame, query)) continue;
            tail.Enqueue(frame);
            if (tail.Count > limit) { tail.Dequeue(); truncated = true; }
        }
        TraceFrame[] page = tail.ToArray();
        return new TracePage(page, page.Length > 0 ? page[^1].Id : null, truncated);
    }
}
