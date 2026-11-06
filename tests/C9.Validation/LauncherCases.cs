using System.Buffers.Binary;
using System.Net;
using C9.Server;

internal static partial class Validation
{
    private static void LauncherVersion()
    {
        string directory = Path.Combine(Path.GetTempPath(), "c9-launcher-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string file = Path.Combine(directory, "version.dat");
        try
        {
            Check.Throws<FileNotFoundException>(() => LauncherUpdateServer.ReadInstalledVersion(directory));
            foreach (byte[] invalid in new byte[][] { [], [1, 2, 3], [1, 0, 0, 0, 0], [0, 0, 0, 0], [255, 255, 255, 255] })
            {
                File.WriteAllBytes(file, invalid);
                Check.Throws<InvalidDataException>(() => LauncherUpdateServer.ReadInstalledVersion(directory));
            }
            File.WriteAllBytes(file, [249, 3, 0, 0]);
            Check.Equal(1017u, LauncherUpdateServer.ReadInstalledVersion(directory));
        }
        finally
        {
            if (File.Exists(file)) File.Delete(file);
            Directory.Delete(directory);
        }
    }

    private static async Task LauncherHttp()
    {
        string directory = Path.Combine(Path.GetTempPath(), "c9-launcher-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string file = Path.Combine(directory, "version.dat");
        byte[] original = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(original, 1017);
        File.WriteAllBytes(file, original);
        try
        {
            await using var app = LauncherUpdateServer.Create(directory, 0, 16123);
            await app.StartAsync();
            try
            {
                using HttpClient http = new() { BaseAddress = new Uri(app.Urls.Single()), Timeout = TimeSpan.FromSeconds(5) };
                string path = LauncherUpdateServer.UpdatePath + "version.ini";
                using HttpResponseMessage response = await http.GetAsync(path);
                Check.Equal(HttpStatusCode.OK, response.StatusCode);
                string body = await response.Content.ReadAsStringAsync();
                Check.True(body.Contains("LatestVersion=1017\r\n", StringComparison.Ordinal));
                Check.True(body.Contains("MinimalVersion=1017\r\n", StringComparison.Ordinal));
                Check.True(body.Contains("OwnVersion=17\r\n", StringComparison.Ordinal));
                Check.True(body.Contains("[SERVER]\r\nCOUNT=1\r\nIP1=127.0.0.1\r\nPORT1=16123\r\n", StringComparison.Ordinal));
                Check.True(body.Contains($"server1={http.BaseAddress!.GetLeftPart(UriPartial.Authority)}{LauncherUpdateServer.UpdatePath}\r\n", StringComparison.Ordinal));
                Check.True(response.Headers.CacheControl?.NoStore == true);
                using HttpResponseMessage head = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, path));
                Check.Equal(HttpStatusCode.OK, head.StatusCode);
                Check.Equal(response.Content.Headers.ContentLength, head.Content.Headers.ContentLength);
                Check.Equal(0, (await head.Content.ReadAsByteArrayAsync()).Length);
                using HttpResponseMessage maintenance = await http.GetAsync(LauncherUpdateServer.UpdatePath + "maintenance.ini");
                Check.Equal(HttpStatusCode.OK, maintenance.StatusCode);
                Check.True((await maintenance.Content.ReadAsStringAsync()).Contains("IsUsed=0", StringComparison.Ordinal));
                foreach (string missing in new[] { "/version.dat", "/localinfo.dat", LauncherUpdateServer.UpdatePath + "1018/version.list", "/" })
                {
                    using HttpResponseMessage absent = await http.GetAsync(missing);
                    Check.Equal(HttpStatusCode.NotFound, absent.StatusCode);
                }
                using HttpResponseMessage post = await http.SendAsync(new HttpRequestMessage(HttpMethod.Post, path));
                Check.Equal(HttpStatusCode.MethodNotAllowed, post.StatusCode);
                Check.Bytes(original, File.ReadAllBytes(file));
            }
            finally { await app.StopAsync(); }
        }
        finally
        {
            File.Delete(file);
            Directory.Delete(directory);
        }
    }
}
