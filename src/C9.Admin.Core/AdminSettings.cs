using System.Text.Json;
using System.Text.Json.Nodes;
using C9.Server;

namespace C9.Admin;

public sealed record RateMultipliers(decimal Drop = 1, decimal RareDrop = 1, decimal Upgrade = 1, decimal Exp = 1)
{
    internal bool IsValid => new[] { Drop, RareDrop, Upgrade, Exp }.All(rate => rate is >= 0 and <= 100);
}

public sealed record AdminParameters
{
    public bool LocalAccountEnabled { get; init; } = true;
    public int InitialHp { get; set; } = 100;
    public int InitialMp { get; set; } = 100;
    public int LoginConnections { get; set; } = 2;
    public int LobbyConnections { get; set; } = 1;
    public double TimeoutSeconds { get; set; } = 120;
    public RateMultipliers ServerRates { get; set; } = new();
    public RateMultipliers EventRates { get; set; } = new();
    public RateMultipliers AccountRates { get; set; } = new();
}

public sealed class AdminSettings(string workspace, string stateDirectory)
{
    internal const string LoginConfig = "lab-official-login-fixture.json";
    internal const string LobbyConfig = "lab-official-character-lobby.json";
    private readonly string path = Path.Combine(stateDirectory, "parameters.local.json");

    public AdminParameters Read()
    {
        AdminParameters? saved = LocalAdminFiles.Read<AdminParameters>(path);
        if (saved is not null) return saved;
        ServerOptions login = ServerOptions.FromJson(File.ReadAllText(BaseConfig(LoginConfig)));
        ServerOptions lobby = ServerOptions.FromJson(File.ReadAllText(BaseConfig(LobbyConfig)));
        return new()
        {
            LocalAccountEnabled = login.LocalAccountEnabled && lobby.LocalAccountEnabled,
            InitialHp = lobby.InitialHp, InitialMp = lobby.InitialMp,
            LoginConnections = login.MaxConnections, LobbyConnections = lobby.MaxConnections,
            TimeoutSeconds = lobby.TimeoutSeconds
        };
    }

    public void Save(AdminParameters parameters)
    {
        if (parameters.ServerRates?.IsValid != true || parameters.EventRates?.IsValid != true ||
            parameters.AccountRates?.IsValid != true)
            throw new ArgumentException("ตัวคูณต้องอยู่ระหว่าง 0 ถึง 100");
        _ = BuildConfig(LoginConfig, parameters);
        _ = BuildConfig(LobbyConfig, parameters);
        LocalAdminFiles.Write(path, parameters, overwrite: true);
    }

    public string CharacterStorePath
    {
        get
        {
            string profile = BaseConfig(LobbyConfig);
            ServerOptions options = ServerOptions.FromJson(File.ReadAllText(profile));
            options.Validate();
            return Path.GetFullPath(options.CharacterStore!, Path.GetDirectoryName(profile)!);
        }
    }

    internal string PrepareConfig(string name)
    {
        if (!File.Exists(path)) return BaseConfig(name);
        JsonObject config = BuildConfig(name, Read());
        string snapshot = Path.Combine(stateDirectory, "profiles", name);
        LocalAdminFiles.Write(snapshot, config, overwrite: true);
        return snapshot;
    }

    private string BaseConfig(string name) => Path.Combine(workspace, "config", name);

    private JsonObject BuildConfig(string name, AdminParameters parameters)
    {
        string profile = BaseConfig(name);
        JsonObject config = JsonNode.Parse(File.ReadAllText(profile))!.AsObject();
        bool lobby = name == LobbyConfig;
        config["local_account_enabled"] = parameters.LocalAccountEnabled;
        config["max_connections"] = lobby ? parameters.LobbyConnections : parameters.LoginConnections;
        config["timeout_seconds"] = parameters.TimeoutSeconds;
        if (lobby)
        {
            config["initial_hp"] = parameters.InitialHp;
            config["initial_mp"] = parameters.InitialMp;
            config["character_store"] = Path.GetFullPath(config["character_store"]!.GetValue<string>(), Path.GetDirectoryName(profile)!);
        }
        ServerOptions.FromJson(config.ToJsonString()).Validate();
        return config;
    }
}

internal static class LocalAdminFiles
{
    internal static T? Read<T>(string path) where T : class
    {
        try
        {
            using FileStream input = new(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
            return JsonSerializer.Deserialize<T>(input) ?? throw new InvalidDataException("Empty Admin Console data");
        }
        catch (FileNotFoundException) { return null; }
        catch (DirectoryNotFoundException) { return null; }
    }

    internal static void Write<T>(string path, T value, bool overwrite)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (FileStream output = new(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(output, value);
                output.Flush(flushToDisk: true);
            }
            File.Move(temporary, path, overwrite);
        }
        finally { File.Delete(temporary); }
    }
}
