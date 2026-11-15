using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using C9.Admin;
using C9.Contracts;
using C9.Portal.Services;
using C9.Server;

namespace C9.Portal.Api;

/// <summary>
/// Core REST API under /api/v1 (see docs/SERVER_PLATFORM_ARCHITECTURE_TH.md). Every endpoint needs a
/// signed-in portal user (cookie or "Authorization: Bearer &lt;operator key&gt;"); changes are audited.
/// </summary>
public static class PortalApi
{
    public static void MapCoreApi(this RouteGroupBuilder api)
    {
        JsonSerializerOptions json = ContractJson.Options;

        api.MapGet("/me", (ClaimsPrincipal user) => Results.Json(new
        {
            id = user.ActorName(), name = user.Identity?.Name,
            roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
        }, json));

        // Services
        api.MapGet("/services", async (GameServices services, CancellationToken ct) =>
            Results.Json(await services.SnapshotAsync(ct), json)).RequireAuthorization(PortalPolicies.ServerRead);
        api.MapGet("/services/{id}", async (string id, GameServices services, CancellationToken ct) =>
            services.Endpoints.Any(e => e.Id == id) ? Results.Json(await services.SnapshotAsync(id, ct), json) : Results.NotFound())
            .RequireAuthorization(PortalPolicies.ServerRead);
        api.MapPost("/services/{id}/control", async (string id, ControlRequest request, GameServices services, PortalActions actions,
            ClaimsPrincipal user, HttpContext context, CancellationToken ct) =>
        {
            if (services.Endpoints.All(e => e.Id != id)) return Results.NotFound();
            if (!ControlActions.IsValid(request.Action)) return Results.Json(new { error = "action must be start, stop or restart" }, json, statusCode: 400);
            ControlResult result = await actions.ControlAsync(user, id, request.Action, Remote(context), ct);
            return Results.Json(result, json, statusCode: result.Success ? 200 : 409);
        }).RequireAuthorization(PortalPolicies.ServerOperate);
        api.MapGet("/services/{id}/events", async (string id, int? limit, GameServices services, CancellationToken ct) =>
            Results.Json(await services.Client(id).EventsAsync(Math.Clamp(limit ?? 100, 1, 512), ct), json))
            .RequireAuthorization(PortalPolicies.ServerRead);
        api.MapGet("/services/{id}/config", async (string id, GameServices services, CancellationToken ct) =>
            Results.Text(await services.Client(id).ConfigAsync(ct), "application/json"))
            .RequireAuthorization(PortalPolicies.ServerRead);

        // Sessions
        api.MapGet("/sessions", async (HttpContext context, GameServices services, CancellationToken ct) =>
            Results.Json(await services.SessionsAsync(context.Request.Query["include_closed"].FirstOrDefault() is "true" or "1", ct), json))
            .RequireAuthorization(PortalPolicies.ServerRead);
        api.MapPost("/sessions/{key}/disconnect", async (string key, PortalActions actions, ClaimsPrincipal user, HttpContext context, CancellationToken ct) =>
            await actions.DisconnectAsync(user, key, Remote(context), ct)
                ? Results.Json(new { disconnected = key }, json)
                : Results.Json(new { error = "session is not connected" }, json, statusCode: 404))
            .RequireAuthorization(PortalPolicies.ServerOperate);

        // Packet trace
        api.MapGet("/trace/sessions", async (string? service, int? limit, GameServices services, CancellationToken ct) =>
        {
            IEnumerable<AgentClient> clients = service is null ? services.Clients : [services.Client(service)];
            TraceSessionSummary[][] pages = await Task.WhenAll(clients.Select(async c =>
            {
                try { return await c.TraceSessionsAsync(Math.Clamp(limit ?? 200, 1, 2000), ct); }
                catch (Exception error) when (GameServices.IsUnavailable(error, ct)) { return []; }
            }));
            return Results.Json(pages.SelectMany(p => p).OrderByDescending(s => s.StartedUtc).Take(limit ?? 200).ToArray(), json);
        }).RequireAuthorization(PortalPolicies.TraceRead);
        api.MapGet("/trace/frames", async (HttpContext context, GameServices services, CancellationToken ct) =>
        {
            if (!AgentHost.TryParseQuery(context.Request.Query, out TraceQuery? query, out string? problem))
                return Results.Json(new { error = problem }, json, statusCode: 400);
            string? service = context.Request.Query["service"].FirstOrDefault();
            AgentClient? client = null;
            if (service is not null) client = services.Clients.FirstOrDefault(c => c.Endpoint.Id == service);
            else if (query!.Session is { } key) services.TryClientForSession(key, out client);
            if (client is null) return Results.Json(new { error = "give service=<id> or a session key" }, json, statusCode: 400);
            TracePage page = await client.TraceFramesAsync(query!, ct);
            return Results.Json(StripBytes(page, context.User), json);
        }).RequireAuthorization(PortalPolicies.TraceRead);
        api.MapGet("/trace/sessions/{key}/export", async (string key, string? format, GameServices services, PortalActions actions,
            ClaimsPrincipal user, HttpContext context, CancellationToken ct) =>
        {
            if (!services.TryClientForSession(key, out AgentClient? client)) return Results.NotFound();
            string kind = format ?? "jsonl";
            if (kind is not ("jsonl" or "pcap" or "pcap-decoded")) return Results.Json(new { error = "format must be jsonl, pcap or pcap-decoded" }, json, statusCode: 400);
            HttpResponseMessage response = await client!.ExportAsync(key, kind, ct);
            context.Response.RegisterForDispose(response);
            await actions.AuditAsync(user, "trace.export", key, true, Remote(context), message: kind, cancellationToken: ct);
            string name = response.Content.Headers.ContentDisposition?.FileNameStar ?? response.Content.Headers.ContentDisposition?.FileName ?? $"c9-{key}.{kind}";
            return Results.Stream(await response.Content.ReadAsStreamAsync(ct), response.Content.Headers.ContentType?.ToString(), name.Trim('"'));
        }).RequireAuthorization(PortalPolicies.TraceExport);
        api.MapGet("/packet-types", () => Results.Json(KnownPackets.All.Select(p => new { p.Type, p.Hex, p.Name, p.Direction, p.Description, p.Sensitive }), json))
            .RequireAuthorization(PortalPolicies.TraceRead);
        api.MapGet("/tools/wireshark/c9.lua", () => Results.File(System.Text.Encoding.UTF8.GetBytes(WiresharkDissector.Lua()), "text/x-lua", "c9_decoded.lua"))
            .RequireAuthorization(PortalPolicies.TraceRead);

        // Settings (shared with the legacy console's parameters.local.json)
        api.MapGet("/settings/parameters", (ParametersService parameters) => Results.Json(parameters.Read(), json))
            .RequireAuthorization(PortalPolicies.ServerRead);
        api.MapPut("/settings/parameters", async (AdminParameters body, PortalActions actions, ClaimsPrincipal user, HttpContext context, CancellationToken ct) =>
        {
            try { return Results.Json(await actions.SaveParametersAsync(user, body, Remote(context), ct), json); }
            catch (ArgumentException error) { return Results.Json(new { error = error.Message }, json, statusCode: 400); }
        }).RequireAuthorization(PortalPolicies.SettingsWrite);

        // Audit
        api.MapGet("/audit", async (int? limit, string? action, IAuditLog audit, CancellationToken ct) =>
            Results.Json(await audit.QueryAsync(new AuditQuery(Action: action, Limit: limit ?? 200), ct), json))
            .RequireAuthorization(PortalPolicies.AuditRead);
    }

    /// <summary>Users without <see cref="PortalPolicies.TraceExport"/> see headers only, never bytes.</summary>
    public static TracePage StripBytes(TracePage page, ClaimsPrincipal user) =>
        CanSeeBytes(user) ? page : page with { Frames = page.Frames.Select(f => f with { Wire = null, Payload = null }).ToArray() };

    public static bool CanSeeBytes(ClaimsPrincipal user) =>
        PortalPolicies.Roles[PortalPolicies.TraceExport].Any(user.IsInRole);

    private static string? Remote(HttpContext context) => context.Connection.RemoteIpAddress?.ToString();
}
