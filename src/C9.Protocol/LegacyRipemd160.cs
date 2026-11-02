using System.Buffers.Binary;
using System.Numerics;

namespace C9.Protocol;

// Managed compatibility primitive, so the server does not depend on OpenSSL's
// optional legacy algorithms. Specification/KATs: Bosselaers' RIPEMD-160 page.
public static class LegacyRipemd160
{
    private static readonly int[] LeftOrder = [
        0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,
        7,4,13,1,10,6,15,3,12,0,9,5,2,14,11,8,
        3,10,14,4,9,15,8,1,2,7,0,6,13,11,5,12,
        1,9,11,10,0,8,12,4,13,3,7,15,14,5,6,2,
        4,0,5,9,7,12,2,10,14,1,3,8,11,6,15,13];
    private static readonly int[] RightOrder = [
        5,14,7,0,9,2,11,4,13,6,15,8,1,10,3,12,
        6,11,3,7,0,13,5,10,14,15,8,12,4,9,1,2,
        15,5,1,3,7,14,6,9,11,8,12,2,10,0,4,13,
        8,6,4,1,3,11,15,0,5,12,2,13,9,7,10,14,
        12,15,10,4,1,5,8,7,6,2,13,14,0,3,9,11];
    private static readonly int[] LeftShift = [
        11,14,15,12,5,8,7,9,11,13,14,15,6,7,9,8,
        7,6,8,13,11,9,7,15,7,12,15,9,11,7,13,12,
        11,13,6,7,14,9,13,15,14,8,13,6,5,12,7,5,
        11,12,14,15,14,15,9,8,9,14,5,6,8,6,5,12,
        9,15,5,11,6,8,13,12,5,12,13,14,11,8,5,6];
    private static readonly int[] RightShift = [
        8,9,9,11,13,15,15,5,7,7,8,11,14,14,12,6,
        9,13,15,7,12,8,9,11,7,7,12,7,6,15,13,11,
        9,7,15,11,8,6,6,14,12,13,5,14,13,13,7,5,
        15,5,8,11,14,14,6,14,6,9,12,9,12,5,15,8,
        8,5,12,9,12,5,14,6,8,13,6,5,15,13,11,11];
    private static readonly uint[] LeftConstant = [0, 0x5A827999, 0x6ED9EBA1, 0x8F1BBCDC, 0xA953FD4E];
    private static readonly uint[] RightConstant = [0x50A28BE6, 0x5C4DD124, 0x6D703EF3, 0x7A6D76E9, 0];

    private static uint Function(int group, uint x, uint y, uint z) => group switch
    {
        0 => x ^ y ^ z,
        1 => (x & y) | (~x & z),
        2 => (x | ~y) ^ z,
        3 => (x & z) | (y & ~z),
        4 => x ^ (y | ~z),
        _ => throw new ArgumentOutOfRangeException(nameof(group))
    };

    public static byte[] Hash(ReadOnlySpan<byte> data)
    {
        int paddedSize = checked((data.Length + 9 + 63) / 64 * 64);
        byte[] padded = new byte[paddedSize];
        data.CopyTo(padded);
        padded[data.Length] = 0x80;
        BinaryPrimitives.WriteUInt64LittleEndian(padded.AsSpan(paddedSize - 8), (ulong)data.Length * 8);
        uint h0 = 0x67452301, h1 = 0xEFCDAB89, h2 = 0x98BADCFE, h3 = 0x10325476, h4 = 0xC3D2E1F0;
        Span<uint> words = stackalloc uint[16];
        unchecked
        {
            for (int offset = 0; offset < padded.Length; offset += 64)
            {
                for (int i = 0; i < 16; i++)
                    words[i] = BinaryPrimitives.ReadUInt32LittleEndian(padded.AsSpan(offset + 4 * i));
                uint a = h0, b = h1, c = h2, d = h3, e = h4;
                uint ar = h0, br = h1, cr = h2, dr = h3, er = h4;
                for (int j = 0; j < 80; j++)
                {
                    int group = j / 16;
                    uint t = BitOperations.RotateLeft(a + Function(group, b, c, d) + words[LeftOrder[j]] + LeftConstant[group], LeftShift[j]) + e;
                    (a, e, d, c, b) = (e, d, BitOperations.RotateLeft(c, 10), b, t);
                    t = BitOperations.RotateLeft(ar + Function(4 - group, br, cr, dr) + words[RightOrder[j]] + RightConstant[group], RightShift[j]) + er;
                    (ar, er, dr, cr, br) = (er, dr, BitOperations.RotateLeft(cr, 10), br, t);
                }
                uint combined = h1 + c + dr;
                h1 = h2 + d + er;
                h2 = h3 + e + ar;
                h3 = h4 + a + br;
                h4 = h0 + b + cr;
                h0 = combined;
            }
        }
        byte[] digest = new byte[20];
        BinaryPrimitives.WriteUInt32LittleEndian(digest, h0);
        BinaryPrimitives.WriteUInt32LittleEndian(digest.AsSpan(4), h1);
        BinaryPrimitives.WriteUInt32LittleEndian(digest.AsSpan(8), h2);
        BinaryPrimitives.WriteUInt32LittleEndian(digest.AsSpan(12), h3);
        BinaryPrimitives.WriteUInt32LittleEndian(digest.AsSpan(16), h4);
        return digest;
    }
}
