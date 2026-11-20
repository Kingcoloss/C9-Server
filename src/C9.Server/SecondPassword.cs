using System.Buffers.Binary;
using System.Text;
using C9.GameData.Entities;
using C9.GameData.Services;
using C9.Protocol;

namespace C9.Server;

/// <summary>
/// The in-game secondary password (client dispatcher C9Network2ndPassword 0x0089B880, panel C9UISecondaryPasswordPanel).
/// The client never sends the password itself, only CRC32(UTF-16LE(password)); the security answer arrives as wchar[25].
/// Layouts and error codes: docs/EXIT_AND_2ND_PASSWORD_TH.md.
/// </summary>
internal static class SecondPasswordPackets
{
    public const ushort Set = 0x2744, SetAck = 0x2745, Reset = 0x2746, ResetAck = 0x2747;
    public const ushort Change = 0x2748, ChangeAck = 0x2749, Certify = 0x274A, CertifyAck = 0x274B;
    /// <summary>Generic error notice handled by C9NetworkDefault (0x008D8720).</summary>
    public const ushort Error = 0x0001;
    public const int ErrorBytes = 28;

    // Error codes are CRC32(UTF-16LE(name)) of the client's C9_ERROR_* strings.
    /// <summary>C9_ERROR_INVALID_2NDPASSWORD_1: wrong password, shows FailCnt (param1) of FailCntMax 5.</summary>
    public const uint InvalidPassword = 0xC22AA395;
    /// <summary>C9_ERROR_INVALID_2NDPASSWORD_2: the client switches to LOCKED with the lock start in param2.</summary>
    public const uint LockedByFailures = 0xE907F056;
    /// <summary>C9_ERROR_LOCKED_2NDPASSWORD.</summary>
    public const uint Locked = 0xE853BEC9;
    /// <summary>C9_ERROR_INVALID_2NDPASSWORD_Q_AND_A.</summary>
    public const uint InvalidAnswer = 0x48E08822;

    // 0x0BC0 select-ack fields read by case 0x008CBCD2; state computed by 0x008842B0.
    public const int SelectIsSet = 0x7234, SelectFailCount = 0x7235, SelectLockedAt = 0x7236, SelectCertified = 0x723F;

    private const int AnswerBytes = SecondPasswordPolicy.MaxAnswerLength * 2;
    private static readonly UnicodeEncoding AnswerEncoding = new(false, false, true);

    /// <summary>True for a secondary-password request with its exact client length (sender 0089C060/C130/C260/C1E0).</summary>
    public static bool IsRequest(Packet packet) => (packet.Type, packet.Payload.Length) is
        (Set, 63) or (Reset, 59) or (Change, 16) or (Certify, 4);

    public static uint SetCrc(ReadOnlySpan<byte> payload) => BinaryPrimitives.ReadUInt32LittleEndian(payload[8..]);
    public static byte SetQuestion(ReadOnlySpan<byte> payload) => payload[0x0C];
    public static string? SetAnswer(ReadOnlySpan<byte> payload) => Answer(payload.Slice(0x0D, AnswerBytes));
    public static byte ResetQuestion(ReadOnlySpan<byte> payload) => payload[8];
    public static string? ResetAnswer(ReadOnlySpan<byte> payload) => Answer(payload.Slice(9, AnswerBytes));
    public static uint ChangeCurrent(ReadOnlySpan<byte> payload) => BinaryPrimitives.ReadUInt32LittleEndian(payload[8..]);
    public static uint ChangeNext(ReadOnlySpan<byte> payload) => BinaryPrimitives.ReadUInt32LittleEndian(payload[0x0C..]);
    public static uint CertifyCrc(ReadOnlySpan<byte> payload) => BinaryPrimitives.ReadUInt32LittleEndian(payload);

    /// <summary>The UTF-16 answer up to its first NUL; null when it is not valid UTF-16.</summary>
    public static string? Answer(ReadOnlySpan<byte> field)
    {
        string text;
        try { text = AnswerEncoding.GetString(field); }
        catch (DecoderFallbackException) { return null; }
        int end = text.IndexOf('\0');
        return end < 0 ? text : text[..end];
    }

    /// <summary>The acknowledgement; the client does not read its payload. Flags follow the client's registration table.</summary>
    public static Packet Ack(ushort request) => request switch
    {
        Set => new Packet(SetAck, default, 0),
        Reset => new Packet(ResetAck, default, 0),
        Change => new Packet(ChangeAck, default, 0),
        Certify => new Packet(CertifyAck, default, 1),
        _ => throw new ArgumentOutOfRangeException(nameof(request))
    };

    /// <summary>0x0001: +8 request opcode, +0xA error code, +0xE popup, +0xF notice, +0x10 param1, +0x14 param2.</summary>
    public static Packet ErrorNotice(ushort request, uint code, uint param1 = 0, ulong param2 = 0)
    {
        byte[] payload = new byte[ErrorBytes];
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(8), request);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(0x0A), code);
        payload[0x0E] = 1;
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(0x10), param1);
        BinaryPrimitives.WriteUInt64LittleEndian(payload.AsSpan(0x14), param2);
        return new Packet(Error, payload, 1);
    }

    /// <summary>Writes the secondary-password fields of the 0x0BC0 select-ack (zero = no password).</summary>
    public static void WriteSelect(Span<byte> payload, SecondPasswordStatus status, bool certified)
    {
        if (!status.IsSet) return;
        payload[SelectIsSet] = 1;
        if (status.LockedAt is { } locked)
        {
            // The client reads the lock start only from a fail count of at least 5.
            payload[SelectFailCount] = SecondPasswordPolicy.MaxFailures;
            BinaryPrimitives.WriteUInt64LittleEndian(payload[SelectLockedAt..], (ulong)Math.Max(0, locked.ToUnixTimeSeconds()));
            return;
        }
        payload[SelectFailCount] = (byte)status.FailCount;
        if (certified) payload[SelectCertified] = 1;
    }

    public static ulong UnixSeconds(DateTimeOffset? at) => at is { } value ? (ulong)Math.Max(0, value.ToUnixTimeSeconds()) : 0;
}

/// <summary>Where secondary passwords are kept: the game database, or memory for the loopback lab.</summary>
internal interface ISecondPasswordStore
{
    ValueTask<SecondPasswordStatus> StatusAsync(long accountId, CancellationToken token);
    ValueTask<SecondPasswordResult> SetAsync(long accountId, uint crc, byte question, string answer, CancellationToken token);
    ValueTask<SecondPasswordResult> VerifyAsync(long accountId, uint crc, CancellationToken token);
    ValueTask<SecondPasswordResult> ChangeAsync(long accountId, uint current, uint next, CancellationToken token);
    ValueTask<SecondPasswordResult> ResetAsync(long accountId, byte question, string? answer, CancellationToken token);
    /// <summary>True when the account id must be a game database account.</summary>
    bool NeedsDatabaseAccount { get; }
}

internal static class SecondPasswordStores
{
    public static ISecondPasswordStore Open(ServerOptions options, TimeProvider? clock = null) =>
        options.GameDatabase is { } connection ? new GameDataSecondPasswordStore(connection) : new MemorySecondPasswordStore(clock ?? TimeProvider.System);
}

/// <summary>Lab store (no game database): rows live in memory for the life of the server, with the same rules.</summary>
internal sealed class MemorySecondPasswordStore(TimeProvider clock) : ISecondPasswordStore
{
    private readonly object gate = new();
    private readonly Dictionary<long, AccountSecondPassword> rows = new();

    public bool NeedsDatabaseAccount => false;

    public ValueTask<SecondPasswordStatus> StatusAsync(long accountId, CancellationToken token)
    {
        lock (gate) return ValueTask.FromResult(SecondPasswordPolicy.StatusOf(rows.GetValueOrDefault(accountId), clock.GetUtcNow()));
    }

    public ValueTask<SecondPasswordResult> SetAsync(long accountId, uint crc, byte question, string answer, CancellationToken token) =>
        Apply(accountId, (row, now) =>
        {
            if (row is not null) return (SecondPasswordOutcome.AlreadySet, row);
            AccountSecondPassword created = SecondPasswordPolicy.Create(accountId, crc, question, answer, now);
            rows[accountId] = created;
            return (SecondPasswordOutcome.Ok, created);
        });

    public ValueTask<SecondPasswordResult> VerifyAsync(long accountId, uint crc, CancellationToken token) =>
        Apply(accountId, (row, now) => row is null ? (SecondPasswordOutcome.NotSet, null) : (SecondPasswordPolicy.Verify(row, crc, now), row));

    public ValueTask<SecondPasswordResult> ChangeAsync(long accountId, uint current, uint next, CancellationToken token) =>
        Apply(accountId, (row, now) => row is null ? (SecondPasswordOutcome.NotSet, null) : (SecondPasswordPolicy.Change(row, current, next, now), row));

    public ValueTask<SecondPasswordResult> ResetAsync(long accountId, byte question, string? answer, CancellationToken token) =>
        Apply(accountId, (row, now) =>
        {
            if (row is null) return (SecondPasswordOutcome.NotSet, null);
            SecondPasswordOutcome outcome = SecondPasswordPolicy.CheckAnswer(row, question, answer, now);
            if (outcome != SecondPasswordOutcome.Ok) return (outcome, row);
            rows.Remove(accountId);
            return (outcome, null);
        });

    private ValueTask<SecondPasswordResult> Apply(long accountId,
        Func<AccountSecondPassword?, DateTimeOffset, (SecondPasswordOutcome, AccountSecondPassword?)> apply)
    {
        lock (gate)
        {
            DateTimeOffset now = clock.GetUtcNow();
            (SecondPasswordOutcome outcome, AccountSecondPassword? after) = apply(rows.GetValueOrDefault(accountId), now);
            return ValueTask.FromResult(new SecondPasswordResult(outcome, SecondPasswordPolicy.StatusOf(after, now)));
        }
    }
}

/// <summary>game.account_second_passwords through <see cref="SecondPasswordService"/> (audited, optimistic writes).</summary>
internal sealed class GameDataSecondPasswordStore(string connection) : ISecondPasswordStore
{
    private const string Operator = "c9-lobby";

    public bool NeedsDatabaseAccount => true;

    private static OperatorContext Context(string reason) => new(Operator, reason, Guid.NewGuid());

    public ValueTask<SecondPasswordStatus> StatusAsync(long accountId, CancellationToken token) =>
        Run(service => service.StatusAsync(accountId, token));

    public ValueTask<SecondPasswordResult> SetAsync(long accountId, uint crc, byte question, string answer, CancellationToken token) =>
        Run(service => service.SetAsync(accountId, crc, question, answer, Context("Player set the secondary password in game"), token));

    public ValueTask<SecondPasswordResult> VerifyAsync(long accountId, uint crc, CancellationToken token) =>
        Run(service => service.VerifyAsync(accountId, crc, Context("Player certified the secondary password in game"), token));

    public ValueTask<SecondPasswordResult> ChangeAsync(long accountId, uint current, uint next, CancellationToken token) =>
        Run(service => service.ChangeAsync(accountId, current, next, Context("Player changed the secondary password in game"), token));

    public ValueTask<SecondPasswordResult> ResetAsync(long accountId, byte question, string? answer, CancellationToken token) =>
        Run(service => service.ResetAsync(accountId, question, answer, Context("Player reset the secondary password with the security answer"), token));

    private async ValueTask<T> Run<T>(Func<SecondPasswordService, Task<T>> call)
    {
        try { return await call(GameDataProviders.Service<SecondPasswordService>(connection)); }
        catch (GameDataException error) when (error.Code is GameDataError.Conflict or GameDataError.Duplicate)
        { throw new ProtocolException(ProtocolError.InvalidPacket, "Secondary password changed concurrently; retry"); }
        catch (Exception error) when (GameDataProviders.Unavailable(error)) { throw GameDataProviders.NotReady(error); }
    }
}

/// <summary>
/// One session's view of the secondary password: answers the four requests and remembers whether the player
/// certified it in this session (0x0BC0 +0x723F, client state 3). Results never carry hashes, CRCs or answers.
/// </summary>
internal sealed class SecondPasswordSession(ISecondPasswordStore store)
{
    public bool Certified { get; private set; }

    public ValueTask<SecondPasswordStatus> StatusAsync(long accountId, CancellationToken token) => store.StatusAsync(accountId, token);

    public async ValueTask<(Packet Reply, SecondPasswordOutcome Outcome, SecondPasswordStatus? Status)> HandleAsync(
        long accountId, Packet request, CancellationToken token)
    {
        ReadOnlySpan<byte> payload = request.Payload.Span;
        SecondPasswordResult result;
        switch (request.Type)
        {
            case SecondPasswordPackets.Set:
            {
                byte question = SecondPasswordPackets.SetQuestion(payload);
                string? answer = SecondPasswordPackets.SetAnswer(payload);
                uint crc = SecondPasswordPackets.SetCrc(payload);
                if (!SecondPasswordPolicy.ValidQuestion(question) || SecondPasswordPolicy.AnswerSecret(answer) is null)
                    return (SecondPasswordPackets.ErrorNotice(request.Type, SecondPasswordPackets.InvalidAnswer), SecondPasswordOutcome.WrongAnswer, null);
                result = await store.SetAsync(accountId, crc, question, answer!, token);
                break;
            }
            case SecondPasswordPackets.Reset:
            {
                byte question = SecondPasswordPackets.ResetQuestion(payload);
                string? answer = SecondPasswordPackets.ResetAnswer(payload);
                result = await store.ResetAsync(accountId, question, answer, token);
                break;
            }
            case SecondPasswordPackets.Change:
            {
                uint current = SecondPasswordPackets.ChangeCurrent(payload), next = SecondPasswordPackets.ChangeNext(payload);
                result = await store.ChangeAsync(accountId, current, next, token);
                break;
            }
            case SecondPasswordPackets.Certify:
                result = await store.VerifyAsync(accountId, SecondPasswordPackets.CertifyCrc(payload), token);
                break;
            default:
                throw new ProtocolException(ProtocolError.InvalidPacket, "Not a secondary password request");
        }
        if (result.Outcome == SecondPasswordOutcome.Ok)
            // Certify -> state 3; set and change -> state 2 (not certified); reset -> state 0.
            Certified = request.Type == SecondPasswordPackets.Certify;
        return (Reply(request.Type, result), result.Outcome, result.Status);
    }

    private static Packet Reply(ushort request, SecondPasswordResult result)
    {
        SecondPasswordStatus status = result.Status;
        return result.Outcome switch
        {
            SecondPasswordOutcome.Ok => SecondPasswordPackets.Ack(request),
            SecondPasswordOutcome.WrongAnswer => SecondPasswordPackets.ErrorNotice(request, SecondPasswordPackets.InvalidAnswer, (uint)status.FailCount),
            SecondPasswordOutcome.LockedNow => SecondPasswordPackets.ErrorNotice(request, SecondPasswordPackets.LockedByFailures,
                SecondPasswordPolicy.MaxFailures, SecondPasswordPackets.UnixSeconds(status.LockedAt)),
            SecondPasswordOutcome.Locked => SecondPasswordPackets.ErrorNotice(request, SecondPasswordPackets.Locked,
                SecondPasswordPolicy.MaxFailures, SecondPasswordPackets.UnixSeconds(status.LockedAt)),
            // Wrong password, and the two out-of-sync cases (set when one exists, use when none exists).
            _ => SecondPasswordPackets.ErrorNotice(request, SecondPasswordPackets.InvalidPassword, (uint)status.FailCount)
        };
    }
}
