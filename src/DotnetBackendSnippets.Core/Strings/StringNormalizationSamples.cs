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
    /// 文字列をトリムし、null の場合は空文字列を返します。
    /// </summary>
    public static string TrimOrEmpty(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 通常の空白と全角空白を取り除きます。
    /// </summary>
    public static string TrimJapaneseWhitespace(string? value)
    {
        return value?.Trim().Trim('\u3000') ?? string.Empty;
    }

    /// <summary>
    /// 文字列を単一行に正規化します。
    /// </summary>
    public static string NormalizeToSingleLine(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.NormalizeWhitespace(value);
    }

    /// <summary>
    /// 改行コードを指定した改行文字へ統一します。
    /// </summary>
    public static string NormalizeLineEndings(string value, string newline = "\n")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(newline);

        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    /// <summary>
    /// 文字列を指定した Unicode 正規化形式へ変換します。
    /// </summary>
    public static string NormalizeUnicode(string value, NormalizationForm normalizationForm = NormalizationForm.FormC)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Normalize(normalizationForm);
    }

    /// <summary>
    /// 検索用にダイアクリティカルマークを除去します。
    /// </summary>
    public static string RemoveDiacriticsForSearch(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// 連続する区切り文字を 1 つにまとめます。
    /// </summary>
    public static string CollapseSeparators(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SeparatorRegex().Replace(value, "$1").Trim('-', '_');
    }

    /// <summary>
    /// 空白文字列を null に変換します。
    /// </summary>
    public static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>
    /// 検索用にかな文字を正規化します。
    /// </summary>
    public static string NormalizeKanaForSearch(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Normalize(NormalizationForm.FormKC);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            builder.Append(character is >= '\u30A1' and <= '\u30F6'
                ? (char)(character - 0x60)
                : character);
        }

        return builder.ToString();
    }

    /// <summary>
    /// ファイル名として使いやすい文字列へ変換します。
    /// </summary>
    public static string ToSafeFileName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var invalid = new HashSet<char>(['<', '>', ':', '"', '/', '\\', '|', '?', '*']);
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            builder.Append(invalid.Contains(character) ? '-' : character);
        }

        return CollapseSeparators(builder.ToString().Trim());
    }
}
