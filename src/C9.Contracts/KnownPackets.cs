namespace C9.Contracts;

public sealed record PacketTypeInfo(ushort Type, string Name, string Direction, string Description, bool Sensitive = false)
{
    public string Hex => $"0x{Type:X4}";
}

/// <summary>
/// Packet types observed and verified in this project (fixtures, SessionProcessor and docs).
/// Names are descriptive labels chosen here, not the client's internal PS_* names.
/// <see cref="PacketTypeInfo.Sensitive"/> marks payloads that carry credentials.
/// </summary>
public static class KnownPackets
{
    public static IReadOnlyList<PacketTypeInfo> All { get; } =
    [
        new(0x0004, "Greeting", TraceDirections.Outbound, "Plain greeting; payload byte selects mode 0 (login) or 1 (lobby)"),
        new(0x0BBB, "CharacterCreateReq", TraceDirections.Inbound, "Create character: slot, template, name, 44-byte appearance"),
        new(0x0BBC, "CharacterCreateAck", TraceDirections.Outbound, "Character created"),
        new(0x0BBF, "CharacterSelectReq", TraceDirections.Inbound, "Select character by 64-bit id"),
        new(0x0BC0, "CharacterSelectAck", TraceDirections.Outbound, "Selected player block (0x1D14 bytes)"),
        new(0x0BC3, "WorldEntry", TraceDirections.Outbound, "Map id, sub-map and position"),
        new(0x0BC5, "WorldLoaded", TraceDirections.Inbound, "Client finished loading the world"),
        new(0x0BC6, "WorldActionStarted", TraceDirections.Outbound, "Plain world-load acknowledgement; 64-bit player object id"),
        new(0x0C1C, "InitialVitals", TraceDirections.Outbound, "Level, HP/MP (legacy layout)"),
        new(0x0C1E, "InitialAttributes", TraceDirections.Outbound, "Attribute blocks"),
        new(0x0C20, "ExtendedVitals", TraceDirections.Outbound, "Character id, HP/MP and maxima (build 55350 layout)"),
        new(0x0EC4, "ReturnToWorldList", TraceDirections.Inbound, "Leave lobby back to Choose World"),
        new(0x0EFA, "CharacterList", TraceDirections.Outbound, "Character lobby list"),
        new(0x0EFB, "CharacterDetailsReq", TraceDirections.Inbound, "Request appearance/equipment for one character"),
        new(0x0EFC, "CharacterDetailsAck", TraceDirections.Outbound, "Appearance and equipment"),
        new(0x1771, "LobbyEnter", TraceDirections.Inbound, "First lobby request after greeting mode 1 (49 bytes)"),
        new(0x17D4, "LogoutReq", TraceDirections.Inbound, "In-world exit/logout request (16 bytes); client waits at Transmitting to Server until acked"),
        new(0x17D5, "LogoutAck", TraceDirections.Outbound, "Logout acknowledgement; client calls PostQuitMessage(0) and closes the game"),
        new(0x26AD, "LobbyHeartbeat9901", TraceDirections.Inbound, "Periodic lobby request (24 bytes), no reply"),
        new(0x270F, "LobbyHeartbeat9999", TraceDirections.Inbound, "Periodic lobby request (8 bytes), no reply"),
        new(0x2EE1, "LoginReqLegacy", TraceDirections.Inbound, "Legacy encrypted login (2,162 bytes)", Sensitive: true),
        new(0x2EE2, "LoginAck", TraceDirections.Outbound, "Login result (account + launcher handoff key)", Sensitive: true),
        new(0x2EE5, "WorldListReq", TraceDirections.Inbound, "Request world/server list"),
        new(0x2EE6, "WorldListAck", TraceDirections.Outbound, "World/server list"),
        new(0x2EE8, "ReconnectReq", TraceDirections.Inbound, "Reconnect to login after lobby (handoff key + account)", Sensitive: true),
        new(0x2EE9, "ReconnectAck", TraceDirections.Outbound, "Reconnect accepted"),
        new(0x2EEA, "LeaveLobbyReq", TraceDirections.Inbound, "Previous button in the character list (16 bytes)"),
        new(0x2EEB, "LeaveLobbyAck", TraceDirections.Outbound, "Return to Choose World"),
        new(0x2EEC, "WorldSelectReq", TraceDirections.Inbound, "Select world (20 bytes)"),
        new(0x2EED, "WorldSelectAck", TraceDirections.Outbound, "Handoff endpoint for the lobby"),
        new(0x2EF6, "LoginFollowupReq", TraceDirections.Inbound, "Follow-up after login (4 bytes)"),
        new(0x2EF7, "LoginFollowupAck", TraceDirections.Outbound, "0 = continue without OTP, 1 = authenticator page"),
        new(0x2F04, "LoginReq", TraceDirections.Inbound, "VFUN build login (1,092 bytes, plain)", Sensitive: true),
    ];

    private static readonly Dictionary<ushort, PacketTypeInfo> lookup = All.ToDictionary(p => p.Type);

    public static PacketTypeInfo? Find(ushort type) => lookup.GetValueOrDefault(type);
    public static string NameOf(ushort type) => lookup.TryGetValue(type, out PacketTypeInfo? info) ? info.Name : $"0x{type:X4}";
    public static bool IsSensitive(ushort type) => lookup.TryGetValue(type, out PacketTypeInfo? info) && info.Sensitive;
}
