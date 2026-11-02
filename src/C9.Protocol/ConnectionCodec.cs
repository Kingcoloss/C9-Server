using System.Buffers.Binary;

namespace C9.Protocol;

// One instance belongs to one connection; each direction is consumed serially.
public sealed class ConnectionCodec
{
    public int MaxFrame { get; }
    public bool DiscoverReceiveSelector { get; }
    public uint SendSequence { get; private set; }
    public uint ReceiveSequence { get; private set; }
    public int? LastReceiveSelector { get; private set; }
    public InitialCipherDirection SendCipher { get; } = new();
    public InitialCipherDirection ReceiveCipher { get; } = new();

    public ConnectionCodec(int maxFrame = 8192, bool discoverReceiveSelector = false)
    {
        if (maxFrame is < 20 or > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(maxFrame));
        MaxFrame = maxFrame;
        DiscoverReceiveSelector = discoverReceiveSelector;
        PacketTwister.EnsureLoaded();
    }

    public byte[] Encode(Packet packet, uint selectorAdjustment = 0)
    {
        if (packet.Flag == 0)
        {
            byte[] plain = packet.ToDecodedBytes();
            if (plain.Length > MaxFrame) throw new ProtocolException(ProtocolError.InvalidLength, "Frame exceeds configured maximum");
            return plain;
        }
        int bodyLength = 10 + packet.Payload.Length;
        int wireLength = 3 + (bodyLength + 15) / 16 * 16 + 1;
        if (wireLength > MaxFrame || wireLength > ushort.MaxValue)
            throw new ProtocolException(ProtocolError.InvalidLength, "Encoded frame exceeds configured maximum");
        SendCipher.CheckEpoch();
        byte[] raw = new Packet(packet.Type, packet.Payload.Span, packet.Flag, SendSequence).ToDecodedBytes();
        uint checksum = Checksum.ComputeDecoded(raw);
        BinaryPrimitives.WriteUInt32LittleEndian(raw.AsSpan(5), checksum);
        uint selector = unchecked(checksum + selectorAdjustment) % 80;
        PacketTwister.Apply(raw.AsSpan(13), selector);
        byte[] encrypted = SendCipher.Encrypt(raw.AsSpan(3));
        byte[] wire = new byte[wireLength];
        BinaryPrimitives.WriteUInt16LittleEndian(wire, (ushort)wireLength);
        wire[2] = packet.Flag;
        encrypted.CopyTo(wire.AsSpan(3));
        SendCipher.Commit(raw.AsSpan(3));
        SendSequence = unchecked(SendSequence + 1);
        return wire;
    }

    public Packet Decode(ReadOnlySpan<byte> wire)
    {
        if (wire.Length < Packet.HeaderSize || wire.Length > MaxFrame || BinaryPrimitives.ReadUInt16LittleEndian(wire) != wire.Length)
            throw new ProtocolException(ProtocolError.InvalidLength, "Wire frame length outside configured bounds");
        if (wire[2] == 0) return Packet.FromDecoded(wire);
        byte[] transformed = ReceiveCipher.Decrypt(wire[3..]);
        if (transformed.Length < 10) throw new ProtocolException(ProtocolError.InvalidLength, "Decrypted body shorter than header");
        byte[] raw = new byte[3 + transformed.Length];
        BinaryPrimitives.WriteUInt16LittleEndian(raw, (ushort)raw.Length);
        raw[2] = wire[2];
        transformed.CopyTo(raw.AsSpan(3));
        uint checksum = BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(5));
        uint sequence = BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(9));
        if (sequence != ReceiveSequence) throw new ProtocolException(ProtocolError.UnexpectedSequence, "Unexpected receive sequence");
        byte[] candidate = (byte[])raw.Clone();
        byte[]? matched = null;
        int matchedSelector = -1;
        int first = DiscoverReceiveSelector ? 0 : (int)(checksum % 80);
        int end = DiscoverReceiveSelector ? 80 : first + 1;
        for (int selector = first; selector < end; selector++)
        {
            raw.AsSpan(13).CopyTo(candidate.AsSpan(13));
            PacketTwister.Apply(candidate.AsSpan(13), (uint)selector, decode: true);
            if (Checksum.ComputeDecoded(candidate) != checksum) continue;
            if (matched is null)
            {
                matched = candidate.AsSpan(13).ToArray();
                matchedSelector = selector;
            }
            else if (!matched.AsSpan().SequenceEqual(candidate.AsSpan(13)))
                throw new ProtocolException(ProtocolError.ChecksumMismatch, "Ambiguous payload transform");
        }
        if (matched is null) throw new ProtocolException(ProtocolError.ChecksumMismatch, "Checksum mismatch");
        matched.CopyTo(raw.AsSpan(13));
        Packet packet = Packet.FromDecoded(raw);
        ReceiveCipher.Commit(transformed);
        ReceiveSequence = unchecked(ReceiveSequence + 1);
        LastReceiveSelector = matchedSelector;
        return packet;
    }
}
