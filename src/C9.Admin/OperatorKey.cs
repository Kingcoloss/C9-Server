using System.Security.Cryptography;
using System.Text;

namespace C9.Admin;

internal sealed class OperatorKey
{
    private readonly byte[] expected;

    public OperatorKey(string stateDirectory)
    {
        string path = Path.Combine(stateDirectory, "access-key.local.txt");
        if (!File.Exists(path))
        {
            using FileStream stream = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            stream.Write(Encoding.ASCII.GetBytes(Convert.ToHexString(RandomNumberGenerator.GetBytes(32))));
        }
        string key = File.ReadAllText(path).Trim();
        if (key.Length != 64 || !key.All(Uri.IsHexDigit))
            throw new InvalidDataException("The console access key must contain 64 hexadecimal characters.");
        expected = SHA256.HashData(Encoding.ASCII.GetBytes(key));
    }

    public bool Matches(string? key) => key?.Trim() is { Length: 64 } candidate &&
        CryptographicOperations.FixedTimeEquals(expected, SHA256.HashData(Encoding.UTF8.GetBytes(candidate)));
}
