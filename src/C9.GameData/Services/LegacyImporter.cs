using System.Text.Json;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

public sealed record LegacyImportReport(long AccountId, int Imported, int Skipped, IReadOnlyList<string> Notes);

/// <summary>
/// Moves the single-account JSON store used by CharacterLobby (e.g. artifacts/official-55350-characters.local.json
/// or config/migration/local-state/*.json) into the "local" account. Safe to run repeatedly: characters whose
/// name already exists are skipped, and the source file is never modified.
/// </summary>
public sealed class LegacyImporter(IDbContextFactory<GameDbContext> factory, AccountService accounts, TimeProvider clock)
{
    private sealed record LegacyCharacter(byte Slot, ushort Job, string Name, byte[] Appearance);
    private static readonly JsonSerializerOptions LegacyJson = new() { PropertyNameCaseInsensitive = true };

    public async Task<LegacyImportReport> ImportCharactersAsync(string path, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        LegacyCharacter[] legacy;
        await using (FileStream input = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            legacy = await JsonSerializer.DeserializeAsync<LegacyCharacter[]>(input, LegacyJson, token)
                ?? throw new GameDataException(GameDataError.Invalid, "Empty legacy character store");

        Account account = await accounts.FindByLoginAsync(AccountService.LocalAccount, token)
            ?? await accounts.CreateAsync(AccountService.LocalAccount, null,
                context with { RequestId = Derive(context.RequestId, "account"), Reason = "Create local lab account for legacy import" }, token);

        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return new(account.Id, 0, legacy.Length, ["Request already applied"]);
        List<string> notes = [];
        int imported = 0, skipped = 0;
        foreach (LegacyCharacter entry in legacy.OrderBy(c => c.Slot))
        {
            string? problem =
                !CharacterService.ValidName(entry.Name) ? "invalid name" :
                entry.Job is < 1 or > (ushort)CharacterService.MaxJob ? "invalid job" :
                entry.Appearance?.Length != Character.AppearanceBytes ? "invalid appearance" :
                entry.Slot >= account.SlotCount ? "slot outside account" :
                await db.Characters.AnyAsync(c => c.Name == entry.Name && c.DeletedAt == null, token) ? "name already in database" :
                await db.Characters.AnyAsync(c => c.AccountId == account.Id && c.Slot == entry.Slot && c.DeletedAt == null, token) ? "slot occupied" :
                null;
            if (problem is not null) { skipped++; notes.Add($"{entry.Name} (slot {entry.Slot}): {problem}"); continue; }
            db.Characters.Add(new Character
            {
                AccountId = account.Id, Slot = entry.Slot, Job = (short)entry.Job, Name = entry.Name,
                Appearance = entry.Appearance!, CreatedAt = clock.GetUtcNow()
            });
            imported++;
        }
        Audited.Record(db, context, "legacy.import_characters", "account", account.Id.ToString(), null,
            new { Source = Path.GetFileName(path), imported, skipped, notes }, clock);
        await Audited.SaveAsync(db, token);
        return new(account.Id, imported, skipped, notes);
    }

    /// <summary>Imports ServerRates from artifacts/admin-console/parameters.local.json (Event/Account rates have no scope there).</summary>
    public async Task<RateSet?> ImportServerRatesAsync(string parametersPath, WorldService world, OperatorContext context, CancellationToken token = default)
    {
        using JsonDocument document = JsonDocument.Parse(await File.ReadAllBytesAsync(parametersPath, token));
        if (!document.RootElement.TryGetProperty("ServerRates", out JsonElement server)) return null;
        RateSet rates = server.Deserialize<RateSet>() ?? RateSet.Neutral;
        await world.SetServerRatesAsync(rates, WorldService.MaxMultiplier, context, token);
        return rates;
    }

    /// <summary>Deterministic child request id so a re-run of the same import stays idempotent.</summary>
    private static Guid Derive(Guid request, string purpose)
    {
        Span<byte> bytes = stackalloc byte[16];
        request.TryWriteBytes(bytes);
        for (int i = 0; i < purpose.Length; i++) bytes[i % 16] ^= (byte)purpose[i];
        return new Guid(bytes);
    }
}
