namespace C9.Portal;

public sealed class PortalOptions
{
    public string Url { get; set; } = "http://127.0.0.1:14880";
    /// <summary>Folder that contains <c>config/</c> (base game profiles).</summary>
    public string Workspace { get; set; } = ".";
    /// <summary>Portal state: operator key, audit log, parameter overlays.</summary>
    public string StateDirectory { get; set; } = "state";
    public string AgentTokenFile { get; set; } = "state/secrets/agent.token";
    public string? GrafanaUrl { get; set; }
    public string? PrometheusUrl { get; set; }
    /// <summary>IANA or Windows time zone used to show times in the UI (data stays UTC).</summary>
    public string DisplayTimeZone { get; set; } = "Asia/Bangkok";
    public List<ServiceOptions> Services { get; set; } = [];

    public sealed class ServiceOptions
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string AgentUrl { get; set; } = "";
        public int PublicPort { get; set; }
        public string? Description { get; set; }
    }

    public void Resolve(string contentRoot)
    {
        Workspace = Path.GetFullPath(Workspace, contentRoot);
        StateDirectory = Path.GetFullPath(StateDirectory, contentRoot);
        AgentTokenFile = Path.GetFullPath(AgentTokenFile, contentRoot);
        if (Services.Count == 0) throw new InvalidOperationException("Portal:Services must list at least one game service");
        if (Services.Select(s => s.Id).Distinct(StringComparer.Ordinal).Count() != Services.Count)
            throw new InvalidOperationException("Portal:Services ids must be unique");
        foreach (ServiceOptions service in Services)
            if (string.IsNullOrWhiteSpace(service.Id) || !Uri.TryCreate(service.AgentUrl, UriKind.Absolute, out Uri? uri) || uri.Scheme != Uri.UriSchemeHttp)
                throw new InvalidOperationException($"Portal service '{service.Id}' needs an id and an http:// AgentUrl");
    }
}
