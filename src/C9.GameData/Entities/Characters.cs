namespace C9.GameData.Entities;

public enum ItemLocation { Inventory, Equipment, Warehouse, Mail }

public sealed class Character
{
    public const int AppearanceBytes = 44;
    public const int MaxNameLength = 20;

    /// <summary>Globally unique; sent to the Client as the UInt64 character ID.</summary>
    public long Id { get; set; }
    public long AccountId { get; set; }
    public short Slot { get; set; }
    public required string Name { get; set; }
    /// <summary>Asset job code (index into CharacterLobby.TemplateIds).</summary>
    public short Job { get; set; }
    public required byte[] Appearance { get; set; }
    public short Level { get; set; } = 1;
    public long Exp { get; set; }
    public long Gold { get; set; }
    public int? Hp { get; set; }
    public int? Mp { get; set; }
    public int? MapId { get; set; }
    public short? Submap { get; set; }
    public float? PosX { get; set; }
    public float? PosY { get; set; }
    public float? PosZ { get; set; }
    public long PlaySeconds { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastPlayedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }

    public Account? Account { get; set; }
    public List<CharacterItem> Items { get; set; } = [];
}

public sealed class CharacterItem
{
    /// <summary>Per-instance serial; a duplicated serial indicates an item dupe.</summary>
    public Guid Serial { get; set; }
    public long CharacterId { get; set; }
    public ItemLocation Location { get; set; }
    public short Slot { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public short Enchant { get; set; }
    /// <summary>Raw JSON (jsonb) until option semantics are recovered from the Client.</summary>
    public string Options { get; set; } = "{}";
    public bool Bound { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public uint Version { get; set; }
}

/// <summary>
/// Reference data exported from the Client (re/scripts/export_item_catalog.py → items.json): names, description and
/// icon. Category, rarity and stack size are not decoded yet.
/// </summary>
public sealed class CatalogItem
{
    public int ItemId { get; set; }
    public string? NameEn { get; set; }
    public string? NameKo { get; set; }
    public string? DescriptionEn { get; set; }
    /// <summary>Icon file key (PNG name without extension in the catalog's icons folder); null when the Client has none.</summary>
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public short? Rarity { get; set; }
    public int? MaxStack { get; set; }
    public string Raw { get; set; } = "{}";
}
