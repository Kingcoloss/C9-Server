using System.Security.Cryptography;
using System.Text;

namespace C9.Portal.Services;

/// <summary>
/// Default sign-in: a random 64-hex operator key in {state}/access-key.txt (created on first start)
/// signs in as <see cref="PortalRoles.Owner"/>. Replaceable by a DB-backed <see cref="IPortalUserStore"/>.
/// </summary>
public sealed class OperatorKeyUserStore : IPortalUserStore
{
    private readonly byte[] expected;

    public OperatorKeyUserStore(string stateDirectory)
    {
        KeyPath = Path.Combine(Directory.CreateDirectory(stateDirectory).FullName, "access-key.txt");
        if (!File.Exists(KeyPath))
        {
            try
            {
                using FileStream stream = new(KeyPath, C9.Server.AgentToken.OwnerOnly(FileMode.CreateNew));
                stream.Write(Encoding.ASCII.GetBytes(Convert.ToHexString(RandomNumberGenerator.GetBytes(32))));
            }
            catch (IOException) when (File.Exists(KeyPath)) { }
        }
        string key = File.ReadAllText(KeyPath).Trim();
        if (key.Length != 64 || !key.All(Uri.IsHexDigit))
            throw new InvalidDataException("The portal access key must contain 64 hexadecimal characters.");
        expected = SHA256.HashData(Encoding.ASCII.GetBytes(key.ToUpperInvariant()));
    }

    public string KeyPath { get; }

    public ValueTask<PortalUser?> ValidateAsync(PortalCredentials credentials, CancellationToken cancellationToken = default)
    {
        string candidate = credentials.Secret.Trim();
        bool valid = candidate.Length == 64 && CryptographicOperations.FixedTimeEquals(expected,
            SHA256.HashData(Encoding.ASCII.GetBytes(candidate.ToUpperInvariant())));
        return ValueTask.FromResult(valid ? new PortalUser("operator", "Local operator", [PortalRoles.Owner]) : null);
    }
}
