namespace C9.Protocol;

// One direction of the transport cipher (client b4580ed4: state 00D04730, transforms
// 00D04940/00D049D0). Every transformed frame writes up to 8 plaintext bytes into a 128-byte
// table; after the epoch's frame threshold the table seeds the next cipher (00D04870).
public sealed class CipherDirection
{
    private readonly byte[] table = Enumerable.Repeat((byte)99, 128).ToArray();
    private CipherEpoch epoch;
    private int framesInEpoch;

    public int FramesProcessed { get; private set; }
    public int Epoch { get; private set; }
    public string CipherName => epoch.Name;
    public int BlockSize => epoch.Cipher.BlockSize;
    public byte[] GetStateTable() => (byte[])table.Clone();

    public CipherDirection()
    {
        epoch = CipherEpoch.Derive(CipherEpoch.InitialSeed);
        if (epoch.Name != "RC6" || epoch.HashCase != 7) throw new InvalidOperationException("Initial cipher derivation mismatch");
    }

    // Length on the wire for a plaintext: zero-padded blocks plus one padding-count byte.
    public int EncryptedLength(int plaintextLength) => checked((plaintextLength + BlockSize - 1) / BlockSize * BlockSize + 1);

    internal void Commit(ReadOnlySpan<byte> transformedPlaintext)
    {
        int count = transformedPlaintext.Length > 8 ? 8 : Math.Min(1, transformedPlaintext.Length);
        foreach (byte value in transformedPlaintext[..count]) table[value & 127] = value;
        FramesProcessed++;
        if (++framesInEpoch < epoch.Threshold) return;
        epoch = CipherEpoch.Derive(table);
        framesInEpoch = 0;
        Epoch++;
    }

    // 00D04940: encrypt one frame, then record its plaintext (and rekey when the epoch ends).
    public byte[] Seal(ReadOnlySpan<byte> plaintext)
    {
        byte[] wire = Encrypt(plaintext);
        Commit(plaintext);
        return wire;
    }

    // 00D049D0: decrypt one frame, then record its plaintext.
    public byte[] Open(ReadOnlySpan<byte> ciphertext)
    {
        byte[] plaintext = Decrypt(ciphertext);
        Commit(plaintext);
        return plaintext;
    }

    public byte[] Encrypt(ReadOnlySpan<byte> plaintext)
    {
        int blockSize = BlockSize;
        int padding = (blockSize - plaintext.Length % blockSize) % blockSize;
        int paddedLength = checked(plaintext.Length + padding);
        byte[] encrypted = new byte[paddedLength + 1];
        Span<byte> block = stackalloc byte[blockSize];
        for (int offset = 0; offset < paddedLength; offset += blockSize)
        {
            block.Clear();
            plaintext.Slice(offset, Math.Min(blockSize, plaintext.Length - offset)).CopyTo(block);
            epoch.Cipher.EncryptBlock(block, encrypted.AsSpan(offset, blockSize));
        }
        encrypted[^1] = (byte)padding;
        return encrypted;
    }

    public byte[] Decrypt(ReadOnlySpan<byte> ciphertext)
    {
        int blockSize = BlockSize;
        if (ciphertext.Length < blockSize + 1 || ciphertext.Length % blockSize != 1 || ciphertext[^1] >= blockSize)
            throw new ProtocolException(ProtocolError.InvalidPadding, $"Invalid {epoch.Name} wrapper length/footer");
        int padding = ciphertext[^1];
        byte[] clear = new byte[ciphertext.Length - 1];
        for (int offset = 0; offset < clear.Length; offset += blockSize)
            epoch.Cipher.DecryptBlock(ciphertext.Slice(offset, blockSize), clear.AsSpan(offset, blockSize));
        for (int i = clear.Length - padding; i < clear.Length; i++)
            if (clear[i] != 0) throw new ProtocolException(ProtocolError.InvalidPadding, $"Nonzero {epoch.Name} wrapper padding");
        return padding == 0 ? clear : clear.AsSpan(0, clear.Length - padding).ToArray();
    }
}
