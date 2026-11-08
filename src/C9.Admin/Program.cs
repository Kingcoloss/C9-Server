using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using C9.Admin;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string workspace = Path.GetFullPath(builder.Configuration["workspace"] ?? Directory.GetCurrentDirectory());
int port = builder.Configuration.GetValue("port", 14880);
if (port is < 1 or > 65535 or 14000 or 14001 || !File.Exists(Path.Combine(workspace, "C9.slnx")))
    throw new InvalidOperationException("Choose the C9 workspace and a separate console port.");
string stateDirectory = Path.Combine(workspace, "artifacts", "admin-console");
string backend = Path.GetFullPath(builder.Configuration["backend"] ?? Path.Combine(stateDirectory, "game", "C9.Server.dll"));
string dotnet = Path.GetFullPath(builder.Configuration["dotnet-host"] ?? Environment.ProcessPath!);
string? clientDirectory = builder.Configuration["client-dir"];
Directory.CreateDirectory(stateDirectory);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Loopback, port);
    options.Limits.MaxRequestBodySize = 32 * 1024;
    options.AddServerHeader = false;
});
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/SignIn");
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/SignIn";
    options.Cookie.Name = $"C9.Admin.{port}";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = false;
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton(new OperatorKey(stateDirectory));
builder.Services.AddSingleton(new ServerOperations(workspace, stateDirectory, backend, dotnet, clientDirectory));
builder.Services.AddHostedService(services => services.GetRequiredService<ServerOperations>());

WebApplication app = builder.Build();
app.Use(async (context, next) =>
{
    if (!string.Equals(context.Request.Host.Host, "127.0.0.1", StringComparison.Ordinal) ||
        (context.Request.Host.Port ?? 80) != port || context.Connection.RemoteIpAddress is not { } remote || !IPAddress.IsLoopback(remote))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }
    context.Response.Headers.CacheControl = "no-store";
    context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; frame-ancestors 'none'; form-action 'self'; base-uri 'none'";
    context.Response.Headers.XContentTypeOptions = "nosniff";
    await next();
});
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
