using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace C9.LocalLauncher;

public sealed record LocalConfigurationResult(string Path, bool Created);

/// <summary>Creates only our sidecar configuration; never writes official Client files.</summary>
public static class LocalLauncherConfiguration
{
    public const string FileName = "L9.local.json";

    /// <summary>
    /// Folder that holds L9.local.json and the logs. In the release layout the runtime lives in
    /// <c>bin\</c> under a small starter <c>L9.exe</c>; then the folder above <c>bin\</c> is used.
    /// </summary>
    public static string LauncherHome(string runtimeDirectory)
    {
        DirectoryInfo runtime = new(Path.TrimEndingDirectorySeparator(Path.GetFullPath(runtimeDirectory)));
        return runtime.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) && runtime.Parent is { } home &&
            File.Exists(Path.Combine(home.FullName, "L9.exe")) ? home.FullName : runtime.FullName;
    }

    public static JsonDocument Read(string directory)
    {
        JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(directory, FileName)));
        try
        {
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object || root.EnumerateObject().Any(p => p.Name is not
                ("host" or "port" or "backend_assembly" or "backend_config" or "handoff_backend_config" or "dotnet_path" or "official_launcher")) ||
                root.EnumerateObject().GroupBy(p => p.Name).Any(group => group.Count() > 1))
                throw new InvalidDataException("Invalid local launcher configuration");
            ValidateEndpoint(root.GetProperty("host").GetString() ?? "", root.GetProperty("port").GetInt32());
            return document;
        }
        catch { document.Dispose(); throw; }
    }

    public static void ValidateEndpoint(string host, int port)
    {
        if (!IPAddress.TryParse(host, out IPAddress? address) || !IPAddress.IsLoopback(address) ||
            address.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException("Local launcher requires a numeric IPv4 loopback address");
        if (port is < 1 or > 65535 or 14001) throw new ArgumentException("Invalid local login port (14001 is reserved for handoff)");
    }

    public static string FindWorkspace(string directory, string? projectHint)
    {
        for (DirectoryInfo? current = new(Path.GetFullPath(directory)); current is not null; current = current.Parent)
            if (File.Exists(Path.Combine(current.FullName, "C9.slnx"))) return current.FullName;
        if (!string.IsNullOrWhiteSpace(projectHint) && File.Exists(Path.Combine(projectHint, "C9.slnx")))
            return Path.GetFullPath(projectHint);
        throw new InvalidOperationException("Cannot locate C9 workspace. Run L9 --configure --workspace <C9-Server> --client-dir <game folder>.");
    }

    public static LocalConfigurationResult Ensure(string directory, string? projectHint, string? output = null)
    {
        string folder = Path.GetFullPath(directory);
        if (!Directory.Exists(folder)) throw new DirectoryNotFoundException(folder);
        string path = output is null ? Path.Combine(folder, FileName) : Path.GetFullPath(output);
        if (Path.GetFileName(path) != FileName) throw new ArgumentException($"Configuration must be named {FileName}");
        if (File.Exists(path))
        {
            using JsonDocument existing = Read(Path.GetDirectoryName(path)!);
            return new(path, false);
        }

        string workspace = FindWorkspace(folder, projectHint);
        string defaultsPath = Path.Combine(workspace, "config", "local-launcher.json");
        using Stream defaultsStream = File.Exists(defaultsPath) ? File.OpenRead(defaultsPath) :
            typeof(LocalLauncherConfiguration).Assembly.GetManifestResourceStream("local-launcher.json")
            ?? throw new InvalidDataException("Missing project launcher defaults");
        using JsonDocument defaultsDocument = JsonDocument.Parse(defaultsStream);
        JsonElement defaults = defaultsDocument.RootElement;
        string host = defaults.GetProperty("host").GetString() ?? "";
        int port = defaults.GetProperty("port").GetInt32();
        ValidateEndpoint(host, port);
        string runtime = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "win-arm64" : "win-x64";
        string backend = defaults.GetProperty("backend_candidates").EnumerateArray()
            .Select(value => Path.GetFullPath((value.GetString() ?? "").Replace("{runtime}", runtime), workspace))
            .FirstOrDefault(File.Exists) ?? throw new FileNotFoundException("Build or publish C9.Server before creating the launcher configuration.");
        Dictionary<string, object> values = new()
        {
            ["host"] = host, ["port"] = port, ["backend_assembly"] = backend,
            ["backend_config"] = Path.GetFullPath(defaults.GetProperty("backend_config").GetString()!, workspace),
            ["handoff_backend_config"] = Path.GetFullPath(defaults.GetProperty("handoff_backend_config").GetString()!, workspace),
            ["dotnet_path"] = FindDotnet()
        };
        string? official = defaults.TryGetProperty("official_launcher", out JsonElement configuredOfficial) ? configuredOfficial.GetString() : null;
        if (official is not null && Path.IsPathFullyQualified(official) && File.Exists(official)) values["official_launcher"] = official;
        byte[] contents = JsonSerializer.SerializeToUtf8Bytes(values, new JsonSerializerOptions { WriteIndented = true });
        using (JsonDocument candidate = JsonDocument.Parse(contents)) Verify(candidate.RootElement);

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (FileStream stream = new(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(contents);
                stream.Flush(flushToDisk: true);
            }
            // A concurrent first launch must not replace a configuration created by another process.
            try { File.Move(temporary, path, overwrite: false); }
            catch (IOException) when (File.Exists(path))
            {
                using JsonDocument existing = Read(Path.GetDirectoryName(path)!);
                return new(path, false);
            }
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
        return new(path, true);
    }

    public static void Verify(JsonElement config)
    {
        string host = config.GetProperty("host").GetString() ?? "";
        int port = config.GetProperty("port").GetInt32();
        ValidateEndpoint(host, port);
        _ = RequiredFile(config, "backend_assembly", "C9.Server.dll");
        _ = RequiredFile(config, "dotnet_path", "dotnet.exe");
        using JsonDocument login = JsonDocument.Parse(File.ReadAllBytes(RequiredFile(config, "backend_config", null)));
        JsonElement root = login.RootElement;
        if (root.GetProperty("host").GetString() != host || root.GetProperty("port").GetInt32() != port ||
            root.GetProperty("profile").GetString() is not ("fixture" or "observer"))
            throw new InvalidDataException("Login profile does not match the configured local endpoint");
        if (config.TryGetProperty("handoff_backend_config", out _))
        {
            using JsonDocument handoff = JsonDocument.Parse(File.ReadAllBytes(RequiredFile(config, "handoff_backend_config", null)));
            JsonElement lobby = handoff.RootElement;
            if (root.GetProperty("profile").GetString() != "fixture" || lobby.GetProperty("host").GetString() != "127.0.0.1" ||
                lobby.GetProperty("port").GetInt32() != 14001 || lobby.GetProperty("profile").GetString() is not ("character_lobby" or "observer"))
                throw new InvalidDataException("Invalid local handoff profile");
        }
        if (config.TryGetProperty("official_launcher", out _)) _ = RequiredFile(config, "official_launcher", null);
    }

    public static string RequiredFile(JsonElement config, string property, string? expectedName)
    {
        string path = config.GetProperty(property).GetString() ?? "";
        if (!Path.IsPathFullyQualified(path) || !File.Exists(path) ||
            (expectedName is not null && !Path.GetFileName(path).Equals(expectedName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException($"Missing or invalid launcher path: {property}");
        return path;
    }

    private static string FindDotnet() => new[]
    {
        Environment.GetEnvironmentVariable("DOTNET_ROOT"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dotnet")
    }.OfType<string>().Where(Path.IsPathFullyQualified).Select(root => Path.Combine(root, "dotnet.exe"))
        .FirstOrDefault(File.Exists) ?? throw new FileNotFoundException("Install .NET 10 to start the local backend.");

    public static string? FindOfficialLauncher(string directory, string? configuredFallback = null)
    {
        string? configured = null;
        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(directory, FileName)));
            if (document.RootElement.ValueKind == JsonValueKind.Object && document.RootElement.TryGetProperty("official_launcher", out JsonElement property) && property.ValueKind == JsonValueKind.String)
                configured = property.GetString();
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException) { }
        // Public startup remains independent from local configuration and local Client hash checks.
        return new[] { configured, configuredFallback, "C:/VFUN/VLauncher/VFUNLauncher.exe" }
            .OfType<string>().FirstOrDefault(path => Path.IsPathFullyQualified(path) && File.Exists(path) && Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase));
    }
}
