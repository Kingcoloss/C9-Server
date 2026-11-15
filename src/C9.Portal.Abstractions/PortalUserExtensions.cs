using System.Security.Claims;

namespace C9.Portal;

public static class PortalUserExtensions
{
    /// <summary>Actor name for audit entries.</summary>
    public static string ActorName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.Identity?.Name ?? "anonymous";
}
