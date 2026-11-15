using C9.GameData;
using Microsoft.EntityFrameworkCore;

namespace C9.Portal.Accounts.Api;

/// <summary>Read-only queries the GameData services do not cover (timeline, session history).</summary>
public sealed class AccountsQueries(IDbContextFactory<GameDbContext> factory)
{
    /// <summary>Audit entries on the account and on every character it owns, newest first.</summary>
    public async Task<IReadOnlyList<TimelineEntry>> TimelineAsync(long accountId, int take = 100, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        string account = accountId.ToString();
        List<string> characters = await db.Characters.Where(c => c.AccountId == accountId)
            .Select(c => c.Id.ToString()).ToListAsync(token);
        return await db.AuditLog.AsNoTracking()
            .Where(a => (a.TargetType == "account" && a.TargetId == account) ||
                        (a.TargetType == "character" && characters.Contains(a.TargetId!)))
            .OrderByDescending(a => a.At).ThenByDescending(a => a.Id).Take(Math.Clamp(take, 1, 500))
            .Select(a => new TimelineEntry(a.At, a.Operator, a.Action, a.TargetType, a.TargetId, a.Reason, a.Before, a.After))
            .ToListAsync(token);
    }

    public async Task<IReadOnlyList<SessionHistoryDto>> SessionsAsync(long accountId, int take = 100, CancellationToken token = default)
    {
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        var rows = await db.Sessions.AsNoTracking().Where(s => s.AccountId == accountId)
            .OrderByDescending(s => s.StartedAt).Take(Math.Clamp(take, 1, 500)).ToListAsync(token);
        return rows.Select(s => new SessionHistoryDto(s.Key, s.Service, s.CharacterId, s.RemoteAddress?.ToString(), s.StartedAt,
            s.EndedAt, s.EndReason, s.LastState, s.MapId)).ToArray();
    }
}
