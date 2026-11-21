using System.Buffers.Binary;
using System.Text;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private static async Task MemoryWorldStartup(WorldVitalsLayout layout)
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-world-start-{Guid.NewGuid():N}.json");
        try
        {
            ServerOptions config = new()
            {
                Profile = "character_lobby", CharacterStore = store, WorldVitals = layout,
                TimeoutSeconds = 3, MaxPackets = 255
            };
            await using MemorySession session = new(config);
            Packet greeting = await session.ReceiveAsync();
            Check.Equal((ushort)4, greeting.Type);
            Check.Bytes(new byte[] { 1 }, greeting.Payload.Span);
            await session.SendAsync(new Packet(0x1771, new byte[49]));
            Check.Equal((ushort)0x0EFA, (await session.ReceiveAsync()).Type);

            byte[] create = new byte[90];
            create[0] = 2;
            Encoding.Unicode.GetBytes("WorldStart").CopyTo(create, 3);
            await session.SendAsync(new Packet(0x0BBB, create));
            Packet created = await session.ReceiveAsync();
            Check.Equal((ushort)0x0BBC, created.Type);
            byte[] identity = created.Payload.Span[..8].ToArray();
            Check.Equal(3ul, BinaryPrimitives.ReadUInt64LittleEndian(identity));
            await session.SendAsync(new Packet(0x0BBF, identity));
            Check.Equal((ushort)0x0BC0, (await session.ReceiveAsync()).Type);
            if (layout == WorldVitalsLayout.Legacy)
            {
                Check.Equal((ushort)0x0C1E, (await session.ReceiveAsync()).Type);
                Check.Equal((ushort)0x0C1C, (await session.ReceiveAsync()).Type);
            }
            Check.Equal((ushort)0x0BC3, (await session.ReceiveAsync()).Type);

            // Malformed load notifications must not start the actor or consume the one-time reply.
            await session.SendAsync(new Packet(0x0BC5, new byte[] { 1 }));
            await session.SendAsync(new Packet(0x0BC5));
            if (layout == WorldVitalsLayout.Extended)
            {
                Check.Equal((ushort)0x0C1E, (await session.ReceiveAsync()).Type);
                Packet vitals = await session.ReceiveAsync();
                Check.Equal((ushort)0x0C20, vitals.Type);
                Check.Equal((byte)0, vitals.Flag);
            }
            Packet started = await session.ReceiveAsync();
            Check.Equal((ushort)0x0BC6, started.Type);
            Check.Equal((byte)0, started.Flag);
            Check.Equal(21, started.ToDecodedBytes().Length);
            Check.Bytes(identity, started.Payload.Span);

            await session.SendAsync(new Packet(0x0BC5));
            await session.SendAsync(new Packet(0x0BCE, new byte[46]));
            await session.SendAsync(new Packet(0x270F, new byte[8]));
            await session.SendAsync(new Packet(0x0EC4)); // 300 s timer: must not trigger a leave ack.
            await session.SendAsync(new Packet(0x2EEA, new byte[16]));
            // The next encrypted reply must still decode and be the leave acknowledgement for 0x2EEA.
            // This also detects extra replies to duplicate load/position/heartbeat/timer requests.
            Check.Equal((ushort)0x2EEB, (await session.ReceiveAsync()).Type);

            // In-world exit: client sends 0x17D4 and quits on the 0x17D5 ack (encrypted).
            await session.SendAsync(new Packet(0x17D4, new byte[16]));
            Packet logout = await session.ReceiveAsync();
            Check.Equal((ushort)0x17D5, logout.Type);
            Check.Equal((byte)1, logout.Flag);
            await session.CompleteInputAsync();
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.Equal(1, session.Events.Values.Count(e => e.Event == "state" && e.State == "world_ready"));
            Check.Equal(layout == WorldVitalsLayout.Extended ? 1 : 0,
                session.Events.Values.Count(e => e.Event == "world_vitals_sent"));
            Check.True(session.Events.Values.Any(e => e.Event == "received" && e.Type == 0x0BCE && e.State == "world_ready"));
            Check.True(!session.Events.Values.Any(e => e.Event == "session_error"));
        }
        finally { File.Delete(store); }
    }
}
