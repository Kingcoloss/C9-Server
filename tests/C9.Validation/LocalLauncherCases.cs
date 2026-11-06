using C9.LocalLauncher;

internal static partial class Validation
{
    private static void LocalLauncherArguments()
    {
        LocalLaunchPlan plan = LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000,
            LocalLaunchPlan.Official52893.ToUpperInvariant());
        string[] fields = plan.Arguments.Split(':');
        Check.Equal(10, fields.Length);
        Check.Equal("127.0.0.1", fields[8]);
        Check.Equal("14000", fields[9]);
        string[] legacy = fields[0].Split(',');
        Check.Equal(3, legacy.Length);
        Check.Equal(fields[8], legacy[0]);
        Check.Equal(fields[9], legacy[1]);
        Check.Equal("C9.exe", Path.GetFileName(plan.Executable));
        Check.Equal(LocalLaunchPlan.Official52893, plan.ClientHash);
        Check.True(!plan.Arguments.Contains(' '));
        LocalLaunchPlan updated = LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000,
            LocalLaunchPlan.OfficialUpdated20261106);
        string[] updatedFields = updated.Arguments.Split(':');
        Check.Equal(11, updatedFields.Length);
        Check.Equal("", updatedFields[1]);
        Check.Equal("VALOFE", updatedFields[2]);
        Check.Equal("test", updatedFields[3]);
        Check.Equal("c9-gl", updatedFields[4]);
        Check.Equal("", updatedFields[7]);
        Check.Equal("127.0.0.1", updatedFields[8]);
        Check.Equal("14000", updatedFields[9]);
        Check.Equal("EN", updatedFields[10]);
        Check.True(!updated.Arguments.Contains(',') && !updated.Arguments.Contains('|'));
        Check.Equal("55350", updated.DisplayName);
        foreach (string invalid in new[] { "8.8.8.8", "localhost", "::1", "127.0.0.1,10021" })
            Check.Throws<ArgumentException>(() => LocalLaunchPlan.Create(Path.GetTempPath(), invalid, 14000,
                LocalLaunchPlan.Official52893));
        foreach (int invalid in new[] { 0, 65536 })
            Check.Throws<ArgumentException>(() => LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", invalid,
                LocalLaunchPlan.Official52893));
        Check.Throws<InvalidDataException>(() => LocalLaunchPlan.Create(Path.GetTempPath(), "127.0.0.1", 14000,
            new string('0', 64)));
    }
}
