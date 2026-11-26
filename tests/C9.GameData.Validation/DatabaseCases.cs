using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

internal static partial class GameDataValidation
{
    private const int DatabaseCaseCount = 18;

    private static IEnumerable<(string, Func<Task>)> DatabaseCases(Harness h)
    {
        yield return ("migrations apply and re-apply cleanly", async () =>
        {
            await h.Services.MigrateGameDataAsync();
            await h.Services.MigrateGameDataAsync();
            await using GameDbContext db = h.Db();
            Check.Equal(0, (await db.Database.GetPendingMigrationsAsync()).Count());
            Check.Equal(0, await db.Accounts.CountAsync());
        });

        yield return ("account create, duplicate, idempotent retry, audit", async () =>
        {
            OperatorContext op = Harness.Op("create tester");
            Account first = await h.Accounts.CreateAsync("tester1", "s3cret-pass", op);
            Account retry = await h.Accounts.CreateAsync("tester1", "s3cret-pass", op);
            Check.Equal(first.Id, retry.Id);
            await Check.Fails(GameDataError.Duplicate, () => h.Accounts.CreateAsync("TESTER1", "s3cret-pass", Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Accounts.CreateAsync("a'; drop", "s3cret-pass", Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Accounts.CreateAsync("tester2", "s3cret", Harness.Op() with { Reason = " " }));
            await using GameDbContext db = h.Db();
            AuditEntry audit = await db.AuditLog.SingleAsync(a => a.RequestId == op.RequestId);
            Check.Equal("account.create", audit.Action);
            Check.True(audit.After!.Contains("tester1") && !audit.After.Contains("s3cret"), "audit never stores the password");
            Check.Equal(17, (int)(await db.Accounts.SingleAsync(a => a.Id == first.Id)).SlotCount);
        });

        yield return ("login verification with sanctions", async () =>
        {
            Account account = await h.Accounts.CreateAsync("player2", "password-2", Harness.Op());
            Check.Equal(LoginResult.Ok, (await h.Accounts.VerifyLoginAsync("PLAYER2", "password-2")).Result);
            Check.Equal(LoginResult.UnknownOrWrongPassword, (await h.Accounts.VerifyLoginAsync("player2", "password-x")).Result);
            Check.Equal(LoginResult.UnknownOrWrongPassword, (await h.Accounts.VerifyLoginAsync("nobody", "password-2")).Result);
            AccountSanction suspension = await h.Sanctions.IssueAsync(account.Id, SanctionKind.Suspend, h.Clock.Now.AddHours(2), Harness.Op("spam"));
            Check.Equal(LoginResult.Suspended, (await h.Accounts.VerifyLoginAsync("player2", "password-2")).Result);
            Check.Equal(AccountStatus.Suspended, (await h.Accounts.GetAsync(account.Id))!.Status);
            h.Clock.Now = h.Clock.Now.AddHours(3);
            Check.Equal(LoginResult.Ok, (await h.Accounts.VerifyLoginAsync("player2", "password-2")).Result);
            Check.Equal(1, await h.Sanctions.RefreshExpiredAsync());
            AccountSanction ban = await h.Sanctions.IssueAsync(account.Id, SanctionKind.Ban, null, Harness.Op("cheating"));
            Check.Equal(LoginResult.Banned, (await h.Accounts.VerifyLoginAsync("player2", "password-2")).Result);
            await h.Sanctions.RevokeAsync(account.Id, ban.Id, Harness.Op("appeal accepted"));
            Check.Equal(AccountStatus.Active, (await h.Accounts.GetAsync(account.Id))!.Status);
            await Check.Fails(GameDataError.Invalid, () => h.Sanctions.IssueAsync(account.Id, SanctionKind.Suspend, null, Harness.Op()));
            Check.True(suspension.Id > 0);
        });

        yield return ("legacy Lab import is repeatable and preserves appearance", async () =>
        {
            string path = Path.Combine(Path.GetTempPath(), "c9-legacy-" + Guid.NewGuid().ToString("N") + ".json");
            await using (Stream resource = typeof(GameDataValidation).Assembly.GetManifestResourceStream("legacy-characters.json")!)
            await using (FileStream output = File.Create(path)) await resource.CopyToAsync(output);
            try
            {
                LegacyImportReport first = await h.Importer.ImportCharactersAsync(path, Harness.Op("import lab"));
                Check.Equal(1, first.Imported);
                LegacyImportReport again = await h.Importer.ImportCharactersAsync(path, Harness.Op("import lab again"));
                Check.Equal(0, again.Imported);
                Check.Equal(1, again.Skipped);
                Account local = (await h.Accounts.FindByLoginAsync(AccountService.LocalAccount))!;
                Check.Equal(PasswordHasher.Disabled, local.PasswordHash);
                Character lab = (await h.Characters.ListAsync(local.Id)).Single();
                Check.Equal("Lab", lab.Name);
                Check.Equal((short)1, lab.Job);
                Check.True(lab.Appearance.AsSpan().SequenceEqual(Appearance));
                Check.Equal(LoginResult.PasswordLoginDisabled, (await h.Accounts.VerifyLoginAsync("local", "whatever-1")).Result);
            }
            finally { File.Delete(path); }
        });

        yield return ("lobby create rules and idempotent retry", async () =>
        {
            Account account = await h.Accounts.CreateAsync("lobby1", "password-3", Harness.Op());
            Character created = await h.Characters.CreateAsync(account.Id, 3, 9, "Hunter_1", Appearance);
            Character retry = await h.Characters.CreateAsync(account.Id, 3, 9, "Hunter_1", Appearance);
            Check.Equal(created.Id, retry.Id);
            await Check.Fails(GameDataError.Duplicate, () => h.Characters.CreateAsync(account.Id, 4, 9, "hunter_1", Appearance));
            await Check.Fails(GameDataError.Duplicate, () => h.Characters.CreateAsync(account.Id, 3, 9, "Other", Appearance));
            await Check.Fails(GameDataError.Invalid, () => h.Characters.CreateAsync(account.Id, 17, 9, "Other", Appearance));
            await Check.Fails(GameDataError.Invalid, () => h.Characters.CreateAsync(account.Id, 5, 41, "Other", Appearance));
            await Check.Fails(GameDataError.Invalid, () => h.Characters.CreateAsync(account.Id, 5, 9, "Other", new byte[43]));
            Check.True(created.Id != (await h.Characters.CreateAsync(account.Id, 0, 1, "Second", Appearance)).Id);
        });

        yield return ("edit with stale version is rejected; snapshot and audit written", async () =>
        {
            Account account = await h.Accounts.CreateAsync("editor1", "password-4", Harness.Op());
            Character c = await h.Characters.CreateAsync(account.Id, 0, 1, "Before", Appearance);
            Character renamed = await h.Characters.RenameAsync(c.Id, c.Version, "After", Harness.Op("typo"));
            Check.Equal("After", renamed.Name);
            Check.True(renamed.Version != c.Version, "xmin changes on write");
            await Check.Fails(GameDataError.Conflict, () => h.Characters.RenameAsync(c.Id, c.Version, "Third", Harness.Op()));
            IReadOnlyList<CharacterSnapshot> snapshots = await h.Characters.SnapshotsAsync(c.Id);
            Check.Equal(1, snapshots.Count);
            Check.True(snapshots[0].Data.Contains("\"Before\""));
            Character moved = await h.Characters.MoveSlotAsync(c.Id, renamed.Version, 5, Harness.Op("tidy"));
            Check.Equal((short)5, moved.Slot);
            await Check.Fails(GameDataError.Invalid, () => h.Characters.MoveSlotAsync(c.Id, moved.Version, 17, Harness.Op()));
            Character placed = await h.Characters.UpdateAsync(c.Id, moved.Version,
                new CharacterChanges(Level: 10, MapId: 10, Submap: 0, PosX: 1, PosY: 2, PosZ: 3), Harness.Op("test spawn"));
            Check.Equal((short)10, placed.Level);
            await Check.Fails(GameDataError.Invalid, () => h.Characters.UpdateAsync(c.Id, placed.Version,
                new CharacterChanges(MapId: 10), Harness.Op()));
            await using GameDbContext db = h.Db();
            Check.Equal(3, await db.AuditLog.CountAsync(a => a.TargetType == "character" && a.TargetId == c.Id.ToString()));
        });

        yield return ("soft delete frees the name; restore guards conflicts", async () =>
        {
            Account account = await h.Accounts.CreateAsync("deleter1", "password-5", Harness.Op());
            Character c = await h.Characters.CreateAsync(account.Id, 0, 1, "Ghost", Appearance);
            Character deleted = await h.Characters.SoftDeleteAsync(c.Id, c.Version, Harness.Op("player request"));
            Check.True(deleted.DeletedAt is not null);
            Check.Equal(0, (await h.Characters.ListAsync(account.Id)).Count);
            Check.Equal(1, (await h.Characters.ListAsync(account.Id, includeDeleted: true)).Count);
            await Check.Fails(GameDataError.Invalid, () => h.Characters.RenameAsync(c.Id, deleted.Version, "Ghost2", Harness.Op()));
            Character reuse = await h.Characters.CreateAsync(account.Id, 0, 1, "Ghost", Appearance);
            await Check.Fails(GameDataError.Duplicate, () => h.Characters.RestoreAsync(c.Id, deleted.Version, Harness.Op()));
            await h.Characters.SoftDeleteAsync(reuse.Id, reuse.Version, Harness.Op());
            Character restored = await h.Characters.RestoreAsync(c.Id, deleted.Version, Harness.Op("undo"));
            Check.True(restored.DeletedAt is null);
        });

        yield return ("search treats input as data (SQL injection and LIKE wildcards)", async () =>
        {
            await h.Accounts.CreateAsync("under_score", "password-6", Harness.Op());
            await h.Accounts.CreateAsync("underXscore", "password-6", Harness.Op());
            foreach (string probe in new[] { "'; DROP TABLE game.accounts; --", "' OR '1'='1", "%", "\\", "\"; SELECT pg_sleep(5); --" })
                Check.Equal(0, (await h.Accounts.SearchAsync(new AccountQuery(Text: probe))).Total);
            Check.Equal(1, (await h.Accounts.SearchAsync(new AccountQuery(Text: "under_s"))).Total);
            Check.Equal(2, (await h.Accounts.SearchAsync(new AccountQuery(Text: "UNDER"))).Total);
            Page<AccountSummary> byCharacter = await h.Accounts.SearchAsync(new AccountQuery(Text: "hunter_"));
            Check.Equal("lobby1", byCharacter.Items.Single().LoginName);
            Page<AccountSummary> sorted = await h.Accounts.SearchAsync(new AccountQuery(Sort: AccountSort.CreatedAt, Descending: true, Take: 2));
            Check.Equal(2, sorted.Items.Count);
            Check.True(sorted.Total > 2);
            await Check.Fails(GameDataError.Invalid, () => h.Accounts.SearchAsync(new AccountQuery(Take: 500)));
            await using GameDbContext db = h.Db();
            Check.True(await db.Accounts.AnyAsync(), "accounts table intact");
        });

        yield return ("effective rates combine server, events and account", async () =>
        {
            Account account = await h.Accounts.CreateAsync("vip1", "password-7", Harness.Op());
            await h.World.SetServerRatesAsync(new RateSet(5, 5, 2, 2), 50, Harness.Op("launch rates"));
            DateTimeOffset now = h.Clock.Now;
            WorldEvent weekend = await h.World.CreateEventAsync(new WorldEventInput("Double exp", null, now.AddHours(-1), now.AddHours(1),
                new RateSet(Exp: 2)), Harness.Op("weekend"));
            await h.World.CreateEventAsync(new WorldEventInput("Future", null, now.AddDays(1), now.AddDays(2), new RateSet(Exp: 10)), Harness.Op());
            await h.World.SetAccountRatesAsync(account.Id, new RateSet(Exp: 3, Drop: 0.5m), now.AddDays(7), Harness.Op("vip"));
            EffectiveRates rates = await h.World.EffectiveRatesAsync(account.Id);
            Check.Equal(30m, rates.Rates.Exp);          // 5 × 2 × 3
            Check.Equal(2.5m, rates.Rates.Drop);        // 5 × 1 × 0.5
            Check.Equal("Double exp", rates.Events.Single().Name);
            Check.Equal(10m, (await h.World.EffectiveRatesAsync()).Rates.Exp);
            await h.World.SetServerRatesAsync(new RateSet(50, 1, 1, 1), 50, Harness.Op());
            Check.Equal(50m, (await h.World.EffectiveRatesAsync(account.Id)).Rates.Exp); // capped once
            await Check.Fails(GameDataError.Conflict, () => h.World.UpdateEventAsync(weekend.Id, weekend.Version + 1,
                new WorldEventInput("Double exp", null, now, now.AddHours(1), new RateSet(Exp: 2)), Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.World.CreateEventAsync(
                new WorldEventInput("Bad", null, now, now.AddDays(2), RateSet.Neutral, RepeatEveryDays: 1), Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.World.CreateEventAsync(
                new WorldEventInput("Bad", null, now, now.AddHours(1), RateSet.Neutral, Extra: "{not json"), Harness.Op()));
            h.Clock.Now = now.AddDays(8);
            Check.Equal(50m, (await h.World.EffectiveRatesAsync(account.Id)).Rates.Exp);
            Check.Equal(1m, (await h.World.EffectiveRatesAsync(account.Id)).Account.Exp); // override expired
            h.Clock.Now = now;
        });

        yield return ("maintenance and announcements", async () =>
        {
            Check.True(!(await h.World.MaintenanceAsync()).Enabled);
            await h.World.SetMaintenanceAsync(new MaintenanceState(true, "Patch", h.Clock.Now.AddMinutes(30), ["tester1"]), Harness.Op("patch"));
            MaintenanceState state = await h.World.MaintenanceAsync();
            Check.True(state.Enabled && state.AllowedLogins.Single() == "tester1");
            h.Clock.Now = h.Clock.Now.AddHours(1);
            Check.True(!(await h.World.MaintenanceAsync()).Enabled, "elapsed window disables itself");
            await h.World.AnnounceAsync(AnnouncementChannel.Launcher, "Welcome", null, h.Clock.Now.AddHours(1), null, Harness.Op());
            await h.World.AnnounceAsync(AnnouncementChannel.Launcher, "Old", null, h.Clock.Now.AddMinutes(-1), null, Harness.Op());
            Check.Equal("Welcome", (await h.World.ActiveAnnouncementsAsync(AnnouncementChannel.Launcher)).Single().Message);
            Check.Equal(0, (await h.World.ActiveAnnouncementsAsync(AnnouncementChannel.InGame)).Count);
        });

        yield return ("export and import a character between accounts", async () =>
        {
            Account source = await h.Accounts.CreateAsync("export1", "password-8", Harness.Op());
            Account target = await h.Accounts.CreateAsync("import1", "password-8", Harness.Op());
            Character c = await h.Characters.CreateAsync(source.Id, 0, 25, "Traveller", Appearance);
            CharacterExport data = await h.Characters.ExportAsync(c.Id);
            await h.Characters.SoftDeleteAsync(c.Id, c.Version, Harness.Op("transfer out"));
            OperatorContext op = Harness.Op("transfer in");
            Character copy = await h.Characters.ImportAsync(target.Id, 2, data, op);
            Character again = await h.Characters.ImportAsync(target.Id, 2, data, op);
            Check.Equal(copy.Id, again.Id);
            Check.Equal((short)25, copy.Job);
            Check.True(copy.Appearance.AsSpan().SequenceEqual(Appearance));
            await Check.Fails(GameDataError.Invalid, () => h.Characters.ImportAsync(target.Id, 3, data with { SchemaVersion = 2 }, Harness.Op()));
        });

        yield return ("stat defaults, base per class and per-level points", async () =>
        {
            StatConfiguration config = await h.Stats.ConfigurationAsync();
            Check.Equal(StatKeys.All.Count, config.Stats.Count);
            Check.Equal(95, config.Levels.Count);
            Check.True(config.Levels.All(l => l.Points == 5));
            Check.Equal(StatService.Classes.Count * StatKeys.All.Count, config.Bases.Count);
            // Client class table values replaced the placeholder 10 (migration StatClientClassBase).
            Check.Equal(9, config.Bases.Single(b => b.ClassCode == 1 && b.StatKey == "wisdom").Value);
            Check.Equal(1, config.Bases.Single(b => b.ClassCode == 0 && b.StatKey == "wisdom").Value);
            Check.Equal(5, config.Bases.Single(b => b.ClassCode == 2 && b.StatKey == "strength").Value);
            Account account = await h.Accounts.CreateAsync("stats1", "password-9", Harness.Op());
            Character fighter = await h.Characters.CreateAsync(account.Id, 0, 1, "Statter", Appearance);
            await h.Stats.SetClassBaseAsync(0, new Dictionary<string, int> { ["strength"] = 25, ["max_hp"] = 150 }, Harness.Op("tune fighter"));
            await Check.Fails(GameDataError.NotFound, () => h.Stats.SetClassBaseAsync(0, new Dictionary<string, int> { ["luck"] = 1 }, Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SetClassBaseAsync(0, new Dictionary<string, int> { ["strength"] = -1 }, Harness.Op()));
            CharacterStatSheet sheet = (await h.Stats.ForCharacterAsync(fighter.Id))!;
            Check.Equal("Fighter", sheet.Class.Name);
            Check.Equal(25, sheet.Total("strength"));
            Check.Equal(150, sheet.Total("max_hp"));
            Check.Equal(5, sheet.EarnedPoints);
            Check.Equal(5, sheet.AvailablePoints);
            await h.Characters.UpdateAsync(fighter.Id, fighter.Version, new CharacterChanges(Level: 10), Harness.Op("level up"));
            CharacterStatSheet level10 = (await h.Stats.ForCharacterAsync(fighter.Id))!;
            Check.Equal(50, level10.EarnedPoints);
            // Client growth: Fighter Strength +4 per level above 1 (base 25 set above), Wisdom 1 + 2 × 9.
            Check.Equal(25 + 4 * 9, level10.Total("strength"));
            Check.Equal(36, level10.Stats.Single(s => s.Key == "strength").LevelBonus);
            Check.Equal(1 + 2 * 9, level10.Total("wisdom"));
            Check.Equal(150, level10.Total("max_hp")); // no growth configured for derived stats
            await h.Stats.SetClassGrowthAsync(0, new Dictionary<string, int> { ["max_hp"] = 20, ["strength"] = 0 }, Harness.Op("hp growth"));
            CharacterStatSheet regrown = (await h.Stats.ForCharacterAsync(fighter.Id))!;
            Check.Equal(150 + 20 * 9, regrown.Total("max_hp"));
            Check.Equal(25, regrown.Total("strength"));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SetClassGrowthAsync(0, new Dictionary<string, int> { ["strength"] = -1 }, Harness.Op()));
            await h.Stats.SetClassGrowthAsync(0, new Dictionary<string, int> { ["max_hp"] = 0, ["strength"] = 4 }, Harness.Op("restore"));
            await h.Stats.SetLevelPointsAsync([0, .. Enumerable.Repeat(3, 98)], Harness.Op("new table"));
            Check.Equal(27, (await h.Stats.ForCharacterAsync(fighter.Id))!.EarnedPoints);
            Check.Equal(99, (await h.Stats.ConfigurationAsync()).Levels.Count);
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SetLevelPointsAsync([], Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SetLevelPointsAsync([1, -2], Harness.Op()));
            await h.Stats.UpdateDefinitionsAsync([new("max_hp", "Maximum HP", true, 10)], Harness.Op("hp per point"));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.UpdateDefinitionsAsync([new("max_hp", "Maximum HP", true, 0)], Harness.Op()));
            await using GameDbContext db = h.Db();
            Check.True(await db.AuditLog.AnyAsync(a => a.Action == "stats.class_base" && a.TargetId == "0"));
            Check.True(await db.AuditLog.AnyAsync(a => a.Action == "stats.class_growth" && a.TargetId == "0"));
            Check.Equal(5, (await db.ClassStatBases.SingleAsync(b => b.ClassCode == 4 && b.StatKey == "strength")).PerLevel);
        });

        yield return ("spending stat points is bounded, owner-only and idempotent", async () =>
        {
            Account owner = await h.Accounts.CreateAsync("stats2", "password-9", Harness.Op());
            Account other = await h.Accounts.CreateAsync("stats3", "password-9", Harness.Op());
            Character c = await h.Characters.CreateAsync(owner.Id, 0, 25, "Spender", Appearance);
            await h.Characters.UpdateAsync(c.Id, c.Version, new CharacterChanges(Level: 4), Harness.Op("level up"));
            CharacterStatSheet start = (await h.Stats.ForCharacterAsync(c.Id))!;
            Check.Equal("Valcan", start.Class.Name);
            int available = start.AvailablePoints;
            Check.True(available >= 4, "level 4 earns points");
            OperatorContext op = Harness.Op("launcher spend");
            CharacterStatSheet spent = await h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["max_hp"] = 2, ["wisdom"] = 1 }, op);
            CharacterStatSheet retried = await h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["max_hp"] = 2, ["wisdom"] = 1 }, op);
            Check.Equal(available - 3, spent.AvailablePoints);
            Check.Equal(spent.AvailablePoints, retried.AvailablePoints);
            Check.Equal(start.Total("max_hp") + 2 * 10, spent.Total("max_hp"));
            Check.Equal(start.Total("wisdom") + 1, spent.Total("wisdom"));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["strength"] = spent.AvailablePoints + 1 }, Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["strength"] = -1 }, Harness.Op()));
            await Check.Fails(GameDataError.NotFound, () => h.Stats.SpendAsync(other.Id, c.Id, new Dictionary<string, int> { ["strength"] = 1 }, Harness.Op()));
            await Check.Fails(GameDataError.NotFound, () => h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["luck"] = 1 }, Harness.Op()));
            await h.Stats.UpdateDefinitionsAsync([new("evasion", "Evasion", false, 1)], Harness.Op("lock evasion"));
            await Check.Fails(GameDataError.Forbidden, () => h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["evasion"] = 1 }, Harness.Op()));
            // Two launchers spending everything at once: the advisory lock lets exactly one of them win.
            int rest = spent.AvailablePoints;
            Task<CharacterStatSheet>[] racing = Enumerable.Range(0, 2).Select(_ =>
                h.Stats.SpendAsync(owner.Id, c.Id, new Dictionary<string, int> { ["strength"] = rest }, Harness.Op("race"))).ToArray();
            int won = 0;
            foreach (Task<CharacterStatSheet> attempt in racing)
            {
                try { await attempt; won++; }
                catch (GameDataException error) when (error.Code == GameDataError.Invalid) { }
            }
            Check.Equal(1, won);
            Check.Equal(0, (await h.Stats.ForCharacterAsync(c.Id))!.AvailablePoints);
            Check.Equal(1, (await h.Stats.ForAccountAsync(owner.Id)).Count);
        });

        yield return ("item catalog import, search and bundles", async () =>
        {
            CatalogImportRow[] rows =
            [
                new(20332, "Skill Recycler", "스킬 리사이클러", "A token.", "icon_gp_skillrecycle", ["material", "skill", "event"]),
                new(9603, "Aura of Strength: 1 min", null, null, "icon_ip_02_001", ["buff"], Grade: 4, UseLevel: 30),
                new(7380, null, "스카라브의 뿔", null, "../../etc/passwd", ["quest", "Bad Tag!"], Grade: 9),
                new(11610, "Test Ring", null, null, null, ["equipment", "accessory", "ring", "admin"], AdminOnly: true),
            ];
            OperatorContext op = Harness.Op("import catalog");
            CatalogImportResult first = await h.Items.ImportCatalogAsync(rows, op);
            Check.Equal(new CatalogImportResult(4, 0, 0), first);
            Check.Equal(new CatalogImportResult(0, 0, 4), await h.Items.ImportCatalogAsync(rows, op)); // retry of the same request
            CatalogImportResult again = await h.Items.ImportCatalogAsync([.. rows[..2], rows[2] with { NameEn = "Scarab Horn" }, rows[3]], Harness.Op("re-export"));
            Check.Equal(new CatalogImportResult(0, 1, 3), again);
            CatalogItem scarab = (await h.Items.GetCatalogAsync(7380))!;
            Check.True(scarab.Tags.SequenceEqual(["quest"]) && scarab.Category == "quest" && scarab.Rarity is null, "bad tag and grade dropped");
            Check.Equal(3, (await h.Items.SearchCatalogAsync(new CatalogFilter(AdminOnly: false))).Total);
            Check.Equal(11610, (await h.Items.SearchCatalogAsync(new CatalogFilter(AdminOnly: true))).Items.Single().ItemId);
            Check.Equal(9603, (await h.Items.SearchCatalogAsync(new CatalogFilter(Tag: "buff", Grade: 4, MinLevel: 20, MaxLevel: 40))).Items.Single().ItemId);
            Check.Equal(0, (await h.Items.SearchCatalogAsync(new CatalogFilter(Tag: "buff", MinLevel: 31))).Total);
            Check.True((await h.Items.CatalogTagsAsync()).Any(t => t.Tag == "admin" && t.Count == 1), "tag counts");
            Check.Equal<string?>(null, (await h.Items.GetCatalogAsync(7380))!.Icon); // unsafe icon key dropped
            Check.Equal(1, (await h.Items.SearchCatalogAsync("recycl")).Total);
            Check.Equal(1, (await h.Items.SearchCatalogAsync("스카라브")).Total);
            Check.Equal(9603, (await h.Items.SearchCatalogAsync("9603")).Items.Single().ItemId);
            Check.Equal(0, (await h.Items.SearchCatalogAsync("100%_")).Total);
            await Check.Fails(GameDataError.Invalid, () => h.Items.ImportCatalogAsync([rows[0], rows[0]], Harness.Op()));

            ItemBundle bundle = await h.Items.CreateBundleAsync("Starter", "first login", [new(20332, 1), new(9603, 5)], Harness.Op("bundle"));
            await Check.Fails(GameDataError.NotFound, () => h.Items.CreateBundleAsync("Bad", null, [new(1, 1)], Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Items.CreateBundleAsync("Bad", null, [new(20332, 0)], Harness.Op()));
            await Check.Fails(GameDataError.Duplicate, () => h.Items.CreateBundleAsync("Starter", null, [new(20332, 1)], Harness.Op()));
            ItemBundle edited = await h.Items.UpdateBundleAsync(bundle.Id, bundle.Version, "Starter", null, true, [new(9603, 2)], Harness.Op("edit"));
            Check.Equal(1, edited.Entries.Count);
            await Check.Fails(GameDataError.Conflict, () => h.Items.UpdateBundleAsync(bundle.Id, bundle.Version, "Starter", null, false, [new(9603, 2)], Harness.Op()));
            Check.Equal(9603, (await h.Items.BundlesAsync()).Single().Entries.Single().ItemId);
        });

        yield return ("sending items to characters and accounts (single, batch, all) and cancelling", async () =>
        {
            Account a1 = await h.Accounts.CreateAsync("gift1", "password-9", Harness.Op());
            Account a2 = await h.Accounts.CreateAsync("gift2", "password-9", Harness.Op());
            Character c1 = await h.Characters.CreateAsync(a1.Id, 0, 1, "Giftee", Appearance);
            Character c2 = await h.Characters.CreateAsync(a2.Id, 0, 9, "Giftee2", Appearance);
            long bundleId = (await h.Items.BundlesAsync()).Single(b => b.Name == "Starter").Id;

            OperatorContext single = Harness.Op("event reward");
            DeliveryResult one = await h.Items.SendAsync(new(DeliveryTarget.Character, ["giftee"], false, bundleId, [new(20332, 1)], "Welcome"), single);
            Check.Equal(new DeliveryResult(single.RequestId, 1, 2), one);
            Check.Equal(one, await h.Items.SendAsync(new(DeliveryTarget.Character, ["giftee"], false, bundleId, [new(20332, 1)], "Welcome"), single));
            DeliveryResult batch = await h.Items.SendAsync(new(DeliveryTarget.Character, ["Giftee", "Giftee2", " giftee "], false, null, [new(9603, 3)], null), Harness.Op("batch"));
            Check.Equal(2, batch.Recipients);
            await Check.Fails(GameDataError.NotFound, () => h.Items.SendAsync(new(DeliveryTarget.Character, ["Giftee", "Nobody"], false, null, [new(9603, 1)], null), Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Items.SendAsync(new(DeliveryTarget.Character, ["Giftee"], false, null, [], null), Harness.Op()));
            DeliveryResult account = await h.Items.SendAsync(new(DeliveryTarget.Account, ["gift2"], false, null, [new(20332, 2)], null), Harness.Op("account"));
            Check.Equal(1, account.Rows);
            DeliveryResult everyone = await h.Items.SendAsync(new(DeliveryTarget.Account, [], true, null, [new(9603, 1)], "Maintenance gift"), Harness.Op("all"));
            await Check.Fails(GameDataError.Forbidden, () => h.Items.SendAsync(new(DeliveryTarget.Character, ["Giftee"], false, null, [new(11610, 1)], null), Harness.Op("god ring")));
            Check.Equal(1, (await h.Items.SendAsync(new(DeliveryTarget.Character, ["Giftee"], false, null, [new(11610, 1)], null, AllowAdminItems: true), Harness.Op("owner"))).Rows);
            IReadOnlyList<RecipientView> found = await h.Items.FindRecipientsAsync("gift");
            Check.True(found.Any(r => r.Kind == DeliveryTarget.Character && r.Name == "Giftee" && r.AccountLogin == "gift1"), "character hit shows its account");
            Check.True(found.Any(r => r.Kind == DeliveryTarget.Account && r.AccountLogin == "gift2"), "account hit");
            Check.Equal(0, (await h.Items.FindRecipientsAsync("%")).Count);
            Check.True(everyone.Recipients >= 2, "every active account");

            Page<DeliveryView> pending = await h.Items.DeliveriesAsync(DeliveryStatus.Pending, recipient: "Giftee");
            Check.Equal(4, pending.Total); // two from the bundle send, one from the batch, one GM ring sent by an Owner
            Check.True(pending.Items.All(d => d.CharacterName == "Giftee" && d.AccountLogin == "gift1" && d.ItemName is not null));
            Check.Equal(2, await h.Items.CancelBatchAsync(single.RequestId, Harness.Op("wrong recipient")));
            await Check.Fails(GameDataError.NotFound, () => h.Items.CancelBatchAsync(single.RequestId, Harness.Op()));
            Check.Equal(2, (await h.Items.DeliveriesAsync(DeliveryStatus.Cancelled)).Total);
            Check.Equal(1, (await h.Items.DeliveriesAsync(recipient: "gift2", status: DeliveryStatus.Pending)).Items.Count(d => d.Target == DeliveryTarget.Account && d.ItemId == 20332));
            Check.True(c2.Id > c1.Id);
        });

        yield return ("Gift Box in game: list, read, take once into the right bag, account gift once, delete", async () =>
        {
            await h.Items.ImportCatalogAsync([new(30001, "Cash Wings", null, null, null, ["costume", "cash"])], Harness.Op("cash item"));
            Account account = await h.Accounts.CreateAsync("boxer", "password-9", Harness.Op());
            Character one = await h.Characters.CreateAsync(account.Id, 0, 1, "BoxOne", Appearance);
            Character two = await h.Characters.CreateAsync(account.Id, 1, 9, "BoxTwo", Appearance);
            await h.Items.SendAsync(new(DeliveryTarget.Character, ["BoxOne"], false, null, [new(9603, 3), new(7380, 1)], "Hello"), Harness.Op("gift"));
            await h.Items.SendAsync(new(DeliveryTarget.Account, ["boxer"], false, null, [new(30001, 1)], null), Harness.Op("account gift"));
            GiftBoxService box = h.Services.GetRequiredService<GiftBoxService>();

            IReadOnlyList<GiftView> mine = await box.ListAsync(one.Id);
            Check.True(mine.Select(g => g.ItemId).SequenceEqual([9603, 7380, 30001]), "own gifts and the account gift, oldest first");
            Check.Equal(1, (await box.ListAsync(two.Id)).Count);
            Check.Equal(0, (await box.ListAsync(-5)).Count);
            long buff = mine[0].Id, quest = mine[1].Id, wings = mine[2].Id;

            Check.Equal<GiftView?>(null, await box.ReadAsync(two.Id, buff)); // another character's gift
            GiftView read = (await box.ReadAsync(one.Id, buff))!;
            Check.True(read.OpenedAt == h.Clock.Now && read.Message == "Hello", "first read records the time");
            h.Clock.Now = h.Clock.Now.AddMinutes(1);
            Check.True((await box.ReadAsync(one.Id, buff))!.OpenedAt < h.Clock.Now, "later reads keep the first time");

            GiftClaim taken = await box.ClaimAsync(one.Id, buff);
            Check.Equal(GiftClaimOutcome.Claimed, taken.Outcome);
            Check.True(taken.Item is { Bag: 0, Slot: 0, ItemId: 9603, Quantity: 3, Location: ItemLocation.Inventory } && taken.Item.GameSerial > 0, "main bag, first slot");
            Check.Equal(GiftClaimOutcome.NotFound, (await box.ClaimAsync(one.Id, buff)).Outcome);
            Check.Equal(GiftClaimOutcome.NotFound, (await box.ClaimAsync(two.Id, quest)).Outcome);
            Check.True((await box.ClaimAsync(one.Id, quest)).Item is { Bag: 2, Slot: 0 }, "quest bag");
            GiftClaim wingsTaken = await box.ClaimAsync(two.Id, wings);
            Check.True(wingsTaken.Item is { Bag: 1, Slot: 0 } && wingsTaken.Item.CharacterId == two.Id, "account gift: cash bag of the character that took it");
            Check.Equal(GiftClaimOutcome.NotFound, (await box.ClaimAsync(one.Id, wings)).Outcome);
            Check.Equal(0, (await box.ListAsync(one.Id)).Count);

            IReadOnlyList<CharacterItem> bag = await box.InventoryAsync(one.Id);
            Check.True(bag.Select(i => (i.Bag, i.ItemId)).SequenceEqual([((short)0, 9603), ((short)2, 7380)]), "inventory by bag");
            Check.True(bag.Select(i => i.GameSerial).Distinct().Count() == 2 && bag[0].GameSerial != wingsTaken.Item!.GameSerial, "unique game serials");
            DeliveryView claimed = (await h.Items.DeliveriesAsync(DeliveryStatus.Delivered, recipient: "boxer")).Items.Single(d => d.ItemId == 30001);
            Check.True(claimed.ClaimedBy == "BoxTwo" && claimed.DeliveredAt is not null, "Portal shows who took the account gift");

            await h.Items.SendAsync(new(DeliveryTarget.Character, ["BoxOne"], false, null, [new(9603, 1)], null), Harness.Op("again"));
            long unwanted = (await box.ListAsync(one.Id)).Single().Id;
            Check.True(!await box.DismissAsync(two.Id, unwanted), "cannot delete another character's gift");
            Check.True(await box.DismissAsync(one.Id, unwanted));
            Check.True(!await box.DismissAsync(one.Id, unwanted));
            DeliveryView dismissed = (await h.Items.DeliveriesAsync(DeliveryStatus.Cancelled, recipient: "BoxOne")).Items.Single();
            Check.Equal($"game:{one.Id}", dismissed.CancelledBy);
            Check.True((await box.ClaimAsync(one.Id, unwanted)).Outcome == GiftClaimOutcome.NotFound);

            await using GameDbContext db = h.Db();
            Check.Equal(3, await db.AuditLog.CountAsync(a => a.Action == "gift.claim"));
            Check.True(await db.AuditLog.AnyAsync(a => a.Action == "gift.dismiss" && a.Operator == $"game:{one.Id}"), "player actions are audited");
            Check.Equal<short?>(2, GiftBoxService.FirstFree([0, 1, 3]));
            Check.Equal<short?>(null, GiftBoxService.FirstFree(Enumerable.Range(0, GiftBoxService.BagSlots).Select(i => (short)i).ToArray()));
            Check.True(GiftBoxService.BagOf(["equipment"]) == 0 && GiftBoxService.BagOf(["quest", "cash"]) == 2, "bag rule");
        });

        yield return ("starter bundle waits in the Gift Box of characters created in game", async () =>
        {
            ItemBundle starter = (await h.Items.BundlesAsync()).Single(b => b.Name == "Starter");
            ItemBundle gm = await h.Items.CreateBundleAsync("GM kit", null, [new(11610, 1), new(20332, 1)], Harness.Op("gm kit"));
            await Check.Fails(GameDataError.Forbidden, () => h.Items.SetStarterBundleAsync(gm.Id, false, Harness.Op("gm starter")));
            await Check.Fails(GameDataError.NotFound, () => h.Items.SetStarterBundleAsync(999_999, true, Harness.Op()));
            Check.Equal(starter.Id, (await h.Items.SetStarterBundleAsync(starter.Id, false, Harness.Op("starter")))!.Id);
            Account account = await h.Accounts.CreateAsync("newbie", "password-9", Harness.Op());
            Character fresh = await h.Characters.CreateAsync(account.Id, 0, 1, "Newbie", Appearance);
            await h.Characters.CreateAsync(account.Id, 0, 1, "Newbie", Appearance); // retried create queues nothing twice
            GiftBoxService box = h.Services.GetRequiredService<GiftBoxService>();
            IReadOnlyList<GiftView> gifts = await box.ListAsync(fresh.Id);
            Check.True(gifts.Count == 1 && gifts[0] is { ItemId: 9603, Quantity: 2, Message: "Starter items", Target: DeliveryTarget.Character },
                "starter bundle lines in the new character's Gift Box");

            Check.Equal(gm.Id, (await h.Items.SetStarterBundleAsync(gm.Id, true, Harness.Op("owner starter")))!.Id);
            Check.True((await h.Items.BundlesAsync()).Count(b => b.Starter) == 1, "one starter bundle");
            Character second = await h.Characters.CreateAsync(account.Id, 1, 9, "Newbie2", Appearance);
            Check.True((await box.ListAsync(second.Id)).Select(g => g.ItemId).SequenceEqual([20332]), "GM items are never queued automatically");

            Check.Equal<ItemBundle?>(null, await h.Items.SetStarterBundleAsync(null, false, Harness.Op("no starter")));
            Character third = await h.Characters.CreateAsync(account.Id, 2, 17, "Newbie3", Appearance);
            Check.Equal(0, (await box.ListAsync(third.Id)).Count);
            uint version = (await h.Items.BundlesAsync()).Single(b => b.Id == gm.Id).Version; // the starter flag changed the row
            await h.Items.UpdateBundleAsync(gm.Id, version, "GM kit", null, false, [new(20332, 1)], Harness.Op("disable"));
            await Check.Fails(GameDataError.Invalid, () => h.Items.SetStarterBundleAsync(gm.Id, true, Harness.Op()));
            await using GameDbContext db = h.Db();
            Check.True(await db.AuditLog.AnyAsync(a => a.Action == "items.starter_send" && a.TargetId == fresh.Id.ToString()), "starter send audited");
        });

        yield return ("migrations apply on filled tables of the previous release (catalog, items)", async () =>
        {
            // The live catalog had 45,996 rows when ItemCatalogProperties first failed (23502): start from data, not empty tables.
            string admin = h.AdminConnection ?? throw new InvalidOperationException("admin connection");
            string database = "c9_validation_" + Guid.NewGuid().ToString("N")[..12];
            await using (NpgsqlConnection connection = new(admin))
            {
                await connection.OpenAsync();
                await using NpgsqlCommand create = new($"CREATE DATABASE {database}", connection);
                await create.ExecuteNonQueryAsync();
            }
            try
            {
                await using (ServiceProvider services = new ServiceCollection()
                    .AddC9GameData(new NpgsqlConnectionStringBuilder(admin) { Database = database }.ConnectionString)
                    .BuildServiceProvider())
                {
                    IDbContextFactory<GameDbContext> factory = services.GetRequiredService<IDbContextFactory<GameDbContext>>();
                    await using (GameDbContext db = await factory.CreateDbContextAsync())
                        await db.GetService<IMigrator>().MigrateAsync("20261120131238_StatClassGrowth");
                    Account account = await services.GetRequiredService<AccountService>().CreateAsync("olddata", "password-9", Harness.Op());
                    Character owner;
                    await using (GameDbContext db = await factory.CreateDbContextAsync())
                    {
                        // Raw SQL: today's CharacterService also reads tables that this old schema does not have yet.
                        await db.Database.ExecuteSqlAsync($"""
                            INSERT INTO game.characters (account_id, slot, name, job, appearance, exp, gold, play_seconds)
                            VALUES ({account.Id}, 0, 'OldOwner', 1, {Appearance}, 0, 0, 0)
                            """);
                        owner = await db.Characters.AsNoTracking().SingleAsync(c => c.Name == "OldOwner");
                        await db.Database.ExecuteSqlAsync($$"""
                            INSERT INTO game.item_catalog (item_id, name_en, category, raw)
                            SELECT g, 'Item ' || g, 'material', '{}'::jsonb FROM generate_series(1, 500) g
                            """);
                        await db.Database.ExecuteSqlAsync($$"""
                            INSERT INTO game.character_items (serial, character_id, location, slot, item_id, quantity, enchant, options, bound)
                            SELECT gen_random_uuid(), {{owner.Id}}, 'Inventory', g, g, 1, 0, '{}'::jsonb, false FROM generate_series(0, 2) g
                            """);
                        await db.Database.MigrateAsync();
                        Check.Equal(0, (await db.Database.GetPendingMigrationsAsync()).Count());
                        Check.Equal(500, await db.ItemCatalog.CountAsync(i => i.Tags.Count == 0 && !i.AdminOnly && i.UseLevel == null));
                        List<CharacterItem> items = await db.CharacterItems.OrderBy(i => i.Slot).ToListAsync();
                        Check.True(items.Count == 3 && items.All(i => i.Bag == 0 && i.GameSerial > 0) && items.Select(i => i.GameSerial).Distinct().Count() == 3,
                            "existing stacks get distinct game serials and bag 0");
                    }
                    GiftClaim claim;
                    await services.GetRequiredService<ItemService>().SendAsync(
                        new(DeliveryTarget.Character, ["OldOwner"], false, null, [new(42, 2)], null), Harness.Op("after upgrade"));
                    GiftBoxService box = services.GetRequiredService<GiftBoxService>();
                    claim = await box.ClaimAsync(owner.Id, (await box.ListAsync(owner.Id)).Single().Id);
                    Check.True(claim.Item is { Slot: 3, Bag: 0 } && claim.Item.GameSerial > 3, "new stack after the old ones");
                }
            }
            finally
            {
                NpgsqlConnection.ClearAllPools();
                await using NpgsqlConnection connection = new(admin);
                await connection.OpenAsync();
                await using NpgsqlCommand drop = new($"DROP DATABASE IF EXISTS {database} WITH (FORCE)", connection);
                await drop.ExecuteNonQueryAsync();
            }
        });
    }
}
