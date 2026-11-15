using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace C9.Portal.Components.Shared;

/// <summary>
/// Base for interactive portal pages: resolves the signed-in user and runs periodic refreshes on the
/// circuit's synchronization context until the page is disposed.
/// </summary>
public abstract class LivePage : ComponentBase, IAsyncDisposable
{
    /// <summary>UI actions run on a Blazor circuit, which has no client address; audit records this marker.</summary>
    protected const string UiOrigin = "portal-ui";
    private readonly CancellationTokenSource stopping = new();
    private readonly List<Task> loops = [];

    [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }

    protected ClaimsPrincipal User { get; private set; } = new(new ClaimsIdentity());
    protected CancellationToken Stopping => stopping.Token;
    protected string? Problem { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthState is not null) User = (await AuthState).User;
        await LoadAsync();
    }

    /// <summary>First load (prerender and interactive).</summary>
    protected virtual Task LoadAsync() => Task.CompletedTask;

    protected bool Can(string policy) => PortalPolicies.Roles.TryGetValue(policy, out string[]? roles) && roles.Any(User.IsInRole);

    /// <summary>Calls <paramref name="tick"/> every <paramref name="period"/> on the renderer and re-renders.</summary>
    protected void Every(TimeSpan period, Func<Task> tick) => loops.Add(Task.Run(async () =>
    {
        using PeriodicTimer timer = new(period);
        try
        {
            while (await timer.WaitForNextTickAsync(stopping.Token))
                await InvokeAsync(async () =>
                {
                    try { await tick(); }
                    catch (Exception error) when (error is not OperationCanceledException || !stopping.IsCancellationRequested)
                    {
                        Problem = error.Message;
                    }
                    StateHasChanged();
                });
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
    }));

    /// <summary>Runs a background consumer (for example a <see cref="Services.PortalBus"/> subscription).</summary>
    protected void Background(Func<CancellationToken, Task> work) => loops.Add(Task.Run(async () =>
    {
        try { await work(stopping.Token); }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
    }));

    public virtual async ValueTask DisposeAsync()
    {
        await stopping.CancelAsync();
        try { await Task.WhenAll(loops); }
        catch (OperationCanceledException) { }
        stopping.Dispose();
        GC.SuppressFinalize(this);
    }
}
