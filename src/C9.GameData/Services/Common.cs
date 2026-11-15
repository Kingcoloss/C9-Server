using System.Text.Json;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

/// <summary>Who performs a write, why, and its idempotency key. Required for every audited change (ISO/IEC 27001 A.8.15).</summary>
public sealed record OperatorContext(string Operator, string Reason, Guid RequestId)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Operator) || Operator.Length > 64)
            throw new GameDataException(GameDataError.Invalid, "Operator is required (max 64 characters)");
        if (string.IsNullOrWhiteSpace(Reason) || Reason.Length > 500)
            throw new GameDataException(GameDataError.Invalid, "Reason is required (max 500 characters)");
        if (RequestId == Guid.Empty)
            throw new GameDataException(GameDataError.Invalid, "RequestId must be a non-empty UUID");
    }
}

public enum GameDataError { Invalid, NotFound, Duplicate, Conflict, Forbidden }

public sealed class GameDataException(GameDataError code, string message) : Exception(message)
{
    public GameDataError Code { get; } = code;
}

public sealed record Page<T>(IReadOnlyList<T> Items, int Total, int Skip, int Take);

internal static class Json
{
    internal static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    internal static string Of<T>(T value) => JsonSerializer.Serialize(value, Options);
}

/// <summary>Shared write helpers: idempotency check and audit row in the same SaveChanges transaction.</summary>
internal static class Audited
{
    /// <summary>True when this RequestId was already applied; callers return the existing outcome.</summary>
    internal static Task<bool> AlreadyAppliedAsync(GameDbContext db, OperatorContext context, CancellationToken token) =>
        db.AuditLog.AsNoTracking().AnyAsync(a => a.RequestId == context.RequestId, token);

    internal static void Record(GameDbContext db, OperatorContext context, string action,
        string targetType, string targetId, object? before, object? after, TimeProvider clock) =>
        db.AuditLog.Add(new AuditEntry
        {
            At = clock.GetUtcNow(), Operator = context.Operator, Action = action,
            TargetType = targetType, TargetId = targetId,
            Before = before is null ? null : Json.Of(before), After = after is null ? null : Json.Of(after),
            Reason = context.Reason, RequestId = context.RequestId
        });

    internal static async Task SaveAsync(GameDbContext db, CancellationToken token)
    {
        try { await db.SaveChangesAsync(token); }
        catch (DbUpdateConcurrencyException)
        { throw new GameDataException(GameDataError.Conflict, "The record was changed by another operator or the game server; reload and retry"); }
        catch (DbUpdateException error) when (error.InnerException is Npgsql.PostgresException { SqlState: "23505" } unique)
        {
            if (unique.ConstraintName == "ix_audit_log_request_id")
                throw new GameDataException(GameDataError.Duplicate, "This request was already applied");
            throw new GameDataException(GameDataError.Duplicate, "A record with the same unique value already exists");
        }
    }
}
