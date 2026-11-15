using System.Buffers.Binary;
using System.Net;
using C9.Contracts;

namespace C9.Server;

/// <summary>
/// Writes a session's frames as a classic libpcap file (LINKTYPE_RAW, IPv4) that Wireshark opens.
/// <list type="bullet">
/// <item><c>Wire</c>: one TCP stream with synthetic handshake, the exact socket bytes, and FIN.
/// Redacted frames keep their length but are zero-filled.</item>
/// <item><c>Decoded</c>: one UDP datagram per frame (port <see cref="DecodedPort"/>) holding the
/// decoded C9 frame (13-byte header + plaintext payload) for the C9 Wireshark dissector.</item>
/// </list>
/// Addresses are synthetic: the client is 10.99.0.2 and the server 10.99.0.1.
/// </summary>
public static class PcapExport
{
    public const int DecodedPort = 49099;
    private const uint LinkTypeRaw = 101;
    private static readonly IPAddress ClientAddress = IPAddress.Parse("10.99.0.2");
    private static readonly IPAddress ServerAddress = IPAddress.Parse("10.99.0.1");

    public static byte[] Wire(SessionInfo? session, IReadOnlyList<TraceFrame> frames)
    {
        using MemoryStream output = new();
        WriteGlobalHeader(output);
        if (frames.Count == 0) return output.ToArray();
        ushort serverPort = (ushort)(session?.LocalPort is > 0 and <= 65535 ? session.LocalPort : 14000);
        ushort clientPort = ClientPort(session);
        const uint clientIsn = 1000, serverIsn = 5000;
        DateTimeOffset start = session?.ConnectedUtc ?? frames[0].TimeUtc;
        long clientBytes = frames.Where(f => f.Direction == TraceDirections.Inbound).Select(f => f.StreamOffset + f.WireLength).DefaultIfEmpty(0).Max();
        long serverBytes = frames.Where(f => f.Direction == TraceDirections.Outbound).Select(f => f.StreamOffset + f.WireLength).DefaultIfEmpty(0).Max();
        void Segment(DateTimeOffset time, bool fromClient, uint seq, uint ack, byte flags, ReadOnlySpan<byte> data)
        {
            byte[] packet = Tcp(fromClient ? ClientAddress : ServerAddress, fromClient ? ServerAddress : ClientAddress,
                fromClient ? clientPort : serverPort, fromClient ? serverPort : clientPort, seq, ack, flags, data);
            WriteRecord(output, time, packet);
        }
        Segment(start, true, clientIsn, 0, 0x02, default);                      // SYN
        Segment(start, false, serverIsn, clientIsn + 1, 0x12, default);         // SYN, ACK
        Segment(start, true, clientIsn + 1, serverIsn + 1, 0x10, default);      // ACK
        long clientNext = 0, serverNext = 0;
        foreach (TraceFrame frame in frames.OrderBy(f => f.Id))
        {
            bool fromClient = frame.Direction == TraceDirections.Inbound;
            byte[] data = frame.Wire is { } wire ? Convert.FromBase64String(wire) : [];
            if (data.Length < frame.WireLength) Array.Resize(ref data, frame.WireLength); // Redacted: zero-filled.
            uint seq = (uint)((fromClient ? clientIsn : serverIsn) + 1 + frame.StreamOffset);
            uint ack = (uint)((fromClient ? serverIsn : clientIsn) + 1 + (fromClient ? serverNext : clientNext));
            Segment(frame.TimeUtc, fromClient, seq, ack, 0x18, data);           // PSH, ACK
            if (fromClient) clientNext = Math.Max(clientNext, frame.StreamOffset + data.Length);
            else serverNext = Math.Max(serverNext, frame.StreamOffset + data.Length);
        }
        DateTimeOffset end = session?.ClosedUtc ?? frames[^1].TimeUtc;
        if (session?.ClosedUtc is not null)
        {
            Segment(end, false, (uint)(serverIsn + 1 + serverBytes), (uint)(clientIsn + 1 + clientBytes), 0x11, default); // FIN, ACK
            Segment(end, true, (uint)(clientIsn + 1 + clientBytes), (uint)(serverIsn + 2 + serverBytes), 0x11, default);
        }
        return output.ToArray();
    }

    public static byte[] Decoded(SessionInfo? session, IReadOnlyList<TraceFrame> frames)
    {
        using MemoryStream output = new();
        WriteGlobalHeader(output);
        ushort clientPort = ClientPort(session);
        foreach (TraceFrame frame in frames.OrderBy(f => f.Id))
        {
            bool fromClient = frame.Direction == TraceDirections.Inbound;
            byte[] payload = frame.Payload is { } text ? Convert.FromBase64String(text) : [];
            // C9 decoded header: u16 length, u8 flag, u16 type, u32 checksum, u32 sequence, then payload.
            // When the frame did not decode or was redacted, the header carries 0xFFFF length as a marker.
            byte[] body = new byte[13 + payload.Length];
            bool decoded = frame.Payload is not null && frame.Error is null;
            BinaryPrimitives.WriteUInt16LittleEndian(body, decoded ? (ushort)Math.Min(body.Length, ushort.MaxValue) : ushort.MaxValue);
            body[2] = frame.Flag ?? 0;
            BinaryPrimitives.WriteUInt16LittleEndian(body.AsSpan(3), frame.Type ?? 0);
            BinaryPrimitives.WriteUInt32LittleEndian(body.AsSpan(5), frame.Checksum ?? 0);
            BinaryPrimitives.WriteUInt32LittleEndian(body.AsSpan(9), frame.Sequence ?? 0);
            payload.CopyTo(body, 13);
            byte[] packet = Udp(fromClient ? ClientAddress : ServerAddress, fromClient ? ServerAddress : ClientAddress,
                fromClient ? clientPort : (ushort)DecodedPort, fromClient ? (ushort)DecodedPort : clientPort, body);
            WriteRecord(output, frame.TimeUtc, packet);
        }
        return output.ToArray();
    }

    private static ushort ClientPort(SessionInfo? session) =>
        session?.RemoteEndpoint is { } remote && IPEndPoint.TryParse(remote, out IPEndPoint? endpoint) && endpoint.Port > 0
            ? (ushort)endpoint.Port : (ushort)(40000 + (session?.Number ?? 0) % 20000);

    private static void WriteGlobalHeader(Stream output)
    {
        Span<byte> header = stackalloc byte[24];
        BinaryPrimitives.WriteUInt32LittleEndian(header, 0xA1B23C4D);           // Nanosecond-resolution pcap.
        BinaryPrimitives.WriteUInt16LittleEndian(header[4..], 2);
        BinaryPrimitives.WriteUInt16LittleEndian(header[6..], 4);
        BinaryPrimitives.WriteUInt32LittleEndian(header[16..], 262_144);
        BinaryPrimitives.WriteUInt32LittleEndian(header[20..], LinkTypeRaw);
        output.Write(header);
    }

    private static void WriteRecord(Stream output, DateTimeOffset time, byte[] packet)
    {
        Span<byte> header = stackalloc byte[16];
        long ticks = time.UtcTicks - DateTimeOffset.UnixEpoch.UtcTicks;
        BinaryPrimitives.WriteUInt32LittleEndian(header, (uint)(ticks / TimeSpan.TicksPerSecond));
        BinaryPrimitives.WriteUInt32LittleEndian(header[4..], (uint)(ticks % TimeSpan.TicksPerSecond * 100));
        BinaryPrimitives.WriteUInt32LittleEndian(header[8..], (uint)packet.Length);
        BinaryPrimitives.WriteUInt32LittleEndian(header[12..], (uint)packet.Length);
        output.Write(header);
        output.Write(packet);
    }

    private static byte[] Tcp(IPAddress source, IPAddress destination, ushort sourcePort, ushort destinationPort,
        uint seq, uint ack, byte flags, ReadOnlySpan<byte> data)
    {
        byte[] packet = new byte[20 + 20 + data.Length];
        WriteIpv4(packet, 6, source, destination);
        Span<byte> tcp = packet.AsSpan(20);
        BinaryPrimitives.WriteUInt16BigEndian(tcp, sourcePort);
        BinaryPrimitives.WriteUInt16BigEndian(tcp[2..], destinationPort);
        BinaryPrimitives.WriteUInt32BigEndian(tcp[4..], seq);
        BinaryPrimitives.WriteUInt32BigEndian(tcp[8..], ack);
        tcp[12] = 5 << 4;
        tcp[13] = flags;
        BinaryPrimitives.WriteUInt16BigEndian(tcp[14..], 65535);
        data.CopyTo(tcp[20..]);
        BinaryPrimitives.WriteUInt16BigEndian(tcp[16..], TransportChecksum(packet, 6));
        return packet;
    }

    private static byte[] Udp(IPAddress source, IPAddress destination, ushort sourcePort, ushort destinationPort, ReadOnlySpan<byte> data)
    {
        byte[] packet = new byte[20 + 8 + data.Length];
        WriteIpv4(packet, 17, source, destination);
        Span<byte> udp = packet.AsSpan(20);
        BinaryPrimitives.WriteUInt16BigEndian(udp, sourcePort);
        BinaryPrimitives.WriteUInt16BigEndian(udp[2..], destinationPort);
        BinaryPrimitives.WriteUInt16BigEndian(udp[4..], (ushort)(8 + data.Length));
        data.CopyTo(udp[8..]);
        ushort checksum = TransportChecksum(packet, 17);
        BinaryPrimitives.WriteUInt16BigEndian(udp[6..], checksum == 0 ? (ushort)0xFFFF : checksum);
        return packet;
    }

    private static void WriteIpv4(byte[] packet, byte protocol, IPAddress source, IPAddress destination)
    {
        Span<byte> ip = packet;
        ip[0] = 0x45;
        BinaryPrimitives.WriteUInt16BigEndian(ip[2..], (ushort)Math.Min(packet.Length, ushort.MaxValue));
        ip[8] = 64;
        ip[9] = protocol;
        source.GetAddressBytes().CopyTo(ip[12..]);
        destination.GetAddressBytes().CopyTo(ip[16..]);
        BinaryPrimitives.WriteUInt16BigEndian(ip[10..], Fold(Sum(ip[..20], 0)));
    }

    private static ushort TransportChecksum(byte[] packet, byte protocol)
    {
        int length = packet.Length - 20;
        Span<byte> pseudo = stackalloc byte[12];
        packet.AsSpan(12, 8).CopyTo(pseudo);
        pseudo[9] = protocol;
        BinaryPrimitives.WriteUInt16BigEndian(pseudo[10..], (ushort)length);
        return Fold(Sum(packet.AsSpan(20), Sum(pseudo, 0)));
    }

    private static uint Sum(ReadOnlySpan<byte> data, uint sum)
    {
        int i = 0;
        for (; i + 1 < data.Length; i += 2) sum += BinaryPrimitives.ReadUInt16BigEndian(data[i..]);
        if (i < data.Length) sum += (uint)(data[i] << 8);
        return sum;
    }

    private static ushort Fold(uint sum)
    {
        while (sum >> 16 != 0) sum = (sum & 0xFFFF) + (sum >> 16);
        return (ushort)~sum;
    }
}
