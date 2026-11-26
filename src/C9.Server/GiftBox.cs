using System.Buffers.Binary;
using System.Text;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Protocol;

namespace C9.Server;

/// <summary>A pending gift as the Gift Box packets need it (mail number = delivery id).</summary>
public sealed record GiftMail(ulong MailNo, int ItemId, int Quantity, string? Message, DateTimeOffset CreatedAt, bool Opened);

/// <summary>An inventory stack as 0x0BC1/0x0CFB need it. <see cref="Serial"/> is the client's 64-bit item serial.</summary>
public sealed record InventoryItem(ulong Serial, int ItemId, int Quantity, byte Bag, short Slot);

public enum GiftClaimStatus { Claimed, NotFound, InventoryFull }

public sealed record GiftClaimResult(GiftClaimStatus Status, InventoryItem? Item);

/// <summary>Where the Gift Box and the inventory live: the game database (C9.GameData), or a test double.</summary>
public interface IGiftBoxStore
{
    ValueTask<IReadOnlyList<GiftMail>> ListAsync(ulong characterId, CancellationToken token);
    /// <summary>The gift, marked as opened; null when it is not this character's (or already gone).</summary>
    ValueTask<GiftMail?> ReadAsync(ulong characterId, ulong mailNo, CancellationToken token);
    ValueTask<GiftClaimResult> ClaimAsync(ulong characterId, ulong mailNo, CancellationToken token);
    ValueTask<bool> DismissAsync(ulong characterId, ulong mailNo, CancellationToken token);
    ValueTask<IReadOnlyList<InventoryItem>> InventoryAsync(ulong characterId, CancellationToken token);
}

/// <summary>
/// Gift Box = the Client's "MailEx" mail (Mailbox window, "Gift Chest" tab), decoded from the 2026-11 client image
/// (docs/ITEM_CATALOG_DELIVERY_TH.md section 7). Mail dispatcher 009131BB: 0EB2 count -> UI event 0x19 (bag icon),
/// 0EA0 list -> Inboxes[1] (the tab the buy-list panel 00AC1610 shows), 0E9E content, 0E9A delete, 0EB3 open failure.
/// Requests: 0E97 list {u8 type = 0, bool all = 1} (00914080), 0E9D content / 0E9B take / 0E99 delete {u64 mail no}
/// for every type except EV_MT_USER (00913EC0, 00913FA0).
/// </summary>
public static class GiftBoxPackets
{
    /// <summary>EF_MAILTYPE EV_MT_BILLING: the type of shop/gift mails; any type but EV_MT_USER (2) uses the MailEx requests.</summary>
    public const byte GiftMailType = 1;
    public const string Sender = "GM";
    public const int ListEntries = 15;          // PS_Mail_Ex_List_Ack::m_List[15]
    public const int ListEntryBytes = 106;      // PS_Mail_Ex_List_Ack::MailInfo
    public const int ItemBaseBytes = 104;       // PSDBItemBase in the 2026-11 client (0BC1 handler 009336FC copies 0x1A DWORDs)
    public const int InventoryEntryBytes = 112; // PSDBInventorySimpleInfo: item base + u8 tab, stride 0x70
    public const int InventoryEntries = 48;     // per 0BC1 packet
    public const int ItemPropertyBytes = 296;   // PSDBItemProperty in the 2026-11 client (0CFB handler 00935490)
    public const int ItemAddBytes = 13 + ItemPropertyBytes + 4 + 64;
    public const int ReasonAddMailEx = 31;      // PSItemChangeReason PS_ICR_ADD_MAIL_EX
    private static readonly TimeSpan DisplayedLifetime = TimeSpan.FromDays(365);

    public static bool Handles(ushort type) => type is 0x0EB1 or 0x0E97 or 0x0E9D or 0x0E9B or 0x0E99;

    /// <summary>Replies to one Gift Box request; null when the request has an unexpected length (left unanswered).</summary>
    public static async ValueTask<IReadOnlyList<Packet>?> ReplyAsync(IGiftBoxStore? store, Packet request, ulong characterId,
        CancellationToken token)
    {
        ReadOnlySpan<byte> payload = request.Payload.Span;
        ulong mailNo = payload.Length == 8 ? BinaryPrimitives.ReadUInt64LittleEndian(payload) : 0;
        switch (request.Type, payload.Length)
        {
            case (0x0EB1, 0):
            {
                // The legacy server answered with GetMailExCount + GetPcMailExList; the list fills the client's box early.
                IReadOnlyList<GiftMail> gifts = store is null ? [] : await store.ListAsync(characterId, token);
                return gifts.Count == 0 ? [Count(0)] : [Count(gifts.Count), .. List(gifts)];
            }
            case (0x0E97, 2):
                return List(store is null ? [] : await store.ListAsync(characterId, token));
            case (0x0E9D, 8):
                return store is not null && await store.ReadAsync(characterId, mailNo, token) is { } gift
                    ? [Content(gift)] : [OpenFailed(mailNo)];
            case (0x0E9B, 8):
            {
                if (store is null) return [OpenFailed(mailNo)];
                GiftClaimResult claim = await store.ClaimAsync(characterId, mailNo, token);
                return claim is { Status: GiftClaimStatus.Claimed, Item: { } item }
                    ? [ItemAdd(characterId, item, ReasonAddMailEx), new Packet(0x0E9C, MailNo(mailNo)), new Packet(0x0E9A, MailNo(mailNo))]
                    : [OpenFailed(mailNo)];
            }
            case (0x0E99, 8):
                // Always confirm, so a stale entry also leaves the client's box.
                if (store is not null) await store.DismissAsync(characterId, mailNo, token);
                return [new Packet(0x0E9A, MailNo(mailNo))];
            default:
                return null;
        }
    }

    /// <summary>0x0EB2 PS_Mail_Ex_Count_Ack {i32 count}: a count above zero shows the Gift Box icon.</summary>
    public static Packet Count(int count)
    {
        byte[] payload = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(payload, count);
        return new Packet(0x0EB2, payload);
    }

    /// <summary>0x0EA0 PS_Mail_Ex_List_Ack {u8 count, MailInfo[15]}: one packet per 15 gifts, at least one packet.</summary>
    public static IReadOnlyList<Packet> List(IReadOnlyList<GiftMail> gifts)
    {
        List<Packet> packets = [];
        for (int start = 0; start == 0 || start < gifts.Count; start += ListEntries)
        {
            byte[] payload = new byte[1 + ListEntries * ListEntryBytes];
            int count = Math.Min(ListEntries, gifts.Count - start);
            payload[0] = (byte)count;
            for (int index = 0; index < count; index++)
            {
                GiftMail gift = gifts[start + index];
                Span<byte> entry = payload.AsSpan(1 + index * ListEntryBytes, ListEntryBytes);
                BinaryPrimitives.WriteUInt64LittleEndian(entry, gift.MailNo);
                entry[0x08] = GiftMailType;
                WriteText(entry.Slice(0x09, 42), Sender);
                WriteText(entry.Slice(0x33, 26), Subject(gift));
                // +0x4D subject key 0: the client shows the subject text above (MailInfoEx::GetSubject 00914220).
                BinaryPrimitives.WriteInt32LittleEndian(entry[0x51..], gift.ItemId);
                BinaryPrimitives.WriteInt32LittleEndian(entry[0x55..], gift.Quantity);
                BinaryPrimitives.WriteInt64LittleEndian(entry[0x59..], gift.CreatedAt.ToUnixTimeSeconds()); // _localtime64_s in the row
                entry[0x61] = gift.Opened ? (byte)1 : (byte)0;
                BinaryPrimitives.WriteInt64LittleEndian(entry[0x62..], (gift.CreatedAt + DisplayedLifetime).ToUnixTimeSeconds());
            }
            packets.Add(new Packet(0x0EA0, payload));
        }
        return packets;
    }

    /// <summary>0x0E9E PS_Mail_Ex_Content_Ack {u64 mail no, wchar[81] text, u32 content key, u32 mail type}.</summary>
    public static Packet Content(GiftMail gift)
    {
        byte[] payload = new byte[8 + 162 + 4 + 4];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, gift.MailNo);
        WriteText(payload.AsSpan(8, 162), string.IsNullOrWhiteSpace(gift.Message) ? "Item gift from the GM." : gift.Message);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8 + 162 + 4), GiftMailType);
        return new Packet(0x0E9E, payload);
    }

    /// <summary>0x0EB3 PS_Mail_Ex_OpenFail_Ack {i32 reason (EXPIRE = 0), u64 mail no}.</summary>
    public static Packet OpenFailed(ulong mailNo)
    {
        byte[] payload = new byte[12];
        BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(4), mailNo);
        return new Packet(0x0EB3, payload);
    }

    /// <summary>0x0CFB PS_ItemAdd_Ack: the client checks the character id against its own and skips serial 0.</summary>
    public static Packet ItemAdd(ulong characterId, InventoryItem item, int reason)
    {
        byte[] payload = new byte[ItemAddBytes];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, characterId);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(8), reason);
        payload[12] = item.Bag;
        WriteItemBase(payload.AsSpan(13, ItemBaseBytes), item);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(13 + ItemPropertyBytes), (uint)item.Quantity); // m_nAddStackCount
        return new Packet(0x0CFB, payload);
    }

    /// <summary>
    /// 0x0BC1 PS_InventoryInfo_Ack {u16 sequence, u8 end, i32 count, u8 tab, entries}: per bag that has items, 48 per packet.
    /// Sequence 0 clears the tab (009350F0); the client places items in slots itself (slot map from 0x0CFA, not sent yet).
    /// </summary>
    public static IReadOnlyList<Packet> Inventory(IReadOnlyList<InventoryItem> items)
    {
        List<Packet> packets = [];
        foreach (IGrouping<byte, InventoryItem> bag in items.GroupBy(i => i.Bag).OrderBy(g => g.Key))
        {
            InventoryItem[] stacks = bag.OrderBy(i => i.Slot).ToArray();
            for (int start = 0, sequence = 0; start < stacks.Length; start += InventoryEntries, sequence++)
            {
                int count = Math.Min(InventoryEntries, stacks.Length - start);
                byte[] payload = new byte[8 + count * InventoryEntryBytes];
                BinaryPrimitives.WriteUInt16LittleEndian(payload, (ushort)sequence);
                payload[2] = start + count >= stacks.Length ? (byte)1 : (byte)0;
                BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(3), count);
                payload[7] = bag.Key;
                for (int index = 0; index < count; index++)
                {
                    Span<byte> entry = payload.AsSpan(8 + index * InventoryEntryBytes, InventoryEntryBytes);
                    WriteItemBase(entry[..ItemBaseBytes], stacks[start + index]);
                    entry[ItemBaseBytes] = bag.Key;
                }
                packets.Add(new Packet(0x0BC1, payload));
            }
        }
        return packets;
    }

    // PSDBItemBase: +0 i32 item id, +8 u64 serial, +0x10 i32 durability, +0x14 i32 stack count; the rest stays zero
    // (no enchant, lock, expiry or options yet). Durability of equipment is not decoded from the Client data yet.
    private static void WriteItemBase(Span<byte> target, InventoryItem item)
    {
        BinaryPrimitives.WriteInt32LittleEndian(target, item.ItemId);
        BinaryPrimitives.WriteUInt64LittleEndian(target[8..], item.Serial);
        BinaryPrimitives.WriteInt32LittleEndian(target[0x14..], item.Quantity);
    }

    private static string Subject(GiftMail gift) =>
        string.IsNullOrWhiteSpace(gift.Message) ? "Gift" : gift.Message.Trim();

    private static byte[] MailNo(ulong mailNo)
    {
        byte[] payload = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, mailNo);
        return payload;
    }

    // UTF-16LE, cut to leave room for the terminating zero of the fixed wchar array.
    private static void WriteText(Span<byte> target, string text)
    {
        int chars = Math.Min(text.Length, target.Length / 2 - 1);
        if (chars > 0 && char.IsHighSurrogate(text[chars - 1])) chars--;
        Encoding.Unicode.GetBytes(text.AsSpan(0, chars), target);
    }
}

internal static class GiftBoxStores
{
    /// <summary>The game database store; null without one (the lab answers an empty Gift Box).</summary>
    public static IGiftBoxStore? Open(ServerOptions options) =>
        options.GameDatabase is { } connection ? new GameDataGiftBoxStore(connection) : null;
}

/// <summary>Gift Box and inventory in C9.GameData (<see cref="GiftBoxService"/>).</summary>
internal sealed class GameDataGiftBoxStore(string connection) : IGiftBoxStore
{
    private GiftBoxService Service => GameDataProviders.Service<GiftBoxService>(connection);

    public async ValueTask<IReadOnlyList<GiftMail>> ListAsync(ulong characterId, CancellationToken token) =>
        (await Run(() => Service.ListAsync((long)characterId, token))).Select(Mail).ToArray();

    public async ValueTask<GiftMail?> ReadAsync(ulong characterId, ulong mailNo, CancellationToken token) =>
        mailNo > long.MaxValue ? null : await Run(() => Service.ReadAsync((long)characterId, (long)mailNo, token)) is { } gift ? Mail(gift) : null;

    public async ValueTask<GiftClaimResult> ClaimAsync(ulong characterId, ulong mailNo, CancellationToken token)
    {
        if (mailNo > long.MaxValue) return new(GiftClaimStatus.NotFound, null);
        GiftClaim claim = await Run(() => Service.ClaimAsync((long)characterId, (long)mailNo, token));
        return claim.Outcome switch
        {
            GiftClaimOutcome.Claimed when claim.Item is { } item => new(GiftClaimStatus.Claimed, Stack(item)),
            GiftClaimOutcome.InventoryFull => new(GiftClaimStatus.InventoryFull, null),
            _ => new(GiftClaimStatus.NotFound, null),
        };
    }

    public async ValueTask<bool> DismissAsync(ulong characterId, ulong mailNo, CancellationToken token) =>
        mailNo <= long.MaxValue && await Run(() => Service.DismissAsync((long)characterId, (long)mailNo, token));

    public async ValueTask<IReadOnlyList<InventoryItem>> InventoryAsync(ulong characterId, CancellationToken token) =>
        (await Run(() => Service.InventoryAsync((long)characterId, token))).Select(Stack).ToArray();

    private static GiftMail Mail(GiftView gift) =>
        new((ulong)gift.Id, gift.ItemId, gift.Quantity, gift.Message, gift.CreatedAt, gift.OpenedAt is not null);

    private static InventoryItem Stack(CharacterItem item) =>
        new((ulong)item.GameSerial, item.ItemId, item.Quantity, (byte)item.Bag, item.Slot);

    private async Task<T> Run<T>(Func<Task<T>> action)
    {
        try { return await action(); }
        catch (Exception error) when (GameDataProviders.Unavailable(error)) { throw GameDataProviders.NotReady(error); }
    }
}
