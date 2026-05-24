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
    /// <param name="value">検索対象の文字列。</param>
    /// <param name="search">検索する文字列。</param>
    /// <returns><paramref name="value"/> が <paramref name="search"/> を含む場合は <see langword="true"/>。</returns>
    public static bool ContainsIgnoreCase(string value, string search)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(search);

        return value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 大文字小文字を無視して前方一致を判定します。
    /// </summary>
    /// <param name="value">判定対象の文字列。</param>
    /// <param name="prefix">先頭にあるかを確認する文字列。</param>
    /// <returns><paramref name="value"/> が <paramref name="prefix"/> で始まる場合は <see langword="true"/>。</returns>
    public static bool StartsWithIgnoreCase(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 大文字小文字を無視して後方一致を判定します。
    /// </summary>
    /// <param name="value">判定対象の文字列。</param>
    /// <param name="suffix">末尾にあるかを確認する文字列。</param>
    /// <returns><paramref name="value"/> が <paramref name="suffix"/> で終わる場合は <see langword="true"/>。</returns>
    public static bool EndsWithIgnoreCase(string value, string suffix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// OrdinalIgnoreCase で文字列の等価性を判定します。
    /// </summary>
    /// <param name="left">比較する左辺の文字列。</param>
    /// <param name="right">比較する右辺の文字列。</param>
    /// <returns>2 つの文字列が大文字小文字を無視して等しい場合は <see langword="true"/>。</returns>
    public static bool EqualsOrdinalIgnoreCase(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// すべてのキーワードを含むかを判定します。キーワードが空の場合は <see langword="true"/> を返します。
    /// </summary>
    /// <param name="value">検索対象の文字列。</param>
    /// <param name="keywords">すべて含まれているかを確認するキーワード。</param>
    /// <returns>すべてのキーワードが含まれる場合は <see langword="true"/>。キーワードが空の場合も <see langword="true"/>。</returns>
    public static bool ContainsAllKeywords(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.All(keyword => ContainsIgnoreCase(value, keyword));
    }

    /// <summary>
    /// いずれかのキーワードを含むかを判定します。
    /// </summary>
    /// <param name="value">検索対象の文字列。</param>
    /// <param name="keywords">いずれかが含まれているかを確認するキーワード。</param>
    /// <returns>いずれかのキーワードが含まれる場合は <see langword="true"/>。</returns>
    public static bool ContainsAnyKeyword(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.Any(keyword => ContainsIgnoreCase(value, keyword));
    }

    /// <summary>
    /// 英数字ベースの単語境界で含まれているかを判定します。日本語など空白で区切られない言語の検索には向きません。
    /// </summary>
    /// <param name="value">検索対象の文字列。</param>
    /// <param name="word">単語として検索する文字列。</param>
    /// <returns><paramref name="word"/> が英数字ベースの単語として含まれる場合は <see langword="true"/>。</returns>
    public static bool ContainsWholeWord(string value, string word)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        return Regex.IsMatch(value, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
