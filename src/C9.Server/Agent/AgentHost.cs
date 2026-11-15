using System.Globalization;
using System.Text;
using System.Text.Json;
using C9.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace C9.Server;

/// <summary>
/// The game-service agent: an internal HTTP API (contract <see cref="AgentApi"/>) used by the Admin Portal
/// and Prometheus. <c>/healthz</c> and <c>/metrics</c> are open; everything under <c>/agent/v1</c> requires
/// <c>Authorization: Bearer &lt;agent token&gt;</c>.
/// </summary>
public static class AgentHost
{
    public static WebApplication Create(GameServiceController controller, ServiceRuntime runtime, string url, AgentToken token)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? address) || address.Scheme != Uri.UriSchemeHttp)
            throw new ArgumentException("Agent URL must be an absolute http:// URL");
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { Args = [] });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls(url);
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
            options.Limits.MaxRequestBodySize = 16 * 1024;
            options.Limits.MaxConcurrentConnections = 64;
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(10);
        });
        WebApplication app = builder.Build();
        JsonSerializerOptions json = ContractJson.Options;

        app.Use(async (context, next) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            if (context.Request.Path.StartsWithSegments(AgentApi.Prefix) &&
                !token.Matches(context.Request.Headers.Authorization.ToString()))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            await next();
        });

        app.MapGet(AgentApi.HealthPath, () => Results.Text("ok"));
        app.MapGet("/readyz", () => controller.Status().Listener == ListenerState.Running
            ? Results.Text("ready") : Results.Text("listener not running", statusCode: StatusCodes.Status503ServiceUnavailable));
        app.MapGet(AgentApi.MetricsPath, () =>
            Results.Text(runtime.Metrics.Render(controller.Status(), runtime.LiveSessions()), PrometheusText.ContentType));

        string v1 = AgentApi.Prefix;
        app.MapGet(v1 + "/status", () => Results.Json(controller.Status(), json));
        app.MapPost(v1 + "/control", async (HttpContext context) =>
        {
            ControlRequest? request;
            try { request = await context.Request.ReadFromJsonAsync<ControlRequest>(json, context.RequestAborted); }
            catch (JsonException) { request = null; }
            if (request is null || !ControlActions.IsValid(request.Action))
                return Results.Json(new { error = "action must be start, stop or restart" }, json, statusCode: StatusCodes.Status400BadRequest);
            ControlResult result = await controller.ExecuteAsync(request.Action, context.RequestAborted);
            return Results.Json(result, json, statusCode: result.Success ? StatusCodes.Status200OK : StatusCodes.Status409Conflict);
        });
        app.MapGet(v1 + "/config", () => Results.Text(controller.EffectiveConfigJson(), "application/json"));
        app.MapGet(v1 + "/sessions", (HttpContext context) =>
        {
            IEnumerable<SessionInfo> sessions = runtime.LiveSessions();
            if (context.Request.Query["include_closed"].FirstOrDefault() is "1" or "true")
                sessions = sessions.Concat(runtime.RecentClosedSessions());
            return Results.Json(sessions.ToArray(), json);
        });
        app.MapPost(v1 + "/sessions/{key}/disconnect", (string key) => controller.Disconnect(key)
            ? Results.Json(new { disconnected = key }, json)
            : Results.Json(new { error = "session is not connected" }, json, statusCode: StatusCodes.Status404NotFound));
        app.MapGet(v1 + "/events", (int? limit) => Results.Json(runtime.RecentEvents(Math.Clamp(limit ?? 200, 1, 512)), json));

        app.MapGet(v1 + "/trace/sessions", (int? limit) =>
            runtime.Tracer is { } tracer ? Results.Json(tracer.Sessions(limit ?? 200), json) : TraceDisabled());
        app.MapGet(v1 + "/trace/frames", (HttpContext context) =>
        {
            if (runtime.Tracer is not { } tracer) return TraceDisabled();
            if (!TryParseQuery(context.Request.Query, out TraceQuery? query, out string? problem))
                return Results.Json(new { error = problem }, json, statusCode: StatusCodes.Status400BadRequest);
            return Results.Json(tracer.Query(query!), json);
        });
        app.MapGet(v1 + "/trace/sessions/{key}/export", (string key, string? format) =>
        {
            if (runtime.Tracer is not { } tracer) return TraceDisabled();
            if (tracer.Session(key) is null) return Results.NotFound();
            IReadOnlyList<TraceFrame> frames = tracer.SessionFrames(key);
            SessionInfo? info = tracer.SessionInfoOf(key);
            string name = string.Concat(key.Select(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' ? c : '_'));
            return (format ?? "jsonl") switch
            {
                "jsonl" => Results.File(Encoding.UTF8.GetBytes(string.Join('\n', frames.Select(f =>
                    JsonSerializer.Serialize(new TraceRecord(TraceRecordKinds.Frame, Frame: f), json))) + "\n"),
                    "application/x-ndjson", $"c9-trace-{name}.jsonl"),
                "pcap" => Results.File(PcapExport.Wire(info, frames), "application/vnd.tcpdump.pcap", $"c9-wire-{name}.pcap"),
                "pcap-decoded" => Results.File(PcapExport.Decoded(info, frames), "application/vnd.tcpdump.pcap", $"c9-decoded-{name}.pcap"),
                _ => Results.Json(new { error = "format must be jsonl, pcap or pcap-decoded" }, json, statusCode: StatusCodes.Status400BadRequest)
            };
        });
        app.MapGet(v1 + "/stream", async (HttpContext context, string? session, bool? frames) =>
        {
            context.Response.Headers.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpResponseBodyFeature>()?.DisableBuffering();
            using AgentStreamHub.Subscription subscription = runtime.Stream.Subscribe(session, frames ?? true);
            CancellationToken aborted = context.RequestAborted;
            try
            {
                await context.Response.WriteAsync(": connected\n\n", aborted);
                await context.Response.Body.FlushAsync(aborted);
                Task<bool>? pending = null;
                while (!aborted.IsCancellationRequested)
                {
                    pending ??= subscription.Reader.WaitToReadAsync(aborted).AsTask();
                    if (await Task.WhenAny(pending, Task.Delay(TimeSpan.FromSeconds(15), aborted)) != pending)
                    {
                        await context.Response.WriteAsync(": ping\n\n", aborted);
                        await context.Response.Body.FlushAsync(aborted);
                        continue;
                    }
                    if (!await pending) break;
                    pending = null;
                    StringBuilder batch = new();
                    while (subscription.Reader.TryRead(out StreamMessage? message))
                        batch.Append("event: ").Append(message.Event).Append("\ndata: ").Append(message.Json).Append("\n\n");
                    await context.Response.WriteAsync(batch.ToString(), aborted);
                    await context.Response.Body.FlushAsync(aborted);
                }
            }
            catch (OperationCanceledException) when (aborted.IsCancellationRequested) { }
            catch (IOException) { }
        });
        return app;
    }

    private static IResult TraceDisabled() =>
        Results.Json(new { error = "packet trace is disabled for this service" }, ContractJson.Options, statusCode: StatusCodes.Status404NotFound);

    public static bool TryParseQuery(IQueryCollection query, out TraceQuery? result, out string? problem)
    {
        result = null;
        problem = null;
        List<ushort> types = [];
        foreach (string part in query["type"].Concat(query["types"]).SelectMany(v => (v ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
        {
            if (!TryParseType(part, out ushort type)) { problem = $"invalid packet type '{part}'"; return false; }
            types.Add(type);
        }
        string? direction = query["dir"].FirstOrDefault() ?? query["direction"].FirstOrDefault();
        if (direction is not (null or TraceDirections.Inbound or TraceDirections.Outbound)) { problem = "dir must be c2s or s2c"; return false; }
        long? after = null, before = null;
        if (query["after_id"].FirstOrDefault() is { } a) { if (!long.TryParse(a, NumberStyles.None, CultureInfo.InvariantCulture, out long v)) { problem = "invalid after_id"; return false; } after = v; }
        if (query["before_id"].FirstOrDefault() is { } b) { if (!long.TryParse(b, NumberStyles.None, CultureInfo.InvariantCulture, out long v)) { problem = "invalid before_id"; return false; } before = v; }
        int limit = 200;
        if (query["limit"].FirstOrDefault() is { } l && (!int.TryParse(l, NumberStyles.None, CultureInfo.InvariantCulture, out limit) || limit < 1))
        { problem = "invalid limit"; return false; }
        DateTimeOffset? since = null;
        if (query["since"].FirstOrDefault() is { } s)
        {
            if (!DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset parsed)) { problem = "invalid since"; return false; }
            since = parsed;
        }
        string? errors = query["errors_only"].FirstOrDefault();
        result = new TraceQuery
        {
            Session = query["session"].FirstOrDefault() is { Length: > 0 } session ? session : null,
            Direction = direction, Types = types.Count > 0 ? types : null,
            ErrorsOnly = errors is "1" or "true", AfterId = after, BeforeId = before, SinceUtc = since,
            Limit = Math.Min(limit, TraceQuery.MaxLimit)
        };
        return true;
    }

    public static bool TryParseType(string text, out ushort type)
    {
        string value = text.Trim();
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return ushort.TryParse(value[2..], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out type);
        if (ushort.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out type)) return true;
        PacketTypeInfo? named = KnownPackets.All.FirstOrDefault(p => p.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
        type = named?.Type ?? 0;
        return named is not null;
    }
}
