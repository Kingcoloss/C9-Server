using System.Buffers.Binary;
using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using C9.Contracts;
using C9.GameData.Services;
using C9.Protocol;

namespace C9.Server;

// The same session runtime is used by TCP and socket-free validation.
public sealed class SessionProcessor(ServerOptions options, IServerEvents events, ILauncherAuthority? authority = null)
{
    private static readonly Meter Metrics = new("C9.Server", "0.1.0");
    private static readonly Counter<long> Received = Metrics.CreateCounter<long>("c9.packets.received");
    private readonly CharacterLobby? characterLobby = options.Profile == "character_lobby"
        ? new CharacterLobby(options, events) : null;
    // Set with C9_AUTH_URL: login requires an L9 ticket and the lobby serves the account behind the handoff.
    private readonly ILauncherAuthority? launcherAuthority = authority ?? HttpLauncherAuthority.FromOptions(options);
    // Account-level secondary password: game.account_second_passwords, or memory on the lab (shared by this server's sessions).
    private readonly ISecondPasswordStore? secondPasswords = options.Profile == "character_lobby" ? SecondPasswordStores.Open(options) : null;

    private readonly NpcSpawnCatalog npcSpawns = options.Profile == "character_lobby" ? NpcSpawnCatalog.Load(options.NpcSpawnFile) : NpcSpawnCatalog.Empty;

    public Task RunAsync(long id, PipeReader reader, PipeWriter writer, CancellationToken cancellationToken = default) =>
        RunAsync(SessionContext.Legacy(id), reader, writer, cancellationToken);

    public async Task RunAsync(SessionContext session, PipeReader reader, PipeWriter writer, CancellationToken cancellationToken = default)
    {
        ConnectionCodec codec = new(options.MaxFrame, discoverReceiveSelector: true);
        string state = options.Profile == "observer" ? "observing" : "connected";
        ulong selectedCharacterId = 0;
        CharacterLobby? lobby = characterLobby;
        SecondPasswordSession? secondPassword = secondPasswords is null ? null : new SecondPasswordSession(secondPasswords);
        long? launcherAccount = null;
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
        // The secondary password belongs to the account bound at 0x1771: the lobby's game database account, else the
        // launcher account, else the single lab account 0.
        long SecondPasswordAccount() => lobby?.AccountId ?? (secondPasswords!.NeedsDatabaseAccount
            ? throw new ProtocolException(ProtocolError.InvalidPacket, "Secondary password needs a game database account")
            : launcherAccount ?? 0);
        // S01: the selected character's position, saved while it moves and once when the session leaves the world.
        WorldPositionTracker? position = null;
        async ValueTask LeaveWorldAsync(CancellationToken token)
        {
            if (position is not { } last) return;
            position = null;
            await last.TrySaveAsync(events, Event("position_saved") with { State = state }, token);
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
                if (lobby is not null)
                {
                    if (state == "connected" && packet.Type == 0x1771 && packet.Payload.Length == 49)
                    {
                        if (launcherAuthority is not null)
                        {
                            // The client echoes the 2EE2 account number and handoff key (see LauncherLoginPacket).
                            if (!LauncherLoginPacket.TryReadLobbyIdentity(packet.Payload.Span, out long account, out ulong key))
                                throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher login required: 1771 has no account handoff");
                            TicketOwner owner = await launcherAuthority.ResolveHandoffAsync(account, key, cancellationToken)
                                ?? throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher handoff rejected");
                            if (owner.AccountId != account)
                                throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher handoff belongs to another account");
                            if (options.GameDatabase is not null) lobby = new CharacterLobby(options, events, owner.AccountId);
                            launcherAccount = owner.AccountId;
                            events.Emit(Event("launcher_identity") with { State = state, Reason = $"account {owner.AccountId}" });
                        }
                        await SendAsync(await lobby.ListAsync(cancellationToken));
                        ChangeState("character_lobby", accountId: lobby.AccountId);
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0BBB)
                    {
                        await SendAsync(await lobby.CreateAsync(packet.Payload.Span, cancellationToken));
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0EFB)
                    {
                        await SendAsync(await lobby.DetailsAsync(packet.Payload.Span, cancellationToken));
                    }
                    else if (state == "character_lobby" && packet.Type == 0x0BBF)
                    {
                        SecondPasswordStatus secondPasswordStatus = secondPassword is null
                            ? SecondPasswordStatus.None : await secondPassword.StatusAsync(SecondPasswordAccount(), cancellationToken);
                        await SendAsync(await lobby.SelectAsync(packet.Payload.Span, secondPasswordStatus, secondPassword?.Certified == true,
                            cancellationToken));
                        selectedCharacterId = BinaryPrimitives.ReadUInt64LittleEndian(packet.Payload.Span);
                        if (options.WorldVitals == WorldVitalsLayout.Legacy)
                        {
                            await SendAsync(lobby.InitialAttributes());
                            await SendAsync(lobby.InitialVitals());
                        }
                        // Enter where the last world session left the character while that is the entry map (map changes are S05).
                        SavedPosition? saved = await lobby.SavedPositionAsync(selectedCharacterId, cancellationToken);
                        MapPosition spawn = WorldMapCatalog.Resolve(options.InitialMapId, options.InitialSubmap,
                            saved is { } previous && previous.MapId == options.InitialMapId && previous.Submap == options.InitialSubmap
                                ? previous.Position : options.InitialPosition).Position;
                        await SendAsync(WorldMapCatalog.Entry(options.InitialMapId, options.InitialSubmap, spawn));
                        position = new WorldPositionTracker(lobby, selectedCharacterId, options.InitialMapId, options.InitialSubmap, spawn);
                        ChangeState("world_loading", selectedCharacterId, options.InitialMapId, options.InitialSubmap);
                        // Town NPCs exist only when the server announces them (0x0BE3); the client never spawns them itself.
                        // Send them while the map loads: 0x0BC3 holds world events, and StageMapLoad (00C9A8C0) hands the
                        // held 0x16 creates to the loader thread (00C9C352) before the client sends 0x0BC4 and 0x0BC5.
                        IReadOnlyList<(ulong ObjectId, NpcSpawn Spawn)> npcs = npcSpawns.For(options.InitialMapId, options.InitialSubmap);
                        foreach (Packet npcPacket in NpcPackets.ExistPackets(npcs)) await SendAsync(npcPacket);
                        if (npcs.Count > 0) events.Emit(Event("npc_spawn") with { State = state, MapId = options.InitialMapId, Reason = $"{npcs.Count} NPC" });
                    }
                    else if (state == "world_loading" && packet.Type == 0x0BC5 && packet.Payload.IsEmpty)
                    {
                        if (options.WorldVitals == WorldVitalsLayout.Extended)
                        {
                            await SendAsync(lobby.ExtendedAttributes());
                            await SendAsync(lobby.ExtendedVitals(selectedCharacterId));
                            events.Emit(Event("world_vitals_sent") with { State = state });
                        }
                        await SendAsync(WorldStartupPackets.ActionStarted(selectedCharacterId));
                        ChangeState("world_ready");
                    }
                    else if (state is "character_lobby" or "world_loading" or "world_ready" &&
                        packet.Type == 0x2EEA && packet.Payload.Length == 16)
                    {
                        // Client 00A62D76 reconnects to login and requests 2EE8.
                        await LeaveWorldAsync(cancellationToken);
                        await SendAsync(new Packet(0x2EEB));
                    }
                    else if ((state is "world_loading" or "world_ready") && packet.Type == 0x17D4)
                    {
                        // Client 00A47CB0 calls PostQuitMessage(0) the moment it receives a
                        // 0x17D5, so the in-world exit request (0x17D4) must be acknowledged or
                        // the client waits forever at "Transmitting to Server". The client does
                        // not read the ack payload.
                        await LeaveWorldAsync(cancellationToken);
                        await SendAsync(new Packet(0x17D5));
                        events.Emit(Event("logout_ack") with { State = state });
                    }
                    else if (secondPassword is not null && (state is "character_lobby" or "world_loading" or "world_ready") &&
                        SecondPasswordPackets.IsRequest(packet))
                    {
                        // 2744 set / 2746 reset / 2748 change / 274A certify -> ack (2745/2747/2749/274B) or 0x0001 error.
                        long account = SecondPasswordAccount();
                        (Packet reply, SecondPasswordOutcome outcome, SecondPasswordStatus? status) =
                            await secondPassword.HandleAsync(account, packet, cancellationToken);
                        await SendAsync(reply);
                        // No CRC, hash or answer in the event; the account and the outcome are enough for support.
                        events.Emit(Event("second_password") with
                        {
                            State = state, Type = packet.Type,
                            Reason = $"account {account}: {outcome}" + (status is { } s ? $", fails {s.FailCount}{(s.IsLocked ? ", locked" : "")}" : "")
                        });
                    }
                    else if (state is "world_loading" or "world_ready")
                    {
                        // Observe the next world requests without inventing replies.
                        // Gameplay handlers are added only after verifying their readers.
                        // 0x0EC4 (empty) is a timer packet: client 0085DFD0 sends it every 300 s of play
                        // while connected. It is not a leave request, so it gets no 0x2EEB.
                        if (position?.Observe(packet) == true)
                            await position.TrySaveAsync(events, Event("position_saved") with { State = state }, cancellationToken);
                        if (WorldSessionPackets.Reply(packet, DateTimeOffset.Now) is { } worldReply)
                            await SendAsync(worldReply);
                    }
                    else if (state != "character_lobby" || (packet.Type, packet.Payload.Length) is not ((9999, 8) or (9901, 24) or (12005, 0) or (0x0EC4, 0)))
                    {
                        throw new ProtocolException(ProtocolError.InvalidPacket, "Unexpected character lobby request");
                    }
                    continue;
                }
                if (!rules.TryGetValue((state, packet.Type), out ResponseRule? rule))
                    throw new ProtocolException(ProtocolError.InvalidPacket, "No fixture rule for session state/type");
                if (launcherAuthority is not null && state == "connected" && packet.Type == 0x2F04)
                {
                    // Login is mandatory: the 0x2F04 must carry the one-time ticket L9 got from c9-auth.
                    if (!LauncherLoginPacket.TryParse(packet.Payload.Span, out string ticket, out long account))
                        throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher login required: 2F04 has no ticket");
                    LauncherHandoff handoff = await launcherAuthority.RedeemTicketAsync(ticket, cancellationToken)
                        ?? throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher ticket rejected (unknown, reused or expired)");
                    if (handoff.AccountId != account)
                        throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher ticket belongs to another account");
                    await SendAsync(new Packet(0x2EE2, LauncherLoginPacket.LoginReply(handoff.AccountId, handoff.Key), 0));
                    events.Emit(Event("launcher_login") with { State = state, Reason = $"account {handoff.AccountId}" });
                    ChangeState(rule.NextState, accountId: handoff.AccountId);
                    continue;
                }
                if (launcherAuthority is not null && state == "connected" && packet.Type == 0x2EE8)
                {
                    // Return to character select (lobby 0x2EEB -> login reconnect): the 0x2EE8 must carry the
                    // c9-auth handoff from this login's 0x2EE2, so the mandatory launcher login also holds here.
                    if (!LauncherLoginPacket.TryReadReturnIdentity(packet.Payload.Span, out long account, out ulong key))
                    {
                        events.Emit(Event("launcher_return_rejected") with { State = state, Reason = "no handoff" });
                        throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher login required: 2EE8 has no handoff");
                    }
                    TicketOwner? owner = await launcherAuthority.ResolveHandoffAsync(account, key, cancellationToken);
                    if (owner is null || owner.AccountId != account)
                    {
                        events.Emit(Event("launcher_return_rejected") with { State = state, Reason = $"account {account}" });
                        throw new ProtocolException(ProtocolError.InvalidPacket, "Launcher handoff rejected (unknown, expired or another account)");
                    }
                    // Replies come from the rule: 2EE9, then 2EF7 and the server list (2EE6), because the client's
                    // server-select stage does not resend 2EF6/2EE5 after a reconnect (docs/LAUNCHER_ACCOUNTS_TH.md section 8).
                    foreach (Packet reply in rule.Replies) await SendAsync(reply);
                    events.Emit(Event("launcher_return") with { State = state, Reason = $"account {account}" });
                    ChangeState(rule.NextState, accountId: account);
                    continue;
                }
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
            // Any other way out of the world (disconnect, timeout, error, server stop) keeps the last position too.
            using (CancellationTokenSource flush = new(TimeSpan.FromSeconds(5))) await LeaveWorldAsync(flush.Token);
            events.Emit(Event("disconnected"));
            session.Close(closeReason);
        }
    }
}
