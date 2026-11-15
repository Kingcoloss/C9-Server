using System.Buffers.Binary;
using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using C9.Contracts;
using C9.Protocol;

namespace C9.Server;

// The same session runtime is used by TCP and socket-free validation.
public sealed class SessionProcessor(ServerOptions options, IServerEvents events)
{
    private static readonly Meter Metrics = new("C9.Server", "0.1.0");
    private static readonly Counter<long> Received = Metrics.CreateCounter<long>("c9.packets.received");
    private readonly CharacterLobby? characterLobby = options.Profile == "character_lobby"
        ? new CharacterLobby(options, events) : null;

    public Task RunAsync(long id, PipeReader reader, PipeWriter writer, CancellationToken cancellationToken = default) =>
        RunAsync(SessionContext.Legacy(id), reader, writer, cancellationToken);

    public async Task RunAsync(SessionContext session, PipeReader reader, PipeWriter writer, CancellationToken cancellationToken = default)
    {
        ConnectionCodec codec = new(options.MaxFrame, discoverReceiveSelector: true);
        string state = options.Profile == "observer" ? "observing" : "connected";
        ulong selectedCharacterId = 0;
        bool worldVitalsSent = false;
        long inboundOffset = 0, outboundOffset = 0;
        string? closeReason = null;
        Dictionary<(string, ushort), ResponseRule> rules = options.Rules.ToDictionary(r => (r.State, r.RequestType));
        ServerEvent Event(string name) => new(name) { Session = session.Number, SessionKey = session.Keyed ? session.Key : null };
        void ChangeState(string value, ulong? characterId = null, int? mapId = null, int? submap = null, long? accountId = null)
        {
            state = value;
            session.SetState(value, characterId, mapId, submap, accountId);
            events.Emit(Event("state") with { State = value, CharacterId = characterId, MapId = mapId });
        }
        void Fail(string reason)
        {
            closeReason = reason;
            session.RecordError(reason);
            events.Emit(Event("session_error") with { Reason = reason });
        }

        session.InitState(state);
        session.Observer?.Opened(session);
        events.Emit(Event("connected") with { Remote = session.Keyed ? session.Remote : null });
        try
        {
            async ValueTask SendAsync(Packet packet)
            {
                byte[] wire = codec.Encode(packet);
                session.Trace?.Frame(session, TraceDirections.Outbound, outboundOffset, wire, packet, state, null,
                    codec.SendCipher.FramesProcessed, null);
                outboundOffset += wire.Length;
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
                session.RecordPacket(TraceDirections.Outbound, packet.Type, wire.Length);
                events.Emit(Event("sent") with { Type = packet.Type, WireLength = wire.Length });
            }

            if (characterLobby is not null) await SendAsync(new Packet(4, new byte[] { 1 }, 0));
            foreach (Packet greeting in options.Greetings) await SendAsync(greeting);
            for (int count = 0; count < options.MaxPackets; count++)
            {
                byte[]? frame;
                try
                {
                    frame = await LengthPrefixedPipe.ReadFrameAsync(reader, options.MaxFrame,
                        TimeSpan.FromSeconds(options.TimeoutSeconds), cancellationToken);
                }
                catch (ProtocolException error) when (error.Data[LengthPrefixedPipe.PreviewKey] is byte[] preview)
                {
                    session.Trace?.Frame(session, TraceDirections.Inbound, inboundOffset, preview, null, state,
                        $"{error.Code}: {error.Message}", codec.ReceiveCipher.FramesProcessed, null);
                    throw;
                }
                if (frame is null)
                {
                    closeReason = "peer_closed";
                    events.Emit(Event("peer_closed"));
                    return;
                }
                Packet packet;
                try { packet = codec.Decode(frame); }
                catch (ProtocolException error)
                {
                    session.Trace?.Frame(session, TraceDirections.Inbound, inboundOffset, frame, null, state,
                        $"{error.Code}: {error.Message}", codec.ReceiveCipher.FramesProcessed, null);
                    throw;
                }
                session.Trace?.Frame(session, TraceDirections.Inbound, inboundOffset, frame, packet, state, null,
                    codec.ReceiveCipher.FramesProcessed, packet.Flag == 0 ? null : codec.LastReceiveSelector);
                inboundOffset += frame.Length;
                session.RecordPacket(TraceDirections.Inbound, packet.Type, frame.Length);
                Received.Add(1);
                events.Emit(Event("received") with
                {
                    Type = packet.Type, PayloadLength = packet.Payload.Length,
                    Flag = packet.Flag, Sequence = packet.Sequence, State = state
                });
                if (options.Profile is "probe" or "observer") continue;
                if (state == "connected" && !options.LocalAccountEnabled)
                    throw new ProtocolException(ProtocolError.InvalidPacket, "Local account is disabled");
                if (characterLobby is not null)
                {
                    if (state == "connected" && packet.Type == 0x1771 && packet.Payload.Length == 49)
                    {
                        await SendAsync(await characterLobby.ListAsync(cancellationToken));
                        ChangeState("character_lobby", accountId: characterLobby.AccountId);
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0BBB)
                    {
                        await SendAsync(await characterLobby.CreateAsync(packet.Payload.Span, cancellationToken));
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0EFB)
                    {
                        await SendAsync(await characterLobby.DetailsAsync(packet.Payload.Span, cancellationToken));
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0BBF)
                    {
                        await SendAsync(await characterLobby.SelectAsync(packet.Payload.Span, cancellationToken));
                        selectedCharacterId = BinaryPrimitives.ReadUInt64LittleEndian(packet.Payload.Span);
                        if (options.WorldVitals == WorldVitalsLayout.Legacy)
                        {
                            await SendAsync(characterLobby.InitialAttributes());
                            await SendAsync(characterLobby.InitialVitals());
                        }
                        await SendAsync(WorldMapCatalog.Entry(options.InitialMapId, options.InitialSubmap, options.InitialPosition));
                        ChangeState("world_loading", selectedCharacterId, options.InitialMapId, options.InitialSubmap);
                    }
                    else if (state == "world_loading" && options.WorldVitals == WorldVitalsLayout.Extended &&
                        packet.Type == 0x0BC5 && packet.Payload.IsEmpty && !worldVitalsSent)
                    {
                        await SendAsync(characterLobby.ExtendedAttributes());
                        await SendAsync(characterLobby.ExtendedVitals(selectedCharacterId));
                        worldVitalsSent = true;
                        events.Emit(Event("world_vitals_sent") with { State = state });
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
                ChangeState(rule.NextState);
            }
            closeReason = "packet_limit";
            events.Emit(Event("packet_limit"));
        }
        catch (ProtocolException error)
        {
            if (error.Code == ProtocolError.IncompleteFrame)
            {
                closeReason = "truncated_frame";
                session.RecordError("truncated_frame");
                events.Emit(Event("truncated_frame"));
            }
            else Fail(error.Message);
        }
        catch (TimeoutException) { Fail("TimeoutException"); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { closeReason = "server_stopped"; }
        catch (IOException) { Fail("ConnectionError"); }
        catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested) { closeReason = "server_stopped"; }
        finally
        {
            events.Emit(Event("disconnected"));
            session.Close(closeReason);
        }
    }
}
