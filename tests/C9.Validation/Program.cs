using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using C9.Protocol;
using C9.Server;

return await Validation.RunAsync(args);

internal static class Check
{
    public static long Assertions;
    public static void True(bool value, string message = "Assertion failed")
    {
        Interlocked.Increment(ref Assertions);
        if (!value) throw new InvalidOperationException(message);
    }
    public static void Equal<T>(T expected, T actual) => True(EqualityComparer<T>.Default.Equals(expected, actual), $"Expected {expected}, got {actual}");
    public static void Bytes(ReadOnlySpan<byte> expected, ReadOnlySpan<byte> actual) => True(expected.SequenceEqual(actual), "Byte sequence differs");
    public static T Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T error) { Interlocked.Increment(ref Assertions); return error; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}");
    }
    public static async Task<T> ThrowsAsync<T>(Func<Task> action) where T : Exception
    {
        try { await action(); } catch (T error) { Interlocked.Increment(ref Assertions); return error; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}");
    }
}

internal sealed class CollectedEvents : IServerEvents
{
    public ConcurrentQueue<ServerEvent> Values { get; } = new();
    public void Emit(ServerEvent value) => Values.Enqueue(value);
}

internal static partial class Validation
{
    public static JsonDocument Fixture(string name)
    {
        using Stream stream = typeof(Validation).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException("Missing embedded fixture " + name);
        return JsonDocument.Parse(stream);
    }

    public static ServerOptions LabOptions()
    {
        using JsonDocument fixture = Fixture("lab-fixture.json");
        return ServerOptions.FromJson(fixture.RootElement.GetRawText()) with { Port = 0, TimeoutSeconds = 2 };
    }

    public static async Task<int> RunAsync(string[] args)
    {
        bool offline = args.Length == 1 && args[0] == "--offline";
        if (args.Length > 0 && !offline)
        {
            if (args.Length == 1 && args[0] == "--stdio-fixture")
            {
                PipeReader input = PipeReader.Create(Console.OpenStandardInput());
                PipeWriter output = PipeWriter.Create(Console.OpenStandardOutput());
                try { await new SessionProcessor(LabOptions(), new StderrEvents()).RunAsync(1, input, output); }
                finally { await input.CompleteAsync(); await output.CompleteAsync(); }
                return 0;
            }
            if (args.Length == 3 && args[0] == "--client")
            {
                await InteropClientAsync(args[1], int.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture));
                Console.WriteLine("{\"event\":\"interop_client_passed\"}");
                return 0;
            }
            Console.Error.WriteLine("Usage: C9.Validation [--offline | --client LOOPBACK PORT | --stdio-fixture]");
            return 2;
        }
        (string Name, Func<Task> Run)[] cases = [
            ("RIPEMD160 independent known answers", Sync(RipemdKnownAnswers)),
            ("RIPEMD160 OpenSSL padding boundaries", Sync(RipemdReference)),
            ("RC6 independent known answers", Sync(Rc6KnownAnswers)),
            ("Signed checksum vectors", Sync(ChecksumVectors)),
            ("All 80 twisters match Python vectors", Sync(TwisterVectors)),
            ("Wire bytes and state tables match Python", Sync(WireVectors)),
            ("Codec error handling and atomic state", Sync(CodecErrors)),
            ("Initial epoch boundary", Sync(EpochBoundary)),
            ("Opaque server list schema", Sync(ServerListSchema)),
            ("Configuration validation", Sync(Configuration)),
            ("World map catalog and destinations", Sync(WorldMapDestinations)),
            ("Launcher installed version validation", Sync(LauncherVersion)),
            ("Official Client local launch arguments", Sync(LocalLauncherArguments)),
            ("Concurrent metadata file logging", Sync(MetadataFileLogging)),
            ("Pipe fragmentation and coalescing", PipeBoundaries),
            ("Pipe EOF and invalid lengths", PipeErrors),
            ("Pipe read timeout", PipeTimeout),
            ("Memory session encrypted fixture", MemoryFixture),
            ("Memory official-build plain login experiment", MemoryOfficialLoginFixture),
            ("Memory official-build fixture rejects wrong size and state", MemoryOfficialLoginInvalid),
            ("Stream pipe adapters encrypted fixture", StreamAdapterFixture),
            ("Memory session coalescing and reset", MemoryReconnect),
            ("Memory shared processor concurrency", MemoryConcurrent),
            ("Memory session rejects invalid requests", MemoryInvalid),
            ("Memory session truncated frame", MemoryTruncated),
            ("Memory session timeout", MemoryTimeout),
            ("Memory session packet limit", MemoryPacketLimit),
            ("Memory session cancellation", MemoryCancellation),
            ("Memory probe stays passive", MemoryProbe),
            ("Memory output backpressure deadline", MemoryBackpressure),
            ("TCP encrypted fixture flow", TcpFixture),
            ("TCP repeated server-list refresh", TcpServerListRefresh),
            ("TCP synthetic server-selection handoff", TcpHandoff),
            ("TCP greeting observer accepts interleaved packets without replies", TcpObserver),
            ("TCP legacy character creation persists across server restarts", () => TcpCharacterLobby(42, 3)),
            ("TCP extended lobby supports configured character capacity", () => TcpCharacterLobby(47, 9)),
            ("TCP coalescing and reconnect isolation", TcpReconnect),
            ("TCP rejects wrong session state", TcpWrongState),
            ("TCP rejects oversized prefix", TcpOversize),
            ("TCP truncated frame", TcpTruncated),
            ("TCP timeout", TcpTimeout),
            ("TCP connection limit", TcpConnectionLimit),
            ("TCP packet limit", TcpPacketLimit),
            ("TCP graceful shutdown during partial frame", TcpShutdown),
            ("TCP probe profile stays passive", TcpProbe),
            ("TCP launcher update HTTP contract", LauncherHttp)
        ];
        var selected = offline ? cases.Where(c => !c.Name.StartsWith("TCP ", StringComparison.Ordinal)).ToArray() : cases;
        int passed = 0;
        foreach ((string name, Func<Task> test) in selected)
        {
            try
            {
                await test();
                passed++;
                Console.WriteLine(JsonSerializer.Serialize(new { test = name, status = "passed" }));
            }
            catch (Exception error)
            {
                Console.Error.WriteLine($"FAILED {name}: {error}");
                Console.WriteLine(JsonSerializer.Serialize(new { passed, total = selected.Length, skipped = cases.Length - selected.Length, assertions = Check.Assertions, status = "failed" }));
                return 1;
            }
        }
        Console.WriteLine(JsonSerializer.Serialize(new { passed, total = selected.Length, skipped = cases.Length - selected.Length, assertions = Check.Assertions, mode = offline ? "offline" : "full", status = "passed" }));
        return 0;
    }

    private static Func<Task> Sync(Action action) => () => { action(); return Task.CompletedTask; };

    private static void RipemdKnownAnswers()
    {
        (string Message, string Digest)[] vectors = [
            ("", "9c1185a5c5e9fc54612808977ee8f548b2258d31"),
            ("a", "0bdc9d2d256b3ee9daae347be6f4dc835a467ffe"),
            ("abc", "8eb208f7e05d987a9b044a8e98c6b087f15a0bfc"),
            ("message digest", "5d0689ef49d2fae572b881b123a85ffa21595f36"),
            ("abcdefghijklmnopqrstuvwxyz", "f71c27109c692c1b56bbdceb5b9d2865b3708dbc"),
            ("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", "12a053384a9c0c88e405a06c27dcf49ada62eb2b"),
            ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "b0e20b6e3116640286ed3a87a5713079b21f5189"),
            (string.Concat(Enumerable.Repeat("1234567890", 8)), "9b752e45573d4b39f4dbd3323cab82bf63326bfb"),
            (new string('a', 1_000_000), "52783243c1697bdbe16d37f97f68f08325dc1528")
        ];
        foreach ((string message, string digest) in vectors)
            Check.Bytes(Convert.FromHexString(digest), LegacyRipemd160.Hash(Encoding.ASCII.GetBytes(message)));
    }

    private static void RipemdReference()
    {
        using JsonDocument source = Fixture("python_port_vectors.json");
        foreach (JsonElement vector in source.RootElement.GetProperty("ripemd_openssl").EnumerateArray())
            Check.Bytes(Convert.FromHexString(vector.GetProperty("digest_hex").GetString()!),
                LegacyRipemd160.Hash(Convert.FromHexString(vector.GetProperty("input_hex").GetString()!)));
    }

    private static void Rc6KnownAnswers()
    {
        using JsonDocument source = Fixture("rc6_kats.json");
        foreach (JsonElement vector in source.RootElement.GetProperty("vectors").EnumerateArray())
        {
            Rc6Cipher cipher = new(Convert.FromHexString(vector.GetProperty("key_hex").GetString()!));
            byte[] clear = Convert.FromHexString(vector.GetProperty("plaintext_hex").GetString()!);
            byte[] encrypted = Convert.FromHexString(vector.GetProperty("ciphertext_hex").GetString()!);
            byte[] output = new byte[16];
            cipher.EncryptBlock(clear, output);
            Check.Bytes(encrypted, output);
            cipher.DecryptBlock(encrypted, output);
            Check.Bytes(clear, output);
        }
    }

    private static void ChecksumVectors()
    {
        (string Data, uint Expected)[] literals = [("", 0xBD136D48), ("00", 0x1E6422B8),
            ("7f", 0x1E6422C7), ("80", 0xE19BDD38), ("ff", 0xE19BDD47), ("007f80ff", 0x8EDB4D48)];
        foreach ((string data, uint expected) in literals) Check.Equal(expected, Checksum.Compute(Convert.FromHexString(data)));
        using JsonDocument source = Fixture("python_port_vectors.json");
        foreach (JsonElement vector in source.RootElement.GetProperty("checksums").EnumerateArray())
            Check.Equal(vector.GetProperty("checksum").GetUInt32(), Checksum.Compute(Convert.FromHexString(vector.GetProperty("input_hex").GetString()!)));
        byte[] frame = new Packet(0x2EE1, [128, 255]).ToDecodedBytes();
        uint checksum = Checksum.ComputeDecoded(frame);
        BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(5), 0xFFFFFFFF);
        Check.Equal(checksum, Checksum.ComputeDecoded(frame));
    }

    private static void TwisterVectors()
    {
        using JsonDocument source = Fixture("python_port_vectors.json");
        int count = 0;
        foreach (JsonElement vector in source.RootElement.GetProperty("twisters").EnumerateArray())
        {
            uint selector = vector.GetProperty("selector").GetUInt32();
            byte[] original = Convert.FromHexString(vector.GetProperty("input_hex").GetString()!);
            byte[] data = (byte[])original.Clone();
            PacketTwister.Apply(data, selector);
            Check.Bytes(Convert.FromHexString(vector.GetProperty("encoded_hex").GetString()!), data);
            PacketTwister.Apply(data, selector, decode: true);
            Check.Bytes(original, data);
            count++;
        }
        Check.Equal(960, count);
        byte[] literal = new byte[4];
        PacketTwister.Apply(literal, 0);
        Check.Bytes(Convert.FromHexString("ef000000"), literal);
    }

    private static void WireVectors()
    {
        using JsonDocument source = Fixture("python_port_vectors.json");
        ConnectionCodec sender = new(), receiver = new(discoverReceiveSelector: true);
        int count = 0;
        foreach (JsonElement vector in source.RootElement.GetProperty("packets").EnumerateArray())
        {
            Packet packet = new(vector.GetProperty("type").GetUInt16(),
                Convert.FromHexString(vector.GetProperty("payload_hex").GetString()!), vector.GetProperty("flag").GetByte());
            byte[] wire = sender.Encode(packet, vector.GetProperty("selector_adjustment").GetUInt32());
            Check.Bytes(Convert.FromHexString(vector.GetProperty("wire_hex").GetString()!), wire);
            Check.Bytes(Convert.FromHexString(vector.GetProperty("decoded_hex").GetString()!), receiver.Decode(wire).ToDecodedBytes());
            count++;
        }
        Check.Equal(64, count);
        Check.Bytes(Convert.FromHexString(source.RootElement.GetProperty("send_state_table_hex").GetString()!), sender.SendCipher.GetStateTable());
        Check.Bytes(Convert.FromHexString(source.RootElement.GetProperty("receive_state_table_hex").GetString()!), receiver.ReceiveCipher.GetStateTable());
    }

    private static void CodecErrors()
    {
        ConnectionCodec constrained = new(maxFrame: 20);
        Check.Throws<ProtocolException>(() => constrained.Encode(new Packet(0x2EE6, new byte[52])));
        Check.Equal(0u, constrained.SendSequence);
        Check.Equal(0, constrained.SendCipher.FramesProcessed);
        ConnectionCodec sender = new(), receiver = new();
        byte[] plain = sender.Encode(new Packet(4, [0], flag: 0));
        receiver.Decode(plain);
        Check.Equal(0u, receiver.ReceiveSequence);
        Check.Equal(0, sender.SendCipher.FramesProcessed);
        byte[] first = sender.Encode(new Packet(0x2EE5)), second = sender.Encode(new Packet(0x2EE5));
        Check.Equal(ProtocolError.UnexpectedSequence, Check.Throws<ProtocolException>(() => receiver.Decode(second)).Code);
        Check.Equal(0u, receiver.ReceiveSequence);
        receiver.Decode(first);
        byte[] corrupted = (byte[])second.Clone(); corrupted[4] ^= 128;
        Check.Throws<ProtocolException>(() => receiver.Decode(corrupted));
        Check.Equal(1u, receiver.ReceiveSequence);
        Check.Equal(1, receiver.ReceiveCipher.FramesProcessed);
        receiver.Decode(second);
        Check.Equal(2u, receiver.ReceiveSequence);
        byte[] invalid = new ConnectionCodec().Encode(new Packet(0x2EE5)); invalid[^1] = 16;
        Check.Throws<ProtocolException>(() => new ConnectionCodec().Decode(invalid));
        Check.Throws<ProtocolException>(() => Packet.FromDecoded(plain.AsSpan(0, plain.Length - 1)));
    }

    private static void EpochBoundary()
    {
        ConnectionCodec sender = new(), receiver = new();
        for (uint i = 0; i < 256; i++) Check.Equal(i, receiver.Decode(sender.Encode(new Packet(0x2EE5))).Sequence);
        Check.Throws<UnsupportedCipherEpochException>(() => sender.Encode(new Packet(0x2EE5)));
        Check.Throws<UnsupportedCipherEpochException>(() => receiver.Decode(new ConnectionCodec().Encode(new Packet(0x2EE5))));
    }

    private static void ServerListSchema()
    {
        byte[] entry = new byte[52]; entry[0] = 3; BinaryPrimitives.WriteUInt32LittleEndian(entry.AsSpan(1), 17);
        Packet packet = ServerList.Build([entry]);
        var (entries, trailing) = ServerList.Parse(packet);
        Check.Equal(1, entries.Length); Check.Bytes(entry, entries[0]); Check.Equal(0, trailing.Length);
        Check.Equal(17u, ServerList.LookupKey(entries[0]));
        Check.Throws<ProtocolException>(() => ServerList.Build([new byte[51]]));
        Check.Throws<ProtocolException>(() => ServerList.Build(Enumerable.Range(0, 256).Select(_ => new byte[52]).ToArray()));
        Check.Throws<ProtocolException>(() => ServerList.Parse(new Packet(0x2EE6, [2])));
    }

    private static void Configuration()
    {
        LabOptions().Validate();
        Check.Throws<ArgumentException>(() => new ServerOptions { Host = "0.0.0.0" }.Validate());
        Check.Throws<ArgumentException>(() => new ServerOptions { MaxFrame = 19 }.Validate());
        Check.Throws<ArgumentException>(() => (LabOptions() with { Profile = "probe" }).Validate());
        Check.Throws<ArgumentException>(() => ServerOptions.FromJson("{\"unexpected\":1}"));
        Check.Throws<ArgumentException>(() => ServerOptions.FromJson("{\"port\":1,\"port\":2}"));
        Check.Throws<ArgumentException>(() => ServerOptions.FromJson("{\"host\":null}"));
        ServerOptions observer = new() { Profile = "observer", Greetings = [new Packet(4, [1], flag: 0)] };
        observer.Validate();
        foreach (IReadOnlyList<Packet> greetings in new IReadOnlyList<Packet>[]
                 { [], [new Packet(4, [2], flag: 0)], [new Packet(4, [1])],
                   [new Packet(0x2EE2, [1], flag: 0)], [new Packet(4, [], flag: 0)],
                   [new Packet(4, [0], flag: 0), new Packet(4, [1], flag: 0)] })
            Check.Throws<ArgumentException>(() => (observer with { Greetings = greetings }).Validate());
        Check.Throws<ArgumentException>(() => (observer with { Rules = LabOptions().Rules }).Validate());
    }

    private static async Task PipeBoundaries()
    {
        ConnectionCodec codec = new();
        byte[] first = codec.Encode(new Packet(0x2EE5)), second = codec.Encode(new Packet(0x2EE1, new byte[52]));
        byte[] stream = [.. first, .. second];
        for (int boundary = 0; boundary <= stream.Length; boundary++)
        {
            Pipe pipe = new();
            Task<byte[]?> read = LengthPrefixedPipe.ReadFrameAsync(pipe.Reader, 8192, TimeSpan.FromSeconds(2)).AsTask();
            await pipe.Writer.WriteAsync(stream.AsMemory(0, boundary));
            await pipe.Writer.WriteAsync(stream.AsMemory(boundary));
            await pipe.Writer.CompleteAsync();
            byte[]? actualFirst = await read;
            byte[]? actualSecond = await LengthPrefixedPipe.ReadFrameAsync(pipe.Reader, 8192, TimeSpan.FromSeconds(2));
            Check.Bytes(first, actualFirst!);
            Check.Bytes(second, actualSecond!);
            Check.True(await LengthPrefixedPipe.ReadFrameAsync(pipe.Reader, 8192, TimeSpan.FromSeconds(2)) is null);
            await pipe.Reader.CompleteAsync();
        }
    }

    private static async Task PipeErrors()
    {
        foreach (int length in new[] { 0, 12, 8193 })
        {
            Pipe pipe = new(); byte[] prefix = new byte[2]; BinaryPrimitives.WriteUInt16LittleEndian(prefix, (ushort)length);
            await pipe.Writer.WriteAsync(prefix); await pipe.Writer.CompleteAsync();
            Check.Equal(ProtocolError.InvalidLength, (await Check.ThrowsAsync<ProtocolException>(async () =>
                await LengthPrefixedPipe.ReadFrameAsync(pipe.Reader, 8192, TimeSpan.FromSeconds(1)))).Code);
            await pipe.Reader.CompleteAsync();
        }
        Pipe incomplete = new(); await incomplete.Writer.WriteAsync(new byte[] { 13 }); await incomplete.Writer.CompleteAsync();
        Check.Equal(ProtocolError.IncompleteFrame, (await Check.ThrowsAsync<ProtocolException>(async () =>
            await LengthPrefixedPipe.ReadFrameAsync(incomplete.Reader, 8192, TimeSpan.FromSeconds(1)))).Code);
        await incomplete.Reader.CompleteAsync();
    }

    private static async Task PipeTimeout()
    {
        Pipe pipe = new();
        await Check.ThrowsAsync<TimeoutException>(async () => await LengthPrefixedPipe.ReadFrameAsync(pipe.Reader, 8192, TimeSpan.FromMilliseconds(25)));
        await pipe.Writer.CompleteAsync(); await pipe.Reader.CompleteAsync();
    }

    public static async Task<byte[]> ReadWireAsync(NetworkStream stream)
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(3));
        byte[] prefix = new byte[2]; await stream.ReadExactlyAsync(prefix, timeout.Token);
        int size = BinaryPrimitives.ReadUInt16LittleEndian(prefix);
        Check.True(size is >= 13 and <= 8192);
        byte[] frame = new byte[size]; prefix.CopyTo(frame, 0);
        await stream.ReadExactlyAsync(frame.AsMemory(2), timeout.Token);
        return frame;
    }

    private static async Task<(TcpClient Client, ConnectionCodec Codec)> ConnectAsync(TcpGameServer server, byte greetingMode = 0)
    {
        TcpClient client = new(); await client.ConnectAsync(IPAddress.Loopback, server.Port);
        ConnectionCodec codec = new();
        Packet greeting = codec.Decode(await ReadWireAsync(client.GetStream()));
        Check.Equal((ushort)4, greeting.Type); Check.Bytes(new byte[] { greetingMode }, greeting.Payload.Span);
        return (client, codec);
    }

    private static Task ClosedAsync(TcpClient client) => ClosedAsync(client.GetStream());

    private static async Task ClosedAsync(NetworkStream stream)
    {
        try { Check.Equal(0, await stream.ReadAsync(new byte[1]).AsTask().WaitAsync(TimeSpan.FromSeconds(3))); }
        catch (IOException) { Check.True(true); }
    }

    private static async Task WithServer(Func<TcpGameServer, CollectedEvents, Task> action, ServerOptions? config = null)
    {
        CollectedEvents events = new(); await using TcpGameServer server = new(config ?? LabOptions(), events);
        await server.StartAsync(); await action(server, events); await server.StopAsync().WaitAsync(TimeSpan.FromSeconds(3));
        Check.Equal(0, server.ActiveSessions);
    }

    private static Task TcpFixture() => WithServer(async (server, events) =>
    {
        var (client, codec) = await ConnectAsync(server); using (client)
        {
            NetworkStream stream = client.GetStream(); byte[] payload = new byte[2162];
            Encoding.ASCII.GetBytes("PRIVATE-FIXTURE-DO-NOT-LOG").CopyTo(payload, 0);
            byte[] wire = codec.Encode(new Packet(0x2EE1, payload));
            foreach (ReadOnlyMemory<byte> part in new[] { wire.AsMemory(0, 1), wire.AsMemory(1, 6), wire.AsMemory(7, 1017), wire.AsMemory(1024) })
                await stream.WriteAsync(part);
            Check.Equal((ushort)0x2EE2, codec.Decode(await ReadWireAsync(stream)).Type);
            await stream.WriteAsync(codec.Encode(new Packet(0x2EE5)));
            var (entries, trailing) = ServerList.Parse(codec.Decode(await ReadWireAsync(stream)));
            Check.Equal(1, entries.Length); Check.Equal(1u, ServerList.LookupKey(entries[0])); Check.Equal(0, trailing.Length);
            Check.True(!JsonSerializer.Serialize(events.Values).Contains("PRIVATE-FIXTURE-DO-NOT-LOG", StringComparison.Ordinal));
        }
    });

    private static Task TcpServerListRefresh() => WithServer(async (server, events) =>
    {
        var (client, codec) = await ConnectAsync(server);
        using (client)
        {
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(codec.Encode(new Packet(0x2EE1, new byte[2162])));
            Check.Equal((ushort)0x2EE2, codec.Decode(await ReadWireAsync(stream)).Type);
            for (uint refresh = 0; refresh < 3; refresh++)
            {
                await stream.WriteAsync(codec.Encode(new Packet(0x2EE5)));
                Packet reply = codec.Decode(await ReadWireAsync(stream));
                Check.Equal(refresh + 1, reply.Sequence);
                var (entries, trailing) = ServerList.Parse(reply);
                Check.Equal(1, entries.Length);
                Check.Equal(1u, ServerList.LookupKey(entries[0]));
                // These are explicitly synthetic fields read by the selected
                // Client: UTF-16 name at +5, selection gate byte at +0x33.
                Check.Bytes(Encoding.Unicode.GetBytes("C9 Local\0"), entries[0].AsSpan(5, 18));
                Check.Equal((byte)1, entries[0][51]);
                Check.Equal(0, trailing.Length);
            }
            Check.Equal(3, events.Values.Count(e => e.Event == "received" && e.Type == 0x2EE5));
            Check.True(!events.Values.Any(e => e.Event == "session_error"));
        }
    });

    private static async Task TcpHandoff()
    {
        await WithServer(async (server, events) =>
        {
            var (client, codec) = await ConnectAsync(server);
            using (client)
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(codec.Encode(new Packet(0x2EE1, new byte[2162])));
                Check.Equal((ushort)0x2EE2, codec.Decode(await ReadWireAsync(stream)).Type);
                await stream.WriteAsync(codec.Encode(new Packet(0x2EE5)));
                Check.Equal((ushort)0x2EE6, codec.Decode(await ReadWireAsync(stream)).Type);
                byte[] selection = new byte[20];
                BinaryPrimitives.WriteUInt32LittleEndian(selection.AsSpan(8), 1);
                await stream.WriteAsync(codec.Encode(new Packet(0x2EEC, selection)));
                Packet handoff = codec.Decode(await ReadWireAsync(stream));
                Check.Equal((ushort)0x2EED, handoff.Type);
                Check.Equal(2u, handoff.Sequence);
                ReadOnlySpan<byte> payload = handoff.Payload.Span;
                Check.Equal(27, payload.Length);
                Check.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(payload[8..]));
                Check.Bytes(IPAddress.Loopback.GetAddressBytes(), payload.Slice(13, 4));
                Check.Equal((ushort)14001, BinaryPrimitives.ReadUInt16LittleEndian(payload[17..]));
                Check.Bytes(IPAddress.Loopback.GetAddressBytes(), payload.Slice(19, 4));
                Check.Equal((ushort)14001, BinaryPrimitives.ReadUInt16LittleEndian(payload[23..]));
                Check.True(!events.Values.Any(e => e.Event == "session_error"));
            }
        });
    }

    private static async Task TcpObserver()
    {
        using JsonDocument fixture = Fixture("lab-handoff-probe.json");
        ServerOptions observer = ServerOptions.FromJson(fixture.RootElement.GetRawText())
            with { Port = 0, TimeoutSeconds = 2 };
        await WithServer(async (server, events) =>
        {
            async Task WaitForReceived(int expected)
            {
                for (int i = 0; i < 100 && events.Values.Count(e => e.Event == "received") < expected; i++)
                    await Task.Delay(10);
                Check.Equal(expected, events.Values.Count(e => e.Event == "received"));
            }
            var (client, codec) = await ConnectAsync(server, greetingMode: 1);
            using (client)
            {
                byte[] marker = Encoding.ASCII.GetBytes("PRIVATE-OBSERVER-DO-NOT-LOG");
                Packet[] requests = [new(0x1771, new byte[49]), new(9999, new byte[8]),
                    new(0x2EE5), new(9901, new byte[24]), new(0x7777, marker)];
                byte[] combined = requests.SelectMany(packet => codec.Encode(packet)).ToArray();
                await client.GetStream().WriteAsync(combined);
                await WaitForReceived(requests.Length);
                await client.GetStream().WriteAsync(codec.Encode(new Packet(9999, new byte[8])));
                await WaitForReceived(6);
                ServerEvent[] received = events.Values.Where(e => e.Event == "received").ToArray();
                for (int i = 0; i < received.Length; i++)
                {
                    Check.Equal((uint)i, received[i].Sequence);
                    Check.Equal("observing", received[i].State);
                }
                Check.True(!events.Values.Any(e => e.Event == "session_error"));
                Check.True(!JsonSerializer.Serialize(events.Values).Contains("PRIVATE-OBSERVER-DO-NOT-LOG", StringComparison.Ordinal));
            }
            var (nextClient, nextCodec) = await ConnectAsync(server, greetingMode: 1);
            using (nextClient)
            {
                await nextClient.GetStream().WriteAsync(nextCodec.Encode(new Packet(0x2EE5)));
                await WaitForReceived(7);
                Check.Equal(0u, events.Values.Last(e => e.Event == "received").Sequence);
                byte[] corrupt = nextCodec.Encode(new Packet(9999, new byte[8]));
                corrupt[^1] = 16; // Invalid outer padding footer must still close an observer.
                await nextClient.GetStream().WriteAsync(corrupt);
                await ClosedAsync(nextClient);
            }
            Check.Equal(1, events.Values.Count(e => e.Event == "session_error"));
            Check.Equal(2, events.Values.Count(e => e.Event == "sent" && e.Type == 4));
            Check.True(!events.Values.Any(e => e.Event == "sent" && e.Type != 4));
        }, observer);
    }

    private static Task TcpReconnect() => WithServer(async (server, _) =>
    {
        for (int i = 0; i < 2; i++)
        {
            var (client, codec) = await ConnectAsync(server); using (client)
            {
                byte[] login = codec.Encode(new Packet(0x2EE1, new byte[2162]));
                byte[] listing = codec.Encode(new Packet(0x2EE5));
                byte[] combined = [.. login, .. listing];
                await client.GetStream().WriteAsync(combined);
                Check.Equal(0u, codec.Decode(await ReadWireAsync(client.GetStream())).Sequence);
                Check.Equal(1u, codec.Decode(await ReadWireAsync(client.GetStream())).Sequence);
            }
        }
    });

    private static Task TcpWrongState() => WithServer(async (server, events) =>
    {
        var (client, codec) = await ConnectAsync(server); using (client)
        { await client.GetStream().WriteAsync(codec.Encode(new Packet(0x2EE5))); await ClosedAsync(client); }
        Check.True(events.Values.Any(e => e.Event == "session_error"));
    });

    private static Task TcpOversize() => WithServer(async (server, _) =>
    {
        var (client, _) = await ConnectAsync(server); using (client)
        { await client.GetStream().WriteAsync(new byte[] { 1, 32 }); await ClosedAsync(client); }
    });

    private static Task TcpTruncated() => WithServer(async (server, events) =>
    {
        var (client, _) = await ConnectAsync(server); using (client)
        {
            // Windows marks TcpClient disconnected after a send half-close;
            // retain the established stream to verify the server's receive EOF.
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(new byte[] { 100, 0, 1 });
            client.Client.Shutdown(SocketShutdown.Send); await ClosedAsync(stream);
        }
        Check.True(events.Values.Any(e => e.Event == "truncated_frame"));
    });

    private static Task TcpTimeout() => WithServer(async (server, events) =>
    {
        var (client, _) = await ConnectAsync(server); using (client) { await ClosedAsync(client); }
        Check.True(events.Values.Any(e => e.Reason == "TimeoutException"));
    }, LabOptions() with { TimeoutSeconds = 0.15 });

    private static Task TcpConnectionLimit() => WithServer(async (server, events) =>
    {
        var (first, _) = await ConnectAsync(server); using (first)
        {
            using TcpClient second = new(); await second.ConnectAsync(IPAddress.Loopback, server.Port); await ClosedAsync(second);
            Check.True(events.Values.Any(e => e.Event == "connection_limit"));
        }
    }, LabOptions() with { MaxConnections = 1 });

    private static Task TcpPacketLimit() => WithServer(async (server, events) =>
    {
        var (client, codec) = await ConnectAsync(server); using (client)
        {
            await client.GetStream().WriteAsync(codec.Encode(new Packet(0x2EE1, new byte[2162])));
            Check.Equal((ushort)0x2EE2, codec.Decode(await ReadWireAsync(client.GetStream())).Type);
            await ClosedAsync(client); Check.True(events.Values.Any(e => e.Event == "packet_limit"));
        }
    }, LabOptions() with { MaxPackets = 1 });

    private static Task TcpShutdown() => WithServer(async (server, _) =>
    {
        var (client, _) = await ConnectAsync(server); using (client)
        {
            await client.GetStream().WriteAsync(new byte[] { 100, 0, 1 });
            await server.StopAsync().WaitAsync(TimeSpan.FromSeconds(2));
            Check.Equal(0, server.ActiveSessions);
        }
    });

    private static Task TcpProbe() => WithServer(async (server, events) =>
    {
        using TcpClient client = new(); await client.ConnectAsync(IPAddress.Loopback, server.Port);
        await client.GetStream().WriteAsync(new ConnectionCodec().Encode(new Packet(0x2EE5)));
        using CancellationTokenSource deadline = new(TimeSpan.FromMilliseconds(50));
        await Check.ThrowsAsync<OperationCanceledException>(async () => await client.GetStream().ReadExactlyAsync(new byte[1], deadline.Token));
        Check.True(events.Values.Any(e => e.Event == "received"));
        Check.True(!events.Values.Any(e => e.Event == "sent"));
    }, new ServerOptions { Port = 0 });

    private static async Task InteropClientAsync(string address, int port)
    {
        if (!IPAddress.TryParse(address, out IPAddress? ip) || !IPAddress.IsLoopback(ip)) throw new ArgumentException("Interop target must be loopback");
        using TcpClient client = new(); await client.ConnectAsync(ip, port);
        NetworkStream stream = client.GetStream(); ConnectionCodec codec = new();
        Check.Equal((ushort)4, codec.Decode(await ReadWireAsync(stream)).Type);
        byte[] request = codec.Encode(new Packet(0x2EE1, new byte[2162]));
        byte[] list = codec.Encode(new Packet(0x2EE5));
        await stream.WriteAsync(request.AsMemory(0, 1));
        await stream.WriteAsync(request.AsMemory(1));
        await stream.WriteAsync(list);
        Check.Equal((ushort)0x2EE2, codec.Decode(await ReadWireAsync(stream)).Type);
        var (entries, trailing) = ServerList.Parse(codec.Decode(await ReadWireAsync(stream)));
        Check.Equal(1, entries.Length); Check.Equal(1u, ServerList.LookupKey(entries[0])); Check.Equal(0, trailing.Length);
    }
}
