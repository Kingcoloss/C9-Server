using System.Net;

namespace C9.GameData.Entities;

/// <summary>Persisted copy of the Portal session feed; Key matches the packet trace "{service}-{bootId}-{n}".</summary>
public sealed class SessionRecord
{
    public required string Key { get; set; }
    public required string Service { get; set; }
    public long? AccountId { get; set; }
    public long? CharacterId { get; set; }
    public IPAddress? RemoteAddress { get; set; }
    public string? ClientHash { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public string? EndReason { get; set; }
    public string? LastState { get; set; }
    public int? MapId { get; set; }
}

public sealed class AuditEntry
{
    public long Id { get; set; }
    public DateTimeOffset At { get; set; }
    public required string Operator { get; set; }
    public required string Action { get; set; }
    public string? TargetType { get; set; }
    public string? TargetId { get; set; }
    public string? Before { get; set; }
    public string? After { get; set; }
    public required string Reason { get; set; }
    public Guid RequestId { get; set; }
}

public enum AdminCommandStatus { Pending, Applied, Rejected, Expired }

/// <summary>Portal → game server command for online characters (applied by the server, never by the Portal).</summary>
public sealed class AdminCommand
{
    public Guid RequestId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public required string Operator { get; set; }
    public required string Kind { get; set; }
    public long? CharacterId { get; set; }
    public required string Payload { get; set; }
    public AdminCommandStatus Status { get; set; } = AdminCommandStatus.Pending;
    public DateTimeOffset? AppliedAt { get; set; }
    public string? Result { get; set; }
}

public sealed class CharacterSnapshot
{
    public long Id { get; set; }
    public long CharacterId { get; set; }
    public DateTimeOffset TakenAt { get; set; }
    public required string Cause { get; set; }
    /// <summary>JSON produced by <see cref="Services.CharacterExport"/>.</summary>
    public required string Data { get; set; }
}
