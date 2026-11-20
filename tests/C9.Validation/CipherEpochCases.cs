using System.Security.Cryptography;
using System.Text.Json;
using C9.Protocol;
using C9.Server;

internal static partial class Validation
{
    // Vectors from tools/cipher-epoch-ref (Crypto++ 8.9, the library family the client links).
    private static void CipherEpochKnownAnswers()
    {
        using JsonDocument source = Fixture("cipher_epoch_vectors.json");
        JsonElement root = source.RootElement;
        HashSet<string> names = [];
        foreach (JsonElement vector in root.GetProperty("ciphers").EnumerateArray())
        {
            byte[] key = new byte[64];
            Convert.FromHexString(vector.GetProperty("key").GetString()!).CopyTo(key, 0);
            CipherEpoch epoch = CipherEpoch.Create(vector.GetProperty("index").GetInt32(), key);
            Check.Equal(vector.GetProperty("name").GetString(), epoch.Name);
            Check.Equal(vector.GetProperty("threshold").GetInt32(), epoch.Threshold);
            Check.Equal(vector.GetProperty("block").GetInt32(), epoch.Cipher.BlockSize);
            byte[] plain = Convert.FromHexString(vector.GetProperty("plain").GetString()!);
            byte[] expected = Convert.FromHexString(vector.GetProperty("cipher").GetString()!);
            byte[] encrypted = new byte[plain.Length], decrypted = new byte[plain.Length];
            for (int i = 0; i < plain.Length; i += epoch.Cipher.BlockSize)
            {
                epoch.Cipher.EncryptBlock(plain.AsSpan(i, epoch.Cipher.BlockSize), encrypted.AsSpan(i));
                epoch.Cipher.DecryptBlock(expected.AsSpan(i, epoch.Cipher.BlockSize), decrypted.AsSpan(i));
            }
            Check.Bytes(expected, encrypted);
            Check.Bytes(plain, decrypted);
            names.Add(epoch.Name);
        }
        Check.Equal(16, names.Count);
        foreach (JsonElement vector in root.GetProperty("hashes").EnumerateArray())
            Check.Equal(vector.GetProperty("digest").GetString(), Convert.ToHexStringLower(LegacyHashes.ForEpochCase(
                vector.GetProperty("case").GetInt32(), Convert.FromHexString(vector.GetProperty("input").GetString()!))));
        foreach (JsonElement vector in root.GetProperty("selections").EnumerateArray())
        {
            CipherEpoch epoch = CipherEpoch.Derive(Convert.FromHexString(vector.GetProperty("seed").GetString()!));
            Check.Equal(vector.GetProperty("cipher").GetString(), epoch.Name);
            Check.Equal(vector.GetProperty("hashCase").GetInt32(), epoch.HashCase);
            Check.Equal(vector.GetProperty("threshold").GetInt32(), epoch.Threshold);
        }

        // Published vectors, independent of Crypto++.
        static string Encrypt(ILegacyBlockCipher cipher, string plain)
        {
            byte[] input = Convert.FromHexString(plain), output = new byte[input.Length];
            cipher.EncryptBlock(input, output);
            return Convert.ToHexStringLower(output);
        }
        Check.Equal("238b4fe5847e44b2", Encrypt(new Cast128Cipher(Convert.FromHexString("0123456712345678234567893456789A")), "0123456789ABCDEF"));
        Check.Equal("85e813540f0ab405", Encrypt(new DesCipher(Convert.FromHexString("133457799BBCDFF1")), "0123456789ABCDEF"));
        Check.Equal("2587cae27a12d300", Encrypt(new SkipjackCipher(Convert.FromHexString("00998877665544332211"), cryptoPPByteOrder: false), "33221100DDCCBBAA"));
        Check.Equal("9f589f5cf6122c32b6bfec2f2ae8c35a", Encrypt(new TwofishCipher(new byte[16]), "00000000000000000000000000000000"));
        Check.Equal("11fbed2b01986de5", Encrypt(new IdeaCipher(Convert.FromHexString("00010002000300040005000600070008")), "0000000100020003"));
        Check.Equal("a448017aaf21d8525fc10ae87aa6729d", Convert.ToHexStringLower(LegacyHashes.Md4("abc"u8)));
        Check.Equal("c14a12199c66e4ba84636b0f69144c77", Convert.ToHexStringLower(LegacyHashes.Ripemd128("abc"u8)));
        Check.Equal("afbd6e228b9d8cbbcef5ca2d03e6dba10ac0bc7dcbe4680e1e42d2e975459b65", Convert.ToHexStringLower(LegacyHashes.Ripemd256("abc"u8)));
        Check.Equal("de4c01b3054f8930a79d09ae738e92301e5a17085beffdc1b8d116713e74f82fa942d64cdbc4682d", Convert.ToHexStringLower(LegacyHashes.Ripemd320("abc"u8)));
    }

    // Whole directions over many epochs: the wire bytes must equal the Crypto++ model exactly.
    private static void CipherEpochStreams()
    {
        using JsonDocument source = Fixture("cipher_epoch_vectors.json");
        foreach (JsonElement stream in source.RootElement.GetProperty("streams").EnumerateArray())
        {
            ulong state = stream.GetProperty("rngSeed").GetUInt64();
            uint Next()
            {
                state ^= state << 13; state ^= state >> 7; state ^= state << 17;
                return (uint)(state >> 32);
            }
            int frames = stream.GetProperty("frames").GetInt32();
            uint maxExtra = stream.GetProperty("maxExtra").GetUInt32();
            Dictionary<int, string> samples = stream.GetProperty("samples").EnumerateArray()
                .ToDictionary(s => s.GetProperty("frame").GetInt32(), s => s.GetProperty("wire").GetString()!);
            List<string> epochs = stream.GetProperty("firstEpochs").EnumerateArray().Select(e => e.GetString()!).ToList();
            CipherDirection client = new(), server = new();
            using IncrementalHash wireHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            List<string> seen = [client.CipherName];
            for (int frame = 1; frame <= frames; frame++)
            {
                byte[] plain = new byte[10 + (int)(Next() % maxExtra)];
                for (int i = 0; i < plain.Length; i++) plain[i] = (byte)Next();
                int epochBefore = client.Epoch;
                byte[] wire = client.Seal(plain);
                Check.Bytes(plain, server.Open(wire));
                wireHash.AppendData(wire);
                if (samples.TryGetValue(frame, out string? expected)) Check.Equal(expected, Convert.ToHexStringLower(wire));
                if (client.Epoch != epochBefore) seen.Add(client.CipherName);
            }
            Check.Equal(stream.GetProperty("wireSha256").GetString(), Convert.ToHexStringLower(wireHash.GetHashAndReset()));
            Check.Equal(stream.GetProperty("epochs").GetInt32(), seen.Count);
            Check.True(seen.Take(epochs.Count).SequenceEqual(epochs));
            Check.Equal(frames, server.FramesProcessed);
            Check.Bytes(client.GetStateTable(), server.GetStateTable());
        }
    }

    // The codec rekeys after the 256th encrypted frame instead of stopping, in both directions.
    private static void CipherEpochCodecRotation()
    {
        ConnectionCodec sender = new(), receiver = new();
        for (uint i = 0; i < 2000; i++)
        {
            byte[] payload = BitConverter.GetBytes(i * 2654435761u).Concat(new byte[i % 37]).ToArray();
            Packet decoded = receiver.Decode(sender.Encode(new Packet(0x2EE5, payload)));
            Check.Equal(i, decoded.Sequence);
            Check.Bytes(payload, decoded.Payload.Span);
            if (i == 254) Check.Equal(0, sender.SendCipher.Epoch);
            if (i == 255) Check.Equal(1, sender.SendCipher.Epoch);
        }
        Check.True(sender.SendCipher.Epoch > 1);
        Check.Equal(sender.SendCipher.Epoch, receiver.ReceiveCipher.Epoch);
        Check.Equal(sender.SendCipher.CipherName, receiver.ReceiveCipher.CipherName);
        Check.Equal(2000, receiver.ReceiveCipher.FramesProcessed);
        Check.Equal(0, receiver.SendCipher.FramesProcessed);
        (LabOptions() with { MaxPackets = ServerOptions.MaxPacketsLimit }).Validate();
        Check.Throws<ArgumentException>(() => (LabOptions() with { MaxPackets = ServerOptions.MaxPacketsLimit + 1 }).Validate());
    }
}
