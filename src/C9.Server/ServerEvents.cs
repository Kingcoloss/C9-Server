using System.Text.Json;
using System.Text.Json.Serialization;

namespace C9.Server;

public sealed record ServerEvent(string Event)
{
    public DateTimeOffset RecordedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public long? Session { get; init; }
    public ushort? Type { get; init; }
    public int? WireLength { get; init; }
    public int? PayloadLength { get; init; }
    public byte? Flag { get; init; }
    public uint? Sequence { get; init; }
    public string? State { get; init; }
    public string? Reason { get; init; }
    public string? Host { get; init; }
    public int? Port { get; init; }
    public string? Profile { get; init; }
    public string? ClientCompatibility { get; init; }
    public int? CipherEpochLimit { get; init; }
    public string? Runtime { get; init; }
}

public interface IServerEvents { void Emit(ServerEvent value); }

public sealed class JsonServerEvents : IServerEvents
{
    private readonly object sync = new();
    private readonly string? filePath;

    public JsonServerEvents(string? filePath = null)
    {
        this.filePath = filePath is null ? null : Path.GetFullPath(filePath);
        if (this.filePath is not null) File.AppendAllText(this.filePath, "");
    }
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public void Emit(ServerEvent value)
    {
        // Event schema deliberately has no payload/credential fields.
        lock (sync)
        {
            string line = JsonSerializer.Serialize(value, Options);
            if (filePath is null) Console.WriteLine(line);
            else File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }
}
