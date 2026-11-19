using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;

internal static partial class GameDataValidation
{
    private static IEnumerable<(string, Func<Task>)> OfflineCases()
    {
        yield return ("password hash verifies and rejects", () =>
        {
            string hash = PasswordHasher.Hash("correct horse");
            Check.True(hash.StartsWith("pbkdf2-sha256$210000$"));
            Check.True(PasswordHasher.Verify("correct horse", hash));
            Check.True(!PasswordHasher.Verify("correct horsE", hash));
            Check.True(!PasswordHasher.Verify("anything", PasswordHasher.Disabled));
            Check.True(!PasswordHasher.Verify("anything", "pbkdf2-sha256$1$AA==$AA=="), "iteration floor");
            Check.True(PasswordHasher.Hash("correct horse") != hash, "salted");
            Check.Throws<GameDataException>(() => PasswordHasher.Hash("short"));
            return Task.CompletedTask;
        });

        yield return ("snake_case naming", () =>
        {
            Check.Equal("rare_drop", GameDbContext.Snake("RareDrop"));
            Check.Equal("pk_accounts", GameDbContext.Snake("PK_accounts"));
            Check.Equal("ix_characters_account_id_slot", GameDbContext.Snake("IX_characters_AccountId_Slot"));
            Check.Equal("pos_x", GameDbContext.Snake("PosX"));
            return Task.CompletedTask;
        });

        yield return ("rate cap is applied once at the end", () =>
        {
            // CURRENT_STATE_20261114 §3: server=100, event=2, account=0.1 must give min(20, 100) = 20.
            RateSet result = WorldService.Combine(new(100, 100, 100, 100), [new(2, 2, 2, 2)], new(0.1m, 0.1m, 0.1m, 0.1m), 100);
            Check.Equal(20m, result.Exp);
            RateSet stacked = WorldService.Combine(new(5, 5, 2, 2), [new(2, 1, 1, 1), new(3, 1, 1, 1)], RateSet.Neutral, 100);
            Check.Equal(30m, stacked.Exp);
            Check.Equal(5m, stacked.Drop);
            Check.Equal(100m, WorldService.Combine(new(50, 1, 1, 1), [new(4, 1, 1, 1)], RateSet.Neutral, 100).Exp);
            Check.True(!new RateSet(-1).IsValid(100) && !new RateSet(101).IsValid(100) && RateSet.Neutral.IsValid(100));
            return Task.CompletedTask;
        });

        yield return ("event recurrence windows", () =>
        {
            DateTimeOffset start = new(2026, 11, 1, 18, 0, 0, TimeSpan.Zero);
            WorldEvent weekly = new()
            {
                Name = "Weekend exp", CreatedBy = "t", StartsAt = start, EndsAt = start.AddHours(4), RepeatEveryDays = 7,
                RecurUntil = start.AddDays(28)
            };
            Check.True(weekly.IsActiveAt(start.AddHours(1)));
            Check.True(!weekly.IsActiveAt(start.AddHours(5)));
            Check.True(weekly.IsActiveAt(start.AddDays(14).AddHours(3)));
            Check.True(!weekly.IsActiveAt(start.AddDays(14).AddHours(4)));
            Check.True(!weekly.IsActiveAt(start.AddDays(28).AddHours(1)), "ends at RecurUntil");
            Check.True(!weekly.IsActiveAt(start.AddMinutes(-1)));
            weekly.Enabled = false;
            Check.True(!weekly.IsActiveAt(start.AddHours(1)));
            return Task.CompletedTask;
        });

        yield return ("sanction status precedence", () =>
        {
            DateTimeOffset now = new(2026, 11, 15, 0, 0, 0, TimeSpan.Zero);
            AccountSanction suspend = new() { Kind = SanctionKind.Suspend, Reason = "r", IssuedBy = "t", StartsAt = now.AddDays(-1), EndsAt = now.AddDays(1) };
            AccountSanction ban = new() { Kind = SanctionKind.Ban, Reason = "r", IssuedBy = "t", StartsAt = now.AddDays(-1) };
            AccountSanction expired = new() { Kind = SanctionKind.Ban, Reason = "r", IssuedBy = "t", StartsAt = now.AddDays(-3), EndsAt = now.AddDays(-2) };
            Check.Equal(AccountStatus.Active, SanctionService.StatusAt([expired], now));
            Check.Equal(AccountStatus.Suspended, SanctionService.StatusAt([suspend, expired], now));
            Check.Equal(AccountStatus.Banned, SanctionService.StatusAt([suspend, ban], now));
            ban.RevokedAt = now;
            Check.Equal(AccountStatus.Suspended, SanctionService.StatusAt([suspend, ban], now));
            return Task.CompletedTask;
        });

        yield return ("character name rules match CharacterLobby", () =>
        {
            Check.True(CharacterService.ValidName("Lab") && CharacterService.ValidName("ทดสอบ_1"));
            Check.True(!CharacterService.ValidName("") && !CharacterService.ValidName("a b") &&
                !CharacterService.ValidName("x'; DROP") && !CharacterService.ValidName(new string('a', 21)));
            return Task.CompletedTask;
        });

        yield return ("stat keys are unique and every job has a base class", () =>
        {
            Check.Equal(StatKeys.All.Count, StatKeys.All.Select(s => s.Key).Distinct().Count());
            Check.True(StatKeys.All.All(s => s.Key.Length <= StatKeys.MaxKeyLength && s.DefaultBase >= 0));
            Check.Equal((short)0, StatService.ClassOf(1));
            Check.Equal((short)0, StatService.ClassOf(8));
            Check.Equal((short)1, StatService.ClassOf(9));
            Check.Equal((short)3, StatService.ClassOf(17));
            Check.Equal((short)2, StatService.ClassOf(25));
            Check.Equal((short)4, StatService.ClassOf(40));
            Check.Throws<GameDataException>(() => StatService.ClassOf(0));
            Check.Throws<GameDataException>(() => StatService.ClassOf(41));
            return Task.CompletedTask;
        });
    }
}
