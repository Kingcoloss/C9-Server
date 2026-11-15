using C9.Contracts;

namespace C9.Server;

public sealed record TraceOptions
{
    public string PayloadPolicy { get; init; } = TracePayloadPolicies.RedactCredentials;
    /// <summary>Root directory for JSONL trace files; null keeps the in-memory ring only.</summary>
    public string? Directory { get; init; }
    public int RingCapacity { get; init; } = 20_000;
    public long MaxFileBytes { get; init; } = 128L * 1024 * 1024;
    public int RetentionDays { get; init; } = 7;
    public long MaxTotalBytes { get; init; } = 2L * 1024 * 1024 * 1024;
    /// <summary>Bytes of wire kept for an undecodable pre-login frame under the redact policy.</summary>
    public const int RedactedFailurePrefix = 16;

    public void Validate()
    {
        if (!TracePayloadPolicies.IsValid(PayloadPolicy)) throw new ArgumentException("Unknown trace payload policy");
        if (RingCapacity is < 100 or > 1_000_000) throw new ArgumentException("Trace ring capacity must be 100..1000000");
        if (MaxFileBytes < 1024 * 1024 || RetentionDays is < 1 or > 365 || MaxTotalBytes < MaxFileBytes)
            throw new ArgumentException("Invalid trace retention limits");
    }
}
