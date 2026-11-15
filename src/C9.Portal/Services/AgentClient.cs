using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using C9.Contracts;

namespace C9.Portal.Services;

/// <summary>Typed HTTP client for one game-service agent (contract C9.Contracts.AgentApi v1).</summary>
public sealed class AgentClient
{
    private readonly HttpClient http;

    public AgentClient(GameServiceEndpoint endpoint, string token, HttpMessageHandler? handler = null)
    {
        Endpoint = endpoint;
        http = new HttpClient(handler ?? new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5), ConnectTimeout = TimeSpan.FromSeconds(3) }, disposeHandler: handler is null)
        {
            BaseAddress = endpoint.AgentUrl,
            Timeout = Timeout.InfiniteTimeSpan
        };
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public GameServiceEndpoint Endpoint { get; }

    private static CancellationTokenSource Deadline(CancellationToken cancellationToken, int seconds = 5)
    {
        CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        source.CancelAfter(TimeSpan.FromSeconds(seconds));
        return source;
    }

    private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken, int seconds = 5)
    {
        using CancellationTokenSource deadline = Deadline(cancellationToken, seconds);
        using HttpResponseMessage response = await http.GetAsync(path, deadline.Token);
        await EnsureAsync(response, deadline.Token);
        return (await response.Content.ReadFromJsonAsync<T>(ContractJson.Options, deadline.Token))!;
    }

    private static async Task EnsureAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;
        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new AgentException(response.StatusCode, body.Length > 300 ? body[..300] : body);
    }

    public Task<AgentStatus> StatusAsync(CancellationToken cancellationToken = default) =>
        GetAsync<AgentStatus>(AgentApi.Prefix + "/status", cancellationToken, 3);

    public async Task<ControlResult> ControlAsync(string action, CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource deadline = Deadline(cancellationToken, 30);
        using HttpResponseMessage response = await http.PostAsJsonAsync(AgentApi.Prefix + "/control", new ControlRequest(action), ContractJson.Options, deadline.Token);
        if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Conflict)
            return (await response.Content.ReadFromJsonAsync<ControlResult>(ContractJson.Options, deadline.Token))!;
        await EnsureAsync(response, deadline.Token);
        throw new AgentException(response.StatusCode, "Unexpected control response");
    }

    public Task<SessionInfo[]> SessionsAsync(bool includeClosed, CancellationToken cancellationToken = default) =>
        GetAsync<SessionInfo[]>(AgentApi.Prefix + "/sessions" + (includeClosed ? "?include_closed=true" : ""), cancellationToken);

    public async Task<bool> DisconnectAsync(string key, CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource deadline = Deadline(cancellationToken);
        using HttpResponseMessage response = await http.PostAsync($"{AgentApi.Prefix}/sessions/{Uri.EscapeDataString(key)}/disconnect", null, deadline.Token);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        await EnsureAsync(response, deadline.Token);
        return true;
    }

    public Task<ServiceEvent[]> EventsAsync(int limit, CancellationToken cancellationToken = default) =>
        GetAsync<ServiceEvent[]>($"{AgentApi.Prefix}/events?limit={limit.ToString(CultureInfo.InvariantCulture)}", cancellationToken);

    public async Task<string> ConfigAsync(CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource deadline = Deadline(cancellationToken);
        using HttpResponseMessage response = await http.GetAsync(AgentApi.Prefix + "/config", deadline.Token);
        await EnsureAsync(response, deadline.Token);
        return await response.Content.ReadAsStringAsync(deadline.Token);
    }

    public Task<TraceSessionSummary[]> TraceSessionsAsync(int limit, CancellationToken cancellationToken = default) =>
        GetAsync<TraceSessionSummary[]>($"{AgentApi.Prefix}/trace/sessions?limit={limit.ToString(CultureInfo.InvariantCulture)}", cancellationToken);

    public Task<TracePage> TraceFramesAsync(TraceQuery query, CancellationToken cancellationToken = default) =>
        GetAsync<TracePage>(AgentApi.Prefix + "/trace/frames" + ToQueryString(query), cancellationToken, 15);

    /// <summary>Streams an export (jsonl, pcap, pcap-decoded). The caller disposes the response.</summary>
    public async Task<HttpResponseMessage> ExportAsync(string key, string format, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await http.GetAsync(
            $"{AgentApi.Prefix}/trace/sessions/{Uri.EscapeDataString(key)}/export?format={Uri.EscapeDataString(format)}",
            HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        try { await EnsureAsync(response, cancellationToken); }
        catch { response.Dispose(); throw; }
        return response;
    }

    /// <summary>Opens the agent's server-sent event stream.</summary>
    public async Task<Stream> OpenStreamAsync(bool frames, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await http.GetAsync($"{AgentApi.Prefix}/stream?frames={(frames ? "true" : "false")}",
            HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureAsync(response, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public static string ToQueryString(TraceQuery query)
    {
        StringBuilder builder = new();
        void Add(string name, string? value)
        {
            if (value is null) return;
            builder.Append(builder.Length == 0 ? '?' : '&').Append(name).Append('=').Append(Uri.EscapeDataString(value));
        }
        Add("session", query.Session);
        Add("dir", query.Direction);
        if (query.Types is { Count: > 0 } types) Add("type", string.Join(',', types.Select(t => $"0x{t:X4}")));
        if (query.ErrorsOnly) Add("errors_only", "true");
        Add("after_id", query.AfterId?.ToString(CultureInfo.InvariantCulture));
        Add("before_id", query.BeforeId?.ToString(CultureInfo.InvariantCulture));
        Add("since", query.SinceUtc?.ToString("O", CultureInfo.InvariantCulture));
        Add("limit", query.Limit.ToString(CultureInfo.InvariantCulture));
        return builder.ToString();
    }
}

public sealed class AgentException(HttpStatusCode status, string message) : Exception($"Agent returned {(int)status}: {message}")
{
    public HttpStatusCode Status { get; } = status;
}
