using System.Security.Cryptography;
using System.Text.Json;

namespace C9.Protocol;

public static class PacketTwister
{
    private sealed record Mapping(int Width, (int Destination, int Source)[] Moves, byte[] Xor);
    private static readonly Mapping[][] EncodeMaps;
    private static readonly Mapping[][] DecodeMaps;

    static PacketTwister()
    {
        using Stream resource = typeof(PacketTwister).Assembly.GetManifestResourceStream("C9.Protocol.twisters.json")
            ?? throw new InvalidOperationException("Embedded PacketTwister catalog missing");
        using MemoryStream copy = new();
        resource.CopyTo(copy);
        byte[] bytes = copy.ToArray();
        string digest = Convert.ToHexStringLower(SHA256.HashData(bytes));
        if (digest != "5b380265f92ff3b2c900e505d322934ea400658591095f93c1bd45b66e741fc3")
            throw new InvalidOperationException("PacketTwister catalog provenance mismatch");
        using JsonDocument document = JsonDocument.Parse(bytes);
        JsonElement root = document.RootElement;
        JsonElement entries = root.GetProperty("entries");
        if (root.GetProperty("selector_modulus").GetInt32() != 80 || entries.GetArrayLength() != 80)
            throw new InvalidOperationException("Incomplete PacketTwister catalog");
        EncodeMaps = new Mapping[80][];
        DecodeMaps = new Mapping[80][];
        for (int selector = 0; selector < 80; selector++)
        {
            JsonElement entry = entries[selector];
            if (entry.GetProperty("selector").GetInt32() != selector)
                throw new InvalidOperationException("PacketTwister selector order mismatch");
            EncodeMaps[selector] = ReadMaps(entry.GetProperty("encode"));
            DecodeMaps[selector] = ReadMaps(entry.GetProperty("decode"));
        }
    }

    public static void EnsureLoaded() { _ = EncodeMaps.Length; }

    private static Mapping[] ReadMaps(JsonElement direction)
    {
        JsonElement branches = direction.GetProperty("length_branches");
        if (branches.GetArrayLength() != 4)
            throw new InvalidOperationException("Incomplete PacketTwister length branches");
        Mapping[] result = new Mapping[4];
        int[] widths = [4, 8, 16, 32];
        for (int branch = 0; branch < 4; branch++)
        {
            JsonElement item = branches[branch];
            int width = item.GetProperty("width").GetInt32();
            if (width != widths[branch]) throw new InvalidOperationException("Unexpected PacketTwister width");
            byte[] xor = Convert.FromHexString(item.GetProperty("xor_hex").GetString()!);
            if (xor.Length != width) throw new InvalidOperationException("Invalid PacketTwister XOR width");
            Array.Reverse(xor); // Catalog is a hexadecimal integer; mapping bits are little endian.
            List<(int, int)> moves = [];
            int[] permutation = Enumerable.Range(0, width * 8).ToArray();
            HashSet<int> destinations = [];
            foreach (JsonElement move in item.GetProperty("moves").EnumerateArray())
            {
                int destination = move[0].GetInt32(), source = move[1].GetInt32();
                if (destination < 0 || source < 0 || destination >= width * 8 || source >= width * 8 || !destinations.Add(destination))
                    throw new InvalidOperationException("Invalid PacketTwister bit mapping");
                permutation[destination] = source;
                moves.Add((destination, source));
            }
            if (permutation.Distinct().Count() != permutation.Length)
                throw new InvalidOperationException("PacketTwister mapping is not bijective");
            result[branch] = new Mapping(width, moves.ToArray(), xor);
        }
        return result;
    }

    public static void Apply(Span<byte> payload, uint selector, bool decode = false)
    {
        if (payload.Length < 4) return;
        int branch = payload.Length < 8 ? 0 : payload.Length < 16 ? 1 : payload.Length < 32 ? 2 : 3;
        Mapping map = (decode ? DecodeMaps : EncodeMaps)[selector % 80][branch];
        Span<byte> original = stackalloc byte[32];
        payload[..map.Width].CopyTo(original);
        foreach ((int destination, int source) in map.Moves)
        {
            int value = (original[source >> 3] >> (source & 7)) & 1;
            int mask = 1 << (destination & 7);
            payload[destination >> 3] = (byte)((payload[destination >> 3] & ~mask) | value << (destination & 7));
        }
        for (int i = 0; i < map.Width; i++) payload[i] ^= map.Xor[i];
    }
}
