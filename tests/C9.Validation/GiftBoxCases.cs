using System.Buffers.Binary;
using System.Text;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    private sealed class FakeGiftBox(params GiftMail[] gifts) : IGiftBoxStore
    {
        private readonly List<GiftMail> pending = [.. gifts];
        public List<InventoryItem> Items { get; } = [];
        public List<ulong> Characters { get; } = [];
        public bool Broken { get; set; }

        private void Use(ulong characterId)
        {
            if (Broken) throw new ProtocolException(ProtocolError.InvalidPacket, "Game database is not ready: test");
            Characters.Add(characterId);
        }

        public ValueTask<IReadOnlyList<GiftMail>> ListAsync(ulong characterId, CancellationToken token)
        { Use(characterId); return ValueTask.FromResult<IReadOnlyList<GiftMail>>(pending.ToArray()); }

        public ValueTask<GiftMail?> ReadAsync(ulong characterId, ulong mailNo, CancellationToken token)
        {
            Use(characterId);
            int index = pending.FindIndex(g => g.MailNo == mailNo);
            if (index < 0) return ValueTask.FromResult<GiftMail?>(null);
            pending[index] = pending[index] with { Opened = true };
            return ValueTask.FromResult<GiftMail?>(pending[index]);
        }

        public ValueTask<GiftClaimResult> ClaimAsync(ulong characterId, ulong mailNo, CancellationToken token)
        {
            Use(characterId);
            GiftMail? gift = pending.FirstOrDefault(g => g.MailNo == mailNo);
            if (gift is null) return ValueTask.FromResult(new GiftClaimResult(GiftClaimStatus.NotFound, null));
            pending.Remove(gift);
            InventoryItem item = new(1000 + mailNo, gift.ItemId, gift.Quantity, 0, (short)Items.Count);
            Items.Add(item);
            return ValueTask.FromResult(new GiftClaimResult(GiftClaimStatus.Claimed, item));
        }

        public ValueTask<bool> DismissAsync(ulong characterId, ulong mailNo, CancellationToken token)
        { Use(characterId); return ValueTask.FromResult(pending.RemoveAll(g => g.MailNo == mailNo) > 0); }

        public ValueTask<IReadOnlyList<InventoryItem>> InventoryAsync(ulong characterId, CancellationToken token)
        { Use(characterId); return ValueTask.FromResult<IReadOnlyList<InventoryItem>>(Items.ToArray()); }
    }

    private static readonly DateTimeOffset GiftTime = new(2026, 11, 26, 3, 0, 0, TimeSpan.Zero);

    private static string Utf16(ReadOnlySpan<byte> field) => Encoding.Unicode.GetString(field).TrimEnd('\0');

    private static void GiftBoxPacketLayouts()
    {
        Check.Bytes(new byte[] { 3, 0, 0, 0 }, GiftBoxPackets.Count(3).Payload.Span);
        Check.Equal((byte)1, GiftBoxPackets.Count(3).Flag);

        GiftMail[] gifts = Enumerable.Range(1, 16).Select(i =>
            new GiftMail((ulong)i, 20000 + i, i, i == 1 ? "Welcome to the local server!" : null, GiftTime, i == 2)).ToArray();
        IReadOnlyList<Packet> list = GiftBoxPackets.List(gifts);
        Check.Equal(2, list.Count);
        Check.True(list.All(p => p.Type == 0x0EA0 && p.Payload.Length == 1 + 15 * 106 && p.ToDecodedBytes().Length == 1604));
        Check.Equal((byte)15, list[0].Payload.Span[0]);
        Check.Equal((byte)1, list[1].Payload.Span[0]);
        ReadOnlySpan<byte> first = list[0].Payload.Span.Slice(1, 106);
        Check.Equal(1UL, BinaryPrimitives.ReadUInt64LittleEndian(first));
        Check.Equal((byte)1, first[8]); // EV_MT_BILLING, the Gift Chest tab
        Check.Equal("GM", Utf16(first.Slice(0x09, 42)));
        Check.Equal("Welcome to t", Utf16(first.Slice(0x33, 26))); // wchar[13] keeps 12 characters + terminator
        Check.Equal(0, BinaryPrimitives.ReadInt32LittleEndian(first[0x4D..]));
        Check.Equal(20001, BinaryPrimitives.ReadInt32LittleEndian(first[0x51..]));
        Check.Equal(1, BinaryPrimitives.ReadInt32LittleEndian(first[0x55..]));
        Check.Equal(GiftTime.ToUnixTimeSeconds(), BinaryPrimitives.ReadInt64LittleEndian(first[0x59..]));
        Check.Equal((byte)0, first[0x61]);
        Check.Equal(GiftTime.AddDays(365).ToUnixTimeSeconds(), BinaryPrimitives.ReadInt64LittleEndian(first[0x62..]));
        ReadOnlySpan<byte> second = list[0].Payload.Span.Slice(1 + 106, 106);
        Check.Equal("Gift", Utf16(second.Slice(0x33, 26)));
        Check.Equal((byte)1, second[0x61]);
        Check.Equal(16UL, BinaryPrimitives.ReadUInt64LittleEndian(list[1].Payload.Span[1..]));
        IReadOnlyList<Packet> empty = GiftBoxPackets.List([]);
        Check.Equal(1, empty.Count);
        Check.Equal((byte)0, empty[0].Payload.Span[0]);

        Packet content = GiftBoxPackets.Content(gifts[0]);
        Check.Equal((ushort)0x0E9E, content.Type);
        Check.Equal(178, content.Payload.Length);
        Check.Equal(1UL, BinaryPrimitives.ReadUInt64LittleEndian(content.Payload.Span));
        Check.Equal("Welcome to the local server!", Utf16(content.Payload.Span.Slice(8, 162)));
        Check.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(content.Payload.Span[0xAA..]));
        Check.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(content.Payload.Span[0xAE..]));
        Check.Equal("Item gift from the GM.", Utf16(GiftBoxPackets.Content(gifts[1]).Payload.Span.Slice(8, 162)));

        Packet failed = GiftBoxPackets.OpenFailed(77);
        Check.Equal((ushort)0x0EB3, failed.Type);
        Check.Equal(12, failed.Payload.Length);
        Check.Equal(0, BinaryPrimitives.ReadInt32LittleEndian(failed.Payload.Span));
        Check.Equal(77UL, BinaryPrimitives.ReadUInt64LittleEndian(failed.Payload.Span[4..]));

        InventoryItem potion = new(0x1122334455667788, 1001, 5, 1, 3);
        Packet add = GiftBoxPackets.ItemAdd(42, potion, GiftBoxPackets.ReasonAddMailEx);
        Check.Equal((ushort)0x0CFB, add.Type);
        Check.Equal(377, add.Payload.Length); // 390-byte packet: the 0CFB handler reads the party list at packet +0x146
        ReadOnlySpan<byte> a = add.Payload.Span;
        Check.Equal(42UL, BinaryPrimitives.ReadUInt64LittleEndian(a));
        Check.Equal(31, BinaryPrimitives.ReadInt32LittleEndian(a[8..]));
        Check.Equal((byte)1, a[12]);
        Check.Equal(1001, BinaryPrimitives.ReadInt32LittleEndian(a[13..]));
        Check.Equal(0x1122334455667788UL, BinaryPrimitives.ReadUInt64LittleEndian(a[21..]));
        Check.Equal(5, BinaryPrimitives.ReadInt32LittleEndian(a[(13 + 0x14)..]));
        Check.Equal(5u, BinaryPrimitives.ReadUInt32LittleEndian(a[309..])); // packet +0x142 m_nAddStackCount

        List<InventoryItem> stacks = [.. Enumerable.Range(0, 50).Select(i => new InventoryItem((ulong)(i + 1), 500 + i, 1, 0, (short)(49 - i))), potion];
        IReadOnlyList<Packet> inventory = GiftBoxPackets.Inventory(stacks);
        Check.Equal(3, inventory.Count);
        Check.True(inventory.All(p => p.Type == 0x0BC1 && p.Flag == 1));
        (int Sequence, byte End, int Count, byte Tab, int Length)[] headers = inventory.Select(p => (
            (int)BinaryPrimitives.ReadUInt16LittleEndian(p.Payload.Span), p.Payload.Span[2],
            BinaryPrimitives.ReadInt32LittleEndian(p.Payload.Span[3..]), p.Payload.Span[7], p.Payload.Length)).ToArray();
        Check.True(headers.SequenceEqual(new[] { (0, (byte)0, 48, (byte)0, 8 + 48 * 112), (1, (byte)1, 2, (byte)0, 8 + 2 * 112), (0, (byte)1, 1, (byte)1, 8 + 112) }),
            "0BC1 sequence/end/count/tab per bag");
        ReadOnlySpan<byte> slot0 = inventory[0].Payload.Span.Slice(8, 112); // ordered by slot: slot 0 holds item 549
        Check.Equal(549, BinaryPrimitives.ReadInt32LittleEndian(slot0));
        Check.Equal(50UL, BinaryPrimitives.ReadUInt64LittleEndian(slot0[8..]));
        Check.Equal(1, BinaryPrimitives.ReadInt32LittleEndian(slot0[0x14..]));
        Check.Equal((byte)0, slot0[104]);
        Check.Equal((byte)1, inventory[2].Payload.Span[8 + 104]);
        Check.Equal(0, GiftBoxPackets.Inventory([]).Count);

        foreach (ushort type in new ushort[] { 0x0EB1, 0x0E97, 0x0E9D, 0x0E9B, 0x0E99 }) Check.True(GiftBoxPackets.Handles(type));
        Check.True(!GiftBoxPackets.Handles(0x0E93) && !GiftBoxPackets.Handles(0x0E95) && !GiftBoxPackets.Handles(0x0E91));
        foreach (Packet wrong in new[] { new Packet(0x0EB1, new byte[4]), new Packet(0x0E97, new byte[1]), new Packet(0x0E9B, new byte[4]),
            new Packet(0x0E9D, new byte[9]), new Packet(0x0E99) })
            Check.True(GiftBoxPackets.ReplyAsync(new FakeGiftBox(), wrong, 1, default).AsTask().GetAwaiter().GetResult() is null);
    }

    // World entry with a Gift Box store: inventory after 0x0BC0, the real 0EB1 count, then read / take / delete in the window.
    private static async Task MemoryGiftBoxSession()
    {
        string store = Path.Combine(Path.GetTempPath(), $"c9-gift-box-{Guid.NewGuid():N}.json");
        try
        {
            ServerOptions config = new()
            {
                Profile = "character_lobby", CharacterStore = store, WorldVitals = WorldVitalsLayout.Extended,
                TimeoutSeconds = 3, MaxPackets = 255
            };
            FakeGiftBox gifts = new(new GiftMail(7, 1001, 3, "Starter pack", GiftTime, false), new GiftMail(8, 1002, 1, null, GiftTime, false));
            gifts.Items.Add(new InventoryItem(900, 1500, 1, 0, 0));
            CollectedEvents events = new();
            await using MemorySession session = new(config, new SessionProcessor(config, events, giftBox: gifts), events);
            Check.Equal((ushort)4, (await session.ReceiveAsync()).Type);
            await session.SendAsync(new Packet(0x1771, new byte[49]));
            Check.Equal((ushort)0x0EFA, (await session.ReceiveAsync()).Type);
            byte[] create = new byte[90];
            create[0] = 2;
            Encoding.Unicode.GetBytes("GiftBox").CopyTo(create, 3);
            await session.SendAsync(new Packet(0x0BBB, create));
            byte[] identity = (await session.ReceiveAsync()).Payload.Span[..8].ToArray();
            ulong characterId = BinaryPrimitives.ReadUInt64LittleEndian(identity);
            await session.SendAsync(new Packet(0x0BBF, identity));
            Check.Equal((ushort)0x0BC0, (await session.ReceiveAsync()).Type);
            Packet inventory = await session.ReceiveAsync();
            Check.Equal((ushort)0x0BC1, inventory.Type);
            Check.Equal(900UL, BinaryPrimitives.ReadUInt64LittleEndian(inventory.Payload.Span[16..]));
            Check.Equal((ushort)0x0BC3, (await session.ReceiveAsync()).Type);

            await session.SendAsync(new Packet(0x0EB1));
            Packet count = await session.ReceiveAsync();
            Check.Equal((ushort)0x0EB2, count.Type);
            Check.Equal(2, BinaryPrimitives.ReadInt32LittleEndian(count.Payload.Span));
            Check.Equal((byte)2, (await session.ReceiveAsync()).Payload.Span[0]); // 0EA0 with both gifts

            await session.SendAsync(new Packet(0x0E97, new byte[] { 0, 1 }));
            Check.Equal((ushort)0x0EA0, (await session.ReceiveAsync()).Type);

            byte[] seven = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(seven, 7);
            await session.SendAsync(new Packet(0x0E9D, seven));
            Packet content = await session.ReceiveAsync();
            Check.Equal((ushort)0x0E9E, content.Type);
            Check.Equal("Starter pack", Utf16(content.Payload.Span.Slice(8, 162)));

            await session.SendAsync(new Packet(0x0E9B, seven));
            Packet added = await session.ReceiveAsync();
            Check.Equal((ushort)0x0CFB, added.Type);
            Check.Equal(characterId, BinaryPrimitives.ReadUInt64LittleEndian(added.Payload.Span));
            Check.Equal(1007UL, BinaryPrimitives.ReadUInt64LittleEndian(added.Payload.Span[21..]));
            foreach (ushort type in new ushort[] { 0x0E9C, 0x0E9A })
            {
                Packet ack = await session.ReceiveAsync();
                Check.Equal(type, ack.Type);
                Check.Bytes(seven, ack.Payload.Span);
            }
            // Taking it again (stale window) fails instead of duplicating the item.
            await session.SendAsync(new Packet(0x0E9B, seven));
            Check.Equal((ushort)0x0EB3, (await session.ReceiveAsync()).Type);
            Check.Equal(2, gifts.Items.Count);

            byte[] eight = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(eight, 8);
            await session.SendAsync(new Packet(0x0E99, eight));
            Check.Equal((ushort)0x0E9A, (await session.ReceiveAsync()).Type);
            await session.SendAsync(new Packet(0x0EB1));
            Packet zero = await session.ReceiveAsync();
            Check.Bytes(new byte[4], zero.Payload.Span);

            // A database failure leaves the player in the world with an empty Gift Box.
            gifts.Broken = true;
            await session.SendAsync(new Packet(0x0EB1));
            Packet empty = await session.ReceiveAsync();
            Check.Bytes(new byte[4], empty.Payload.Span);
            await session.SendAsync(new Packet(0x0EFD, flag: 0));
            Check.Equal((ushort)0x0EFE, (await session.ReceiveAsync()).Type);
            Check.True(gifts.Characters.All(c => c == characterId), "every Gift Box call uses the selected character");
            Check.True(events.Values.Any(e => e.Event == "gift_box_error"));
            await session.CompleteInputAsync();
            await session.Completion.WaitAsync(TimeSpan.FromSeconds(2));
            Check.True(!events.Values.Any(e => e.Event == "session_error"));
        }
        finally { File.Delete(store); }
    }
}
