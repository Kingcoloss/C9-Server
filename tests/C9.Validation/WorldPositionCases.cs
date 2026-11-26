using System.Buffers.Binary;
using System.Text;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    // Captured in game (trace lobby/20261120/e3bf4b-001.jsonl, session 5): the first 0x0BCE repeats the 0x0BC3 spawn point.
    private static readonly byte[] TraceMove = Convert.FromHexString(
        "f560164730d75a45de1f0c4700000000516e7b4b00000000000000000000000000000000000000000000fdf98716");
    private static readonly byte[] TraceAction = Convert.FromHexString(
        "bdef1647980a5b458b2b0d47e17b2f3f41f6cc560a0000000800000000000000430b8816");

    private static byte[] MoveAt(float x, float y, float z)
    {
        byte[] payload = TraceMove.ToArray();
        BinaryPrimitives.WriteSingleLittleEndian(payload, x);
        BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(4), y);
        BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(8), z);
        return payload;
    }

    private static MapPosition EntryPosition(Packet entry)
    {
        Check.Equal((ushort)0x0BC3, entry.Type);
        ReadOnlySpan<byte> p = entry.Payload.Span;
        return new MapPosition(BinaryPrimitives.ReadSingleLittleEndian(p[6..]), BinaryPrimitives.ReadSingleLittleEndian(p[10..]),
            BinaryPrimitives.ReadSingleLittleEndian(p[14..]));
    }

    private static void WorldPositionPacketLayout()
    {
        Check.True(WorldPositionPackets.TryRead(new Packet(0x0BCE, TraceMove), out MapPosition move), "0x0BCE position");
        Check.Equal(new MapPosition(38496.95703125f, 3501.44921875f, 35871.8671875f), move);
        // The same point the server sent in 0x0BC3 for map 10 in that session.
        Check.Equal(move, EntryPosition(WorldMapCatalog.Entry(10, 0, move)));
        Check.True(WorldPositionPackets.TryRead(new Packet(0x0BCC, TraceAction), out MapPosition action), "0x0BCC position");
        Check.Equal(new MapPosition(38639.73828125f, 3504.662109375f, 36139.54296875f), action);

        Check.True(!WorldPositionPackets.TryRead(new Packet(0x0BCE, TraceMove[..45]), out _), "short 0x0BCE");
        Check.True(!WorldPositionPackets.TryRead(new Packet(0x0BCC, TraceMove), out _), "0x0BCC with the 0x0BCE size");
        Check.True(!WorldPositionPackets.TryRead(new Packet(0x0BCB, TraceMove), out _), "other packet types");
        Check.True(!WorldPositionPackets.TryRead(new Packet(0x0BCE, MoveAt(float.NaN, 0, 0)), out _), "NaN");
        Check.True(!WorldPositionPackets.TryRead(new Packet(0x0BCE, MoveAt(0, float.PositiveInfinity, 0)), out _), "infinity");
        Check.True(!WorldPositionPackets.TryRead(new Packet(0x0BCE, MoveAt(0, 0, 2e7f)), out _), "outside the map bounds");
    }

    /// <summary>Selects <paramref name="identity"/> and loads the world; returns the 0x0BC3 entry.</summary>
    private static async Task<Packet> EnterWorldAsync(MemorySession session, byte[] identity)
    {
        await session.SendAsync(new Packet(0x0BBF, identity));
        Check.Equal((ushort)0x0BC0, (await session.ReceiveAsync()).Type);
        Packet entry = await session.ReceiveAsync();
        await session.SendAsync(new Packet(0x0BC5));
        Check.Equal((ushort)0x0C1E, (await session.ReceiveAsync()).Type);
        Check.Equal((ushort)0x0C20, (await session.ReceiveAsync()).Type);
        Check.Equal((ushort)0x0BC6, (await session.ReceiveAsync()).Type);
        return entry;
    }

    private static async Task MemoryWorldPositionPersistence()
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-world-position-{Guid.NewGuid():N}.json");
        try
        {
            ServerOptions config = new()
            {
                Profile = "character_lobby", CharacterStore = store, WorldVitals = WorldVitalsLayout.Extended,
                TimeoutSeconds = 3, MaxPackets = 255
            };
            CollectedEvents events = new();
            SessionProcessor processor = new(config, events);
            MapPosition defaultSpawn = EntryPosition(WorldMapCatalog.Entry(config.InitialMapId, config.InitialSubmap, config.InitialPosition));
            MapPosition later = new(100.5f, 200.25f, -300.75f);
            byte[] identity;

            // 1. First entry: the map's spawn point. Leaving with 0x2EEA saves the last reported point once.
            await using (MemorySession first = new(config, processor, events))
            {
                Check.Equal((ushort)4, (await first.ReceiveAsync()).Type);
                await first.SendAsync(new Packet(0x1771, new byte[49]));
                Check.Equal((ushort)0x0EFA, (await first.ReceiveAsync()).Type);
                byte[] create = new byte[90];
                create[0] = 1;
                Encoding.Unicode.GetBytes("Walker").CopyTo(create, 3);
                await first.SendAsync(new Packet(0x0BBB, create));
                Packet created = await first.ReceiveAsync();
                identity = created.Payload.Span[..8].ToArray();
                Check.Equal(defaultSpawn, EntryPosition(await EnterWorldAsync(first, identity)));
                Check.Equal(0, events.Values.Count(e => e.Event == "position_saved"));
                await first.SendAsync(new Packet(0x0BCE, TraceMove));
                await first.SendAsync(new Packet(0x0BCC, TraceAction));
                await first.SendAsync(new Packet(0x2EEA, new byte[16]));
                // Position packets get no reply: the next frame is the leave acknowledgement.
                Check.Equal((ushort)0x2EEB, (await first.ReceiveAsync()).Type);
                await first.CompleteInputAsync();
                await first.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            }
            ServerEvent saved = events.Values.Single(e => e.Event == "position_saved");
            Check.Equal<ulong?>(BinaryPrimitives.ReadUInt64LittleEndian(identity), saved.CharacterId);
            Check.Equal<int?>(config.InitialMapId, saved.MapId);

            // 2. Next entry starts at the saved point. Bad position packets are ignored; a disconnect also saves.
            await using (MemorySession second = new(config, processor, events))
            {
                Check.Equal((ushort)4, (await second.ReceiveAsync()).Type);
                await second.SendAsync(new Packet(0x1771, new byte[49]));
                Check.Equal((ushort)0x0EFA, (await second.ReceiveAsync()).Type);
                Packet entry = await EnterWorldAsync(second, identity);
                Check.Equal(new MapPosition(38639.73828125f, 3504.662109375f, 36139.54296875f), EntryPosition(entry));
                Check.Equal((uint)config.InitialMapId, BinaryPrimitives.ReadUInt32LittleEndian(entry.Payload.Span));
                await second.SendAsync(new Packet(0x0BCE, MoveAt(later.X, later.Y, later.Z)));
                await second.SendAsync(new Packet(0x0BCE, MoveAt(float.NaN, 1, 1)));
                await second.SendAsync(new Packet(0x0BCE, MoveAt(1, 1, 1)[..45]));
                await second.CompleteInputAsync();
                await second.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            }
            Check.Equal(2, events.Values.Count(e => e.Event == "position_saved"));

            // 3. The in-world exit (0x17D4) saves before its acknowledgement.
            await using (MemorySession third = new(config, processor, events))
            {
                Check.Equal((ushort)4, (await third.ReceiveAsync()).Type);
                await third.SendAsync(new Packet(0x1771, new byte[49]));
                Check.Equal((ushort)0x0EFA, (await third.ReceiveAsync()).Type);
                Check.Equal(later, EntryPosition(await EnterWorldAsync(third, identity)));
                await third.SendAsync(new Packet(0x17D4, new byte[16]));
                Check.Equal((ushort)0x17D5, (await third.ReceiveAsync()).Type);
                Check.Equal(3, events.Values.Count(e => e.Event == "position_saved"));
                await third.CompleteInputAsync();
                await third.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            }
            Check.Equal(3, events.Values.Count(e => e.Event == "position_saved"));
            Check.True(!events.Values.Any(e => e.Event is "position_save_failed" or "session_error"), "no save failure or session error");
            Check.True(!File.ReadAllText(store).Contains("38639", StringComparison.Ordinal), "the JSON file format is unchanged");
        }
        finally { File.Delete(store); }
    }
}
