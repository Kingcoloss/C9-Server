using System.Text.Json;

namespace C9.Portal;

/// <summary>
/// One administrative action. Write an entry for every change made through the portal (ISO/IEC 27001
/// A.8.15 logging): who, when, what, target, reason, result, and before/after values when they exist.
/// Never put secrets or credential bytes in <see cref="Before"/>/<see cref="After"/>.
/// </summary>
public sealed record AuditEntry(
    Guid Id,
    DateTimeOffset TimeUtc,
    string Actor,
    string Action,
    string Target,
    bool Success,
    string? Reason = null,
    string? Message = null,
    JsonElement? Before = null,
    JsonElement? After = null,
    string? RemoteAddress = null);

public sealed record AuditQuery(string? Actor = null, string? Action = null, string? Target = null,
    DateTimeOffset? SinceUtc = null, int Limit = 200);

/// <summary>Append-only audit trail. Default: JSONL in the portal state directory; may be replaced by a DB implementation.</summary>
public interface IAuditLog
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditEntry>> QueryAsync(AuditQuery query, CancellationToken cancellationToken = default);
}
