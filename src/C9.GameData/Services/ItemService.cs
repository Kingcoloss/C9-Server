using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

public sealed record CatalogImportRow(int Id, string? NameEn, string? NameKo, string? DescriptionEn, string? Icon);

public sealed record CatalogImportResult(int Added, int Updated, int Unchanged);

public sealed record ItemLine(int ItemId, int Quantity);

/// <summary>
/// What to send and to whom. <see cref="Recipients"/> are character names (<see cref="DeliveryTarget.Character"/>) or
/// account login names (<see cref="DeliveryTarget.Account"/>); <see cref="AllAccounts"/> sends to every active account.
/// Contents are a bundle and/or item lines.
/// </summary>
public sealed record DeliveryRequest(DeliveryTarget Target, IReadOnlyList<string> Recipients, bool AllAccounts,
    long? BundleId, IReadOnlyList<ItemLine> Items, string? Message);

public sealed record DeliveryResult(Guid BatchId, int Recipients, int Rows);

/// <summary>A delivery row with the names an operator needs to read it.</summary>
public sealed record DeliveryView(long Id, Guid BatchId, DeliveryTarget Target, long AccountId, string AccountLogin,
    long? CharacterId, string? CharacterName, int ItemId, string? ItemName, string? Icon, int Quantity, long? BundleId,
    string? Message, DeliveryStatus Status, string CreatedBy, string Reason, DateTimeOffset CreatedAt, DateTimeOffset? CancelledAt);

/// <summary>Item catalog (from the Client), item bundles and the item delivery queue (Admin Portal "Items").</summary>
public sealed class ItemService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    public const int MaxQuantity = 9_999;
    public const int MaxLines = 50;
    public const int MaxRecipients = 5_000;
    public const int MaxRows = 100_000;

    public async Task<Page<CatalogItem>> SearchCatalogAsync(string? text, int skip = 0, int take = 50, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        IQueryable<CatalogItem> items = db.ItemCatalog.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(text))
        {
            string trimmed = text.Trim();
            if (int.TryParse(trimmed, out int id)) items = items.Where(i => i.ItemId == id);
            else
            {
                string pattern = "%" + AccountService.EscapeLike(trimmed) + "%";
                items = items.Where(i => EF.Functions.ILike(i.NameEn!, pattern, "\\") || EF.Functions.ILike(i.NameKo!, pattern, "\\"));
            }
        }
        take = Math.Clamp(take, 1, 200);
        skip = Math.Max(0, skip);
        return new(await items.OrderBy(i => i.ItemId).Skip(skip).Take(take).ToListAsync(token), await items.CountAsync(token), skip, take);
    }

    public async Task<CatalogItem?> GetCatalogAsync(int id, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.ItemCatalog.AsNoTracking().FirstOrDefaultAsync(i => i.ItemId == id, token);
    }

    /// <summary>Adds or updates catalog rows (names, description, icon); never deletes. Audited once per import.</summary>
    public async Task<CatalogImportResult> ImportCatalogAsync(IReadOnlyList<CatalogImportRow> rows, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (rows.Count == 0) throw new GameDataException(GameDataError.Invalid, "The catalog file has no items");
        if (rows.Select(r => r.Id).Distinct().Count() != rows.Count || rows.Any(r => r.Id < 0))
            throw new GameDataException(GameDataError.Invalid, "Item ids must be unique and not negative");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return new(0, 0, rows.Count);
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        Dictionary<int, CatalogItem> existing = await db.ItemCatalog.ToDictionaryAsync(i => i.ItemId, token);
        int added = 0, updated = 0;
        foreach (CatalogImportRow row in rows)
        {
            string? name = Clip(row.NameEn, 200), korean = Clip(row.NameKo, 200), description = Clip(row.DescriptionEn, 4000), icon = Clip(row.Icon, 128);
            if (icon is not null && !ValidIcon(icon)) icon = null;
            if (!existing.TryGetValue(row.Id, out CatalogItem? item))
            {
                db.ItemCatalog.Add(new CatalogItem { ItemId = row.Id, NameEn = name, NameKo = korean, DescriptionEn = description, Icon = icon });
                added++;
            }
            else if (item.NameEn != name || item.NameKo != korean || item.DescriptionEn != description || item.Icon != icon)
            {
                (item.NameEn, item.NameKo, item.DescriptionEn, item.Icon) = (name, korean, description, icon);
                updated++;
            }
        }
        Audited.Record(db, context, "items.catalog_import", "item_catalog", "all", new { Before = existing.Count },
            new { Rows = rows.Count, Added = added, Updated = updated }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return new(added, updated, rows.Count - added - updated);
    }

    /// <summary>Icon keys become file names under the catalog's icons folder: lower-case letters, digits, '_', '-', '.' only.</summary>
    public static bool ValidIcon(string icon) =>
        icon.Length is > 0 and <= 128 && !icon.StartsWith('.') && icon.All(c => c is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '_' or '-' or '.');

    public async Task<IReadOnlyList<ItemBundle>> BundlesAsync(bool includeDisabled = true, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.ItemBundles.AsNoTracking().Include(b => b.Entries.OrderBy(x => x.Line))
            .Where(b => includeDisabled || b.Enabled).OrderBy(b => b.Name).ToListAsync(token);
    }

    public async Task<ItemBundle> CreateBundleAsync(string name, string? description, IReadOnlyList<ItemLine> lines, OperatorContext context,
        CancellationToken token = default)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return await db.ItemBundles.AsNoTracking().Include(b => b.Entries).FirstAsync(b => b.Name == name.Trim(), token);
        await ValidateLinesAsync(db, lines, token);
        DateTimeOffset now = clock.GetUtcNow();
        ItemBundle bundle = new()
        {
            Name = ValidName(name), Description = Clip(description?.Trim(), 1000), CreatedBy = context.Operator, CreatedAt = now, UpdatedAt = now,
            Entries = lines.Select((l, i) => new ItemBundleEntry { Line = (short)i, ItemId = l.ItemId, Quantity = l.Quantity }).ToList()
        };
        db.ItemBundles.Add(bundle);
        await Audited.SaveAsync(db, token);
        Audited.Record(db, context, "items.bundle_create", "item_bundle", bundle.Id.ToString(), null, Summary(bundle), clock);
        await Audited.SaveAsync(db, token);
        return bundle;
    }

    public async Task<ItemBundle> UpdateBundleAsync(long id, uint expectedVersion, string name, string? description, bool enabled,
        IReadOnlyList<ItemLine> lines, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        ItemBundle bundle = await db.ItemBundles.Include(b => b.Entries).FirstOrDefaultAsync(b => b.Id == id, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Bundle not found");
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return bundle;
        await ValidateLinesAsync(db, lines, token);
        db.Entry(bundle).Property(b => b.Version).OriginalValue = expectedVersion;
        object before = Summary(bundle);
        (bundle.Name, bundle.Description, bundle.Enabled, bundle.UpdatedAt) = (ValidName(name), Clip(description?.Trim(), 1000), enabled, clock.GetUtcNow());
        db.ItemBundleEntries.RemoveRange(bundle.Entries);
        bundle.Entries = lines.Select((l, i) => new ItemBundleEntry { BundleId = id, Line = (short)i, ItemId = l.ItemId, Quantity = l.Quantity }).ToList();
        Audited.Record(db, context, "items.bundle_update", "item_bundle", id.ToString(), before, Summary(bundle), clock);
        await Audited.SaveAsync(db, token);
        return bundle;
    }

    /// <summary>
    /// Queues one row per recipient and item stack (bundle lines first, then extra lines). All recipients must exist, or
    /// nothing is queued. The request id becomes the batch id, so a retried request queues nothing twice.
    /// </summary>
    public async Task<DeliveryResult> SendAsync(DeliveryRequest request, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
        {
            List<ItemDelivery> previous = await db.ItemDeliveries.AsNoTracking().Where(d => d.BatchId == context.RequestId).ToListAsync(token);
            return new(context.RequestId, previous.Select(d => (d.AccountId, d.CharacterId)).Distinct().Count(), previous.Count);
        }
        List<ItemLine> lines = [];
        long? bundleId = null;
        if (request.BundleId is { } id)
        {
            ItemBundle bundle = await db.ItemBundles.AsNoTracking().Include(b => b.Entries).FirstOrDefaultAsync(b => b.Id == id, token)
                ?? throw new GameDataException(GameDataError.NotFound, "Bundle not found");
            if (!bundle.Enabled) throw new GameDataException(GameDataError.Invalid, "The bundle is disabled");
            lines.AddRange(bundle.Entries.OrderBy(x => x.Line).Select(x => new ItemLine(x.ItemId, x.Quantity)));
            bundleId = id;
        }
        lines.AddRange(request.Items ?? []);
        if (lines.Count == 0) throw new GameDataException(GameDataError.Invalid, "Choose a bundle or at least one item");
        await ValidateLinesAsync(db, lines, token, MaxLines * 2);
        string? message = Clip(request.Message?.Trim(), 200);

        List<(long Account, long? Character)> recipients;
        if (request.Target == DeliveryTarget.Account && request.AllAccounts)
        {
            recipients = (await db.Accounts.AsNoTracking().Where(a => a.Status == AccountStatus.Active && a.LoginName != AccountService.LocalAccount)
                .OrderBy(a => a.Id).Select(a => a.Id).ToListAsync(token)).Select(a => (a, (long?)null)).ToList();
        }
        else
        {
            string[] names = (request.Recipients ?? []).Select(n => n.Trim()).Where(n => n.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (names.Length == 0) throw new GameDataException(GameDataError.Invalid, "Give at least one recipient");
            if (names.Length > MaxRecipients) throw new GameDataException(GameDataError.Invalid, $"At most {MaxRecipients} recipients per send");
            if (request.Target == DeliveryTarget.Character)
            {
                var found = await db.Characters.AsNoTracking().Where(c => c.DeletedAt == null && names.Contains(c.Name))
                    .Select(c => new { c.Id, c.AccountId, c.Name }).ToListAsync(token);
                Missing(names, found.Select(f => f.Name));
                recipients = found.OrderBy(f => f.Id).Select(f => (f.AccountId, (long?)f.Id)).ToList();
            }
            else
            {
                var found = await db.Accounts.AsNoTracking().Where(a => names.Contains(a.LoginName))
                    .Select(a => new { a.Id, a.LoginName }).ToListAsync(token);
                Missing(names, found.Select(f => f.LoginName));
                recipients = found.OrderBy(f => f.Id).Select(f => (f.Id, (long?)null)).ToList();
            }
        }
        if (recipients.Count == 0) throw new GameDataException(GameDataError.Invalid, "No recipients");
        if ((long)recipients.Count * lines.Count > MaxRows)
            throw new GameDataException(GameDataError.Invalid, $"Too many deliveries in one send (at most {MaxRows} rows)");

        DateTimeOffset now = clock.GetUtcNow();
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        foreach ((long account, long? character) in recipients)
            foreach (ItemLine line in lines)
                db.ItemDeliveries.Add(new ItemDelivery
                {
                    BatchId = context.RequestId, Target = request.Target, AccountId = account, CharacterId = character,
                    ItemId = line.ItemId, Quantity = line.Quantity, BundleId = bundleId, Message = message,
                    CreatedBy = context.Operator, Reason = context.Reason, CreatedAt = now
                });
        Audited.Record(db, context, "items.send", "item_delivery_batch", context.RequestId.ToString(), null,
            new { request.Target, Recipients = recipients.Count, request.AllAccounts, BundleId = bundleId, Lines = lines, Message = message }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return new(context.RequestId, recipients.Count, recipients.Count * lines.Count);
    }

    public async Task<Page<DeliveryView>> DeliveriesAsync(DeliveryStatus? status = null, Guid? batchId = null, string? recipient = null,
        int skip = 0, int take = 50, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        var rows = from d in db.ItemDeliveries.AsNoTracking()
                   join a in db.Accounts on d.AccountId equals a.Id
                   join c in db.Characters on d.CharacterId equals c.Id into chars
                   from c in chars.DefaultIfEmpty()
                   join i in db.ItemCatalog on d.ItemId equals i.ItemId
                   select new { d, a.LoginName, CharacterName = c == null ? null : c.Name, i.NameEn, i.NameKo, i.Icon };
        if (status is { } s) rows = rows.Where(r => r.d.Status == s);
        if (batchId is { } b) rows = rows.Where(r => r.d.BatchId == b);
        if (!string.IsNullOrWhiteSpace(recipient))
        {
            string name = recipient.Trim();
            rows = rows.Where(r => r.LoginName == name || r.CharacterName == name);
        }
        take = Math.Clamp(take, 1, 200);
        skip = Math.Max(0, skip);
        var page = await rows.OrderByDescending(r => r.d.Id).Skip(skip).Take(take).ToListAsync(token);
        return new(page.Select(r => new DeliveryView(r.d.Id, r.d.BatchId, r.d.Target, r.d.AccountId, r.LoginName, r.d.CharacterId,
            r.CharacterName, r.d.ItemId, r.NameEn ?? r.NameKo, r.Icon, r.d.Quantity, r.d.BundleId, r.d.Message, r.d.Status,
            r.d.CreatedBy, r.d.Reason, r.d.CreatedAt, r.d.CancelledAt)).ToArray(), await rows.CountAsync(token), skip, take);
    }

    /// <summary>Cancels the rows of a batch that are still pending (not yet handed to the game).</summary>
    public async Task<int> CancelBatchAsync(Guid batchId, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return 0;
        List<ItemDelivery> pending = await db.ItemDeliveries.Where(d => d.BatchId == batchId && d.Status == DeliveryStatus.Pending).ToListAsync(token);
        if (pending.Count == 0) throw new GameDataException(GameDataError.NotFound, "No pending deliveries in this batch");
        DateTimeOffset now = clock.GetUtcNow();
        foreach (ItemDelivery delivery in pending)
            (delivery.Status, delivery.CancelledAt, delivery.CancelledBy) = (DeliveryStatus.Cancelled, now, context.Operator);
        Audited.Record(db, context, "items.cancel", "item_delivery_batch", batchId.ToString(), new { Pending = pending.Count }, new { Cancelled = pending.Count }, clock);
        await Audited.SaveAsync(db, token);
        return pending.Count;
    }

    private static async Task ValidateLinesAsync(GameDbContext db, IReadOnlyList<ItemLine> lines, CancellationToken token, int maxLines = MaxLines)
    {
        if (lines.Count == 0) throw new GameDataException(GameDataError.Invalid, "Add at least one item");
        if (lines.Count > maxLines) throw new GameDataException(GameDataError.Invalid, $"At most {maxLines} items");
        if (lines.Any(l => l.Quantity is < 1 or > MaxQuantity))
            throw new GameDataException(GameDataError.Invalid, $"Quantity must be 1–{MaxQuantity}");
        int[] ids = lines.Select(l => l.ItemId).Distinct().ToArray();
        HashSet<int> known = (await db.ItemCatalog.Where(i => ids.Contains(i.ItemId)).Select(i => i.ItemId).ToListAsync(token)).ToHashSet();
        int[] unknown = ids.Where(i => !known.Contains(i)).ToArray();
        if (unknown.Length > 0)
            throw new GameDataException(GameDataError.NotFound, "Unknown item id(s) (import the catalog first?): " + string.Join(", ", unknown.Take(20)));
    }

    private static void Missing(IEnumerable<string> wanted, IEnumerable<string> found)
    {
        HashSet<string> have = found.ToHashSet(StringComparer.OrdinalIgnoreCase);
        string[] missing = wanted.Where(n => !have.Contains(n)).ToArray();
        if (missing.Length > 0)
            throw new GameDataException(GameDataError.NotFound, "Not found: " + string.Join(", ", missing.Take(20)) + (missing.Length > 20 ? $" (+{missing.Length - 20})" : ""));
    }

    private static string ValidName(string name) =>
        name?.Trim() is { Length: > 0 and <= 100 } trimmed ? trimmed : throw new GameDataException(GameDataError.Invalid, "Bundle name must be 1–100 characters");

    private static string? Clip(string? text, int max) => string.IsNullOrEmpty(text) ? null : text.Length <= max ? text : text[..max];

    private static object Summary(ItemBundle b) =>
        new { b.Name, b.Description, b.Enabled, Lines = b.Entries.OrderBy(x => x.Line).Select(x => new { x.ItemId, x.Quantity }).ToArray() };
}
