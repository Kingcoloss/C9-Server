using System.Text.Json;
using C9.LocalLauncher;

internal static partial class Validation
{
    private static void LocalConfigurationBootstrap()
    {
        string root = Path.Combine(Path.GetTempPath(), "c9-config-" + Guid.NewGuid().ToString("N"));
        string workspace = Path.Combine(root, "workspace"), client = Path.Combine(root, "game");
        string? previousDotnet = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        Directory.CreateDirectory(client);
        Directory.CreateDirectory(Path.Combine(workspace, "config"));
        Directory.CreateDirectory(Path.Combine(workspace, "src", "C9.Server", "bin", "Release", "net10.0"));
        string runtime = Path.Combine(root, "runtime");
        Directory.CreateDirectory(runtime);
        try
        {
            File.WriteAllText(Path.Combine(workspace, "C9.slnx"), "<Solution />");
            File.WriteAllText(Path.Combine(runtime, "dotnet.exe"), "fixture");
            Environment.SetEnvironmentVariable("DOTNET_ROOT", runtime);
            File.WriteAllText(Path.Combine(workspace, "src", "C9.Server", "bin", "Release", "net10.0", "C9.Server.dll"), "fixture");
            File.WriteAllText(Path.Combine(workspace, "config", "lab-official-login-fixture.json"), """
                {"host":"127.0.0.1","port":14000,"profile":"fixture"}
                """);
            File.WriteAllText(Path.Combine(workspace, "config", "lab-official-character-lobby.json"), """
                {"host":"127.0.0.1","port":14001,"profile":"character_lobby"}
                """);
            string[] official = ["C9.exe", "C9Launcher.exe", "C9Patcher.exe", "C9Server.ini", "VerCli.C9"];
            foreach (string name in official) File.WriteAllText(Path.Combine(client, name), "unchanged official " + name);
            LocalConfigurationResult[] concurrent = new LocalConfigurationResult[4];
            Parallel.For(0, concurrent.Length, index => concurrent[index] = LocalLauncherConfiguration.Ensure(client, workspace));
            Check.Equal(1, concurrent.Count(value => value.Created));
            string configPath = Path.Combine(client, LocalLauncherConfiguration.FileName);
            byte[] original = File.ReadAllBytes(configPath);
            Check.True(!LocalLauncherConfiguration.Ensure(client, null).Created);
            Check.Bytes(original, File.ReadAllBytes(configPath));
            using (JsonDocument config = LocalLauncherConfiguration.Read(client))
            {
                LocalLauncherConfiguration.Verify(config.RootElement);
                Check.Equal("127.0.0.1", config.RootElement.GetProperty("host").GetString());
                Check.Equal(14000, config.RootElement.GetProperty("port").GetInt32());
                Check.True(config.RootElement.GetProperty("handoff_backend_config").GetString()!.EndsWith("lab-official-character-lobby.json", StringComparison.Ordinal));
            }
            foreach (string name in official) Check.Equal("unchanged official " + name, File.ReadAllText(Path.Combine(client, name)));
            Check.Equal(official.Length + 1, Directory.GetFiles(client).Length);
            // A config can be prepared inside the workspace for a read-only Client folder.
            string prepared = Path.Combine(root, "prepared", LocalLauncherConfiguration.FileName);
            Check.True(LocalLauncherConfiguration.Ensure(client, workspace, prepared).Created);
            Check.Bytes(original, File.ReadAllBytes(prepared));
            // Existing malformed configurations are reported and preserved, never silently reset.
            File.WriteAllText(configPath, "{broken");
            Check.Throws<JsonException>(() => LocalLauncherConfiguration.Ensure(client, workspace));
            Check.Equal("{broken", File.ReadAllText(configPath));
            File.WriteAllText(configPath, """{"host":"8.8.8.8","port":14000}""");
            Check.Throws<ArgumentException>(() => LocalLauncherConfiguration.Read(client));
            File.WriteAllText(configPath, """{"host":"127.0.0.1","host":"127.0.0.2","port":14000}""");
            Check.Throws<InvalidDataException>(() => LocalLauncherConfiguration.Read(client));
            // A changed project endpoint must match the backend profile before a sidecar is written.
            string other = Path.Combine(root, "other");
            Directory.CreateDirectory(other);
            using Stream defaults = typeof(LocalLauncherConfiguration).Assembly.GetManifestResourceStream("local-launcher.json")!;
            using StreamReader reader = new(defaults);
            File.WriteAllText(Path.Combine(workspace, "config", "local-launcher.json"), reader.ReadToEnd().Replace("14000", "14002"));
            Check.Throws<InvalidDataException>(() => LocalLauncherConfiguration.Ensure(other, workspace));
            Check.True(!File.Exists(Path.Combine(other, LocalLauncherConfiguration.FileName)));
            Check.Throws<ArgumentException>(() => LocalLauncherConfiguration.ValidateEndpoint("127.0.0.1", 14001));
        }
        finally
        {
            Environment.SetEnvironmentVariable("DOTNET_ROOT", previousDotnet);
            Directory.Delete(root, recursive: true);
        }
    }

    private static void PublicLauncherIndependent()
    {
        string directory = Path.Combine(Path.GetTempPath(), "c9-public-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string fallback = Path.Combine(directory, "VFUNLauncher.exe");
        File.WriteAllText(fallback, "fixture");
        string config = Path.Combine(directory, LocalLauncherConfiguration.FileName);
        try
        {
            Check.Equal(fallback, LocalLauncherConfiguration.FindOfficialLauncher(directory, fallback));
            File.WriteAllText(config, "{broken");
            Check.Equal(fallback, LocalLauncherConfiguration.FindOfficialLauncher(directory, fallback));
            Check.Equal("{broken", File.ReadAllText(config));
            File.WriteAllText(config, JsonSerializer.Serialize(new { host = "invalid local host", port = -1, official_launcher = fallback }));
            Check.Equal(fallback, LocalLauncherConfiguration.FindOfficialLauncher(directory));
            Check.Equal(2, Directory.GetFiles(directory).Length);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }
}
