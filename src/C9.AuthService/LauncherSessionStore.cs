using System.Collections.Concurrent;
using System.Security.Cryptography;
using C9.Contracts;

namespace C9.AuthService;

/// <summary>
/// In-memory launcher sessions and one-time game tickets. Only SHA-256 digests of tokens are kept, so a
/// memory dump or log line never yields a usable token. Restarting c9-auth signs every launcher out.
/// </summary>
public sealed class LauncherSessionStore(TimeProvider clock)
{
    public static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(12);
    public static readonly TimeSpan TicketLifetime = TimeSpan.FromMinutes(2);
    /// <summary>A game handoff lives as long as a launcher session: the client reuses it for every lobby reconnect.</summary>
    public static readonly TimeSpan HandoffLifetime = SessionLifetime;
    private const int MaxEntries = 10_000;

    private sealed record Entry(string Login, long AccountId, DateTimeOffset ExpiresAt);

    private readonly ConcurrentDictionary<string, Entry> sessions = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, Entry> tickets = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, Entry> handoffs = new(StringComparer.Ordinal);

    public int ActiveSessions => sessions.Count;

    public LauncherSession CreateSession(string login, long accountId)
    {
        Prune();
        if (sessions.Count >= MaxEntries) throw new InvalidOperationException("Too many launcher sessions");
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        DateTimeOffset expires = clock.GetUtcNow() + SessionLifetime;
        sessions[Digest(token)] = new(login, accountId, expires);
        return new(login, accountId, token, expires);
    }

    public bool RevokeSession(string token) => sessions.TryRemove(Digest(token), out _);

    /// <summary>Issues a single-use ticket for a live session, or null when the session is unknown or expired.</summary>
    public LauncherTicket? IssueTicket(string sessionToken)
    {
        if (!sessions.TryGetValue(Digest(sessionToken), out Entry? session) || session.ExpiresAt <= clock.GetUtcNow())
        {
            sessions.TryRemove(Digest(sessionToken), out _);
            return null;
        }
        Prune();
        if (tickets.Count >= MaxEntries) throw new InvalidOperationException("Too many pending tickets");
        byte[] raw = RandomNumberGenerator.GetBytes(LauncherAuthRoutes.TicketBytes);
        DateTimeOffset expires = clock.GetUtcNow() + TicketLifetime;
        tickets[Digest(raw)] = session with { ExpiresAt = expires };
        return new(session.Login, session.AccountId, Convert.ToBase64String(raw), expires);
    }

    /// <summary>Redeems a ticket exactly once. Unknown, malformed, reused or expired tickets return null.</summary>
    public TicketOwner? ConsumeTicket(string ticket)
    {
        byte[] raw;
        try { raw = Convert.FromBase64String(ticket); }
        catch (FormatException) { return null; }
        if (raw.Length != LauncherAuthRoutes.TicketBytes) return null;
        if (!tickets.TryRemove(Digest(raw), out Entry? entry) || entry.ExpiresAt <= clock.GetUtcNow()) return null;
        return new(entry.Login, entry.AccountId);
    }

    /// <summary>
    /// Creates the 64-bit game handoff key for a redeemed ticket (never 0, which the client sends when it has none).
    /// Only its digest is stored; it stays valid for <see cref="HandoffLifetime"/>.
    /// </summary>
    public ulong CreateHandoff(TicketOwner owner)
    {
        Prune();
        if (handoffs.Count >= MaxEntries) throw new InvalidOperationException("Too many game handoffs");
        ulong key;
        do key = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)); while (key == 0);
        handoffs[Digest(owner.AccountId, key)] = new(owner.Login, owner.AccountId, clock.GetUtcNow() + HandoffLifetime);
        return key;
    }

    /// <summary>Looks up a handoff (not single use). Unknown, expired, zero or other-account keys return null.</summary>
    public TicketOwner? ResolveHandoff(long accountId, ulong key)
    {
        if (key == 0 || accountId <= 0) return null;
        string digest = Digest(accountId, key);
        if (!handoffs.TryGetValue(digest, out Entry? entry)) return null;
        if (entry.ExpiresAt <= clock.GetUtcNow()) { handoffs.TryRemove(digest, out _); return null; }
        return entry.AccountId == accountId ? new(entry.Login, entry.AccountId) : null;
    }

    private void Prune()
    {
        DateTimeOffset now = clock.GetUtcNow();
        foreach ((string key, Entry entry) in handoffs) if (entry.ExpiresAt <= now) handoffs.TryRemove(key, out _);
        foreach ((string key, Entry entry) in sessions) if (entry.ExpiresAt <= now) sessions.TryRemove(key, out _);
        foreach ((string key, Entry entry) in tickets) if (entry.ExpiresAt <= now) tickets.TryRemove(key, out _);
    }

    private static string Digest(string token) => Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
    private static string Digest(byte[] raw) => "t:" + Convert.ToHexString(SHA256.HashData(raw));
    private static string Digest(long accountId, ulong key) =>
        "h:" + Convert.ToHexString(SHA256.HashData(System.Text.Encoding.ASCII.GetBytes($"{accountId}:{key:x16}")));
}
