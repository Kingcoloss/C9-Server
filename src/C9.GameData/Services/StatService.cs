using System.Data;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

/// <summary>
/// Stat keys the server knows. Names follow the Client's own option texts ("Strength is increased." etc. in
/// ENG_GameData.c9t). Which packet field carries each stat is decided in C9.Server, not here.
/// </summary>
public static class StatKeys
{
    public sealed record Info(string Key, string Name, int DefaultBase);

    public static readonly IReadOnlyList<Info> All =
    [
        new("strength", "Strength", 10),
        new("intelligence", "Intelligence", 10),
        new("health", "Health", 10),
        new("wisdom", "Wisdom", 10),
        new("max_hp", "Maximum HP", 100),
        new("max_mp", "Maximum MP", 100),
        new("physical_attack", "Physical Attack", 0),
        new("magic_attack", "Magic Attack", 0),
        new("physical_defense", "Physical Defense", 0),
        new("magic_defense", "Magic Defense", 0),
        new("critical_rate", "Critical Rate", 0),
        new("critical_damage", "Critical Damage", 0),
        new("critical_resistance", "Critical Resistance", 0),
        new("accuracy", "Accuracy", 0),
        new("evasion", "Evasion", 0),
        new("attack_speed", "Attack Speed", 0),
        new("move_speed", "Movement Speed", 0),
        new("cast_speed", "Casting Speed", 0),
        new("concentration", "Concentration Rate", 0),
        new("guard_limit", "Guard Limit", 0),
        new("guard_recovery", "Guard Limit Recovery", 0),
        new("piercing", "Piercing Chance", 0),
        new("fire_attack", "Fire Attack", 0),
        new("water_attack", "Water Attack", 0),
        new("earth_attack", "Earth Attack", 0),
        new("air_attack", "Air Attack", 0),
        new("fire_resistance", "Fire Resistance", 0),
        new("water_resistance", "Water Resistance", 0),
        new("earth_resistance", "Earth Resistance", 0),
        new("air_resistance", "Air Resistance", 0),
    ];

    public const int MaxKeyLength = 32;
}

public sealed record StatClass(short Code, string Name);

/// <summary>Effective stats of one character: base of its class + spent points × per-point gain.</summary>
public sealed record CharacterStatSheet(long CharacterId, string Name, short Job, StatClass Class, short Level,
    int EarnedPoints, int SpentPoints, int AvailablePoints, IReadOnlyList<CharacterStatSheet.Line> Stats)
{
    public sealed record Line(string Key, string Name, bool Upgradable, int PerPoint, int Base, int Points, int Total);

    public int Total(string key) => Stats.FirstOrDefault(s => s.Key == key)?.Total ?? 0;
}

public sealed record StatConfiguration(IReadOnlyList<StatDefinition> Stats, IReadOnlyList<StatClass> Classes,
    IReadOnlyList<ClassStatBase> Bases, IReadOnlyList<StatPointLevel> Levels);

public sealed record StatDefinitionChange(string Key, string Name, bool Upgradable, int PerPoint);

/// <summary>
/// Base stats per class, stat upgrade points per level and the points players spend in the launcher.
/// Earned points = sum of <see cref="StatPointLevel"/> up to the character's level; spending is final (no reset yet).
/// </summary>
public sealed class StatService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    public const short MaxLevel = 999;
    public const int MaxPointsPerLevel = 10_000;
    public const int MaxStatValue = 100_000_000;
    public const int MaxPerPoint = 1_000_000;

    /// <summary>Base classes in Client template order (CharacterLobby.TemplateIds templates 0..4).</summary>
    public static readonly IReadOnlyList<StatClass> Classes =
    [
        new(0, "Fighter"), new(1, "Hunter"), new(2, "Shaman"), new(3, "WitchBlade"), new(4, "Mistic")
    ];

    // Job codes come in blocks of eight per base class; CharacterLobby.TemplateIds maps the
    // first job of each block to templates 0, 1, 3, 2, 4 (C9.Validation checks this stays in sync).
    private static readonly short[] ClassOfJobBlock = [0, 1, 3, 2, 4];

    /// <summary>Base class of an asset job code (1..40).</summary>
    public static short ClassOf(short job) => job is >= 1 and <= CharacterService.MaxJob
        ? ClassOfJobBlock[(job - 1) / 8]
        : throw new GameDataException(GameDataError.Invalid, $"Job must be 1–{CharacterService.MaxJob}");

    public async Task<StatConfiguration> ConfigurationAsync(CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return new(await db.StatDefinitions.AsNoTracking().OrderBy(s => s.Sort).ToListAsync(token), Classes,
            await db.ClassStatBases.AsNoTracking().OrderBy(b => b.ClassCode).ThenBy(b => b.StatKey).ToListAsync(token),
            await db.StatPointLevels.AsNoTracking().OrderBy(l => l.Level).ToListAsync(token));
    }

    /// <summary>Changes the label, upgradability and per-point gain of existing stats.</summary>
    public async Task<IReadOnlyList<StatDefinition>> UpdateDefinitionsAsync(IReadOnlyList<StatDefinitionChange> changes,
        OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        List<StatDefinition> all = await db.StatDefinitions.OrderBy(s => s.Sort).ToListAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return all;
        if (changes.Count == 0 || changes.Select(c => c.Key).Distinct().Count() != changes.Count)
            throw new GameDataException(GameDataError.Invalid, "Send each stat at most once");
        var before = all.Select(s => new { s.Key, s.Name, s.Upgradable, s.PerPoint }).ToArray();
        foreach (StatDefinitionChange change in changes)
        {
            StatDefinition stat = all.FirstOrDefault(s => s.Key == change.Key)
                ?? throw new GameDataException(GameDataError.NotFound, $"Unknown stat {change.Key}");
            if (string.IsNullOrWhiteSpace(change.Name) || change.Name.Length > 64)
                throw new GameDataException(GameDataError.Invalid, "Stat name must be 1–64 characters");
            if (change.PerPoint is < 1 or > MaxPerPoint)
                throw new GameDataException(GameDataError.Invalid, $"Per-point gain must be 1–{MaxPerPoint}");
            (stat.Name, stat.Upgradable, stat.PerPoint) = (change.Name.Trim(), change.Upgradable, change.PerPoint);
        }
        Audited.Record(db, context, "stats.definitions", "stat_definitions", "all", before,
            all.Select(s => new { s.Key, s.Name, s.Upgradable, s.PerPoint }).ToArray(), clock);
        await Audited.SaveAsync(db, token);
        return all;
    }

    /// <summary>Replaces the base values of the given stats for one class.</summary>
    public async Task<IReadOnlyList<ClassStatBase>> SetClassBaseAsync(short classCode, IReadOnlyDictionary<string, int> values,
        OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (Classes.All(c => c.Code != classCode)) throw new GameDataException(GameDataError.NotFound, "Unknown class");
        if (values.Count == 0) throw new GameDataException(GameDataError.Invalid, "No stat values given");
        if (values.Values.Any(v => v is < 0 or > MaxStatValue))
            throw new GameDataException(GameDataError.Invalid, $"Base values must be 0–{MaxStatValue}");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        List<ClassStatBase> rows = await db.ClassStatBases.Where(b => b.ClassCode == classCode).ToListAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return rows;
        HashSet<string> known = (await db.StatDefinitions.Select(s => s.Key).ToListAsync(token)).ToHashSet();
        if (values.Keys.FirstOrDefault(k => !known.Contains(k)) is { } unknown)
            throw new GameDataException(GameDataError.NotFound, $"Unknown stat {unknown}");
        var before = rows.ToDictionary(r => r.StatKey, r => r.Value);
        foreach ((string key, int value) in values)
        {
            if (rows.FirstOrDefault(r => r.StatKey == key) is { } row) row.Value = value;
            else
            {
                ClassStatBase added = new() { ClassCode = classCode, StatKey = key, Value = value };
                db.ClassStatBases.Add(added);
                rows.Add(added);
            }
        }
        Audited.Record(db, context, "stats.class_base", "stat_class", classCode.ToString(), before,
            rows.ToDictionary(r => r.StatKey, r => r.Value), clock);
        await Audited.SaveAsync(db, token);
        return rows;
    }

    /// <summary>Replaces the whole level table. <paramref name="points"/>[i] is granted on reaching level i + 1.</summary>
    public async Task<IReadOnlyList<StatPointLevel>> SetLevelPointsAsync(IReadOnlyList<int> points, OperatorContext context,
        CancellationToken token = default)
    {
        context.Validate();
        if (points.Count is < 1 or > MaxLevel)
            throw new GameDataException(GameDataError.Invalid, $"The table must cover levels 1–N with N at most {MaxLevel}");
        if (points.Any(p => p is < 0 or > MaxPointsPerLevel))
            throw new GameDataException(GameDataError.Invalid, $"Points per level must be 0–{MaxPointsPerLevel}");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        List<StatPointLevel> rows = await db.StatPointLevels.OrderBy(l => l.Level).ToListAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return rows;
        int[] before = rows.Select(r => r.Points).ToArray();
        db.StatPointLevels.RemoveRange(rows);
        await Audited.SaveAsync(db, token);
        List<StatPointLevel> replaced = points.Select((p, i) => new StatPointLevel { Level = (short)(i + 1), Points = p }).ToList();
        db.StatPointLevels.AddRange(replaced);
        Audited.Record(db, context, "stats.level_points", "stat_point_levels", "all", before, points, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return replaced;
    }

    public async Task<IReadOnlyList<CharacterStatSheet>> ForAccountAsync(long accountId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        List<Character> characters = await db.Characters.AsNoTracking()
            .Where(c => c.AccountId == accountId && c.DeletedAt == null).OrderBy(c => c.Slot).ToListAsync(token);
        Sources sources = await SourcesAsync(db, characters.Select(c => c.Id).ToArray(), token);
        return characters.Select(c => Sheet(c, sources)).ToArray();
    }

    /// <summary>Stat sheet the game server applies when the character enters the world.</summary>
    public async Task<CharacterStatSheet?> ForCharacterAsync(long characterId, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        Character? character = await db.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == characterId && c.DeletedAt == null, token);
        return character is null ? null : Sheet(character, await SourcesAsync(db, [characterId], token));
    }

    /// <summary>
    /// Spends points of an account's own character. Serialised per character with a transaction-scoped advisory lock,
    /// so two launcher requests cannot spend the same points twice. Idempotent on <see cref="OperatorContext.RequestId"/>.
    /// </summary>
    public async Task<CharacterStatSheet> SpendAsync(long accountId, long characterId, IReadOnlyDictionary<string, int> add,
        OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (add.Count == 0 || add.Values.Any(v => v is < 0 or > MaxPointsPerLevel * MaxLevel) || add.Values.All(v => v == 0))
            throw new GameDataException(GameDataError.Invalid, "Give a positive number of points for at least one stat");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({LockSpace}, {(int)(characterId % int.MaxValue)})", token);
        Character character = await db.Characters.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == characterId && c.AccountId == accountId && c.DeletedAt == null, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Character not found");
        Sources sources = await SourcesAsync(db, [characterId], token);
        CharacterStatSheet before = Sheet(character, sources);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return before;
        foreach (string key in add.Keys)
        {
            StatDefinition stat = sources.Definitions.FirstOrDefault(s => s.Key == key)
                ?? throw new GameDataException(GameDataError.NotFound, $"Unknown stat {key}");
            if (!stat.Upgradable && add[key] > 0)
                throw new GameDataException(GameDataError.Forbidden, $"{stat.Name} cannot be upgraded");
        }
        long wanted = add.Values.Sum(v => (long)v);
        if (wanted > before.AvailablePoints)
            throw new GameDataException(GameDataError.Invalid, $"Only {before.AvailablePoints} points are available");
        List<CharacterStat> spent = await db.CharacterStats.Where(s => s.CharacterId == characterId).ToListAsync(token);
        foreach ((string key, int value) in add.Where(a => a.Value > 0))
        {
            if (spent.FirstOrDefault(s => s.StatKey == key) is { } row) row.Points += value;
            else db.CharacterStats.Add(new CharacterStat { CharacterId = characterId, StatKey = key, Points = value });
        }
        Audited.Record(db, context, "character.stat_spend", "character", characterId.ToString(),
            new { before.SpentPoints, before.AvailablePoints }, new { add, SpentPoints = before.SpentPoints + wanted }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return Sheet(character, await SourcesAsync(db, [characterId], token));
    }

    /// <summary>Advisory lock namespace for stat spending ("C9ST").</summary>
    private const int LockSpace = 0x43395354;

    private sealed record Sources(IReadOnlyList<StatDefinition> Definitions, IReadOnlyList<ClassStatBase> Bases,
        IReadOnlyList<StatPointLevel> Levels, ILookup<long, CharacterStat> Spent);

    private static async Task<Sources> SourcesAsync(GameDbContext db, long[] characterIds, CancellationToken token) => new(
        await db.StatDefinitions.AsNoTracking().OrderBy(s => s.Sort).ToListAsync(token),
        await db.ClassStatBases.AsNoTracking().ToListAsync(token),
        await db.StatPointLevels.AsNoTracking().OrderBy(l => l.Level).ToListAsync(token),
        (await db.CharacterStats.AsNoTracking().Where(s => characterIds.Contains(s.CharacterId)).ToListAsync(token))
            .ToLookup(s => s.CharacterId));

    private static CharacterStatSheet Sheet(Character character, Sources sources)
    {
        short classCode = ClassOf(character.Job);
        int earned = (int)Math.Min(int.MaxValue, sources.Levels.Where(l => l.Level <= character.Level).Sum(l => (long)l.Points));
        Dictionary<string, int> spent = sources.Spent[character.Id].ToDictionary(s => s.StatKey, s => s.Points);
        int spentTotal = (int)Math.Min(int.MaxValue, spent.Values.Sum(v => (long)v));
        CharacterStatSheet.Line[] lines = sources.Definitions.Select(d =>
        {
            int baseValue = sources.Bases.FirstOrDefault(b => b.ClassCode == classCode && b.StatKey == d.Key)?.Value ?? 0;
            int points = spent.GetValueOrDefault(d.Key);
            int total = (int)Math.Clamp(baseValue + (long)points * d.PerPoint, int.MinValue, int.MaxValue);
            return new CharacterStatSheet.Line(d.Key, d.Name, d.Upgradable, d.PerPoint, baseValue, points, total);
        }).ToArray();
        return new(character.Id, character.Name, character.Job, Classes[classCode], character.Level,
            earned, spentTotal, Math.Max(0, earned - spentTotal), lines);
    }
}
