using System.Buffers.Binary;
using System.Text;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private static void WorldSessionReplyContracts()
    {
        DateTimeOffset bangkok = new(2026, 11, 19, 23, 40, 0, TimeSpan.FromHours(7));
        Packet time = WorldSessionPackets.Reply(new Packet(0x0EFD, flag: 0), bangkok)!;
        Check.Equal((ushort)0x0EFE, time.Type);
        Check.Equal((byte)0, time.Flag);
        Check.Equal(25, time.ToDecodedBytes().Length);
        Check.Equal(bangkok.ToUnixTimeSeconds(), BinaryPrimitives.ReadInt64LittleEndian(time.Payload.Span));
        Check.Equal(-25200, BinaryPrimitives.ReadInt32LittleEndian(time.Payload.Span[8..]));
        Check.Equal(0, BinaryPrimitives.ReadInt32LittleEndian(
            WorldSessionPackets.ServerTime(bangkok.ToUniversalTime()).Payload.Span[8..]));

        Packet keyed = WorldSessionPackets.Reply(new Packet(0xED1D, new byte[] { 0x1C, 0, 0, 0 }), bangkok)!;
        Check.Equal((ushort)0xED1E, keyed.Type);
        Check.Equal((byte)1, keyed.Flag);
        Check.Equal(22, keyed.ToDecodedBytes().Length);
        Check.Bytes(new byte[] { 0x1C, 0, 0, 0, 0, 0, 0, 0, 0 }, keyed.Payload.Span);

        Packet points = WorldSessionPackets.Reply(new Packet(0x11A1), bangkok)!;
        Check.Equal((ushort)0x11A2, points.Type);
        Check.Equal((byte)1, points.Flag);
        Check.Bytes(new byte[13], points.Payload.Span);

        Packet mail = WorldSessionPackets.Reply(new Packet(0x0EB1), bangkok)!;
        Check.Equal((ushort)0x0EB2, mail.Type);
        Check.Equal((byte)1, mail.Flag);
        Check.Bytes(new byte[4], mail.Payload.Span);

        Packet roster = WorldSessionPackets.Reply(new Packet(0x0E4B, new byte[] { 10, 0, 0, 0, 0 }), bangkok)!;
        Check.Equal((ushort)0x0E4C, roster.Type);
        Check.Equal((byte)1, roster.Flag);
        Check.Equal(20, roster.ToDecodedBytes().Length);
        Check.Bytes(new byte[] { 10, 0, 0, 0, 0, 0, 0 }, roster.Payload.Span);
        Check.Equal((byte)2, WorldSessionPackets.Reply(
            new Packet(0x0E4B, new byte[] { 7, 0, 0, 0, 2 }), bangkok)!.Payload.Span[4]);

        Packet events = WorldSessionPackets.Reply(new Packet(0xF74F), bangkok)!;
        Check.Equal((ushort)0xF750, events.Type);
        Check.Equal((byte)1, events.Flag);
        Check.Bytes(new byte[1], events.Payload.Span);

        // Wrong sizes and requests whose readers are not decoded yet get no reply.
        foreach (Packet request in new[]
        {
            new Packet(0x0EFD, new byte[1], 0), new Packet(0xED1D, new byte[3]), new Packet(0xED1D, new byte[5]),
            new Packet(0x11A1, new byte[1]), new Packet(0x0EB1, new byte[4]), new Packet(0x0E4B, new byte[4]),
            new Packet(0xF74F, new byte[1]), new Packet(0x0BC4), new Packet(0x2347, new byte[8]), new Packet(0xEEB3, flag: 0),
            new Packet(0x0BCE, new byte[46]), new Packet(0x270F, new byte[8]), new Packet(0x0BCC, new byte[36]),
        })
            Check.True(WorldSessionPackets.Reply(request, bangkok) is null);
    }

    // Replays the world-entry order of trace 20261119/d9c138-001 session 3 and checks that
    // only the decoded requests are answered, in order, and the session stays healthy.
    private static async Task MemoryWorldSessionReplies()
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-world-session-{Guid.NewGuid():N}.json");
        try
        {
            ServerOptions config = new()
            {
                Profile = "character_lobby", CharacterStore = store, WorldVitals = WorldVitalsLayout.Extended,
                TimeoutSeconds = 3, MaxPackets = 255
            };
            await using MemorySession session = new(config);
            Check.Equal((ushort)4, (await session.ReceiveAsync()).Type);
            await session.SendAsync(new Packet(0x1771, new byte[49]));
            Check.Equal((ushort)0x0EFA, (await session.ReceiveAsync()).Type);
            byte[] create = new byte[90];
            create[0] = 2;
            Encoding.Unicode.GetBytes("WorldSession").CopyTo(create, 3);
            await session.SendAsync(new Packet(0x0BBB, create));
            byte[] identity = (await session.ReceiveAsync()).Payload.Span[..8].ToArray();
            await session.SendAsync(new Packet(0x0BBF, identity));
            Check.Equal((ushort)0x0BC0, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0BC3, (await session.ReceiveAsync()).Type);

            await session.SendAsync(new Packet(0xF74F));
            foreach (byte key in new byte[] { 0x20, 0x19 })
                await session.SendAsync(new Packet(0xED1D, new byte[] { key, 0, 0, 0 }));
            await session.SendAsync(new Packet(0x0E4B, new byte[] { 10, 0, 0, 0, 0 }));
            await session.SendAsync(new Packet(0x0BC4));
            await session.SendAsync(new Packet(0x0EFD, flag: 0));
            await session.SendAsync(new Packet(0x0EB1));
            await session.SendAsync(new Packet(0x2347, identity));
            await session.SendAsync(new Packet(0xEEB3, flag: 0));
            await session.SendAsync(new Packet(0x11A1));
            await session.SendAsync(new Packet(0x0BC5));

            Check.Equal((ushort)0xF750, (await session.ReceiveAsync()).Type);
            foreach (byte key in new byte[] { 0x20, 0x19 })
            {
                Packet keyed = await session.ReceiveAsync();
                Check.Equal((ushort)0xED1E, keyed.Type);
                Check.Equal(key, keyed.Payload.Span[0]);
            }
            Packet roster = await session.ReceiveAsync();
            Check.Equal((ushort)0x0E4C, roster.Type);
            Check.Equal(10u, BinaryPrimitives.ReadUInt32LittleEndian(roster.Payload.Span));
            Packet time = await session.ReceiveAsync();
            Check.Equal((ushort)0x0EFE, time.Type);
            Check.Equal((byte)0, time.Flag);
            long seconds = BinaryPrimitives.ReadInt64LittleEndian(time.Payload.Span);
            Check.True(Math.Abs(seconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds()) < 60);
            Check.Equal((ushort)0x0EB2, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x11A2, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0C1E, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0C20, (await session.ReceiveAsync()).Type);
            Check.Equal((ushort)0x0BC6, (await session.ReceiveAsync()).Type);

            // Requests in world_ready are answered too; the next reply proves nothing extra was queued.
            await session.SendAsync(new Packet(0x0EFD, flag: 0));
            Check.Equal((ushort)0x0EFE, (await session.ReceiveAsync()).Type);
            await session.SendAsync(new Packet(0x0EC4));
            Check.Equal((ushort)0x2EEB, (await session.ReceiveAsync()).Type);
            await session.CompleteInputAsync();
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.True(!session.Events.Values.Any(e => e.Event == "session_error"));
        }
        finally { File.Delete(store); }
    }
}
