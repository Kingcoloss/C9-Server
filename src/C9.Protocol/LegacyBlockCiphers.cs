using System.Buffers.Binary;
using System.Numerics;
using static C9.Protocol.LegacyCipherTables;

namespace C9.Protocol;

// One raw (ECB) block transform. Implementations follow the Crypto++ classes the client
// instantiates in 00D0A480; each is checked against Crypto++ 8.9 vectors and its own round trip.
public interface ILegacyBlockCipher
{
    int BlockSize { get; }
    void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output);
    void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output);
}

internal static class Be
{
    public static uint U32(ReadOnlySpan<byte> b, int o) => BinaryPrimitives.ReadUInt32BigEndian(b[o..]);
    public static void Put(Span<byte> b, int o, uint v) => BinaryPrimitives.WriteUInt32BigEndian(b[o..], v);
}

internal static class Le
{
    public static uint U32(ReadOnlySpan<byte> b, int o) => BinaryPrimitives.ReadUInt32LittleEndian(b[o..]);
    public static void Put(Span<byte> b, int o, uint v) => BinaryPrimitives.WriteUInt32LittleEndian(b[o..], v);
}

// Crypto++ TEA: 32 rounds, big-endian words.
public sealed class TeaCipher : ILegacyBlockCipher
{
    private const uint Delta = 0x9E3779B9;
    private readonly uint[] k = new uint[4];
    public int BlockSize => 8;

    public TeaCipher(ReadOnlySpan<byte> key)
    {
        for (int i = 0; i < 4; i++) k[i] = Be.U32(key, 4 * i);
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint y = Be.U32(input, 0), z = Be.U32(input, 4), sum = 0;
        unchecked
        {
            for (int i = 0; i < 32; i++)
            {
                sum += Delta;
                y += ((z << 4) + k[0]) ^ (z + sum) ^ ((z >> 5) + k[1]);
                z += ((y << 4) + k[2]) ^ (y + sum) ^ ((y >> 5) + k[3]);
            }
        }
        Be.Put(output, 0, y);
        Be.Put(output, 4, z);
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint y = Be.U32(input, 0), z = Be.U32(input, 4);
        unchecked
        {
            uint sum = Delta * 32;
            for (int i = 0; i < 32; i++)
            {
                z -= ((y << 4) + k[2]) ^ (y + sum) ^ ((y >> 5) + k[3]);
                y -= ((z << 4) + k[0]) ^ (z + sum) ^ ((z >> 5) + k[1]);
                sum -= Delta;
            }
        }
        Be.Put(output, 0, y);
        Be.Put(output, 4, z);
    }
}

// Crypto++ ThreeWay (3-Way): 11 rounds, 12-byte blocks.
public sealed class ThreeWayCipher : ILegacyBlockCipher
{
    private const int Rounds = 11;
    private readonly uint[] encryptKey = new uint[3];
    private readonly uint[] decryptKey = new uint[3];
    public int BlockSize => 12;

    public ThreeWayCipher(ReadOnlySpan<byte> key)
    {
        for (int i = 0; i < 3; i++) encryptKey[i] = decryptKey[i] = Be.U32(key, 4 * i);
        Theta(ref decryptKey[0], ref decryptKey[1], ref decryptKey[2]);
        Mu(ref decryptKey[0], ref decryptKey[1], ref decryptKey[2]);
        for (int i = 0; i < 3; i++) decryptKey[i] = BinaryPrimitives.ReverseEndianness(decryptKey[i]);
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a0 = Be.U32(input, 0), a1 = Be.U32(input, 4), a2 = Be.U32(input, 8);
        Run(encryptKey, 0x0B0B, ref a0, ref a1, ref a2);
        Be.Put(output, 0, a0);
        Be.Put(output, 4, a1);
        Be.Put(output, 8, a2);
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a0 = Le.U32(input, 0), a1 = Le.U32(input, 4), a2 = Le.U32(input, 8);
        Mu(ref a0, ref a1, ref a2);
        Run(decryptKey, 0xB1B1, ref a0, ref a1, ref a2);
        Mu(ref a0, ref a1, ref a2);
        Le.Put(output, 0, a0);
        Le.Put(output, 4, a1);
        Le.Put(output, 8, a2);
    }

    private static void Run(uint[] k, uint rc, ref uint a0, ref uint a1, ref uint a2)
    {
        for (int i = 0; i < Rounds; i++)
        {
            a0 ^= k[0] ^ (rc << 16);
            a1 ^= k[1];
            a2 ^= k[2] ^ rc;
            Theta(ref a0, ref a1, ref a2);
            PiGammaPi(ref a0, ref a1, ref a2);
            rc <<= 1;
            if ((rc & 0x10000) != 0) rc ^= 0x11011;
        }
        a0 ^= k[0] ^ (rc << 16);
        a1 ^= k[1];
        a2 ^= k[2] ^ rc;
        Theta(ref a0, ref a1, ref a2);
    }

    private static uint ReverseBits(uint a)
    {
        a = ((a & 0xAAAAAAAA) >> 1) | ((a & 0x55555555) << 1);
        a = ((a & 0xCCCCCCCC) >> 2) | ((a & 0x33333333) << 2);
        return ((a & 0xF0F0F0F0) >> 4) | ((a & 0x0F0F0F0F) << 4);
    }

    private static void Mu(ref uint a0, ref uint a1, ref uint a2)
    {
        a1 = ReverseBits(a1);
        uint t = ReverseBits(a0);
        a0 = ReverseBits(a2);
        a2 = t;
    }

    private static void PiGammaPi(ref uint a0, ref uint a1, ref uint a2)
    {
        uint b2 = BitOperations.RotateLeft(a2, 1);
        uint b0 = BitOperations.RotateLeft(a0, 22);
        a0 = BitOperations.RotateLeft(b0 ^ (a1 | ~b2), 1);
        a2 = BitOperations.RotateLeft(b2 ^ (b0 | ~a1), 22);
        a1 ^= b2 | ~b0;
    }

    private static void Theta(ref uint a0, ref uint a1, ref uint a2)
    {
        uint c = a0 ^ a1 ^ a2;
        c = BitOperations.RotateLeft(c, 16) ^ BitOperations.RotateLeft(c, 8);
        uint b0 = (a0 << 24) ^ (a2 >> 8) ^ (a1 << 8) ^ (a0 >> 24);
        uint b1 = (a1 << 24) ^ (a0 >> 8) ^ (a2 << 8) ^ (a1 >> 24);
        a0 ^= c ^ b0;
        a1 ^= c ^ b1;
        a2 ^= c ^ (b0 >> 16) ^ (b1 << 16);
    }
}

// CAST-128 (RFC 2144), 16-byte key -> 16 rounds.
public sealed class Cast128Cipher : ILegacyBlockCipher
{
    private readonly uint[] km = new uint[16];
    private readonly int[] kr = new int[16];
    public int BlockSize => 8;

    private static uint S(int box, uint index) => CastS[box * 256 + (int)(index & 0xFF)];

    public Cast128Cipher(ReadOnlySpan<byte> key)
    {
        uint[] x = new uint[4], z = new uint[4], k = new uint[32];
        for (int i = 0; i < 4; i++) x[i] = Be.U32(key, 4 * i);
        static uint B(uint[] w, int n) => (w[n >> 2] >> (24 - 8 * (n & 3))) & 0xFF;
        for (int half = 0; half < 2; half++)
        {
            int o = half * 16;
            z[0] = x[0] ^ S(4, B(x, 13)) ^ S(5, B(x, 15)) ^ S(6, B(x, 12)) ^ S(7, B(x, 14)) ^ S(6, B(x, 8));
            z[1] = x[2] ^ S(4, B(z, 0)) ^ S(5, B(z, 2)) ^ S(6, B(z, 1)) ^ S(7, B(z, 3)) ^ S(7, B(x, 10));
            z[2] = x[3] ^ S(4, B(z, 7)) ^ S(5, B(z, 6)) ^ S(6, B(z, 5)) ^ S(7, B(z, 4)) ^ S(4, B(x, 9));
            z[3] = x[1] ^ S(4, B(z, 10)) ^ S(5, B(z, 9)) ^ S(6, B(z, 11)) ^ S(7, B(z, 8)) ^ S(5, B(x, 11));
            k[o + 0] = S(4, B(z, 8)) ^ S(5, B(z, 9)) ^ S(6, B(z, 7)) ^ S(7, B(z, 6)) ^ S(4, B(z, 2));
            k[o + 1] = S(4, B(z, 10)) ^ S(5, B(z, 11)) ^ S(6, B(z, 5)) ^ S(7, B(z, 4)) ^ S(5, B(z, 6));
            k[o + 2] = S(4, B(z, 12)) ^ S(5, B(z, 13)) ^ S(6, B(z, 3)) ^ S(7, B(z, 2)) ^ S(6, B(z, 9));
            k[o + 3] = S(4, B(z, 14)) ^ S(5, B(z, 15)) ^ S(6, B(z, 1)) ^ S(7, B(z, 0)) ^ S(7, B(z, 12));
            x[0] = z[2] ^ S(4, B(z, 5)) ^ S(5, B(z, 7)) ^ S(6, B(z, 4)) ^ S(7, B(z, 6)) ^ S(6, B(z, 0));
            x[1] = z[0] ^ S(4, B(x, 0)) ^ S(5, B(x, 2)) ^ S(6, B(x, 1)) ^ S(7, B(x, 3)) ^ S(7, B(z, 2));
            x[2] = z[1] ^ S(4, B(x, 7)) ^ S(5, B(x, 6)) ^ S(6, B(x, 5)) ^ S(7, B(x, 4)) ^ S(4, B(z, 1));
            x[3] = z[3] ^ S(4, B(x, 10)) ^ S(5, B(x, 9)) ^ S(6, B(x, 11)) ^ S(7, B(x, 8)) ^ S(5, B(z, 3));
            k[o + 4] = S(4, B(x, 3)) ^ S(5, B(x, 2)) ^ S(6, B(x, 12)) ^ S(7, B(x, 13)) ^ S(4, B(x, 8));
            k[o + 5] = S(4, B(x, 1)) ^ S(5, B(x, 0)) ^ S(6, B(x, 14)) ^ S(7, B(x, 15)) ^ S(5, B(x, 13));
            k[o + 6] = S(4, B(x, 7)) ^ S(5, B(x, 6)) ^ S(6, B(x, 8)) ^ S(7, B(x, 9)) ^ S(6, B(x, 3));
            k[o + 7] = S(4, B(x, 5)) ^ S(5, B(x, 4)) ^ S(6, B(x, 10)) ^ S(7, B(x, 11)) ^ S(7, B(x, 7));
            z[0] = x[0] ^ S(4, B(x, 13)) ^ S(5, B(x, 15)) ^ S(6, B(x, 12)) ^ S(7, B(x, 14)) ^ S(6, B(x, 8));
            z[1] = x[2] ^ S(4, B(z, 0)) ^ S(5, B(z, 2)) ^ S(6, B(z, 1)) ^ S(7, B(z, 3)) ^ S(7, B(x, 10));
            z[2] = x[3] ^ S(4, B(z, 7)) ^ S(5, B(z, 6)) ^ S(6, B(z, 5)) ^ S(7, B(z, 4)) ^ S(4, B(x, 9));
            z[3] = x[1] ^ S(4, B(z, 10)) ^ S(5, B(z, 9)) ^ S(6, B(z, 11)) ^ S(7, B(z, 8)) ^ S(5, B(x, 11));
            k[o + 8] = S(4, B(z, 3)) ^ S(5, B(z, 2)) ^ S(6, B(z, 12)) ^ S(7, B(z, 13)) ^ S(4, B(z, 9));
            k[o + 9] = S(4, B(z, 1)) ^ S(5, B(z, 0)) ^ S(6, B(z, 14)) ^ S(7, B(z, 15)) ^ S(5, B(z, 12));
            k[o + 10] = S(4, B(z, 7)) ^ S(5, B(z, 6)) ^ S(6, B(z, 8)) ^ S(7, B(z, 9)) ^ S(6, B(z, 2));
            k[o + 11] = S(4, B(z, 5)) ^ S(5, B(z, 4)) ^ S(6, B(z, 10)) ^ S(7, B(z, 11)) ^ S(7, B(z, 6));
            x[0] = z[2] ^ S(4, B(z, 5)) ^ S(5, B(z, 7)) ^ S(6, B(z, 4)) ^ S(7, B(z, 6)) ^ S(6, B(z, 0));
            x[1] = z[0] ^ S(4, B(x, 0)) ^ S(5, B(x, 2)) ^ S(6, B(x, 1)) ^ S(7, B(x, 3)) ^ S(7, B(z, 2));
            x[2] = z[1] ^ S(4, B(x, 7)) ^ S(5, B(x, 6)) ^ S(6, B(x, 5)) ^ S(7, B(x, 4)) ^ S(4, B(z, 1));
            x[3] = z[3] ^ S(4, B(x, 10)) ^ S(5, B(x, 9)) ^ S(6, B(x, 11)) ^ S(7, B(x, 8)) ^ S(5, B(z, 3));
            k[o + 12] = S(4, B(x, 8)) ^ S(5, B(x, 9)) ^ S(6, B(x, 7)) ^ S(7, B(x, 6)) ^ S(4, B(x, 3));
            k[o + 13] = S(4, B(x, 10)) ^ S(5, B(x, 11)) ^ S(6, B(x, 5)) ^ S(7, B(x, 4)) ^ S(5, B(x, 7));
            k[o + 14] = S(4, B(x, 12)) ^ S(5, B(x, 13)) ^ S(6, B(x, 3)) ^ S(7, B(x, 2)) ^ S(6, B(x, 8));
            k[o + 15] = S(4, B(x, 14)) ^ S(5, B(x, 15)) ^ S(6, B(x, 1)) ^ S(7, B(x, 0)) ^ S(7, B(x, 13));
        }
        for (int i = 0; i < 16; i++)
        {
            km[i] = k[i];
            kr[i] = (int)(k[16 + i] & 31);
        }
    }

    private uint F(int round, uint d)
    {
        unchecked
        {
            uint i;
            switch (round % 3)
            {
                case 0:
                    i = BitOperations.RotateLeft(km[round] + d, kr[round]);
                    return ((S(0, i >> 24) ^ S(1, i >> 16)) - S(2, i >> 8)) + S(3, i);
                case 1:
                    i = BitOperations.RotateLeft(km[round] ^ d, kr[round]);
                    return ((S(0, i >> 24) - S(1, i >> 16)) + S(2, i >> 8)) ^ S(3, i);
                default:
                    i = BitOperations.RotateLeft(km[round] - d, kr[round]);
                    return ((S(0, i >> 24) + S(1, i >> 16)) ^ S(2, i >> 8)) - S(3, i);
            }
        }
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint l = Be.U32(input, 0), r = Be.U32(input, 4);
        for (int round = 0; round < 16; round++) (l, r) = (r, l ^ F(round, r));
        Be.Put(output, 0, r);
        Be.Put(output, 4, l);
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint l = Be.U32(input, 0), r = Be.U32(input, 4);
        for (int round = 15; round >= 0; round--) (l, r) = (r, l ^ F(round, r));
        Be.Put(output, 0, r);
        Be.Put(output, 4, l);
    }
}

// Crypto++ GOST 28147-89 with its fixed S-box set, little-endian words.
public sealed class GostCipher : ILegacyBlockCipher
{
    private static readonly uint[] STable = BuildSTable();
    private readonly uint[] k = new uint[8];
    public int BlockSize => 8;

    public GostCipher(ReadOnlySpan<byte> key)
    {
        for (int i = 0; i < 8; i++) k[i] = Le.U32(key, 4 * i);
    }

    private static uint[] BuildSTable()
    {
        uint[] table = new uint[4 * 256];
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 256; j++)
            {
                uint t = (uint)(GostSbox[2 * i * 16 + j % 16] | (GostSbox[(2 * i + 1) * 16 + j / 16] << 4));
                table[i * 256 + j] = BitOperations.RotateLeft(t, 11 + 8 * i);
            }
        return table;
    }

    private static uint F(uint t) =>
        STable[3 * 256 + (t >> 24)] ^ STable[2 * 256 + ((t >> 16) & 0xFF)] ^ STable[256 + ((t >> 8) & 0xFF)] ^ STable[t & 0xFF];

    private static ReadOnlySpan<int> EncryptOrder => [0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 0];

    private void Run(ReadOnlySpan<byte> input, Span<byte> output, bool decrypt)
    {
        uint n1 = Le.U32(input, 0), n2 = Le.U32(input, 4);
        ReadOnlySpan<int> order = EncryptOrder;
        unchecked
        {
            for (int i = 0; i < 32; i += 2)
            {
                int i1 = decrypt ? order[31 - i] : order[i];
                int i2 = decrypt ? order[30 - i] : order[i + 1];
                n2 ^= F(n1 + k[i1]);
                n1 ^= F(n2 + k[i2]);
            }
        }
        Le.Put(output, 0, n2);
        Le.Put(output, 4, n1);
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Run(input, output, decrypt: false);
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Run(input, output, decrypt: true);
}

// RC5-32/16/16.
public sealed class Rc5Cipher : ILegacyBlockCipher
{
    private const int Rounds = 16;
    private readonly uint[] s = new uint[2 * (Rounds + 1)];
    public int BlockSize => 8;

    public Rc5Cipher(ReadOnlySpan<byte> key)
    {
        int c = Math.Max((key.Length + 3) / 4, 1);
        uint[] l = new uint[c];
        for (int i = 0; i < key.Length; i++) l[i / 4] |= (uint)key[i] << (8 * (i % 4));
        unchecked
        {
            s[0] = 0xB7E15163;
            for (int j = 1; j < s.Length; j++) s[j] = s[j - 1] + 0x9E3779B9;
            uint a = 0, b = 0;
            for (int h = 0; h < 3 * Math.Max(s.Length, c); h++)
            {
                a = s[h % s.Length] = BitOperations.RotateLeft(s[h % s.Length] + a + b, 3);
                b = l[h % c] = BitOperations.RotateLeft(l[h % c] + a + b, (int)(a + b));
            }
        }
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a = Le.U32(input, 0), b = Le.U32(input, 4);
        unchecked
        {
            a += s[0];
            b += s[1];
            for (int i = 1; i <= Rounds; i++)
            {
                a = BitOperations.RotateLeft(a ^ b, (int)b) + s[2 * i];
                b = BitOperations.RotateLeft(a ^ b, (int)a) + s[2 * i + 1];
            }
        }
        Le.Put(output, 0, a);
        Le.Put(output, 4, b);
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a = Le.U32(input, 0), b = Le.U32(input, 4);
        unchecked
        {
            for (int i = Rounds; i >= 1; i--)
            {
                b = BitOperations.RotateRight(b - s[2 * i + 1], (int)a) ^ a;
                a = BitOperations.RotateRight(a - s[2 * i], (int)b) ^ b;
            }
            b -= s[1];
            a -= s[0];
        }
        Le.Put(output, 0, a);
        Le.Put(output, 4, b);
    }
}

// MARS (IBM AES candidate, tweaked key schedule), little-endian words.
public sealed class MarsCipher : ILegacyBlockCipher
{
    private readonly uint[] k = new uint[40];
    public int BlockSize => 16;

    private static uint S(uint a) => MarsSbox[a & 0x1FF];
    private static uint S0(uint a) => MarsSbox[a & 0xFF];
    private static uint S1(uint a) => MarsSbox[(a & 0xFF) + 256];

    public MarsCipher(ReadOnlySpan<byte> key)
    {
        uint[] t = new uint[15];
        for (int i = 0; i < key.Length / 4; i++) t[i] = Le.U32(key, 4 * i);
        t[key.Length / 4] = (uint)(key.Length / 4);
        unchecked
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 15; i++)
                    t[i] ^= BitOperations.RotateLeft(t[(i + 8) % 15] ^ t[(i + 13) % 15], 3) ^ (uint)(4 * i + j);
                for (int round = 0; round < 4; round++)
                    for (int i = 0; i < 15; i++)
                        t[i] = BitOperations.RotateLeft(t[i] + MarsSbox[t[(i + 14) % 15] % 512], 9);
                for (int i = 0; i < 10; i++) k[10 * j + i] = t[4 * i % 15];
            }
            for (int i = 5; i < 37; i += 2)
            {
                uint w = k[i] | 3;
                uint m = (~w ^ (w << 1)) & (~w ^ (w >> 1)) & 0x7FFFFFFE;
                m &= m >> 1; m &= m >> 2; m &= m >> 4;
                m |= m << 1; m |= m << 2; m |= m << 4;
                m &= 0x7FFFFFFC;
                w ^= BitOperations.RotateLeft(MarsSbox[265 + (k[i] & 3)], (int)k[i - 1]) & m;
                k[i] = w;
            }
        }
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a = Le.U32(input, 0), b = Le.U32(input, 4), c = Le.U32(input, 8), d = Le.U32(input, 12), t;
        unchecked
        {
            a += k[0]; b += k[1]; c += k[2]; d += k[3];
            for (int i = 0; i < 8; i++)
            {
                b = (b ^ S0(a)) + S1(a >> 8);
                c += S0(a >> 16);
                a = BitOperations.RotateRight(a, 24);
                d ^= S1(a);
                if (i % 4 == 0) a += d;
                if (i % 4 == 1) a += b;
                (a, b, c, d) = (b, c, d, a);
            }
            for (int i = 0; i < 16; i++)
            {
                t = BitOperations.RotateLeft(a, 13);
                uint r = BitOperations.RotateLeft(t * k[2 * i + 5], 10);
                uint m = a + k[2 * i + 4];
                uint l = BitOperations.RotateLeft(S(m) ^ BitOperations.RotateRight(r, 5) ^ r, (int)r);
                c += BitOperations.RotateLeft(m, (int)BitOperations.RotateRight(r, 5));
                if (i < 8) { b += l; d ^= r; } else { d += l; b ^= r; }
                (a, b, c, d) = (b, c, d, t);
            }
            for (int i = 0; i < 8; i++)
            {
                if (i % 4 == 2) a -= d;
                if (i % 4 == 3) a -= b;
                b ^= S1(a);
                c -= S0(a >> 24);
                t = BitOperations.RotateLeft(a, 24);
                d = (d - S1(a >> 16)) ^ S0(t);
                (a, b, c, d) = (b, c, d, t);
            }
            a -= k[36]; b -= k[37]; c -= k[38]; d -= k[39];
        }
        Le.Put(output, 0, a);
        Le.Put(output, 4, b);
        Le.Put(output, 8, c);
        Le.Put(output, 12, d);
    }

    // Exact step-by-step inverse of EncryptBlock.
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a = Le.U32(input, 0), b = Le.U32(input, 4), c = Le.U32(input, 8), d = Le.U32(input, 12);
        unchecked
        {
            a += k[36]; b += k[37]; c += k[38]; d += k[39];
            for (int i = 7; i >= 0; i--)
            {
                uint t = d;
                (b, c, d) = (a, b, c);
                a = BitOperations.RotateRight(t, 24);
                d = (d ^ S0(t)) + S1(a >> 16);
                c += S0(a >> 24);
                b ^= S1(a);
                if (i % 4 == 3) a += b;
                if (i % 4 == 2) a += d;
            }
            for (int i = 15; i >= 0; i--)
            {
                uint t = d;
                (b, c, d) = (a, b, c);
                a = BitOperations.RotateRight(t, 13);
                uint r = BitOperations.RotateLeft(t * k[2 * i + 5], 10);
                uint m = a + k[2 * i + 4];
                uint l = BitOperations.RotateLeft(S(m) ^ BitOperations.RotateRight(r, 5) ^ r, (int)r);
                c -= BitOperations.RotateLeft(m, (int)BitOperations.RotateRight(r, 5));
                if (i < 8) { b -= l; d ^= r; } else { d -= l; b ^= r; }
            }
            for (int i = 7; i >= 0; i--)
            {
                uint aRot = d;
                (b, c, d) = (a, b, c);
                if (i % 4 == 0) aRot -= d;
                if (i % 4 == 1) aRot -= b;
                d ^= S1(aRot);
                a = BitOperations.RotateLeft(aRot, 24);
                c -= S0(a >> 16);
                b = (b - S1(a >> 8)) ^ S0(a);
            }
            a -= k[0]; b -= k[1]; c -= k[2]; d -= k[3];
        }
        Le.Put(output, 0, a);
        Le.Put(output, 4, b);
        Le.Put(output, 8, c);
        Le.Put(output, 12, d);
    }
}

// IDEA, 8.5 rounds, big-endian 16-bit words.
public sealed class IdeaCipher : ILegacyBlockCipher
{
    private readonly ushort[] ek = new ushort[52];
    private readonly ushort[] dk = new ushort[52];
    public int BlockSize => 8;

    public IdeaCipher(ReadOnlySpan<byte> key)
    {
        UInt128 value = BinaryPrimitives.ReadUInt128BigEndian(key);
        for (int i = 0; i < 52; i++)
        {
            if (i > 0 && i % 8 == 0) value = (value << 25) | (value >> 103);
            ek[i] = (ushort)(value >> (112 - 16 * (i % 8)));
        }
        dk[0] = Inverse(ek[48]); dk[1] = Negate(ek[49]); dk[2] = Negate(ek[50]); dk[3] = Inverse(ek[51]);
        for (int r = 1; r < 8; r++)
        {
            int e = 48 - 6 * r;
            dk[6 * r - 2] = ek[e + 4]; dk[6 * r - 1] = ek[e + 5];
            dk[6 * r] = Inverse(ek[e]); dk[6 * r + 1] = Negate(ek[e + 2]); dk[6 * r + 2] = Negate(ek[e + 1]); dk[6 * r + 3] = Inverse(ek[e + 3]);
        }
        dk[46] = ek[4]; dk[47] = ek[5];
        dk[48] = Inverse(ek[0]); dk[49] = Negate(ek[1]); dk[50] = Negate(ek[2]); dk[51] = Inverse(ek[3]);
    }

    private static ushort Negate(ushort x) => (ushort)(0x10000 - x);

    private static ushort Mul(ushort a, ushort b)
    {
        long x = a == 0 ? 0x10000 : a, y = b == 0 ? 0x10000 : b;
        long p = x * y % 0x10001;
        return (ushort)(p == 0x10000 ? 0 : p);
    }

    private static ushort Inverse(ushort x)
    {
        if (x <= 1) return x;
        long result = 1, b = x, e = 0xFFFF;
        while (e > 0)
        {
            if ((e & 1) != 0) result = result * b % 0x10001;
            b = b * b % 0x10001;
            e >>= 1;
        }
        return (ushort)result;
    }

    private static void Run(ushort[] k, ReadOnlySpan<byte> input, Span<byte> output)
    {
        ushort x1 = BinaryPrimitives.ReadUInt16BigEndian(input), x2 = BinaryPrimitives.ReadUInt16BigEndian(input[2..]);
        ushort x3 = BinaryPrimitives.ReadUInt16BigEndian(input[4..]), x4 = BinaryPrimitives.ReadUInt16BigEndian(input[6..]);
        unchecked
        {
            for (int r = 0; r < 8; r++)
            {
                int o = 6 * r;
                ushort a = Mul(x1, k[o]), b = (ushort)(x2 + k[o + 1]), c = (ushort)(x3 + k[o + 2]), d = Mul(x4, k[o + 3]);
                ushort e = Mul((ushort)(a ^ c), k[o + 4]);
                ushort f = Mul((ushort)((b ^ d) + e), k[o + 5]);
                e = (ushort)(e + f);
                x1 = (ushort)(a ^ f); x2 = (ushort)(c ^ f); x3 = (ushort)(b ^ e); x4 = (ushort)(d ^ e);
            }
            BinaryPrimitives.WriteUInt16BigEndian(output, Mul(x1, k[48]));
            BinaryPrimitives.WriteUInt16BigEndian(output[2..], (ushort)(x3 + k[49]));
            BinaryPrimitives.WriteUInt16BigEndian(output[4..], (ushort)(x2 + k[50]));
            BinaryPrimitives.WriteUInt16BigEndian(output[6..], Mul(x4, k[51]));
        }
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Run(ek, input, output);
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Run(dk, input, output);
}

// Twofish with Crypto++'s combined q/MDS tables, 16-byte key.
public sealed class TwofishCipher : ILegacyBlockCipher
{
    private readonly uint[] k = new uint[40];
    private readonly uint[] s = new uint[4 * 256];
    public int BlockSize => 16;

    private static uint Q(int a, int b, int c, int d, uint t) =>
        TwofishQ[a * 256 + (int)(t & 0xFF)] ^ ((uint)TwofishQ[b * 256 + (int)((t >> 8) & 0xFF)] << 8) ^
        ((uint)TwofishQ[c * 256 + (int)((t >> 16) & 0xFF)] << 16) ^ ((uint)TwofishQ[d * 256 + (int)(t >> 24)] << 24);

    private static uint H0(uint x, uint[] key, int offset)
    {
        x |= (x << 8) | (x << 16) | (x << 24);
        x = Q(0, 1, 0, 1, x) ^ key[offset + 2];
        return Q(0, 0, 1, 1, x) ^ key[offset];
    }

    private static uint H(uint x, uint[] key, int offset)
    {
        x = H0(x, key, offset);
        return TwofishMds[x & 0xFF] ^ TwofishMds[256 + ((x >> 8) & 0xFF)] ^ TwofishMds[512 + ((x >> 16) & 0xFF)] ^ TwofishMds[768 + (x >> 24)];
    }

    private static uint Mod(uint c)
    {
        const uint modulus = 0x14D;
        uint c2 = (c << 1) ^ ((c & 0x80) != 0 ? modulus : 0);
        uint c1 = c2 ^ (c >> 1) ^ ((c & 1) != 0 ? modulus >> 1 : 0);
        return c | (c1 << 8) | (c2 << 16) | (c1 << 24);
    }

    private static uint ReedSolomon(uint high, uint low)
    {
        for (int i = 0; i < 8; i++)
        {
            high = Mod(high >> 24) ^ (high << 8) ^ (low >> 24);
            low <<= 8;
        }
        return high;
    }

    public TwofishCipher(ReadOnlySpan<byte> userKey)
    {
        uint[] key = new uint[4];
        for (int i = 0; i < 4; i++) key[i] = Le.U32(userKey, 4 * i);
        unchecked
        {
            for (uint i = 0; i < 40; i += 2)
            {
                uint a = H(i, key, 0);
                uint b = BitOperations.RotateLeft(H(i + 1, key, 1), 8);
                k[i] = a + b;
                k[i + 1] = BitOperations.RotateLeft(a + 2 * b, 9);
            }
        }
        uint[] svec = new uint[4];
        for (int i = 0; i < 2; i++) svec[2 * (2 - i - 1)] = ReedSolomon(key[2 * i + 1], key[2 * i]);
        for (uint i = 0; i < 256; i++)
        {
            uint t = H0(i, svec, 0);
            s[i] = TwofishMds[t & 0xFF];
            s[256 + i] = TwofishMds[256 + ((t >> 8) & 0xFF)];
            s[512 + i] = TwofishMds[512 + ((t >> 16) & 0xFF)];
            s[768 + i] = TwofishMds[768 + (t >> 24)];
        }
    }

    private uint G1(uint x) => s[x & 0xFF] ^ s[256 + ((x >> 8) & 0xFF)] ^ s[512 + ((x >> 16) & 0xFF)] ^ s[768 + (x >> 24)];
    private uint G2(uint x) => s[x >> 24] ^ s[256 + (x & 0xFF)] ^ s[512 + ((x >> 8) & 0xFF)] ^ s[768 + ((x >> 16) & 0xFF)];

    private void EncRound(int n, uint a, uint b, ref uint c, ref uint d)
    {
        unchecked
        {
            uint x = G1(a), y = G2(b);
            x += y;
            y += x + k[8 + 2 * n + 1];
            c = BitOperations.RotateRight(c ^ (x + k[8 + 2 * n]), 1);
            d = BitOperations.RotateLeft(d, 1) ^ y;
        }
    }

    private void DecRound(int n, uint a, uint b, ref uint c, ref uint d)
    {
        unchecked
        {
            uint x = G1(a), y = G2(b);
            x += y;
            y += x;
            d = BitOperations.RotateRight(d ^ (y + k[8 + 2 * n + 1]), 1);
            c = BitOperations.RotateLeft(c, 1) ^ (x + k[8 + 2 * n]);
        }
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint a = Le.U32(input, 0) ^ k[0], b = Le.U32(input, 4) ^ k[1], c = Le.U32(input, 8) ^ k[2], d = Le.U32(input, 12) ^ k[3];
        for (int n = 0; n < 8; n++)
        {
            EncRound(2 * n, a, b, ref c, ref d);
            EncRound(2 * n + 1, c, d, ref a, ref b);
        }
        Le.Put(output, 0, c ^ k[4]);
        Le.Put(output, 4, d ^ k[5]);
        Le.Put(output, 8, a ^ k[6]);
        Le.Put(output, 12, b ^ k[7]);
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint c = Le.U32(input, 0) ^ k[4], d = Le.U32(input, 4) ^ k[5], a = Le.U32(input, 8) ^ k[6], b = Le.U32(input, 12) ^ k[7];
        for (int n = 7; n >= 0; n--)
        {
            DecRound(2 * n + 1, c, d, ref a, ref b);
            DecRound(2 * n, a, b, ref c, ref d);
        }
        Le.Put(output, 0, a ^ k[0]);
        Le.Put(output, 4, b ^ k[1]);
        Le.Put(output, 8, c ^ k[2]);
        Le.Put(output, 12, d ^ k[3]);
    }
}

// DES core in Crypto++'s RawDES form (SP-box rounds between IP/FP), plus EDE2/EDE3/XEX3.
public sealed class DesCipher : ILegacyBlockCipher
{
    private static ReadOnlySpan<byte> Pc1 => [
        57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
        63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4];
    private static ReadOnlySpan<byte> TotalRotations => [1, 2, 4, 6, 8, 10, 12, 14, 15, 17, 19, 21, 23, 25, 27, 28];
    private static ReadOnlySpan<byte> Pc2 => [
        14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2,
        41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32];
    private static ReadOnlySpan<byte> ByteBit => [0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01];

    private readonly uint[] k = new uint[32];
    private readonly uint[] kd = new uint[32];
    public int BlockSize => 8;

    public DesCipher(ReadOnlySpan<byte> key)
    {
        Span<byte> pc1m = stackalloc byte[56], pcr = stackalloc byte[56], ks = stackalloc byte[8];
        for (int j = 0; j < 56; j++)
        {
            int l = Pc1[j] - 1;
            pc1m[j] = (byte)((key[l >> 3] & ByteBit[l & 7]) != 0 ? 1 : 0);
        }
        for (int i = 0; i < 16; i++)
        {
            ks.Clear();
            for (int j = 0; j < 56; j++)
            {
                int l = j + TotalRotations[i];
                pcr[j] = pc1m[l < (j < 28 ? 28 : 56) ? l : l - 28];
            }
            for (int j = 0; j < 48; j++)
                if (pcr[Pc2[j] - 1] != 0) ks[j / 6] |= (byte)(ByteBit[j % 6] >> 2);
            k[2 * i] = ((uint)ks[0] << 24) | ((uint)ks[2] << 16) | ((uint)ks[4] << 8) | ks[6];
            k[2 * i + 1] = ((uint)ks[1] << 24) | ((uint)ks[3] << 16) | ((uint)ks[5] << 8) | ks[7];
        }
        for (int i = 0; i < 16; i++)
        {
            kd[2 * i] = k[30 - 2 * i];
            kd[2 * i + 1] = k[31 - 2 * i];
        }
    }

    private static uint Sp(int box, uint work) => DesSpbox[box * 64 + (int)(work & 0x3F)];

    internal void Rounds(bool decrypt, ref uint left, ref uint right)
    {
        uint[] key = decrypt ? kd : k;
        uint l = left, r = right;
        for (int i = 0; i < 8; i++)
        {
            uint work = BitOperations.RotateRight(r, 4) ^ key[4 * i];
            l ^= Sp(6, work) ^ Sp(4, work >> 8) ^ Sp(2, work >> 16) ^ Sp(0, work >> 24);
            work = r ^ key[4 * i + 1];
            l ^= Sp(7, work) ^ Sp(5, work >> 8) ^ Sp(3, work >> 16) ^ Sp(1, work >> 24);
            work = BitOperations.RotateRight(l, 4) ^ key[4 * i + 2];
            r ^= Sp(6, work) ^ Sp(4, work >> 8) ^ Sp(2, work >> 16) ^ Sp(0, work >> 24);
            work = l ^ key[4 * i + 3];
            r ^= Sp(7, work) ^ Sp(5, work >> 8) ^ Sp(3, work >> 16) ^ Sp(1, work >> 24);
        }
        left = l;
        right = r;
    }

    internal static void InitialPermutation(ref uint left, ref uint right)
    {
        uint work;
        right = BitOperations.RotateLeft(right, 4);
        work = (left ^ right) & 0xF0F0F0F0; left ^= work; right = BitOperations.RotateRight(right ^ work, 20);
        work = (left ^ right) & 0xFFFF0000; left ^= work; right = BitOperations.RotateRight(right ^ work, 18);
        work = (left ^ right) & 0x33333333; left ^= work; right = BitOperations.RotateRight(right ^ work, 6);
        work = (left ^ right) & 0x00FF00FF; left ^= work; right = BitOperations.RotateLeft(right ^ work, 9);
        work = (left ^ right) & 0xAAAAAAAA; left = BitOperations.RotateLeft(left ^ work, 1); right ^= work;
    }

    internal static void FinalPermutation(ref uint left, ref uint right)
    {
        uint work;
        right = BitOperations.RotateRight(right, 1);
        work = (left ^ right) & 0xAAAAAAAA; right ^= work; left = BitOperations.RotateRight(left ^ work, 9);
        work = (left ^ right) & 0x00FF00FF; right ^= work; left = BitOperations.RotateLeft(left ^ work, 6);
        work = (left ^ right) & 0x33333333; right ^= work; left = BitOperations.RotateLeft(left ^ work, 18);
        work = (left ^ right) & 0xFFFF0000; right ^= work; left = BitOperations.RotateLeft(left ^ work, 20);
        work = (left ^ right) & 0xF0F0F0F0; right ^= work; left = BitOperations.RotateRight(left ^ work, 4);
    }

    private void Process(bool decrypt, ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint l = Be.U32(input, 0), r = Be.U32(input, 4);
        InitialPermutation(ref l, ref r);
        Rounds(decrypt, ref l, ref r);
        FinalPermutation(ref l, ref r);
        Be.Put(output, 0, r);
        Be.Put(output, 4, l);
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Process(false, input, output);
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Process(true, input, output);
}

// Crypto++ DES_EDE2 (K1,K2,K1) and DES_EDE3 (K1,K2,K3): one IP/FP around three raw passes.
public sealed class DesEdeCipher : ILegacyBlockCipher
{
    private readonly DesCipher k1, k2, k3;
    public int BlockSize => 8;

    public DesEdeCipher(ReadOnlySpan<byte> key)
    {
        k1 = new DesCipher(key[..8]);
        k2 = new DesCipher(key[8..16]);
        k3 = key.Length >= 24 ? new DesCipher(key[16..24]) : k1;
    }

    private void Process(bool decrypt, ReadOnlySpan<byte> input, Span<byte> output)
    {
        uint l = Be.U32(input, 0), r = Be.U32(input, 4);
        DesCipher.InitialPermutation(ref l, ref r);
        if (!decrypt)
        {
            k1.Rounds(false, ref l, ref r);
            k2.Rounds(true, ref r, ref l);
            k3.Rounds(false, ref l, ref r);
        }
        else
        {
            k3.Rounds(true, ref l, ref r);
            k2.Rounds(false, ref r, ref l);
            k1.Rounds(true, ref l, ref r);
        }
        DesCipher.FinalPermutation(ref l, ref r);
        Be.Put(output, 0, r);
        Be.Put(output, 4, l);
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Process(false, input, output);
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Process(true, input, output);
}

// Crypto++ DES_XEX3 (DESX): out = x3 ^ DES_k(in ^ x1), key = x1 || k || x3.
public sealed class DesXex3Cipher : ILegacyBlockCipher
{
    private readonly byte[] x1, x3;
    private readonly DesCipher des;
    public int BlockSize => 8;

    public DesXex3Cipher(ReadOnlySpan<byte> key)
    {
        x1 = key[..8].ToArray();
        des = new DesCipher(key[8..16]);
        x3 = key[16..24].ToArray();
    }

    private void Process(bool decrypt, ReadOnlySpan<byte> input, Span<byte> output)
    {
        byte[] pre = decrypt ? x3 : x1, post = decrypt ? x1 : x3;
        Span<byte> block = stackalloc byte[8];
        for (int i = 0; i < 8; i++) block[i] = (byte)(input[i] ^ pre[i]);
        if (decrypt) des.DecryptBlock(block, block); else des.EncryptBlock(block, block);
        for (int i = 0; i < 8; i++) output[i] = (byte)(block[i] ^ post[i]);
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Process(false, input, output);
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Process(true, input, output);
}

// SKIPJACK (32 rounds, 80-bit key). The core follows the NSA specification; Crypto++ (and so
// the client) reverses the key and block bytes around it: CryptoPP(k, p) = rev(Spec(rev k, rev p)).
public sealed class SkipjackCipher : ILegacyBlockCipher
{
    private readonly byte[] cv;
    public int BlockSize => 8;

    public SkipjackCipher(ReadOnlySpan<byte> key, bool cryptoPPByteOrder = true)
    {
        cv = key[..10].ToArray();
        reversed = cryptoPPByteOrder;
        if (reversed) Array.Reverse(cv);
    }

    private readonly bool reversed;

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Transform(input, output, decrypt: false);
    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output) => Transform(input, output, decrypt: true);

    private void Transform(ReadOnlySpan<byte> input, Span<byte> output, bool decrypt)
    {
        Span<byte> block = stackalloc byte[8];
        input[..8].CopyTo(block);
        if (reversed) block.Reverse();
        if (decrypt) SpecDecrypt(block, block); else SpecEncrypt(block, block);
        if (reversed) block.Reverse();
        block.CopyTo(output);
    }

    private ushort G(int step, ushort w)
    {
        byte g1 = (byte)(w >> 8), g2 = (byte)w;
        byte g3 = (byte)(SkipjackF[g2 ^ cv[4 * step % 10]] ^ g1);
        byte g4 = (byte)(SkipjackF[g3 ^ cv[(4 * step + 1) % 10]] ^ g2);
        byte g5 = (byte)(SkipjackF[g4 ^ cv[(4 * step + 2) % 10]] ^ g3);
        byte g6 = (byte)(SkipjackF[g5 ^ cv[(4 * step + 3) % 10]] ^ g4);
        return (ushort)((g5 << 8) | g6);
    }

    private ushort GInverse(int step, ushort w)
    {
        byte g5 = (byte)(w >> 8), g6 = (byte)w;
        byte g4 = (byte)(SkipjackF[g5 ^ cv[(4 * step + 3) % 10]] ^ g6);
        byte g3 = (byte)(SkipjackF[g4 ^ cv[(4 * step + 2) % 10]] ^ g5);
        byte g2 = (byte)(SkipjackF[g3 ^ cv[(4 * step + 1) % 10]] ^ g4);
        byte g1 = (byte)(SkipjackF[g2 ^ cv[4 * step % 10]] ^ g3);
        return (ushort)((g1 << 8) | g2);
    }

    private static bool RuleA(int round) => (round - 1) / 8 % 2 == 0;

    private void SpecEncrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        ushort w1 = BinaryPrimitives.ReadUInt16BigEndian(input), w2 = BinaryPrimitives.ReadUInt16BigEndian(input[2..]);
        ushort w3 = BinaryPrimitives.ReadUInt16BigEndian(input[4..]), w4 = BinaryPrimitives.ReadUInt16BigEndian(input[6..]);
        for (int round = 1; round <= 32; round++)
        {
            ushort g = G(round - 1, w1);
            if (RuleA(round)) (w1, w2, w3, w4) = ((ushort)(g ^ w4 ^ round), g, w2, w3);
            else (w1, w2, w3, w4) = (w4, g, (ushort)(w1 ^ w2 ^ round), w3);
        }
        BinaryPrimitives.WriteUInt16BigEndian(output, w1);
        BinaryPrimitives.WriteUInt16BigEndian(output[2..], w2);
        BinaryPrimitives.WriteUInt16BigEndian(output[4..], w3);
        BinaryPrimitives.WriteUInt16BigEndian(output[6..], w4);
    }

    private void SpecDecrypt(ReadOnlySpan<byte> input, Span<byte> output)
    {
        ushort w1 = BinaryPrimitives.ReadUInt16BigEndian(input), w2 = BinaryPrimitives.ReadUInt16BigEndian(input[2..]);
        ushort w3 = BinaryPrimitives.ReadUInt16BigEndian(input[4..]), w4 = BinaryPrimitives.ReadUInt16BigEndian(input[6..]);
        for (int round = 32; round >= 1; round--)
        {
            ushort prior = GInverse(round - 1, w2);
            if (RuleA(round)) (w1, w2, w3, w4) = (prior, w3, w4, (ushort)(w1 ^ w2 ^ round));
            else (w1, w2, w3, w4) = (prior, (ushort)(w3 ^ prior ^ round), w4, w1);
        }
        BinaryPrimitives.WriteUInt16BigEndian(output, w1);
        BinaryPrimitives.WriteUInt16BigEndian(output[2..], w2);
        BinaryPrimitives.WriteUInt16BigEndian(output[4..], w3);
        BinaryPrimitives.WriteUInt16BigEndian(output[6..], w4);
    }
}

// SAFER K-128 / SK-128 (Crypto++ SAFER_K / SAFER_SK with a 16-byte key: 10 rounds).
public sealed class SaferCipher : ILegacyBlockCipher
{
    private readonly byte[] keySchedule;
    private readonly int rounds;
    public int BlockSize => 8;

    public SaferCipher(ReadOnlySpan<byte> userKey, bool strengthened)
    {
        rounds = 10;
        ReadOnlySpan<byte> key1 = userKey[..8], key2 = userKey[8..16];
        keySchedule = new byte[8 * (1 + 2 * rounds)];
        Span<byte> ka = stackalloc byte[9], kb = stackalloc byte[9];
        int p = 0;
        for (int j = 0; j < 8; j++)
        {
            ka[8] ^= ka[j] = (byte)((key1[j] << 5) | (key1[j] >> 3));
            kb[8] ^= kb[j] = keySchedule[p++] = key2[j];
        }
        for (int i = 1; i <= rounds; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                ka[j] = (byte)((ka[j] << 6) | (ka[j] >> 2));
                kb[j] = (byte)((kb[j] << 6) | (kb[j] >> 2));
            }
            for (int j = 0; j < 8; j++)
                keySchedule[p++] = (byte)((strengthened ? ka[(j + 2 * i - 1) % 9] : ka[j]) + SaferExp[SaferExp[18 * i + j + 1]]);
            for (int j = 0; j < 8; j++)
                keySchedule[p++] = (byte)((strengthened ? kb[(j + 2 * i) % 9] : kb[j]) + SaferExp[SaferExp[18 * i + j + 10]]);
        }
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Span<byte> x = stackalloc byte[8];
        input[..8].CopyTo(x);
        ReadOnlySpan<byte> key = keySchedule;
        int p = 0;
        for (int round = 0; round < rounds; round++, p += 16)
        {
            x[0] = (byte)(SaferExp[x[0] ^ key[p]] + key[p + 8]);
            x[1] = (byte)(SaferLog[(byte)(x[1] + key[p + 1])] ^ key[p + 9]);
            x[2] = (byte)(SaferLog[(byte)(x[2] + key[p + 2])] ^ key[p + 10]);
            x[3] = (byte)(SaferExp[x[3] ^ key[p + 3]] + key[p + 11]);
            x[4] = (byte)(SaferExp[x[4] ^ key[p + 4]] + key[p + 12]);
            x[5] = (byte)(SaferLog[(byte)(x[5] + key[p + 5])] ^ key[p + 13]);
            x[6] = (byte)(SaferLog[(byte)(x[6] + key[p + 6])] ^ key[p + 14]);
            x[7] = (byte)(SaferExp[x[7] ^ key[p + 7]] + key[p + 15]);
            byte a = x[0], b = x[1], c = x[2], d = x[3], e = x[4], f = x[5], g = x[6], h = x[7];
            Pht(ref a, ref b); Pht(ref c, ref d); Pht(ref e, ref f); Pht(ref g, ref h);
            Pht(ref a, ref c); Pht(ref e, ref g); Pht(ref b, ref d); Pht(ref f, ref h);
            Pht(ref a, ref e); Pht(ref b, ref f); Pht(ref c, ref g); Pht(ref d, ref h);
            x[0] = a; x[1] = e; x[2] = b; x[3] = f; x[4] = c; x[5] = g; x[6] = d; x[7] = h;
        }
        output[0] = (byte)(x[0] ^ key[p]); output[1] = (byte)(x[1] + key[p + 1]);
        output[2] = (byte)(x[2] + key[p + 2]); output[3] = (byte)(x[3] ^ key[p + 3]);
        output[4] = (byte)(x[4] ^ key[p + 4]); output[5] = (byte)(x[5] + key[p + 5]);
        output[6] = (byte)(x[6] + key[p + 6]); output[7] = (byte)(x[7] ^ key[p + 7]);
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        ReadOnlySpan<byte> key = keySchedule;
        int p = 16 * rounds;
        byte a = (byte)(input[0] ^ key[p]), b = (byte)(input[1] - key[p + 1]), c = (byte)(input[2] - key[p + 2]), d = (byte)(input[3] ^ key[p + 3]);
        byte e = (byte)(input[4] ^ key[p + 4]), f = (byte)(input[5] - key[p + 5]), g = (byte)(input[6] - key[p + 6]), h = (byte)(input[7] ^ key[p + 7]);
        for (int round = rounds - 1; round >= 0; round--)
        {
            p = 16 * round;
            (b, c, d, e, f, g) = (c, e, g, b, d, f);
            Ipht(ref a, ref e); Ipht(ref b, ref f); Ipht(ref c, ref g); Ipht(ref d, ref h);
            Ipht(ref a, ref c); Ipht(ref e, ref g); Ipht(ref b, ref d); Ipht(ref f, ref h);
            Ipht(ref a, ref b); Ipht(ref c, ref d); Ipht(ref e, ref f); Ipht(ref g, ref h);
            a = (byte)(SaferLog[(byte)(a - key[p + 8])] ^ key[p]);
            b = (byte)(SaferExp[b ^ key[p + 9]] - key[p + 1]);
            c = (byte)(SaferExp[c ^ key[p + 10]] - key[p + 2]);
            d = (byte)(SaferLog[(byte)(d - key[p + 11])] ^ key[p + 3]);
            e = (byte)(SaferLog[(byte)(e - key[p + 12])] ^ key[p + 4]);
            f = (byte)(SaferExp[f ^ key[p + 13]] - key[p + 5]);
            g = (byte)(SaferExp[g ^ key[p + 14]] - key[p + 6]);
            h = (byte)(SaferLog[(byte)(h - key[p + 15])] ^ key[p + 7]);
        }
        output[0] = a; output[1] = b; output[2] = c; output[3] = d; output[4] = e; output[5] = f; output[6] = g; output[7] = h;
    }

    private static void Pht(ref byte x, ref byte y)
    {
        y = (byte)(y + x);
        x = (byte)(x + y);
    }

    private static void Ipht(ref byte x, ref byte y)
    {
        x = (byte)(x - y);
        y = (byte)(y - x);
    }
}
