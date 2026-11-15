using System.Text.Json;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.GameData.Services;

public sealed record EffectiveRates(RateSet Rates, RateSet Server, RateSet Account, decimal Cap,
    IReadOnlyList<(long Id, string Name, RateSet Rates)> Events);

public sealed record WorldEventInput(string Name, string? Description, DateTimeOffset StartsAt, DateTimeOffset EndsAt,
    RateSet Rates, int? RepeatEveryDays = null, DateTimeOffset? RecurUntil = null, bool Enabled = true, string Extra = "{}");

public sealed record MaintenanceState(bool Enabled, string? Message, DateTimeOffset? Until, IReadOnlyList<string> AllowedLogins);

public sealed class WorldService(IDbContextFactory<GameDbContext> factory, TimeProvider clock)
{
    public const decimal MaxMultiplier = 100;
    public const string MaintenanceKey = "maintenance";

    /// <summary>
    /// effective = min(server × Π(active events) × account, cap); the cap is applied once at the end
    /// (fixes the per-step cap in app/Economy/RatePolicy.cs noted in CURRENT_STATE_20261114 §3).
    /// </summary>
    public static RateSet Combine(RateSet server, IEnumerable<RateSet> events, RateSet account, decimal cap) =>
        events.Aggregate(server, (total, e) => total.Times(e)).Times(account).Cap(cap);

    public async Task<EffectiveRates> EffectiveRatesAsync(long? accountId = null, DateTimeOffset? at = null, CancellationToken token = default)
    {
        DateTimeOffset moment = at ?? clock.GetUtcNow();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        RatePolicy policy = await db.RatePolicies.AsNoTracking().FirstOrDefaultAsync(p => p.Scope == RatePolicy.ServerScope, token)
            ?? new RatePolicy();
        RateSet account = RateSet.Neutral;
        if (accountId is { } id && await db.AccountRateOverrides.AsNoTracking().FirstOrDefaultAsync(o => o.AccountId == id, token) is { } found &&
            (found.ExpiresAt is null || found.ExpiresAt > moment))
            account = found.Rates;
        // Candidate events are narrowed in SQL; recurrence is evaluated in memory.
        List<WorldEvent> candidates = await db.WorldEvents.AsNoTracking()
            .Where(e => e.Enabled && e.StartsAt <= moment && (e.EndsAt > moment || e.RepeatEveryDays != null))
            .OrderBy(e => e.StartsAt).ToListAsync(token);
        var active = candidates.Where(e => e.IsActiveAt(moment)).Select(e => (e.Id, e.Name, e.Rates)).ToArray();
        return new(Combine(policy.Rates, active.Select(e => e.Rates), account, policy.Cap), policy.Rates, account, policy.Cap, active);
    }

    public async Task<RatePolicy> SetServerRatesAsync(RateSet rates, decimal cap, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (!rates.IsValid(MaxMultiplier) || cap is <= 0 or > MaxMultiplier)
            throw new GameDataException(GameDataError.Invalid, $"Multipliers must be 0–{MaxMultiplier} and the cap 0–{MaxMultiplier}");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        RatePolicy? policy = await db.RatePolicies.FirstOrDefaultAsync(p => p.Scope == RatePolicy.ServerScope, token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return policy ?? new RatePolicy();
        object? before = policy is null ? null : new { policy.Rates, policy.Cap };
        if (policy is null) db.RatePolicies.Add(policy = new RatePolicy());
        policy.Rates = rates;
        policy.Cap = cap;
        policy.UpdatedAt = clock.GetUtcNow();
        Audited.Record(db, context, "world.rates", "rate_policy", RatePolicy.ServerScope, before, new { rates, cap }, clock);
        await Audited.SaveAsync(db, token);
        return policy;
    }

    public async Task SetAccountRatesAsync(long accountId, RateSet? rates, DateTimeOffset? expiresAt, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (rates is not null && !rates.IsValid(MaxMultiplier))
            throw new GameDataException(GameDataError.Invalid, $"Multipliers must be 0–{MaxMultiplier}");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return;
        if (!await db.Accounts.AnyAsync(a => a.Id == accountId, token))
            throw new GameDataException(GameDataError.NotFound, "Account not found");
        AccountRateOverride? current = await db.AccountRateOverrides.FirstOrDefaultAsync(o => o.AccountId == accountId, token);
        object? before = current is null ? null : new { current.Rates, current.ExpiresAt };
        if (rates is null) { if (current is not null) db.AccountRateOverrides.Remove(current); }
        else
        {
            if (current is null) db.AccountRateOverrides.Add(current = new AccountRateOverride { AccountId = accountId });
            current.Rates = rates;
            current.ExpiresAt = expiresAt;
        }
        Audited.Record(db, context, "account.rates", "account", accountId.ToString(), before, rates is null ? null : new { rates, expiresAt }, clock);
        await Audited.SaveAsync(db, token);
    }

    public async Task<IReadOnlyList<WorldEvent>> EventsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.WorldEvents.AsNoTracking()
            .Where(e => e.StartsAt < to && (e.EndsAt > from || e.RepeatEveryDays != null))
            .OrderBy(e => e.StartsAt).ToListAsync(token);
    }

    public async Task<WorldEvent> CreateEventAsync(WorldEventInput input, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        Validate(input);
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return await db.WorldEvents.AsNoTracking().OrderByDescending(e => e.Id).FirstAsync(e => e.Name == input.Name, token);
        WorldEvent created = new() { Name = input.Name, CreatedBy = context.Operator, CreatedAt = clock.GetUtcNow() };
        Apply(created, input);
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        db.WorldEvents.Add(created);
        await Audited.SaveAsync(db, token);
        Audited.Record(db, context, "world.event_create", "world_event", created.Id.ToString(), null, input, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return created;
    }

    public async Task<WorldEvent> UpdateEventAsync(long id, uint expectedVersion, WorldEventInput input, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        Validate(input);
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        WorldEvent current = await db.WorldEvents.FirstOrDefaultAsync(e => e.Id == id, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Event not found");
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return current;
        db.Entry(current).Property(e => e.Version).OriginalValue = expectedVersion;
        WorldEventInput before = new(current.Name, current.Description, current.StartsAt, current.EndsAt, current.Rates,
            current.RepeatEveryDays, current.RecurUntil, current.Enabled, current.Extra);
        Apply(current, input);
        Audited.Record(db, context, "world.event_update", "world_event", id.ToString(), before, input, clock);
        await Audited.SaveAsync(db, token);
        return current;
    }

    public async Task DeleteEventAsync(long id, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return;
        WorldEvent current = await db.WorldEvents.FirstOrDefaultAsync(e => e.Id == id, token)
            ?? throw new GameDataException(GameDataError.NotFound, "Event not found");
        db.WorldEvents.Remove(current);
        Audited.Record(db, context, "world.event_delete", "world_event", id.ToString(), new { current.Name, current.StartsAt, current.EndsAt }, null, clock);
        await Audited.SaveAsync(db, token);
    }

    public async Task<MaintenanceState> MaintenanceAsync(CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        string? json = await db.WorldSettings.AsNoTracking().Where(s => s.Key == MaintenanceKey).Select(s => s.Value).FirstOrDefaultAsync(token);
        MaintenanceState state = json is null ? new(false, null, null, []) : JsonSerializer.Deserialize<MaintenanceState>(json, Json.Options)!;
        // An elapsed window switches itself off without a write.
        return state.Until is { } until && until <= clock.GetUtcNow() ? state with { Enabled = false } : state;
    }

    public async Task SetMaintenanceAsync(MaintenanceState state, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (state.Message is { Length: > 500 } || state.AllowedLogins.Count > 100)
            throw new GameDataException(GameDataError.Invalid, "Message up to 500 characters and up to 100 allowed logins");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token)) return;
        WorldSetting? setting = await db.WorldSettings.FirstOrDefaultAsync(s => s.Key == MaintenanceKey, token);
        string? before = setting?.Value;
        if (setting is null) db.WorldSettings.Add(setting = new WorldSetting { Key = MaintenanceKey, Value = "{}" });
        setting.Value = Json.Of(state);
        setting.UpdatedAt = clock.GetUtcNow();
        Audited.Record(db, context, "world.maintenance", "world_setting", MaintenanceKey,
            before is null ? null : JsonDocument.Parse(before).RootElement, state, clock);
        await Audited.SaveAsync(db, token);
    }

    public async Task<Announcement> AnnounceAsync(AnnouncementChannel channel, string message, DateTimeOffset? startsAt,
        DateTimeOffset? endsAt, int? repeatMinutes, OperatorContext context, CancellationToken token = default)
    {
        context.Validate();
        if (string.IsNullOrWhiteSpace(message) || message.Length > 500)
            throw new GameDataException(GameDataError.Invalid, "Message is required (max 500 characters)");
        if (repeatMinutes is < 1 or > 1440 || (startsAt is { } s && endsAt is { } e && e <= s))
            throw new GameDataException(GameDataError.Invalid, "Repeat must be 1–1440 minutes and the window must end after it starts");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        if (await Audited.AlreadyAppliedAsync(db, context, token))
            return await db.Announcements.AsNoTracking().OrderByDescending(a => a.Id).FirstAsync(token);
        Announcement announcement = new()
        {
            Channel = channel, Message = message, StartsAt = startsAt, EndsAt = endsAt, RepeatMinutes = repeatMinutes,
            CreatedBy = context.Operator, CreatedAt = clock.GetUtcNow()
        };
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        db.Announcements.Add(announcement);
        await Audited.SaveAsync(db, token);
        Audited.Record(db, context, "world.announce", "announcement", announcement.Id.ToString(), null,
            new { channel, message, startsAt, endsAt, repeatMinutes }, clock);
        await Audited.SaveAsync(db, token);
        await transaction.CommitAsync(token);
        return announcement;
    }

    public async Task<IReadOnlyList<Announcement>> ActiveAnnouncementsAsync(AnnouncementChannel channel, CancellationToken token = default)
    {
        DateTimeOffset now = clock.GetUtcNow();
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        return await db.Announcements.AsNoTracking()
            .Where(a => a.Channel == channel && (a.StartsAt == null || a.StartsAt <= now) && (a.EndsAt == null || a.EndsAt > now))
            .OrderByDescending(a => a.CreatedAt).Take(20).ToListAsync(token);
    }

    private static void Validate(WorldEventInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length > 100)
            throw new GameDataException(GameDataError.Invalid, "Event name is required (max 100 characters)");
        if (input.EndsAt <= input.StartsAt)
            throw new GameDataException(GameDataError.Invalid, "Event must end after it starts");
        if (input.RepeatEveryDays is { } days && (days < 1 || TimeSpan.FromDays(days) < input.EndsAt - input.StartsAt))
            throw new GameDataException(GameDataError.Invalid, "Repeat period must be at least one day and not shorter than the event");
        if (!input.Rates.IsValid(MaxMultiplier))
            throw new GameDataException(GameDataError.Invalid, $"Multipliers must be 0–{MaxMultiplier}");
        try { using JsonDocument _ = JsonDocument.Parse(input.Extra); }
        catch (JsonException) { throw new GameDataException(GameDataError.Invalid, "Extra must be valid JSON"); }
    }

    private static void Apply(WorldEvent target, WorldEventInput input)
    {
        target.Name = input.Name;
        target.Description = input.Description;
        target.StartsAt = input.StartsAt;
        target.EndsAt = input.EndsAt;
        target.RepeatEveryDays = input.RepeatEveryDays;
        target.RecurUntil = input.RecurUntil;
        target.Rates = input.Rates;
        target.Enabled = input.Enabled;
        target.Extra = input.Extra;
    }
}
