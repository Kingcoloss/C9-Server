using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace C9.Server;

/// <summary>Local, no-patch manifest for an already installed research Client.</summary>
public static class LauncherUpdateServer
{
    public const string UpdatePath = "/C9Update/EN/";

    public static uint ReadInstalledVersion(string clientDirectory)
    {
        byte[] version = File.ReadAllBytes(Path.Combine(clientDirectory, "version.dat"));
        if (version.Length != sizeof(uint))
            throw new InvalidDataException("Client version.dat must contain exactly four bytes");
        uint value = BinaryPrimitives.ReadUInt32LittleEndian(version);
        if (value == 0 || value > int.MaxValue)
            throw new InvalidDataException("Client version.dat is outside the launcher's version range");
        return value;
    }

    public static string VersionManifest(uint version, int port, int loginPort = 14000) =>
        "[VERSION]\r\n" +
        $"LatestVersion={version.ToString(CultureInfo.InvariantCulture)}\r\n" +
        $"MinimalVersion={version.ToString(CultureInfo.InvariantCulture)}\r\n" +
        "OwnVersion=17\r\nCoreVersion=0\r\n" +
        // Launcher 00413570 enumerates server1, server2, ... and stops at the first gap.
        $"[DOWNLOAD]\r\nserver1=http://127.0.0.1:{port.ToString(CultureInfo.InvariantCulture)}{UpdatePath}\r\n" +
        // The original archive and C9.exe's version reader require this section.
        "[SERVER]\r\nCOUNT=1\r\nIP1=127.0.0.1\r\n" +
        $"PORT1={loginPort.ToString(CultureInfo.InvariantCulture)}\r\n";

    public static WebApplication Create(string clientDirectory, int port, int loginPort = 14000)
    {
        if (port is < 0 or > 65535) throw new ArgumentException("Invalid launcher update port");
        if (loginPort is < 1 or > 65535) throw new ArgumentException("Invalid advertised login port");
        uint version = ReadInstalledVersion(clientDirectory);
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { Args = [] });
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls($"http://127.0.0.1:{port.ToString(CultureInfo.InvariantCulture)}");
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 0;
            options.Limits.MaxConcurrentConnections = 8;
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(10);
        });
        WebApplication app = builder.Build();
        app.Run(async context =>
        {
            string path = context.Request.Path.Value ?? "";
            string? body = path switch
            {
                UpdatePath + "version.ini" => VersionManifest(version, context.Connection.LocalPort, loginPort),
                UpdatePath + "maintenance.ini" => "[MAINTENANCE]\r\nIsUsed=0\r\nMsg=Maintenance\r\n",
                UpdatePath + "delete.ini" => "",
                _ => null
            };
            bool read = HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method);
            context.Response.StatusCode = !read ? StatusCodes.Status405MethodNotAllowed :
                body is null ? StatusCodes.Status404NotFound : StatusCodes.Status200OK;
            context.Response.Headers.CacheControl = "no-store";
            if (!read) context.Response.Headers.Allow = "GET, HEAD";
            if (read && body is not null)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(body);
                context.Response.ContentType = "text/plain; charset=us-ascii";
                context.Response.ContentLength = bytes.Length;
                if (HttpMethods.IsGet(context.Request.Method))
                    await context.Response.Body.WriteAsync(bytes, context.RequestAborted);
            }
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                @event = "launcher_update_request", path = body is null ? "unknown" : path,
                status = context.Response.StatusCode, version
            }));
        });
        return app;
    }

    public static async Task RunAsync(string clientDirectory, int port, int loginPort = 14000)
    {
        await using WebApplication app = Create(clientDirectory, port, loginPort);
        await app.StartAsync();
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            @event = "launcher_update_listening", addresses = app.Urls,
            version = ReadInstalledVersion(clientDirectory), login_port = loginPort, mode = "installed_client_no_patch"
        }));
        await app.WaitForShutdownAsync();
    }
}
