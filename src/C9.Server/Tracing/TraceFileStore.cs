using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using C9.Contracts;

namespace C9.Server;

/// <summary>
/// Append-only JSONL persistence for trace records: {root}/{service}/{yyyyMMdd}/{boot}-{n}.jsonl.
/// A bounded queue decouples sessions from disk; when full, records are dropped and counted.
/// Retention removes day folders older than the limit and the oldest files above the size cap.
/// </summary>
public sealed record TraceIndexEntry(string Kind, SessionInfo? Session, string? File, int? FrameCount, int? ErrorCount);

public sealed class TraceFileStore : IAsyncDisposable
{
    private readonly Channel<TraceRecord> queue = Channel.CreateBounded<TraceRecord>(
        new BoundedChannelOptions(65_536) { FullMode = BoundedChannelFullMode.Wait, SingleReader = true });
    private readonly string serviceRoot;
    private readonly string bootId;
    private readonly TraceOptions options;
    private readonly Task? writer;
    private long dropped;
    private int fileNumber;
    private string? currentPath;

    public TraceFileStore(string root, string serviceId, string bootId, TraceOptions options)
    {
        serviceRoot = Path.Combine(Path.GetFullPath(root), serviceId);
        this.bootId = bootId;
        this.options = options;
        Directory.CreateDirectory(serviceRoot);
        ApplyRetention();
        writer = Task.Run(WriteLoopAsync);
    }

    public string Root => serviceRoot;
    /// <summary>Small side index of session records so history loads without scanning frame files.</summary>
    public string IndexPath => Path.Combine(serviceRoot, "sessions-index.jsonl");
    public long Dropped => Interlocked.Read(ref dropped);
    /// <summary>Invoked on the writer thread with the file that received each session record.</summary>
    public Action<string, string>? SessionWritten { get; set; }

    public void Enqueue(TraceRecord record)
    {
        if (!queue.Writer.TryWrite(record)) Interlocked.Increment(ref dropped);
    }

    private async Task WriteLoopAsync()
    {
        StreamWriter? output = null;
        await using StreamWriter index = new(new FileStream(IndexPath, FileMode.Append, FileAccess.Write,
            FileShare.Read | FileShare.Delete), new UTF8Encoding(false)) { NewLine = "\n" };
        string day = "";
        DateTimeOffset lastRetention = DateTimeOffset.UtcNow;
        try
        {
            while (await queue.Reader.WaitToReadAsync())
            {
                while (queue.Reader.TryRead(out TraceRecord? record))
                {
                    string today = DateTimeOffset.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    if (output is null || today != day || output.BaseStream.Length >= options.MaxFileBytes)
                    {
                        if (output is not null) await output.DisposeAsync();
                        day = today;
                        output = Open(day);
                    }
                    string line = JsonSerializer.Serialize(record, ContractJson.Options);
                    await output.WriteLineAsync(line);
                    if (record.Session is { } session)
                    {
                        SessionWritten?.Invoke(session.Key, currentPath!);
                        await index.WriteLineAsync(JsonSerializer.Serialize(
                            new TraceIndexEntry(record.Kind, session, Path.GetRelativePath(serviceRoot, currentPath!), record.FrameCount, record.ErrorCount),
                            ContractJson.Options));
                    }
                    else if (record.Frame is { } frame) SessionWritten?.Invoke(frame.Session, currentPath!);
                }
                if (output is not null) await output.FlushAsync();
                await index.FlushAsync();
                if (DateTimeOffset.UtcNow - lastRetention > TimeSpan.FromHours(1))
                {
                    lastRetention = DateTimeOffset.UtcNow;
                    ApplyRetention();
                }
            }
        }
        finally
        {
            if (output is not null) await output.DisposeAsync();
        }
    }

    private StreamWriter Open(string day)
    {
        string folder = Directory.CreateDirectory(Path.Combine(serviceRoot, day)).FullName;
        string path;
        do path = Path.Combine(folder, $"{bootId}-{++fileNumber:D3}.jsonl"); while (File.Exists(path));
        currentPath = path;
        FileStream stream = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read | FileShare.Delete, 64 * 1024);
        return new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = false, NewLine = "\n" };
    }

    /// <summary>Lists trace files of this service, newest first.</summary>
    public IEnumerable<FileInfo> Files() =>
        Directory.Exists(serviceRoot)
            ? new DirectoryInfo(serviceRoot).EnumerateFiles("*.jsonl", SearchOption.AllDirectories).OrderByDescending(f => f.FullName, StringComparer.Ordinal)
            : [];

    public IEnumerable<TraceIndexEntry> ReadIndex()
    {
        if (!File.Exists(IndexPath)) yield break;
        using FileStream stream = new(IndexPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using StreamReader reader = new(stream, Encoding.UTF8);
        while (reader.ReadLine() is { } line)
        {
            TraceIndexEntry? entry;
            try { entry = JsonSerializer.Deserialize<TraceIndexEntry>(line, ContractJson.Options); }
            catch (JsonException) { continue; }
            if (entry?.Session is not null && entry.File is not null) yield return entry;
        }
    }

    public string FullPath(string relative) => Path.GetFullPath(Path.Combine(serviceRoot, relative));

    public static IEnumerable<TraceRecord> Read(string path, string? sessionFilter = null)
    {
        string? needle = sessionFilter is null ? null : $"\"session\":\"{sessionFilter}\"";
        string? sessionNeedle = sessionFilter is null ? null : $"\"key\":\"{sessionFilter}\"";
        FileStream stream;
        try { stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException) { yield break; }
        using StreamReader reader = new(stream, Encoding.UTF8);
        while (reader.ReadLine() is { } line)
        {
            if (needle is not null && !line.Contains(needle, StringComparison.Ordinal) && !line.Contains(sessionNeedle!, StringComparison.Ordinal))
                continue;
            TraceRecord? record;
            try { record = JsonSerializer.Deserialize<TraceRecord>(line, ContractJson.Options); }
            catch (JsonException) { continue; } // A partially written last line.
            if (record is not null) yield return record;
        }
    }

    private void ApplyRetention()
    {
        try
        {
            string cutoff = DateTimeOffset.UtcNow.AddDays(-options.RetentionDays).ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            foreach (DirectoryInfo day in new DirectoryInfo(serviceRoot).EnumerateDirectories())
                if (day.Name.Length == 8 && day.Name.All(char.IsAsciiDigit) && string.CompareOrdinal(day.Name, cutoff) < 0)
                    day.Delete(recursive: true);
            long total = 0;
            foreach (FileInfo file in Files())
            {
                total += file.Length;
                if (total > options.MaxTotalBytes && file.FullName != currentPath) file.Delete();
            }
            CompactIndex();
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"c9server: trace retention skipped: {error.Message}");
        }
    }

    /// <summary>Drops index entries whose data file was removed by retention (only before the writer opens it).</summary>
    private void CompactIndex()
    {
        if (writer is not null || !File.Exists(IndexPath)) return;
        string[] kept = File.ReadAllLines(IndexPath).Where(line =>
        {
            try
            {
                return JsonSerializer.Deserialize<TraceIndexEntry>(line, ContractJson.Options) is { File: { } file } && File.Exists(FullPath(file));
            }
            catch (JsonException) { return false; }
        }).ToArray();
        string temporary = IndexPath + ".tmp";
        File.WriteAllLines(temporary, kept);
        File.Move(temporary, IndexPath, overwrite: true);
    }

    public async ValueTask DisposeAsync()
    {
        queue.Writer.TryComplete();
        if (writer is not null) await writer;
    }
}
