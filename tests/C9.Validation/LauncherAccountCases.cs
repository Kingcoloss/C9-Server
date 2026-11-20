using System.Buffers.Binary;
using System.Net;
using System.Text;
using C9.AuthService;
using C9.Contracts;
using C9.LocalLauncher;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private sealed class ManualClock(DateTimeOffset start) : TimeProvider
    {
        public DateTimeOffset Now = start;
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public readonly List<HttpRequestMessage> Requests = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Requests.Add(request);
            return Task.FromResult(respond(request));
        }
    }

    private sealed class StubAuthority : ILauncherAuthority
    {
        public readonly Dictionary<string, LauncherHandoff> Tickets = new(StringComparer.Ordinal);
        public readonly List<string> Redeemed = [];
        public ValueTask<LauncherHandoff?> RedeemTicketAsync(string ticket, CancellationToken token)
        {
            Redeemed.Add(ticket);
            return ValueTask.FromResult(Tickets.Remove(ticket, out LauncherHandoff? handoff) ? handoff : null);
        }
        public ValueTask<TicketOwner?> ResolveHandoffAsync(long accountId, ulong key, CancellationToken token) =>
            ValueTask.FromResult(Tickets.Count == 0 && accountId == 4 && key == 0x1122334455667788
                ? new TicketOwner("zoomer", 4) : (TicketOwner?)null);
    }

    // Layout of the official build's 0x2F04 (trace login-ced44f-7, 2026-11-16): UTF-16LE ticket field at 0,
    // "c9-gl:{13-digit account}" at 0x200 and the service code at 0x400.
    private static byte[] LauncherLoginPayload(string escapedTicket, string account)
    {
        byte[] payload = new byte[LauncherLoginPacket.PayloadBytes];
        Encoding.Unicode.GetBytes(escapedTicket).CopyTo(payload, LauncherLoginPacket.TicketOffset);
        Encoding.Unicode.GetBytes(account).CopyTo(payload, LauncherLoginPacket.AccountOffset);
        Encoding.Unicode.GetBytes("c9-gl").CopyTo(payload, 0x400);
        return payload;
    }

    private static void LauncherLoginPacketLayout()
    {
        byte[] ticket = Enumerable.Range(1, 16).Select(value => (byte)(value * 15)).ToArray();
        string escaped = Uri.EscapeDataString(Convert.ToBase64String(ticket));
        Check.True(LauncherLoginPacket.TryParse(LauncherLoginPayload(escaped, "c9-gl:0000000000004"), out string parsed, out long account));
        Check.Equal(Convert.ToBase64String(ticket), parsed);
        Check.Equal(4L, account);
        // The field L9 builds is exactly what the client copies into 0x2F04.
        string[] fields = LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000, LocalLaunchPlan.Observed20261114,
            compatibilityTest: true, identity: new LaunchIdentity("zoomer", 4, ticket)).Arguments.Split(':');
        Check.True(LauncherLoginPacket.TryParse(LauncherLoginPayload(fields[3], $"{fields[4]}:{fields[5]}"), out string fromPlan, out _));
        Check.Equal(parsed, fromPlan);
        foreach (byte[] invalid in new[]
        {
            new byte[LauncherLoginPacket.PayloadBytes], new byte[1091],
            LauncherLoginPayload("AAAAAAAAAAAAAAAAAAAAAA%3D%3D", "c9-gl:0000000000001"), // the old lab argument (zero ticket)
            LauncherLoginPayload(escaped[..10], "c9-gl:0000000000004"),
            LauncherLoginPayload(escaped, "c9-gl:00000000000x4"), LauncherLoginPayload(escaped, "c9-gl:0000000000000"),
            LauncherLoginPayload(escaped, ""),
        })
            Check.True(!LauncherLoginPacket.TryParse(invalid, out _, out _));

        byte[] reply = LauncherLoginPacket.LoginReply(4, 0x1122334455667788);
        Check.Bytes(Convert.FromHexString("0400000000000000887766554433221101000000"), reply);
        byte[] lobby = new byte[LauncherLoginPacket.LobbyRequestBytes];
        reply.AsSpan(0, 16).CopyTo(lobby);
        Check.True(LauncherLoginPacket.TryReadLobbyIdentity(lobby, out long lobbyAccount, out ulong key));
        Check.Equal(4L, lobbyAccount);
        Check.Equal(0x1122334455667788ul, key);
        Check.True(!LauncherLoginPacket.TryReadLobbyIdentity(new byte[49], out _, out _));
    }

    private static void LauncherHandoffStore()
    {
        ManualClock clock = new(new DateTimeOffset(2026, 11, 16, 0, 0, 0, TimeSpan.Zero));
        LauncherSessionStore store = new(clock);
        TicketOwner owner = new("zoomer", 4);
        ulong key = store.CreateHandoff(owner);
        Check.True(key != 0);
        Check.Equal(owner, store.ResolveHandoff(4, key));
        Check.Equal(owner, store.ResolveHandoff(4, key)); // reused for every lobby reconnect
        Check.Equal<TicketOwner?>(null, store.ResolveHandoff(5, key));
        Check.Equal<TicketOwner?>(null, store.ResolveHandoff(4, key ^ 1));
        Check.Equal<TicketOwner?>(null, store.ResolveHandoff(4, 0));
        clock.Now += LauncherSessionStore.HandoffLifetime;
        Check.Equal<TicketOwner?>(null, store.ResolveHandoff(4, key));
    }

    private static async Task LauncherMandatoryLogin()
    {
        byte[] ticket = Enumerable.Range(0, 16).Select(value => (byte)(value + 1)).ToArray();
        string b64 = Convert.ToBase64String(ticket);
        StubAuthority authority = new();
        authority.Tickets[b64] = new LauncherHandoff("zoomer", 4, 0x1122334455667788);
        ServerOptions login = OfficialLoginOptions();
        CollectedEvents events = new();
        await using (MemorySession session = new(login, new SessionProcessor(login, events, authority), events))
        {
            await session.HelloAsync();
            await session.SendAsync(new Packet(0x2F04, LauncherLoginPayload(Uri.EscapeDataString(b64), "c9-gl:0000000000004"), flag: 0));
            Packet reply = await session.ReceiveAsync();
            Check.Equal((ushort)0x2EE2, reply.Type);
            Check.Equal((byte)0, reply.Flag);
            Check.Bytes(LauncherLoginPacket.LoginReply(4, 0x1122334455667788), reply.Payload.Span);
            await session.SendAsync(new Packet(0x2EF6, new byte[4], flag: 0));
            Check.Equal((ushort)0x2EF7, (await session.ReceiveAsync()).Type);
            Check.True(events.Values.Any(e => e.Event == "launcher_login"));
            Check.True(!events.Values.Any(e => e.Event == "session_error"));
        }
        // Reused ticket, no ticket (old lab argument) and a ticket for another account are all refused.
        authority.Tickets["AAECAwQFBgcICQoLDA0ODw=="] = new LauncherHandoff("other", 9, 5);
        foreach (byte[] payload in new[]
        {
            LauncherLoginPayload(Uri.EscapeDataString(b64), "c9-gl:0000000000004"),
            LauncherLoginPayload("AAAAAAAAAAAAAAAAAAAAAA%3D%3D", "c9-gl:0000000000001"),
            LauncherLoginPayload("AAECAwQFBgcICQoLDA0ODw%3D%3D", "c9-gl:0000000000004"),
        })
        {
            CollectedEvents rejected = new();
            await using MemorySession session = new(login, new SessionProcessor(login, rejected, authority), rejected);
            await session.HelloAsync();
            await session.SendAsync(new Packet(0x2F04, payload, flag: 0));
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.Equal(1, rejected.Values.Count(e => e.Event == "session_error"));
            Check.Equal(1, rejected.Values.Count(e => e.Event == "sent")); // only the greeting
        }

        string store = Path.Combine(Path.GetTempPath(), $"c9-launcher-lobby-{Guid.NewGuid():N}.json");
        try
        {
            ServerOptions lobby = new() { Profile = "character_lobby", CharacterStore = store, TimeoutSeconds = 3 };
            byte[] handoff = new byte[49];
            LauncherLoginPacket.LoginReply(4, 0x1122334455667788).AsSpan(0, 16).CopyTo(handoff);
            CollectedEvents accepted = new();
            await using (MemorySession session = new(lobby, new SessionProcessor(lobby, accepted, authority), accepted))
            {
                Check.Equal((ushort)4, (await session.ReceiveAsync()).Type);
                authority.Tickets.Clear();
                await session.SendAsync(new Packet(0x1771, handoff));
                Check.Equal((ushort)0x0EFA, (await session.ReceiveAsync()).Type);
                Check.True(accepted.Values.Any(e => e.Event == "launcher_identity" && e.Reason == "account 4"));
            }
            foreach (byte[] payload in new[] { new byte[49], handoff.Select((b, i) => i == 8 ? (byte)(b ^ 1) : b).ToArray() })
            {
                CollectedEvents rejected = new();
                await using MemorySession session = new(lobby, new SessionProcessor(lobby, rejected, authority), rejected);
                Check.Equal((ushort)4, (await session.ReceiveAsync()).Type);
                await session.SendAsync(new Packet(0x1771, payload));
                await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
                Check.Equal(1, rejected.Values.Count(e => e.Event == "session_error"));
            }
        }
        finally { File.Delete(store); }
    }

    // 0x2EE8 as the client builds it after 0x2EEB: the 2EE2 block in stored order (key, then account), 12 zero bytes.
    private static byte[] ReturnRequest(long accountId, ulong key)
    {
        byte[] request = new byte[LauncherLoginPacket.ReturnRequestBytes];
        BinaryPrimitives.WriteUInt64LittleEndian(request, key);
        BinaryPrimitives.WriteInt64LittleEndian(request.AsSpan(8), accountId);
        return request;
    }

    // The fixture-era 2EE8 from trace login-ced44f-8 (account 1, key 0): no launcher handoff.
    private static readonly byte[] FixtureReturnRequest =
        Convert.FromHexString("00000000000000000100000000000000000000000000000000000000");

    private static void LauncherReturnPacketLayout()
    {
        Check.True(LauncherLoginPacket.TryReadReturnIdentity(ReturnRequest(4, 0x1122334455667788), out long account, out ulong key));
        Check.Equal(4L, account);
        Check.Equal(0x1122334455667788ul, key);
        // Same bytes the client stored from 2EE2 (account at +8, key at +0 of its block).
        byte[] reply = LauncherLoginPacket.LoginReply(4, 0x1122334455667788);
        Check.Bytes(reply.AsSpan(8, 8), ReturnRequest(4, 0x1122334455667788).AsSpan(0, 8));
        Check.Bytes(reply.AsSpan(0, 8), ReturnRequest(4, 0x1122334455667788).AsSpan(8, 8));
        foreach (byte[] invalid in new[]
        {
            FixtureReturnRequest, new byte[LauncherLoginPacket.ReturnRequestBytes], ReturnRequest(4, 0x1122334455667788)[..27],
            ReturnRequest(4, 0x1122334455667788).Concat(new byte[1]).ToArray(), ReturnRequest(0, 0x1122334455667788), ReturnRequest(-4, 0x1122334455667788),
        })
            Check.True(!LauncherLoginPacket.TryReadReturnIdentity(invalid, out _, out _));
    }

    // "Return to character select": lobby 0x2EEB -> the client reconnects to login with 0x2EE8. The handoff is checked
    // with c9-auth, then 2EE9 + 2EF7 + 2EE6 are sent because the client's server-select stage does not ask again.
    private static async Task LauncherReturnToCharacterSelect()
    {
        StubAuthority authority = new(); // no tickets: resolves account 4 / key 0x1122334455667788
        ServerOptions login = OfficialLoginOptions();
        CollectedEvents events = new();
        await using (MemorySession session = new(login, new SessionProcessor(login, events, authority), events))
        {
            await session.HelloAsync();
            await session.SendAsync(new Packet(0x2EE8, ReturnRequest(4, 0x1122334455667788)));
            Check.Equal((ushort)0x2EE9, (await session.ReceiveAsync()).Type);
            Packet ready = await session.ReceiveAsync();
            Check.Equal((ushort)0x2EF7, ready.Type);
            Check.Equal((byte)0, ready.Flag);
            Check.Bytes(new byte[4], ready.Payload.Span);
            Packet list = await session.ReceiveAsync();
            Check.Equal((ushort)0x2EE6, list.Type);
            Check.Equal((byte)0, list.Flag);
            Check.Equal(53, list.Payload.Length);
            // The server-select stage can still resend 2EF6 and must reach the lobby redirect with 2EEC -> 2EED.
            await session.SendAsync(new Packet(0x2EF6, new byte[4], flag: 0));
            Check.Equal((ushort)0x2EF7, (await session.ReceiveAsync()).Type);
            await session.SendAsync(new Packet(0x2EEC, new byte[20]));
            Check.Equal((ushort)0x2EED, (await session.ReceiveAsync()).Type);
            Check.True(events.Values.Any(e => e.Event == "launcher_return" && e.Reason == "account 4"));
            Check.True(!events.Values.Any(e => e.Event == "session_error"));
        }
        // No handoff (fixture-era 2EE8), a wrong key and another account are refused before any reply.
        foreach (byte[] payload in new[] { FixtureReturnRequest, ReturnRequest(4, 0x1122334455667789), ReturnRequest(5, 0x1122334455667788) })
        {
            CollectedEvents rejected = new();
            await using MemorySession session = new(login, new SessionProcessor(login, rejected, authority), rejected);
            await session.HelloAsync();
            await session.SendAsync(new Packet(0x2EE8, payload));
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.Equal(1, rejected.Values.Count(e => e.Event == "launcher_return_rejected"));
            Check.Equal(1, rejected.Values.Count(e => e.Event == "session_error"));
            Check.Equal(1, rejected.Values.Count(e => e.Event == "sent")); // only the greeting
        }
    }

    private static HttpResponseMessage JsonReply(HttpStatusCode status, string json) =>
        new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    private static void LauncherIdentityArguments()
    {
        byte[] ticket = Enumerable.Range(1, 16).Select(value => (byte)(value * 15)).ToArray();
        LaunchIdentity identity = new("player.one", 42, ticket);
        LocalLaunchPlan plan = LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000,
            LocalLaunchPlan.Observed20261114, compatibilityTest: true, identity: identity);
        string[] fields = plan.Arguments.Split(':');
        Check.Equal(11, fields.Length);
        Check.Equal("fromVLauncher", fields[0]);
        Check.Bytes(ticket, Convert.FromBase64String(Uri.UnescapeDataString(fields[3])));
        Check.Equal("0000000000042", fields[5]);
        Check.Equal("player.one", fields[6]);
        Check.Equal("127.0.0.1", fields[8]);
        Check.True(!plan.Arguments.Contains(' ') && !plan.Arguments.Contains('"'));
        // Without a launcher login the synthetic lab argument is unchanged.
        LocalLaunchPlan lab = LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000,
            LocalLaunchPlan.Observed20261114, compatibilityTest: true);
        Check.Equal("fromVLauncher::VALOFE:AAAAAAAAAAAAAAAAAAAAAA%3D%3D:c9-gl:0000000000001:test::127.0.0.1:14000:EN", lab.Arguments);
        foreach (LaunchIdentity invalid in new[]
        {
            new LaunchIdentity("player:one", 42, ticket), new LaunchIdentity("ab", 42, ticket),
            new LaunchIdentity("player", 0, ticket), new LaunchIdentity("player", 42, new byte[8]),
        })
            Check.Throws<ArgumentException>(() => LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000,
                LocalLaunchPlan.Observed20261114, compatibilityTest: true, identity: invalid));
    }

    private static void LauncherAuthUrlPolicy()
    {
        Check.Equal("http://127.0.0.1:14890/", LauncherAuthClient.ValidateUrl("http://127.0.0.1:14890").ToString());
        Check.Equal("https://auth.example.org/", LauncherAuthClient.ValidateUrl("https://auth.example.org").ToString());
        LauncherAuthClient.ValidateUrl("http://localhost:14890");
        foreach (string invalid in new[] { "http://192.168.1.5:14890", "ftp://127.0.0.1", "http://user:pw@127.0.0.1:14890",
            "http://127.0.0.1:14890/launcher", "", "not a url" })
            Check.Throws<ArgumentException>(() => LauncherAuthClient.ValidateUrl(invalid));
    }

    private static async Task LauncherAuthClientFlow()
    {
        StubHandler handler = new(request => request.RequestUri!.AbsolutePath switch
        {
            LauncherAuthRoutes.Login => JsonReply(HttpStatusCode.OK,
                """{"login":"player","account_id":7,"session_token":"tok","expires_at":"2026-11-16T00:00:00+00:00"}"""),
            LauncherAuthRoutes.Register => JsonReply(HttpStatusCode.Conflict, """{"code":"login_taken","message":"ชื่อผู้ใช้นี้ถูกใช้แล้ว"}"""),
            LauncherAuthRoutes.Ticket => JsonReply(HttpStatusCode.OK,
                """{"login":"player","account_id":7,"ticket":"AAECAwQFBgcICQoLDA0ODw==","expires_at":"2026-11-16T00:00:00+00:00"}"""),
            _ => new HttpResponseMessage(HttpStatusCode.NoContent),
        });
        using LauncherAuthClient client = new(LauncherAuthClient.DefaultUrl, handler);
        AuthResult<LauncherSession> login = await client.LoginAsync(" player ", "secret-password");
        Check.True(login.Ok);
        Check.Equal(7L, login.Value!.AccountId);
        AuthResult<LauncherSession> register = await client.RegisterAsync("player", "secret-password");
        Check.True(!register.Ok);
        Check.Equal("login_taken", register.ErrorCode);
        AuthResult<LauncherTicket> ticket = await client.TicketAsync("tok");
        Check.True(ticket.Ok);
        Check.Equal("Bearer tok", handler.Requests[^1].Headers.Authorization!.ToString());
        Check.Bytes(Enumerable.Range(0, 16).Select(value => (byte)value).ToArray(), LaunchIdentity.From(ticket.Value!).Ticket);
        Check.True(handler.Requests.All(request => request.Method == HttpMethod.Post));

        StubHandler offline = new(_ => throw new HttpRequestException("refused"));
        using LauncherAuthClient unreachable = new(LauncherAuthClient.DefaultUrl, offline);
        AuthResult<LauncherSession> failed = await unreachable.LoginAsync("player", "secret-password");
        Check.Equal("unreachable", failed.ErrorCode);
    }

    private static async Task LauncherStatClientFlow()
    {
        const string sheet = """{"character_id":11,"name":"Lab","class_name":"Fighter","level":3,"earned_points":15,"spent_points":2,"available_points":13,"stats":[{"key":"strength","name":"Strength","upgradable":true,"per_point":1,"base":10,"points":2,"total":20,"level_bonus":8}]}""";
        string? body = null;
        StubHandler handler = new(request => (body = request.Content?.ReadAsStringAsync().Result, request.RequestUri!.AbsolutePath) switch
        {
            (_, LauncherAuthRoutes.CharacterStats) => JsonReply(HttpStatusCode.OK, $$"""{"characters":[{{sheet}}]}"""),
            (_, "/launcher/v1/characters/11/stats") => JsonReply(HttpStatusCode.OK, sheet),
            _ => JsonReply(HttpStatusCode.BadRequest, """{"code":"invalid","message":"point ไม่พอหรือจำนวนไม่ถูกต้อง"}"""),
        });
        using LauncherAuthClient client = new(LauncherAuthClient.DefaultUrl, handler);
        AuthResult<LauncherStatSheets> sheets = await client.StatsAsync("tok");
        Check.True(sheets.Ok);
        Check.Equal(HttpMethod.Get, handler.Requests[^1].Method);
        Check.Equal("Bearer tok", handler.Requests[^1].Headers.Authorization!.ToString());
        LauncherCharacterStats first = sheets.Value!.Characters.Single();
        Check.Equal(13, first.AvailablePoints);
        Check.Equal(20, first.Stats.Single().Total);
        Check.Equal(8, first.Stats.Single().LevelBonus);
        Guid request = Guid.NewGuid();
        AuthResult<LauncherCharacterStats> spent = await client.SpendStatsAsync("tok", 11,
            new LauncherStatSpend(request, new Dictionary<string, int> { ["strength"] = 1 }));
        Check.True(spent.Ok);
        Check.Equal(HttpMethod.Post, handler.Requests[^1].Method);
        Check.True(body!.Contains($"\"request_id\":\"{request}\"") && body.Contains("\"add\":{\"strength\":1}"), body);
        AuthResult<LauncherCharacterStats> refused = await client.SpendStatsAsync("tok", 12,
            new LauncherStatSpend(Guid.NewGuid(), new Dictionary<string, int> { ["strength"] = 99 }));
        Check.Equal("invalid", refused.ErrorCode);

        ManualClock clock = new(new DateTimeOffset(2026, 11, 19, 0, 0, 0, TimeSpan.Zero));
        LauncherSessionStore store = new(clock);
        LauncherSession session = store.CreateSession("player", 7);
        Check.Equal(new TicketOwner("player", 7), store.FindSession(session.SessionToken));
        Check.Equal<TicketOwner?>(null, store.FindSession("not-a-session"));
        clock.Now += LauncherSessionStore.SessionLifetime;
        Check.Equal<TicketOwner?>(null, store.FindSession(session.SessionToken));
    }

    private static void LauncherTicketStore()
    {
        ManualClock clock = new(new DateTimeOffset(2026, 11, 16, 0, 0, 0, TimeSpan.Zero));
        LauncherSessionStore store = new(clock);
        LauncherSession session = store.CreateSession("player", 7);
        Check.True(session.SessionToken.Length >= 40);
        Check.Equal<LauncherTicket?>(null, store.IssueTicket("not-a-session"));
        LauncherTicket ticket = store.IssueTicket(session.SessionToken)!;
        Check.Equal(LauncherAuthRoutes.TicketBytes, Convert.FromBase64String(ticket.Ticket).Length);
        Check.Equal(new TicketOwner("player", 7), store.ConsumeTicket(ticket.Ticket));
        Check.Equal<TicketOwner?>(null, store.ConsumeTicket(ticket.Ticket)); // single use
        Check.Equal<TicketOwner?>(null, store.ConsumeTicket("%%%"));
        Check.Equal<TicketOwner?>(null, store.ConsumeTicket(Convert.ToBase64String(new byte[8])));

        LauncherTicket late = store.IssueTicket(session.SessionToken)!;
        clock.Now += LauncherSessionStore.TicketLifetime + TimeSpan.FromSeconds(1);
        Check.Equal<TicketOwner?>(null, store.ConsumeTicket(late.Ticket)); // expired

        Check.True(store.RevokeSession(session.SessionToken));
        Check.Equal<LauncherTicket?>(null, store.IssueTicket(session.SessionToken));
        LauncherSession old = store.CreateSession("player", 7);
        clock.Now += LauncherSessionStore.SessionLifetime;
        Check.Equal<LauncherTicket?>(null, store.IssueTicket(old.SessionToken));
    }
}
