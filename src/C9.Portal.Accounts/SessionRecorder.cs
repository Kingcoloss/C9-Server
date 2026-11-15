using System.Net;
using C9.Contracts;
using C9.GameData;
using C9.GameData.Entities;
using C9.GameData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace C9.Portal.Accounts;

/// <summary>
/// Persists the portal's live session feed to ops.sessions (login history, A9) and refreshes expired
/// sanctions every few minutes. Does nothing when the game database or the feed is not configured.
/// </summary>
internal sealed class SessionRecorder(IServiceProvider services, ILogger<SessionRecorder> logger) : BackgroundService
{
    private static readonly TimeSpan SanctionRefresh = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IDbContextFactory<GameDbContext>? factory = services.GetService<IDbContextFactory<GameDbContext>>();
        if (factory is null) return;
        Task sanctions = RefreshSanctionsAsync(stoppingToken);
        if (services.GetService<ISessionFeed>() is { } feed)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await foreach (SessionLifecycleEvent change in feed.SubscribeAsync(stoppingToken))
                        await RecordAsync(factory, change, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
                catch (Exception error) when (error is DbUpdateException or Npgsql.NpgsqlException or InvalidOperationException)
                {
                    logger.LogWarning(error, "Session recording failed; retrying in 10 seconds");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken).ContinueWith(_ => { });
                }
            }
        }
        await sanctions.ContinueWith(_ => { });
    }

    internal static async Task RecordAsync(IDbContextFactory<GameDbContext> factory, SessionLifecycleEvent change, CancellationToken token)
    {
        SessionInfo session = change.Session;
        await using GameDbContext db = await factory.CreateDbContextAsync(token);
        SessionRecord? row = await db.Sessions.FirstOrDefaultAsync(s => s.Key == session.Key, token);
        if (row is null)
        {
            row = new SessionRecord { Key = session.Key, Service = session.ServiceId, StartedAt = session.ConnectedUtc };
            db.Sessions.Add(row);
        }
        if (session.RemoteEndpoint is { } remote && IPEndPoint.TryParse(remote, out IPEndPoint? endpoint))
            row.RemoteAddress = endpoint.Address;
        row.LastState = session.State;
        // Only a lobby backed by the game database reports AccountId; its character ids are database ids.
        // A JSON-store lobby reports Slot+1, which must not be linked to a database character.
        if (session.AccountId is { } account)
        {
            row.AccountId = account;
            if (session.CharacterId is { } character && character <= long.MaxValue) row.CharacterId = (long)character;
        }
        if (session.MapId is { } map) row.MapId = map;
        if (change.Kind == SessionLifecycleKinds.Closed || session.ClosedUtc is not null)
        {
            row.EndedAt = session.ClosedUtc ?? change.TimeUtc;
            row.EndReason = session.CloseReason ?? session.LastError;
        }
        await db.SaveChangesAsync(token);
    }

    private async Task RefreshSanctionsAsync(CancellationToken token)
    {
        SanctionService? sanctions = services.GetService<SanctionService>();
        if (sanctions is null) return;
        using PeriodicTimer timer = new(SanctionRefresh);
        do
        {
            try { await sanctions.RefreshExpiredAsync(token); }
            catch (Exception error) when (error is DbUpdateException or Npgsql.NpgsqlException or InvalidOperationException)
            { logger.LogWarning(error, "Sanction refresh failed"); }
        }
        while (await timer.WaitForNextTickAsync(token).AsTask().ContinueWith(t => !t.IsCanceled && t.Result));
    }
}
