using C9.Contracts;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Portal.GameData;
using C9.Portal.World.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static C9.Portal.GameData.GameDataApi;

namespace C9.Portal.World;

public sealed record RatesRequest(RateSet? Rates, decimal Cap, string? Reason) : IWriteRequest;
public sealed record EventRequest(uint Version, string? Name, string? Description, DateTimeOffset StartsAt, DateTimeOffset EndsAt,
    RateSet? Rates, int? RepeatEveryDays, DateTimeOffset? RecurUntil, bool Enabled, System.Text.Json.JsonElement? Extra, string? Reason) : IWriteRequest
{
    public WorldEventInput Input() => new(Name ?? "", Description, StartsAt, EndsAt, Rates ?? RateSet.Neutral, RepeatEveryDays, RecurUntil,
        Enabled, Extra is { } extra ? extra.GetRawText() : "{}");
}
public sealed record MaintenanceRequest(bool Enabled, string? Message, DateTimeOffset? Until, IReadOnlyList<string>? AllowedLogins, string? Reason) : IWriteRequest;
public sealed record AnnouncementRequest(AnnouncementChannel Channel, string? Message, DateTimeOffset? StartsAt, DateTimeOffset? EndsAt,
    int? RepeatMinutes, string? Reason) : IWriteRequest;
public sealed record MapPopulation(int MapId, int? Submap, int Players, IReadOnlyList<string> Sessions);
public sealed record EffectiveRatesDto(RateSet Rates, RateSet Server, RateSet Account, decimal Cap, IReadOnlyList<ActiveEventDto> Events)
{
    public static EffectiveRatesDto From(EffectiveRates r) =>
        new(r.Rates, r.Server, r.Account, r.Cap, r.Events.Select(e => new ActiveEventDto(e.Id, e.Name, e.Rates)).ToArray());
}
public sealed record ActiveEventDto(long Id, string Name, RateSet Rates);
public sealed record WorldEventDto(long Id, string Name, string? Description, DateTimeOffset StartsAt, DateTimeOffset EndsAt,
    int? RepeatEveryDays, DateTimeOffset? RecurUntil, RateSet Rates, string Extra, bool Enabled, bool ActiveNow, string CreatedBy, uint Version)
{
    public static WorldEventDto From(WorldEvent e, DateTimeOffset now) => new(e.Id, e.Name, e.Description, e.StartsAt, e.EndsAt,
        e.RepeatEveryDays, e.RecurUntil, e.Rates, e.Extra, e.Enabled, e.IsActiveAt(now), e.CreatedBy, e.Version);
}

/// <summary>Game World Management (task 3.3): rates, events, maintenance, announcements, population and economy.</summary>
public sealed class WorldModule : IPortalModule
{
    public string Id => "world";
    public string Title => "Game World";
    public int Order => 210;

    public IReadOnlyList<PortalNavItem> Navigation { get; } =
    [
        new("ภาพรวมโลกเกม", "/world", "globe", PortalPolicies.WorldRead),
        new("Event", "/world/events", "calendar", PortalPolicies.WorldRead),
        new("เศรษฐกิจ", "/economy", "coins", PortalPolicies.WorldRead),
    ];

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddSingleton<EconomyQueries>();

    public void MapApi(RouteGroupBuilder api)
    {
        RouteGroupBuilder read = api.MapGroup("").RequireAuthorization(PortalPolicies.WorldRead);
        RouteGroupBuilder write = api.MapGroup("").RequireAuthorization(PortalPolicies.WorldWrite);

        read.MapGet("/rates", async (HttpContext http, long? accountId, DateTimeOffset? at) =>
            Configured(http.RequestServices)
                ? Json(EffectiveRatesDto.From(await Svc<WorldService>(http).EffectiveRatesAsync(accountId, at, http.RequestAborted)))
                : NotConfigured());

        write.MapPut("/rates", async (HttpContext http) =>
        {
            RatesRequest? body = await Body<RatesRequest>(http);
            return await Write(http, "world.rates", "rate_policy:server", body,
                (op, s) => s.GetRequiredService<WorldService>().SetServerRatesAsync(body!.Rates ?? RateSet.Neutral, body.Cap, op, http.RequestAborted),
                p => new { p.Rates, p.Cap, p.UpdatedAt, p.Version });
        });

        read.MapGet("/events", async (HttpContext http, DateTimeOffset? from, DateTimeOffset? to) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset start = from ?? now.AddDays(-30), end = to ?? now.AddDays(90);
            if (end <= start || end - start > TimeSpan.FromDays(EconomyQueries.MaxDays))
                return Results.Problem("Range must be positive and at most 366 days", statusCode: StatusCodes.Status400BadRequest);
            return Json((await Svc<WorldService>(http).EventsAsync(start, end, http.RequestAborted)).Select(e => WorldEventDto.From(e, now)));
        });

        write.MapPost("/events", async (HttpContext http) =>
        {
            EventRequest? body = await Body<EventRequest>(http);
            return await Write(http, "world.event_create", "world_event:new", body,
                (op, s) => s.GetRequiredService<WorldService>().CreateEventAsync(body!.Input(), op, http.RequestAborted),
                e => WorldEventDto.From(e, DateTimeOffset.UtcNow), StatusCodes.Status201Created);
        });

        write.MapPut("/events/{id:long}", async (HttpContext http, long id) =>
        {
            EventRequest? body = await Body<EventRequest>(http);
            return await Write(http, "world.event_update", $"world_event:{id}", body,
                (op, s) => s.GetRequiredService<WorldService>().UpdateEventAsync(id, body!.Version, body.Input(), op, http.RequestAborted),
                e => WorldEventDto.From(e, DateTimeOffset.UtcNow));
        });

        write.MapPost("/events/{id:long}/delete", async (HttpContext http, long id) =>
        {
            ReasonOnly? body = await Body<ReasonOnly>(http);
            return await Write(http, "world.event_delete", $"world_event:{id}", body,
                async (op, s) => { await s.GetRequiredService<WorldService>().DeleteEventAsync(id, op, http.RequestAborted); return new { deleted = id }; });
        });

        read.MapGet("/maintenance", async (HttpContext http) =>
            Configured(http.RequestServices) ? Json(await Svc<WorldService>(http).MaintenanceAsync(http.RequestAborted)) : NotConfigured());

        // Maintenance blocks logins, so it is a server-operation decision (Owner/Administrator).
        api.MapPut("/maintenance", async (HttpContext http) =>
        {
            MaintenanceRequest? body = await Body<MaintenanceRequest>(http);
            return await Write(http, "world.maintenance", "world_setting:maintenance", body, async (op, s) =>
            {
                MaintenanceState state = new(body!.Enabled, body.Message, body.Until, body.AllowedLogins ?? []);
                await s.GetRequiredService<WorldService>().SetMaintenanceAsync(state, op, http.RequestAborted);
                return state;
            });
        }).RequireAuthorization(PortalPolicies.ServerOperate);

        read.MapGet("/announcements", async (HttpContext http, string? channel) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            if (!TryEnum(channel, out AnnouncementChannel? value)) return BadQuery("channel");
            return Json(await Svc<WorldService>(http).ActiveAnnouncementsAsync(value ?? AnnouncementChannel.Launcher, http.RequestAborted));
        });

        write.MapPost("/announcements", async (HttpContext http) =>
        {
            AnnouncementRequest? body = await Body<AnnouncementRequest>(http);
            return await Write(http, "world.announce", "announcement:new", body,
                (op, s) => s.GetRequiredService<WorldService>().AnnounceAsync(body!.Channel, body.Message ?? "", body.StartsAt, body.EndsAt,
                    body.RepeatMinutes, op, http.RequestAborted), status: StatusCodes.Status201Created);
        });

        read.MapGet("/population", async (HttpContext http) =>
        {
            if (http.RequestServices.GetService<IGameServices>() is not { } game)
                return Results.Problem("Game services are not available", statusCode: StatusCodes.Status503ServiceUnavailable);
            return Json(Population(await game.SessionsAsync(false, http.RequestAborted)));
        });

        read.MapGet("/economy", async (HttpContext http, DateTimeOffset? from, DateTimeOffset? to, int? top) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            DateTimeOffset end = to ?? DateTimeOffset.UtcNow, start = from ?? end.AddDays(-30);
            try { return Json(await Svc<EconomyQueries>(http).SummaryAsync(start, end, top ?? 20, http.RequestAborted)); }
            catch (ArgumentOutOfRangeException error) { return Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest); }
        });
    }

    /// <summary>Players per map/sub-map from live sessions that have entered the world.</summary>
    public static IReadOnlyList<MapPopulation> Population(IEnumerable<SessionInfo> sessions) =>
        sessions.Where(s => s.ClosedUtc is null && s.MapId is not null)
            .GroupBy(s => (s.MapId!.Value, s.Submap))
            .Select(g => new MapPopulation(g.Key.Value, g.Key.Submap, g.Count(), g.Select(s => s.Key).ToArray()))
            .OrderByDescending(p => p.Players).ThenBy(p => p.MapId).ToArray();

    private sealed record ReasonOnly(string? Reason) : IWriteRequest;

    private static T Svc<T>(HttpContext http) where T : notnull => http.RequestServices.GetRequiredService<T>();
}
