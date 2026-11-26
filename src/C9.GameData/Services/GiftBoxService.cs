using System.Data;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

/// <summary>A pending delivery as the in-game Gift Box shows it.</summary>
public sealed record GiftView(long Id, DeliveryTarget Target, int ItemId, int Quantity, string? Message,
    DateTimeOffset CreatedAt, DateTimeOffset? OpenedAt);

public enum GiftClaimOutcome { Claimed, NotFound, InventoryFull }

/// <summary>Result of taking a gift; <see cref="Item"/> is the new inventory stack when <see cref="Outcome"/> is Claimed.</summary>
public sealed record GiftClaim(GiftClaimOutcome Outcome, CharacterItem? Item);

/// <summary>
/// The in-game side of item deliveries (docs/ITEM_CATALOG_DELIVERY_TH.md): a character sees its own pending rows and its
/// account's pending rows; taking one creates a <see cref="CharacterItem"/> in the inventory. Player actions are audited
/// with the operator "game:&lt;character id&gt;".
/// </summary>
public sealed class GiftBoxService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    /// <summary>Most gifts one character is shown at a time (oldest first); the rest appear after these are taken.</summary>
    public const int MaxListed = 150;
    /// <summary>Slots per inventory bag: the Client's slot table holds 256 serials per bag (PS_Item_InvenSlotInfo_Ack).</summary>
    public const int BagSlots = 256;
    // Transaction-scoped advisory lock space for one character's inventory ("C9IT").
    private const int LockSpace = 0x43394954;

    public async Task<IReadOnlyList<GiftView>> ListAsync(long characterId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        long? account = await AccountOfAsync(db, characterId, token);
        if (account is null) return [];
        return await Visible(db, characterId, account.Value).OrderBy(d => d.Id).Take(MaxListed)
            .Select(d => new GiftView(d.Id, d.Target, d.ItemId, d.Quantity, d.Message, d.CreatedAt, d.OpenedAt))
            .ToListAsync(token);
    }

    /// <summary>Returns a visible gift and records the first time it was opened; null when it is not this character's.</summary>
    public async Task<GiftView?> ReadAsync(long characterId, long deliveryId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        long? account = await AccountOfAsync(db, characterId, token);
        if (account is null) return null;
        ItemDelivery? gift = await Visible(db, characterId, account.Value).FirstOrDefaultAsync(d => d.Id == deliveryId, token);
        if (gift is null) return null;
        if (gift.OpenedAt is null)
        {
            gift.OpenedAt = clock.GetUtcNow();
            try { await db.SaveChangesAsync(token); }
            catch (DbUpdateConcurrencyException) { } // Taken or opened elsewhere at the same moment; reading still succeeds.
        }
        return new GiftView(gift.Id, gift.Target, gift.ItemId, gift.Quantity, gift.Message, gift.CreatedAt, gift.OpenedAt);
    }

    /// <summary>
    /// Moves a gift into the character's inventory: first free slot of the bag its item belongs to. Serialised per character
    /// (advisory lock); an account gift taken by two characters at once goes to the first (row version), the other gets NotFound.
    /// </summary>
    public async Task<GiftClaim> ClaimAsync(long characterId, long deliveryId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({LockSpace}, {(int)(characterId % int.MaxValue)})", token);
        long? account = await AccountOfAsync(db, characterId, token);
        if (account is null) return new(GiftClaimOutcome.NotFound, null);
        ItemDelivery? gift = await Visible(db, characterId, account.Value).FirstOrDefaultAsync(d => d.Id == deliveryId, token);
        if (gift is null) return new(GiftClaimOutcome.NotFound, null);
        List<string> tags = await db.ItemCatalog.Where(i => i.ItemId == gift.ItemId).Select(i => i.Tags).FirstOrDefaultAsync(token) ?? [];
        short bag = BagOf(tags);
        List<short> used = await db.CharacterItems
            .Where(i => i.CharacterId == characterId && i.Location == ItemLocation.Inventory && i.Bag == bag)
            .Select(i => i.Slot).ToListAsync(token);
        short? slot = FirstFree(used);
        if (slot is null) return new(GiftClaimOutcome.InventoryFull, null);

        DateTimeOffset now = clock.GetUtcNow();
        CharacterItem item = new()
        {
            Serial = Guid.CreateVersion7(), CharacterId = characterId, Location = ItemLocation.Inventory, Bag = bag,
            Slot = slot.Value, ItemId = gift.ItemId, Quantity = gift.Quantity
        };
        db.CharacterItems.Add(item);
        (gift.Status, gift.DeliveredAt, gift.ClaimedCharacterId, gift.ItemSerial) = (DeliveryStatus.Delivered, now, characterId, item.Serial);
        OperatorContext context = Player(characterId, "Player took a Gift Box item in game");
        Audited.Record(db, context, "gift.claim", "item_delivery", gift.Id.ToString(), new { Status = DeliveryStatus.Pending },
            new { Status = DeliveryStatus.Delivered, CharacterId = characterId, item.Serial, item.ItemId, item.Quantity, Bag = bag, Slot = slot }, clock);
        try { await Audited.SaveAsync(db, token); }
        catch (GameDataException error) when (error.Code == GameDataError.Conflict) { return new(GiftClaimOutcome.NotFound, null); }
        await transaction.CommitAsync(token);
        return new(GiftClaimOutcome.Claimed, item);
    }

    /// <summary>The player deleted the gift in game without taking it. False when it is not this character's (or already gone).</summary>
    public async Task<bool> DismissAsync(long characterId, long deliveryId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        long? account = await AccountOfAsync(db, characterId, token);
        if (account is null) return false;
        ItemDelivery? gift = await Visible(db, characterId, account.Value).FirstOrDefaultAsync(d => d.Id == deliveryId, token);
        if (gift is null) return false;
        OperatorContext context = Player(characterId, "Player deleted a Gift Box item in game");
        (gift.Status, gift.CancelledAt, gift.CancelledBy) = (DeliveryStatus.Cancelled, clock.GetUtcNow(), context.Operator);
        Audited.Record(db, context, "gift.dismiss", "item_delivery", gift.Id.ToString(), new { Status = DeliveryStatus.Pending },
            new { Status = DeliveryStatus.Cancelled, gift.ItemId, gift.Quantity }, clock);
        try { await Audited.SaveAsync(db, token); }
        catch (GameDataException error) when (error.Code == GameDataError.Conflict) { return false; }
        return true;
    }

    /// <summary>Inventory stacks of a character, by bag and slot.</summary>
    public async Task<IReadOnlyList<CharacterItem>> InventoryAsync(long characterId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.CharacterItems.AsNoTracking()
            .Where(i => i.CharacterId == characterId && i.Location == ItemLocation.Inventory)
            .OrderBy(i => i.Bag).ThenBy(i => i.Slot).ToListAsync(token);
    }

    /// <summary>
    /// Inventory bag of an item from its catalog tags: quest items go to the quest tab, cash items to the charge tab,
    /// everything else to the main tab. The Client decides with its own item data; this mirrors it for the common cases.
    /// </summary>
    public static short BagOf(IReadOnlyCollection<string> tags) => tags.Contains("quest") ? (short)2 : tags.Contains("cash") ? (short)1 : (short)0;

    internal static short? FirstFree(IReadOnlyCollection<short> used)
    {
        HashSet<short> taken = used.ToHashSet();
        for (short slot = 0; slot < BagSlots; slot++)
            if (!taken.Contains(slot)) return slot;
        return null;
    }

    private static OperatorContext Player(long characterId, string reason) => new($"game:{characterId}", reason, Guid.NewGuid());

    private static Task<long?> AccountOfAsync(GameDbContext db, long characterId, CancellationToken token) =>
        db.Characters.Where(c => c.Id == characterId && c.DeletedAt == null).Select(c => (long?)c.AccountId).FirstOrDefaultAsync(token);

    private static IQueryable<ItemDelivery> Visible(GameDbContext db, long characterId, long accountId) =>
        db.ItemDeliveries.Where(d => d.Status == DeliveryStatus.Pending &&
            (d.CharacterId == characterId || (d.Target == DeliveryTarget.Account && d.AccountId == accountId)));
}
