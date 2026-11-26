using C9.LocalLauncher;

internal static partial class Validation
{
    private static void LauncherSettingsPersistence()
    {
        string root = Path.Combine(Path.GetTempPath(), "c9-settings-" + Guid.NewGuid().ToString("N"));
        string storage = Path.Combine(root, "storage"), game = Path.Combine(root, "game"), launcher = Path.Combine(root, "launcher");
        Directory.CreateDirectory(game);
        Directory.CreateDirectory(launcher);
        try
        {
            Check.Equal(new LauncherUserSettings(), LauncherUserSettings.Load(storage));
            Check.Equal(LauncherUserSettings.FallbackGameDirectory, LauncherUserSettings.DefaultGameDirectory(launcher, null));
            Check.True(LauncherUserSettings.ValidateGameDirectory(game) is not null, "A folder without C9.exe must be rejected");
            Check.True(LauncherUserSettings.ValidateGameDirectory("relative") is not null, "A relative folder must be rejected");
            File.WriteAllText(Path.Combine(game, "C9.exe"), "fixture");
            Check.Equal<string?>(null, LauncherUserSettings.ValidateGameDirectory(game));
            Check.Equal<string?>(null, LauncherUserSettings.ValidateGameDirectory("\"" + game + "\""));

            LauncherUserSettings saved = new() { GameDirectory = game, LastLogin = "player" };
            saved.Save(storage);
            Check.Equal(saved, LauncherUserSettings.Load(storage));
            Check.Equal(game, LauncherUserSettings.Load(storage).ResolveGameDirectory(launcher, null));
            Check.Equal(0, Directory.GetFiles(storage, "*.tmp").Length);
            (saved with { LastLogin = null }).Save(storage);
            Check.True(LauncherUserSettings.Load(storage).LastLogin is null, "Saving again must replace the previous value");

            string file = Path.Combine(storage, LauncherUserSettings.FileName);
            File.WriteAllText(file, "{broken");
            Check.Equal(new LauncherUserSettings(), LauncherUserSettings.Load(storage));
            // Settings saved by older L9 builds still load: the removed "compatibility_test" key is ignored.
            File.WriteAllText(file, """{"game_directory":"relative\\dir","compatibility_test":true,"last_login":"player"}""");
            Check.Equal(new LauncherUserSettings { LastLogin = "player" }, LauncherUserSettings.Load(storage));
            Check.Throws<ArgumentException>(() => new LauncherUserSettings { GameDirectory = "relative" }.Save(storage));

            File.WriteAllText(Path.Combine(launcher, "C9.exe"), "fixture");
            Check.Equal(Path.GetFullPath(launcher), LauncherUserSettings.DefaultGameDirectory(launcher, null));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static void LauncherReleaseLayoutHome()
    {
        string root = Path.Combine(Path.GetTempPath(), "c9-layout-" + Guid.NewGuid().ToString("N"));
        string bin = Path.Combine(root, "bin");
        Directory.CreateDirectory(bin);
        try
        {
            Check.Equal(bin, LocalLauncherConfiguration.LauncherHome(bin + Path.DirectorySeparatorChar));
            File.WriteAllText(Path.Combine(root, "L9.exe"), "starter");
            Check.Equal(root, LocalLauncherConfiguration.LauncherHome(bin + Path.DirectorySeparatorChar));
            Check.Equal(root, LocalLauncherConfiguration.LauncherHome(bin));
            Check.Equal(root, LocalLauncherConfiguration.LauncherHome(root));
            string flat = Path.Combine(root, "win-x64");
            Directory.CreateDirectory(flat);
            Check.Equal(flat, LocalLauncherConfiguration.LauncherHome(flat + Path.DirectorySeparatorChar));
        }
        finally { Directory.Delete(root, recursive: true); }
    }
}
