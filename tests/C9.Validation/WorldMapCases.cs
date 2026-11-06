using System.Buffers.Binary;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private static void WorldMapDestinations()
    {
        Check.Equal(516, WorldMapCatalog.Routes.Count);
        Check.Equal(257, WorldMapCatalog.Routes.Select(r => r.MapId).Distinct().Count());
        Check.Equal(450, WorldMapCatalog.Routes.Count(r => r.FilesReady));
        Check.Equal(420, WorldMapCatalog.Routes.Count(r => r.SamplePosition is not null));
        foreach (WorldMapRoute route in WorldMapCatalog.Routes)
        {
            MapPosition point = route.SamplePosition ?? new MapPosition(1, 2, 3);
            if (!route.FilesReady)
            {
                Check.Throws<ArgumentException>(() => WorldMapCatalog.Entry(route.MapId, route.Submap, point));
                continue;
            }
            Packet entry = WorldMapCatalog.Entry(route.MapId, route.Submap, point);
            Check.Equal((ushort)0x0BC3, entry.Type);
            Check.Equal(46, entry.Payload.Length);
            Check.Equal((uint)route.MapId, BinaryPrimitives.ReadUInt32LittleEndian(entry.Payload.Span));
            Check.Equal((byte)route.Submap, entry.Payload.Span[4]);
            Check.Equal(point.X, BinaryPrimitives.ReadSingleLittleEndian(entry.Payload.Span[6..]));
            Check.Equal(point.Y, BinaryPrimitives.ReadSingleLittleEndian(entry.Payload.Span[10..]));
            Check.Equal(point.Z, BinaryPrimitives.ReadSingleLittleEndian(entry.Payload.Span[14..]));
        }
        Check.Equal("_VILLAGE", WorldMapCatalog.Resolve(10, 0).Route.Suffix);
        Check.Throws<ArgumentException>(() => WorldMapCatalog.Entry(10, 1));
        Check.Throws<ArgumentException>(() => WorldMapCatalog.Entry(0, 0));
        Check.Throws<ArgumentException>(() => WorldMapCatalog.Entry(10, 0, new(float.NaN, 0, 0)));
        WorldMapRoute unsampled = WorldMapCatalog.Routes.First(r => r.FilesReady && r.SamplePosition is null);
        Check.Throws<ArgumentException>(() => WorldMapCatalog.Entry(unsampled.MapId, unsampled.Submap));
        ServerOptions alternate = ServerOptions.FromJson("""
            {"profile":"character_lobby","character_store":"unused.local.json","initial_map_id":19,"initial_submap":0}
            """);
        alternate.Validate();
        Check.Equal(19, alternate.InitialMapId);
        Check.Throws<ArgumentException>(() => (alternate with { InitialSubmap = 255 }).Validate());
        Check.Throws<ArgumentException>(() => ServerOptions.FromJson("""{"initial_position":[1,2]}"""));
    }
}
