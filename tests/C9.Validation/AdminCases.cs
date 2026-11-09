using System.Net.Sockets;
using C9.Admin;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private static void AdminWorkspaceStorage()
    {
        string workspace = Directory.CreateTempSubdirectory("c9-admin-test-").FullName;
        try
        {
            string configs = Directory.CreateDirectory(Path.Combine(workspace, "config")).FullName;
            foreach (string name in new[] { "lab-official-login-fixture.json", "lab-official-character-lobby.json" })
            {
                using var config = Fixture(name);
                File.WriteAllText(Path.Combine(configs, name), config.RootElement.GetRawText());
            }
            string state = Path.Combine(workspace, "artifacts", "admin-console");
            AdminSettings settings = new(workspace, state);
            AdminParameters defaults = settings.Read();
            Check.True(defaults.LocalAccountEnabled);
            Check.Equal(100, defaults.InitialMp);
            AdminParameters update = defaults with { LocalAccountEnabled = false, InitialMp = 175, AccountRates = new(Exp: 2.5m) };
            settings.Save(update);
            Check.Equal(update, new AdminSettings(workspace, state).Read());
            Check.Throws<ArgumentException>(() => settings.Save(update with { LobbyConnections = 0 }));
            Check.Throws<ArgumentException>(() => settings.Save(update with { EventRates = new(Upgrade: 101) }));
            Check.Equal(update, settings.Read());

            PlayerAdministration players = new(settings, state);
            Check.Equal(0, players.Characters().Count);
            File.WriteAllText(settings.CharacterStorePath, """[{"Slot":0,"Job":1,"Name":"Lab","Appearance":""}]""");
            Guid request = Guid.NewGuid();
            Check.Throws<ArgumentException>(() => players.QueueGrant(request, 2, GrantKind.Item, 17, 3));
            Check.Throws<ArgumentException>(() => players.QueueGrant(request, 1, GrantKind.Item, null, 3));
            players.QueueGrant(request, 1, GrantKind.Item, 17, 3);
            players.QueueGrant(request, 1, GrantKind.Item, 17, 3);
            Check.Throws<IOException>(() => players.QueueGrant(request, 1, GrantKind.Item, 17, 4));
            PlayerGrant grant = new PlayerAdministration(settings, state).RecentGrants().Single();
            Check.Equal("Lab", grant.CharacterName);
            Check.Equal(3L, grant.Amount);
        }
        finally { Directory.Delete(workspace, recursive: true); }
    }

    private static async Task TcpSuspendedLocalAccount()
    {
        using var fixture = Fixture("lab-official-login-fixture.json");
        ServerOptions login = ServerOptions.FromJson(fixture.RootElement.GetRawText()) with
        {
            Port = 0, LocalAccountEnabled = false, TimeoutSeconds = 2
        };
        await WithServer(async (server, events) =>
        {
            foreach (Packet packet in new[] { new Packet(0x2F04, new byte[1092], 0), new Packet(0x2EE8, new byte[28]) })
            {
                var (client, codec) = await ConnectAsync(server);
                using (client)
                {
                    await client.GetStream().WriteAsync(codec.Encode(packet));
                    await ClosedAsync(client);
                }
            }
            Check.Equal(2, events.Values.Count(entry => entry.Reason == "Local account is disabled"));
        }, login);

        string store = Path.Combine(Path.GetTempPath(), $"c9-disabled-{Guid.NewGuid():N}.json");
        ServerOptions lobby = new()
        {
            Port = 0, Profile = "character_lobby", CharacterStore = store, LocalAccountEnabled = false, TimeoutSeconds = 2
        };
        await WithServer(async (server, events) =>
        {
            var (client, codec) = await ConnectAsync(server, greetingMode: 1);
            using (client)
            {
                await client.GetStream().WriteAsync(codec.Encode(new Packet(0x1771, new byte[49])));
                await ClosedAsync(client);
            }
            Check.True(events.Values.Any(entry => entry.Reason == "Local account is disabled"));
            Check.True(!File.Exists(store));
        }, lobby);
    }
}
