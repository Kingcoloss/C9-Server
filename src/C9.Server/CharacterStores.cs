using System.Collections.Concurrent;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace C9.Server;

/// <summary>A character as the lobby packets need it. <see cref="Id"/> is what the client echoes back.</summary>
internal sealed record LobbyCharacter(ulong Id, byte Slot, ushort Job, string Name, byte[] Appearance);

/// <summary>Where the character lobby keeps characters: the local JSON file, or the game database.</summary>
internal interface ICharacterStore
{
    /// <summary>Game database account of the lobby's characters; null for the JSON store.</summary>
    long? AccountId { get; }

    /// <summary>Characters in the lobby's slot range, ordered as the list packet sends them.</summary>
    ValueTask<IReadOnlyList<LobbyCharacter>> LoadAsync(CancellationToken token);

    /// <summary>Creates a character, or returns the same one again for a retried identical request.</summary>
    ValueTask<LobbyCharacter> AddAsync(byte slot, ushort job, string name, byte[] appearance, CancellationToken token);

    /// <summary>Total of each stat (class base + spent points), or null when the store has no stat system (JSON).</summary>
    ValueTask<IReadOnlyDictionary<string, int>?> StatsAsync(ulong characterId, CancellationToken token);
}

internal static class CharacterStores
{
    public static ICharacterStore Open(ServerOptions options, IServerEvents? events = null, long? accountId = null) =>
        options.GameDatabase is { } connection ? new GameDataCharacterStore(options, connection, events, accountId) : new JsonCharacterStore(options);

    internal static bool ValidName(string? name) => name is { Length: > 0 and <= 20 } &&
        name.EnumerateRunes().All(r => Rune.IsLetterOrDigit(r) || r.Value == '_');
}

/// <summary>
/// The original single-account store (Id = Slot + 1) in <see cref="ServerOptions.CharacterStore"/>.
/// Used whenever no game database is configured; its behaviour and file format are unchanged.
/// </summary>
internal sealed class JsonCharacterStore : ICharacterStore
{
    private sealed record Character(byte Slot, ushort Job, string Name, byte[] Appearance);

    private readonly object gate = new();
    private readonly string path;
    private Character[] characters;

    public JsonCharacterStore(ServerOptions options)
    {
        path = Path.GetFullPath(options.CharacterStore!);
        byte slotCount = checked((byte)options.CharacterSlots);
        characters = File.Exists(path)
            ? JsonSerializer.Deserialize<Character[]>(File.ReadAllBytes(path)) ?? throw new InvalidDataException("Empty character store")
            : [];
        if (characters.Length > slotCount || characters.Any(c => c is null || c.Slot >= slotCount ||
            !CharacterLobby.ValidJob(c.Job) || !CharacterStores.ValidName(c.Name) || c.Appearance?.Length != CharacterLobby.AppearanceBytes) ||
            characters.Select(c => c.Slot).Distinct().Count() != characters.Length ||
            characters.Select(c => c.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != characters.Length)
            throw new InvalidDataException("Invalid local character store");
    }

    public long? AccountId => null;

    public ValueTask<IReadOnlyDictionary<string, int>?> StatsAsync(ulong characterId, CancellationToken token) =>
        ValueTask.FromResult<IReadOnlyDictionary<string, int>?>(null);

    public ValueTask<IReadOnlyList<LobbyCharacter>> LoadAsync(CancellationToken token)
    {
        lock (gate) return ValueTask.FromResult<IReadOnlyList<LobbyCharacter>>(characters.Select(Lobby).ToArray());
    }

    public ValueTask<LobbyCharacter> AddAsync(byte slot, ushort job, string name, byte[] appearance, CancellationToken token)
    {
        lock (gate)
        {
            Character? existing = characters.FirstOrDefault(c => c.Slot == slot);
            if (existing is not null && existing.Job == job && existing.Name == name && appearance.AsSpan().SequenceEqual(existing.Appearance))
                return ValueTask.FromResult(Lobby(existing)); // A retry after the original reply was lost.
            if (existing is not null || characters.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ProtocolException(ProtocolError.InvalidPacket, "Character name or slot is already in use");

            Character character = new(slot, job, name, appearance);
            Character[] updated = [.. characters, character];
            Save(updated);
            characters = updated;
            return ValueTask.FromResult(Lobby(character));
        }
    }

    private static LobbyCharacter Lobby(Character c) => new((ulong)c.Slot + 1, c.Slot, c.Job, c.Name, c.Appearance);

    private void Save(Character[] updated)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (FileStream file = new(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(file, updated);
                file.Flush(flushToDisk: true);
            }
            File.Move(temporary, path, overwrite: true);
        }
        finally { File.Delete(temporary); }
    }
}

/// <summary>
/// Characters of the game database account "local" (C9.GameData), so the Portal sees and manages what
/// players create. Ids are database ids. The lobby never migrates the schema: until the Portal has
/// applied the migrations every lobby request fails with "game database is not ready" and is retried
/// on the next request. On the first successful start the JSON characters are imported once (same slots),
/// and only when the account has no characters at all, so characters deleted in the Portal never come back.
/// With a launcher account (<c>fixedAccount</c>, resolved by c9-auth) the store serves that account only and neither
/// creates "local" nor imports the JSON characters.
/// </summary>
internal sealed class GameDataCharacterStore(ServerOptions options, string connection, IServerEvents? events, long? fixedAccount = null) : ICharacterStore
{
    private const string Operator = "c9-lobby";
    private readonly SemaphoreSlim opening = new(1, 1);
    private readonly byte slotCount = checked((byte)options.CharacterSlots);
    private long account;

    public long? AccountId => Interlocked.Read(ref account) is > 0 and var id ? id : null;

    public async ValueTask<IReadOnlyList<LobbyCharacter>> LoadAsync(CancellationToken token)
    {
        long id = await AccountAsync(token);
        try
        {
            IReadOnlyList<Character> rows = await Service<CharacterService>().ListAsync(id, false, token);
            return rows.Where(c => c.Slot >= 0 && c.Slot < slotCount && c.Job is > 0 and <= CharacterService.MaxJob &&
                    CharacterLobby.ValidJob((ushort)c.Job) && CharacterStores.ValidName(c.Name) && c.Appearance.Length == CharacterLobby.AppearanceBytes)
                .OrderBy(c => c.Slot)
                .Select(c => new LobbyCharacter((ulong)c.Id, (byte)c.Slot, (ushort)c.Job, c.Name, c.Appearance))
                .ToArray();
        }
        catch (Exception error) when (Unavailable(error)) { throw NotReady(error); }
    }

    public async ValueTask<LobbyCharacter> AddAsync(byte slot, ushort job, string name, byte[] appearance, CancellationToken token)
    {
        long id = await AccountAsync(token);
        try
        {
            Character c = await Service<CharacterService>().CreateAsync(id, slot, (short)job, name, appearance, token);
            return new LobbyCharacter((ulong)c.Id, (byte)c.Slot, (ushort)c.Job, c.Name, c.Appearance);
        }
        catch (GameDataException error) when (error.Code == GameDataError.Duplicate)
        { throw new ProtocolException(ProtocolError.InvalidPacket, "Character name or slot is already in use"); }
        catch (GameDataException error)
        { throw new ProtocolException(ProtocolError.InvalidPacket, "Character rejected by the game database: " + error.Message); }
        catch (Exception error) when (Unavailable(error)) { throw NotReady(error); }
    }

    public async ValueTask<IReadOnlyDictionary<string, int>?> StatsAsync(ulong characterId, CancellationToken token)
    {
        try
        {
            CharacterStatSheet? sheet = await Service<StatService>().ForCharacterAsync((long)characterId, token);
            return sheet?.Stats.ToDictionary(s => s.Key, s => s.Total);
        }
        catch (Exception error) when (Unavailable(error)) { throw NotReady(error); }
    }

    private async ValueTask<long> AccountAsync(CancellationToken token)
    {
        if (AccountId is { } known) return known;
        await opening.WaitAsync(token);
        try
        {
            if (AccountId is { } again) return again;
            IDbContextFactory<GameDbContext> factory = Service<IDbContextFactory<GameDbContext>>();
            await using (GameDbContext db = await factory.CreateDbContextAsync(token))
            {
                if ((await db.Database.GetPendingMigrationsAsync(token)).Any())
                {
                    // Npgsql caches the database types per data source; after the Portal migrates (citext etc.)
                    // the next attempt must start from a fresh data source.
                    Evict();
                    throw new ProtocolException(ProtocolError.InvalidPacket,
                        "Game database is not ready: the schema is not current (the Portal applies migrations at start-up)");
                }
            }
            if (fixedAccount is { } launcherAccount)
            {
                Interlocked.Exchange(ref account, launcherAccount);
                return launcherAccount;
            }
            OperatorContext context = new(Operator, "Character lobby uses the game database", Guid.NewGuid());
            AccountService accounts = Service<AccountService>();
            Account local;
            try
            {
                local = await accounts.FindByLoginAsync(AccountService.LocalAccount, token)
                    ?? await accounts.CreateAsync(AccountService.LocalAccount, null, context with { Reason = "Create the local lab account for the lobby" }, token);
            }
            catch (GameDataException error) when (error.Code == GameDataError.Duplicate)
            {
                // Another lobby process created it first.
                local = await accounts.FindByLoginAsync(AccountService.LocalAccount, token)
                    ?? throw new ProtocolException(ProtocolError.InvalidPacket, "Game database is not ready: local account missing");
            }
            await ImportLegacyOnceAsync(factory, local.Id, context, token);
            Interlocked.Exchange(ref account, local.Id);
            // Visible in the event log / Loki and the Portal's recent events; no connection details or names.
            events?.Emit(new ServerEvent("character_store_ready")
            {
                Profile = options.Profile, Reason = $"game database, account {AccountService.LocalAccount} (id {local.Id})"
            });
            return local.Id;
        }
        catch (Exception error) when (Unavailable(error)) { Evict(); throw NotReady(error); }
        finally { opening.Release(); }
    }

    private async Task ImportLegacyOnceAsync(IDbContextFactory<GameDbContext> factory, long accountId, OperatorContext context, CancellationToken token)
    {
        if (options.CharacterStore is not { } path || !File.Exists(path)) return;
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await db.Characters.AnyAsync(c => c.AccountId == accountId, token)) return;
        LegacyImportReport report = await Service<LegacyImporter>().ImportCharactersAsync(path,
            context with { RequestId = Guid.NewGuid(), Reason = "Import the lobby's JSON characters into the game database (first start)" }, token);
        events?.Emit(new ServerEvent("legacy_import")
        {
            Profile = options.Profile, Reason = $"{report.Imported} imported, {report.Skipped} skipped from {Path.GetFileName(path)}"
        });
    }

    private T Service<T>() where T : notnull => GameDataProviders.Service<T>(connection);

    private void Evict() => GameDataProviders.Evict(connection);

    private static bool Unavailable(Exception error) => GameDataProviders.Unavailable(error);

    private static ProtocolException NotReady(Exception error) => GameDataProviders.NotReady(error);
}

/// <summary>One C9.GameData service provider per connection string, shared by the lobby's stores (characters, 2nd password).</summary>
internal static class GameDataProviders
{
    private static readonly ConcurrentDictionary<string, Lazy<ServiceProvider>> Providers = new();

    public static T Service<T>(string connection) where T : notnull =>
        Providers.GetOrAdd(connection, raw => new Lazy<ServiceProvider>(() =>
        {
            string resolved = GameDataConnection.ResolveWithEnvironmentPassword(raw)
                ?? throw new InvalidOperationException("C9_DB_CONNECTION is empty");
            ServiceCollection services = new();
            services.AddLogging();
            services.AddC9GameData(resolved);
            return services.BuildServiceProvider();
        })).Value.GetRequiredService<T>();

    public static void Evict(string connection)
    {
        if (Providers.TryRemove(connection, out Lazy<ServiceProvider>? stale) && stale.IsValueCreated)
            _ = stale.Value.DisposeAsync().AsTask();
    }

    public static bool Unavailable(Exception error) =>
        error is DbException or DbUpdateException or TimeoutException or InvalidOperationException or ArgumentException;

    public static ProtocolException NotReady(Exception error) =>
        new(ProtocolError.InvalidPacket, "Game database is not ready: " + error.GetType().Name + ": " + error.Message);
}
