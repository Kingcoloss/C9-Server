using System.Buffers.Binary;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using C9.Contracts;
using C9.Protocol;
using C9.Server;
using Microsoft.AspNetCore.Builder;

internal static partial class Validation
{
    private static readonly byte[] PrivateIdentity = Encoding.ASCII.GetBytes("PRIVATE-LOCAL-IDENTITY");

    private static string WriteOfficialLoginConfig(string directory, int port = 0)
    {
        using JsonDocument fixture = Fixture("lab-official-login-fixture.json");
        System.Text.Json.Nodes.JsonObject config = System.Text.Json.Nodes.JsonNode.Parse(fixture.RootElement.GetRawText())!.AsObject();
        config["port"] = port;
        config["timeout_seconds"] = 3;
        string path = Path.Combine(directory, "login.json");
        File.WriteAllText(path, config.ToJsonString());
        return path;
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string message)
    {
        for (int attempt = 0; attempt < 100 && !condition(); attempt++) await Task.Delay(50);
        Check.True(condition(), message);
    }

    private static async Task OfficialLoginFlowAsync(int port, bool sendInvalidFrame)
    {
        using TcpClient client = new();
        await client.ConnectAsync(IPAddress.Loopback, port);
        NetworkStream stream = client.GetStream();
        ConnectionCodec codec = new();
        Check.Equal((ushort)4, codec.Decode(await ReadWireAsync(stream)).Type);
        byte[] login = new byte[1092];
        PrivateIdentity.CopyTo(login, 0);
        await stream.WriteAsync(codec.Encode(new Packet(0x2F04, login, flag: 0)));
        Check.Equal((ushort)0x2EE2, codec.Decode(await ReadWireAsync(stream)).Type);
        await stream.WriteAsync(codec.Encode(new Packet(0x2EF6, new byte[4], flag: 0)));
        Check.Equal((ushort)0x2EF7, codec.Decode(await ReadWireAsync(stream)).Type);
        await stream.WriteAsync(codec.Encode(new Packet(0x2EE5)));
        Check.Equal((ushort)0x2EE6, codec.Decode(await ReadWireAsync(stream)).Type);
        if (!sendInvalidFrame) return; // Closing our side ends the session with peer_closed.
        await stream.WriteAsync(new byte[] { 5, 0, 1, 2, 3 });
        await ClosedAsync(client);
    }

    private static async Task TcpAgentTraceAndControl()
    {
        string root = Directory.CreateTempSubdirectory("c9-agent-").FullName;
        try
        {
            string config = WriteOfficialLoginConfig(root);
            string traces = Path.Combine(root, "traces");
            TraceOptions traceOptions = new() { Directory = traces };
            string key;
            int frameCount;
            await using (PacketTracer tracer = new("login", "b00t01", traceOptions))
            {
                ServiceRuntime runtime = new("login", tracer, "b00t01");
                tracer.Stream = runtime.Stream;
                await using GameServiceController controller = new(new ConfigSource(config), runtime, runtime);
                ControlResult started = await controller.ExecuteAsync(ControlActions.Start);
                Check.True(started.Success && started.Status.Listener == ListenerState.Running && started.Status.Port > 0, started.Message);
                int port = started.Status.Port;
                await OfficialLoginFlowAsync(port, sendInvalidFrame: true);
                await WaitUntilAsync(() => runtime.LiveSessions().Count == 0 && runtime.RecentClosedSessions().Count == 1, "session closed");

                SessionInfo closed = runtime.RecentClosedSessions().Single();
                key = closed.Key;
                Check.Equal("login-b00t01-1", key);
                Check.Equal(3L, closed.PacketsReceived);
                Check.True(closed.LastError is not null && closed.ClosedUtc is not null);
                IReadOnlyList<TraceFrame> frames = tracer.SessionFrames(key);
                frameCount = frames.Count;
                Check.Equal(8, frameCount); // 1 greeting + 3 requests + 3 replies + 1 framing failure.
                TraceFrame loginFrame = frames.Single(f => f.Type == 0x2F04);
                Check.True(loginFrame.Redacted && loginFrame.Wire is null && loginFrame.Payload is null && loginFrame.PayloadSha256 is null);
                Check.Equal(1092, loginFrame.PayloadLength!.Value);
                TraceFrame worldList = frames.Single(f => f.Type == 0x2EE5);
                Check.True(worldList.Flag == 1 && worldList.Wire is not null && worldList.Payload == "" && worldList.Selector is not null);
                Check.Equal(TraceDirections.Inbound, worldList.Direction);
                TraceFrame failure = frames.Single(f => f.Error is not null);
                Check.True(failure.Error!.StartsWith("InvalidLength", StringComparison.Ordinal));
                Check.Bytes(new byte[] { 5, 0, 1, 2, 3 }, Convert.FromBase64String(failure.Wire!));
                Check.True(frames.Zip(frames.Skip(1)).All(pair => pair.First.Id < pair.Second.Id));
                long inbound = 0;
                foreach (TraceFrame frame in frames.Where(f => f.Direction == TraceDirections.Inbound && f.Error is null))
                {
                    Check.Equal(inbound, frame.StreamOffset);
                    inbound += frame.WireLength;
                }

                TraceSessionSummary summary = tracer.Session(key)!;
                Check.True(!summary.Live && summary.Frames == 8 && summary.Errors == 1);
                TracePage inboundOnly = tracer.Query(new TraceQuery { Session = key, Direction = TraceDirections.Inbound });
                Check.Equal(4, inboundOnly.Frames.Count);
                TracePage typed = tracer.Query(new TraceQuery { Types = [0x2EE5, 0x2EE6] });
                Check.Equal(2, typed.Frames.Count);
                TracePage tail = tracer.Query(new TraceQuery { Session = key, AfterId = frames[4].Id, Limit = 2 });
                Check.True(tail.Truncated && tail.Frames.Count == 2 && tail.Frames[0].Id == frames[5].Id);

                string metrics = runtime.Metrics.Render(controller.Status(), runtime.LiveSessions());
                Check.True(metrics.Contains("c9_packets_total{service=\"login\",direction=\"c2s\",type=\"0x2EE5\",name=\"WorldListReq\"} 1", StringComparison.Ordinal), metrics);
                Check.True(metrics.Contains("c9_session_errors_total{service=\"login\",reason=\"length\"} 1", StringComparison.Ordinal), metrics);
                Check.True(metrics.Contains("c9_listener_up{service=\"login\",port=", StringComparison.Ordinal));
                Check.True(metrics.Contains("c9_session_duration_seconds_count{service=\"login\"} 1", StringComparison.Ordinal));

                byte[] wirePcap = PcapExport.Wire(closed, frames);
                List<byte[]> wirePackets = ReadPcap(wirePcap);
                Check.Equal(3 + frames.Count + 2, wirePackets.Count);
                Check.Equal((byte)0x02, wirePackets[0][33]); // SYN
                Check.Equal((byte)0x11, wirePackets[^1][33]); // FIN, ACK
                Check.True(wirePackets.All(p => IpChecksumValid(p)));
                byte[] loginSegment = wirePackets[3 + frames.ToList().IndexOf(loginFrame)];
                Check.True(loginSegment.AsSpan(40).IndexOf(PrivateIdentity) < 0 && loginSegment.Length == 40 + loginFrame.WireLength);
                List<byte[]> decoded = ReadPcap(PcapExport.Decoded(closed, frames));
                Check.Equal(frames.Count, decoded.Count);
                Check.Equal((ushort)PcapExport.DecodedPort, BinaryPrimitives.ReadUInt16BigEndian(decoded[0].AsSpan(20)));

                ControlResult stopped = await controller.ExecuteAsync(ControlActions.Stop);
                Check.True(stopped.Success && stopped.Status.Listener == ListenerState.Stopped);
                await Check.ThrowsAsync<SocketException>(async () => { using TcpClient refused = new(); await refused.ConnectAsync(IPAddress.Loopback, port); });
                ControlResult restarted = await controller.ExecuteAsync(ControlActions.Restart);
                Check.True(restarted.Success && restarted.Status.Listener == ListenerState.Running);
                Check.True(!controller.Disconnect("login-b00t01-404"));
            }
            string persisted = string.Concat(Directory.EnumerateFiles(traces, "*.jsonl", SearchOption.AllDirectories).Select(File.ReadAllText));
            Check.True(!persisted.Contains(Convert.ToBase64String(PrivateIdentity)[..20], StringComparison.Ordinal));
            Check.True(!persisted.Contains("PRIVATE-LOCAL-IDENTITY", StringComparison.Ordinal));

            await using PacketTracer reloaded = new("login", "b00t02", traceOptions);
            TraceSessionSummary history = reloaded.Session(key) ?? throw new InvalidOperationException("history missing");
            Check.True(!history.Live && history.Frames == frameCount && history.Errors == 1);
            Check.Equal(frameCount, reloaded.SessionFrames(key).Count);
            Check.Equal(frameCount, reloaded.Query(new TraceQuery { Session = key }).Frames.Count);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static async Task TcpAgentHttpApi()
    {
        string root = Directory.CreateTempSubdirectory("c9-agent-http-").FullName;
        try
        {
            string config = WriteOfficialLoginConfig(root);
            await using PacketTracer tracer = new("login", "a9e7f1", new TraceOptions());
            ServiceRuntime runtime = new("login", tracer, "a9e7f1");
            tracer.Stream = runtime.Stream;
            await using GameServiceController controller = new(new ConfigSource(config), runtime, runtime);
            AgentToken token = AgentToken.LoadOrCreate(Path.Combine(root, "secrets", "agent.token"));
            await using WebApplication agent = AgentHost.Create(controller, runtime, "http://127.0.0.1:0", token);
            await agent.StartAsync();
            await controller.ExecuteAsync(ControlActions.Start);
            using HttpClient http = new() { BaseAddress = new Uri(agent.Urls.First()), Timeout = TimeSpan.FromSeconds(10) };
            Check.Equal(HttpStatusCode.OK, (await http.GetAsync(AgentApi.HealthPath)).StatusCode);
            Check.Equal(HttpStatusCode.Unauthorized, (await http.GetAsync(AgentApi.Prefix + "/status")).StatusCode);
            using (HttpRequestMessage wrong = new(HttpMethod.Get, AgentApi.Prefix + "/status"))
            {
                wrong.Headers.Authorization = new AuthenticationHeaderValue("Bearer", new string('0', 64));
                Check.Equal(HttpStatusCode.Unauthorized, (await http.SendAsync(wrong)).StatusCode);
            }
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            AgentStatus status = (await http.GetFromJsonAsync<AgentStatus>(AgentApi.Prefix + "/status", ContractJson.Options))!;
            Check.True(status.ServiceId == "login" && status.Listener == ListenerState.Running && status.Trace.Enabled);
            Check.Equal(TracePayloadPolicies.RedactCredentials, status.Trace.PayloadPolicy);

            // Live stream: subscribe, then run a session and expect lifecycle and frame events.
            using HttpRequestMessage streamRequest = new(HttpMethod.Get, AgentApi.Prefix + "/stream");
            using HttpResponseMessage streamResponse = await http.SendAsync(streamRequest, HttpCompletionOption.ResponseHeadersRead);
            Check.Equal("text/event-stream", streamResponse.Content.Headers.ContentType?.MediaType);
            using StreamReader events = new(await streamResponse.Content.ReadAsStreamAsync());
            Task flow = OfficialLoginFlowAsync(status.Port, sendInvalidFrame: false);
            HashSet<string> seen = [];
            using (CancellationTokenSource limit = new(TimeSpan.FromSeconds(10)))
                while (!(seen.Contains("event: session") && seen.Contains("event: frame")) && await events.ReadLineAsync(limit.Token) is { } line)
                    if (line.StartsWith("event: ", StringComparison.Ordinal)) seen.Add(line);
            Check.True(seen.Contains("event: session") && seen.Contains("event: frame"));
            await flow;
            await WaitUntilAsync(() => runtime.LiveSessions().Count == 0, "session closed");

            SessionInfo[] sessions = (await http.GetFromJsonAsync<SessionInfo[]>(AgentApi.Prefix + "/sessions?include_closed=true", ContractJson.Options))!;
            Check.Equal(1, sessions.Length);
            string key = sessions[0].Key;
            TracePage page = (await http.GetFromJsonAsync<TracePage>($"{AgentApi.Prefix}/trace/frames?session={key}&dir=c2s&type=0x2EE5,LoginReq", ContractJson.Options))!;
            Check.Equal(2, page.Frames.Count);
            Check.Equal(HttpStatusCode.BadRequest, (await http.GetAsync(AgentApi.Prefix + "/trace/frames?dir=sideways")).StatusCode);
            HttpResponseMessage pcap = await http.GetAsync($"{AgentApi.Prefix}/trace/sessions/{key}/export?format=pcap");
            Check.Equal(HttpStatusCode.OK, pcap.StatusCode);
            Check.Equal(0xA1B23C4Du, BinaryPrimitives.ReadUInt32LittleEndian(await pcap.Content.ReadAsByteArrayAsync()));
            string jsonl = await http.GetStringAsync($"{AgentApi.Prefix}/trace/sessions/{key}/export?format=jsonl");
            Check.Equal(7, jsonl.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
            Check.True(!jsonl.Contains(Convert.ToBase64String(PrivateIdentity)[..20], StringComparison.Ordinal));
            string metrics = await http.GetStringAsync(AgentApi.MetricsPath);
            Check.True(metrics.Contains("c9_sessions_total{service=\"login\"} 1", StringComparison.Ordinal), metrics);
            HttpResponseMessage bad = await http.PostAsJsonAsync(AgentApi.Prefix + "/control", new ControlRequest("explode"), ContractJson.Options);
            Check.Equal(HttpStatusCode.BadRequest, bad.StatusCode);
            HttpResponseMessage restart = await http.PostAsJsonAsync(AgentApi.Prefix + "/control", new ControlRequest(ControlActions.Restart), ContractJson.Options);
            Check.Equal(HttpStatusCode.OK, restart.StatusCode);
            ServiceEvent[] recent = (await http.GetFromJsonAsync<ServiceEvent[]>(AgentApi.Prefix + "/events?limit=50", ContractJson.Options))!;
            Check.True(recent.Any(e => e.Event == "control" && e.State == "restart") && recent.All(e => e.SessionKey is null || e.SessionKey == key));
            Check.Equal(HttpStatusCode.NotFound, (await http.PostAsync(AgentApi.Prefix + "/sessions/nope/disconnect", null)).StatusCode);
            await agent.StopAsync();
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static void AgentTokenAndContainerBind()
    {
        string root = Directory.CreateTempSubdirectory("c9-token-").FullName;
        try
        {
            string path = Path.Combine(root, "nested", "agent.token");
            AgentToken first = AgentToken.LoadOrCreate(path);
            Check.True(first.Value.Length == 64 && first.Value.All(Uri.IsHexDigit));
            Check.Equal(first.Value, AgentToken.LoadOrCreate(path).Value);
            Check.True(first.Matches("Bearer " + first.Value) && first.Matches("Bearer " + first.Value.ToUpperInvariant()));
            Check.True(!first.Matches(first.Value) && !first.Matches("Bearer " + new string('a', 64)) && !first.Matches(null));
            File.WriteAllText(path, "not-a-token");
            Check.Throws<InvalidDataException>(() => AgentToken.LoadOrCreate(path, TimeSpan.Zero));
        }
        finally { Directory.Delete(root, recursive: true); }

        ServerOptions login = OfficialLoginOptions();
        Check.Throws<ArgumentException>(() => (login with { Host = "0.0.0.0" }).Validate());
        (login with { Host = "0.0.0.0", AllowContainerBind = true }).Validate();
        (login with { Host = "::", AllowContainerBind = true }).Validate();
        Check.Throws<ArgumentException>(() => (login with { Host = "192.168.1.5", AllowContainerBind = true }).Validate());
        Check.Throws<ArgumentException>(() => (login with { Host = "localhost", AllowContainerBind = true }).Validate());
    }

    private static void TracePayloadPolicy()
    {
        SessionContext session = new(7, "login-test-7", "login");
        byte[] credential = new byte[1092];
        PrivateIdentity.CopyTo(credential, 0);
        Packet login = new(0x2F04, credential, flag: 0);
        Packet list = new(0x2EE6, new byte[] { 1, 2, 3 }, flag: 0);
        TraceFrame Capture(string policy, Packet? packet, byte[] wire, string state = "connected")
        {
            PacketTracer tracer = new("login", "p0l1cy", new TraceOptions { PayloadPolicy = policy, RingCapacity = 100 });
            tracer.Frame(session, packet?.Type == 0x2EE6 ? TraceDirections.Outbound : TraceDirections.Inbound, 0, wire, packet, state,
                packet is null ? "ChecksumMismatch: Checksum mismatch" : null, 0, null);
            return tracer.Query(new TraceQuery()).Frames.Single();
        }
        byte[] loginWire = login.ToDecodedBytes();
        TraceFrame redacted = Capture(TracePayloadPolicies.RedactCredentials, login, loginWire);
        Check.True(redacted.Redacted && redacted.Wire is null && redacted.Payload is null && redacted.PayloadSha256 is null);
        TraceFrame full = Capture(TracePayloadPolicies.Full, login, loginWire);
        Check.True(!full.Redacted && Convert.FromBase64String(full.Payload!).AsSpan().StartsWith(PrivateIdentity));
        Check.Bytes(loginWire, Convert.FromBase64String(full.Wire!));
        TraceFrame headers = Capture(TracePayloadPolicies.HeadersOnly, list, list.ToDecodedBytes(), "server_list_sent");
        Check.True(headers.Redacted && headers.Wire is null && headers.Payload is null && headers.PayloadSha256 is { Length: 16 });
        TraceFrame kept = Capture(TracePayloadPolicies.RedactCredentials, list, list.ToDecodedBytes(), "server_list_sent");
        Check.True(!kept.Redacted && kept.Payload == Convert.ToBase64String(new byte[] { 1, 2, 3 }));
        // An undecodable plain frame still exposes a sensitive type in its header and is redacted.
        TraceFrame failedPlain = Capture(TracePayloadPolicies.RedactCredentials, null, loginWire);
        Check.True(failedPlain.Type == 0x2F04 && failedPlain.Redacted && failedPlain.Wire is null);
        // An undecodable encrypted first request keeps only its first bytes.
        byte[] encrypted = new ConnectionCodec().Encode(new Packet(0x2EE1, credential));
        TraceFrame failedEncrypted = Capture(TracePayloadPolicies.RedactCredentials, null, encrypted);
        Check.True(failedEncrypted.Redacted && Convert.FromBase64String(failedEncrypted.Wire!).Length == TraceOptions.RedactedFailurePrefix);
        Check.Throws<ArgumentException>(() => new TraceOptions { PayloadPolicy = "everything" }.Validate());
        Check.True(KnownPackets.IsSensitive(0x2F04) && KnownPackets.IsSensitive(0x2EE1) &&
            KnownPackets.IsSensitive(0x2EE2) && KnownPackets.IsSensitive(0x2EE8) && !KnownPackets.IsSensitive(0x2EE5));
        // The launcher handoff key (2EE2 reply, 2EE8 return request) is a bearer credential for the lobby: never traced.
        foreach (Packet handoff in new[] { new Packet(0x2EE2, new byte[20], flag: 0), new Packet(0x2EE8, new byte[28]) })
        {
            TraceFrame hidden = Capture(TracePayloadPolicies.RedactCredentials, handoff, handoff.ToDecodedBytes());
            Check.True(hidden.Redacted && hidden.Wire is null && hidden.Payload is null && hidden.PayloadSha256 is null);
        }
        Check.Equal("WorldListAck", KnownPackets.NameOf(0x2EE6));
        Check.True(AgentHost.TryParseType("0x2ee5", out ushort hex) && hex == 0x2EE5);
        Check.True(AgentHost.TryParseType("12006", out ushort dec) && dec == 0x2EE6);
        Check.True(AgentHost.TryParseType("loginreq", out ushort named) && named == 0x2F04);
        Check.True(!AgentHost.TryParseType("0xZZ", out _));
    }

    private static List<byte[]> ReadPcap(byte[] file)
    {
        Check.Equal(0xA1B23C4Du, BinaryPrimitives.ReadUInt32LittleEndian(file));
        Check.Equal(101u, BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(20)));
        List<byte[]> packets = [];
        for (int offset = 24; offset < file.Length;)
        {
            int length = (int)BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(offset + 8));
            packets.Add(file.AsSpan(offset + 16, length).ToArray());
            offset += 16 + length;
        }
        return packets;
    }

    private static bool IpChecksumValid(byte[] packet)
    {
        uint sum = 0;
        for (int i = 0; i < 20; i += 2) sum += BinaryPrimitives.ReadUInt16BigEndian(packet.AsSpan(i));
        while (sum >> 16 != 0) sum = (sum & 0xFFFF) + (sum >> 16);
        return sum == 0xFFFF && BinaryPrimitives.ReadUInt16BigEndian(packet.AsSpan(2)) == packet.Length;
    }
}
