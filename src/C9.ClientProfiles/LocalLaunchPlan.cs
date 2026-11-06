using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace C9.LocalLauncher;

public sealed record LocalLaunchPlan(string Executable, string Directory, string Arguments, string ClientHash)
{
    public const string Official52893 = "704687c39a293db92999c8a1ef5f22d9e147d743a94d3decb3de0a35af69dd63";

    public static LocalLaunchPlan Create(string directory, string host, int port, string clientHash)
    {
        if (!IPAddress.TryParse(host, out IPAddress? address) || !IPAddress.IsLoopback(address) ||
            address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            throw new ArgumentException("Local launcher requires a numeric IPv4 loopback address");
        if (port is < 1 or > 65535) throw new ArgumentException("Invalid local server port");
        if (!clientHash.Equals(Official52893, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("The updated Client needs a compatibility profile before local launch");
        string folder = Path.GetFullPath(directory);
        // The ordinary argument reader (007E9480) requires three comma fields.
        // The later VFUN reader (007E9EC0) additionally uses colon fields 3–5
        // and 8–9 without a count check. A legacy three-field launch crashes.
        // Every identity below belongs to the synthetic loopback lab.
        string endpoint = address.ToString();
        string arguments = $"{endpoint},{port},test|1|local|0|-1:0:0:lab:0:lab:0:0:{endpoint}:{port}";
        return new(Path.Combine(folder, "C9.exe"), folder, arguments, clientHash.ToLowerInvariant());
    }

    public static LocalLaunchPlan Read(string directory)
    {
        string folder = Path.GetFullPath(directory);
        using JsonDocument config = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(folder, "L9.local.json")));
        JsonElement root = config.RootElement;
        if (root.ValueKind != JsonValueKind.Object ||
            root.EnumerateObject().Any(p => p.Name is not ("host" or "port" or
                "backend_assembly" or "backend_config" or "dotnet_path")) ||
            !root.TryGetProperty("host", out _) || !root.TryGetProperty("port", out _))
            throw new InvalidDataException("Invalid local launcher configuration");
        string host = root.GetProperty("host").GetString() ?? "";
        int port = root.GetProperty("port").GetInt32();
        using FileStream game = File.OpenRead(Path.Combine(folder, "C9.exe"));
        string hash = Convert.ToHexString(SHA256.HashData(game)).ToLowerInvariant();
        return Create(folder, host, port, hash);
    }
}
