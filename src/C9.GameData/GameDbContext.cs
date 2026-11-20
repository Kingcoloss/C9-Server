using System.Text;
using C9.GameData.Entities;
using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C9.GameData;

/// <summary>
/// Shared model for schemas "game" (player and world data) and "ops" (sessions, audit, commands).
/// Every query goes through LINQ or FromSql($"...") so values are always sent as parameters (ADR 0002).
/// </summary>
public sealed class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
    public const string GameSchema = "game";
    public const string OpsSchema = "ops";

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountSanction> AccountSanctions => Set<AccountSanction>();
    public DbSet<AccountSecondPassword> AccountSecondPasswords => Set<AccountSecondPassword>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterItem> CharacterItems => Set<CharacterItem>();
    public DbSet<CatalogItem> ItemCatalog => Set<CatalogItem>();
    public DbSet<LedgerEntry> Ledger => Set<LedgerEntry>();
    public DbSet<RatePolicy> RatePolicies => Set<RatePolicy>();
    public DbSet<AccountRateOverride> AccountRateOverrides => Set<AccountRateOverride>();
    public DbSet<WorldEvent> WorldEvents => Set<WorldEvent>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<WorldSetting> WorldSettings => Set<WorldSetting>();
    public DbSet<SessionRecord> Sessions => Set<SessionRecord>();
    public DbSet<AuditEntry> AuditLog => Set<AuditEntry>();
    public DbSet<AdminCommand> AdminCommands => Set<AdminCommand>();
    public DbSet<CharacterSnapshot> CharacterSnapshots => Set<CharacterSnapshot>();
    public DbSet<StatDefinition> StatDefinitions => Set<StatDefinition>();
    public DbSet<ClassStatBase> ClassStatBases => Set<ClassStatBase>();
    public DbSet<StatPointLevel> StatPointLevels => Set<StatPointLevel>();
    public DbSet<CharacterStat> CharacterStats => Set<CharacterStat>();
    public DbSet<ItemBundle> ItemBundles => Set<ItemBundle>();
    public DbSet<ItemBundleEntry> ItemBundleEntries => Set<ItemBundleEntry>();
    public DbSet<ItemDelivery> ItemDeliveries => Set<ItemDelivery>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<Enum>().HaveConversion<string>().HaveMaxLength(32);
        builder.Properties<decimal>().HavePrecision(12, 4);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.HasPostgresExtension("citext");

        model.Entity<Account>(e =>
        {
            e.ToTable("accounts", GameSchema);
            e.Property(a => a.LoginName).HasColumnType("citext").HasMaxLength(32);
            e.HasIndex(a => a.LoginName).IsUnique();
            e.Property(a => a.PasswordHash).HasMaxLength(256);
            e.Property(a => a.Note).HasMaxLength(2000);
            e.Property(a => a.SlotCount).HasDefaultValue((short)17);
            e.Property(a => a.CreatedAt).HasDefaultValueSql("now()");
            e.ToTable(t => t.HasCheckConstraint("ck_accounts_slot_count", "slot_count BETWEEN 1 AND 17"));
            Xmin(e);
        });

        model.Entity<AccountSanction>(e =>
        {
            e.ToTable("account_sanctions", GameSchema);
            e.HasOne<Account>().WithMany(a => a.Sanctions).HasForeignKey(s => s.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.Property(s => s.Reason).HasMaxLength(500);
            e.Property(s => s.IssuedBy).HasMaxLength(64);
            e.Property(s => s.RevokedBy).HasMaxLength(64);
            e.HasIndex(s => new { s.AccountId, s.StartsAt });
        });

        model.Entity<AccountSecondPassword>(e =>
        {
            e.ToTable("account_second_passwords", GameSchema, t =>
            {
                t.HasCheckConstraint("ck_account_second_passwords_question", "question BETWEEN 0 AND 9");
                t.HasCheckConstraint("ck_account_second_passwords_fail_count", "fail_count BETWEEN 0 AND 5");
            });
            e.HasKey(p => p.AccountId);
            e.Property(p => p.AccountId).ValueGeneratedNever();
            e.HasOne<Account>().WithOne().HasForeignKey<AccountSecondPassword>(p => p.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.Property(p => p.PasswordHash).HasMaxLength(256);
            e.Property(p => p.AnswerHash).HasMaxLength(256);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            Xmin(e);
        });

        model.Entity<Character>(e =>
        {
            e.ToTable("characters", GameSchema, t =>
            {
                t.HasCheckConstraint("ck_characters_appearance", $"length(appearance) = {Character.AppearanceBytes}");
                t.HasCheckConstraint("ck_characters_slot", "slot BETWEEN 0 AND 16");
                t.HasCheckConstraint("ck_characters_gold", "gold >= 0");
                t.HasCheckConstraint("ck_characters_level", "level >= 1");
            });
            e.HasOne(c => c.Account).WithMany(a => a.Characters).HasForeignKey(c => c.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.Property(c => c.Name).HasColumnType("citext").HasMaxLength(Character.MaxNameLength);
            e.Property(c => c.Level).HasDefaultValue((short)1);
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            // Slots and names are unique among characters that are not soft-deleted.
            e.HasIndex(c => new { c.AccountId, c.Slot }).IsUnique().HasFilter("deleted_at IS NULL");
            e.HasIndex(c => c.Name).IsUnique().HasFilter("deleted_at IS NULL");
            Xmin(e);
        });

        model.Entity<CharacterItem>(e =>
        {
            e.ToTable("character_items", GameSchema, t => t.HasCheckConstraint("ck_character_items_quantity", "quantity > 0"));
            e.HasKey(i => i.Serial);
            e.HasOne<Character>().WithMany(c => c.Items).HasForeignKey(i => i.CharacterId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(i => new { i.CharacterId, i.Location, i.Slot }).IsUnique();
            e.HasIndex(i => i.ItemId);
            e.Property(i => i.Options).HasColumnType("jsonb");
            Xmin(e);
        });

        model.Entity<CatalogItem>(e =>
        {
            e.ToTable("item_catalog", GameSchema);
            e.HasKey(i => i.ItemId);
            e.Property(i => i.ItemId).ValueGeneratedNever();
            e.Property(i => i.Raw).HasColumnType("jsonb");
            e.Property(i => i.NameEn).HasMaxLength(200);
            e.Property(i => i.NameKo).HasMaxLength(200);
            e.Property(i => i.DescriptionEn).HasMaxLength(4000);
            e.Property(i => i.Icon).HasMaxLength(128);
        });

        model.Entity<LedgerEntry>(e =>
        {
            e.ToTable("ledger", GameSchema);
            e.Property(l => l.At).HasDefaultValueSql("now()");
            e.Property(l => l.Reason).HasMaxLength(64);
            e.Property(l => l.Ref).HasMaxLength(200);
            e.HasIndex(l => l.At);
            e.HasIndex(l => new { l.CharacterId, l.At });
            e.HasIndex(l => new { l.Reason, l.At });
            e.HasIndex(l => l.RequestId).IsUnique();
        });

        model.Entity<RatePolicy>(e =>
        {
            e.ToTable("rate_policy", GameSchema);
            e.HasKey(r => r.Scope);
            e.Property(r => r.Scope).HasMaxLength(32);
            e.ComplexProperty(r => r.Rates, Rates);
            Xmin(e);
        });

        model.Entity<AccountRateOverride>(e =>
        {
            e.ToTable("account_rate_overrides", GameSchema);
            e.HasKey(r => r.AccountId);
            e.HasOne<Account>().WithOne().HasForeignKey<AccountRateOverride>(r => r.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.ComplexProperty(r => r.Rates, Rates);
            e.Property(r => r.Note).HasMaxLength(500);
            Xmin(e);
        });

        model.Entity<WorldEvent>(e =>
        {
            e.ToTable("world_events", GameSchema, t =>
            {
                t.HasCheckConstraint("ck_world_events_window", "ends_at > starts_at");
                t.HasCheckConstraint("ck_world_events_repeat", "repeat_every_days IS NULL OR repeat_every_days > 0");
            });
            e.Property(w => w.Name).HasMaxLength(100);
            e.Property(w => w.Description).HasMaxLength(2000);
            e.Property(w => w.CreatedBy).HasMaxLength(64);
            e.Property(w => w.Extra).HasColumnType("jsonb");
            e.Property(w => w.CreatedAt).HasDefaultValueSql("now()");
            e.ComplexProperty(w => w.Rates, Rates);
            e.HasIndex(w => new { w.StartsAt, w.EndsAt });
            Xmin(e);
        });

        model.Entity<Announcement>(e =>
        {
            e.ToTable("announcements", GameSchema);
            e.Property(a => a.Message).HasMaxLength(500);
            e.Property(a => a.CreatedBy).HasMaxLength(64);
            e.Property(a => a.CreatedAt).HasDefaultValueSql("now()");
        });

        model.Entity<WorldSetting>(e =>
        {
            e.ToTable("world_settings", GameSchema);
            e.HasKey(s => s.Key);
            e.Property(s => s.Key).HasMaxLength(64);
            e.Property(s => s.Value).HasColumnType("jsonb");
            Xmin(e);
        });

        model.Entity<SessionRecord>(e =>
        {
            e.ToTable("sessions", OpsSchema);
            e.HasKey(s => s.Key);
            e.Property(s => s.Key).HasMaxLength(96);
            e.Property(s => s.Service).HasMaxLength(32);
            e.Property(s => s.ClientHash).HasMaxLength(64);
            e.Property(s => s.EndReason).HasMaxLength(200);
            e.Property(s => s.LastState).HasMaxLength(32);
            e.HasIndex(s => new { s.AccountId, s.StartedAt });
            e.HasIndex(s => s.EndedAt);
        });

        model.Entity<AuditEntry>(e =>
        {
            e.ToTable("audit_log", OpsSchema);
            e.Property(a => a.At).HasDefaultValueSql("now()");
            e.Property(a => a.Operator).HasMaxLength(64);
            e.Property(a => a.Action).HasMaxLength(64);
            e.Property(a => a.TargetType).HasMaxLength(32);
            e.Property(a => a.TargetId).HasMaxLength(64);
            e.Property(a => a.Before).HasColumnType("jsonb");
            e.Property(a => a.After).HasColumnType("jsonb");
            e.Property(a => a.Reason).HasMaxLength(500);
            e.HasIndex(a => a.RequestId).IsUnique();
            e.HasIndex(a => new { a.TargetType, a.TargetId, a.At });
            e.HasIndex(a => new { a.Operator, a.At });
        });

        model.Entity<AdminCommand>(e =>
        {
            e.ToTable("admin_commands", OpsSchema);
            e.HasKey(c => c.RequestId);
            e.Property(c => c.Operator).HasMaxLength(64);
            e.Property(c => c.Kind).HasMaxLength(64);
            e.Property(c => c.Payload).HasColumnType("jsonb");
            e.Property(c => c.Result).HasColumnType("jsonb");
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.HasIndex(c => new { c.Status, c.CreatedAt });
        });

        model.Entity<CharacterSnapshot>(e =>
        {
            e.ToTable("character_snapshots", OpsSchema);
            e.Property(s => s.Cause).HasMaxLength(64);
            e.Property(s => s.Data).HasColumnType("jsonb");
            e.Property(s => s.TakenAt).HasDefaultValueSql("now()");
            e.HasIndex(s => new { s.CharacterId, s.TakenAt });
        });

        model.Entity<StatDefinition>(e =>
        {
            e.ToTable("stat_definitions", GameSchema, t => t.HasCheckConstraint("ck_stat_definitions_per_point", "per_point > 0"));
            e.HasKey(s => s.Key);
            e.Property(s => s.Key).HasMaxLength(StatKeys.MaxKeyLength);
            e.Property(s => s.Name).HasMaxLength(64);
            e.HasData(StatKeys.All.Select((s, i) => new StatDefinition { Key = s.Key, Name = s.Name, Sort = (short)i }));
        });

        model.Entity<ClassStatBase>(e =>
        {
            e.ToTable("class_stat_bases", GameSchema, t => t.HasCheckConstraint("ck_class_stat_bases_value", "value >= 0"));
            e.HasKey(b => new { b.ClassCode, b.StatKey });
            e.HasOne<StatDefinition>().WithMany().HasForeignKey(b => b.StatKey).OnDelete(DeleteBehavior.Restrict);
            e.HasData(StatService.Classes.SelectMany(c => StatKeys.All.Select(s =>
                new ClassStatBase { ClassCode = c.Code, StatKey = s.Key, Value = s.DefaultBase })));
        });

        model.Entity<StatPointLevel>(e =>
        {
            e.ToTable("stat_point_levels", GameSchema, t =>
            {
                t.HasCheckConstraint("ck_stat_point_levels_level", $"level BETWEEN 1 AND {StatService.MaxLevel}");
                t.HasCheckConstraint("ck_stat_point_levels_points", "points >= 0");
            });
            e.HasKey(l => l.Level);
            e.Property(l => l.Level).ValueGeneratedNever();
            // Default: 5 points on every level up to 95, the level cap of the current patch.
            e.HasData(Enumerable.Range(1, 95).Select(level => new StatPointLevel { Level = (short)level, Points = 5 }));
        });

        model.Entity<CharacterStat>(e =>
        {
            e.ToTable("character_stats", GameSchema, t => t.HasCheckConstraint("ck_character_stats_points", "points >= 0"));
            e.HasKey(s => new { s.CharacterId, s.StatKey });
            e.HasOne<Character>().WithMany().HasForeignKey(s => s.CharacterId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<StatDefinition>().WithMany().HasForeignKey(s => s.StatKey).OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<ItemBundle>(e =>
        {
            e.ToTable("item_bundles", GameSchema);
            e.Property(b => b.Name).HasMaxLength(100);
            e.Property(b => b.Description).HasMaxLength(1000);
            e.Property(b => b.CreatedBy).HasMaxLength(64);
            e.Property(b => b.CreatedAt).HasDefaultValueSql("now()");
            e.HasIndex(b => b.Name).IsUnique();
            Xmin(e);
        });

        model.Entity<ItemBundleEntry>(e =>
        {
            e.ToTable("item_bundle_entries", GameSchema, t => t.HasCheckConstraint("ck_item_bundle_entries_quantity", "quantity > 0"));
            e.HasKey(x => new { x.BundleId, x.Line });
            e.HasOne<ItemBundle>().WithMany(b => b.Entries).HasForeignKey(x => x.BundleId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<CatalogItem>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<ItemDelivery>(e =>
        {
            e.ToTable("item_deliveries", GameSchema, t =>
            {
                t.HasCheckConstraint("ck_item_deliveries_quantity", "quantity > 0");
                t.HasCheckConstraint("ck_item_deliveries_target", "(target = 'Character') = (character_id IS NOT NULL)");
            });
            e.HasOne<Account>().WithMany().HasForeignKey(d => d.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Character>().WithMany().HasForeignKey(d => d.CharacterId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<CatalogItem>().WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ItemBundle>().WithMany().HasForeignKey(d => d.BundleId).OnDelete(DeleteBehavior.Restrict);
            e.Property(d => d.Message).HasMaxLength(200);
            e.Property(d => d.Reason).HasMaxLength(500);
            e.Property(d => d.CreatedBy).HasMaxLength(64);
            e.Property(d => d.CancelledBy).HasMaxLength(64);
            e.Property(d => d.CreatedAt).HasDefaultValueSql("now()");
            e.HasIndex(d => d.BatchId);
            e.HasIndex(d => new { d.AccountId, d.Status });
            e.HasIndex(d => new { d.CharacterId, d.Status });
            e.HasIndex(d => d.CreatedAt);
            Xmin(e);
        });

        // Player data is never hard-deleted through a parent row (soft delete only).
        ApplySnakeCase(model);
    }

    private static void Xmin<T>(EntityTypeBuilder<T> entity) where T : class =>
        entity.Property<uint>("Version").IsRowVersion();

    private static void Rates(ComplexPropertyBuilder<RateSet> rates)
    {
        rates.Property(r => r.Exp).HasColumnName("rate_exp");
        rates.Property(r => r.Drop).HasColumnName("rate_drop");
        rates.Property(r => r.RareDrop).HasColumnName("rate_rare_drop");
        rates.Property(r => r.Upgrade).HasColumnName("rate_upgrade");
    }

    private static void ApplySnakeCase(ModelBuilder model)
    {
        foreach (var entity in model.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
                if (property.Name != "Version") property.SetColumnName(Snake(property.Name));
            foreach (var key in entity.GetKeys()) key.SetName(Snake(key.GetName()!));
            foreach (var foreignKey in entity.GetForeignKeys())
                foreignKey.SetConstraintName(Snake(foreignKey.GetConstraintName()!));
            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(Snake(index.GetDatabaseName()!));
        }
    }

    internal static string Snake(string name)
    {
        StringBuilder result = new(name.Length + 8);
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && name[i - 1] != '_' && (char.IsLower(name[i - 1]) ||
                    (i + 1 < name.Length && char.IsLower(name[i + 1])))) result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else result.Append(c);
        }
        return result.ToString();
    }
}
