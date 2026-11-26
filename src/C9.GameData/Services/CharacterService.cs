using System.Text;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

/// <summary>Portable character record for export/import and snapshots (schema version 1).</summary>
public sealed record CharacterExport(int SchemaVersion, string Name, short Job, string Appearance, short Level, long Exp,
    long Gold, int? MapId, short? Submap, float? PosX, float? PosY, float? PosZ, long PlaySeconds,
    IReadOnlyList<CharacterExport.Item> Items)
{
    public sealed record Item(ItemLocation Location, short Slot, int ItemId, int Quantity, short Enchant, string Options, bool Bound, DateTimeOffset? ExpiresAt);

    public static CharacterExport From(Character c) => new(1, c.Name, c.Job, Convert.ToBase64String(c.Appearance), c.Level,
        c.Exp, c.Gold, c.MapId, c.Submap, c.PosX, c.PosY, c.PosZ, c.PlaySeconds,
        c.Items.OrderBy(i => i.Location).ThenBy(i => i.Slot)
            .Select(i => new Item(i.Location, i.Slot, i.ItemId, i.Quantity, i.Enchant, i.Options, i.Bound, i.ExpiresAt)).ToArray());
}

public sealed record CharacterChanges(short? Level = null, int? MapId = null, short? Submap = null,
    float? PosX = null, float? PosY = null, float? PosZ = null, string? Appearance = null);

public sealed class CharacterService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    /// <summary>Largest asset job code the selected Client accepts (CharacterLobby.TemplateIds has 41 entries).</summary>
    public const short MaxJob = 40;

    public static bool ValidName(string? name) => name is { Length: > 0 and <= Character.MaxNameLength } &&
        name.EnumerateRunes().All(r => Rune.IsLetterOrDigit(r) || r.Value == '_');

    public async Task<IReadOnlyList<Character>> ListAsync(long accountId, bool includeDeleted = false, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.Characters.AsNoTracking()
            .Where(c => c.AccountId == accountId && (includeDeleted || c.DeletedAt == null))
            .OrderBy(c => c.DeletedAt != null).ThenBy(c => c.Slot).ToListAsync(token);
    }

    public async Task<Character?> GetAsync(long id, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.Characters.AsNoTracking().Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id, token);
    }

    /// <summary>Lobby create (0BBB). Idempotent for an identical retry, like CharacterLobby.Create.</summary>
    public async Task<Character> CreateAsync(long accountId, short slot, short job, string name, byte[] appearance, CancellationToken token = default)
    {
        Validate(job, name, appearance);
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        Account account = await db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == accountId, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Account not found");
        if (slot < 0 || slot >= account.SlotCount)
            throw new GameDataException(GameDataError.Invalid, $"Slot must be 0–{account.SlotCount - 1}");
        Character? existing = await db.Characters.AsNoTracking()
            .FirstOrDefaultAsync(c => c.AccountId == accountId && c.Slot == slot && c.DeletedAt == null, token);
        if (existing is not null && existing.Job == job && existing.Name == name && existing.Appearance.AsSpan().SequenceEqual(appearance))
            return existing;
        if (existing is not null || await db.Characters.AnyAsync(c => c.Name == name && c.DeletedAt == null, token))
            throw new GameDataException(GameDataError.Duplicate, "Character name or slot is already in use");
        Character character = new()
        {
            AccountId = accountId, Slot = slot, Job = job, Name = name, Appearance = appearance, CreatedAt = clock.GetUtcNow()
        };
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        db.Characters.Add(character);
        await Audited.SaveAsync(db, token);
        // The starter bundle (Items management) waits in the new character's Gift Box.
        await ItemService.QueueStarterAsync(db, character, clock, token);
        await transaction.CommitAsync(token);
        return character;
    }

    public Task<Character> RenameAsync(long id, uint expectedVersion, string newName, OperatorContext context, CancellationToken token = default)
    {
        if (!ValidName(newName))
            throw new GameDataException(GameDataError.Invalid, "Name must be 1–20 letters, digits or _");
        return MutateAsync(id, expectedVersion, context, "character.rename", async (db, c) =>
        {
            if (await db.Characters.AnyAsync(o => o.Id != id && o.Name == newName && o.DeletedAt == null, token))
                throw new GameDataException(GameDataError.Duplicate, "Name is already in use");
            c.Name = newName;
        }, token);
    }

    public Task<Character> MoveSlotAsync(long id, uint expectedVersion, short newSlot, OperatorContext context, CancellationToken token = default) =>
        MutateAsync(id, expectedVersion, context, "character.move_slot", async (db, c) =>
        {
            short slots = await db.Accounts.Where(a => a.Id == c.AccountId).Select(a => a.SlotCount).FirstAsync(token);
            if (newSlot < 0 || newSlot >= slots)
                throw new GameDataException(GameDataError.Invalid, $"Slot must be 0–{slots - 1}");
            if (await db.Characters.AnyAsync(o => o.AccountId == c.AccountId && o.Slot == newSlot && o.DeletedAt == null && o.Id != id, token))
                throw new GameDataException(GameDataError.Duplicate, "Target slot is occupied");
            c.Slot = newSlot;
        }, token);

    /// <summary>Edits fields that are safe while the gameplay systems that own them do not exist yet.</summary>
    public Task<Character> UpdateAsync(long id, uint expectedVersion, CharacterChanges changes, OperatorContext context, CancellationToken token = default) =>
        MutateAsync(id, expectedVersion, context, "character.update", (_, c) =>
        {
            if (changes.Level is { } level)
            {
                if (level is < 1 or > 999) throw new GameDataException(GameDataError.Invalid, "Level must be 1–999");
                c.Level = level;
            }
            if (changes.Appearance is { } appearance)
            {
                byte[] bytes = DecodeAppearance(appearance);
                c.Appearance = bytes;
            }
            if (changes.MapId is not null || changes.Submap is not null || changes.PosX is not null)
            {
                float?[] position = [changes.PosX, changes.PosY, changes.PosZ];
                if (changes.MapId is null || changes.Submap is null || position.Any(p => p is null || !float.IsFinite(p.Value) || MathF.Abs(p.Value) > 1e7f))
                    throw new GameDataException(GameDataError.Invalid, "Map, sub-map and three finite coordinates are required together");
                (c.MapId, c.Submap, c.PosX, c.PosY, c.PosZ) = (changes.MapId, changes.Submap, changes.PosX, changes.PosY, changes.PosZ);
            }
            return Task.CompletedTask;
        }, token);

    /// <summary>
    /// Game server write (c9-lobby): the last position the client reported in the world, plus the play time since the
    /// previous save. It runs every few seconds of play, so it keeps no snapshot or audit row and does not take the
    /// operator's expected version (a Portal edit made meanwhile gets the usual version conflict). Only a live character
    /// of <paramref name="accountId"/> is written; false when none matched.
    /// </summary>
    public async Task<bool> SaveWorldPositionAsync(long id, long accountId, int mapId, short submap, float x, float y, float z,
        long playSeconds, CancellationToken token = default)
    {
        if (mapId is < 1 or > 65535 || submap < 0 || playSeconds < 0 || !ValidCoordinate(x) || !ValidCoordinate(y) || !ValidCoordinate(z))
            throw new GameDataException(GameDataError.Invalid, "Map 1–65535, sub-map >= 0, three finite coordinates and play seconds >= 0 are required");
        DateTimeOffset now = clock.GetUtcNow();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        int rows = await db.Characters
            .Where(c => c.Id == id && c.AccountId == accountId && c.DeletedAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.MapId, (int?)mapId)
                .SetProperty(c => c.Submap, (short?)submap)
                .SetProperty(c => c.PosX, (float?)x)
                .SetProperty(c => c.PosY, (float?)y)
                .SetProperty(c => c.PosZ, (float?)z)
                .SetProperty(c => c.PlaySeconds, c => c.PlaySeconds + playSeconds)
                .SetProperty(c => c.LastPlayedAt, (DateTimeOffset?)now), token);
        return rows == 1;
    }

    private static bool ValidCoordinate(float value) => float.IsFinite(value) && MathF.Abs(value) <= 1e7f;

    public Task<Character> SoftDeleteAsync(long id, uint expectedVersion, OperatorContext context, CancellationToken token = default) =>
        MutateAsync(id, expectedVersion, context, "character.delete", (_, c) =>
        {
            if (c.DeletedAt is not null) throw new GameDataException(GameDataError.Invalid, "Character is already deleted");
            c.DeletedAt = clock.GetUtcNow();
            return Task.CompletedTask;
        }, token, allowDeleted: false);

    public Task<Character> RestoreAsync(long id, uint expectedVersion, OperatorContext context, CancellationToken token = default) =>
        MutateAsync(id, expectedVersion, context, "character.restore", async (db, c) =>
        {
            if (c.DeletedAt is null) throw new GameDataException(GameDataError.Invalid, "Character is not deleted");
            if (await db.Characters.AnyAsync(o => o.AccountId == c.AccountId && o.Slot == c.Slot && o.DeletedAt == null, token))
                throw new GameDataException(GameDataError.Duplicate, "The original slot is occupied; move the other character first");
            if (await db.Characters.AnyAsync(o => o.Name == c.Name && o.DeletedAt == null, token))
                throw new GameDataException(GameDataError.Duplicate, "The name is now used by another character; rename first");
            c.DeletedAt = null;
        }, token, allowDeleted: true);

    public async Task<CharacterExport> ExportAsync(long id, CancellationToken token = default) =>
        CharacterExport.From(await GetAsync(id, token) ?? throw new GameDataException(GameDataError.NotFound, "Character not found"));

    public async Task<Character> ImportAsync(long accountId, short slot, CharacterExport data, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (data.SchemaVersion != 1) throw new GameDataException(GameDataError.Invalid, "Unsupported export schema version");
        byte[] appearance = DecodeAppearance(data.Appearance);
        Validate(data.Job, data.Name, appearance);
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return await db.Characters.AsNoTracking().FirstAsync(c => c.Name == data.Name && c.DeletedAt == null, token);
        short slots = await db.Accounts.Where(a => a.Id == accountId).Select(a => (short?)a.SlotCount).FirstOrDefaultAsync(token)
            ?? throw new GameDataException(GameDataError.NotFound, "Account not found");
        if (slot < 0 || slot >= slots) throw new GameDataException(GameDataError.Invalid, $"Slot must be 0–{slots - 1}");
        Character character = new()
        {
            AccountId = accountId, Slot = slot, Name = data.Name, Job = data.Job, Appearance = appearance,
            Level = data.Level, Exp = data.Exp, Gold = data.Gold, MapId = data.MapId, Submap = data.Submap,
            PosX = data.PosX, PosY = data.PosY, PosZ = data.PosZ, PlaySeconds = data.PlaySeconds, CreatedAt = clock.GetUtcNow(),
            Items = data.Items.Select(i => new CharacterItem
            {
                Serial = Guid.CreateVersion7(), Location = i.Location, Slot = i.Slot, ItemId = i.ItemId, Quantity = i.Quantity,
                Enchant = i.Enchant, Options = i.Options, Bound = i.Bound, ExpiresAt = i.ExpiresAt
            }).ToList()
        };
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        db.Characters.Add(character);
        await Audited.SaveAsync(db, token);
        Audited.Record(db, context, "character.import", "character", character.Id.ToString(), null, data with { Items = [] }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return character;
    }

    public async Task<IReadOnlyList<CharacterSnapshot>> SnapshotsAsync(long id, int take = 50, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.CharacterSnapshots.AsNoTracking().Where(s => s.CharacterId == id)
            .OrderByDescending(s => s.TakenAt).Take(Math.Clamp(take, 1, 200)).ToListAsync(token);
    }

    /// <summary>
    /// Shared write path: optimistic concurrency on xmin, snapshot of the previous state, audit row,
    /// all committed in one SaveChanges (single transaction).
    /// </summary>
    private async Task<Character> MutateAsync(long id, uint expectedVersion, OperatorContext context, string action,
        Func<GameDbContext, Character, Task> change, CancellationToken token, bool allowDeleted = false)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        Character character = await db.Characters.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Character not found");
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return character;
        if (character.DeletedAt is not null && !allowDeleted && action != "character.delete")
            throw new GameDataException(GameDataError.Invalid, "Restore the character before editing it");
        db.Entry(character).Property(c => c.Version).OriginalValue = expectedVersion;
        CharacterExport before = CharacterExport.From(character);
        await change(db, character);
        db.CharacterSnapshots.Add(new CharacterSnapshot
        {
            CharacterId = id, TakenAt = clock.GetUtcNow(), Cause = action, Data = Json.Of(before)
        });
        Audited.Record(db, context, action, "character", id.ToString(), Summary(before, character.Slot, null),
            Summary(CharacterExport.From(character), character.Slot, character.DeletedAt), clock);
        await Audited.SaveAsync(db, token);
        return character;
    }

    private static object Summary(CharacterExport c, short slot, DateTimeOffset? deletedAt) =>
        new { c.Name, Slot = slot, c.Job, c.Level, c.MapId, c.Submap, c.PosX, c.PosY, c.PosZ, c.Appearance, DeletedAt = deletedAt };

    private static void Validate(short job, string name, byte[] appearance)
    {
        if (job is < 1 or > MaxJob) throw new GameDataException(GameDataError.Invalid, $"Job must be 1–{MaxJob}");
        if (!ValidName(name)) throw new GameDataException(GameDataError.Invalid, "Name must be 1–20 letters, digits or _");
        if (appearance is not { Length: Character.AppearanceBytes })
            throw new GameDataException(GameDataError.Invalid, $"Appearance must be {Character.AppearanceBytes} bytes");
    }

    private static byte[] DecodeAppearance(string base64)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            if (bytes.Length == Character.AppearanceBytes) return bytes;
        }
        catch (FormatException) { }
        throw new GameDataException(GameDataError.Invalid, $"Appearance must be base64 of {Character.AppearanceBytes} bytes");
    }

}
