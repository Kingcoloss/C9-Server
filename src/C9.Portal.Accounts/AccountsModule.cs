using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Portal.Accounts.Api;
using C9.Portal.GameData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static C9.Portal.GameData.GameDataApi;

namespace C9.Portal.Accounts;

/// <summary>Game Account Management (task 3.2): accounts, characters, sanctions, online sessions.</summary>
public sealed class AccountsModule : IPortalModule
{
    public string Id => "accounts";
    public string Title => "Game Accounts";
    public int Order => 200;

    public IReadOnlyList<PortalNavItem> Navigation { get; } =
    [
        new("บัญชีผู้เล่น", "/accounts", "users", PortalPolicies.AccountsRead),
        new("ผู้เล่นออนไลน์", "/accounts/online", "user", PortalPolicies.AccountsRead),
    ];

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // C9.GameData itself is registered by the host when C9_DB_CONNECTION is set.
        services.AddScoped<PortalActions>();
        services.AddSingleton<AccountsQueries>(provider =>
            new AccountsQueries(provider.GetRequiredService<Microsoft.EntityFrameworkCore.IDbContextFactory<C9.GameData.GameDbContext>>()));
        services.AddHostedService<SessionRecorder>();
    }

    public void MapApi(RouteGroupBuilder api)
    {
        RouteGroupBuilder read = api.MapGroup("").RequireAuthorization(PortalPolicies.AccountsRead);
        RouteGroupBuilder write = api.MapGroup("").RequireAuthorization(PortalPolicies.AccountsWrite);

        read.MapGet("/", async (HttpContext http, string? q, string? status, string? tag, string? sort, bool? desc, int? skip, int? take) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            if (!TryEnum(status, out AccountStatus? statusValue)) return BadQuery("status");
            if (!TryEnum(sort, out AccountSort? sortValue)) return BadQuery("sort");
            try
            {
                return Json(await Svc<AccountService>(http).SearchAsync(new AccountQuery(q, statusValue, tag, sortValue ?? AccountSort.LoginName,
                    desc ?? false, skip ?? 0, take ?? 50), http.RequestAborted));
            }
            catch (GameDataException error) { return Problem(error); }
        });

        read.MapGet("/{id:long}", async (HttpContext http, long id) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            Account? account = await Svc<AccountService>(http).GetAsync(id, http.RequestAborted);
            if (account is null) return Results.NotFound();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return Json(new AccountDetailDto(AccountDto.From(account), account.Characters.Select(CharacterDto.From).ToArray(),
                account.Sanctions.Select(s => SanctionDto.From(s, now)).ToArray()));
        });

        write.MapPost("/", async (HttpContext http) =>
        {
            CreateAccountRequest? body = await Body<CreateAccountRequest>(http);
            return await Write(http, "accounts.create", "account:" + body?.LoginName, body,
                (op, s) => s.GetRequiredService<AccountService>().CreateAsync(body!.LoginName ?? "", body.Password, op, http.RequestAborted),
                AccountDto.From, StatusCodes.Status201Created);
        });

        write.MapPatch("/{id:long}", async (HttpContext http, long id) =>
        {
            UpdateAccountRequest? body = await Body<UpdateAccountRequest>(http);
            // Granting GM rights is an administrator decision, not a GM's (segregation of duties).
            if (body?.IsGm is not null && !HasRole(http.User, PortalRoles.Owner, PortalRoles.Administrator))
                return Results.Problem("Only Owner or Administrator may change GM status", statusCode: StatusCodes.Status403Forbidden);
            return await Write(http, "accounts.update", $"account:{id}", body,
                (op, s) => s.GetRequiredService<AccountService>().UpdateAsync(id, body!.Version,
                    new AccountChanges(body.IsGm, body.SlotCount, body.Note, body.Tags), op, http.RequestAborted),
                AccountDto.From);
        });

        write.MapPost("/{id:long}/password", async (HttpContext http, long id) =>
        {
            PasswordRequest? body = await Body<PasswordRequest>(http);
            return await Write(http, "accounts.password_reset", $"account:{id}", body,
                async (op, s) => { await s.GetRequiredService<AccountService>().ResetPasswordAsync(id, body!.Password ?? "", op, http.RequestAborted); return new { reset = true }; });
        });

        write.MapPost("/{id:long}/sanctions", async (HttpContext http, long id) =>
        {
            SanctionRequest? body = await Body<SanctionRequest>(http);
            return await Write(http, "accounts.sanction", $"account:{id}", body,
                (op, s) => s.GetRequiredService<SanctionService>().IssueAsync(id, body!.Kind, body.EndsAt, op, http.RequestAborted),
                sanction => SanctionDto.From(sanction, DateTimeOffset.UtcNow), StatusCodes.Status201Created);
        });

        write.MapPost("/{id:long}/sanctions/{sanctionId:long}/revoke", async (HttpContext http, long id, long sanctionId) =>
        {
            ReasonRequest? body = await Body<ReasonRequest>(http);
            return await Write(http, "accounts.sanction_revoke", $"account:{id}", body,
                async (op, s) => { await s.GetRequiredService<SanctionService>().RevokeAsync(id, sanctionId, op, http.RequestAborted); return new { revoked = sanctionId }; });
        });

        read.MapGet("/{id:long}/timeline", async (HttpContext http, long id, int? take) =>
            Configured(http.RequestServices)
                ? Json(await Svc<AccountsQueries>(http).TimelineAsync(id, take ?? 100, http.RequestAborted))
                : NotConfigured());

        read.MapGet("/{id:long}/sessions", async (HttpContext http, long id, int? take) =>
            Configured(http.RequestServices)
                ? Json(await Svc<AccountsQueries>(http).SessionsAsync(id, take ?? 100, http.RequestAborted))
                : NotConfigured());

        read.MapGet("/{id:long}/rates", async (HttpContext http, long id) =>
            Configured(http.RequestServices)
                ? Json(await Svc<WorldService>(http).EffectiveRatesAsync(id, null, http.RequestAborted) is var r
                    ? new { r.Rates, r.Server, r.Account, r.Cap, Events = r.Events.Select(e => new { e.Id, e.Name, e.Rates }) } : null)
                : NotConfigured());

        write.MapPut("/{id:long}/rates", async (HttpContext http, long id) =>
        {
            AccountRatesRequest? body = await Body<AccountRatesRequest>(http);
            return await Write(http, "accounts.rates", $"account:{id}", body,
                async (op, s) => { await s.GetRequiredService<WorldService>().SetAccountRatesAsync(id, body!.Rates, body.ExpiresAt, op, http.RequestAborted); return new { saved = true }; });
        });

        // Characters (under /api/v1/accounts/characters/...).
        read.MapGet("/{id:long}/characters", async (HttpContext http, long id, bool? includeDeleted) =>
            Configured(http.RequestServices)
                ? Json((await Svc<CharacterService>(http).ListAsync(id, includeDeleted ?? false, http.RequestAborted)).Select(CharacterDto.From))
                : NotConfigured());

        read.MapGet("/characters/{cid:long}", async (HttpContext http, long cid) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            Character? c = await Svc<CharacterService>(http).GetAsync(cid, http.RequestAborted);
            return c is null ? Results.NotFound() : Json(new CharacterDetailDto(CharacterDto.From(c),
                c.Items.OrderBy(i => i.Location).ThenBy(i => i.Slot).Select(i => new ItemDto(i.Serial, i.Location, i.Slot, i.ItemId,
                    i.Quantity, i.Enchant, i.Options, i.Bound, i.ExpiresAt)).ToArray()));
        });

        read.MapGet("/characters/{cid:long}/snapshots", async (HttpContext http, long cid, int? take) =>
            Configured(http.RequestServices)
                ? Json((await Svc<CharacterService>(http).SnapshotsAsync(cid, take ?? 50, http.RequestAborted))
                    .Select(s => new { s.Id, s.TakenAt, s.Cause, s.Data }))
                : NotConfigured());

        read.MapGet("/characters/{cid:long}/export", async (HttpContext http, long cid) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            try { return Json(await Svc<CharacterService>(http).ExportAsync(cid, http.RequestAborted)); }
            catch (GameDataException error) { return Problem(error); }
        });

        write.MapPost("/characters/{cid:long}/rename", async (HttpContext http, long cid) =>
        {
            RenameRequest? body = await Body<RenameRequest>(http);
            return await Write(http, "characters.rename", $"character:{cid}", body,
                (op, s) => s.GetRequiredService<CharacterService>().RenameAsync(cid, body!.Version, body.Name ?? "", op, http.RequestAborted), CharacterDto.From);
        });

        write.MapPost("/characters/{cid:long}/move", async (HttpContext http, long cid) =>
        {
            MoveRequest? body = await Body<MoveRequest>(http);
            return await Write(http, "characters.move_slot", $"character:{cid}", body,
                (op, s) => s.GetRequiredService<CharacterService>().MoveSlotAsync(cid, body!.Version, body.Slot, op, http.RequestAborted), CharacterDto.From);
        });

        write.MapPatch("/characters/{cid:long}", async (HttpContext http, long cid) =>
        {
            UpdateCharacterRequest? body = await Body<UpdateCharacterRequest>(http);
            return await Write(http, "characters.update", $"character:{cid}", body,
                (op, s) => s.GetRequiredService<CharacterService>().UpdateAsync(cid, body!.Version,
                    new CharacterChanges(body.Level, body.MapId, body.Submap, body.PosX, body.PosY, body.PosZ, body.Appearance), op, http.RequestAborted),
                CharacterDto.From);
        });

        write.MapPost("/characters/{cid:long}/delete", async (HttpContext http, long cid) =>
        {
            VersionedRequest? body = await Body<VersionedRequest>(http);
            return await Write(http, "characters.delete", $"character:{cid}", body,
                (op, s) => s.GetRequiredService<CharacterService>().SoftDeleteAsync(cid, body!.Version, op, http.RequestAborted), CharacterDto.From);
        });

        write.MapPost("/characters/{cid:long}/restore", async (HttpContext http, long cid) =>
        {
            VersionedRequest? body = await Body<VersionedRequest>(http);
            return await Write(http, "characters.restore", $"character:{cid}", body,
                (op, s) => s.GetRequiredService<CharacterService>().RestoreAsync(cid, body!.Version, op, http.RequestAborted), CharacterDto.From);
        });

        write.MapPost("/{id:long}/characters/import", async (HttpContext http, long id) =>
        {
            ImportCharacterRequest? body = await Body<ImportCharacterRequest>(http);
            if (body?.Data is null) return Results.Problem("data is required", statusCode: StatusCodes.Status400BadRequest);
            return await Write(http, "characters.import", $"account:{id}", body,
                (op, s) => s.GetRequiredService<CharacterService>().ImportAsync(id, body.Slot, body.Data, op, http.RequestAborted),
                CharacterDto.From, StatusCodes.Status201Created);
        });

        // Owner/Administrator only: one-off migration of the legacy CharacterLobby JSON store.
        api.MapPost("/import-legacy", async (HttpContext http) =>
        {
            LegacyImportRequest? body = await Body<LegacyImportRequest>(http);
            return await Write(http, "accounts.import_legacy", "account:local", body, async (op, s) =>
            {
                string path = Path.Combine(Path.GetTempPath(), "c9-legacy-" + op.RequestId.ToString("N") + ".json");
                await File.WriteAllTextAsync(path, body!.Characters.GetRawText(), http.RequestAborted);
                try { return await s.GetRequiredService<LegacyImporter>().ImportCharactersAsync(path, op, http.RequestAborted); }
                finally { File.Delete(path); }
            });
        }).RequireAuthorization(PortalPolicies.ServerOperate);

        // Live sessions from the game-service agents (IGameServices, registered by the host).
        read.MapGet("/online", async (HttpContext http) =>
        {
            IGameServices? game = http.RequestServices.GetService<IGameServices>();
            return game is null ? Results.Problem("Game services are not available", statusCode: StatusCodes.Status503ServiceUnavailable)
                : Json(await game.SessionsAsync(false, http.RequestAborted));
        });

        write.MapPost("/online/{key}/disconnect", async (HttpContext http, string key) =>
        {
            ReasonRequest? body = await Body<ReasonRequest>(http);
            if (http.RequestServices.GetService<IGameServices>() is not { } game)
                return Results.Problem("Game services are not available", statusCode: StatusCodes.Status503ServiceUnavailable);
            if (body is null || string.IsNullOrWhiteSpace(body.Reason))
                return Results.Problem("reason is required", statusCode: StatusCodes.Status400BadRequest);
            if (!Guid.TryParse(http.Request.Headers[IdempotencyHeader].ToString(), out Guid requestId))
                return Results.Problem($"Header {IdempotencyHeader} must be a UUID", statusCode: StatusCodes.Status400BadRequest);
            bool done = await game.DisconnectAsync(key, http.RequestAborted);
            await Audit(http.RequestServices.GetService<IAuditLog>(), Operator(http.User, body.Reason, requestId),
                "accounts.disconnect", "session:" + key, done, done ? null : "Session not found or already closed",
                http.Connection.RemoteIpAddress?.ToString(), http.RequestAborted);
            return done ? Json(new { disconnected = key }) : Results.NotFound();
        });
    }

    private static T Svc<T>(HttpContext http) where T : notnull => http.RequestServices.GetRequiredService<T>();
}
