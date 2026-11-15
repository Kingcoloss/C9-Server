using C9.Contracts;
using C9.Portal.Services;

namespace C9.Portal.Modules;

/// <summary>
/// Built-in module that uses the same <see cref="IPortalModule"/> contract as the Accounts and World
/// modules, so the contract stays exercised: packet catalog page plus /api/v1/reference/*.
/// </summary>
public sealed class ReferenceModule : IPortalModule
{
    public string Id => "reference";
    public string Title => "อ้างอิง";
    public int Order => 900;

    public IReadOnlyList<PortalNavItem> Navigation { get; } =
    [
        new("Packet catalog", "/reference/packets", "book", PortalPolicies.TraceRead),
        new("Modules / API", "/reference/modules", "list", PortalPolicies.ServerRead)
    ];

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }

    public void MapApi(RouteGroupBuilder api)
    {
        api.MapGet("/modules", (ModuleCatalog catalog) => Results.Json(catalog.Modules.Select(m => new
        {
            m.Id, m.Title, m.Order, assembly = m.GetType().Assembly.GetName().Name,
            navigation = m.Navigation.Select(n => new { n.Title, n.Href, n.Policy })
        }), ContractJson.Options)).RequireAuthorization(PortalPolicies.ServerRead);
    }
}
