using System.Buffers.Binary;
using C9.Protocol;

namespace C9.Server;

/// <summary>
/// Replies to the world-entry requests whose readers are verified in client b4580ed4
/// (image 52e0527a). Requests not listed here stay unanswered until their readers are decoded.
/// </summary>
public static class WorldSessionPackets
{
    public static Packet? Reply(Packet request, DateTimeOffset now)
    {
        // F750 reader 0090C0B0 (registered in 0090B480): +0 u8 count, +1 count x 67-byte entries.
        ReadOnlySpan<byte> payload = request.Payload.Span;
        return (request.Type, payload.Length) switch
        {
            (0x0EFD, 0) => ServerTime(now),
            (0xED1D, 4) => EmptyKeyedList(BinaryPrimitives.ReadUInt32LittleEndian(payload)),
            (0x11A1, 0) => new Packet(0x11A2, new byte[13]),
            // 0x0EB1 (Gift Box count) is answered by GiftBoxPackets with the character's real count.
            (0x0E4B, 5) => EmptyMapRoster(BinaryPrimitives.ReadUInt32LittleEndian(payload), payload[4]),
            (0xF74F, 0) => new Packet(0xF750, new byte[1]),
            _ => null,
        };
    }

    // Client 008D56E0: +0 i64 time64 seconds, +8 i32 timezone in seconds west of UTC
    // (_get_timezone); the client shifts the time when its own zone differs by >= 3600 s.
    public static Packet ServerTime(DateTimeOffset now)
    {
        byte[] payload = new byte[12];
        BinaryPrimitives.WriteInt64LittleEndian(payload, now.ToUnixTimeSeconds());
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(8), -(int)now.Offset.TotalSeconds);
        return new Packet(0x0EFE, payload, flag: 0);
    }

    // Client 008DA840: +0 u32 key (echo), +4 u8, +5 i32 count, +9 count x 72-byte entries.
    public static Packet EmptyKeyedList(uint key)
    {
        byte[] payload = new byte[9];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, key);
        return new Packet(0xED1E, payload);
    }

    // Client case 0E4C in 008EF110: +0 u32 map (echo), +4 u8 kind, +5 u16 count,
    // +7 count x 80-byte entries. Kind 2 opens a window; other kinds post UI event 0x13.
    // Echoing the requested kind is inferred, not verified in game yet.
    public static Packet EmptyMapRoster(uint map, byte kind)
    {
        byte[] payload = new byte[7];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, map);
        payload[4] = kind;
        return new Packet(0x0E4C, payload);
    }
}
