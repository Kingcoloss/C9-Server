using System.Text.Json;
using System.Text.Json.Serialization;

namespace C9.LocalLauncher;

/// <summary>
/// Per-user launcher preferences (game folder and launch mode). Stored outside the game folder and
/// separately from <see cref="LocalLauncherConfiguration"/>, whose schema is strict and project-owned.
/// </summary>
public sealed record LauncherUserSettings
{
    public const string FileName = "launcher-settings.json";
    public const string FallbackGameDirectory = @"C:\VFUN\C9-GL";
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    [JsonPropertyName("game_directory")] public string? GameDirectory { get; init; }
    [JsonPropertyName("compatibility_test")] public bool CompatibilityTest { get; init; }
    /// <summary>c9-auth base URL; null means <see cref="LauncherAuthClient.DefaultUrl"/>.</summary>
    [JsonPropertyName("auth_url")] public string? AuthUrl { get; init; }
    /// <summary>Last login name, pre-filled on the next start. Passwords and tokens are never saved.</summary>
    [JsonPropertyName("last_login")] public string? LastLogin { get; init; }

    public static string DefaultStorageDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "C9Local", "L9");

    /// <summary>Missing or damaged settings fall back to defaults so the launcher always opens.</summary>
    public static LauncherUserSettings Load(string? storageDirectory = null)
    {
        string path = Path.Combine(storageDirectory ?? DefaultStorageDirectory, FileName);
        try
        {
            if (!File.Exists(path)) return new();
            LauncherUserSettings? value = JsonSerializer.Deserialize<LauncherUserSettings>(File.ReadAllBytes(path), Options);
            if (value is null) return new();
            return value.GameDirectory is { } directory && !Path.IsPathFullyQualified(directory)
                ? value with { GameDirectory = null } : value;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException) { return new(); }
    }

    public void Save(string? storageDirectory = null)
    {
        if (GameDirectory is { } directory && !Path.IsPathFullyQualified(directory))
            throw new ArgumentException("Game directory must be a full path");
        string folder = storageDirectory ?? DefaultStorageDirectory;
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, FileName);
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllBytes(temporary, JsonSerializer.SerializeToUtf8Bytes(this, Options));
            File.Move(temporary, path, overwrite: true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    /// <summary>
    /// The folder used when the user has not chosen one: C9.exe beside the launcher, then the project
    /// default in config/local-launcher.json, then the standard VFUN install folder.
    /// </summary>
    public static string DefaultGameDirectory(string launcherDirectory, string? workspace)
    {
        if (File.Exists(Path.Combine(launcherDirectory, "C9.exe"))) return Path.GetFullPath(launcherDirectory);
        if (workspace is not null)
        {
            try
            {
                string defaults = Path.Combine(workspace, "config", "local-launcher.json");
                if (File.Exists(defaults))
                {
                    using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(defaults));
                    if (document.RootElement.TryGetProperty("client_directory", out JsonElement client) &&
                        client.GetString() is { } path && Path.IsPathFullyQualified(path))
                        return Path.GetFullPath(path);
                }
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException) { }
        }
        return FallbackGameDirectory;
    }

    public string ResolveGameDirectory(string launcherDirectory, string? workspace) =>
        GameDirectory is { } chosen ? Path.GetFullPath(chosen) : DefaultGameDirectory(launcherDirectory, workspace);

    /// <summary>Returns a Thai message describing why <paramref name="directory"/> is unusable, or null.</summary>
    public static string? ValidateGameDirectory(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory)) return "กรุณาเลือกโฟลเดอร์เกม";
        string trimmed = directory.Trim().Trim('"');
        if (!Path.IsPathFullyQualified(trimmed)) return "ต้องเป็น path เต็ม เช่น C:\\VFUN\\C9-GL";
        if (!Directory.Exists(trimmed)) return "ไม่พบโฟลเดอร์นี้";
        if (!File.Exists(Path.Combine(trimmed, "C9.exe"))) return "ไม่พบ C9.exe ในโฟลเดอร์นี้";
        return null;
    }

    public static string NormalizeDirectory(string directory) =>
        Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory.Trim().Trim('"')));
}
