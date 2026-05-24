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
    /// 大文字小文字を無視して部分一致を判定します。
    /// </summary>
    public static bool ContainsIgnoreCase(string value, string search)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(search);

        return value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 大文字小文字を無視して前方一致を判定します。
    /// </summary>
    public static bool StartsWithIgnoreCase(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 大文字小文字を無視して後方一致を判定します。
    /// </summary>
    public static bool EndsWithIgnoreCase(string value, string suffix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// OrdinalIgnoreCase で文字列の等価性を判定します。
    /// </summary>
    public static bool EqualsOrdinalIgnoreCase(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// すべてのキーワードを含むかを判定します。
    /// </summary>
    public static bool ContainsAllKeywords(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.All(keyword => ContainsIgnoreCase(value, keyword));
    }

    /// <summary>
    /// いずれかのキーワードを含むかを判定します。
    /// </summary>
    public static bool ContainsAnyKeyword(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.Any(keyword => ContainsIgnoreCase(value, keyword));
    }

    /// <summary>
    /// 単語単位で含まれているかを判定します。
    /// </summary>
    public static bool ContainsWholeWord(string value, string word)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        return Regex.IsMatch(value, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
