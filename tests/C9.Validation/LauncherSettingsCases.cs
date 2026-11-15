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

            LauncherUserSettings saved = new() { GameDirectory = game, CompatibilityTest = true };
            saved.Save(storage);
            Check.Equal(saved, LauncherUserSettings.Load(storage));
            Check.Equal(game, LauncherUserSettings.Load(storage).ResolveGameDirectory(launcher, null));
            Check.Equal(0, Directory.GetFiles(storage, "*.tmp").Length);
            (saved with { CompatibilityTest = false }).Save(storage);
            Check.True(!LauncherUserSettings.Load(storage).CompatibilityTest, "Saving again must replace the previous value");

            string file = Path.Combine(storage, LauncherUserSettings.FileName);
            File.WriteAllText(file, "{broken");
            Check.Equal(new LauncherUserSettings(), LauncherUserSettings.Load(storage));
            File.WriteAllText(file, """{"game_directory":"relative\\dir","compatibility_test":true}""");
            Check.Equal(new LauncherUserSettings { CompatibilityTest = true }, LauncherUserSettings.Load(storage));
            Check.Throws<ArgumentException>(() => new LauncherUserSettings { GameDirectory = "relative" }.Save(storage));

            File.WriteAllText(Path.Combine(launcher, "C9.exe"), "fixture");
            Check.Equal(Path.GetFullPath(launcher), LauncherUserSettings.DefaultGameDirectory(launcher, null));
        }
        finally { Directory.Delete(root, recursive: true); }
    }
}
