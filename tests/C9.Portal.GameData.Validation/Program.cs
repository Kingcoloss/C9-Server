using System.Diagnostics;
using System.Text.Json;
using C9.Contracts;
using C9.GameData;
using C9.GameData.Entities;
using C9.Portal.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

// Usage: C9.Portal.GameData.Validation   (set C9_DB_ADMIN_CONNECTION for the database cases)
int failed = 0, total = 0;
long assertions = 0;
Stopwatch watch = Stopwatch.StartNew();

void True(bool value, string message)
{
    Interlocked.Increment(ref assertions);
    if (!value) throw new InvalidOperationException(message);
}
void Status(int expected, (int Status, JsonElement Body) response, string what) =>
    True(response.Status == expected, $"{what}: expected HTTP {expected}, got {response.Status} {response.Body}");

async Task Case(string name, Func<Task> run)
{
    total++;
    try { await run(); Console.WriteLine($"PASS {name}"); }
    catch (Exception error) { failed++; Console.WriteLine($"FAIL {name}: {error.Message}"); if (Environment.GetEnvironmentVariable("C9_VALIDATION_VERBOSE") is not null) Console.WriteLine(error); }
}

await using (PortalTestHost bare = await PortalTestHost.StartAsync(null))
{
    await Case("without a database the modules answer 503 with setup guidance", async () =>
    {
        var accounts = await bare.Get("/api/v1/accounts/");
        Status(503, accounts, "accounts");
        True(accounts.Body.GetProperty("detail").GetString()!.Contains("C9_DB_CONNECTION"), "guidance names the variable");
        Status(503, await bare.Get("/api/v1/world/rates"), "rates");
        Status(503, await bare.Post("/api/v1/world/events", new { name = "x", reason = "r" }), "event write");
        Status(200, await bare.Get("/api/v1/world/population"), "population still works from live sessions");
    });
}

string? admin = Environment.GetEnvironmentVariable("C9_DB_ADMIN_CONNECTION");
if (admin is null)
{
    Console.WriteLine($"{total - failed}/{total} passed; database cases skipped (set C9_DB_ADMIN_CONNECTION)");
    return failed == 0 ? 0 : 1;
}

string database = "c9_portal_validation_" + Guid.NewGuid().ToString("N")[..12];
await using (NpgsqlConnection connection = new(admin))
{
    await connection.OpenAsync();
    await using NpgsqlCommand create = new($"CREATE DATABASE {database}", connection); // generated hex name, not input
    await create.ExecuteNonQueryAsync();
}

try
{
    await using PortalTestHost h = await PortalTestHost.StartAsync(new NpgsqlConnectionStringBuilder(admin) { Database = database }.ConnectionString);
    long accountId = 0, characterId = 0;
    uint characterVersion = 0;

    await Case("authentication and RBAC are enforced per endpoint", async () =>
    {
        Status(401, await h.SendAsync(HttpMethod.Get, "/api/v1/accounts/", null), "anonymous");
        Status(403, await h.Get("/api/v1/accounts/", "ContentEditor"), "ContentEditor cannot read accounts");
        Status(200, await h.Get("/api/v1/accounts/", "ReadOnly"), "ReadOnly can read accounts");
        Status(403, await h.Post("/api/v1/accounts/", new { login_name = "nope", reason = "r" }, "ReadOnly"), "ReadOnly cannot write");
        Status(403, await h.Put("/api/v1/world/rates", new { rates = new { exp = 2 }, cap = 100, reason = "r" }, "GM"), "GM cannot change world rates");
        Status(403, await h.Put("/api/v1/world/maintenance", new { enabled = true, reason = "r" }, "ContentEditor"), "maintenance needs ServerOperate");
        Status(403, await h.Post("/api/v1/accounts/import-legacy", new { characters = Array.Empty<object>(), reason = "r" }, "GM"), "legacy import needs ServerOperate");
    });

    await Case("writes require Idempotency-Key and a reason; retries are idempotent", async () =>
    {
        Status(400, await h.SendAsync(HttpMethod.Post, "/api/v1/accounts/", "Owner", new { login_name = "player1", password = "password-1", reason = "r" }), "no key");
        Status(400, await h.Post("/api/v1/accounts/", new { login_name = "player1", password = "password-1" }), "no reason");
        Status(400, await h.Post("/api/v1/accounts/", new { login_name = "player1", password = "password-1", reason = "  " }), "blank reason");
        Guid key = Guid.NewGuid();
        var created = await h.Post("/api/v1/accounts/", new { login_name = "player1", password = "password-1", reason = "new tester" }, "GM", key);
        Status(201, created, "create");
        var again = await h.Post("/api/v1/accounts/", new { login_name = "player1", password = "password-1", reason = "new tester" }, "GM", key);
        Status(201, again, "retry");
        accountId = created.Body.GetProperty("id").GetInt64();
        True(again.Body.GetProperty("id").GetInt64() == accountId, "retry returns the same account");
        True(!created.Body.ToString().Contains("password_hash") && !created.Body.ToString().Contains("pbkdf2"), "no password material in responses");
        True(created.Body.GetProperty("password_login").GetBoolean(), "password login flag");
        Status(409, await h.Post("/api/v1/accounts/", new { login_name = "PLAYER1", password = "password-1", reason = "dup" }), "duplicate is 409");
        True(h.Audit.Entries.Any(e => e.Action == "accounts.create" && e.Success && e.Actor == "tester" && e.Reason == "new tester"), "portal audit success");
        True(h.Audit.Entries.Any(e => e.Action == "accounts.create" && !e.Success), "portal audit failure");
    });

    await Case("search input is data (injection probes), sort is a whitelist", async () =>
    {
        foreach (string probe in new[] { "' OR 1=1 --", "%", "_", "\"; DROP TABLE game.accounts; --" })
        {
            var result = await h.Get("/api/v1/accounts/?q=" + Uri.EscapeDataString(probe));
            Status(200, result, "probe " + probe);
            True(result.Body.GetProperty("total").GetInt32() == 0, "probe matched nothing: " + probe);
        }
        Status(400, await h.Get("/api/v1/accounts/?sort=password_hash"), "unknown sort value rejected");
        Status(400, await h.Get("/api/v1/accounts/?take=1000"), "page size bound");
        True((await h.Get("/api/v1/accounts/?q=PLAY")).Body.GetProperty("total").GetInt32() == 1, "case-insensitive search");
    });

    await Case("GM status needs Owner/Administrator; stale versions get 412", async () =>
    {
        var detail = await h.Get($"/api/v1/accounts/{accountId}");
        uint version = detail.Body.GetProperty("account").GetProperty("version").GetUInt32();
        Status(403, await h.Patch($"/api/v1/accounts/{accountId}", new { version, is_gm = true, reason = "promote" }, "GM"), "GM cannot grant GM");
        var updated = await h.Patch($"/api/v1/accounts/{accountId}", new { version, tags = new[] { "tester" }, note = "QA", reason = "tag" }, "GM");
        Status(200, updated, "GM can tag");
        Status(412, await h.Patch($"/api/v1/accounts/{accountId}", new { version, note = "late", reason = "stale" }), "stale version");
        uint fresh = updated.Body.GetProperty("version").GetUInt32();
        Status(200, await h.Patch($"/api/v1/accounts/{accountId}", new { version = fresh, is_gm = true, reason = "promote" }, "Administrator"), "admin grants GM");
        True((await h.Get("/api/v1/accounts/?tag=tester")).Body.GetProperty("total").GetInt32() == 1, "tag filter");
    });

    await Case("legacy import, character edits, snapshots and timeline through the API", async () =>
    {
        object[] legacy = [new { Slot = 0, Job = 1, Name = "Lab", Appearance = "AQAAAAEAAQABAH+Af39/f4B/f39/f39/f39/AAB/cgAAAAEBAAABAIBrsgA=" }];
        var imported = await h.Post("/api/v1/accounts/import-legacy", new { characters = legacy, reason = "move JSON store" });
        Status(200, imported, "import");
        True(imported.Body.GetProperty("imported").GetInt32() == 1, "one imported");
        long local = imported.Body.GetProperty("account_id").GetInt64();
        var characters = await h.Get($"/api/v1/accounts/{local}/characters");
        JsonElement lab = characters.Body.EnumerateArray().Single();
        characterId = lab.GetProperty("id").GetInt64();
        characterVersion = lab.GetProperty("version").GetUInt32();
        var renamed = await h.Post($"/api/v1/accounts/characters/{characterId}/rename", new { version = characterVersion, name = "Lab2", reason = "test rename" }, "GM");
        Status(200, renamed, "rename");
        characterVersion = renamed.Body.GetProperty("version").GetUInt32();
        Status(400, await h.Post($"/api/v1/accounts/characters/{characterId}/rename", new { version = characterVersion, name = "bad name!", reason = "x" }), "invalid name");
        var placed = await h.Patch($"/api/v1/accounts/characters/{characterId}", new { version = characterVersion, level = 5, map_id = 10, submap = 0, pos_x = 1.5, pos_y = 2, pos_z = 3, reason = "spawn" });
        Status(200, placed, "place");
        characterVersion = placed.Body.GetProperty("version").GetUInt32();
        var snapshots = await h.Get($"/api/v1/accounts/characters/{characterId}/snapshots");
        True(snapshots.Body.GetArrayLength() == 2, "two snapshots");
        var timeline = await h.Get($"/api/v1/accounts/{local}/timeline");
        True(timeline.Body.EnumerateArray().Any(t => t.GetProperty("action").GetString() == "character.rename"), "timeline shows rename");
        var export = await h.Get($"/api/v1/accounts/characters/{characterId}/export");
        Status(200, export, "export");
        Status(409, await h.Post($"/api/v1/accounts/{accountId}/characters/import", new { slot = 1, data = export.Body, reason = "copy" }), "import with a taken name");
    });

    await Case("sanctions through the API", async () =>
    {
        var issued = await h.Post($"/api/v1/accounts/{accountId}/sanctions", new { kind = "suspend", ends_at = DateTimeOffset.UtcNow.AddHours(1), reason = "spam" }, "GM");
        Status(201, issued, "suspend");
        True((await h.Get($"/api/v1/accounts/{accountId}")).Body.GetProperty("account").GetProperty("status").GetString() == "suspended", "status suspended");
        Status(400, await h.Post($"/api/v1/accounts/{accountId}/sanctions", new { kind = "suspend", reason = "no end" }), "suspend needs end");
        long sid = issued.Body.GetProperty("id").GetInt64();
        Status(200, await h.Post($"/api/v1/accounts/{accountId}/sanctions/{sid}/revoke", new { reason = "appeal" }), "revoke");
        True((await h.Get($"/api/v1/accounts/{accountId}")).Body.GetProperty("account").GetProperty("status").GetString() == "active", "status active");
        Status(404, await h.Post("/api/v1/accounts/999999/sanctions", new { kind = "ban", reason = "x" }), "unknown account");
    });

    await Case("world rates, events, maintenance and announcements", async () =>
    {
        Status(200, await h.Put("/api/v1/world/rates", new { rates = new { exp = 5, drop = 5, rare_drop = 2, upgrade = 2 }, cap = 50, reason = "launch" }, "ContentEditor"), "rates");
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var created = await h.Post("/api/v1/world/events", new { name = "Double exp", starts_at = now.AddHours(-1), ends_at = now.AddHours(1), rates = new { exp = 2 }, enabled = true, reason = "weekend" }, "ContentEditor");
        Status(201, created, "event");
        True(created.Body.GetProperty("active_now").GetBoolean(), "event active");
        var rates = await h.Get("/api/v1/world/rates");
        True(rates.Body.GetProperty("rates").GetProperty("exp").GetDecimal() == 10m, "5 × 2 = 10");
        True(rates.Body.GetProperty("rates").GetProperty("drop").GetDecimal() == 5m, "drop unaffected");
        long id = created.Body.GetProperty("id").GetInt64();
        uint version = created.Body.GetProperty("version").GetUInt32();
        Status(412, await h.Put($"/api/v1/world/events/{id}", new { version = version + 7, name = "Double exp", starts_at = now, ends_at = now.AddHours(1), enabled = true, reason = "x" }, "ContentEditor"), "stale event");
        Status(400, await h.Post("/api/v1/world/events", new { name = "Bad", starts_at = now, ends_at = now.AddHours(-1), enabled = true, reason = "x" }), "end before start");
        Status(200, await h.Post($"/api/v1/world/events/{id}/delete", new { reason = "cancel" }), "delete");
        True((await h.Get("/api/v1/world/rates")).Body.GetProperty("rates").GetProperty("exp").GetDecimal() == 5m, "event removed");
        Status(200, await h.Put("/api/v1/world/maintenance", new { enabled = true, message = "patch", until = now.AddMinutes(30), allowed_logins = new[] { "player1" }, reason = "patch" }, "Administrator"), "maintenance");
        True((await h.Get("/api/v1/world/maintenance")).Body.GetProperty("enabled").GetBoolean(), "maintenance on");
        Status(201, await h.Post("/api/v1/world/announcements", new { channel = "launcher", message = "Welcome", reason = "motd" }, "ContentEditor"), "announce");
        True((await h.Get("/api/v1/world/announcements?channel=launcher")).Body.GetArrayLength() == 1, "announcement listed");
        Status(200, await h.Put($"/api/v1/accounts/{accountId}/rates", new { rates = new { exp = 3 }, reason = "vip" }, "GM"), "account rates");
        True((await h.Get($"/api/v1/world/rates?accountId={accountId}")).Body.GetProperty("rates").GetProperty("exp").GetDecimal() == 15m, "5 × 3");
    });

    await Case("economy summary is derived from the ledger", async () =>
    {
        var empty = await h.Get("/api/v1/world/economy");
        Status(200, empty, "economy");
        True(!empty.Body.GetProperty("has_ledger_data").GetBoolean(), "no ledger yet");
        await using (GameDbContext db = h.Services.GetRequiredService<IDbContextFactory<GameDbContext>>().CreateDbContext())
        {
            Character c = await db.Characters.FirstAsync(x => x.Id == characterId);
            c.Gold = 1500;
            DateTimeOffset now = DateTimeOffset.UtcNow;
            db.Ledger.AddRange(
                new LedgerEntry { At = now.AddHours(-2), CharacterId = characterId, Asset = LedgerAsset.Gold, Delta = 2000, Reason = "drop" },
                new LedgerEntry { At = now.AddHours(-1), CharacterId = characterId, Asset = LedgerAsset.Gold, Delta = -500, Reason = "npc_shop_buy" },
                new LedgerEntry { At = now.AddHours(-1), CharacterId = characterId, Asset = LedgerAsset.Item, ItemId = 7, Delta = 1, Reason = "drop" },
                new LedgerEntry { At = now.AddDays(-40), CharacterId = characterId, Asset = LedgerAsset.Gold, Delta = 99999, Reason = "drop" });
            await db.SaveChangesAsync();
        }
        var summary = await h.Get("/api/v1/world/economy");
        True(summary.Body.GetProperty("gold_faucet").GetInt64() == 2000, "faucet excludes entries outside the range");
        True(summary.Body.GetProperty("gold_sink").GetInt64() == 500, "sink");
        True(summary.Body.GetProperty("gold_in_circulation").GetInt64() == 1500, "circulation");
        True(summary.Body.GetProperty("top_holders")[0].GetProperty("gold").GetInt64() == 1500, "top holder");
        True(summary.Body.GetProperty("daily").EnumerateArray().Sum(d => d.GetProperty("items_created").GetInt32()) == 1, "daily item count");
        Status(400, await h.Get("/api/v1/world/economy?from=2020-01-01T00:00:00Z&to=2026-01-01T00:00:00Z"), "range bound");
    });

    await Case("online sessions: population per map, audited disconnect, session history", async () =>
    {
        h.Game.Sessions.Add(FakeGameServices.Session("lobby-b1-1", 10, 0, (ulong)characterId));
        h.Game.Sessions.Add(FakeGameServices.Session("lobby-b1-2", 10, 0));
        h.Game.Sessions.Add(FakeGameServices.Session("lobby-b1-3", null));
        var population = await h.Get("/api/v1/world/population", "ReadOnly");
        True(population.Body[0].GetProperty("map_id").GetInt32() == 10 && population.Body[0].GetProperty("players").GetInt32() == 2, "two on map 10");
        Status(403, await h.Post("/api/v1/accounts/online/lobby-b1-2/disconnect", new { reason = "afk" }, "ReadOnly"), "ReadOnly cannot kick");
        Status(400, await h.Post("/api/v1/accounts/online/lobby-b1-2/disconnect", new { reason = "" }, "GM"), "reason required");
        Status(200, await h.Post("/api/v1/accounts/online/lobby-b1-2/disconnect", new { reason = "afk" }, "GM"), "kick");
        Status(404, await h.Post("/api/v1/accounts/online/missing/disconnect", new { reason = "afk" }, "GM"), "unknown session");
        True(h.Game.Disconnected.SequenceEqual(["lobby-b1-2"]), "agent asked to disconnect");
        True(h.Audit.Entries.Any(e => e.Action == "accounts.disconnect" && e.Target == "session:lobby-b1-2" && e.Success), "kick audited");

        var factory = h.Services.GetRequiredService<IDbContextFactory<GameDbContext>>();
        await using GameDbContext db = factory.CreateDbContext();
        long owner = (await db.Characters.AsNoTracking().SingleAsync(c => c.Id == characterId)).AccountId;
        // A lobby on the game database reports AccountId and database character ids.
        SessionInfo open = FakeGameServices.Session("lobby-b1-1", 10, 0, (ulong)characterId) with { AccountId = owner };
        await SessionRecorder.RecordAsync(factory, new SessionLifecycleEvent(1, DateTimeOffset.UtcNow, SessionLifecycleKinds.State, open), CancellationToken.None);
        await SessionRecorder.RecordAsync(factory, new SessionLifecycleEvent(2, DateTimeOffset.UtcNow, SessionLifecycleKinds.Closed,
            open with { ClosedUtc = DateTimeOffset.UtcNow, CloseReason = "peer_closed" }), CancellationToken.None);
        SessionRecord row = await db.Sessions.SingleAsync(s => s.Key == "lobby-b1-1");
        True(row.MapId == 10 && row.CharacterId == characterId && row.AccountId == owner && row.EndReason == "peer_closed" && row.EndedAt is not null, "session persisted");
        True(row.RemoteAddress?.ToString() == "127.0.0.1", "remote address parsed");
        // A JSON-store lobby reports Slot+1 without an account: never linked to a database character.
        await SessionRecorder.RecordAsync(factory, new SessionLifecycleEvent(3, DateTimeOffset.UtcNow, SessionLifecycleKinds.State,
            FakeGameServices.Session("lobby-b1-9", 10, 0, 1)), CancellationToken.None);
        SessionRecord legacy = await db.Sessions.AsNoTracking().SingleAsync(s => s.Key == "lobby-b1-9");
        True(legacy.CharacterId is null && legacy.AccountId is null, "Slot+1 ids are not linked");
    });

    await Case("module pages render with data (Owner) and hide write forms (ReadOnly)", async () =>
    {
        async Task<string> Html(string path, string role)
        {
            (int status, string html) = await h.Page(path, role);
            True(status == 200, $"GET {path} as {role} -> {status}");
            // Optional: keep the prerendered HTML for a visual check (C9_VALIDATION_DUMP=<folder>).
            if (Environment.GetEnvironmentVariable("C9_VALIDATION_DUMP") is { Length: > 0 } dump)
            {
                Directory.CreateDirectory(dump);
                await File.WriteAllTextAsync(Path.Combine(dump, $"{path.Trim('/').Replace('/', '_')}_{role}.html"), html);
            }
            return html;
        }
        True((await h.Page("/economy", "Player")).Status == 403, "page policy enforced");
        string list = await Html("/accounts", "Owner");
        True(list.Contains("player1") && list.Contains("สร้างบัญชี"), "account list with create form");
        string listRo = await Html("/accounts", "ReadOnly");
        True(listRo.Contains("player1") && !listRo.Contains("สร้างบัญชี"), "ReadOnly sees no create form");
        string detail = await Html($"/accounts/{accountId}", "Owner");
        True(detail.Contains("player1") && detail.Contains("ประวัติการลงโทษ") && detail.Contains("เป็น GM"), "account detail");
        string character = await Html($"/characters/{characterId}", "GM");
        True(character.Contains("Lab2") && character.Contains("Snapshot"), "character detail");
        string online = await Html("/accounts/online", "GM");
        True(online.Contains("lobby-b1-1"), "online sessions");
        string world = await Html("/world", "ContentEditor");
        True(world.Contains("ใช้จริงตอนนี้") && world.Contains("Welcome") && !world.Contains("เปิดโหมดปิดปรับปรุง"), "world overview; maintenance form only for ServerOperate");
        string events = await Html("/world/events", "ContentEditor");
        True(events.Contains("สร้าง event"), "events page");
        string economy = await Html("/economy", "ReadOnly");
        True(economy.Contains("1,500") && economy.Contains("npc_shop_buy"), "economy page shows ledger data");
    });
}
finally
{
    NpgsqlConnection.ClearAllPools();
    await using NpgsqlConnection connection = new(admin);
    await connection.OpenAsync();
    await using NpgsqlCommand drop = new($"DROP DATABASE IF EXISTS {database} WITH (FORCE)", connection);
    await drop.ExecuteNonQueryAsync();
}

Console.WriteLine($"{total - failed}/{total} passed, {assertions} assertions, {watch.Elapsed.TotalSeconds:F1}s");
return failed == 0 ? 0 : 1;
