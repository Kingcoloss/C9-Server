using System.Text.Json;
using System.Text.Json.Serialization;

namespace C9.Server;

public sealed record ServerEvent(string Event)
{
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
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public void Emit(ServerEvent value)
    {
        // Event schema deliberately has no payload/credential fields.
        lock (sync) Console.WriteLine(JsonSerializer.Serialize(value, Options));
    }
}
