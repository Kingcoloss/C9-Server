using System.Diagnostics;
using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

// Usage: C9.GameData.Validation [--offline]
// Database cases create a throw-away database from C9_DB_ADMIN_CONNECTION and drop it afterwards.
return await GameDataValidation.RunAsync(args);

internal static class Check
{
    public static long Assertions;
    public static void True(bool value, string message = "Assertion failed")
    {
        Interlocked.Increment(ref Assertions);
        if (!value) throw new InvalidOperationException(message);
    }
    public static void Equal<T>(T expected, T actual) => True(EqualityComparer<T>.Default.Equals(expected, actual), $"Expected {expected}, got {actual}");
    public static T Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T error) { Interlocked.Increment(ref Assertions); return error; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}");
    }
    public static async Task<GameDataException> Fails(GameDataError code, Func<Task> action)
    {
        try { await action(); }
        catch (GameDataException error)
        {
            True(error.Code == code, $"Expected {code}, got {error.Code}: {error.Message}");
            return error;
        }
        throw new InvalidOperationException($"Expected GameDataException {code}");
    }
}

internal sealed class ManualClock(DateTimeOffset now) : TimeProvider
{
    public DateTimeOffset Now { get; set; } = now;
    public override DateTimeOffset GetUtcNow() => Now;
}

internal sealed record Harness(ServiceProvider Services, ManualClock Clock)
{
    public AccountService Accounts => Services.GetRequiredService<AccountService>();
    public SanctionService Sanctions => Services.GetRequiredService<SanctionService>();
    public CharacterService Characters => Services.GetRequiredService<CharacterService>();
    public WorldService World => Services.GetRequiredService<WorldService>();
    public LegacyImporter Importer => Services.GetRequiredService<LegacyImporter>();
    public StatService Stats => Services.GetRequiredService<StatService>();
    public GameDbContext Db() => Services.GetRequiredService<IDbContextFactory<GameDbContext>>().CreateDbContext();
    public static OperatorContext Op(string reason = "validation") => new("validation", reason, Guid.NewGuid());
}

internal static partial class GameDataValidation
{
    internal static readonly byte[] Appearance = Convert.FromBase64String("AQAAAAEAAQABAH+Af39/f4B/f39/f39/f39/AAB/cgAAAAEBAAABAIBrsgA=");

    public static async Task<int> RunAsync(string[] args)
    {
        bool offline = args is ["--offline"];
        List<(string Name, Func<Task> Run)> cases = [.. OfflineCases()];
        string? admin = Environment.GetEnvironmentVariable("C9_DB_ADMIN_CONNECTION");
        string? database = null;
        ServiceProvider? services = null;
        int skipped = 0;
        if (!offline && admin is not null)
        {
            database = "c9_validation_" + Guid.NewGuid().ToString("N")[..12];
            await using (NpgsqlConnection connection = new(admin))
            {
                await connection.OpenAsync();
                // Identifier is generated here (hex only), never from input.
                await using NpgsqlCommand create = new($"CREATE DATABASE {database}", connection);
                await create.ExecuteNonQueryAsync();
            }
            ManualClock clock = new(new DateTimeOffset(2026, 11, 15, 6, 0, 0, TimeSpan.Zero));
            services = new ServiceCollection()
                .AddC9GameData(new NpgsqlConnectionStringBuilder(admin) { Database = database }.ConnectionString)
                .AddSingleton<TimeProvider>(clock)
                .BuildServiceProvider();
            Harness harness = new(services, clock);
            cases.AddRange(DatabaseCases(harness));
        }
        else skipped = DatabaseCaseCount;

        int failed = 0;
        Stopwatch watch = Stopwatch.StartNew();
        foreach (var (name, run) in cases)
        {
            try { await run(); Console.WriteLine($"PASS {name}"); }
            catch (Exception error) { failed++; Console.WriteLine($"FAIL {name}: {error.GetType().Name}: {error.Message}"); }
        }
        if (services is not null) await services.DisposeAsync();
        if (database is not null)
        {
            NpgsqlConnection.ClearAllPools();
            await using NpgsqlConnection connection = new(admin);
            await connection.OpenAsync();
            await using NpgsqlCommand drop = new($"DROP DATABASE IF EXISTS {database} WITH (FORCE)", connection);
            await drop.ExecuteNonQueryAsync();
        }
        Console.WriteLine($"{cases.Count - failed}/{cases.Count} passed, {skipped} database cases skipped, " +
            $"{Check.Assertions} assertions, {watch.Elapsed.TotalSeconds:F1}s");
        if (skipped > 0) Console.WriteLine("Set C9_DB_ADMIN_CONNECTION to run the PostgreSQL cases.");
        return failed == 0 ? 0 : 1;
    }
}
