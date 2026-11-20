using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;

internal static partial class GameDataValidation
{
    private const int DatabaseCaseCount = 15;

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
            Check.Equal(50, (await h.Stats.ForCharacterAsync(fighter.Id))!.EarnedPoints);
            await h.Stats.SetLevelPointsAsync([0, .. Enumerable.Repeat(3, 98)], Harness.Op("new table"));
            Check.Equal(27, (await h.Stats.ForCharacterAsync(fighter.Id))!.EarnedPoints);
            Check.Equal(99, (await h.Stats.ConfigurationAsync()).Levels.Count);
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SetLevelPointsAsync([], Harness.Op()));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.SetLevelPointsAsync([1, -2], Harness.Op()));
            await h.Stats.UpdateDefinitionsAsync([new("max_hp", "Maximum HP", true, 10)], Harness.Op("hp per point"));
            await Check.Fails(GameDataError.Invalid, () => h.Stats.UpdateDefinitionsAsync([new("max_hp", "Maximum HP", true, 0)], Harness.Op()));
            await using GameDbContext db = h.Db();
            Check.True(await db.AuditLog.AnyAsync(a => a.Action == "stats.class_base" && a.TargetId == "0"));
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
                new(20332, "Skill Recycler", "스킬 리사이클러", "A token.", "icon_gp_skillrecycle"),
                new(9603, "Aura of Strength: 1 min", null, null, "icon_ip_02_001"),
                new(7380, null, "스카라브의 뿔", null, "../../etc/passwd"),
            ];
            OperatorContext op = Harness.Op("import catalog");
            CatalogImportResult first = await h.Items.ImportCatalogAsync(rows, op);
            Check.Equal(new CatalogImportResult(3, 0, 0), first);
            Check.Equal(new CatalogImportResult(0, 0, 3), await h.Items.ImportCatalogAsync(rows, op)); // retry of the same request
            CatalogImportResult again = await h.Items.ImportCatalogAsync([.. rows[..2], rows[2] with { NameEn = "Scarab Horn" }], Harness.Op("re-export"));
            Check.Equal(new CatalogImportResult(0, 1, 2), again);
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
            Check.True(everyone.Recipients >= 2, "every active account");

            Page<DeliveryView> pending = await h.Items.DeliveriesAsync(DeliveryStatus.Pending, recipient: "Giftee");
            Check.Equal(3, pending.Total); // two from the bundle send, one from the batch
            Check.True(pending.Items.All(d => d.CharacterName == "Giftee" && d.AccountLogin == "gift1" && d.ItemName is not null));
            Check.Equal(2, await h.Items.CancelBatchAsync(single.RequestId, Harness.Op("wrong recipient")));
            await Check.Fails(GameDataError.NotFound, () => h.Items.CancelBatchAsync(single.RequestId, Harness.Op()));
            Check.Equal(2, (await h.Items.DeliveriesAsync(DeliveryStatus.Cancelled)).Total);
            Check.Equal(1, (await h.Items.DeliveriesAsync(recipient: "gift2", status: DeliveryStatus.Pending)).Items.Count(d => d.Target == DeliveryTarget.Account && d.ItemId == 20332));
            Check.True(c2.Id > c1.Id);
        });
    }
}
