using System.Security.Claims;
using C9.GameData.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace C9.Portal.GameData;

/// <summary>Component-side counterpart of <see cref="GameDataApi.Write"/> for Interactive Server pages.</summary>
public sealed class PortalActions(AuthenticationStateProvider authentication, IServiceProvider services)
{
    public string? Error { get; private set; }
    public string? Notice { get; private set; }

    public async Task<ClaimsPrincipal> UserAsync() => (await authentication.GetAuthenticationStateAsync()).User;

    /// <summary>Runs an audited change; returns default on failure and exposes the message in <see cref="Error"/>.</summary>
    public async Task<T?> RunAsync<T>(string action, string target, string? reason, Func<OperatorContext, Task<T>> work, string? success = null)
    {
        Error = Notice = null;
        ClaimsPrincipal user = await UserAsync();
        OperatorContext context = GameDataApi.Operator(user, reason, Guid.NewGuid());
        IAuditLog? audit = services.GetService(typeof(IAuditLog)) as IAuditLog;
        try
        {
            T result = await work(context);
            await GameDataApi.Audit(audit, context, action, target, true, null, null, CancellationToken.None);
            Notice = success ?? "บันทึกแล้ว";
            return result;
        }
        catch (GameDataException error)
        {
            await GameDataApi.Audit(audit, context, action, target, false, error.Message, null, CancellationToken.None);
            Error = error.Code switch
            {
                GameDataError.Conflict => "ข้อมูลถูกแก้โดยผู้อื่นหรือ game server แล้ว กรุณาโหลดใหม่แล้วลองอีกครั้ง",
                _ => error.Message
            };
            return default;
        }
    }

    public void Clear() => Error = Notice = null;
}
