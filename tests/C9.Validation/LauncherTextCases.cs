using System.Text.RegularExpressions;
using C9.LocalLauncher;

internal static partial class Validation
{
    private static readonly Regex TextPlaceholder = new(@"\{\d+\}", RegexOptions.CultureInvariant);
    private static readonly Regex ThaiLetter = new(@"[฀-๿]", RegexOptions.CultureInvariant);
    private static readonly Regex TextCall = new(@"\b[TF]\(""((?:[^""\\]|\\.)*)""", RegexOptions.CultureInvariant);

    private static void LauncherTextTranslations()
    {
        foreach ((string thai, (string english, string chinese)) in LauncherText.Translations)
        {
            string[] expected = TextPlaceholder.Matches(thai).Select(m => m.Value).Order().ToArray();
            foreach (string text in new[] { english, chinese })
            {
                Check.True(text.Length > 0 && !ThaiLetter.IsMatch(text), $"Translation of \"{thai}\" is missing or still Thai");
                Check.True(TextPlaceholder.Matches(text).Select(m => m.Value).Order().SequenceEqual(expected),
                    $"Translation of \"{thai}\" must keep the placeholders {string.Join(",", expected)}");
            }
        }
        Check.Equal("บันทึก", LauncherText.T("บันทึก", LauncherLanguage.Thai));
        Check.Equal("Save", LauncherText.T("บันทึก", LauncherLanguage.English));
        Check.Equal("保存", LauncherText.T("บันทึก", LauncherLanguage.Chinese));
        Check.Equal("unknown server text", LauncherText.T("unknown server text", LauncherLanguage.English));
        foreach (LauncherLanguage language in Enum.GetValues<LauncherLanguage>())
            Check.Equal(language, LauncherText.Parse(LauncherText.Code(language)));
        Check.Equal(LauncherLanguage.Thai, LauncherText.Parse(null));
        Check.Equal(LauncherLanguage.Thai, LauncherText.Parse("fr"));

        // Every Thai text the launcher shows through T(...) / F(...) has an English and Chinese entry.
        string? root = AppContext.BaseDirectory;
        while (root is not null && !File.Exists(Path.Combine(root, "C9.slnx"))) root = Path.GetDirectoryName(root);
        if (root is null) return;
        string[] sources = Directory.GetFiles(Path.Combine(root, "src", "C9.ClientProfiles"), "*.cs")
            .Append(Path.Combine(root, "src", "C9.LocalLauncher.Maui", "LauncherPage.cs"))
            .Where(path => !path.EndsWith("LauncherText.cs", StringComparison.Ordinal)).ToArray();
        int used = 0;
        foreach (string path in sources)
            foreach (Match call in TextCall.Matches(File.ReadAllText(path)))
            {
                string thai = Regex.Unescape(call.Groups[1].Value);
                if (!ThaiLetter.IsMatch(thai)) continue;
                used++;
                Check.True(LauncherText.Translations.ContainsKey(thai), $"No translation for \"{thai}\" ({Path.GetFileName(path)})");
            }
        Check.True(used > 80, $"Expected the launcher's Thai texts in the sources, found {used}");
    }
}
