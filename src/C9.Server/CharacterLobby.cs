using System.Buffers.Binary;
using System.Text;
using C9.Protocol;

namespace C9.Server;

// Packet layouts of the character lobby. Characters live in an ICharacterStore: the JSON file of the
// single, synthetic account on the loopback lab, or the game database when C9_DB_CONNECTION is set.
internal sealed class CharacterLobby
{
    internal const int LegacySlotCount = 3;
    private const int NameBytes = 42;
    internal const int AppearanceBytes = 44;
    internal const int LegacyListHeaderBytes = 42;
    internal const int ExtendedListHeaderBytes = 47;
    internal const int ListEntryBytes = 136;
    private readonly uint initialHp;
    private readonly uint initialMp;
    private static readonly UnicodeEncoding NameEncoding = new(false, false, true);

    // Indexed by the asset job code. The 0BBB request sends the template instead.
    // Values come from the selected Client's
    // asset table (record +0x04 is the job code, record +0x48 is the template).
    private static readonly byte[] TemplateIds =
    [
        0, 0, 5, 10, 11, 12, 13, 14, 15, 1, 6, 16, 17, 18, 19, 20, 21, 3, 8, 28,
        29, 30, 31, 32, 33, 2, 7, 22, 23, 24, 25, 26, 27, 4, 9, 34, 35, 36, 37, 38, 39
    ];

    private readonly ICharacterStore store;
    private readonly int listHeaderBytes;
    private readonly byte slotCount;

    public CharacterLobby(ServerOptions options, IServerEvents? events = null)
    {
        listHeaderBytes = options.CharacterListHeaderBytes;
        slotCount = checked((byte)options.CharacterSlots);
        initialHp = checked((uint)options.InitialHp);
        initialMp = checked((uint)options.InitialMp);
        store = CharacterStores.Open(options, events);
    }

    /// <summary>Game database account of the lobby (null with the JSON store); known after the first list.</summary>
    public long? AccountId => store.AccountId;

    private static byte Template(LobbyCharacter character) => TemplateIds[character.Job];

    public async ValueTask<Packet> ListAsync(CancellationToken token = default)
    {
        IReadOnlyList<LobbyCharacter> characters = await store.LoadAsync(token);
        {
            byte[] payload = new byte[listHeaderBytes + characters.Count * ListEntryBytes];
            payload[0] = 1;
            payload[8] = 1; // Enables creation when the account has an unused slot (00CF65DB).
            // The extended reader in 52893 uses decoded offsets 0x3A/0x3B.
            payload[listHeaderBytes - 2] = slotCount;
            payload[listHeaderBytes - 1] = (byte)characters.Count;
            for (int index = 0; index < characters.Count; index++)
            {
                LobbyCharacter character = characters[index];
                Span<byte> entry = payload.AsSpan(listHeaderBytes + index * ListEntryBytes, ListEntryBytes);
                BinaryPrimitives.WriteUInt64LittleEndian(entry, character.Id);
                NameEncoding.GetBytes(character.Name, entry.Slice(0x08, NameBytes));
                BinaryPrimitives.WriteUInt32LittleEndian(entry[0x32..], character.Slot);
                BinaryPrimitives.WriteUInt16LittleEndian(entry[0x36..], 1); // Level.
                BinaryPrimitives.WriteUInt32LittleEndian(entry[0x38..], Template(character));
                // 00A394A1 initializes these two DWORDs to -1 after creation;
                // 00A3A7F8 loads them from the list. Zero means pending deletion.
                BinaryPrimitives.WriteUInt64LittleEndian(entry[0x3C..], ulong.MaxValue);
            }
            return new Packet(0x0EFA, payload);
        }
    }

    public ValueTask<Packet> CreateAsync(ReadOnlySpan<byte> payload, CancellationToken token = default)
    {
        if (payload.Length != 90)
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid character creation length");
        byte slot = payload[0];
        ushort template = BinaryPrimitives.ReadUInt16LittleEndian(payload[1..]);
        int jobIndex = template < 40 ? Array.IndexOf(TemplateIds, (byte)template, 1) : -1;
        if (slot >= slotCount)
            throw new ProtocolException(ProtocolError.InvalidPacket, $"Unsupported local character slot {slot}");
        if (jobIndex < 1)
            throw new ProtocolException(ProtocolError.InvalidPacket, $"Unsupported local character template {template}");
        ushort job = (ushort)jobIndex;
        string name;
        try { name = NameEncoding.GetString(payload.Slice(3, NameBytes)).TrimEnd('\0'); }
        catch (DecoderFallbackException)
        { throw new ProtocolException(ProtocolError.InvalidPacket, "Invalid character name encoding"); }
        if (!CharacterStores.ValidName(name))
            throw new ProtocolException(ProtocolError.InvalidPacket, "Invalid local character name");
        if (payload[89] != 0)
            throw new ProtocolException(ProtocolError.InvalidPacket, $"Unsupported local character mode {payload[89]}");
        byte[] appearance = payload.Slice(45, AppearanceBytes).ToArray();
        return CreatedAsync(slot, job, name, appearance, token);
    }

    private async ValueTask<Packet> CreatedAsync(byte slot, ushort job, string name, byte[] appearance, CancellationToken token) =>
        Created(await store.AddAsync(slot, job, name, appearance, token));

    public ValueTask<Packet> DetailsAsync(ReadOnlySpan<byte> request, CancellationToken token = default)
    {
        if (request.Length != sizeof(ulong))
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid character details length");
        return DetailsAsync(BinaryPrimitives.ReadUInt64LittleEndian(request), token);
    }

    private async ValueTask<Packet> DetailsAsync(ulong id, CancellationToken token)
    {
        LobbyCharacter character = await FindAsync(id, token);
        {
            byte[] payload = new byte[sizeof(ulong) + AppearanceBytes + 1];
            BinaryPrimitives.WriteUInt64LittleEndian(payload, id);
            character.Appearance.CopyTo(payload.AsSpan(sizeof(ulong)));
            return new Packet(0x0EFC, payload); // Zero equipment entries for a new character.
        }
    }

    public ValueTask<Packet> SelectAsync(ReadOnlySpan<byte> request, CancellationToken token = default)
    {
        if (request.Length != sizeof(ulong))
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid character selection length");
        return SelectAsync(BinaryPrimitives.ReadUInt64LittleEndian(request), token);
    }

    private async ValueTask<LobbyCharacter> FindAsync(ulong id, CancellationToken token) =>
        (await store.LoadAsync(token)).FirstOrDefault(c => c.Id == id)
            ?? throw new ProtocolException(ProtocolError.InvalidPacket, "Unknown local character");

    private async ValueTask<Packet> SelectAsync(ulong id, CancellationToken token)
    {
        LobbyCharacter character = await FindAsync(id, token);
        {
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
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(24), initialHp);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(28), initialMp);
            NameEncoding.GetBytes(character.Name, payload.AsSpan(44, NameBytes)); // 009D31E0 / 009A7A50.
            // Existing diagnostic progression fields; their full semantics
            // remain unresolved. The actor's class is the DWORD at +12 above.
            BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(0x6E), character.Job); // 009FA200.
            BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(0x70), 1); // 009ECA00.
            return new Packet(0x0BC0, payload);
        }
    }

    public Packet InitialAttributes()
    {
        // 0C1E -> 00A3FBC3: two 168-byte attribute blocks, then a WORD
        // count of 8-byte entries. 00A41070/00A410A0 copy the fixed blocks.
        byte[] payload = new byte[338];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, initialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(4), initialMp);
        // 009EC5D0 scales both signed WORD factors by 0.001 before use.
        // Keep their diagnostic value at 1.0; exact attribute names pending.
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(0x2C), 1000);
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(0x2E), 1000);
        return new Packet(0x0C1E, payload);
    }

    public Packet InitialVitals()
    {
        // 0C1C -> 00A320F0 -> 009EC370: level, current/max HP and MP.
        // The handler also reads optional progression fields through +0x4F
        // in the decoded packet. Leave those fields zero for the new character.
        byte[] payload = new byte[70];
        BinaryPrimitives.WriteUInt16LittleEndian(payload, 1);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(2), initialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(6), initialMp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(10), initialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(14), initialMp);
        return new Packet(0x0C1C, payload);
    }

    public Packet ExtendedAttributes()
    {
        // Build b4580ed4 0C1E -> client 008CFF30: two 192-byte blocks start
        // at decoded payload +0x0C and +0xCC, then a WORD count of 8-byte
        // entries at +0x18C (zero for a fresh character), so 398 bytes total.
        // Block A is copied to the player attribute block (player+0x560); the
        // actor builder 00882C50 scales the signed WORD at block-A +0x2C
        // (payload +0x38) and +0x2E (payload +0x3A) by 0.001 into the actor's
        // move and action speed (actor+0x8A0/+0x8B8, read back by 007FCF20 /
        // 007FCEA0 and clamped to 1.7 / 1.3). A zero move factor left the
        // character standing but unable to walk, so both factors are 1.0.
        // HP/MP are applied by the 0C20 vitals packet, not here.
        byte[] payload = new byte[398];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, initialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), initialMp);
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(0x38), 1000);
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(0x3A), 1000);
        return new Packet(0x0C1E, payload);
    }

    public Packet ExtendedVitals(ulong characterId)
    {
        // 52893 00858506 -> 00812F20 checks the 64-bit character ID before
        // applying current HP/MP and their maxima to the local player.
        byte[] payload = new byte[32];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, characterId);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), initialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(16), initialMp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(20), initialHp);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(28), initialMp);
        return new Packet(0x0C20, payload);
    }

    private static Packet Created(LobbyCharacter character)
    {
        byte[] payload = new byte[113];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, character.Id);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8), character.Job);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(12), 1); // Level.
        payload[14] = character.Slot;
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(15), Template(character));
        NameEncoding.GetBytes(character.Name, payload.AsSpan(17, NameBytes));
        character.Appearance.CopyTo(payload.AsSpan(61));
        return new Packet(0x0BBC, payload);
    }

    internal static bool ValidJob(ushort job) => job > 0 && job < TemplateIds.Length;
}
