using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Security;

public static partial class SecuritySamples
{
    private const string PasswordHashAlgorithm = "PBKDF2-SHA256";
    private const int DefaultIterations = 100_000;
    private const int DefaultSaltSize = 16;
    private const int DefaultHashSize = 32;

    private static readonly string[] SensitiveKeyFragments =
    [
        "apikey",
        "clientsecret",
        "connectionstring",
        "password",
        "passwd",
        "privatekey",
        "pwd",
        "secret",
        "token",
    ];

    public static string HashPassword(
        string password,
        int iterations = DefaultIterations,
        int saltSize = DefaultSaltSize,
        int hashSize = DefaultHashSize)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (iterations < 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be 10,000 or greater.");
        }

        if (saltSize < 16)
        {
            throw new ArgumentOutOfRangeException(nameof(saltSize), "Salt size must be 16 bytes or greater.");
        }

        if (hashSize < 32)
        {
            throw new ArgumentOutOfRangeException(nameof(hashSize), "Hash size must be 32 bytes or greater.");
        }

        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            hashSize);

        return string.Join(
            '$',
            PasswordHashAlgorithm,
            iterations.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public static bool VerifyPassword(string password, string encodedHash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(encodedHash);

        if (!TryParsePasswordHash(encodedHash, out var iterations, out var salt, out var expectedHash))
        {
            return false;
        }

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    public static bool AreApiKeysEqual(string providedApiKey, string expectedApiKey)
    {
        ArgumentNullException.ThrowIfNull(providedApiKey);
        ArgumentNullException.ThrowIfNull(expectedApiKey);

        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(providedApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expectedApiKey));

        return CryptographicOperations.FixedTimeEquals(providedHash, expectedHash);
    }

    public static IReadOnlyList<SecretFinding> FindPotentialSecrets(
        IEnumerable<KeyValuePair<string, string?>> configurationValues)
    {
        ArgumentNullException.ThrowIfNull(configurationValues);

        var findings = new List<SecretFinding>();

        foreach (var (key, value) in configurationValues)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (LooksLikeSensitiveKey(key))
            {
                findings.Add(new SecretFinding(key, "設定キー名が secret、password、token などの機密値を示している可能性があります。"));
                continue;
            }

            if (LooksLikeSecretValue(value))
            {
                findings.Add(new SecretFinding(key, "設定値が長くランダムなトークンや秘密鍵に見えます。"));
            }
        }

        return findings;
    }

    public static string HtmlEncodeForDisplay(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.HtmlEncode(value);
    }

    public static string QuoteSqlIdentifier(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        if (!SqlIdentifierRegex().IsMatch(identifier))
        {
            throw new ArgumentException("SQL identifier must be a simple whitelisted name.", nameof(identifier));
        }

        return $"[{identifier}]";
    }

    private static bool TryParsePasswordHash(
        string encodedHash,
        out int iterations,
        out byte[] salt,
        out byte[] expectedHash)
    {
        iterations = 0;
        salt = [];
        expectedHash = [];

        var parts = encodedHash.Split('$');
        if (parts.Length != 4 || parts[0] != PasswordHashAlgorithm)
        {
            return false;
        }

        if (!int.TryParse(parts[1], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out iterations)
            || iterations < 10_000)
        {
            return false;
        }

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length >= 16 && expectedHash.Length >= 32;
    }

    private static bool LooksLikeSensitiveKey(string key)
    {
        var normalizedKey = NonAlphaNumericRegex().Replace(key, string.Empty).ToLowerInvariant();

        return SensitiveKeyFragments.Any(normalizedKey.Contains);
    }

    private static bool LooksLikeSecretValue(string value)
    {
        if (value.Contains("BEGIN PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (value.Length < 32 || value.Any(char.IsWhiteSpace))
        {
            return false;
        }

        return value.Any(char.IsLetter) && value.Any(char.IsDigit);
    }

    [GeneratedRegex("[^a-zA-Z0-9]+")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$")]
    private static partial Regex SqlIdentifierRegex();
}

public sealed record SecretFinding(string Key, string Reason);
