using System.Data.Common;

namespace C9.GameData;

/// <summary>
/// Resolves the game database connection string the same way in every process (Portal, game services):
/// <c>C9_DB_CONNECTION</c> without a password plus the password read from <c>C9_DB_PASSWORD_FILE</c>,
/// so the secret never sits in an environment variable, a config file or a log line.
/// </summary>
public static class GameDataConnection
{
    public const string PasswordFileVariable = "C9_DB_PASSWORD_FILE";

    /// <summary>Null when <paramref name="connection"/> is empty. A password already in the string wins over the file.</summary>
    public static string? Resolve(string? connection, string? passwordFile)
    {
        if (string.IsNullOrWhiteSpace(connection)) return null;
        DbConnectionStringBuilder parts = new() { ConnectionString = connection };
        if (!parts.ContainsKey("Password") && !string.IsNullOrWhiteSpace(passwordFile) && File.Exists(passwordFile))
            parts["Password"] = File.ReadAllText(passwordFile).Trim();
        return parts.ConnectionString;
    }

    /// <summary>The raw <c>C9_DB_CONNECTION</c> value (no password resolution), or null when unset.</summary>
    public static string? FromEnvironment() =>
        Environment.GetEnvironmentVariable(GameDataServiceCollectionExtensions.ConnectionVariable) is { Length: > 0 } value ? value : null;

    /// <summary>Resolves a raw connection string with the password file named by <c>C9_DB_PASSWORD_FILE</c>.</summary>
    public static string? ResolveWithEnvironmentPassword(string? connection) =>
        Resolve(connection, Environment.GetEnvironmentVariable(PasswordFileVariable));
}
