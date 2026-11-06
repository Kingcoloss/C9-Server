using System.Text.Json;
using C9.Server;

internal static partial class Validation
{
    private static void MetadataFileLogging()
    {
        string path = Path.Combine(Path.GetTempPath(), "c9-events-" + Guid.NewGuid().ToString("N") + ".jsonl");
        try
        {
            JsonServerEvents events = new(path);
            Parallel.For(0, 24, i => events.Emit(new ServerEvent("received") { Session = i, Type = 4 }));
            // A second server invocation appends instead of losing the previous run.
            new JsonServerEvents(path).Emit(new ServerEvent("listening") { Port = 14000 });
            string[] lines = File.ReadAllLines(path);
            Check.Equal(25, lines.Length);
            HashSet<long> sessions = [];
            foreach (string line in lines)
            {
                using JsonDocument document = JsonDocument.Parse(line);
                JsonElement root = document.RootElement;
                Check.True(!root.TryGetProperty("payload", out _) && !root.TryGetProperty("credentials", out _));
                if (root.TryGetProperty("session", out JsonElement session)) sessions.Add(session.GetInt64());
            }
            Check.Equal(24, sessions.Count);
        }
        finally { File.Delete(path); }
    }
}
