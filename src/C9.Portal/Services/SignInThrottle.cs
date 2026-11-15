using System.Collections.Concurrent;

namespace C9.Portal.Services;

/// <summary>
/// Brute-force guard for sign-in (ISO/IEC 27001 A.8.5): after <see cref="MaxFailures"/> failures from
/// one address inside <see cref="Window"/>, further attempts are refused until the oldest failure ages out.
/// </summary>
public sealed class SignInThrottle(TimeProvider? clock = null)
{
    public const int MaxFailures = 5;
    public static readonly TimeSpan Window = TimeSpan.FromMinutes(5);
    private readonly TimeProvider time = clock ?? TimeProvider.System;
    private readonly ConcurrentDictionary<string, Queue<DateTimeOffset>> failures = new(StringComparer.Ordinal);

    /// <summary>Returns how long the address must wait, or null when it may try now.</summary>
    public TimeSpan? RetryAfter(string address)
    {
        if (!failures.TryGetValue(address, out Queue<DateTimeOffset>? queue)) return null;
        lock (queue)
        {
            DateTimeOffset now = time.GetUtcNow();
            while (queue.Count > 0 && now - queue.Peek() >= Window) queue.Dequeue();
            return queue.Count >= MaxFailures ? Window - (now - queue.Peek()) : null;
        }
    }

    public void Failed(string address)
    {
        Queue<DateTimeOffset> queue = failures.GetOrAdd(address, _ => new Queue<DateTimeOffset>());
        lock (queue)
        {
            queue.Enqueue(time.GetUtcNow());
            while (queue.Count > MaxFailures) queue.Dequeue();
        }
        if (failures.Count > 4096) failures.Clear();
    }

    public void Succeeded(string address) => failures.TryRemove(address, out _);
}
