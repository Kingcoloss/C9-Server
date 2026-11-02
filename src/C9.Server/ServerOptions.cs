using System.Net;
using System.Text.Json;
using C9.Protocol;

namespace C9.Server;

public sealed record ResponseRule(string State, ushort RequestType, int PayloadLength,
    string NextState, IReadOnlyList<Packet> Replies);

public sealed record ServerOptions
{
    public string Host { get; init; } = "127.0.0.1";
    public int Port { get; init; } = 14000;
    public string Profile { get; init; } = "probe";
    public int MaxFrame { get; init; } = 8192;
    public int MaxConnections { get; init; } = 4;
    public int MaxPackets { get; init; } = 64;
    public double TimeoutSeconds { get; init; } = 10;
    public IReadOnlyList<Packet> Greetings { get; init; } = Array.Empty<Packet>();
    public IReadOnlyList<ResponseRule> Rules { get; init; } = Array.Empty<ResponseRule>();

    public void Validate()
    {
        if (!IPAddress.TryParse(Host, out IPAddress? address) || !IPAddress.IsLoopback(address))
            throw new ArgumentException("Current protocol-validation host requires a numeric loopback bind address");
        if (Port is < 0 or > 65535 || Profile is not ("probe" or "fixture"))
            throw new ArgumentException("Invalid port or profile");
        if (MaxFrame is < 20 or > 65535 || MaxConnections is < 1 or > 32)
            throw new ArgumentException("Invalid frame or connection limit");
        if (MaxPackets is < 1 or > 255 || !double.IsFinite(TimeoutSeconds) || TimeoutSeconds <= 0 || TimeoutSeconds > 300)
            throw new ArgumentException("Invalid packet or timeout limit");
        if (Profile == "probe" && (Greetings.Count > 0 || Rules.Count > 0))
            throw new ArgumentException("Response rules require the explicit fixture profile");
        if (Rules.Select(r => (r.State, r.RequestType)).Distinct().Count() != Rules.Count)
            throw new ArgumentException("Duplicate state/type rule");
        if (Greetings.Count > 3 || Rules.Any(r => r.Replies.Count > 3))
            throw new ArgumentException("At most three greeting/reply packets are supported");
        // Preserve the reference server's conservative budget for the initial epoch.
        if (3 * MaxPackets + Greetings.Count > 255)
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
            "max_packets", "timeout_seconds", "greetings", "rules"]);
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
            Profile = GetString(root, "profile", "probe"), MaxFrame = GetInt(root, "max_frame", 8192),
            MaxConnections = GetInt(root, "max_connections", 4), MaxPackets = GetInt(root, "max_packets", 64),
            TimeoutSeconds = root.TryGetProperty("timeout_seconds", out JsonElement t) ? t.GetDouble() : 10,
            Greetings = greetings.AsReadOnly(), Rules = rules.AsReadOnly()
        };
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
