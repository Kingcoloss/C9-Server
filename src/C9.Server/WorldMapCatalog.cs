using System.Buffers.Binary;
using System.Text.Json;
using C9.Protocol;

namespace C9.Server;

public sealed record MapPosition(float X, float Y, float Z)
{
    public bool IsValid => float.IsFinite(X) && float.IsFinite(Y) && float.IsFinite(Z) &&
        MathF.Abs(X) <= 1e7f && MathF.Abs(Y) <= 1e7f && MathF.Abs(Z) <= 1e7f;
}

public sealed record WorldMapRoute(int MapId, int Submap, string Name, string Folder,
    string Suffix, bool FilesReady, MapPosition? SamplePosition);

// Catalogued files/prefixes establish a route's asset availability, not gameplay.
// Sample positions are first-marker observations, not verified player spawn points.
public static class WorldMapCatalog
{
    private static readonly IReadOnlyList<WorldMapRoute> routes = Load();
    private static readonly IReadOnlyDictionary<(int MapId, int Submap), WorldMapRoute> lookup =
        routes.ToDictionary(r => (r.MapId, r.Submap));
    public static IReadOnlyList<WorldMapRoute> Routes => routes;

    public static (WorldMapRoute Route, MapPosition Position) Resolve(int mapId, int submap, MapPosition? position = null)
    {
        if (!lookup.TryGetValue((mapId, submap), out WorldMapRoute? route))
            throw new ArgumentException($"Unknown map/sub-map pair {mapId}/{submap}");
        if (!route.FilesReady)
            throw new ArgumentException($"Client geometry or marker files are missing for map/sub-map {mapId}/{submap}");
        MapPosition point = position ?? route.SamplePosition ??
            throw new ArgumentException($"Map/sub-map {mapId}/{submap} requires an explicit initial_position");
        if (!point.IsValid) throw new ArgumentException("Initial position must contain three finite coordinates within the diagnostic bounds");
        return (route, point);
    }

    public static Packet Entry(int mapId, int submap, MapPosition? position = null)
    {
        var (_, point) = Resolve(mapId, submap, position);
        // 00A6DAEA -> 009EC200 -> 004B5890: +4 is a zero-based sub-map
        // index. 00DA40E9..00DA4173 selects the corresponding marker suffix.
        byte[] payload = new byte[46];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, (uint)mapId);
        payload[4] = (byte)submap;
        BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(6), point.X);
        BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(10), point.Y);
        BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(14), point.Z);
        return new Packet(0x0BC3, payload);
    }

    private static IReadOnlyList<WorldMapRoute> Load()
    {
        using Stream stream = typeof(WorldMapCatalog).Assembly.GetManifestResourceStream("map-manifest.json")
            ?? throw new InvalidOperationException("Missing embedded map manifest");
        using JsonDocument document = JsonDocument.Parse(stream);
        JsonElement root = document.RootElement;
        if (root.GetProperty("schema_version").GetInt32() != 1 ||
            root.GetProperty("client_exe_sha256").GetString() != "b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25")
            throw new InvalidDataException("Map manifest does not match the selected Client");
        List<WorldMapRoute> result = [];
        HashSet<int> ids = [];
        foreach (JsonElement map in root.GetProperty("maps").EnumerateArray())
        {
            int id = map.GetProperty("id").GetInt32();
            if (id is < 1 or > 65535 || !ids.Add(id)) throw new InvalidDataException("Invalid map identity");
            string name = map.GetProperty("name").GetString()!;
            string folder = map.GetProperty("folder").GetString()!;
            int expectedIndex = 0;
            foreach (JsonElement variant in map.GetProperty("submaps").EnumerateArray())
            {
                int index = variant.GetProperty("index").GetInt32();
                if (index != expectedIndex++ || index > 255) throw new InvalidDataException("Invalid sub-map index");
                MapPosition? point = null;
                if (variant.GetProperty("markers").TryGetProperty("sample_position", out JsonElement coordinates))
                {
                    float[] values = coordinates.EnumerateArray().Select(v => v.GetSingle()).ToArray();
                    if (values.Length != 3) throw new InvalidDataException("Invalid map position length");
                    point = new(values[0], values[1], values[2]);
                    if (!point.IsValid) throw new InvalidDataException("Invalid map sample position");
                }
                result.Add(new(id, index, name, folder, variant.GetProperty("suffix").GetString()!,
                    variant.GetProperty("files_ready").GetBoolean(), point));
            }
        }
        if (ids.Count != root.GetProperty("map_count").GetInt32() || result.Count != root.GetProperty("submap_count").GetInt32() ||
            result.Count(r => r.FilesReady) != root.GetProperty("files_ready_submaps").GetInt32())
            throw new InvalidDataException("Map manifest count mismatch");
        return result.AsReadOnly();
    }
}
