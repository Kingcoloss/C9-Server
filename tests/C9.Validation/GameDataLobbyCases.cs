using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Protocol;
using C9.Server;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

internal static partial class Validation
{
    private const int ListEntry = 136; // Bytes per character in the 0x0EFA list.

    /// <summary>
    /// Character lobby on the game database (C9_DB_CONNECTION). Needs C9_DB_ADMIN_CONNECTION (a role that may create
    /// databases); a temporary database is created and dropped. Without it the case only checks that the option is off.
    /// </summary>
    private static async Task GameDataLobby()
    {
        ServerOptions official;
        using (JsonDocument fixture = Fixture("lab-official-character-lobby.json"))
            official = ServerOptions.FromJson(fixture.RootElement.GetRawText());
        Check.True(official.GameDatabase is null, "profiles never enable the database");
        Check.Throws<ArgumentException>(() => (LabOptions() with { GameDatabase = "Host=127.0.0.1" }).Validate());

        string? admin = Environment.GetEnvironmentVariable("C9_DB_ADMIN_CONNECTION");
        if (string.IsNullOrWhiteSpace(admin))
        {
            Console.Error.WriteLine("note: lobby database checks skipped (set C9_DB_ADMIN_CONNECTION to run them)");
            return;
        }

        string database = "c9_lobby_" + Guid.NewGuid().ToString("N")[..12];
        string connection = new NpgsqlConnectionStringBuilder(admin) { Database = database }.ConnectionString;
        string store = Path.Combine(Path.GetTempPath(), $"c9-lobby-db-{Guid.NewGuid():N}.json");
        byte[] appearance = Enumerable.Range(0, 44).Select(i => (byte)(i + 1)).ToArray();
        // The legacy JSON format the lobby wrote before (Slot, Job, Name, Appearance as base64).
        File.WriteAllText(store, JsonSerializer.Serialize(new object[]
        {
            new { Slot = 0, Job = 1, Name = "Lab", Appearance = appearance },
            new { Slot = 1, Job = 9, Name = "Lab2", Appearance = appearance }
        }));
        byte[] legacyBytes = File.ReadAllBytes(store);
        ServerOptions options = official with
        {
            Port = 0, TimeoutSeconds = 2, MaxPackets = 255, CharacterStore = store, GameDatabase = connection
        };
        options.Validate();

        await using (NpgsqlConnection server = new(admin))
        {
            await server.OpenAsync();
            await using NpgsqlCommand create = new($"CREATE DATABASE {database}", server);
            await create.ExecuteNonQueryAsync();
        }
        await using ServiceProvider portal = new ServiceCollection().AddLogging().AddC9GameData(connection).BuildServiceProvider();
        try
        {
            // 1. The schema is not there yet: the lobby refuses (it never migrates) and stays usable for a retry.
            CollectedEvents events = new();
            SessionProcessor processor = new(options, events);
            await using (LobbyPipe early = new(processor))
            {
                await early.SendAsync(new Packet(0x1771, new byte[49]));
                await early.Completion.WaitAsync(TimeSpan.FromSeconds(10));
                Check.True(events.Values.Any(e => e.Event == "session_error" && e.Reason?.StartsWith("Game database is not ready", StringComparison.Ordinal) == true),
                    "not-ready error reported");
                Check.Equal<long?>(null, early.Session.Snapshot().AccountId);
            }

            // 2. The Portal migrates. Another account owns the first character id, so lobby ids cannot be Slot+1.
            await portal.MigrateGameDataAsync();
            AccountService accounts = portal.GetRequiredService<AccountService>();
            CharacterService characters = portal.GetRequiredService<CharacterService>();
            OperatorContext op = new("validation", "lobby database case", Guid.NewGuid());
            Account other = await accounts.CreateAsync("other", null, op);
            await characters.CreateAsync(other.Id, 0, 1, "Other", appearance);

            // 3. Same processor, next login: JSON characters imported once with their slots, database ids in the packets.
            ulong lab2Id, lab3Id;
            long localId;
            await using (LobbyPipe lobby = new(processor))
            {
                await lobby.SendAsync(new Packet(0x1771, new byte[49]));
                Packet list = await lobby.ReceiveAsync();
                Account local = await accounts.FindByLoginAsync(AccountService.LocalAccount) ?? throw new InvalidOperationException("local account missing");
                localId = local.Id;
                IReadOnlyList<Character> rows = await characters.ListAsync(local.Id);
                Check.Equal(2, rows.Count);
                Character lab = rows.Single(c => c.Name == "Lab"), lab2 = rows.Single(c => c.Name == "Lab2");
                Check.True(lab.Slot == 0 && lab2.Slot == 1 && lab2.Job == 9, "slots and jobs kept");
                int header = options.CharacterListHeaderBytes;
                Check.Equal((ushort)0x0EFA, list.Type);
                Check.Equal(header + 2 * ListEntry, list.Payload.Length);
                Check.Bytes(new byte[] { (byte)options.CharacterSlots, 2 }, list.Payload.Span.Slice(header - 2, 2));
                Check.Equal((ulong)lab.Id, BinaryPrimitives.ReadUInt64LittleEndian(list.Payload.Span[header..]));
                Check.True(lab.Id != 1, "database id, not Slot+1");
                Check.Equal((ulong)lab2.Id, BinaryPrimitives.ReadUInt64LittleEndian(list.Payload.Span[(header + ListEntry)..]));
                Check.Equal<long?>(local.Id, lobby.Session.Snapshot().AccountId);
                Check.Equal("character_lobby", lobby.Session.Snapshot().State);
                Check.True(events.Values.Any(e => e.Event == "legacy_import" && e.Reason == "2 imported, 0 skipped from " + Path.GetFileName(store)), "import event");
                Check.Equal(1, events.Values.Count(e => e.Event == "character_store_ready"));
                lab2Id = (ulong)lab2.Id;

                // Create in slot 2; an identical retry returns the same character.
                byte[] create = new byte[90];
                create[0] = 2;
                Encoding.Unicode.GetBytes("Lab3").CopyTo(create, 3);
                appearance.CopyTo(create, 45);
                await lobby.SendAsync(new Packet(0x0BBB, create));
                Packet ack = await lobby.ReceiveAsync();
                Check.Equal((ushort)0x0BBC, ack.Type);
                Check.Equal(113, ack.Payload.Length);
                Character lab3 = (await characters.ListAsync(local.Id)).Single(c => c.Name == "Lab3");
                lab3Id = (ulong)lab3.Id;
                Check.Equal(lab3Id, BinaryPrimitives.ReadUInt64LittleEndian(ack.Payload.Span));
                Check.True(lab3.Slot == 2 && lab3.Job == 1, "created in the database");
                await lobby.SendAsync(new Packet(0x0BBB, create));
                Packet retry = await lobby.ReceiveAsync();
                Check.Bytes(ack.Payload.Span, retry.Payload.Span);

                byte[] identity = new byte[8];
                BinaryPrimitives.WriteUInt64LittleEndian(identity, lab2Id);
                await lobby.SendAsync(new Packet(0x0EFB, identity));
                Packet details = await lobby.ReceiveAsync();
                Check.Equal((ushort)0x0EFC, details.Type);
                Check.Bytes(identity, details.Payload.Span[..8]);
                // Stat system: Lab2 (job 9 = Shaman line) enters with the Shaman base Max HP/MP plus spent points.
                StatService stats = portal.GetRequiredService<StatService>();
                await stats.SetClassBaseAsync(1, new Dictionary<string, int> { ["max_hp"] = 240, ["max_mp"] = 80 }, op with { RequestId = Guid.NewGuid() });
                await stats.UpdateDefinitionsAsync([new("max_hp", "Maximum HP", true, 5)], op with { RequestId = Guid.NewGuid() });
                await stats.SpendAsync(local.Id, (long)lab2Id, new Dictionary<string, int> { ["max_hp"] = 2 }, op with { RequestId = Guid.NewGuid() });
                await lobby.SendAsync(new Packet(0x0BBF, identity));
                Packet selected = await lobby.ReceiveAsync();
                Check.Equal((ushort)0x0BC0, selected.Type);
                Check.Equal(lab2Id, BinaryPrimitives.ReadUInt64LittleEndian(selected.Payload.Span));
                Check.Equal(9u, BinaryPrimitives.ReadUInt32LittleEndian(selected.Payload.Span[12..]));
                Check.Equal(0L, BinaryPrimitives.ReadInt64LittleEndian(selected.Payload.Span[24..])); // m_nExp
                Check.Equal(250u, BinaryPrimitives.ReadUInt32LittleEndian(selected.Payload.Span[32..]));
                Check.Equal(80u, BinaryPrimitives.ReadUInt32LittleEndian(selected.Payload.Span[36..]));
                Check.Equal((ushort)0x0BC3, (await lobby.ReceiveAsync()).Type);
                await lobby.SendAsync(new Packet(0x0BC5));
                Packet status = await lobby.ReceiveAsync();
                Check.Equal((ushort)0x0C1E, status.Type);
                Check.Equal(250L, BinaryPrimitives.ReadInt64LittleEndian(status.Payload.Span[0x0C..])); // m_nMaxHP
                Check.Equal(80, BinaryPrimitives.ReadInt32LittleEndian(status.Payload.Span[0x14..])); // m_nMaxMP
                Check.Equal(6, BinaryPrimitives.ReadInt32LittleEndian(status.Payload.Span[0x18..])); // m_nSTR, Client Shaman base
                Check.Equal(9, BinaryPrimitives.ReadInt32LittleEndian(status.Payload.Span[0x24..])); // m_nWIS
                Check.Equal((short)1000, BinaryPrimitives.ReadInt16LittleEndian(status.Payload.Span[0x38..])); // final move speed
                Check.Equal(0, BinaryPrimitives.ReadInt32LittleEndian(status.Payload.Span[0x70..])); // m_nMinDD[0]
                Check.Equal<ulong?>(lab2Id, lobby.Session.Snapshot().CharacterId);
                Check.Equal<long?>(local.Id, lobby.Session.Snapshot().AccountId);
                // S01: the last reported position is written when the session ends.
                await lobby.SendAsync(new Packet(0x0BCE, MoveAt(1234.5f, 678.25f, -910.75f)));
            }
            Check.Bytes(legacyBytes, File.ReadAllBytes(store)); // The JSON store is no longer written.
            Character walked = await characters.GetAsync((long)lab2Id) ?? throw new InvalidOperationException("Lab2 missing");
            Check.True(walked is { Submap: 0, PosX: 1234.5f, PosY: 678.25f, PosZ: -910.75f, LastPlayedAt: not null } &&
                walked.MapId == options.InitialMapId, "position saved in the database");
            Check.True(events.Values.Any(e => e.Event == "position_saved" && e.CharacterId == lab2Id), "position_saved event");
            // The game-server write touches only a live character of the given account and checks its input.
            Check.True(!await characters.SaveWorldPositionAsync((long)lab2Id, other.Id, 10, 0, 1, 2, 3, 0), "another account's character");
            try
            {
                await characters.SaveWorldPositionAsync((long)lab2Id, localId, 10, 0, float.NaN, 0, 0, 0);
                Check.True(false, "NaN position rejected");
            }
            catch (GameDataException error) { Check.Equal(GameDataError.Invalid, error.Code); }

            // 4. Portal edits reach the game: rename Lab, delete Lab3. A restarted lobby does not import again.
            Character renamed = (await characters.ListAsync(localId)).Single(c => c.Name == "Lab");
            await characters.RenameAsync(renamed.Id, renamed.Version, "LabRenamed", op with { RequestId = Guid.NewGuid() });
            Character deleted = await characters.GetAsync((long)lab3Id) ?? throw new InvalidOperationException("Lab3 missing");
            await characters.SoftDeleteAsync(deleted.Id, deleted.Version, op with { RequestId = Guid.NewGuid() });
            await using (LobbyPipe restarted = new(new SessionProcessor(options, new CollectedEvents())))
            {
                await restarted.SendAsync(new Packet(0x1771, new byte[49]));
                Packet list = await restarted.ReceiveAsync();
                int header = options.CharacterListHeaderBytes;
                Check.Equal(header + 2 * ListEntry, list.Payload.Length);
                Check.Bytes(Encoding.Unicode.GetBytes("LabRenamed"), list.Payload.Span.Slice(header + 8, 20));
                byte[] identity = new byte[8];
                BinaryPrimitives.WriteUInt64LittleEndian(identity, lab3Id);
                await restarted.SendAsync(new Packet(0x0BBF, identity)); // Deleted in the Portal: unknown to the lobby.
                await restarted.Completion.WaitAsync(TimeSpan.FromSeconds(10));
            }
            Check.Equal(1, events.Values.Count(e => e.Event == "legacy_import")); // Not repeated after a restart.
            Check.Equal(3, (await characters.ListAsync(localId, includeDeleted: true)).Count);
            Check.True(!await characters.SaveWorldPositionAsync((long)lab3Id, localId, 10, 0, 1, 2, 3, 0), "deleted character");

            // 5. S01: the next entry starts at the saved point. A point saved on another map is not used, because map
            // changes (S05) do not exist yet; the character enters the entry map at its spawn point.
            async Task<MapPosition> EntryAsync()
            {
                await using LobbyPipe again = new(new SessionProcessor(options, new CollectedEvents()));
                await again.SendAsync(new Packet(0x1771, new byte[49]));
                Check.Equal((ushort)0x0EFA, (await again.ReceiveAsync()).Type);
                byte[] identity = new byte[8];
                BinaryPrimitives.WriteUInt64LittleEndian(identity, lab2Id);
                await again.SendAsync(new Packet(0x0BBF, identity));
                Check.Equal((ushort)0x0BC0, (await again.ReceiveAsync()).Type);
                return EntryPosition(await again.ReceiveAsync());
            }
            Check.Equal(new MapPosition(1234.5f, 678.25f, -910.75f), await EntryAsync());
            Character elsewhere = await characters.GetAsync((long)lab2Id) ?? throw new InvalidOperationException("Lab2 missing");
            await characters.UpdateAsync(elsewhere.Id, elsewhere.Version,
                new CharacterChanges(MapId: options.InitialMapId + 1, Submap: 0, PosX: 1, PosY: 2, PosZ: 3), op with { RequestId = Guid.NewGuid() });
            Check.Equal(EntryPosition(WorldMapCatalog.Entry(options.InitialMapId, options.InitialSubmap, options.InitialPosition)), await EntryAsync());
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

    /// <summary>One lobby session over in-memory pipes with a keyed <see cref="SessionContext"/> (to read AccountId).</summary>
    private sealed class LobbyPipe : IAsyncDisposable
    {
        private static long next;
        private readonly Pipe input = new();
        private readonly Pipe output = new();
        private readonly CancellationTokenSource lifetime = new();
        private readonly ConnectionCodec client = new(ushort.MaxValue);
        public SessionContext Session { get; }
        public Task Completion { get; }

        public LobbyPipe(SessionProcessor processor)
        {
            long number = Interlocked.Increment(ref next);
            Session = new SessionContext(number, "lobby-test-" + number, "lobby");
            Completion = RunAsync(processor);
            Greeting = ReceiveAsync();
        }

        private Task<Packet> Greeting { get; }

        private async Task RunAsync(SessionProcessor processor)
        {
            try { await processor.RunAsync(Session, input.Reader, output.Writer, lifetime.Token); }
            finally { await output.Writer.CompleteAsync(); }
        }

        public async Task SendAsync(Packet packet)
        {
            Packet greeting = await Greeting;
            Check.Equal((ushort)4, greeting.Type);
            _ = await input.Writer.WriteAsync(client.Encode(packet));
        }

        public async Task<Packet> ReceiveAsync()
        {
            byte[]? wire = await LengthPrefixedPipe.ReadFrameAsync(output.Reader, ushort.MaxValue, TimeSpan.FromSeconds(10));
            Check.True(wire is not null, "Expected a reply frame");
            return client.Decode(wire!);
        }

        public async ValueTask DisposeAsync()
        {
            await input.Writer.CompleteAsync();
            try { await Completion.WaitAsync(TimeSpan.FromSeconds(10)); }
            finally { lifetime.Cancel(); lifetime.Dispose(); }
        }
    }
}
