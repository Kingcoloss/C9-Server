using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private static async Task TcpCharacterLobby(int listHeaderBytes)
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-characters-{Guid.NewGuid():N}.json");
        ServerOptions config = new()
        {
            Profile = "character_lobby", Port = 0, CharacterStore = store, CharacterListHeaderBytes = listHeaderBytes,
            MaxPackets = 255, TimeoutSeconds = 3
        };
        byte[] name = Encoding.Unicode.GetBytes("LabFighter");
        byte[] appearance = Enumerable.Range(0, 44).Select(i => (byte)i).ToArray();
        byte[] create = new byte[90];
        create[0] = 2;
        create[1] = 0; // Fighter template; the asset job code is 1.
        name.CopyTo(create, 3);
        appearance.CopyTo(create, 45);

        try
        {
            await WithServer(async (server, events) =>
            {
                var (client, codec) = await ConnectAsync(server, greetingMode: 1);
                using (client)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(codec.Encode(new Packet(0x1771, new byte[49])));
                    Packet list = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0EFA, list.Type);
                    Check.Equal(listHeaderBytes, list.Payload.Length);
                    Check.Equal((byte)1, list.Payload.Span[8]);
                    Check.Bytes(new byte[] { 3, 0 }, list.Payload.Span[(listHeaderBytes - 2)..]);
                    await stream.WriteAsync(codec.Encode(new Packet(9999, new byte[8])));
                    await stream.WriteAsync(codec.Encode(new Packet(0x0BBB, create)));
                    Packet ack = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0BBC, ack.Type);
                    Check.Equal(113, ack.Payload.Length);
                    Check.Equal(3ul, BinaryPrimitives.ReadUInt64LittleEndian(ack.Payload.Span));
                    Check.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(ack.Payload.Span[8..]));
                    Check.Equal((byte)2, ack.Payload.Span[14]);
                    Check.Equal((ushort)0, BinaryPrimitives.ReadUInt16LittleEndian(ack.Payload.Span[15..]));
                    Check.Bytes(name, ack.Payload.Span.Slice(17, name.Length));
                    Check.Bytes(appearance, ack.Payload.Span.Slice(61, 44));
                    byte[] saved = File.ReadAllBytes(store);
                    await stream.WriteAsync(codec.Encode(new Packet(0x0BBB, create)));
                    Packet retry = codec.Decode(await ReadWireAsync(stream));
                    Check.Bytes(ack.Payload.Span, retry.Payload.Span);
                    Check.Bytes(saved, File.ReadAllBytes(store));
                    await stream.WriteAsync(codec.Encode(new Packet(0x0BBB, create.AsSpan(0, 89))));
                    await ClosedAsync(stream);
                    Check.Bytes(saved, File.ReadAllBytes(store));
                    Check.True(events.Values.Any(e => e.Event == "session_error"));
                }
            }, config);

            await WithServer(async (server, _) =>
            {
                var (client, codec) = await ConnectAsync(server, greetingMode: 1);
                using (client)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(codec.Encode(new Packet(0x1771, new byte[49])));
                    Packet list = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0EFA, list.Type);
                    Check.Equal(listHeaderBytes + 136, list.Payload.Length);
                    Check.Bytes(new byte[] { 3, 1 }, list.Payload.Span.Slice(listHeaderBytes - 2, 2));
                    Check.Equal(3ul, BinaryPrimitives.ReadUInt64LittleEndian(list.Payload.Span[listHeaderBytes..]));
                    Check.Bytes(name, list.Payload.Span.Slice(listHeaderBytes + 8, name.Length));
                    Check.Equal(2u, BinaryPrimitives.ReadUInt32LittleEndian(list.Payload.Span[(listHeaderBytes + 0x32)..]));
                    Check.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(list.Payload.Span[(listHeaderBytes + 0x38)..]));
                    Check.Equal(ulong.MaxValue, BinaryPrimitives.ReadUInt64LittleEndian(list.Payload.Span[(listHeaderBytes + 0x3C)..]));
                    byte[] identity = new byte[8];
                    BinaryPrimitives.WriteUInt64LittleEndian(identity, 3);
                    await stream.WriteAsync(codec.Encode(new Packet(0x0EFB, identity)));
                    Packet details = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0EFC, details.Type);
                    Check.Equal(53, details.Payload.Length);
                    Check.Bytes(identity, details.Payload.Span[..8]);
                    Check.Bytes(appearance, details.Payload.Span.Slice(8, 44));
                    Check.Equal((byte)0, details.Payload.Span[52]);
                    await stream.WriteAsync(codec.Encode(new Packet(0x0BBF, identity)));
                    Packet selected = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0BC0, selected.Type);
                    Check.Equal(7444, selected.Payload.Length);
                    Check.Equal(3ul, BinaryPrimitives.ReadUInt64LittleEndian(selected.Payload.Span));
                    Check.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(selected.Payload.Span[12..]));
                    Check.Equal((ushort)1, BinaryPrimitives.ReadUInt16LittleEndian(selected.Payload.Span[16..]));
                    Check.Equal(100u, BinaryPrimitives.ReadUInt32LittleEndian(selected.Payload.Span[24..]));
                    Check.Equal(100u, BinaryPrimitives.ReadUInt32LittleEndian(selected.Payload.Span[28..]));
                    Check.Equal(0ul, BinaryPrimitives.ReadUInt64LittleEndian(selected.Payload.Span[32..]));
                    Check.Bytes(name, selected.Payload.Span.Slice(44, name.Length));
                    Check.Equal((ushort)1, BinaryPrimitives.ReadUInt16LittleEndian(selected.Payload.Span[0x6E..]));
                    Packet attributes = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0C1E, attributes.Type);
                    Check.Equal(338, attributes.Payload.Length);
                    Check.Equal(100u, BinaryPrimitives.ReadUInt32LittleEndian(attributes.Payload.Span));
                    Check.Equal(100u, BinaryPrimitives.ReadUInt32LittleEndian(attributes.Payload.Span[4..]));
                    Check.Equal((short)1000, BinaryPrimitives.ReadInt16LittleEndian(attributes.Payload.Span[0x2C..]));
                    Check.Equal((short)1000, BinaryPrimitives.ReadInt16LittleEndian(attributes.Payload.Span[0x2E..]));
                    Check.Equal((ushort)0, BinaryPrimitives.ReadUInt16LittleEndian(attributes.Payload.Span[336..]));
                    Packet vitals = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0C1C, vitals.Type);
                    Check.Equal(70, vitals.Payload.Length);
                    Check.Equal((ushort)1, BinaryPrimitives.ReadUInt16LittleEndian(vitals.Payload.Span));
                    foreach (int offset in new[] { 2, 6, 10, 14 })
                        Check.Equal(100u, BinaryPrimitives.ReadUInt32LittleEndian(vitals.Payload.Span[offset..]));
                    Packet map = codec.Decode(await ReadWireAsync(stream));
                    Check.Equal((ushort)0x0BC3, map.Type);
                    Check.Equal(46, map.Payload.Length);
                    Check.Equal(10u, BinaryPrimitives.ReadUInt32LittleEndian(map.Payload.Span));
                    Check.Equal((byte)0, map.Payload.Span[4]); // First sub-map; 1 was out of bounds for Waterford.
                    await stream.WriteAsync(codec.Encode(new Packet(0x177D))); // Unimplemented world request is observed.
                    await stream.WriteAsync(codec.Encode(new Packet(0x0EC4)));
                    Check.Equal((ushort)0x2EEB, codec.Decode(await ReadWireAsync(stream)).Type);
                    await stream.WriteAsync(codec.Encode(new Packet(0x2EEA, new byte[16])));
                    Check.Equal((ushort)0x2EEB, codec.Decode(await ReadWireAsync(stream)).Type);
                }
            }, config);
        }
        finally { File.Delete(store); }
    }
}
