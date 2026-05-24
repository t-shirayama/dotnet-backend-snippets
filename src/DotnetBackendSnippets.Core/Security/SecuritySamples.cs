using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Security;

/// <summary>
/// セキュリティ関連の基本的な実装例を提供します。
/// </summary>
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

    /// <summary>
    /// PBKDF2-SHA256 でパスワードをハッシュ化します。
    /// </summary>
    /// <param name="password">ハッシュ化するパスワード。</param>
    /// <param name="iterations">PBKDF2 の反復回数。</param>
    /// <param name="saltSize">生成するソルトのバイト数。</param>
    /// <param name="hashSize">生成するハッシュのバイト数。</param>
    /// <returns>アルゴリズム、反復回数、ソルト、ハッシュを含む文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="password"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException">反復回数、ソルトサイズ、またはハッシュサイズが小さすぎる場合。</exception>
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

    /// <summary>
    /// パスワードが保存済みハッシュと一致するか検証します。
    /// </summary>
    /// <param name="password">検証するパスワード。</param>
    /// <param name="encodedHash">保存済みハッシュ文字列。</param>
    /// <returns>一致する場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="password"/> または <paramref name="encodedHash"/> が <see langword="null"/> の場合。</exception>
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

    /// <summary>
    /// API キーを固定時間比較で照合します。
    /// </summary>
    /// <param name="providedApiKey">利用者が提示した API キー。</param>
    /// <param name="expectedApiKey">期待する API キー。</param>
    /// <returns>一致する場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="providedApiKey"/> または <paramref name="expectedApiKey"/> が <see langword="null"/> の場合。</exception>
    public static bool AreApiKeysEqual(string providedApiKey, string expectedApiKey)
    {
        ArgumentNullException.ThrowIfNull(providedApiKey);
        ArgumentNullException.ThrowIfNull(expectedApiKey);

        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(providedApiKey));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expectedApiKey));

        return CryptographicOperations.FixedTimeEquals(providedHash, expectedHash);
    }

    /// <summary>
    /// 設定値から秘密情報らしい項目を検出します。
    /// </summary>
    /// <param name="configurationValues">設定キーと値の一覧。</param>
    /// <returns>秘密情報の可能性がある項目一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configurationValues"/> が <see langword="null"/> の場合。</exception>
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

    /// <summary>
    /// 表示用に HTML エンコードします。
    /// </summary>
    /// <param name="value">エンコードする文字列。</param>
    /// <returns>HTML エンコード後の文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string HtmlEncodeForDisplay(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.HtmlEncode(value);
    }

    /// <summary>
    /// SQL Server 形式で識別子を安全に引用します。
    /// </summary>
    /// <param name="identifier">引用する識別子。</param>
    /// <returns>角括弧で引用した識別子。</returns>
    /// <exception cref="ArgumentException"><paramref name="identifier"/> が空、または許可されない形式の場合。</exception>
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

/// <summary>
/// 秘密情報の可能性がある設定項目を表します。
/// </summary>
/// <param name="Key">検出された設定キー。</param>
/// <param name="Reason">検出理由。</param>
public sealed record SecretFinding(string Key, string Reason);
