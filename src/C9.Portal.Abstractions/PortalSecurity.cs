namespace C9.Portal;

/// <summary>Portal roles (see docs/SERVER_PLATFORM_ARCHITECTURE_TH.md, section RBAC).</summary>
public static class PortalRoles
{
    public const string Owner = "Owner";
    public const string Administrator = "Administrator";
    public const string GameMaster = "GM";
    public const string ContentEditor = "ContentEditor";
    public const string ReadOnly = "ReadOnly";
    public static IReadOnlyList<string> All { get; } = [Owner, Administrator, GameMaster, ContentEditor, ReadOnly];
}

/// <summary>Authorization policy names. Each policy is satisfied by the roles in <see cref="Roles"/>.</summary>
public static class PortalPolicies
{
    public const string ServerRead = "server.read";
    public const string ServerOperate = "server.operate";
    public const string TraceRead = "trace.read";
    /// <summary>Downloading raw/decoded bytes (pcap, JSONL) and viewing payload hex.</summary>
    public const string TraceExport = "trace.export";
    public const string SettingsWrite = "settings.write";
    public const string AccountsRead = "accounts.read";
    public const string AccountsWrite = "accounts.write";
    public const string WorldRead = "world.read";
    public const string WorldWrite = "world.write";
    public const string AuditRead = "audit.read";

    public static IReadOnlyDictionary<string, string[]> Roles { get; } = new Dictionary<string, string[]>
    {
        [ServerRead] = [.. PortalRoles.All],
        [ServerOperate] = [PortalRoles.Owner, PortalRoles.Administrator],
        [TraceRead] = [PortalRoles.Owner, PortalRoles.Administrator, PortalRoles.GameMaster],
        [TraceExport] = [PortalRoles.Owner, PortalRoles.Administrator],
        [SettingsWrite] = [PortalRoles.Owner, PortalRoles.Administrator],
        [AccountsRead] = [PortalRoles.Owner, PortalRoles.Administrator, PortalRoles.GameMaster, PortalRoles.ReadOnly],
        [AccountsWrite] = [PortalRoles.Owner, PortalRoles.Administrator, PortalRoles.GameMaster],
        [WorldRead] = [.. PortalRoles.All],
        [WorldWrite] = [PortalRoles.Owner, PortalRoles.Administrator, PortalRoles.ContentEditor],
        [AuditRead] = [PortalRoles.Owner, PortalRoles.Administrator],
    };
}

public sealed record PortalUser(string Id, string DisplayName, IReadOnlyList<string> Roles);

/// <param name="Login">Null for the single operator-key sign-in.</param>
public sealed record PortalCredentials(string? Login, string Secret);

/// <summary>
/// Validates portal sign-in. The default implementation accepts the operator key file and returns an
/// <see cref="PortalRoles.Owner"/>. A module may register a database-backed store; the last registration wins.
/// </summary>
public interface IPortalUserStore
{
    ValueTask<PortalUser?> ValidateAsync(PortalCredentials credentials, CancellationToken cancellationToken = default);
}
