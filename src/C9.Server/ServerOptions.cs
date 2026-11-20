using System.Net;
using System.Text.Json;
using C9.Protocol;

namespace C9.Server;

public sealed record ResponseRule(string State, ushort RequestType, int PayloadLength,
    string NextState, IReadOnlyList<Packet> Replies);

public enum WorldVitalsLayout { Legacy, Extended }

public sealed record ServerOptions
{
    public string Host { get; init; } = "127.0.0.1";
    public int Port { get; init; } = 14000;
    public string Profile { get; init; } = "probe";
    public string? CharacterStore { get; init; }
    public int CharacterListHeaderBytes { get; init; } = CharacterLobby.LegacyListHeaderBytes;
    public int CharacterSlots { get; init; } = CharacterLobby.LegacySlotCount;
    public WorldVitalsLayout WorldVitals { get; init; }
    public bool LocalAccountEnabled { get; init; } = true;
    public int InitialHp { get; init; } = 100;
    public int InitialMp { get; init; } = 100;
    public int InitialMapId { get; init; } = 10;
    public int InitialSubmap { get; init; }
    public MapPosition? InitialPosition { get; init; }
    // JSON list of NPCs per map/sub-map sent as 0x0BE3 after world entry (see NpcSpawnCatalog); null sends none.
    public string? NpcSpawnFile { get; init; }
    public int MaxFrame { get; init; } = DefaultMaxFrame;
    // Large enough for the 33,734-byte 0x0BC0 select acknowledgement of the current client.
    public const int DefaultMaxFrame = 40960;
    // Encoded wire size of 0x0BC0: length/flag prefix, RC6-padded body, trailing checksum byte.
    internal static int SelectAckFrameBytes => 3 + (Packet.HeaderSize - 3 + CharacterLobby.SelectAckBytes + 15) / 16 * 16 + 1;
    public int MaxConnections { get; init; } = 4;
    public int MaxPackets { get; init; } = 64;
    public double TimeoutSeconds { get; init; } = 10;
    public IReadOnlyList<Packet> Greetings { get; init; } = Array.Empty<Packet>();
    public IReadOnlyList<ResponseRule> Rules { get; init; } = Array.Empty<ResponseRule>();
    /// <summary>
    /// Runtime-only (command line <c>--allow-container-bind</c>): permits the wildcard address
    /// 0.0.0.0 / :: inside a container whose published ports stay on the host loopback.
    /// Never read from JSON profiles.
    /// </summary>
    public bool AllowContainerBind { get; init; }
    /// <summary>
    /// Runtime-only (environment <c>C9_DB_CONNECTION</c>, see <see cref="WithGameDatabaseFromEnvironment"/>): when set,
    /// the character lobby keeps characters in the game database (C9.GameData) instead of <see cref="CharacterStore"/>.
    /// Holds the connection string without the password; the password is read from <c>C9_DB_PASSWORD_FILE</c>
    /// when the store opens. Never read from JSON profiles.
    /// </summary>
    public string? GameDatabase { get; init; }

    /// <summary>Applies <c>C9_DB_CONNECTION</c> to a character lobby; other profiles and an unset variable are unchanged.</summary>
    public ServerOptions WithGameDatabaseFromEnvironment() =>
        Profile == "character_lobby" && Environment.GetEnvironmentVariable(GameDatabaseVariable) is { Length: > 0 } connection
            ? this with { GameDatabase = connection }
            : this;

    /// <summary>Same variable as C9.GameData and the Portal (compose.yaml, profile "gamedata").</summary>
    public const string GameDatabaseVariable = "C9_DB_CONNECTION";

    /// <summary>
    /// Runtime-only (environment <c>C9_AUTH_URL</c> and <c>C9_AUTH_TOKEN_FILE</c>): when set, login only accepts a
    /// 0x2F04 that carries a valid L9 ticket and the lobby only serves the account c9-auth resolved for the handoff
    /// (docs/LAUNCHER_ACCOUNTS_TH.md). Never read from JSON profiles.
    /// </summary>
    public Uri? LauncherAuth { get; init; }
    /// <summary>File with the bearer token for c9-auth's internal endpoints (the shared agent token).</summary>
    public string? LauncherAuthTokenFile { get; init; }
    public const string LauncherAuthVariable = "C9_AUTH_URL";
    public const string LauncherAuthTokenVariable = "C9_AUTH_TOKEN_FILE";

    /// <summary>Applies <c>C9_AUTH_URL</c>/<c>C9_AUTH_TOKEN_FILE</c> to login and lobby profiles; unset leaves options unchanged.</summary>
    public ServerOptions WithLauncherAuthFromEnvironment()
    {
        if (Profile is not ("fixture" or "character_lobby") ||
            Environment.GetEnvironmentVariable(LauncherAuthVariable) is not { Length: > 0 } url) return this;
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? address) || address.Scheme is not ("http" or "https") ||
            !string.IsNullOrEmpty(address.UserInfo))
            throw new ArgumentException(LauncherAuthVariable + " must be an absolute http(s) URL without credentials");
        string tokenFile = Environment.GetEnvironmentVariable(LauncherAuthTokenVariable) is { Length: > 0 } file ? file
            : throw new ArgumentException(LauncherAuthTokenVariable + " is required when " + LauncherAuthVariable + " is set");
        return this with { LauncherAuth = address, LauncherAuthTokenFile = tokenFile };
    }

    public void Validate()
    {
        if (!IPAddress.TryParse(Host, out IPAddress? address) || !(IPAddress.IsLoopback(address) ||
            (AllowContainerBind && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any)))))
            throw new ArgumentException(AllowContainerBind
                ? "Container mode accepts only a loopback or wildcard (0.0.0.0 / ::) bind address"
                : "Current protocol-validation host requires a numeric loopback bind address");
        if (Port is < 0 or > 65535 || Profile is not ("probe" or "fixture" or "observer" or "character_lobby"))
            throw new ArgumentException("Invalid port or profile");
        if (MaxFrame is < 20 or > 65535 || MaxConnections is < 1 or > 32)
            throw new ArgumentException("Invalid frame or connection limit");
        if (MaxPackets is < 1 or > 255 || !double.IsFinite(TimeoutSeconds) || TimeoutSeconds <= 0 || TimeoutSeconds > 300)
            throw new ArgumentException("Invalid packet or timeout limit");
        if (Profile == "probe" && (Greetings.Count > 0 || Rules.Count > 0))
            throw new ArgumentException("Passive probe cannot send greetings or replies");
        if (Profile == "observer" && (Rules.Count != 0 || Greetings.Count != 1 ||
            Greetings[0].Type != 4 || Greetings[0].Flag != 0 ||
            Greetings[0].Payload.Length != 1 || Greetings[0].Payload.Span[0] > 1))
            throw new ArgumentException("Observer requires one plain type-4 mode-0/1 greeting and no response rules");
        if (Profile == "character_lobby" && (string.IsNullOrWhiteSpace(CharacterStore) ||
            Greetings.Count != 0 || Rules.Count != 0 || MaxFrame < SelectAckFrameBytes))
            throw new ArgumentException($"Character lobby requires a store path, a frame limit of at least {SelectAckFrameBytes} and no fixture rules or greetings");
        if (CharacterListHeaderBytes is not (CharacterLobby.LegacyListHeaderBytes or CharacterLobby.ExtendedListHeaderBytes))
            throw new ArgumentException("Unsupported character list header size");
        if (CharacterSlots is < 1 or > byte.MaxValue)
            throw new ArgumentException("Character slots must fit in the lobby capacity byte");
        if (WorldVitals is not (WorldVitalsLayout.Legacy or WorldVitalsLayout.Extended))
            throw new ArgumentException("Unknown world vitals layout");
        if (InitialHp is < 1 or > 1_000_000 || InitialMp is < 0 or > 1_000_000)
            throw new ArgumentException("Invalid starting HP or MP");
        if (!LocalAccountEnabled && Profile is not ("character_lobby" or "fixture"))
            throw new ArgumentException("Local account access requires a login or lobby profile");
        if (Profile != "character_lobby" && (CharacterStore is not null || GameDatabase is not null || CharacterListHeaderBytes != CharacterLobby.LegacyListHeaderBytes ||
            CharacterSlots != CharacterLobby.LegacySlotCount || WorldVitals != WorldVitalsLayout.Legacy ||
            InitialHp != 100 || InitialMp != 100))
            throw new ArgumentException("Character options require the character_lobby profile");
        if (Profile == "character_lobby")
        {
            int maxListBody = Packet.HeaderSize - 3 + CharacterListHeaderBytes + CharacterSlots * CharacterLobby.ListEntryBytes;
            if (3 + (maxListBody + 15) / 16 * 16 + 1 > MaxFrame)
                throw new ArgumentException("Character slot capacity can exceed the configured frame limit");
            _ = WorldMapCatalog.Resolve(InitialMapId, InitialSubmap, InitialPosition);
        }
        else if (InitialMapId != 10 || InitialSubmap != 0 || InitialPosition is not null)
            throw new ArgumentException("Initial map options require the character_lobby profile");
        if (Rules.Select(r => (r.State, r.RequestType)).Distinct().Count() != Rules.Count)
            throw new ArgumentException("Duplicate state/type rule");
        if (Greetings.Count > 3 || Rules.Any(r => r.Replies.Count > 3))
            throw new ArgumentException("At most three greeting/reply packets are supported");
        // Preserve the reference server's conservative budget for the initial epoch.
        if (Profile != "character_lobby" && 3 * MaxPackets + Greetings.Count > 255)
            throw new ArgumentException("Reply budget can exceed initial cipher epoch");
        foreach (ResponseRule rule in Rules)
        {
            if (string.IsNullOrWhiteSpace(rule.State) || string.IsNullOrWhiteSpace(rule.NextState) ||
                rule.State.Length > 128 || rule.NextState.Length > 128 ||
                rule.PayloadLength < 0 || rule.PayloadLength > MaxFrame - Packet.HeaderSize)
                throw new ArgumentException("Fixture rule outside configured bounds");
        }
        foreach (Packet packet in Greetings.Concat(Rules.SelectMany(r => r.Replies)))
        {
            int size = Packet.HeaderSize + packet.Payload.Length;
            if (packet.Flag != 0) size = 3 + (size - 3 + 15) / 16 * 16 + 1;
            if (size > MaxFrame) throw new ArgumentException("Fixture reply exceeds configured maximum");
        }
    }

    public static ServerOptions FromJson(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;
        RequireFields(root, ["description", "host", "port", "profile", "max_frame", "max_connections",
            "local_account_enabled", "initial_hp", "initial_mp",
            "max_packets", "timeout_seconds", "greetings", "rules", "character_store",
            "character_list_header_bytes", "character_slots", "world_vitals_layout", "initial_map_id", "initial_submap", "initial_position"]);
        List<Packet> greetings = [];
        if (root.TryGetProperty("greetings", out JsonElement greetingArray))
            foreach (JsonElement packet in greetingArray.EnumerateArray()) greetings.Add(ParsePacket(packet));
        List<ResponseRule> rules = [];
        if (root.TryGetProperty("rules", out JsonElement ruleArray))
        {
            foreach (JsonElement rule in ruleArray.EnumerateArray())
            {
                RequireFields(rule, ["state", "request_type", "payload_length", "next_state", "replies"]);
                List<Packet> replies = [];
                foreach (JsonElement packet in rule.GetProperty("replies").EnumerateArray()) replies.Add(ParsePacket(packet));
                rules.Add(new ResponseRule(rule.GetProperty("state").GetString()!,
                    ToType(rule.GetProperty("request_type").GetInt32()), rule.GetProperty("payload_length").GetInt32(),
                    rule.GetProperty("next_state").GetString()!, replies.AsReadOnly()));
            }
        }
        return new ServerOptions
        {
            Host = GetString(root, "host", "127.0.0.1"), Port = GetInt(root, "port", 14000),
            Profile = GetString(root, "profile", "probe"), MaxFrame = GetInt(root, "max_frame", DefaultMaxFrame),
            CharacterStore = root.TryGetProperty("character_store", out JsonElement store) ? store.GetString() : null,
            CharacterListHeaderBytes = GetInt(root, "character_list_header_bytes", CharacterLobby.LegacyListHeaderBytes),
            CharacterSlots = GetInt(root, "character_slots", CharacterLobby.LegacySlotCount),
            LocalAccountEnabled = !root.TryGetProperty("local_account_enabled", out JsonElement enabled) || enabled.GetBoolean(),
            InitialHp = GetInt(root, "initial_hp", 100), InitialMp = GetInt(root, "initial_mp", 100),
            WorldVitals = GetString(root, "world_vitals_layout", "legacy") switch
            {
                "legacy" => WorldVitalsLayout.Legacy,
                "extended" => WorldVitalsLayout.Extended,
                _ => throw new ArgumentException("Unknown world vitals layout")
            },
            InitialMapId = GetInt(root, "initial_map_id", 10), InitialSubmap = GetInt(root, "initial_submap", 0),
            InitialPosition = ParsePosition(root),
            NpcSpawnFile = root.TryGetProperty("npc_spawn_file", out JsonElement npcFile) ? npcFile.GetString() : null,
            MaxConnections = GetInt(root, "max_connections", 4), MaxPackets = GetInt(root, "max_packets", 64),
            TimeoutSeconds = root.TryGetProperty("timeout_seconds", out JsonElement t) ? t.GetDouble() : 10,
            Greetings = greetings.AsReadOnly(), Rules = rules.AsReadOnly()
        };
    }

    private static MapPosition? ParsePosition(JsonElement root)
    {
        if (!root.TryGetProperty("initial_position", out JsonElement value)) return null;
        float[] coordinates = value.EnumerateArray().Select(v => v.GetSingle()).ToArray();
        if (coordinates.Length != 3) throw new ArgumentException("initial_position requires three coordinates");
        return new(coordinates[0], coordinates[1], coordinates[2]);
    }

    private static Packet ParsePacket(JsonElement element)
    {
        RequireFields(element, ["type", "flag", "payload_hex"]);
        int flag = GetInt(element, "flag", 1);
        if (flag is < 0 or > 255) throw new ArgumentException("Packet flag out of range");
        return new Packet(ToType(element.GetProperty("type").GetInt32()),
            Convert.FromHexString(GetString(element, "payload_hex", "")), (byte)flag);
    }

    private static ushort ToType(int type) => type is >= 0 and <= 65535 ? (ushort)type : throw new ArgumentException("Packet type out of range");
    private static int GetInt(JsonElement e, string key, int fallback) => e.TryGetProperty(key, out JsonElement v) ? v.GetInt32() : fallback;
    private static string GetString(JsonElement e, string key, string fallback) => e.TryGetProperty(key, out JsonElement v) ? v.GetString() ?? throw new ArgumentException("Configuration string cannot be null") : fallback;

    private static void RequireFields(JsonElement element, string[] allowed)
    {
        if (element.ValueKind != JsonValueKind.Object) throw new ArgumentException("Configuration entry must be an object");
        HashSet<string> seen = [];
        foreach (JsonProperty property in element.EnumerateObject())
            if (!allowed.Contains(property.Name, StringComparer.Ordinal) || !seen.Add(property.Name))
                throw new ArgumentException("Unknown or duplicate configuration field");
    }
}
