using System.Security.Claims;
using System.Text.Json;
using C9.Admin;
using C9.Contracts;

namespace C9.Portal.Services;

/// <summary>State-changing operations shared by the REST API and the Blazor pages; each one writes an audit entry.</summary>
public sealed class PortalActions(GameServices services, ParametersService parameters, IAuditLog audit)
{
    public async Task<ControlResult> ControlAsync(ClaimsPrincipal user, string serviceId, string action, string? remote, CancellationToken cancellationToken = default)
    {
        ControlResult result;
        try { result = await services.ControlAsync(serviceId, action, cancellationToken); }
        catch (Exception error) when (GameServices.IsUnavailable(error, cancellationToken))
        {
            await AuditAsync(user, "service." + action, serviceId, false, remote, message: GameServices.Describe(error), cancellationToken: cancellationToken);
            throw;
        }
        await AuditAsync(user, "service." + action, serviceId, result.Success, remote, message: result.Message, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<bool> DisconnectAsync(ClaimsPrincipal user, string key, string? remote, CancellationToken cancellationToken = default)
    {
        bool done = await services.DisconnectAsync(key, cancellationToken);
        await AuditAsync(user, "session.disconnect", key, done, remote, message: done ? null : "session is not connected", cancellationToken: cancellationToken);
        return done;
    }

    public async Task<AdminParameters> SaveParametersAsync(ClaimsPrincipal user, AdminParameters next, string? remote, CancellationToken cancellationToken = default)
    {
        AdminParameters before = parameters.Read();
        try { parameters.Save(next); }
        catch (ArgumentException error)
        {
            await AuditAsync(user, "settings.parameters", "parameters.local.json", false, remote, message: error.Message, cancellationToken: cancellationToken);
            throw;
        }
        await AuditAsync(user, "settings.parameters", "parameters.local.json", true, remote,
            before: JsonSerializer.SerializeToElement(before, ContractJson.Options), after: JsonSerializer.SerializeToElement(next, ContractJson.Options),
            message: "Profiles updated; restart services to apply", cancellationToken: cancellationToken);
        return parameters.Read();
    }

    public Task AuditAsync(ClaimsPrincipal user, string action, string target, bool success, string? remote,
        string? reason = null, string? message = null, JsonElement? before = null, JsonElement? after = null, CancellationToken cancellationToken = default) =>
        audit.WriteAsync(new AuditEntry(Guid.NewGuid(), DateTimeOffset.UtcNow, user.ActorName(), action, target, success, reason, message, before, after, remote), cancellationToken);
}
