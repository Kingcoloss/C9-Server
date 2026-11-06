using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using C9.Protocol;

namespace C9.Server;

// This store belongs to the single, synthetic account on the loopback lab.
internal sealed class CharacterLobby
{
    private const int SlotCount = 3;
    private const int NameBytes = 42;
    private const int AppearanceBytes = 44;
    private const int ListHeaderBytes = 42;
    private const int ListEntryBytes = 136;
    // Diagnostic baseline, pending extraction of the original level/job tables.
    private const uint InitialHp = 100;
    private const uint InitialMp = 100;
    private static readonly UnicodeEncoding NameEncoding = new(false, false, true);

    // Indexed by the asset job code. The 0BBB request sends the template instead.
    // Values come from the selected Client's
    // asset table (record +0x04 is the job code, record +0x48 is the template).
    private static readonly byte[] TemplateIds =
    [
        0, 0, 5, 10, 11, 12, 13, 14, 15, 1, 6, 16, 17, 18, 19, 20, 21, 3, 8, 28,
        29, 30, 31, 32, 33, 2, 7, 22, 23, 24, 25, 26, 27, 4, 9, 34, 35, 36, 37, 38, 39
    ];

    private sealed record Character(byte Slot, ushort Job, string Name, byte[] Appearance)
    {
        [JsonIgnore] public ulong Id => (ulong)Slot + 1;
        [JsonIgnore] public byte Template => TemplateIds[Job];
    }

    private readonly object gate = new();
    private readonly string path;
    private Character[] characters;

    public CharacterLobby(string path)
    {
        this.path = Path.GetFullPath(path);
        characters = File.Exists(this.path)
            ? JsonSerializer.Deserialize<Character[]>(File.ReadAllBytes(this.path)) ?? throw new InvalidDataException("Empty character store")
            : [];
        if (characters.Length > SlotCount || characters.Any(c => c is null || c.Slot >= SlotCount ||
            !ValidJob(c.Job) || !ValidName(c.Name) || c.Appearance?.Length != AppearanceBytes) ||
            characters.Select(c => c.Slot).Distinct().Count() != characters.Length ||
            characters.Select(c => c.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != characters.Length)
            throw new InvalidDataException("Invalid local character store");
    }

    public Packet List()
    {
        lock (gate)
        {
            byte[] payload = new byte[ListHeaderBytes + characters.Length * ListEntryBytes];
            payload[0] = 1;
            payload[8] = 1; // Enables creation when the account has an unused slot (00CF65DB).
            payload[40] = SlotCount;
            payload[41] = (byte)characters.Length;
            for (int index = 0; index < characters.Length; index++)
            {
                Character character = characters[index];
                Span<byte> entry = payload.AsSpan(ListHeaderBytes + index * ListEntryBytes, ListEntryBytes);
                BinaryPrimitives.WriteUInt64LittleEndian(entry, character.Id);
                NameEncoding.GetBytes(character.Name, entry.Slice(0x08, NameBytes));
                BinaryPrimitives.WriteUInt32LittleEndian(entry[0x32..], character.Slot);
                BinaryPrimitives.WriteUInt16LittleEndian(entry[0x36..], 1); // Level.
                BinaryPrimitives.WriteUInt32LittleEndian(entry[0x38..], character.Template);
                // 00A394A1 initializes these two DWORDs to -1 after creation;
                // 00A3A7F8 loads them from the list. Zero means pending deletion.
                BinaryPrimitives.WriteUInt64LittleEndian(entry[0x3C..], ulong.MaxValue);
            }
            return new Packet(0x0EFA, payload);
        }
    }

    public Packet Create(ReadOnlySpan<byte> payload)
    {
        if (payload.Length != 90)
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid character creation length");
        byte slot = payload[0];
        ushort template = BinaryPrimitives.ReadUInt16LittleEndian(payload[1..]);
        int jobIndex = template < 40 ? Array.IndexOf(TemplateIds, (byte)template, 1) : -1;
        if (slot >= SlotCount)
            throw new ProtocolException(ProtocolError.InvalidPacket, $"Unsupported local character slot {slot}");
        if (jobIndex < 1)
            throw new ProtocolException(ProtocolError.InvalidPacket, $"Unsupported local character template {template}");
        ushort job = (ushort)jobIndex;
        string name;
        try { name = NameEncoding.GetString(payload.Slice(3, NameBytes)).TrimEnd('\0'); }
        catch (DecoderFallbackException)
        { throw new ProtocolException(ProtocolError.InvalidPacket, "Invalid character name encoding"); }
        if (!ValidName(name))
            throw new ProtocolException(ProtocolError.InvalidPacket, "Invalid local character name");
        if (payload[89] != 0)
            throw new ProtocolException(ProtocolError.InvalidPacket, $"Unsupported local character mode {payload[89]}");
        ReadOnlySpan<byte> appearance = payload.Slice(45, AppearanceBytes);

        lock (gate)
        {
            Character? existing = characters.FirstOrDefault(c => c.Slot == slot);
            if (existing is not null && existing.Job == job && existing.Name == name && appearance.SequenceEqual(existing.Appearance))
                return Created(existing); // A retry after the original reply was lost.
            if (existing is not null || characters.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ProtocolException(ProtocolError.InvalidPacket, "Character name or slot is already in use");

            Character character = new(slot, job, name, appearance.ToArray());
            Character[] updated = [.. characters, character];
            Save(updated);
            characters = updated;
            return Created(character);
        }
    }

    public Packet Details(ReadOnlySpan<byte> request)
    {
        if (request.Length != sizeof(ulong))
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid character details length");
        ulong id = BinaryPrimitives.ReadUInt64LittleEndian(request);
        lock (gate)
        {
            Character character = characters.FirstOrDefault(c => c.Id == id)
                ?? throw new ProtocolException(ProtocolError.InvalidPacket, "Unknown local character");
            byte[] payload = new byte[sizeof(ulong) + AppearanceBytes + 1];
            request.CopyTo(payload);
            character.Appearance.CopyTo(payload.AsSpan(sizeof(ulong)));
            return new Packet(0x0EFC, payload); // Zero equipment entries for a new character.
        }
    }

    public Packet Select(ReadOnlySpan<byte> request)
    {
        if (request.Length != sizeof(ulong))
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid character selection length");
        ulong id = BinaryPrimitives.ReadUInt64LittleEndian(request);
        lock (gate)
        {
            Character character = characters.FirstOrDefault(c => c.Id == id)
                ?? throw new ProtocolException(ProtocolError.InvalidPacket, "Unknown local character");
            // 00A397F8 reads through decoded offset 0x1D20. Its two fixed
            // copies populate Player +0x10 (80 bytes) and +0x60 (168 bytes).
            byte[] payload = new byte[0x1D14];
            BinaryPrimitives.WriteUInt64LittleEndian(payload, character.Id);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), (uint)character.Id);
            // 00DA5130 -> 00DA5150 -> 00499F20 reads Player +0x14.
            // Zero skips actor creation, leaving CharacterLODState +0xE0 null
            // when the HUD calls 0078F5A0 after map loading.
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(12), character.Job);
            BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(16), 1);
            // 009EC510 reads Player +0x20/+0x24, not +0x28/+0x2C.
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(24), InitialHp);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(28), InitialMp);
            NameEncoding.GetBytes(character.Name, payload.AsSpan(44, NameBytes)); // 009D31E0 / 009A7A50.
            // Existing diagnostic progression fields; their full semantics
            // remain unresolved. The actor's class is the DWORD at +12 above.
            BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(0x6E), character.Job); // 009FA200.
            BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(0x70), 1); // 009ECA00.
            return new Packet(0x0BC0, payload);
        }
    }

    public static Packet InitialAttributes()
    {
        // 0C1E -> 00A3FBC3: two 168-byte attribute blocks, then a WORD
        // count of 8-byte entries. 00A41070/00A410A0 copy the fixed blocks.
        byte[] payload = new byte[338];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, InitialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(4), InitialMp);
        // 009EC5D0 scales both signed WORD factors by 0.001 before use.
        // Keep their diagnostic value at 1.0; exact attribute names pending.
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(0x2C), 1000);
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(0x2E), 1000);
        return new Packet(0x0C1E, payload);
    }

    public static Packet InitialVitals()
    {
        // 0C1C -> 00A320F0 -> 009EC370: level, current/max HP and MP.
        // The handler also reads optional progression fields through +0x4F
        // in the decoded packet. Leave those fields zero for the new character.
        byte[] payload = new byte[70];
        BinaryPrimitives.WriteUInt16LittleEndian(payload, 1);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(2), InitialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(6), InitialMp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(10), InitialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(14), InitialMp);
        return new Packet(0x0C1C, payload);
    }

    private static Packet Created(Character character)
    {
        byte[] payload = new byte[113];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, character.Id);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), character.Job);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(12), 1); // Level.
        payload[14] = character.Slot;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(15), character.Template);
        NameEncoding.GetBytes(character.Name, payload.AsSpan(17, NameBytes));
        character.Appearance.CopyTo(payload.AsSpan(61));
        return new Packet(0x0BBC, payload);
    }

    private void Save(Character[] updated)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (FileStream file = new(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(file, updated);
                file.Flush(flushToDisk: true);
            }
            File.Move(temporary, path, overwrite: true);
        }
        finally { File.Delete(temporary); }
    }

    private static bool ValidJob(ushort job) => job > 0 && job < TemplateIds.Length;
    private static bool ValidName(string? name) => name is { Length: > 0 and <= 20 } &&
        name.EnumerateRunes().All(r => Rune.IsLetterOrDigit(r) || r.Value == '_');
}
