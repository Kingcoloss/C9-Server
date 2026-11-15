using System.Text;
using System.Text.Json;
using C9.Contracts;

namespace C9.Portal.Services;

/// <summary>Default append-only audit trail: one JSON object per line in {state}/audit/portal-audit.jsonl.</summary>
public sealed class JsonlAuditLog(string directory) : IAuditLog
{
    private readonly SemaphoreSlim gate = new(1, 1);
    private readonly string path = Path.Combine(Directory.CreateDirectory(directory).FullName, "portal-audit.jsonl");

    public async Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        string line = JsonSerializer.Serialize(entry, ContractJson.Options) + "\n";
        await gate.WaitAsync(cancellationToken);
        try
        {
            await using FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            await stream.WriteAsync(Encoding.UTF8.GetBytes(line), cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
        finally { gate.Release(); }
    }

    public async Task<IReadOnlyList<AuditEntry>> QueryAsync(AuditQuery query, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path)) return [];
        List<AuditEntry> matches = [];
        await using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new(stream, Encoding.UTF8);
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            AuditEntry? entry;
            try { entry = JsonSerializer.Deserialize<AuditEntry>(line, ContractJson.Options); }
            catch (JsonException) { continue; }
            if (entry is null ||
                (query.Actor is not null && entry.Actor != query.Actor) ||
                (query.Action is not null && !entry.Action.StartsWith(query.Action, StringComparison.Ordinal)) ||
                (query.Target is not null && !entry.Target.Contains(query.Target, StringComparison.OrdinalIgnoreCase)) ||
                (query.SinceUtc is not null && entry.TimeUtc < query.SinceUtc)) continue;
            matches.Add(entry);
        }
        return matches.OrderByDescending(e => e.TimeUtc).Take(Math.Clamp(query.Limit, 1, 5000)).ToArray();
    }
}
