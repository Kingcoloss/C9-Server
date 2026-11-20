using System.Buffers.Binary;
using System.Numerics;
using System.Security.Cryptography;

namespace C9.Protocol;

// Hashes that the client cipher-epoch rekey (00D0A880) can select. MD4 and RIPEMD-128/256/320
// are managed so the server does not depend on OpenSSL legacy providers; vectors come from
// Crypto++ 8.9 (tools/cipher-epoch-ref), the library the client is built with.
public static class LegacyHashes
{
    public static byte[] Md4(ReadOnlySpan<byte> data)
    {
        uint[] h = [0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476];
        Span<uint> x = stackalloc uint[16];
        unchecked
        {
            foreach (int offset in Blocks(data, out byte[] padded))
            {
                Load(padded.AsSpan(offset), x);
                uint a = h[0], b = h[1], c = h[2], d = h[3];
                int[] s1 = [3, 7, 11, 19], s2 = [3, 5, 9, 13], s3 = [3, 9, 11, 15];
                for (int i = 0; i < 16; i++)
                {
                    uint t = BitOperations.RotateLeft(a + ((b & c) | (~b & d)) + x[i], s1[i % 4]);
                    (a, b, c, d) = (d, t, b, c);
                }
                for (int i = 0; i < 16; i++)
                {
                    int k = (i % 4) * 4 + i / 4;
                    uint t = BitOperations.RotateLeft(a + ((b & c) | (b & d) | (c & d)) + x[k] + 0x5A827999, s2[i % 4]);
                    (a, b, c, d) = (d, t, b, c);
                }
                int[] order3 = [0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15];
                for (int i = 0; i < 16; i++)
                {
                    uint t = BitOperations.RotateLeft(a + (b ^ c ^ d) + x[order3[i]] + 0x6ED9EBA1, s3[i % 4]);
                    (a, b, c, d) = (d, t, b, c);
                }
                h[0] += a; h[1] += b; h[2] += c; h[3] += d;
            }
        }
        return Store(h);
    }

    public static byte[] Ripemd128(ReadOnlySpan<byte> data) => RipemdDouble(data, wide: false);

    public static byte[] Ripemd256(ReadOnlySpan<byte> data) => RipemdDouble(data, wide: true);

    // RIPEMD-128 and its double-width variant 256 (registers swapped after each round).
    private static byte[] RipemdDouble(ReadOnlySpan<byte> data, bool wide)
    {
        uint[] h = wide
            ? [0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0x76543210, 0xFEDCBA98, 0x89ABCDEF, 0x01234567]
            : [0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476];
        uint[] leftK = [0, 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC];
        uint[] rightK = [0x50A28BE6, 0x5C4DD124, 0x6D703EF3, 0];
        Span<uint> x = stackalloc uint[16];
        Span<uint> l = stackalloc uint[4];
        Span<uint> r = stackalloc uint[4];
        unchecked
        {
            foreach (int offset in Blocks(data, out byte[] padded))
            {
                Load(padded.AsSpan(offset), x);
                for (int i = 0; i < 4; i++)
                {
                    l[i] = h[i];
                    r[i] = wide ? h[i + 4] : h[i];
                }
                for (int round = 0; round < 4; round++)
                {
                    for (int j = round * 16; j < round * 16 + 16; j++)
                    {
                        uint t = BitOperations.RotateLeft(l[0] + Ripemd.F(round, l[1], l[2], l[3]) + x[Ripemd.LeftOrder[j]] + leftK[round], Ripemd.LeftShift[j]);
                        (l[0], l[1], l[2], l[3]) = (l[3], t, l[1], l[2]);
                        t = BitOperations.RotateLeft(r[0] + Ripemd.F(3 - round, r[1], r[2], r[3]) + x[Ripemd.RightOrder[j]] + rightK[round], Ripemd.RightShift[j]);
                        (r[0], r[1], r[2], r[3]) = (r[3], t, r[1], r[2]);
                    }
                    if (wide) (l[round], r[round]) = (r[round], l[round]);
                }
                if (wide)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        h[i] += l[i];
                        h[i + 4] += r[i];
                    }
                }
                else
                {
                    uint t = h[1] + l[2] + r[3];
                    h[1] = h[2] + l[3] + r[0];
                    h[2] = h[3] + l[0] + r[1];
                    h[3] = h[0] + l[1] + r[2];
                    h[0] = t;
                }
            }
        }
        return Store(h);
    }

    // RIPEMD-320: two RIPEMD-160 lines, one register exchanged after each round (B, D, A, C, E).
    public static byte[] Ripemd320(ReadOnlySpan<byte> data)
    {
        uint[] h = [0x67452301, 0xEFCDAB89, 0x98BADCFE, 0x10325476, 0xC3D2E1F0,
                    0x76543210, 0xFEDCBA98, 0x89ABCDEF, 0x01234567, 0x3C2D1E0F];
        uint[] leftK = [0, 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xA953FD4E];
        uint[] rightK = [0x50A28BE6, 0x5C4DD124, 0x6D703EF3, 0x7A6D76E9, 0];
        int[] exchange = [1, 3, 0, 2, 4];
        Span<uint> x = stackalloc uint[16];
        Span<uint> l = stackalloc uint[5];
        Span<uint> r = stackalloc uint[5];
        unchecked
        {
            foreach (int offset in Blocks(data, out byte[] padded))
            {
                Load(padded.AsSpan(offset), x);
                for (int i = 0; i < 5; i++)
                {
                    l[i] = h[i];
                    r[i] = h[i + 5];
                }
                for (int round = 0; round < 5; round++)
                {
                    for (int j = round * 16; j < round * 16 + 16; j++)
                    {
                        uint t = BitOperations.RotateLeft(l[0] + Ripemd.F(round, l[1], l[2], l[3]) + x[Ripemd.LeftOrder[j]] + leftK[round], Ripemd.LeftShift[j]) + l[4];
                        (l[0], l[1], l[2], l[3], l[4]) = (l[4], t, l[1], BitOperations.RotateLeft(l[2], 10), l[3]);
                        t = BitOperations.RotateLeft(r[0] + Ripemd.F(4 - round, r[1], r[2], r[3]) + x[Ripemd.RightOrder[j]] + rightK[round], Ripemd.RightShift[j]) + r[4];
                        (r[0], r[1], r[2], r[3], r[4]) = (r[4], t, r[1], BitOperations.RotateLeft(r[2], 10), r[3]);
                    }
                    int e = exchange[round];
                    (l[e], r[e]) = (r[e], l[e]);
                }
                for (int i = 0; i < 5; i++)
                {
                    h[i] += l[i];
                    h[i + 5] += r[i];
                }
            }
        }
        return Store(h);
    }

    // Digest for the rekey hash case (selector % 11) of 00D0A880.
    public static byte[] ForEpochCase(int hashCase, ReadOnlySpan<byte> data) => hashCase switch
    {
        1 => SHA256.HashData(data),
        3 => SHA512.HashData(data),
        4 => SHA384.HashData(data),
        5 => Md4(data),
        7 => LegacyRipemd160.Hash(data),
        8 => Ripemd320(data),
        9 => Ripemd128(data),
        10 => Ripemd256(data),
        0 or 2 or 6 => MD5.HashData(data),
        _ => throw new ArgumentOutOfRangeException(nameof(hashCase)),
    };

    private static IEnumerable<int> Blocks(ReadOnlySpan<byte> data, out byte[] padded)
    {
        int size = checked((data.Length + 9 + 63) / 64 * 64);
        padded = new byte[size];
        data.CopyTo(padded);
        padded[data.Length] = 0x80;
        BinaryPrimitives.WriteUInt64LittleEndian(padded.AsSpan(size - 8), (ulong)data.Length * 8);
        return Enumerable.Range(0, size / 64).Select(i => i * 64);
    }

    private static void Load(ReadOnlySpan<byte> block, Span<uint> x)
    {
        for (int i = 0; i < 16; i++) x[i] = BinaryPrimitives.ReadUInt32LittleEndian(block[(i * 4)..]);
    }

    private static byte[] Store(uint[] h)
    {
        byte[] digest = new byte[h.Length * 4];
        for (int i = 0; i < h.Length; i++) BinaryPrimitives.WriteUInt32LittleEndian(digest.AsSpan(i * 4), h[i]);
        return digest;
    }

    private static class Ripemd
    {
        public static readonly int[] LeftOrder = [
            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15, 7,4,13,1,10,6,15,3,12,0,9,5,2,14,11,8,
            3,10,14,4,9,15,8,1,2,7,0,6,13,11,5,12, 1,9,11,10,0,8,12,4,13,3,7,15,14,5,6,2,
            4,0,5,9,7,12,2,10,14,1,3,8,11,6,15,13];
        public static readonly int[] RightOrder = [
            5,14,7,0,9,2,11,4,13,6,15,8,1,10,3,12, 6,11,3,7,0,13,5,10,14,15,8,12,4,9,1,2,
            15,5,1,3,7,14,6,9,11,8,12,2,10,0,4,13, 8,6,4,1,3,11,15,0,5,12,2,13,9,7,10,14,
            12,15,10,4,1,5,8,7,6,2,13,14,0,3,9,11];
        public static readonly int[] LeftShift = [
            11,14,15,12,5,8,7,9,11,13,14,15,6,7,9,8, 7,6,8,13,11,9,7,15,7,12,15,9,11,7,13,12,
            11,13,6,7,14,9,13,15,14,8,13,6,5,12,7,5, 11,12,14,15,14,15,9,8,9,14,5,6,8,6,5,12,
            9,15,5,11,6,8,13,12,5,12,13,14,11,8,5,6];
        public static readonly int[] RightShift = [
            8,9,9,11,13,15,15,5,7,7,8,11,14,14,12,6, 9,13,15,7,12,8,9,11,7,7,12,7,6,15,13,11,
            9,7,15,11,8,6,6,14,12,13,5,14,13,13,7,5, 15,5,8,11,14,14,6,14,6,9,12,9,12,5,15,8,
            8,5,12,9,12,5,14,6,8,13,6,5,15,13,11,11];

        public static uint F(int group, uint x, uint y, uint z) => group switch
        {
            0 => x ^ y ^ z,
            1 => (x & y) | (~x & z),
            2 => (x | ~y) ^ z,
            3 => (x & z) | (y & ~z),
            _ => x ^ (y | ~z),
        };
    }
}
