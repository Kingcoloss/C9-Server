using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using C9.Contracts;
using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Protocol;
using C9.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

/// <summary>
/// In-game secondary password (docs/EXIT_AND_2ND_PASSWORD_TH.md): 2744 set, 2746 reset, 2748 change, 274A certify,
/// replies 2745/2747/2749/274B or the 0x0001 error notice, and the state fields of the 0x0BC0 select-ack.
/// </summary>
internal static partial class Validation
{
    private const uint InvalidSecondPassword = 0xC22AA395, SecondPasswordLockedNow = 0xE907F056,
        SecondPasswordLocked = 0xE853BEC9, InvalidSecondPasswordAnswer = 0x48E08822;
    private const int SecondPasswordIsSet = 0x7234, SecondPasswordFails = 0x7235, SecondPasswordLockStart = 0x7236,
        SecondPasswordCertified = 0x723F;

    /// <summary>The client's string hash (0x00420380): standard CRC-32 over UTF-16LE.</summary>
    private static uint ClientCrc(string text)
    {
        uint crc = 0xFFFFFFFF;
        foreach (byte value in Encoding.Unicode.GetBytes(text))
        {
            crc ^= value;
            for (int bit = 0; bit < 8; bit++) crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
        }
        return ~crc;
    }

    private static void SecondPasswordRules()
    {
        Check.Equal(InvalidSecondPassword, ClientCrc("C9_ERROR_INVALID_2NDPASSWORD_1"));
        Check.Equal(SecondPasswordLockedNow, ClientCrc("C9_ERROR_INVALID_2NDPASSWORD_2"));
        Check.Equal(SecondPasswordLocked, ClientCrc("C9_ERROR_LOCKED_2NDPASSWORD"));
        Check.Equal(InvalidSecondPasswordAnswer, ClientCrc("C9_ERROR_INVALID_2NDPASSWORD_Q_AND_A"));
        Check.True(KnownPackets.IsSensitive(0x2744) && KnownPackets.IsSensitive(0x2746) && KnownPackets.IsSensitive(0x2748) &&
            KnownPackets.IsSensitive(0x274A) && !KnownPackets.IsSensitive(0x2745) && !KnownPackets.IsSensitive(0x274B),
            "requests carrying CRCs and answers are redacted in traces");

        Check.Equal("BIRTHDAY", SecondPasswordPolicy.AnswerSecret("  birthday "));
        Check.True(SecondPasswordPolicy.AnswerSecret(new string('a', 25)) is not null);
        foreach (string? bad in new[] { null, "", "   ", new string('a', 26), "a\u0001b" })
            Check.True(SecondPasswordPolicy.AnswerSecret(bad) is null, "answer rejected");
        Check.True(SecondPasswordPolicy.ValidQuestion(0) && SecondPasswordPolicy.ValidQuestion(9) && !SecondPasswordPolicy.ValidQuestion(10));

        ManualClock clock = new(new DateTimeOffset(2026, 11, 19, 12, 0, 0, TimeSpan.Zero));
        uint pin = ClientCrc("1234"), wrong = ClientCrc("9999"), next = ClientCrc("5678");
        Check.Throws<GameDataException>(() => SecondPasswordPolicy.Create(7, pin, 10, "Birthday", clock.Now));
        Check.Throws<GameDataException>(() => SecondPasswordPolicy.Create(7, pin, 3, "", clock.Now));
        AccountSecondPassword row = SecondPasswordPolicy.Create(7, pin, 3, "Birthday", clock.Now);
        Check.True(row.PasswordHash.StartsWith("pbkdf2-sha256$", StringComparison.Ordinal) &&
            row.AnswerHash.StartsWith("pbkdf2-sha256$", StringComparison.Ordinal), "PBKDF2 hashes");
        Check.True(!row.PasswordHash.Contains(pin.ToString("x8"), StringComparison.Ordinal) &&
            !row.AnswerHash.Contains("BIRTHDAY", StringComparison.OrdinalIgnoreCase), "no raw secrets stored");

        // A success clears earlier failures.
        Check.Equal(SecondPasswordOutcome.WrongPassword, SecondPasswordPolicy.Verify(row, wrong, clock.Now));
        Check.Equal((short)1, row.FailCount);
        Check.Equal(SecondPasswordOutcome.Ok, SecondPasswordPolicy.Verify(row, pin, clock.Now));
        Check.Equal((short)0, row.FailCount);

        // Five failures lock the account for 72 hours (client FailCntMax, 259,200 s).
        for (short attempt = 1; attempt < 5; attempt++)
        {
            Check.Equal(SecondPasswordOutcome.WrongPassword, SecondPasswordPolicy.Verify(row, wrong, clock.Now));
            Check.Equal(attempt, row.FailCount);
        }
        Check.Equal(SecondPasswordOutcome.LockedNow, SecondPasswordPolicy.Verify(row, wrong, clock.Now));
        DateTimeOffset lockedAt = clock.Now;
        Check.Equal<DateTimeOffset?>(lockedAt, row.LockedAt);
        Check.Equal((short)5, row.FailCount);
        clock.Now = lockedAt.AddHours(1);
        Check.Equal(SecondPasswordOutcome.Locked, SecondPasswordPolicy.Verify(row, pin, clock.Now));
        Check.Equal(SecondPasswordOutcome.Locked, SecondPasswordPolicy.CheckAnswer(row, 3, "birthday", clock.Now));
        Check.Equal(SecondPasswordOutcome.Locked, SecondPasswordPolicy.Change(row, pin, next, clock.Now));
        SecondPasswordStatus locked = SecondPasswordPolicy.StatusOf(row, clock.Now);
        Check.True(locked.IsSet && locked.IsLocked && locked.FailCount == 5, "locked status");
        clock.Now = lockedAt + SecondPasswordPolicy.LockDuration - TimeSpan.FromSeconds(1);
        Check.Equal(SecondPasswordOutcome.Locked, SecondPasswordPolicy.Verify(row, pin, clock.Now));
        clock.Now = lockedAt + SecondPasswordPolicy.LockDuration;
        SecondPasswordStatus expired = SecondPasswordPolicy.StatusOf(row, clock.Now);
        Check.True(expired.IsSet && !expired.IsLocked && expired.FailCount == 0, "lock expires after 72 h");
        Check.Equal(SecondPasswordOutcome.Ok, SecondPasswordPolicy.Verify(row, pin, clock.Now));
        Check.True(row.LockedAt is null && row.FailCount == 0, "lock cleared");

        // Security answer: question must match, case and surrounding spaces do not matter.
        Check.Equal(SecondPasswordOutcome.WrongAnswer, SecondPasswordPolicy.CheckAnswer(row, 4, "birthday", clock.Now));
        Check.Equal(SecondPasswordOutcome.WrongAnswer, SecondPasswordPolicy.CheckAnswer(row, 3, null, clock.Now));
        Check.Equal((short)2, row.FailCount);
        Check.Equal(SecondPasswordOutcome.Ok, SecondPasswordPolicy.CheckAnswer(row, 3, " BIRTHDAY ", clock.Now));
        Check.Equal((short)0, row.FailCount);

        // Change needs the current password.
        Check.Equal(SecondPasswordOutcome.WrongPassword, SecondPasswordPolicy.Change(row, wrong, next, clock.Now));
        Check.Equal(SecondPasswordOutcome.Ok, SecondPasswordPolicy.Change(row, pin, next, clock.Now));
        Check.Equal(SecondPasswordOutcome.WrongPassword, SecondPasswordPolicy.Verify(row, pin, clock.Now));
        Check.Equal(SecondPasswordOutcome.Ok, SecondPasswordPolicy.Verify(row, next, clock.Now));
        Check.Equal(SecondPasswordStatus.None, SecondPasswordPolicy.StatusOf(null, clock.Now));
    }

    private static Packet SecondPasswordSet(uint crc, byte question, string answer)
    {
        byte[] payload = new byte[63];
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), crc);
        payload[0x0C] = question;
        Encoding.Unicode.GetBytes(answer).CopyTo(payload, 0x0D);
        return new Packet(0x2744, payload, 0);
    }

    private static Packet SecondPasswordReset(byte question, string answer)
    {
        byte[] payload = new byte[59];
        payload[8] = question;
        Encoding.Unicode.GetBytes(answer).CopyTo(payload, 9);
        return new Packet(0x2746, payload, 0);
    }

    private static Packet SecondPasswordChange(uint current, uint next)
    {
        byte[] payload = new byte[16];
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), current);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(12), next);
        return new Packet(0x2748, payload, 0);
    }

    private static Packet SecondPasswordCertify(uint crc)
    {
        byte[] payload = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, crc);
        return new Packet(0x274A, payload, 1);
    }

    private static void CheckSecondPasswordAck(Packet reply, ushort type, byte flag)
    {
        Check.Equal(type, reply.Type);
        Check.Equal(flag, reply.Flag);
        Check.Equal(0, reply.Payload.Length);
    }

    /// <returns>param2 (lock start in unix seconds for the lock errors).</returns>
    private static ulong CheckSecondPasswordError(Packet reply, ushort request, uint code, uint param1)
    {
        Check.Equal((ushort)0x0001, reply.Type);
        Check.Equal((byte)1, reply.Flag);
        Check.Equal(28, reply.Payload.Length);
        ReadOnlySpan<byte> payload = reply.Payload.Span;
        Check.Equal(0ul, BinaryPrimitives.ReadUInt64LittleEndian(payload));
        Check.Equal(request, BinaryPrimitives.ReadUInt16LittleEndian(payload[8..]));
        Check.Equal(code, BinaryPrimitives.ReadUInt32LittleEndian(payload[0x0A..]));
        Check.Equal((byte)1, payload[0x0E]); // popup
        Check.Equal((byte)0, payload[0x0F]);
        Check.Equal(param1, BinaryPrimitives.ReadUInt32LittleEndian(payload[0x10..]));
        return BinaryPrimitives.ReadUInt64LittleEndian(payload[0x14..]);
    }

    private static void CheckSecondPasswordSelect(Packet selected, byte isSet, byte fails, ulong lockStart, byte certified)
    {
        Check.Equal((ushort)0x0BC0, selected.Type);
        ReadOnlySpan<byte> payload = selected.Payload.Span;
        Check.Equal(isSet, payload[SecondPasswordIsSet]);
        Check.Equal(fails, payload[SecondPasswordFails]);
        Check.Equal(lockStart, BinaryPrimitives.ReadUInt64LittleEndian(payload[SecondPasswordLockStart..]));
        Check.Equal((byte)0, payload[0x723E]);
        Check.Equal(certified, payload[SecondPasswordCertified]);
    }

    private static ServerOptions SecondPasswordLobbyOptions(string store, string? database = null)
    {
        using JsonDocument fixture = Fixture("lab-official-character-lobby.json");
        ServerOptions options = ServerOptions.FromJson(fixture.RootElement.GetRawText()) with
        {
            Port = 0, TimeoutSeconds = 10, MaxPackets = 255, CharacterStore = store, GameDatabase = database
        };
        options.Validate();
        return options;
    }

    private static async Task<byte[]> SecondPasswordCharacterAsync(LobbyPipe lobby, string name)
    {
        await lobby.SendAsync(new Packet(0x1771, new byte[49]));
        Check.Equal((ushort)0x0EFA, (await lobby.ReceiveAsync()).Type);
        byte[] create = new byte[90];
        Encoding.Unicode.GetBytes(name).CopyTo(create, 3);
        await lobby.SendAsync(new Packet(0x0BBB, create));
        Packet created = await lobby.ReceiveAsync();
        Check.Equal((ushort)0x0BBC, created.Type);
        return created.Payload[..8].ToArray();
    }

    /// <summary>The whole client flow on the lab lobby (memory store): lobby, then world, then the next login.</summary>
    private static async Task MemorySecondPassword()
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-2ndpw-{Guid.NewGuid():N}.json");
        uint pin = ClientCrc("2468"), wrong = ClientCrc("1357"), next = ClientCrc("8642");
        CollectedEvents events = new();
        SessionProcessor processor = new(SecondPasswordLobbyOptions(store), events);
        try
        {
            byte[] identity;
            ulong lockStart;
            await using (LobbyPipe lobby = new(processor))
            {
                identity = await SecondPasswordCharacterAsync(lobby, "PwLab");
                // Nothing set yet: certify and change are out of sync with the server and fail as a wrong password.
                await lobby.SendAsync(SecondPasswordCertify(pin));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, InvalidSecondPassword, 0);
                await lobby.SendAsync(SecondPasswordChange(pin, next));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2748, InvalidSecondPassword, 0);
                // Unknown question or empty answer: refused before anything is stored.
                await lobby.SendAsync(SecondPasswordSet(pin, 10, "birthday"));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2744, InvalidSecondPasswordAnswer, 0);
                await lobby.SendAsync(SecondPasswordSet(pin, 2, ""));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2744, InvalidSecondPasswordAnswer, 0);

                await lobby.SendAsync(SecondPasswordSet(pin, 2, "birthday"));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x2745, 0);
                await lobby.SendAsync(SecondPasswordSet(next, 3, "other"));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2744, InvalidSecondPassword, 0);
                await lobby.SendAsync(SecondPasswordCertify(wrong));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, InvalidSecondPassword, 1);
                await lobby.SendAsync(SecondPasswordCertify(pin));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x274B, 1);

                // The select-ack carries "set" and "certified in this session" (client state 3).
                await lobby.SendAsync(new Packet(0x0BBF, identity));
                CheckSecondPasswordSelect(await lobby.ReceiveAsync(), 1, 0, 0, 1);
                Check.Equal((ushort)0x0BC3, (await lobby.ReceiveAsync()).Type);

                // In the world (the panel is opened from the game): change, then reset with the answer.
                await lobby.SendAsync(SecondPasswordChange(wrong, next));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2748, InvalidSecondPassword, 1);
                await lobby.SendAsync(SecondPasswordChange(pin, next));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x2749, 0);
                await lobby.SendAsync(SecondPasswordReset(2, "nope"));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2746, InvalidSecondPasswordAnswer, 1);
                await lobby.SendAsync(SecondPasswordReset(1, "birthday"));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2746, InvalidSecondPasswordAnswer, 2);
                await lobby.SendAsync(SecondPasswordReset(2, " BIRTHDAY"));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x2747, 0);
                await lobby.SendAsync(SecondPasswordCertify(next));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, InvalidSecondPassword, 0);

                // Five wrong certifications lock it; the fifth reply carries the lock start for the client's timer.
                await lobby.SendAsync(SecondPasswordSet(pin, 5, "Quokka"));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x2745, 0);
                for (uint attempt = 1; attempt < 5; attempt++)
                {
                    await lobby.SendAsync(SecondPasswordCertify(wrong));
                    CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, InvalidSecondPassword, attempt);
                }
                await lobby.SendAsync(SecondPasswordCertify(wrong));
                lockStart = CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, SecondPasswordLockedNow, 5);
                Check.True(Math.Abs((long)lockStart - DateTimeOffset.UtcNow.ToUnixTimeSeconds()) < 120, "lock start is now");
                await lobby.SendAsync(SecondPasswordCertify(pin));
                Check.Equal(lockStart, CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, SecondPasswordLocked, 5));
                await lobby.SendAsync(SecondPasswordReset(5, "Quokka"));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x2746, SecondPasswordLocked, 5);
                Check.Equal("world_loading", lobby.Session.Snapshot().State);
            }

            // Next login on the same server: the password and its lock are kept, the certification is not.
            await using (LobbyPipe again = new(processor))
            {
                await again.SendAsync(new Packet(0x1771, new byte[49]));
                Check.Equal((ushort)0x0EFA, (await again.ReceiveAsync()).Type);
                await again.SendAsync(new Packet(0x0BBF, identity));
                CheckSecondPasswordSelect(await again.ReceiveAsync(), 1, 5, lockStart, 0);
            }

            ServerEvent[] logged = events.Values.Where(e => e.Event == "second_password").ToArray();
            Check.Equal(22, logged.Length);
            Check.True(logged.Any(e => e.Reason == "account 0: LockedNow, fails 5, locked"), "lock event");
            string all = JsonSerializer.Serialize(events.Values.ToArray());
            foreach (string secret in new[] { "birthday", "Quokka", pin.ToString("x8"), pin.ToString(), wrong.ToString("x8") })
                Check.True(!all.Contains(secret, StringComparison.OrdinalIgnoreCase), "no secret in the event log");
            Check.True(!events.Values.Any(e => e.Event == "session_error"), "no session error");
        }
        finally { File.Delete(store); }
    }

    /// <summary>
    /// game.account_second_passwords (migration AccountSecondPassword): service, audit, concurrent failures and the lobby
    /// on the database. Needs C9_DB_ADMIN_CONNECTION; a temporary database is created and dropped.
    /// </summary>
    private static async Task GameDataSecondPassword()
    {
        string? admin = Environment.GetEnvironmentVariable("C9_DB_ADMIN_CONNECTION");
        if (string.IsNullOrWhiteSpace(admin))
        {
            Console.Error.WriteLine("note: 2nd password database checks skipped (set C9_DB_ADMIN_CONNECTION to run them)");
            return;
        }
        string database = "c9_2ndpw_" + Guid.NewGuid().ToString("N")[..12];
        string connection = new NpgsqlConnectionStringBuilder(admin) { Database = database }.ConnectionString;
        string store = Path.Combine(Path.GetTempPath(), $"c9-2ndpw-db-{Guid.NewGuid():N}.json");
        await using (NpgsqlConnection server = new(admin))
        {
            await server.OpenAsync();
            await using NpgsqlCommand create = new($"CREATE DATABASE {database}", server);
            await create.ExecuteNonQueryAsync();
        }
        await using ServiceProvider portal = new ServiceCollection().AddLogging().AddC9GameData(connection).BuildServiceProvider();
        try
        {
            await portal.MigrateGameDataAsync();
            IDbContextFactory<GameDbContext> factory = portal.GetRequiredService<IDbContextFactory<GameDbContext>>();
            AccountService accounts = portal.GetRequiredService<AccountService>();
            SecondPasswordService passwords = portal.GetRequiredService<SecondPasswordService>();
            OperatorContext op = new("validation", "2nd password database case", Guid.NewGuid());
            Account player = await accounts.CreateAsync("pwplayer", null, op);
            uint pin = ClientCrc("1111"), wrong = ClientCrc("2222");

            Check.Equal(SecondPasswordStatus.None, await passwords.StatusAsync(player.Id));
            Check.Equal(SecondPasswordOutcome.NotSet, (await passwords.VerifyAsync(player.Id, pin, op)).Outcome);
            SecondPasswordResult set = await passwords.SetAsync(player.Id, pin, 4, "Seoul", op);
            Check.True(set.Outcome == SecondPasswordOutcome.Ok && set.Status.IsSet && set.Status.FailCount == 0, "set");
            Check.Equal(SecondPasswordOutcome.AlreadySet, (await passwords.SetAsync(player.Id, wrong, 1, "x", op)).Outcome);
            await Check.ThrowsAsync<DbUpdateException>(() => passwords.SetAsync(player.Id + 1000, pin, 1, "x", op)); // No such account (FK).

            // Two sessions failing at the same time: both failures count (xmin retry).
            SecondPasswordResult[] racing = await Task.WhenAll(
                passwords.VerifyAsync(player.Id, wrong, op), passwords.VerifyAsync(player.Id, wrong, op));
            Check.True(racing.All(r => r.Outcome == SecondPasswordOutcome.WrongPassword), "both wrong");
            Check.Equal((short)2, (await passwords.StatusAsync(player.Id)).FailCount);
            for (int attempt = 3; attempt < 5; attempt++)
                Check.Equal(SecondPasswordOutcome.WrongPassword, (await passwords.VerifyAsync(player.Id, wrong, op)).Outcome);
            SecondPasswordResult lockedNow = await passwords.VerifyAsync(player.Id, wrong, op);
            Check.True(lockedNow.Outcome == SecondPasswordOutcome.LockedNow && lockedNow.Status.IsLocked, "locked");
            Check.Equal(SecondPasswordOutcome.Locked, (await passwords.VerifyAsync(player.Id, pin, op)).Outcome);
            Check.Equal(SecondPasswordOutcome.Locked, (await passwords.ResetAsync(player.Id, 4, "seoul", op)).Outcome);

            await using (GameDbContext db = await factory.CreateDbContextAsync())
            {
                AccountSecondPassword row = await db.AccountSecondPasswords.SingleAsync(p => p.AccountId == player.Id);
                Check.True(row.FailCount == 5 && row.LockedAt is not null && row.Question == 4, "row");
                Check.True(row.PasswordHash.StartsWith("pbkdf2-sha256$", StringComparison.Ordinal) && !row.AnswerHash.Contains("SEOUL"), "hashed");
                // The database rejects values the client cannot produce.
                row.Question = 12;
                await Check.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
            }

            // Operator unlock: the GM clears it, then the player sets a new one and resets it with the answer.
            Check.Equal(SecondPasswordOutcome.Ok, (await passwords.ClearAsync(player.Id, op)).Outcome);
            Check.Equal(SecondPasswordStatus.None, await passwords.StatusAsync(player.Id));
            Check.Equal(SecondPasswordOutcome.Ok, (await passwords.SetAsync(player.Id, pin, 4, "Seoul", op)).Outcome);
            Check.Equal(SecondPasswordOutcome.WrongAnswer, (await passwords.ResetAsync(player.Id, 4, "Busan", op)).Outcome);
            Check.Equal(SecondPasswordOutcome.Ok, (await passwords.ResetAsync(player.Id, 4, " seoul ", op)).Outcome);
            Check.Equal(SecondPasswordStatus.None, await passwords.StatusAsync(player.Id));

            await using (GameDbContext db = await factory.CreateDbContextAsync())
            {
                string target = player.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
                AuditEntry[] audit = await db.AuditLog.AsNoTracking()
                    .Where(a => a.TargetType == "account" && a.TargetId == target && a.Action.StartsWith("account.second_password."))
                    .OrderBy(a => a.Id).ToArrayAsync();
                Check.Equal("account.second_password.set,account.second_password.lock,account.second_password.clear," +
                    "account.second_password.set,account.second_password.reset", string.Join(',', audit.Select(a => a.Action)));
                Check.True(audit.All(a => a.Before?.Contains("pbkdf2", StringComparison.Ordinal) != true &&
                    a.After?.Contains("pbkdf2", StringComparison.Ordinal) != true &&
                    a.After?.Contains("SEOUL", StringComparison.OrdinalIgnoreCase) != true), "no hashes or answers in the audit log");
                Check.True(audit.Select(a => a.RequestId).Distinct().Count() == audit.Length, "one request id per audit row");
            }

            // The lobby on the database: the local account's password, its state in the select-ack, audited as c9-lobby.
            CollectedEvents events = new();
            await using (LobbyPipe lobby = new(new SessionProcessor(SecondPasswordLobbyOptions(store, connection), events)))
            {
                byte[] identity = await SecondPasswordCharacterAsync(lobby, "PwDb");
                long local = lobby.Session.Snapshot().AccountId ?? throw new InvalidOperationException("no account");
                await lobby.SendAsync(SecondPasswordSet(pin, 7, "Ferrucio"));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x2745, 0);
                await lobby.SendAsync(SecondPasswordCertify(wrong));
                CheckSecondPasswordError(await lobby.ReceiveAsync(), 0x274A, InvalidSecondPassword, 1);
                await lobby.SendAsync(new Packet(0x0BBF, identity));
                CheckSecondPasswordSelect(await lobby.ReceiveAsync(), 1, 1, 0, 0);
                Check.Equal((ushort)0x0BC3, (await lobby.ReceiveAsync()).Type);
                await lobby.SendAsync(SecondPasswordCertify(pin));
                CheckSecondPasswordAck(await lobby.ReceiveAsync(), 0x274B, 1);
                SecondPasswordStatus status = await passwords.StatusAsync(local);
                Check.True(status.IsSet && status.FailCount == 0, "certify cleared the failure in the database");
                await using GameDbContext db = await factory.CreateDbContextAsync();
                string target = local.ToString(System.Globalization.CultureInfo.InvariantCulture);
                Check.True(await db.AuditLog.AnyAsync(a => a.Operator == "c9-lobby" && a.Action == "account.second_password.set" && a.TargetId == target),
                    "lobby audit row");
            }
            Check.True(!events.Values.Any(e => e.Event == "session_error"), "no session error");
        }
        finally
        {
            NpgsqlConnection.ClearAllPools();
            await using NpgsqlConnection server = new(admin);
            await server.OpenAsync();
            await using NpgsqlCommand drop = new($"DROP DATABASE IF EXISTS {database} WITH (FORCE)", server);
            await drop.ExecuteNonQueryAsync();
            File.Delete(store);
        }
    }
}
