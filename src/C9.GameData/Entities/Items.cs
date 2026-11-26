namespace C9.GameData.Entities;

public enum DeliveryTarget { Character, Account }

/// <summary>
/// Pending = waiting in the in-game Gift Box (Mailbox "Gift Chest" tab); Delivered = the player took it into the inventory;
/// Cancelled by an operator, or deleted by the player in game (<see cref="ItemDelivery.CancelledBy"/> "game:&lt;character id&gt;").
/// </summary>
public enum DeliveryStatus { Pending, Delivered, Cancelled }

/// <summary>A named set of items an operator sends in one go (for example a starter pack or an event reward).</summary>
public sealed class ItemBundle
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; } = true;
    /// <summary>The one bundle every character created in game gets in its Gift Box (at most one bundle has this).</summary>
    public bool Starter { get; set; }
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
    /// <summary>First time the player opened the gift's message in game.</summary>
    public DateTimeOffset? OpenedAt { get; set; }
    /// <summary>Character that took the item (for an account gift: whichever of its characters took it first).</summary>
    public long? ClaimedCharacterId { get; set; }
    /// <summary>The <see cref="CharacterItem.Serial"/> created when the item was taken.</summary>
    public Guid? ItemSerial { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public uint Version { get; set; }
}
