using System.Text.Json;
using System.Reflection;

namespace C9.LocalLauncher;

internal static class Program
{
    internal static string? ProjectWorkspace => typeof(Program).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
        .FirstOrDefault(value => value.Key == "C9WorkspaceRoot")?.Value;

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
                string? command = args.FirstOrDefault() is "--check" or "--configure" ? args[0] : null;
                string directory = AppContext.BaseDirectory;
                if (!File.Exists(Path.Combine(directory, "C9.exe")) && ProjectWorkspace is { } root &&
                    File.Exists(Path.Combine(root, "config", "local-launcher.json")))
                {
                    using JsonDocument defaults = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(root, "config", "local-launcher.json")));
                    if (defaults.RootElement.TryGetProperty("client_directory", out JsonElement client) && client.GetString() is { } path &&
                        Path.IsPathFullyQualified(path) && File.Exists(Path.Combine(path, "C9.exe"))) directory = path;
                }
                string? workspace = ProjectWorkspace, output = null, configurationDirectory = null;
                if (directory != AppContext.BaseDirectory) configurationDirectory = AppContext.BaseDirectory;
                for (int index = command is null ? 0 : 1; index < args.Length; index++)
                {
                    if (index + 1 >= args.Length) throw new ArgumentException("Missing option value");
                    switch (args[index])
                    {
                        case "--client-dir": directory = Path.GetFullPath(args[++index]); break;
                        case "--workspace": workspace = Path.GetFullPath(args[++index]); break;
                        case "--output": output = Path.GetFullPath(args[++index]); break;
                        case "--config-dir": configurationDirectory = Path.GetFullPath(args[++index]); break;
                        default: throw new ArgumentException("Unknown launcher option");
                    }
                }
                if (command is null)
                {
                    if (output is not null) throw new ArgumentException("--output requires --configure or --check");
                    ApplicationConfiguration.Initialize();
                    Application.Run(new LauncherForm(directory, configurationDirectory, workspace));
                    return 0;
                }
                output ??= configurationDirectory is null ? null : Path.Combine(configurationDirectory, LocalLauncherConfiguration.FileName);
                LocalConfigurationResult result = LocalLauncherConfiguration.Ensure(directory, workspace, output);
                using JsonDocument config = LocalLauncherConfiguration.Read(Path.GetDirectoryName(result.Path)!);
                LocalLauncherConfiguration.Verify(config.RootElement);
                if (command == "--configure")
                    Console.WriteLine(JsonSerializer.Serialize(new { status = "passed", configuration = result.Path, created = result.Created,
                        client_compatibility_checked = false, official_files_modified = false }));
                else
                {
                    LocalLaunchPlan plan = LocalLaunchPlan.Read(directory, Path.GetDirectoryName(result.Path));
                    Console.WriteLine(JsonSerializer.Serialize(new { status = "passed", configuration = result.Path, created = result.Created,
                        client_sha256 = plan.ClientHash,
                        endpoint = "loopback", official_files_modified = false }));
                }
                return 0;
        }
        catch (Exception error) when (LauncherForm.IsExpected(error))
            {
                Console.Error.WriteLine(error.Message);
                return 2;
            }
    }
}
