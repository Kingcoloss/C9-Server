using System.Buffers.Binary;
using System.Text.Json;
using C9.Protocol;

namespace C9.Server;

// One NPC placed on a map/sub-map. InfoId is the client NPC template id
// (looked up by 008CEAB0 in the table at [0179FFD4]+0x74).
public sealed record NpcSpawn(uint InfoId, MapPosition Position, float Yaw, uint Hp = 1, uint MaxHp = 1, MapPosition? Up = null)
{
    // The client builds players with (0, 1, 0) here (008CE7B0 -> 007E0FA0); map placement records carry the same unit vector.
    public static readonly MapPosition DefaultUp = new(0, 1, 0);
}

// Client b4580ed4, dispatcher C9NetworkCharacterMove 008CE070:
//   0x0BE3 (case 008CE16F): u8 count + count x 0x70-byte entries, each handled by 008CEAB0
//          -> event 0x16 via 007E0FA0 with object type 2 (PC is 1).
//   0x0BE4: u8 count + count x u64 object id (00780180 removes the objects).
// The name order PS_Exist_PC_Ack / PS_Exist_NPC_Ack / PS_Remove_Object_Ack matches
// 0x0BE2 / 0x0BE3 / 0x0BE4 (Ref 2014 catalog ids 3042-3044). Frame flags follow the client
// registration table (and the Ref catalog): 0x0BE3 encrypted (1), 0x0BE4 plain (0).
// Unknown entry fields stay zero until the Ref server confirms them.
public static class NpcPackets
{
    public const ushort Exist = 0x0BE3;
    public const ushort Remove = 0x0BE4;
    public const int EntryBytes = 0x70;
    public const int MaxPerPacket = byte.MaxValue;

    public static IEnumerable<Packet> ExistPackets(IReadOnlyList<(ulong ObjectId, NpcSpawn Spawn)> npcs)
    {
        for (int start = 0; start < npcs.Count; start += MaxPerPacket)
        {
            int count = Math.Min(MaxPerPacket, npcs.Count - start);
            byte[] payload = new byte[1 + count * EntryBytes];
            payload[0] = (byte)count;
            for (int i = 0; i < count; i++)
                WriteEntry(payload.AsSpan(1 + i * EntryBytes, EntryBytes), npcs[start + i].ObjectId, npcs[start + i].Spawn);
            yield return new Packet(Exist, payload, flag: 1);
        }
    }

    public static IEnumerable<Packet> RemovePackets(IReadOnlyList<ulong> objectIds)
    {
        for (int start = 0; start < objectIds.Count; start += MaxPerPacket)
        {
            int count = Math.Min(MaxPerPacket, objectIds.Count - start);
            byte[] payload = new byte[1 + count * sizeof(ulong)];
            payload[0] = (byte)count;
            for (int i = 0; i < count; i++)
                BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(1 + i * sizeof(ulong)), objectIds[start + i]);
            yield return new Packet(Remove, payload, flag: 0);
        }
    }

    internal static void WriteEntry(Span<byte> entry, ulong objectId, NpcSpawn npc)
    {
        entry.Clear();
        // +0x00 / +0x04 -> event +0x50 / +0x38 (meaning not confirmed, zero).
        BinaryPrimitives.WriteUInt64LittleEndian(entry[0x08..], objectId);
        // +0x10 / +0x14 -> event +0x40 / +0x44. For PCs these are HP/MaxHP; zero HP marks a dead actor.
        BinaryPrimitives.WriteUInt32LittleEndian(entry[0x10..], npc.Hp);
        BinaryPrimitives.WriteUInt32LittleEndian(entry[0x14..], npc.MaxHp);
        BinaryPrimitives.WriteUInt32LittleEndian(entry[0x18..], npc.InfoId);
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x1C..], npc.Position.X);
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x20..], npc.Position.Y);
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x24..], npc.Position.Z);
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x28..], npc.Yaw);
        // +0x2C..+0x37: up vector (event +0x20..+0x28).
        MapPosition up = npc.Up ?? NpcSpawn.DefaultUp;
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x2C..], up.X);
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x30..], up.Y);
        BinaryPrimitives.WriteSingleLittleEndian(entry[0x34..], up.Z);
        // +0x38..+0x4F and the 32-byte record at +0x50 stay zero.
    }
}

// Per-map NPC list loaded from JSON:
// { "maps": [ { "map_id": 10, "submap": 0, "npcs": [ { "info_id": 1, "x": 0, "y": 0, "z": 0, "yaw": 0 } ] } ] }
// Optional per NPC: "hp" (default 1), "up": [x, y, z] (default [0, 1, 0]); other keys such as "name" are notes.
public sealed class NpcSpawnCatalog
{
    // Object ids above every character id, unique per map/sub-map and slot.
    public const ulong ObjectIdBase = 0x4E50_0000_0000_0000;
    public static readonly NpcSpawnCatalog Empty = new(new Dictionary<(int, int), IReadOnlyList<NpcSpawn>>());
    private readonly IReadOnlyDictionary<(int MapId, int Submap), IReadOnlyList<NpcSpawn>> maps;

    public NpcSpawnCatalog(IReadOnlyDictionary<(int MapId, int Submap), IReadOnlyList<NpcSpawn>> maps) => this.maps = maps;

    public int Count => maps.Values.Sum(list => list.Count);

    public IReadOnlyList<(ulong ObjectId, NpcSpawn Spawn)> For(int mapId, int submap)
    {
        if (!maps.TryGetValue((mapId, submap), out IReadOnlyList<NpcSpawn>? list)) return [];
        return list.Select((npc, i) => (ObjectId(mapId, submap, i), npc)).ToArray();
    }

    public static ulong ObjectId(int mapId, int submap, int slot) =>
        ObjectIdBase | ((ulong)(uint)mapId << 24) | ((ulong)(byte)submap << 16) | (ushort)slot;

    public static NpcSpawnCatalog Load(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return Empty;
        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
        return Parse(document.RootElement);
    }

    public static NpcSpawnCatalog Parse(JsonElement root)
    {
        var maps = new Dictionary<(int, int), IReadOnlyList<NpcSpawn>>();
        foreach (JsonElement map in root.GetProperty("maps").EnumerateArray())
        {
            int mapId = map.GetProperty("map_id").GetInt32();
            int submap = map.TryGetProperty("submap", out JsonElement s) ? s.GetInt32() : 0;
            if (mapId <= 0 || submap is < 0 or > byte.MaxValue)
                throw new ArgumentException($"Invalid NPC map/sub-map {mapId}/{submap}");
            var list = new List<NpcSpawn>();
            foreach (JsonElement npc in map.GetProperty("npcs").EnumerateArray())
            {
                var position = new MapPosition(npc.GetProperty("x").GetSingle(), npc.GetProperty("y").GetSingle(),
                    npc.GetProperty("z").GetSingle());
                float yaw = npc.TryGetProperty("yaw", out JsonElement y) ? y.GetSingle() : 0;
                uint hp = npc.TryGetProperty("hp", out JsonElement h) ? h.GetUInt32() : 1;
                MapPosition? up = null;
                if (npc.TryGetProperty("up", out JsonElement u))
                {
                    if (u.GetArrayLength() != 3) throw new ArgumentException($"NPC up vector on map {mapId}/{submap} needs three numbers");
                    up = new MapPosition(u[0].GetSingle(), u[1].GetSingle(), u[2].GetSingle());
                }
                if (!position.IsValid || !float.IsFinite(yaw) || hp == 0 || up is { IsValid: false })
                    throw new ArgumentException($"Invalid NPC entry on map {mapId}/{submap}");
                list.Add(new NpcSpawn(npc.GetProperty("info_id").GetUInt32(), position, yaw, hp, hp, up));
            }
            if (list.Count > ushort.MaxValue) throw new ArgumentException($"Too many NPCs on map {mapId}/{submap}");
            if (!maps.TryAdd((mapId, submap), list.AsReadOnly()))
                throw new ArgumentException($"Duplicate NPC map/sub-map {mapId}/{submap}");
        }
        return new NpcSpawnCatalog(maps);
    }
}
