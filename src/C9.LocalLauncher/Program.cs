using System.Text.Json;

namespace C9.LocalLauncher;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "--check")
        {
            try
            {
                LocalLaunchPlan plan = LocalLaunchPlan.Read(AppContext.BaseDirectory);
                Console.WriteLine(JsonSerializer.Serialize(new { status = "passed", client_sha256 = plan.ClientHash,
                    endpoint = "loopback", official_files_modified = false }));
                return 0;
            }
            catch (Exception error) when (LauncherForm.IsExpected(error))
            {
                Console.Error.WriteLine(error.Message);
                return 2;
            }
        }
        ApplicationConfiguration.Initialize();
        Application.Run(new LauncherForm());
        return 0;
    }
}
