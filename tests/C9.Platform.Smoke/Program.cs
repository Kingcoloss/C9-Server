using System.Buffers.Binary;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using C9.Contracts;
using C9.Portal;
using C9.Protocol;

// Usage: dotnet run --project tests/C9.Platform.Smoke -- [--state artifacts/docker-state] [--portal URL] [--prometheus URL]
//        [--grafana URL] [--login-port 14000] [--lobby-port 14001] [--host 127.0.0.1]
Dictionary<string, string> options = new(StringComparer.Ordinal)
{
    ["--state"] = "artifacts/docker-state", ["--portal"] = "http://127.0.0.1:14880", ["--prometheus"] = "http://127.0.0.1:9090",
    ["--grafana"] = "http://127.0.0.1:3000", ["--login-port"] = "14000", ["--lobby-port"] = "14001", ["--host"] = "127.0.0.1"
};
for (int i = 0; i + 1 < args.Length; i += 2)
{
    if (!options.ContainsKey(args[i])) { Console.Error.WriteLine($"Unknown option {args[i]}"); return 2; }
    options[args[i]] = args[i + 1];
}
string state = Path.GetFullPath(options["--state"]);
IPAddress host = IPAddress.Parse(options["--host"]);
int loginPort = int.Parse(options["--login-port"], System.Globalization.CultureInfo.InvariantCulture);
int lobbyPort = int.Parse(options["--lobby-port"], System.Globalization.CultureInfo.InvariantCulture);
byte[] marker = Encoding.ASCII.GetBytes("PRIVATE-LOCAL-IDENTITY");
JsonSerializerOptions json = ContractJson.Options;
int failed = 0, passed = 0;

async Task Step(string name, Func<Task> body)
{
    try { await body(); passed++; Console.WriteLine($"PASS  {name}"); }
    catch (Exception error) { failed++; Console.WriteLine($"FAIL  {name}: {error.GetType().Name}: {error.Message}"); }
}
static void Expect(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

string key = File.ReadAllText(Path.Combine(state, "portal", "access-key.txt")).Trim();
using HttpClient portal = new() { BaseAddress = new Uri(options["--portal"]), Timeout = TimeSpan.FromSeconds(30) };
portal.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
using HttpClient anonymous = new() { BaseAddress = new Uri(options["--portal"]), Timeout = TimeSpan.FromSeconds(10) };
string? loginKey = null;

await Step("portal health and authentication", async () =>
{
    Expect(await anonymous.GetStringAsync("/healthz") == "ok", "healthz");
    Expect((await anonymous.GetAsync("/api/v1/me")).StatusCode == HttpStatusCode.Unauthorized, "anonymous API must be 401");
    using HttpRequestMessage wrong = new(HttpMethod.Get, "/api/v1/me") { Headers = { Authorization = new("Bearer", new string('0', 64)) } };
    Expect((await anonymous.SendAsync(wrong)).StatusCode == HttpStatusCode.Unauthorized, "wrong key must be 401");
    JsonElement me = await portal.GetFromJsonAsync<JsonElement>("/api/v1/me", json);
    Expect(me.GetProperty("roles").EnumerateArray().Any(r => r.GetString() == "Owner"), "operator key signs in as Owner");
});

await Step("both game services reachable and running", async () =>
{
    ServiceSnapshot[] services = (await portal.GetFromJsonAsync<ServiceSnapshot[]>("/api/v1/services", json))!;
    Expect(services.Length >= 2, "two services configured");
    foreach (ServiceSnapshot s in services)
        Expect(s.Status?.Listener == ListenerState.Running, $"{s.Endpoint.Id}: {s.Error ?? s.Status?.Listener.ToString()}");
});

await Step("login flow on TCP " + loginPort + " (login, world list, world select, handoff, bad frame)", async () =>
{
    using TcpClient client = new();
    await client.ConnectAsync(host, loginPort);
    NetworkStream stream = client.GetStream();
    ConnectionCodec codec = new();
    Expect(codec.Decode(await ReadWireAsync(stream)).Type == 4, "greeting");
    byte[] login = new byte[1092];
    marker.CopyTo(login, 0);
    await stream.WriteAsync(codec.Encode(new Packet(0x2F04, login, flag: 0)));
    Expect(codec.Decode(await ReadWireAsync(stream)).Type == 0x2EE2, "LoginAck");
    await stream.WriteAsync(codec.Encode(new Packet(0x2EF6, new byte[4], flag: 0)));
    Expect(codec.Decode(await ReadWireAsync(stream)).Type == 0x2EF7, "0x2EF7");
    await stream.WriteAsync(codec.Encode(new Packet(0x2EE5)));
    Expect(codec.Decode(await ReadWireAsync(stream)).Type == 0x2EE6, "WorldListAck");
    byte[] selection = new byte[20];
    BinaryPrimitives.WriteUInt32LittleEndian(selection.AsSpan(8), 1);
    await stream.WriteAsync(codec.Encode(new Packet(0x2EEC, selection)));
    Packet handoff = codec.Decode(await ReadWireAsync(stream));
    Expect(handoff.Type == 0x2EED, "handoff 0x2EED");
    Expect(handoff.Payload.Span.IndexOf(new byte[] { 0x7F, 0, 0, 1, 0xB1, 0x36 }) >= 0, "handoff points to 127.0.0.1:14001");
    await stream.WriteAsync(new byte[] { 5, 0, 1, 2, 3 });
    using CancellationTokenSource wait = new(TimeSpan.FromSeconds(5));
    byte[] rest = new byte[64];
    try { while (await stream.ReadAsync(rest, wait.Token) > 0) { } } catch (IOException) { }
});

await Step("lobby flow on TCP " + lobbyPort + " (greeting mode 1, LobbyEnter, character list)", async () =>
{
    using TcpClient client = new();
    await client.ConnectAsync(host, lobbyPort);
    NetworkStream stream = client.GetStream();
    ConnectionCodec codec = new();
    Packet greeting = codec.Decode(await ReadWireAsync(stream));
    Expect(greeting.Type == 4 && greeting.Payload.Span[0] == 1, "lobby greeting mode 1");
    await stream.WriteAsync(codec.Encode(new Packet(0x1771, new byte[49])));
    Expect(codec.Decode(await ReadWireAsync(stream)).Type == 0x0EFA, "CharacterList");
});
await Task.Delay(1500);

await Step("sessions API lists the closed login session with its error", async () =>
{
    SessionInfo[] sessions = (await portal.GetFromJsonAsync<SessionInfo[]>("/api/v1/sessions?include_closed=true", json))!;
    SessionInfo? login = sessions.Where(s => s.ServiceId == "login").MaxBy(s => s.ConnectedUtc);
    Expect(login is { ClosedUtc: not null, LastError: not null }, "login session closed with error");
    Expect(sessions.Any(s => s.ServiceId == "lobby"), "lobby session present");
    loginKey = login!.Key;
});

await Step("packet trace: frames, redaction and framing error", async () =>
{
    TraceSessionSummary[] summaries = (await portal.GetFromJsonAsync<TraceSessionSummary[]>("/api/v1/trace/sessions?service=login", json))!;
    Expect(summaries.Any(s => s.Key == loginKey && s.Errors >= 1), "trace session summary with error");
    TracePage page = (await portal.GetFromJsonAsync<TracePage>($"/api/v1/trace/frames?session={Uri.EscapeDataString(loginKey!)}&limit=500", json))!;
    Expect(page.Frames.Count >= 10, $"expected >= 10 frames, got {page.Frames.Count}");
    TraceFrame credential = page.Frames.Single(f => f.Type == 0x2F04);
    Expect(credential.Redacted && credential.Wire is null && credential.Payload is null, "login frame redacted");
    Expect(page.Frames.Any(f => f.Type == 0x2EED && f.Payload is not null), "handoff payload decoded");
    Expect(page.Frames.Any(f => f.Error is not null && f.Error.StartsWith("InvalidLength", StringComparison.Ordinal)), "framing error frame");
    TracePage filtered = (await portal.GetFromJsonAsync<TracePage>($"/api/v1/trace/frames?session={Uri.EscapeDataString(loginKey!)}&dir=c2s&type=WorldListReq", json))!;
    Expect(filtered.Frames.Count == 1, "filter by direction and packet name");
});

await Step("exports (pcap, pcap-decoded, jsonl) carry no credentials", async () =>
{
    foreach (string format in new[] { "pcap", "pcap-decoded", "jsonl" })
    {
        byte[] body = await portal.GetByteArrayAsync($"/api/v1/trace/sessions/{Uri.EscapeDataString(loginKey!)}/export?format={format}");
        Expect(body.Length > 100, format + " not empty");
        Expect(body.AsSpan().IndexOf(marker) < 0, format + " must not contain the login identity");
        if (format.StartsWith("pcap", StringComparison.Ordinal)) Expect(BinaryPrimitives.ReadUInt32LittleEndian(body) == 0xA1B23C4D, format + " nanosecond pcap magic");
    }
    string traceRoot = Path.Combine(state, "trace");
    if (Directory.Exists(traceRoot))
        foreach (string file in Directory.EnumerateFiles(traceRoot, "*.jsonl", SearchOption.AllDirectories))
            Expect(ReadShared(file).AsSpan().IndexOf(marker) < 0, "trace file " + file);
});

await Step("parameters round trip writes the profile overlays", async () =>
{
    JsonElement parameters = await portal.GetFromJsonAsync<JsonElement>("/api/v1/settings/parameters", json);
    HttpResponseMessage put = await portal.PutAsJsonAsync("/api/v1/settings/parameters", parameters, json);
    Expect(put.IsSuccessStatusCode, "PUT parameters " + (int)put.StatusCode);
    Expect(File.Exists(Path.Combine(state, "portal", "profiles", "lab-official-character-lobby.json")), "lobby overlay written");
});

await Step("restart lobby from the portal and load the overlay", async () =>
{
    HttpResponseMessage response = await portal.PostAsJsonAsync("/api/v1/services/lobby/control", new ControlRequest(ControlActions.Restart), json);
    ControlResult result = (await response.Content.ReadFromJsonAsync<ControlResult>(json))!;
    Expect(result.Success && result.Status.Listener == ListenerState.Running, result.Message);
    Expect(result.Status.ConfigOverlayActive, "overlay active after restart");
});

await Step("audit log records the operator actions", async () =>
{
    AuditEntry[] entries = (await portal.GetFromJsonAsync<AuditEntry[]>("/api/v1/audit?limit=200", json))!;
    foreach (string action in new[] { "trace.export", "settings.parameters", "service.restart" })
        Expect(entries.Any(e => e.Action == action && e.Success), "audit " + action);
});

await Step("modules loaded (reference, accounts, world)", async () =>
{
    JsonElement modules = await portal.GetFromJsonAsync<JsonElement>("/api/v1/reference/modules", json);
    string[] ids = modules.EnumerateArray().Select(m => m.GetProperty("id").GetString()!).ToArray();
    Expect(ids.Contains("reference"), "reference module");
    Console.WriteLine("      modules: " + string.Join(", ", ids));
});

await Step("Prometheus scrapes agents and portal", async () =>
{
    using HttpClient prometheus = new() { BaseAddress = new Uri(options["--prometheus"]) };
    for (int attempt = 0; ; attempt++)
    {
        JsonElement up = await prometheus.GetFromJsonAsync<JsonElement>("/api/v1/query?query=" + Uri.EscapeDataString("sum(c9_listener_up)"));
        JsonElement packets = await prometheus.GetFromJsonAsync<JsonElement>("/api/v1/query?query=" + Uri.EscapeDataString("sum(c9_packets_total)"));
        JsonElement agents = await prometheus.GetFromJsonAsync<JsonElement>("/api/v1/query?query=" + Uri.EscapeDataString("sum(c9_portal_agent_up)"));
        double Value(JsonElement e) => e.GetProperty("data").GetProperty("result").EnumerateArray().Select(r =>
            double.Parse(r.GetProperty("value")[1].GetString()!, System.Globalization.CultureInfo.InvariantCulture)).FirstOrDefault();
        if (Value(up) == 2 && Value(packets) > 0 && Value(agents) == 2) return;
        if (attempt > 6) throw new InvalidOperationException($"listener_up={Value(up)} packets={Value(packets)} agent_up={Value(agents)}");
        await Task.Delay(5000);
    }
});

await Step("Grafana serves the provisioned C9 dashboard", async () =>
{
    using HttpClient grafana = new() { BaseAddress = new Uri(options["--grafana"]) };
    Expect((await grafana.GetStringAsync("/api/health")).Contains("ok", StringComparison.Ordinal), "health");
    JsonElement search = await grafana.GetFromJsonAsync<JsonElement>("/api/search?query=C9");
    Expect(search.EnumerateArray().Any(d => d.GetProperty("uid").GetString() == "c9-overview"), "dashboard c9-overview");
});

string grafanaPassword = Path.Combine(state, "secrets", "grafana", "admin-password.txt");
await Step("Loki receives game-service logs (via Grafana)", async () =>
{
    string password;
    try { password = File.ReadAllText(grafanaPassword).Trim(); }
    catch (UnauthorizedAccessException) { Console.WriteLine("      skipped: Grafana password file not readable by this user"); return; }
    using HttpClient grafana = new() { BaseAddress = new Uri(options["--grafana"]) };
    grafana.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:" + password)));
    for (int attempt = 0; ; attempt++)
    {
        string query = Uri.EscapeDataString("{service=\"c9-login\", event=\"disconnected\"}");
        long start = DateTimeOffset.UtcNow.AddMinutes(-30).ToUnixTimeMilliseconds() * 1_000_000;
        JsonElement result = await grafana.GetFromJsonAsync<JsonElement>($"/api/datasources/proxy/uid/c9-loki/loki/api/v1/query_range?query={query}&start={start}&limit=20");
        int streams = result.GetProperty("data").GetProperty("result").GetArrayLength();
        if (streams > 0) return;
        if (attempt > 6) throw new InvalidOperationException("no 'disconnected' log lines for c9-login in Loki");
        await Task.Delay(5000);
    }
});

Console.WriteLine($"{passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;

static async Task<byte[]> ReadWireAsync(NetworkStream stream)
{
    using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(5));
    byte[] prefix = new byte[2];
    await stream.ReadExactlyAsync(prefix, timeout.Token);
    int size = BinaryPrimitives.ReadUInt16LittleEndian(prefix);
    if (size is < 13 or > 16384) throw new InvalidDataException($"frame size {size}");
    byte[] frame = new byte[size];
    prefix.CopyTo(frame, 0);
    await stream.ReadExactlyAsync(frame.AsMemory(2), timeout.Token);
    return frame;
}

// The game services keep their JSONL trace files open for appending through the bind mount.
// File.ReadAllBytes asks for FileShare.Read, which Windows refuses while a writer holds the file.
static byte[] ReadShared(string path)
{
    using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
    using MemoryStream copy = new();
    stream.CopyTo(copy);
    return copy.ToArray();
}
