using C9.GameData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

// Generating migrations does not need a running database; "dotnet ef database update" does.
internal sealed class DesignTimeFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        string connection = Environment.GetEnvironmentVariable(GameDataServiceCollectionExtensions.ConnectionVariable)
            ?? "Host=127.0.0.1;Port=5432;Database=c9;Username=c9";
        DbContextOptionsBuilder<GameDbContext> options = new();
        GameDataServiceCollectionExtensions.Configure(options, new NpgsqlDataSourceBuilder(connection).Build());
        options.UseNpgsql(o => o.MigrationsAssembly(typeof(GameDbContext).Assembly.GetName().Name));
        return new GameDbContext(options.Options);
    }
}

internal static class Program
{
    private static int Main()
    {
        Console.WriteLine("Design-time host for C9.GameData migrations. See docs/GAME_DATA_TH.md.");
        return 0;
    }
}
