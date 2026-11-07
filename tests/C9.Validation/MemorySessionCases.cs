using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using C9.Protocol;
using C9.Server;

internal sealed class StderrEvents : IServerEvents
{
    public void Emit(ServerEvent value) => Console.Error.WriteLine(JsonSerializer.Serialize(value, JsonServerEvents.Options));
}

internal static partial class Validation
{
    private sealed class MemorySession : IAsyncDisposable
    {
        private readonly Pipe input = new();
        private readonly Pipe output;
        public CollectedEvents Events { get; }
        public ConnectionCodec Client { get; } = new();
        public CancellationTokenSource Lifetime { get; } = new();
        public Task Completion { get; }
        private bool inputCompleted;

        public MemorySession(ServerOptions? config = null, SessionProcessor? shared = null,
            CollectedEvents? events = null, long id = 1, bool backpressure = false)
        {
            ServerOptions options = config ?? LabOptions();
            options.Validate();
            output = backpressure ? new Pipe(new PipeOptions(pauseWriterThreshold: 1, resumeWriterThreshold: 1)) : new Pipe();
            Events = events ?? new CollectedEvents();
            Completion = RunAsync(shared ?? new SessionProcessor(options, Events), id);
        }

        private async Task RunAsync(SessionProcessor processor, long id)
        {
            try { await processor.RunAsync(id, input.Reader, output.Writer, Lifetime.Token); }
            finally { await output.Writer.CompleteAsync(); }
        }

        public async Task SendBytesAsync(ReadOnlyMemory<byte> bytes) => _ = await input.Writer.WriteAsync(bytes);
        public Task SendAsync(Packet packet) => SendBytesAsync(Client.Encode(packet));

        public async Task<Packet> ReceiveAsync()
        {
            byte[]? wire = await LengthPrefixedPipe.ReadFrameAsync(output.Reader, 8192, TimeSpan.FromSeconds(2));
            Check.True(wire is not null, "Expected a reply frame");
            return Client.Decode(wire!);
        }

        public async Task HelloAsync()
        {
            Packet packet = await ReceiveAsync();
            Check.Equal((ushort)4, packet.Type);
            Check.Bytes(new byte[] { 0 }, packet.Payload.Span);
        }

        public async Task CompleteInputAsync()
        {
            if (inputCompleted) return;
            inputCompleted = true;
            await input.Writer.CompleteAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Lifetime.CancelAsync();
            await Completion.WaitAsync(TimeSpan.FromSeconds(2));
            await CompleteInputAsync();
            await input.Reader.CompleteAsync();
            await output.Reader.CompleteAsync();
            Lifetime.Dispose();
        }
    }

    private static async Task MemoryFixture()
    {
        await using MemorySession session = new(); await session.HelloAsync();
        byte[] payload = new byte[2162]; Encoding.ASCII.GetBytes("PRIVATE-FIXTURE-DO-NOT-LOG").CopyTo(payload, 0);
        byte[] wire = session.Client.Encode(new Packet(0x2EE1, payload));
        await session.SendBytesAsync(wire.AsMemory(0, 1));
        await session.SendBytesAsync(wire.AsMemory(1, 6));
        await session.SendBytesAsync(wire.AsMemory(7));
        Check.Equal((ushort)0x2EE2, (await session.ReceiveAsync()).Type);
        await session.SendAsync(new Packet(0x2EE5));
        Packet list = await session.ReceiveAsync(); var (entries, tail) = ServerList.Parse(list);
        Check.Equal(1, entries.Length); Check.Equal(1u, ServerList.LookupKey(entries[0])); Check.Equal(0, tail.Length);
        Check.True(!JsonSerializer.Serialize(session.Events.Values).Contains("PRIVATE-FIXTURE-DO-NOT-LOG", StringComparison.Ordinal));
    }

    private static async Task StreamAdapterFixture()
    {
        ConnectionCodec client = new();
        byte[] login = client.Encode(new Packet(0x2EE1, new byte[2162]));
        byte[] list = client.Encode(new Packet(0x2EE5));
        using MemoryStream inputStream = new([.. login, .. list]);
        using MemoryStream outputStream = new();
        PipeReader input = PipeReader.Create(inputStream, new StreamPipeReaderOptions(bufferSize: 4096, minimumReadSize: 512, leaveOpen: true));
        PipeWriter output = PipeWriter.Create(outputStream, new StreamPipeWriterOptions(leaveOpen: true));
        try { await new SessionProcessor(LabOptions(), new CollectedEvents()).RunAsync(1, input, output); }
        finally { await input.CompleteAsync(); await output.CompleteAsync(); }
        byte[] bytes = outputStream.ToArray();
        List<Packet> packets = [];
        for (int offset = 0; offset < bytes.Length;)
        {
            int size = System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(offset));
            packets.Add(client.Decode(bytes.AsSpan(offset, size)));
            offset += size;
        }
        Check.Equal(3, packets.Count);
        Check.Equal((ushort)4, packets[0].Type);
        Check.Equal((ushort)0x2EE2, packets[1].Type);
        Check.Equal(1, ServerList.Parse(packets[2]).Entries.Length);
    }

    private static ServerOptions OfficialLoginOptions()
    {
        using JsonDocument fixture = Fixture("lab-official-login-fixture.json");
        return ServerOptions.FromJson(fixture.RootElement.GetRawText()) with { Port = 0, TimeoutSeconds = 2 };
    }

    // Validates our proposed exchange only. The latest game has not yet accepted
    // this response; its 2F04 request type/size is the sole observed new layout.
    private static async Task MemoryOfficialLoginFixture()
    {
        await using MemorySession session = new(OfficialLoginOptions());
        await session.HelloAsync();
        byte[] payload = new byte[1092];
        Encoding.ASCII.GetBytes("PRIVATE-LOCAL-IDENTITY").CopyTo(payload, 0);
        byte[] wire = session.Client.Encode(new Packet(0x2F04, payload, flag: 0));
        await session.SendBytesAsync(wire.AsMemory(0, 7));
        await session.SendBytesAsync(wire.AsMemory(7));
        Packet login = await session.ReceiveAsync();
        Check.Equal((ushort)0x2EE2, login.Type);
        Check.Equal((byte)0, login.Flag);
        Check.Bytes(Convert.FromHexString("0100000000000000000000000000000001000000"), login.Payload.Span);
        for (int refresh = 0; refresh < 3; refresh++)
        {
            await session.SendAsync(new Packet(0x2EE5, flag: 0));
            Packet list = await session.ReceiveAsync();
            Check.Equal((byte)0, list.Flag);
            var (entries, tail) = ServerList.Parse(list);
            Check.Equal(1, entries.Length);
            Check.Bytes(Encoding.Unicode.GetBytes("C9 Local\0"), entries[0].AsSpan(5, 18));
            Check.Equal(0, tail.Length);
        }
        Check.True(!session.Events.Values.Any(e => e.Event == "session_error"));
        Check.True(!JsonSerializer.Serialize(session.Events.Values).Contains("PRIVATE-LOCAL-IDENTITY", StringComparison.Ordinal));
    }

    private static async Task MemoryOfficialLoginInvalid()
    {
        foreach (Packet invalid in new[] { new Packet(0x2F04, new byte[1091], flag: 0),
                     new Packet(0x2EE1, new byte[2162], flag: 0), new Packet(0x2EE5, flag: 0) })
        {
            await using MemorySession session = new(OfficialLoginOptions());
            await session.HelloAsync();
            await session.SendAsync(invalid);
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.Equal(1, session.Events.Values.Count(e => e.Event == "session_error"));
            Check.Equal(1, session.Events.Values.Count(e => e.Event == "sent"));
        }
    }

    private static async Task MemoryReconnect()
    {
        for (int i = 0; i < 2; i++)
        {
            await using MemorySession session = new(); await session.HelloAsync();
            byte[] login = session.Client.Encode(new Packet(0x2EE1, new byte[2162]));
            byte[] list = session.Client.Encode(new Packet(0x2EE5));
            byte[] both = [.. login, .. list]; await session.SendBytesAsync(both);
            Check.Equal(0u, (await session.ReceiveAsync()).Sequence);
            Check.Equal(1u, (await session.ReceiveAsync()).Sequence);
        }
    }

    private static async Task MemoryConcurrent()
    {
        CollectedEvents events = new(); SessionProcessor processor = new(LabOptions(), events);
        await using MemorySession first = new(shared: processor, events: events, id: 1);
        await using MemorySession second = new(shared: processor, events: events, id: 2);
        await first.HelloAsync(); await second.HelloAsync();
        await Task.WhenAll(first.SendAsync(new Packet(0x2EE1, new byte[2162])), second.SendAsync(new Packet(0x2EE1, new byte[2162])));
        Check.Equal(0u, (await first.ReceiveAsync()).Sequence); Check.Equal(0u, (await second.ReceiveAsync()).Sequence);
        await first.SendAsync(new Packet(0x2EE5)); Check.Equal(1u, (await first.ReceiveAsync()).Sequence);
        Check.Equal(1, events.Values.Count(e => e.Event == "state" && e.Session == 2));
    }

    private static async Task MemoryInvalid()
    {
        foreach (Packet invalid in new[] { new Packet(0x2EE5), new Packet(0x2EE1, new byte[2161]) })
        {
            await using MemorySession session = new(); await session.HelloAsync();
            await session.SendAsync(invalid); await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.True(session.Events.Values.Any(e => e.Event == "session_error"));
            Check.Equal(1, session.Events.Values.Count(e => e.Event == "sent"));
        }
    }

    private static async Task MemoryTruncated()
    {
        await using MemorySession session = new(); await session.HelloAsync();
        await session.SendBytesAsync(new byte[] { 100, 0, 1 }); await session.CompleteInputAsync();
        await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
        Check.True(session.Events.Values.Any(e => e.Event == "truncated_frame"));
    }

    private static async Task MemoryTimeout()
    {
        await using MemorySession session = new(LabOptions() with { TimeoutSeconds = 0.1 });
        await session.HelloAsync(); await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
        Check.True(session.Events.Values.Any(e => e.Reason == "TimeoutException"));
    }

    private static async Task MemoryPacketLimit()
    {
        await using MemorySession session = new(LabOptions() with { MaxPackets = 1 }); await session.HelloAsync();
        await session.SendAsync(new Packet(0x2EE1, new byte[2162]));
        Check.Equal((ushort)0x2EE2, (await session.ReceiveAsync()).Type);
        await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
        Check.True(session.Events.Values.Any(e => e.Event == "packet_limit"));
    }

    private static async Task MemoryCancellation()
    {
        await using MemorySession session = new(); await session.HelloAsync();
        await session.SendBytesAsync(new byte[] { 100, 0, 1 });
        await session.Lifetime.CancelAsync(); await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
        Check.True(!session.Events.Values.Any(e => e.Event == "session_error"));
        Check.True(session.Events.Values.Any(e => e.Event == "disconnected"));
    }

    private static async Task MemoryProbe()
    {
        await using MemorySession session = new(new ServerOptions());
        await session.SendAsync(new Packet(0x2EE5)); await session.CompleteInputAsync();
        await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
        Check.True(session.Events.Values.Any(e => e.Event == "received"));
        Check.True(!session.Events.Values.Any(e => e.Event == "sent"));
    }

    private static async Task MemoryBackpressure()
    {
        await using MemorySession session = new(LabOptions() with { TimeoutSeconds = 0.1 }, backpressure: true);
        await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
        Check.True(session.Events.Values.Any(e => e.Reason == "TimeoutException"));
    }
}
