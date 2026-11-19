namespace C9.Server;

/// <summary>
/// Resolves the effective game configuration: an overlay written by the Admin Portal wins over the
/// base profile when it exists, then command-line overrides apply. Re-read on every (re)start.
/// </summary>
public sealed record ConfigSource(string BasePath, string? OverlayPath = null, string? HostOverride = null,
    int? PortOverride = null, string? CharacterStoreOverride = null, bool AllowContainerBind = false)
{
    public sealed record Loaded(ServerOptions Options, string Path, bool OverlayActive);

    public Loaded Load()
    {
        bool overlay = OverlayPath is not null && File.Exists(OverlayPath);
        string path = Path.GetFullPath(overlay ? OverlayPath! : BasePath);
        ServerOptions options = ServerOptions.FromJson(File.ReadAllText(path));
        options = options with
        {
            Host = HostOverride ?? options.Host,
            Port = PortOverride ?? options.Port,
            AllowContainerBind = AllowContainerBind
        };
        if (CharacterStoreOverride is not null && options.CharacterStore is not null)
            options = options with { CharacterStore = Path.GetFullPath(CharacterStoreOverride) };
        else if (options.CharacterStore is { } store)
            options = options with { CharacterStore = Path.GetFullPath(store, Path.GetDirectoryName(path)!) };
        options = options.WithGameDatabaseFromEnvironment().WithLauncherAuthFromEnvironment();
        options.Validate();
        return new(options, path, overlay);
    }
}
