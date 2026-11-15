using C9.Admin;

namespace C9.Portal.Services;

/// <summary>
/// Game parameters shared with the legacy console (<see cref="AdminSettings"/>). Saving writes
/// parameters.local.json and the profile overlays that game services load on their next (re)start.
/// </summary>
public sealed class ParametersService(PortalOptions options)
{
    private readonly AdminSettings settings = new(options.Workspace, options.StateDirectory);

    public static IReadOnlyList<string> Profiles { get; } = [AdminSettings.LoginConfig, AdminSettings.LobbyConfig];

    public AdminParameters Read() => settings.Read();

    public IReadOnlyList<string> Save(AdminParameters parameters)
    {
        settings.Save(parameters);
        return Profiles.Select(settings.PrepareConfig).ToArray();
    }
}
