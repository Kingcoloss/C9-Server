using System.Buffers.Binary;

namespace C9.Protocol;

public sealed class Packet
{
    public const int HeaderSize = 13;
    private readonly byte[] payload;
    public ushort Type { get; }
    public byte Flag { get; }
    public uint Sequence { get; }
    public uint Checksum { get; }
    public ReadOnlyMemory<byte> Payload => payload;

    public Packet(ushort type, ReadOnlySpan<byte> payload = default, byte flag = 1,
        uint sequence = 0, uint checksum = 0)
    {
        if (payload.Length > ushort.MaxValue - HeaderSize)
            throw new ProtocolException(ProtocolError.InvalidLength, "Packet payload exceeds header length capacity");
        Type = type;
        this.payload = payload.ToArray();
        Flag = flag;
        Sequence = sequence;
        Checksum = checksum;
    }

    public byte[] ToDecodedBytes()
    {
        byte[] frame = new byte[HeaderSize + payload.Length];
        BinaryPrimitives.WriteUInt16LittleEndian(frame, (ushort)frame.Length);
        frame[2] = Flag;
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(3), Type);
        BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(5), Checksum);
        BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(9), Sequence);
        payload.CopyTo(frame.AsSpan(HeaderSize));
        return frame;
    }

    public static Packet FromDecoded(ReadOnlySpan<byte> frame)
    {
        if (frame.Length < HeaderSize || BinaryPrimitives.ReadUInt16LittleEndian(frame) != frame.Length)
            throw new ProtocolException(ProtocolError.InvalidLength, "Decoded frame length mismatch");
        return new Packet(BinaryPrimitives.ReadUInt16LittleEndian(frame[3..]), frame[HeaderSize..],
            frame[2], BinaryPrimitives.ReadUInt32LittleEndian(frame[9..]),
            BinaryPrimitives.ReadUInt32LittleEndian(frame[5..]));
    }
}
