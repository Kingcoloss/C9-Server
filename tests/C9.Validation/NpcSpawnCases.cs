using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private const string NpcCatalogJson = """
        { "maps": [
          { "map_id": 10, "submap": 0, "npcs": [
            { "info_id": 1001, "x": 38496.5, "y": 3501.25, "z": 35871.75, "yaw": 1.5 },
            { "info_id": 1002, "x": 100, "y": 200, "z": 300, "hp": 50 } ] },
          { "map_id": 19, "npcs": [] } ] }
        """;

    private static void NpcSpawnPacketLayout()
    {
        using JsonDocument document = JsonDocument.Parse(NpcCatalogJson);
        NpcSpawnCatalog catalog = NpcSpawnCatalog.Parse(document.RootElement);
        Check.Equal(2, catalog.Count);
        Check.Equal(0, catalog.For(19, 0).Count);
        Check.Equal(0, catalog.For(10, 1).Count);
        Check.Equal(0, NpcPackets.ExistPackets(catalog.For(19, 0)).Count());

        IReadOnlyList<(ulong ObjectId, NpcSpawn Spawn)> town = catalog.For(10, 0);
        Packet[] packets = NpcPackets.ExistPackets(town).ToArray();
        Check.Equal(1, packets.Length);
        Packet exist = packets[0];
        Check.Equal((ushort)0x0BE3, exist.Type);
        Check.Equal((byte)1, exist.Flag);
        Check.Equal(1 + 2 * NpcPackets.EntryBytes, exist.Payload.Length);
        ReadOnlySpan<byte> payload = exist.Payload.Span;
        Check.Equal((byte)2, payload[0]);
        ReadOnlySpan<byte> first = payload.Slice(1, NpcPackets.EntryBytes);
        ReadOnlySpan<byte> second = payload.Slice(1 + NpcPackets.EntryBytes, NpcPackets.EntryBytes);
        Check.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(first));
        Check.Equal(NpcSpawnCatalog.ObjectId(10, 0, 0), BinaryPrimitives.ReadUInt64LittleEndian(first[0x08..]));
        Check.Equal(NpcSpawnCatalog.ObjectId(10, 0, 1), BinaryPrimitives.ReadUInt64LittleEndian(second[0x08..]));
        Check.True(NpcSpawnCatalog.ObjectId(10, 0, 0) != NpcSpawnCatalog.ObjectId(10, 1, 0), "Sub-maps need distinct object ids");
        Check.True(NpcSpawnCatalog.ObjectId(10, 0, 0) > uint.MaxValue, "NPC ids must stay above character ids");
        Check.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(first[0x10..]));
        Check.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(first[0x14..]));
        Check.Equal(50u, BinaryPrimitives.ReadUInt32LittleEndian(second[0x10..]));
        Check.Equal(1001u, BinaryPrimitives.ReadUInt32LittleEndian(first[0x18..]));
        Check.Equal(1002u, BinaryPrimitives.ReadUInt32LittleEndian(second[0x18..]));
        Check.Equal(38496.5f, BinaryPrimitives.ReadSingleLittleEndian(first[0x1C..]));
        Check.Equal(3501.25f, BinaryPrimitives.ReadSingleLittleEndian(first[0x20..]));
        Check.Equal(35871.75f, BinaryPrimitives.ReadSingleLittleEndian(first[0x24..]));
        Check.Equal(1.5f, BinaryPrimitives.ReadSingleLittleEndian(first[0x28..]));
        Check.Bytes(first.Slice(0x1C, 12), first.Slice(0x2C, 12));
        Check.True(first[0x38..].IndexOfAnyExcept((byte)0) < 0, "Unconfirmed fields must stay zero");

        // More NPCs than a u8 count holds are split over several packets.
        var many = Enumerable.Range(0, 300)
            .Select(i => (NpcSpawnCatalog.ObjectId(10, 0, i), new NpcSpawn(7, new MapPosition(i, 0, 0), 0))).ToArray();
        Packet[] split = NpcPackets.ExistPackets(many).ToArray();
        Check.Equal(2, split.Length);
        Check.Equal((byte)255, split[0].Payload.Span[0]);
        Check.Equal((byte)45, split[1].Payload.Span[0]);
        Check.Equal(1 + 45 * NpcPackets.EntryBytes, split[1].Payload.Length);
        Check.Equal(NpcSpawnCatalog.ObjectId(10, 0, 255), BinaryPrimitives.ReadUInt64LittleEndian(split[1].Payload.Span[9..]));

        Packet remove = NpcPackets.RemovePackets([5ul, NpcSpawnCatalog.ObjectId(10, 0, 1)]).Single();
        Check.Equal((ushort)0x0BE4, remove.Type);
        Check.Equal((byte)0, remove.Flag);
        Check.Equal(17, remove.Payload.Length);
        Check.Equal((byte)2, remove.Payload.Span[0]);
        Check.Equal(5ul, BinaryPrimitives.ReadUInt64LittleEndian(remove.Payload.Span[1..]));
        Check.Equal(NpcSpawnCatalog.ObjectId(10, 0, 1), BinaryPrimitives.ReadUInt64LittleEndian(remove.Payload.Span[9..]));

        foreach (string bad in new[]
        {
            """{ "maps": [ { "map_id": 10, "npcs": [ { "info_id": 1, "x": 0, "y": 0, "z": 0, "hp": 0 } ] } ] }""",
            """{ "maps": [ { "map_id": 10, "npcs": [ { "info_id": 1, "x": 1e9, "y": 0, "z": 0 } ] } ] }""",
            """{ "maps": [ { "map_id": 10, "npcs": [] }, { "map_id": 10, "submap": 0, "npcs": [] } ] }""",
            """{ "maps": [ { "map_id": 0, "npcs": [] } ] }""",
            """{ "maps": [ { "map_id": 10, "submap": 256, "npcs": [] } ] }"""
        })
        {
            using JsonDocument invalid = JsonDocument.Parse(bad);
            Check.Throws<ArgumentException>(() => NpcSpawnCatalog.Parse(invalid.RootElement));
        }
        Check.Equal(0, NpcSpawnCatalog.Load(null).Count);
    }

    private static async Task MemoryNpcSpawn()
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-npc-store-{Guid.NewGuid():N}.json");
        string npcFile = Path.Combine(Path.GetTempPath(), $"c9-npc-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(npcFile, NpcCatalogJson);
        try
        {
            ServerOptions config = new()
            {
                Profile = "character_lobby", CharacterStore = store, NpcSpawnFile = npcFile,
                TimeoutSeconds = 3, MaxPackets = 255
            };
            await using MemorySession session = new(config);
            Check.Equal((ushort)4, (await session.ReceiveAsync()).Type);
            await session.SendAsync(new Packet(0x1771, new byte[49]));
            Check.Equal((ushort)0x0EFA, (await session.ReceiveAsync()).Type);
            byte[] create = new byte[90];
            create[0] = 2;
            Encoding.Unicode.GetBytes("NpcTown").CopyTo(create, 3);
            await session.SendAsync(new Packet(0x0BBB, create));
            byte[] identity = (await session.ReceiveAsync()).Payload.Span[..8].ToArray();
            await session.SendAsync(new Packet(0x0BBF, identity));
            Check.Equal((ushort)0x0BC0, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0C1E, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0C1C, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0BC3, (await session.ReceiveAsync()).Type);

            await session.SendAsync(new Packet(0x0BC5));
            Check.Equal((ushort)0x0BC6, (await session.ReceiveAsync()).Type);
            Packet npcs = await session.ReceiveAsync();
            Check.Equal((ushort)0x0BE3, npcs.Type);
            Check.Equal((byte)2, npcs.Payload.Span[0]);
            Check.Equal(1001u, BinaryPrimitives.ReadUInt32LittleEndian(npcs.Payload.Span[(1 + 0x18)..]));

            // A second load notification must not announce the NPCs again.
            await session.SendAsync(new Packet(0x0BC5));
            await session.SendAsync(new Packet(0x0EC4));
            Check.Equal((ushort)0x2EEB, (await session.ReceiveAsync()).Type);
            await session.CompleteInputAsync();
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            ServerEvent spawned = session.Events.Values.Single(e => e.Event == "npc_spawn");
            Check.Equal(10, spawned.MapId ?? -1);
            Check.True(!session.Events.Values.Any(e => e.Event == "session_error"));
        }
        finally
        {
            File.Delete(store);
            File.Delete(npcFile);
        }
    }
}
