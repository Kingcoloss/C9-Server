using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace C9.Portal;

/// <summary>
/// A feature module of the Admin Portal (for example Accounts or World). The host discovers every
/// public, non-abstract implementation with a parameterless constructor in assemblies named
/// <c>C9.Portal.*.dll</c> next to the host, so a module never edits the host project.
/// <list type="number">
/// <item><see cref="ConfigureServices"/> runs before the app is built.</item>
/// <item><see cref="MapApi"/> receives a group at <c>/api/v1/{Id}</c> that already requires an
/// authenticated portal user; add <c>.RequireAuthorization(PortalPolicies.X)</c> per endpoint.</item>
/// <item>Routable Razor components (<c>@page</c>) in the module assembly are added to the router
/// automatically; declare <c>@rendermode InteractiveServer</c> and <c>@attribute [Authorize(Policy = ...)]</c>.</item>
/// </list>
/// </summary>
public interface IPortalModule
{
    /// <summary>Lower-case URL segment, unique across modules ("accounts", "world").</summary>
    string Id { get; }
    /// <summary>Navigation section title shown in the sidebar.</summary>
    string Title { get; }
    /// <summary>Sidebar order; core sections use 0-99.</summary>
    int Order { get; }
    IReadOnlyList<PortalNavItem> Navigation { get; }
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    void MapApi(RouteGroupBuilder api);
}

/// <param name="Icon">One of the portal icon names: dashboard, server, users, user, globe, coins, calendar, trace, list, shield, settings, book.</param>
/// <param name="Policy">Only users satisfying this policy see the item.</param>
public sealed record PortalNavItem(string Title, string Href, string Icon, string Policy = PortalPolicies.ServerRead);
