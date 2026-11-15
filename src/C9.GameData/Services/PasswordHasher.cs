using System.Security.Cryptography;
using System.Text;

namespace C9.GameData.Services;

/// <summary>PBKDF2-SHA256 with per-password salt, stored as "pbkdf2-sha256$iterations$salt$hash" (base64).</summary>
public static class PasswordHasher
{
    public const string Disabled = "!";
    private const int Iterations = 210_000; // OWASP 2023 guidance for PBKDF2-HMAC-SHA256.
    private const int SaltBytes = 16;
    private const int HashBytes = 32;

    public static string Hash(string password)
    {
        ValidatePolicy(password);
        byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256, HashBytes);
        return $"pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored)
    {
        string[] parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out int iterations) ||
            iterations is < 10_000 or > 10_000_000)
            return false;
        byte[] salt, expected;
        try { salt = Convert.FromBase64String(parts[2]); expected = Convert.FromBase64String(parts[3]); }
        catch (FormatException) { return false; }
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    public static void ValidatePolicy(string password)
    {
        if (password is null || password.Length is < 8 or > 128)
            throw new GameDataException(GameDataError.Invalid, "Password must be 8–128 characters");
    }
}
