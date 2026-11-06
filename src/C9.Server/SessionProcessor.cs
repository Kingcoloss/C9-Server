using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using C9.Protocol;

namespace C9.Server;

// The same session runtime is used by TCP and socket-free validation.
public sealed class SessionProcessor(ServerOptions options, IServerEvents events)
{
    private static readonly Meter Metrics = new("C9.Server", "0.1.0");
    private static readonly Counter<long> Received = Metrics.CreateCounter<long>("c9.packets.received");
    private readonly CharacterLobby? characterLobby = options.Profile == "character_lobby"
        ? new CharacterLobby(options.CharacterStore!) : null;

    public async Task RunAsync(long id, PipeReader reader, PipeWriter writer, CancellationToken cancellationToken = default)
    {
        ConnectionCodec codec = new(options.MaxFrame, discoverReceiveSelector: true);
        string state = options.Profile == "observer" ? "observing" : "connected";
        Dictionary<(string, ushort), ResponseRule> rules = options.Rules.ToDictionary(r => (r.State, r.RequestType));
        events.Emit(new ServerEvent("connected") { Session = id });
        try
        {
            async ValueTask SendAsync(Packet packet)
            {
                byte[] wire = codec.Encode(packet);
                using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                deadline.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));
                try
                {
                    FlushResult result = await writer.WriteAsync(wire, deadline.Token);
                    if (result.IsCanceled) throw new OperationCanceledException(deadline.Token);
                    if (result.IsCompleted) throw new IOException("Peer closed output");
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                { throw new TimeoutException("Frame write deadline exceeded"); }
                events.Emit(new ServerEvent("sent") { Session = id, Type = packet.Type, WireLength = wire.Length });
            }

            if (characterLobby is not null) await SendAsync(new Packet(4, new byte[] { 1 }, 0));
            foreach (Packet greeting in options.Greetings) await SendAsync(greeting);
            for (int count = 0; count < options.MaxPackets; count++)
            {
                byte[]? frame = await LengthPrefixedPipe.ReadFrameAsync(reader, options.MaxFrame,
                    TimeSpan.FromSeconds(options.TimeoutSeconds), cancellationToken);
                if (frame is null)
                {
                    events.Emit(new ServerEvent("peer_closed") { Session = id });
                    return;
                }
                Packet packet = codec.Decode(frame);
                Received.Add(1);
                events.Emit(new ServerEvent("received")
                {
                    Session = id, Type = packet.Type, PayloadLength = packet.Payload.Length,
                    Flag = packet.Flag, Sequence = packet.Sequence, State = state
                });
                if (options.Profile is "probe" or "observer") continue;
                if (characterLobby is not null)
                {
                    if (state == "connected" && packet.Type == 0x1771 && packet.Payload.Length == 49)
                    {
                        await SendAsync(characterLobby.List());
                        state = "character_lobby";
                        events.Emit(new ServerEvent("state") { Session = id, State = state });
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0BBB)
                    {
                        await SendAsync(characterLobby.Create(packet.Payload.Span));
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0EFB)
                    {
                        await SendAsync(characterLobby.Details(packet.Payload.Span));
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0BBF)
                    {
                        await SendAsync(characterLobby.Select(packet.Payload.Span));
                        await SendAsync(WorldMapCatalog.Entry(options.InitialMapId, options.InitialSubmap, options.InitialPosition));
                        state = "world_loading";
                        events.Emit(new ServerEvent("state") { Session = id, State = state });
                    }
                    else if (state is "character_lobby" or "world_loading" &&
                        ((packet.Type == 0x0EC4 && packet.Payload.IsEmpty) ||
                         (packet.Type == 0x2EEA && packet.Payload.Length == 16)))
                    {
                        // Client 00A62D76 reconnects to login and requests 2EE8.
                        await SendAsync(new Packet(0x2EEB));
                    }
                    else if (state == "world_loading")
                    {
                        // Observe the next world requests without inventing replies.
                        // Gameplay handlers are added only after verifying their readers.
                    }
                    else if (state != "character_lobby" || (packet.Type, packet.Payload.Length) is not ((9999, 8) or (9901, 24) or (12005, 0)))
                    {
                        throw new ProtocolException(ProtocolError.InvalidPacket, "Unexpected character lobby request");
                    }
                    continue;
                }
                if (!rules.TryGetValue((state, packet.Type), out ResponseRule? rule))
                    throw new ProtocolException(ProtocolError.InvalidPacket, "No fixture rule for session state/type");
                if (packet.Payload.Length != rule.PayloadLength)
                    throw new ProtocolException(ProtocolError.InvalidLength, "Fixture request payload length mismatch");
                foreach (Packet reply in rule.Replies) await SendAsync(reply);
                state = rule.NextState;
                events.Emit(new ServerEvent("state") { Session = id, State = state });
            }
            events.Emit(new ServerEvent("packet_limit") { Session = id });
        }
        catch (ProtocolException error)
        {
            events.Emit(error.Code == ProtocolError.IncompleteFrame
                ? new ServerEvent("truncated_frame") { Session = id }
                : new ServerEvent("session_error") { Session = id, Reason = error.Message });
        }
        catch (TimeoutException) { events.Emit(new ServerEvent("session_error") { Session = id, Reason = "TimeoutException" }); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (IOException) { events.Emit(new ServerEvent("session_error") { Session = id, Reason = "ConnectionError" }); }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { }
        finally { events.Emit(new ServerEvent("disconnected") { Session = id }); }
    }
}
