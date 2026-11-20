using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace C9.GameData;

public static class GameDataServiceCollectionExtensions
{
    /// <summary>Environment variable shared with compose.yaml (service c9-db, profile "gamedata").</summary>
    public const string ConnectionVariable = "C9_DB_CONNECTION";

    /// <summary>
    /// Registers the pooled DbContext factory, a shared NpgsqlDataSource (for LISTEN/NOTIFY and COPY) and the
    /// GameData services. Pass the connection string from configuration or <see cref="ConnectionVariable"/>.
    /// </summary>
    public static IServiceCollection AddC9GameData(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException($"A PostgreSQL connection string is required ({ConnectionVariable})", nameof(connectionString));
        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(connectionString)
        {
            Name = "c9-gamedata"
        }.Build();
        services.AddSingleton(dataSource);
        services.AddPooledDbContextFactory<GameDbContext>(options => Configure(options, dataSource));
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AccountService>();
        services.AddSingleton<SanctionService>();
        services.AddSingleton<CharacterService>();
        services.AddSingleton<SecondPasswordService>();
        services.AddSingleton<WorldService>();
        services.AddSingleton<StatService>();
        services.AddSingleton<ItemService>();
        services.AddSingleton<LegacyImporter>();
        return services;
    }

    public static DbContextOptionsBuilder Configure(DbContextOptionsBuilder options, NpgsqlDataSource dataSource) =>
        // No automatic retry: writes use explicit transactions, and callers retry with the same RequestId instead.
        options.UseNpgsql(dataSource, npgsql => npgsql
                .MigrationsHistoryTable("__ef_migrations_history", GameDbContext.OpsSchema))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

    /// <summary>Applies pending migrations (idempotent). The Portal host may call this at start-up.</summary>
    public static async Task MigrateGameDataAsync(this IServiceProvider services, CancellationToken token = default)
    {
        await using GameDbContext db = await services.GetRequiredService<IDbContextFactory<GameDbContext>>().CreateDbContextAsync(token);
        await db.Database.MigrateAsync(token);
    }
}
