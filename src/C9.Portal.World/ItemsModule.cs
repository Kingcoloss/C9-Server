using System.Text.Json;
using System.Text.RegularExpressions;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Portal.GameData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static C9.Portal.GameData.GameDataApi;

namespace C9.Portal.World;

/// <summary>Where the exported catalog lives: items.json and icons/*.png from re/scripts/export_item_catalog.py.</summary>
public sealed record ItemCatalogFolder(string Path)
{
    public const string FileName = "items.json";
    private const long MaxBytes = 64L * 1024 * 1024;

    public string IconPath(string icon) => System.IO.Path.Combine(Path, "icons", icon + ".png");

    public sealed record Summary(bool Exists, string? ExportedAt, int Items, int WithIcon);

    public Summary Describe()
    {
        try
        {
            (IReadOnlyList<CatalogImportRow> rows, string? exportedAt) = Load();
            return new(true, exportedAt, rows.Count, rows.Count(r => r.Icon is not null));
        }
        catch (Exception error) when (error is FileNotFoundException or DirectoryNotFoundException or InvalidDataException or JsonException)
        {
            return new(false, null, 0, 0);
        }
    }

    /// <summary>Reads items.json (schema 1). Throws <see cref="InvalidDataException"/> for other content.</summary>
    public (IReadOnlyList<CatalogImportRow> Rows, string? ExportedAt) Load()
    {
        FileInfo file = new(System.IO.Path.Combine(Path, FileName));
        if (!file.Exists) throw new FileNotFoundException("items.json not found", file.FullName);
        if (file.Length > MaxBytes) throw new InvalidDataException("items.json is larger than 64 MB");
        using FileStream stream = file.OpenRead();
        using JsonDocument document = JsonDocument.Parse(stream);
        JsonElement root = document.RootElement;
        if (!root.TryGetProperty("schema", out JsonElement schema) || schema.GetInt32() != 1 || !root.TryGetProperty("items", out JsonElement items))
            throw new InvalidDataException("items.json is not catalog schema 1");
        List<CatalogImportRow> rows = new(items.GetArrayLength());
        foreach (JsonElement item in items.EnumerateArray())
            rows.Add(new(item.GetProperty("id").GetInt32(), Text(item, "name_en"), Text(item, "name_ko"), Text(item, "description_en"), Text(item, "icon"),
                item.TryGetProperty("tags", out JsonElement tags) && tags.ValueKind == JsonValueKind.Array
                    ? tags.EnumerateArray().Where(t => t.ValueKind == JsonValueKind.String).Select(t => t.GetString()!).ToArray() : [],
                Short(item, "grade"), Short(item, "use_level"),
                item.TryGetProperty("admin_only", out JsonElement admin) && admin.ValueKind == JsonValueKind.True));
        return (rows, root.TryGetProperty("exported_at", out JsonElement at) ? at.GetString() : null);
    }

    private static string? Text(JsonElement item, string name) =>
        item.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static short? Short(JsonElement item, string name) =>
        item.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt16(out short number) ? number : null;
}

/// <summary>Parses "itemId x quantity" lines (also "itemId quantity", "itemId*quantity" or just "itemId" for 1).</summary>
public static partial class ItemLines
{
    [GeneratedRegex(@"^\s*(\d{1,9})\s*(?:[xX*×,]?\s*(\d{1,9}))?\s*$")]
    private static partial Regex Line();

    public static (IReadOnlyList<ItemLine> Lines, IReadOnlyList<string> Errors) Parse(string? text)
    {
        List<ItemLine> lines = [];
        List<string> errors = [];
        foreach (string raw in (text ?? "").Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            Match match = Line().Match(raw);
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out int item) ||
                !int.TryParse(match.Groups[2].Success ? match.Groups[2].Value : "1", out int quantity))
                errors.Add(raw.Trim());
            else lines.Add(new(item, quantity));
        }
        return (lines, errors);
    }

    public static string Format(IEnumerable<ItemLine> lines) => string.Join('\n', lines.Select(l => $"{l.ItemId} x {l.Quantity}"));

    /// <summary>Recipients separated by new lines, commas or spaces.</summary>
    public static IReadOnlyList<string> Names(string? text) =>
        (text ?? "").Split(['\n', '\r', ',', ' ', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

public sealed record ImportRequest(string? Reason) : IWriteRequest;
public sealed record BundleRequest(uint Version, string? Name, string? Description, bool Enabled, IReadOnlyList<ItemLine>? Items, string? Reason) : IWriteRequest;
public sealed record SendRequest(DeliveryTarget Target, IReadOnlyList<string>? Recipients, bool AllAccounts, long? BundleId,
    IReadOnlyList<ItemLine>? Items, string? Message, string? Reason) : IWriteRequest
{
    public DeliveryRequest Input() => new(Target, Recipients ?? [], AllAccounts, BundleId, Items ?? [], Message);
}

/// <summary>Thai labels for catalog tags and grades (Client FINDITEM_GRADE order).</summary>
public static class ItemLabels
{
    public static readonly IReadOnlyDictionary<string, string> Tags = new Dictionary<string, string>
    {
        ["equipment"] = "สวมใส่", ["armor"] = "ชุดเกราะ", ["weapon"] = "อาวุธ", ["accessory"] = "เครื่องประดับ", ["wing"] = "ปีก",
        ["head"] = "หัว", ["shoulders"] = "ไหล่", ["chest"] = "ตัว", ["legs"] = "ขา", ["hands"] = "มือ", ["feet"] = "เท้า",
        ["belt"] = "เข็มขัด", ["bracelet"] = "กำไล", ["ring"] = "แหวน", ["earring"] = "ต่างหู", ["necklace"] = "สร้อยคอ", ["brooch"] = "เข็มกลัด",
        ["consumable"] = "ของใช้สิ้นเปลือง", ["potion"] = "ยา", ["food"] = "อาหาร", ["exp_book"] = "หนังสือ EXP", ["throwable"] = "ของขว้าง",
        ["usable"] = "ใช้ได้/กล่อง", ["quest"] = "เควส", ["decoration"] = "ของตกแต่ง", ["material"] = "วัตถุดิบ", ["craft_material"] = "วัตถุดิบคราฟ",
        ["ingredient"] = "วัตถุดิบอาหาร", ["herb"] = "สมุนไพร", ["cloth"] = "ผ้า/หนัง", ["wood"] = "ไม้", ["metal"] = "โลหะ", ["ore"] = "แร่",
        ["enhance_stone"] = "หินตีบวก", ["gem"] = "อัญมณี", ["ammo"] = "กระสุน", ["spellstone"] = "Spellstone", ["recipe"] = "สูตรคราฟ",
        ["growth_stone"] = "หินเติบโต", ["enhancement"] = "ตีบวก", ["buff"] = "บัฟ", ["skill_book"] = "สกิลบุ๊ค", ["skill"] = "สกิล",
        ["skill_reset"] = "รีเซ็ตสกิล", ["holy_statue"] = "รูปปั้นศักดิ์สิทธิ์", ["rune"] = "รูน", ["charm"] = "เครื่องราง", ["attendant"] = "ผู้ติดตาม",
        ["artifact"] = "โบราณวัตถุ", ["test"] = "ทดสอบ", ["cash"] = "Cash", ["event"] = "อีเวนต์", ["admin"] = "Super Admin",
    };

    public static readonly IReadOnlyList<string> Grades = ["", "Old", "Common", "Magic", "Rare", "Master", "Epic", "Legendary"];

    public static string Tag(string tag) => Tags.GetValueOrDefault(tag, tag);
    public static string Grade(short? grade) => grade is >= 1 and <= 7 ? Grades[grade.Value] : "ไม่ทราบ";
}

/// <summary>Items: catalog with icons (from the Client), item bundles and sending items to characters or accounts.</summary>
public sealed class ItemsModule : IPortalModule
{
    public string Id => "items";
    public string Title => "Items";
    public int Order => 220;

    public IReadOnlyList<PortalNavItem> Navigation { get; } =
    [
        new("Items Catalog", "/items", "book", PortalPolicies.WorldRead),
        new("Items management", "/items/manage", "download", PortalPolicies.AccountsRead),
    ];

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        string folder = configuration["Items:CatalogDirectory"] is { Length: > 0 } configured
            ? configured
            : System.IO.Path.Combine(configuration["Portal:StateDirectory"] ?? "state", "item-catalog");
        services.AddSingleton(new ItemCatalogFolder(System.IO.Path.GetFullPath(folder)));
    }

    public void MapApi(RouteGroupBuilder api)
    {
        RouteGroupBuilder read = api.MapGroup("").RequireAuthorization(PortalPolicies.WorldRead);
        RouteGroupBuilder content = api.MapGroup("").RequireAuthorization(PortalPolicies.WorldWrite);
        RouteGroupBuilder deliveriesRead = api.MapGroup("").RequireAuthorization(PortalPolicies.AccountsRead);
        RouteGroupBuilder deliveriesWrite = api.MapGroup("").RequireAuthorization(PortalPolicies.AccountsWrite);

        read.MapGet("/icons/{icon}", (string icon, ItemCatalogFolder folder) =>
        {
            if (!icon.EndsWith(".png", StringComparison.Ordinal) || !ItemService.ValidIcon(icon[..^4])) return Results.BadRequest();
            string path = folder.IconPath(icon[..^4]);
            return File.Exists(path) ? Results.File(path, "image/png", enableRangeProcessing: false) : Results.NotFound();
        });

        read.MapGet("/catalog", async (HttpContext http, string? q, string? tag, short? grade, short? minLevel, short? maxLevel, bool? admin,
            int? skip, int? take) =>
            Configured(http.RequestServices)
                ? Json(await Svc(http).SearchCatalogAsync(new CatalogFilter(q, tag, grade, minLevel, maxLevel, admin), skip ?? 0, take ?? 50, http.RequestAborted))
                : NotConfigured());

        read.MapGet("/catalog/source", (ItemCatalogFolder folder) => Json(folder.Describe()));

        content.MapPost("/catalog/import", async (HttpContext http, ItemCatalogFolder folder) =>
        {
            ImportRequest? body = await Body<ImportRequest>(http);
            IReadOnlyList<CatalogImportRow> rows;
            try { rows = folder.Load().Rows; }
            catch (Exception error) when (error is FileNotFoundException or DirectoryNotFoundException or InvalidDataException or JsonException or KeyNotFoundException)
            { return Results.Problem(error.Message, statusCode: StatusCodes.Status422UnprocessableEntity); }
            return await Write(http, "items.catalog_import", "item_catalog:all", body,
                (op, s) => s.GetRequiredService<ItemService>().ImportCatalogAsync(rows, op, http.RequestAborted));
        });

        read.MapGet("/bundles", async (HttpContext http) =>
            Configured(http.RequestServices) ? Json((await Svc(http).BundlesAsync(true, http.RequestAborted)).Select(BundleDto)) : NotConfigured());

        content.MapPost("/bundles", async (HttpContext http) =>
        {
            BundleRequest? body = await Body<BundleRequest>(http);
            return await Write(http, "items.bundle_create", "item_bundle:new", body,
                (op, s) => s.GetRequiredService<ItemService>().CreateBundleAsync(body!.Name ?? "", body.Description, body.Items ?? [], op, http.RequestAborted),
                BundleDto, StatusCodes.Status201Created);
        });

        content.MapPut("/bundles/{id:long}", async (HttpContext http, long id) =>
        {
            BundleRequest? body = await Body<BundleRequest>(http);
            return await Write(http, "items.bundle_update", $"item_bundle:{id}", body,
                (op, s) => s.GetRequiredService<ItemService>().UpdateBundleAsync(id, body!.Version, body.Name ?? "", body.Description, body.Enabled,
                    body.Items ?? [], op, http.RequestAborted), BundleDto);
        });

        deliveriesRead.MapGet("/deliveries", async (HttpContext http, string? status, Guid? batch, string? recipient, int? skip, int? take) =>
        {
            if (!Configured(http.RequestServices)) return NotConfigured();
            if (!TryEnum(status, out DeliveryStatus? value)) return BadQuery("status");
            return Json(await Svc(http).DeliveriesAsync(value, batch, recipient, skip ?? 0, take ?? 50, http.RequestAborted));
        });

        deliveriesWrite.MapPost("/deliveries", async (HttpContext http) =>
        {
            SendRequest? body = await Body<SendRequest>(http);
            bool owner = http.User.IsInRole(PortalRoles.Owner);
            return await Write(http, "items.send", "item_delivery:new", body,
                (op, s) => s.GetRequiredService<ItemService>().SendAsync(body!.Input() with { AllowAdminItems = owner }, op, http.RequestAborted),
                status: StatusCodes.Status201Created);
        });

        deliveriesRead.MapGet("/recipients", async (HttpContext http, string? q) =>
            Configured(http.RequestServices) ? Json(await Svc(http).FindRecipientsAsync(q, 20, http.RequestAborted)) : NotConfigured());

        deliveriesWrite.MapPost("/deliveries/{batch:guid}/cancel", async (HttpContext http, Guid batch) =>
        {
            ImportRequest? body = await Body<ImportRequest>(http);
            return await Write(http, "items.cancel", $"item_delivery_batch:{batch}", body,
                async (op, s) => new { cancelled = await s.GetRequiredService<ItemService>().CancelBatchAsync(batch, op, http.RequestAborted) });
        });
    }

    public static object BundleDto(ItemBundle b) => new
    {
        b.Id, b.Name, b.Description, b.Enabled, b.CreatedBy, b.CreatedAt, b.UpdatedAt, b.Version,
        Items = b.Entries.OrderBy(e => e.Line).Select(e => new ItemLine(e.ItemId, e.Quantity)).ToArray()
    };

    private static ItemService Svc(HttpContext http) => http.RequestServices.GetRequiredService<ItemService>();
}
