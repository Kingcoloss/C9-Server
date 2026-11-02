using System.Buffers.Binary;

namespace C9.Protocol;

public static class ServerList
{
    public const ushort MessageType = 0x2EE6;
    public const int EntrySize = 52;

    public static Packet Build(IReadOnlyList<byte[]> entries)
    {
        if (entries.Count > 255 || entries.Any(e => e.Length != EntrySize))
            throw new ProtocolException(ProtocolError.InvalidPacket, "Server list requires at most 255 opaque 52-byte entries");
        byte[] payload = new byte[1 + entries.Count * EntrySize];
        payload[0] = (byte)entries.Count;
        for (int i = 0; i < entries.Count; i++) entries[i].CopyTo(payload.AsSpan(1 + i * EntrySize));
        return new Packet(MessageType, payload);
    }

    public static (byte[][] Entries, byte[] Trailing) Parse(Packet packet)
    {
        ReadOnlySpan<byte> payload = packet.Payload.Span;
        if (packet.Type != MessageType || payload.IsEmpty)
            throw new ProtocolException(ProtocolError.InvalidPacket, "Expected server list with count byte");
        int count = payload[0], required = 1 + count * EntrySize;
        if (payload.Length < required) throw new ProtocolException(ProtocolError.InvalidLength, "Server list count exceeds payload");
        byte[][] entries = new byte[count][];
        for (int i = 0; i < count; i++) entries[i] = payload.Slice(1 + i * EntrySize, EntrySize).ToArray();
        return (entries, payload[required..].ToArray());
    }

    public static uint LookupKey(ReadOnlySpan<byte> entry)
    {
        if (entry.Length != EntrySize) throw new ProtocolException(ProtocolError.InvalidLength, "Server entry must be 52 bytes");
        return BinaryPrimitives.ReadUInt32LittleEndian(entry[1..]);
    }
}
