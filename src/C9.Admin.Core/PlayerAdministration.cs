namespace C9.Admin;

public sealed record PlayerCharacter(byte Slot, ushort Job, string Name)
{
    public ulong Id => (ulong)Slot + 1;
}

public enum GrantKind { Item, Gold, Exp }

public sealed record PlayerGrant(Guid RequestId, DateTimeOffset QueuedAtUtc, ulong CharacterId, string CharacterName,
    GrantKind Kind, int? ItemId, long Amount);

public sealed class PlayerAdministration(AdminSettings settings, string stateDirectory)
{
    private readonly string queue = Path.Combine(stateDirectory, "pending-grants");

    public IReadOnlyList<PlayerCharacter> Characters() =>
        (LocalAdminFiles.Read<PlayerCharacter[]>(settings.CharacterStorePath) ?? [])
            .OrderBy(character => character.Slot).ToArray();

    public IReadOnlyList<PlayerGrant> RecentGrants() => Directory.Exists(queue)
        ? Directory.EnumerateFiles(queue, "*.json").OrderByDescending(File.GetLastWriteTimeUtc).Take(30)
            .Select(LocalAdminFiles.Read<PlayerGrant>).OfType<PlayerGrant>().ToArray()
        : [];

    public void QueueGrant(Guid requestId, ulong characterId, GrantKind kind, int? itemId, long amount)
    {
        if (requestId == Guid.Empty || !Enum.IsDefined(kind) || amount is < 1 or > int.MaxValue ||
            (kind == GrantKind.Item ? itemId is null or <= 0 : itemId is not null))
            throw new ArgumentException("กรุณาตรวจประเภท รหัสไอเท็ม และจำนวน");
        PlayerCharacter character = Characters().FirstOrDefault(entry => entry.Id == characterId)
            ?? throw new ArgumentException("ตัวละครนี้ไม่มีในบัญชี Local");
        string path = Path.Combine(queue, requestId.ToString("N") + ".json");
        PlayerGrant grant = new(requestId, DateTimeOffset.UtcNow, character.Id, character.Name, kind, itemId, amount);
        try { LocalAdminFiles.Write(path, grant, overwrite: false); }
        catch (IOException) when (LocalAdminFiles.Read<PlayerGrant>(path) is { } saved &&
            saved == grant with { QueuedAtUtc = saved.QueuedAtUtc }) { }
    }
}
