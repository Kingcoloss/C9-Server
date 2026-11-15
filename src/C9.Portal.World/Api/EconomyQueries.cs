using C9.GameData;
using C9.GameData.Entities;
using Microsoft.EntityFrameworkCore;

namespace C9.Portal.World.Api;

public sealed record FlowRow(string Reason, LedgerAsset Asset, long In, long Out, long Net, int Entries);

public sealed record DailyFlow(DateOnly Day, long GoldIn, long GoldOut, int ItemsCreated, int ItemsDestroyed);

public sealed record Holder(long CharacterId, string Name, long AccountId, short Level, long Gold);

public sealed record EconomySummary(DateTimeOffset From, DateTimeOffset To, long GoldInCirculation, int Characters, int Accounts,
    long GoldFaucet, long GoldSink, int ItemInstances, IReadOnlyList<FlowRow> Flows, IReadOnlyList<DailyFlow> Daily,
    IReadOnlyList<Holder> TopHolders, bool HasLedgerData);

/// <summary>Economic dashboard (W3): every number is derived from characters/items and the append-only ledger.</summary>
public sealed class EconomyQueries(IDbContextFactory<GameDbContext> factory)
{
    public const int MaxDays = 366;

    public async Task<EconomySummary> SummaryAsync(DateTimeOffset from, DateTimeOffset to, int top = 20, CancellationToken token = default)
    {
        if (to <= from || to - from > TimeSpan.FromDays(MaxDays))
            throw new ArgumentOutOfRangeException(nameof(to), $"Range must be positive and at most {MaxDays} days");
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        IQueryable<Character> live = db.Characters.AsNoTracking().Where(c => c.DeletedAt == null);
        long gold = await live.SumAsync(c => (long?)c.Gold, token) ?? 0;
        int characters = await live.CountAsync(token);
        int accounts = await db.Accounts.CountAsync(token);
        int items = await db.CharacterItems.CountAsync(token);

        IQueryable<LedgerEntry> range = db.Ledger.AsNoTracking().Where(l => l.At >= from && l.At < to);
        // Aggregated in SQL by sign, combined here (conditional sums inside GroupBy are not translatable).
        var bySign = await range.GroupBy(l => new { l.Reason, l.Asset, Positive = l.Delta > 0 })
            .Select(g => new { g.Key.Reason, g.Key.Asset, g.Key.Positive, Sum = g.Sum(l => l.Delta), Count = g.Count() })
            .ToListAsync(token);
        List<FlowRow> flows = bySign.GroupBy(r => (r.Reason, r.Asset))
            .Select(g => new FlowRow(g.Key.Reason, g.Key.Asset, g.Where(r => r.Positive).Sum(r => r.Sum), -g.Where(r => !r.Positive).Sum(r => r.Sum),
                g.Sum(r => r.Sum), g.Sum(r => r.Count)))
            .OrderByDescending(f => f.In + f.Out).ToList();

        var daily = await range.Where(l => l.Asset != LedgerAsset.Exp)
            .GroupBy(l => new { l.At.Year, l.At.Month, l.At.Day, l.Asset, Positive = l.Delta > 0 })
            .Select(g => new { g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Asset, g.Key.Positive, Sum = g.Sum(l => l.Delta), Count = g.Count() })
            .ToListAsync(token);

        List<Holder> holders = await live.OrderByDescending(c => c.Gold).ThenBy(c => c.Id).Take(Math.Clamp(top, 1, 100))
            .Select(c => new Holder(c.Id, c.Name, c.AccountId, c.Level, c.Gold)).ToListAsync(token);

        long faucet = flows.Where(f => f.Asset == LedgerAsset.Gold).Sum(f => f.In);
        long sink = flows.Where(f => f.Asset == LedgerAsset.Gold).Sum(f => f.Out);
        return new(from, to, gold, characters, accounts, faucet, sink, items, flows,
            daily.GroupBy(d => new DateOnly(d.Year, d.Month, d.Day)).Select(g => new DailyFlow(g.Key,
                    g.Where(d => d.Asset == LedgerAsset.Gold && d.Positive).Sum(d => d.Sum),
                    -g.Where(d => d.Asset == LedgerAsset.Gold && !d.Positive).Sum(d => d.Sum),
                    g.Where(d => d.Asset == LedgerAsset.Item && d.Positive).Sum(d => d.Count),
                    g.Where(d => d.Asset == LedgerAsset.Item && !d.Positive).Sum(d => d.Count)))
                .OrderBy(d => d.Day).ToArray(),
            holders, flows.Count > 0);
    }
}
