namespace C9.Protocol;

// One cipher epoch of a direction: the block cipher chosen from a seed and the number of
// frames it protects before the next rekey (ConcreteCipher<..., N> in the client).
public sealed record CipherEpoch(string Name, int Threshold, ILegacyBlockCipher Cipher, int HashCase, int CipherIndex)
{
    // 00D04730: both directions start from this 32-byte seed.
    public static ReadOnlySpan<byte> InitialSeed => "33234567890123456789012345678912"u8;

    // 00D0A880: selector byte -> hash (selector % 11) -> digest byte -> cipher (00D0A480).
    public static CipherEpoch Derive(ReadOnlySpan<byte> seed)
    {
        if (seed.Length == 0) throw new ArgumentException("Cipher seed is empty", nameof(seed));
        byte selector = seed.Length < 10
            ? seed[0]
            : (byte)(((byte)((seed[1] ^ seed[0]) << (seed[2] % 3)) | seed[3]) ^ seed[4]);
        int hashCase = selector % 11;
        int digestIndex = hashCase switch { 1 => 0, 3 => 1, 4 => 2, 5 => 3, 6 => 4, 7 => 5, 8 => 6, 9 => 7, 10 => 8, _ => 9 };
        // The client hashes into a zeroed 0x800-byte buffer, so keys longer than the digest end in zeros.
        Span<byte> key = stackalloc byte[64];
        key.Clear();
        LegacyHashes.ForEpochCase(hashCase, seed).CopyTo(key);
        int index = key[digestIndex] % 10;
        CipherEpoch epoch = Create(index, key);
        return epoch with { HashCase = hashCase };
    }

    // 00D0A480 and each ConcreteCipher init (key bytes used, frame threshold).
    public static CipherEpoch Create(int index, ReadOnlySpan<byte> key) => (index % 10) switch
    {
        0 => new("TEA", 256, new TeaCipher(key[..16]), -1, 0),
        1 => new("ThreeWay", 256, new ThreeWayCipher(key[..12]), -1, 1),
        2 => new("CAST128", 256, new Cast128Cipher(key[..16]), -1, 2),
        4 => new("RC5", 256, new Rc5Cipher(key[..16]), -1, 4),
        5 => new("RC6", 256, new Rc6Cipher(key[..16]), -1, 5),
        6 => new("MARS", 256, new MarsCipher(key[..16]), -1, 6),
        7 => new("IDEA", 256, new IdeaCipher(key[..16]), -1, 7),
        9 => (key[0] & 7) switch
        {
            0 => new("Twofish", 8, new TwofishCipher(key[..16]), -1, 9),
            1 => new("DES", 8, new DesCipher(key[..8]), -1, 9),
            2 => new("DES_EDE2", 8, new DesEdeCipher(key[..16]), -1, 9),
            3 => new("DES_EDE3", 32, new DesEdeCipher(key[..24]), -1, 9),
            4 => new("DES_XEX3", 8, new DesXex3Cipher(key[..24]), -1, 9),
            5 => new("SKIPJACK", 32, new SkipjackCipher(key[..10]), -1, 9),
            6 => new("SAFER_K", 8, new SaferCipher(key[..16], strengthened: false), -1, 9),
            _ => new("SAFER_SK", 32, new SaferCipher(key[..16], strengthened: true), -1, 9),
        },
        _ => new("GOST", 256, new GostCipher(key[..32]), -1, index % 10), // 3 and 8
    };
}
