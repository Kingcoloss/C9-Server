using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text.Json;
using C9.Admin;

namespace C9.AdminDesktop;

public partial class ConsoleHome
{
    private enum Section { Services, Players, Parameters }

    private readonly CancellationTokenSource monitorLifetime = new();
    private Task? monitor;
    private ConsoleStatus status = new([], []);
    private Section current;
    private bool busy;
    private string? message;
    private bool failed;
    private (string Action, string Target)? confirmation;
    private AdminParameters? parameters;
    private IReadOnlyList<PlayerCharacter> characters = [];
    private IReadOnlyList<PlayerGrant> grants = [];
    private ulong recipient;
    private GrantKind grantKind;
    private int? itemId;
    private long amount = 1;
    private Guid requestId = Guid.NewGuid();

    protected override void OnInitialized()
    {
        RefreshMonitor();
        monitor = MonitorAsync(monitorLifetime.Token);
    }

    private static string SectionTitle(Section section) => section switch
    {
        Section.Services => "Server & Monitor",
        Section.Players => "บัญชีและส่งของ",
        _ => "ตั้งค่าเกม"
    };

    private void Navigate(Section section)
    {
        if (current == section) return;
        current = section;
        message = null;
        confirmation = null;
        if (section == Section.Players) LoadPlayers();
        if (section == Section.Parameters) LoadParameters();
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await InvokeAsync(() =>
                {
                    if (cancellationToken.IsCancellationRequested || busy || current != Section.Services) return;
                    RefreshMonitor();
                    StateHasChanged();
                });
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
    }

    private void RefreshMonitor() => Run(() => status = Workspace.Operations.Snapshot());

    private Task RequestOperationAsync(string action, string target)
    {
        if (!status.Services.Any(service => service.Connections > 0 && service.CanStop &&
            (target == "all" || service.Id == target))) return OperateAsync(action, target);
        confirmation = (action, target);
        return Task.CompletedTask;
    }

    private async Task OperateAsync(string action, string target)
    {
        if (busy) return;
        busy = true;
        confirmation = null;
        try
        {
            IReadOnlyList<OperationRecord> results = await Workspace.Operations.ExecuteAsync(action, target);
            SetNotice(string.Join(" · ", results.Select(result => $"{result.Service} {result.Message}")),
                results.Any(result => !result.Success));
        }
        catch (Exception error) when (IsOperationalError(error)) { SetNotice(error.Message, true); }
        finally
        {
            RefreshMonitor();
            busy = false;
        }
    }

    private void LoadParameters() => Run(() => parameters = Workspace.Settings.Read());

    private void SaveParameters() => Run(() =>
    {
        Workspace.Settings.Save(parameters!);
        SetNotice("บันทึกแล้ว กด Restart all เพื่อใช้ค่าเริ่มต้นและการเชื่อมต่อชุดใหม่");
    });

    private void LoadPlayers() => Run(() =>
    {
        parameters = Workspace.Settings.Read();
        characters = Workspace.Players.Characters();
        grants = Workspace.Players.RecentGrants();
        if (characters.All(character => character.Id != recipient)) recipient = 0;
    });

    private void ToggleAccount() => Run(() =>
    {
        AdminParameters saved = Workspace.Settings.Read();
        AdminParameters updated = saved with { LocalAccountEnabled = !saved.LocalAccountEnabled };
        Workspace.Settings.Save(updated);
        parameters = updated;
        SetNotice("บันทึกสถานะบัญชีแล้ว กด Restart all เพื่อให้มีผล");
    });

    private void QueueGrant() => Run(() =>
    {
        Workspace.Players.QueueGrant(requestId, recipient, grantKind, grantKind == GrantKind.Item ? itemId : null, amount);
        requestId = Guid.NewGuid();
        recipient = 0;
        itemId = null;
        amount = 1;
        SetNotice("เพิ่มรายการเข้าคิวแล้ว สถานะรอระบบเกม");
        grants = Workspace.Players.RecentGrants();
    });

    private void SetNotice(string text, bool isError = false)
    {
        message = text;
        failed = isError;
    }

    private void Run(Action action)
    {
        try { action(); }
        catch (Exception error) when (IsOperationalError(error)) { SetNotice(error.Message, true); }
    }

    private static bool IsOperationalError(Exception error) => error is IOException or UnauthorizedAccessException
        or ArgumentException or JsonException or InvalidOperationException or Win32Exception or NetworkInformationException;

    public async ValueTask DisposeAsync()
    {
        await monitorLifetime.CancelAsync();
        if (monitor is not null) await monitor;
        monitorLifetime.Dispose();
    }
}
