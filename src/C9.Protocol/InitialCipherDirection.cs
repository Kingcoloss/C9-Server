namespace C9.Protocol;

public sealed class InitialCipherDirection
{
    public const int FrameLimit = 256;
    private static readonly byte[] Seed = Convert.FromHexString("3333323334353637383930313233343536373839303132333435363738393132");
    private readonly Rc6Cipher cipher;
    private readonly byte[] table = Enumerable.Repeat((byte)99, 128).ToArray();
    public int FramesProcessed { get; private set; }
    public byte[] GetStateTable() => (byte[])table.Clone();

    public InitialCipherDirection()
    {
        byte[] digest = LegacyRipemd160.Hash(Seed);
        if (Convert.ToHexStringLower(digest) != "6f749a998ed7373b8413712b6b3a201381850d31" || digest[5] % 10 != 5)
            throw new InvalidOperationException("Initial cipher derivation mismatch");
        cipher = new Rc6Cipher(digest.AsSpan(0, 16));
    }

    public void CheckEpoch()
    {
        if (FramesProcessed >= FrameLimit) throw new UnsupportedCipherEpochException();
    }

    internal void Commit(ReadOnlySpan<byte> transformedPlaintext)
    {
        int count = transformedPlaintext.Length > 8 ? 8 : Math.Min(1, transformedPlaintext.Length);
        foreach (byte value in transformedPlaintext[..count]) table[value & 127] = value;
        FramesProcessed++;
    }

    public byte[] Encrypt(ReadOnlySpan<byte> plaintext)
    {
        CheckEpoch();
        int padding = (16 - plaintext.Length % 16) % 16;
        int paddedLength = checked(plaintext.Length + padding);
        byte[] encrypted = new byte[paddedLength + 1];
        Span<byte> block = stackalloc byte[16];
        for (int offset = 0; offset < paddedLength; offset += 16)
        {
            block.Clear();
            plaintext.Slice(offset, Math.Min(16, plaintext.Length - offset)).CopyTo(block);
            cipher.EncryptBlock(block, encrypted.AsSpan(offset, 16));
        }
        encrypted[^1] = (byte)padding;
        return encrypted;
    }

    public byte[] Decrypt(ReadOnlySpan<byte> ciphertext)
    {
        CheckEpoch();
        if (ciphertext.Length < 17 || ciphertext.Length % 16 != 1 || ciphertext[^1] >= 16)
            throw new ProtocolException(ProtocolError.InvalidPadding, "Invalid RC6 wrapper length/footer");
        int padding = ciphertext[^1];
        byte[] clear = new byte[ciphertext.Length - 1];
        for (int offset = 0; offset < clear.Length; offset += 16)
            cipher.DecryptBlock(ciphertext.Slice(offset, 16), clear.AsSpan(offset, 16));
        for (int i = clear.Length - padding; i < clear.Length; i++)
            if (clear[i] != 0) throw new ProtocolException(ProtocolError.InvalidPadding, "Nonzero RC6 wrapper padding");
        return padding == 0 ? clear : clear.AsSpan(0, clear.Length - padding).ToArray();
    }
}
