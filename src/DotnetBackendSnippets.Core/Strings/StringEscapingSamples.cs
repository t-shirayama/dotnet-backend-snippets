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
    /// URL パスセグメントとして安全にエンコードします。
    /// </summary>
    public static string EncodePathSegment(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Uri.EscapeDataString(value);
    }

    /// <summary>
    /// URL クエリ値として安全にエンコードします。
    /// </summary>
    public static string EncodeQueryValue(string value)
    {
        return UrlEncode(value);
    }

    /// <summary>
    /// 正規表現パターンとして安全にエスケープします。
    /// </summary>
    public static string EscapeRegexPattern(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Regex.Escape(value);
    }

    /// <summary>
    /// HTML として安全にエンコードします。
    /// </summary>
    public static string HtmlEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.HtmlEncode(value);
    }

    /// <summary>
    /// JavaScript 文字列として安全にエンコードします。
    /// </summary>
    public static string JavaScriptStringEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return JavaScriptEncoder.Default.Encode(value);
    }

    /// <summary>
    /// URL 用に文字列をエンコードします。
    /// </summary>
    public static string UrlEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.UrlEncode(value);
    }

    /// <summary>
    /// URL エンコードされた文字列をデコードします。
    /// </summary>
    public static string UrlDecode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.UrlDecode(value);
    }

    /// <summary>
    /// 正規表現用のエスケープ処理を行います。
    /// </summary>
    public static string RegexEscape(string value)
    {
        return EscapeRegexPattern(value);
    }

    /// <summary>
    /// SQL LIKE パターン用にワイルドカードをエスケープします。
    /// </summary>
    public static string EscapeSqlLikePattern(string value, char escapeCharacter = '\\')
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace(escapeCharacter.ToString(), new string(escapeCharacter, 2), StringComparison.Ordinal)
            .Replace("%", $"{escapeCharacter}%", StringComparison.Ordinal)
            .Replace("_", $"{escapeCharacter}_", StringComparison.Ordinal)
            .Replace("[", $"{escapeCharacter}[", StringComparison.Ordinal);
    }

    /// <summary>
    /// 制御文字をログなどで見える形にエスケープします。
    /// </summary>
    public static string EscapeControlCharacters(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);
    }

    /// <summary>
    /// CSV フィールドとして必要な場合にクォートします。
    /// </summary>
    public static string EscapeCsvField(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.IndexOfAny([',', '"', '\r', '\n']) < 0)
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    /// <summary>
    /// 単一行ログ向けに制御文字を無害化します。
    /// </summary>
    public static string SanitizeForSingleLineLog(string value)
    {
        return EscapeControlCharacters(value);
    }

    /// <summary>
    /// 個人情報らしい文字列を伏せ字にします。
    /// </summary>
    public static string RedactPersonalData(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().Replace(value, "***@***");
    }
}
