using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

/// <summary>Root component for the test host, like the portal's App.razor: routes to the module pages and prerenders them.</summary>
internal sealed class TestApp : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, "<!DOCTYPE html>");
        builder.OpenElement(1, "html");
        builder.OpenElement(2, "body");
        builder.OpenComponent<Router>(3);
        builder.AddComponentParameter(4, nameof(Router.AppAssembly), typeof(C9.Portal.Accounts.AccountsModule).Assembly);
        builder.AddComponentParameter(5, nameof(Router.AdditionalAssemblies), new[] { typeof(C9.Portal.World.WorldModule).Assembly });
        builder.AddComponentParameter(6, nameof(Router.Found), (RenderFragment<RouteData>)(route => b =>
        {
            b.OpenComponent<RouteView>(0);
            b.AddComponentParameter(1, nameof(RouteView.RouteData), route);
            b.CloseComponent();
        }));
        builder.CloseComponent();
        builder.CloseElement();
        builder.CloseElement();
    }
}
