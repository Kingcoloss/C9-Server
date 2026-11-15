using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;

internal static partial class GameDataValidation
{
    private const int DatabaseCaseCount = 11;

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
    }
}
