using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    /// <summary>
    /// 文字列が null または空白かを判定します。
    /// </summary>
    public static bool IsBlank(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// メールアドレスらしい形式かを判定します。
    /// </summary>
    public static bool IsEmailLike(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().IsMatch(value);
    }

    /// <summary>
    /// 絶対 URL として有効かを判定します。
    /// </summary>
    public static bool IsAbsoluteUrl(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    /// <summary>
    /// HTTPS の絶対 URL かを判定します。
    /// </summary>
    public static bool IsHttpsUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
    }

    /// <summary>
    /// GUID として解析できるかを判定します。
    /// </summary>
    public static bool IsGuid(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Guid.TryParse(value, out _);
    }

    /// <summary>
    /// 数字だけで構成されているかを判定します。
    /// </summary>
    public static bool IsDigitsOnly(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length > 0 && value.All(char.IsDigit);
    }

    /// <summary>
    /// ASCII スラッグ形式かを判定します。
    /// </summary>
    public static bool IsAsciiSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return AsciiSlugRegex().IsMatch(value);
    }

    /// <summary>
    /// 許可された拡張子かを判定します。
    /// </summary>
    public static bool HasAllowedExtension(string fileName, ISet<string> allowedExtensions)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(allowedExtensions);

        return allowedExtensions.Contains(GetExtensionPortable(fileName));
    }

    /// <summary>
    /// 禁止語を含むかを判定します。
    /// </summary>
    public static bool ContainsBlockedWord(string value, ISet<string> blockedWords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(blockedWords);

        return blockedWords.Any(word => ContainsWholeWord(value, word));
    }

    /// <summary>
    /// 文字列が最大長以内かを検証します。
    /// </summary>
    public static void ValidateMaxLength(string value, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length must be zero or greater.");
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"Value must be {maxLength} characters or fewer.", nameof(value));
        }
    }
}
