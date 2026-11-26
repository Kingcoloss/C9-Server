// Starter L9.exe for the release layout: only L9.exe and L9.local.json (plus logs) sit in the
// launcher folder, the MAUI launcher and its runtime live in bin\. WinUI resolves its classes from
// the manifest of the exe in the runtime folder, so the real app must start from bin\L9.exe.
// Built by scripts/Build-C9Launcher.ps1 with the .NET Framework csc.exe that ships with Windows.
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

[assembly: AssemblyTitle("C9 Local Launcher")]
[assembly: AssemblyProduct("C9 Local Launcher")]

internal static class L9Starter
{
    private const string Title = "C9 Local Launcher";

    [STAThread]
    private static int Main(string[] args)
    {
        string home = AppDomain.CurrentDomain.BaseDirectory;
        string target = Path.Combine(Path.Combine(home, "bin"), "L9.exe");
        if (!File.Exists(target))
        {
            MessageBox.Show("ไม่พบไฟล์ " + target + "\nกรุณาติดตั้ง C9 Local Launcher ใหม่", Title,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 2;
        }
        try
        {
            ProcessStartInfo start = new ProcessStartInfo(target, JoinArguments(args));
            start.UseShellExecute = false;
            start.WorkingDirectory = home;
            Process.Start(start);
            return 0;
        }
        catch (Exception error)
        {
            MessageBox.Show("เปิด " + target + " ไม่สำเร็จ\n" + error.Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }

    // Re-quotes arguments with the rules CommandLineToArgvW uses, so --client-dir "C:\x y\" survives.
    private static string JoinArguments(string[] args)
    {
        StringBuilder line = new StringBuilder();
        foreach (string arg in args)
        {
            if (line.Length > 0) line.Append(' ');
            if (arg.Length > 0 && arg.IndexOfAny(new[] { ' ', '\t', '"' }) < 0) { line.Append(arg); continue; }
            line.Append('"');
            int slashes = 0;
            foreach (char c in arg)
            {
                if (c == '\\') { slashes++; continue; }
                line.Append('\\', c == '"' ? slashes * 2 + 1 : slashes);
                slashes = 0;
                line.Append(c);
            }
            line.Append('\\', slashes * 2);
            line.Append('"');
        }
        return line.ToString();
    }
}
