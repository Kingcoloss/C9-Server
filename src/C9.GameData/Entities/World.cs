namespace C9.GameData.Entities;

public enum LedgerAsset { Gold, Item, Exp }

/// <summary>Append-only record of every asset change; the economy dashboard is derived from it.</summary>
public sealed class LedgerEntry
{
    public long Id { get; set; }
    public DateTimeOffset At { get; set; }
    public long CharacterId { get; set; }
    public LedgerAsset Asset { get; set; }
    public int? ItemId { get; set; }
    public Guid? ItemSerial { get; set; }
    /// <summary>Positive = into the world economy (faucet), negative = out (sink).</summary>
    public long Delta { get; set; }
    public long? BalanceAfter { get; set; }
    /// <summary>drop, quest, npc_shop_buy, npc_shop_sell, trade, mail, enchant, admin_grant, rollback, ...</summary>
    public required string Reason { get; set; }
    public string? Ref { get; set; }
    public int? MapId { get; set; }
    public Guid? RequestId { get; set; }
}

/// <summary>Multipliers for one scope. A value of 1 is neutral.</summary>
public sealed record RateSet(decimal Exp = 1, decimal Drop = 1, decimal RareDrop = 1, decimal Upgrade = 1)
{
    public static readonly RateSet Neutral = new();
    public bool IsValid(decimal max) =>
        new[] { Exp, Drop, RareDrop, Upgrade }.All(value => value >= 0 && value <= max);
    public RateSet Times(RateSet other) =>
        new(Exp * other.Exp, Drop * other.Drop, RareDrop * other.RareDrop, Upgrade * other.Upgrade);
    public RateSet Cap(decimal cap) =>
        new(Math.Min(Exp, cap), Math.Min(Drop, cap), Math.Min(RareDrop, cap), Math.Min(Upgrade, cap));
}

/// <summary>Single "server" row: base multipliers and the final cap.</summary>
public sealed class RatePolicy
{
    public const string ServerScope = "server";
    public string Scope { get; set; } = ServerScope;
    public RateSet Rates { get; set; } = RateSet.Neutral;
    public decimal Cap { get; set; } = 100;
    public DateTimeOffset UpdatedAt { get; set; }
    public uint Version { get; set; }
}

public sealed class AccountRateOverride
{
    public long AccountId { get; set; }
    public RateSet Rates { get; set; } = RateSet.Neutral;
    public DateTimeOffset? ExpiresAt { get; set; }
    public string? Note { get; set; }
    public uint Version { get; set; }
}

public sealed class WorldEvent
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    /// <summary>Repeat every N days within [StartsAt, RecurUntil]; null = one-off.</summary>
    public int? RepeatEveryDays { get; set; }
    public DateTimeOffset? RecurUntil { get; set; }
    public RateSet Rates { get; set; } = RateSet.Neutral;
    /// <summary>Effects that need gameplay systems (login rewards, open maps): raw JSON for now.</summary>
    public string Extra { get; set; } = "{}";
    public bool Enabled { get; set; } = true;
    public required string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public uint Version { get; set; }

    public bool IsActiveAt(DateTimeOffset at)
    {
        if (!Enabled || at < StartsAt) return false;
        if (at < EndsAt) return true;
        if (RepeatEveryDays is not { } days || days <= 0 || (RecurUntil is { } until && at >= until)) return false;
        TimeSpan period = TimeSpan.FromDays(days);
        TimeSpan length = EndsAt - StartsAt;
        long cycles = (at - StartsAt).Ticks / period.Ticks;
        DateTimeOffset start = StartsAt + TimeSpan.FromTicks(period.Ticks * cycles);
        return at < start + length && (RecurUntil is null || start < RecurUntil);
    }
}

public enum AnnouncementChannel { Launcher, InGame }

public sealed class Announcement
{
    public long Id { get; set; }
    public AnnouncementChannel Channel { get; set; }
    public required string Message { get; set; }
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public int? RepeatMinutes { get; set; }
    public required string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>Key/value world configuration (maintenance mode, default spawn map, ...), value is JSON.</summary>
public sealed class WorldSetting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public uint Version { get; set; }
}
