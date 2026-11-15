using System.Security.Claims;
using System.Text.Json;
using C9.Contracts;
using C9.GameData;
using C9.GameData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace C9.Portal.GameData;

/// <summary>Request body fields common to every write: why (required) and, for edits, the row version read earlier.</summary>
public interface IWriteRequest
{
    string? Reason { get; }
}

/// <summary>
/// Shared plumbing for the Accounts and World modules (compiled into both, internal):
/// idempotency key, operator identity, error mapping and the portal audit trail.
/// </summary>
public static class GameDataApi
{
    public const string IdempotencyHeader = "Idempotency-Key";

    public static bool Configured(IServiceProvider services) =>
        services.GetService<IDbContextFactory<GameDbContext>>() is not null;

    public static IResult NotConfigured() => Results.Problem(
        title: "Game database is not configured",
        detail: $"Set {GameDataServiceCollectionExtensions.ConnectionVariable} and start the c9-db service (compose profile \"gamedata\").",
        statusCode: StatusCodes.Status503ServiceUnavailable);

    public static IResult Json(object? value, int status = StatusCodes.Status200OK) =>
        Results.Json(value, ContractJson.Options, statusCode: status);

    public static async Task<T?> Body<T>(HttpContext http) where T : class
    {
        try { return await http.Request.ReadFromJsonAsync<T>(ContractJson.Options, http.RequestAborted); }
        catch (JsonException) { return null; }
        catch (InvalidOperationException) { return null; } // Missing or non-JSON content type.
    }

    public static OperatorContext Operator(ClaimsPrincipal user, string? reason, Guid requestId) =>
        new(user.ActorName(), reason?.Trim() ?? "", requestId);

    /// <summary>
    /// Runs one audited write. Requires an Idempotency-Key (UUID) header and a reason, records the outcome in the
    /// portal audit log (success or failure), and maps <see cref="GameDataException"/> to HTTP problems.
    /// </summary>
    public static async Task<IResult> Write<T>(HttpContext http, string action, string target, IWriteRequest? body,
        Func<OperatorContext, IServiceProvider, Task<T>> work, Func<T, object?>? project = null, int status = StatusCodes.Status200OK)
    {
        IServiceProvider services = http.RequestServices;
        if (!Configured(services)) return NotConfigured();
        if (body is null) return Results.Problem("A JSON body is required", statusCode: StatusCodes.Status400BadRequest);
        if (!Guid.TryParse(http.Request.Headers[IdempotencyHeader].ToString(), out Guid requestId) || requestId == Guid.Empty)
            return Results.Problem($"Header {IdempotencyHeader} must be a UUID", statusCode: StatusCodes.Status400BadRequest);
        OperatorContext context = Operator(http.User, body.Reason, requestId);
        IAuditLog? audit = services.GetService<IAuditLog>();
        string? remote = http.Connection.RemoteIpAddress?.ToString();
        try
        {
            T result = await work(context, services);
            await Audit(audit, context, action, target, true, null, remote, http.RequestAborted);
            return Json(project is null ? result : project(result), status);
        }
        catch (GameDataException error)
        {
            await Audit(audit, context, action, target, false, error.Message, remote, http.RequestAborted);
            return Problem(error);
        }
    }

    public static IResult Problem(GameDataException error) => Results.Problem(error.Message, statusCode: error.Code switch
    {
        GameDataError.NotFound => StatusCodes.Status404NotFound,
        GameDataError.Duplicate => StatusCodes.Status409Conflict,
        GameDataError.Conflict => StatusCodes.Status412PreconditionFailed,
        GameDataError.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status400BadRequest
    }, title: error.Code.ToString());

    public static Task Audit(IAuditLog? audit, OperatorContext context, string action, string target, bool success,
        string? message, string? remote, CancellationToken token) =>
        audit is null ? Task.CompletedTask : audit.WriteAsync(new AuditEntry(context.RequestId, DateTimeOffset.UtcNow,
            context.Operator, action, target, success, context.Reason, message, RemoteAddress: remote), token);

    /// <summary>Parses an optional enum query value ("rare_drop", "RareDrop", "launcher"); false when present but unknown.</summary>
    public static bool TryEnum<TEnum>(string? value, out TEnum? result) where TEnum : struct, Enum
    {
        result = null;
        if (string.IsNullOrWhiteSpace(value)) return true;
        if (!Enum.TryParse(value.Replace("_", ""), ignoreCase: true, out TEnum parsed) || !Enum.IsDefined(parsed) ||
            value.Any(char.IsDigit)) return false;
        result = parsed;
        return true;
    }

    public static IResult BadQuery(string name) =>
        Results.Problem($"Unknown value for '{name}'", statusCode: StatusCodes.Status400BadRequest);

    public static bool HasRole(ClaimsPrincipal user, params string[] roles) => roles.Any(user.IsInRole);
}
