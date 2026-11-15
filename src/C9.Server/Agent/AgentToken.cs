using System.Security.Cryptography;
using System.Text;

namespace C9.Server;

/// <summary>
/// Shared secret between game-service agents and the Admin Portal, stored in a file on the shared
/// state volume. The first process to start creates it; the others wait for it and read it.
/// </summary>
public sealed class AgentToken
{
    private readonly byte[] expected;

    private AgentToken(string value)
    {
        Value = value;
        expected = SHA256.HashData(Encoding.ASCII.GetBytes(value));
    }

    public string Value { get; }

    public static AgentToken LoadOrCreate(string path, TimeSpan? wait = null)
    {
        string full = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        DateTimeOffset deadline = DateTimeOffset.UtcNow + (wait ?? TimeSpan.FromSeconds(10));
        while (true)
        {
            if (!File.Exists(full))
            {
                try
                {
                    using FileStream created = new(full, OwnerOnly(FileMode.CreateNew));
                    created.Write(Encoding.ASCII.GetBytes(Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32))));
                }
                catch (IOException) when (File.Exists(full)) { } // Another service created it first.
            }
            string text;
            try { text = File.ReadAllText(full).Trim(); }
            catch (IOException) { text = ""; }
            if (text.Length == 64 && text.All(Uri.IsHexDigit)) return new AgentToken(text.ToLowerInvariant());
            if (text.Length > 0 || DateTimeOffset.UtcNow > deadline)
                throw new InvalidDataException("The agent token file must contain 64 hexadecimal characters");
            Thread.Sleep(100);
        }
    }

    public bool Matches(string? authorization)
    {
        const string scheme = "Bearer ";
        if (authorization is null || !authorization.StartsWith(scheme, StringComparison.Ordinal)) return false;
        string candidate = authorization[scheme.Length..].Trim();
        return candidate.Length == 64 && CryptographicOperations.FixedTimeEquals(expected,
            SHA256.HashData(Encoding.ASCII.GetBytes(candidate.ToLowerInvariant())));
    }

    /// <summary>Create options that give the file owner-only permissions (0600) on Linux/macOS.</summary>
    public static FileStreamOptions OwnerOnly(FileMode mode)
    {
        FileStreamOptions options = new() { Mode = mode, Access = FileAccess.Write, Share = FileShare.None };
        if (!OperatingSystem.IsWindows()) options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
        return options;
    }
}
