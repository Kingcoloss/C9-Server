using System.Buffers.Binary;
using System.Numerics;

namespace C9.Protocol;

// RC6-32/20 for the recovered legacy wire protocol, with independent KATs.
public sealed class Rc6Cipher
{
    public const int BlockSize = 16;
    private readonly uint[] schedule = new uint[44];

    public Rc6Cipher(ReadOnlySpan<byte> key)
    {
        if (key.Length is not (16 or 24 or 32))
            throw new ArgumentException("RC6 requires a 16, 24 or 32 byte key", nameof(key));
        Span<uint> words = stackalloc uint[key.Length / 4];
        for (int n = 0; n < words.Length; n++)
            words[n] = BinaryPrimitives.ReadUInt32LittleEndian(key[(n * 4)..]);
        unchecked
        {
            for (uint n = 0; n < schedule.Length; n++)
                schedule[n] = 0xB7E15163 + n * 0x9E3779B9;
            uint a = 0, b = 0;
            int i = 0, j = 0;
            for (int n = 0; n < 3 * Math.Max(schedule.Length, words.Length); n++)
            {
                a = schedule[i] = BitOperations.RotateLeft(schedule[i] + a + b, 3);
                b = words[j] = BitOperations.RotateLeft(words[j] + a + b, (int)(a + b));
                i = (i + 1) % schedule.Length;
                j = (j + 1) % words.Length;
            }
        }
        words.Clear();
    }

    public void EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Validate(input, output);
        uint a = BinaryPrimitives.ReadUInt32LittleEndian(input);
        uint b = BinaryPrimitives.ReadUInt32LittleEndian(input[4..]);
        uint c = BinaryPrimitives.ReadUInt32LittleEndian(input[8..]);
        uint d = BinaryPrimitives.ReadUInt32LittleEndian(input[12..]);
        unchecked
        {
            b += schedule[0];
            d += schedule[1];
            for (int round = 1; round <= 20; round++)
            {
                uint t = BitOperations.RotateLeft(b * (2 * b + 1), 5);
                uint u = BitOperations.RotateLeft(d * (2 * d + 1), 5);
                a = BitOperations.RotateLeft(a ^ t, (int)u) + schedule[2 * round];
                c = BitOperations.RotateLeft(c ^ u, (int)t) + schedule[2 * round + 1];
                (a, b, c, d) = (b, c, d, a);
            }
            Write(output, a + schedule[42], b, c + schedule[43], d);
        }
    }

    public void DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Validate(input, output);
        uint a = BinaryPrimitives.ReadUInt32LittleEndian(input);
        uint b = BinaryPrimitives.ReadUInt32LittleEndian(input[4..]);
        uint c = BinaryPrimitives.ReadUInt32LittleEndian(input[8..]);
        uint d = BinaryPrimitives.ReadUInt32LittleEndian(input[12..]);
        unchecked
        {
            a -= schedule[42];
            c -= schedule[43];
            for (int round = 20; round > 0; round--)
            {
                (a, b, c, d) = (d, a, b, c);
                uint u = BitOperations.RotateLeft(d * (2 * d + 1), 5);
                uint t = BitOperations.RotateLeft(b * (2 * b + 1), 5);
                c = BitOperations.RotateRight(c - schedule[2 * round + 1], (int)t) ^ u;
                a = BitOperations.RotateRight(a - schedule[2 * round], (int)u) ^ t;
            }
            Write(output, a, b - schedule[0], c, d - schedule[1]);
        }
    }

    private static void Validate(ReadOnlySpan<byte> input, Span<byte> output)
    {
        if (input.Length != BlockSize || output.Length < BlockSize)
            throw new ArgumentException("RC6 requires a full 16-byte input/output block");
    }

    private static void Write(Span<byte> output, uint a, uint b, uint c, uint d)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(output, a);
        BinaryPrimitives.WriteUInt32LittleEndian(output[4..], b);
        BinaryPrimitives.WriteUInt32LittleEndian(output[8..], c);
        BinaryPrimitives.WriteUInt32LittleEndian(output[12..], d);
    }
}
