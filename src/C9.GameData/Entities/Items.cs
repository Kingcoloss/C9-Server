namespace C9.GameData.Entities;

public enum DeliveryTarget { Character, Account }

/// <summary>Pending = queued for the in-game Gift Box (not visible in game yet); Delivered = handed to the game; Cancelled by an operator.</summary>
public enum DeliveryStatus { Pending, Delivered, Cancelled }

/// <summary>A named set of items an operator sends in one go (for example a starter pack or an event reward).</summary>
public sealed class ItemBundle
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;
    public required string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public uint Version { get; set; }
    public List<ItemBundleEntry> Entries { get; set; } = [];
}

public sealed class ItemBundleEntry
{
    public long BundleId { get; set; }
    public short Line { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// One item stack sent to a character, or to an account (any of its characters may receive it once). Every row of one
/// send shares <see cref="BatchId"/> (the operator's request id); a bundle send also records <see cref="BundleId"/>.
/// </summary>
public sealed class ItemDelivery
{
    public long Id { get; set; }
    public Guid BatchId { get; set; }
    public DeliveryTarget Target { get; set; }
    public long AccountId { get; set; }
    public long? CharacterId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public long? BundleId { get; set; }
    public string? Message { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public required string CreatedBy { get; set; }
    public required string Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public uint Version { get; set; }
}
