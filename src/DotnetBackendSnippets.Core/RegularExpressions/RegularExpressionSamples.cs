using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.RegularExpressions;

/// <summary>
/// 正規表現の抽出、検証、置換、timeout のサンプルです。
/// </summary>
public static partial class RegularExpressionSamples
{
    /// <summary>
    /// 商品コード形式かどうかを検証します。
    /// </summary>
    /// <param name="value">検証する文字列。</param>
    /// <returns><c>ABC-1234</c> 形式の場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static bool IsProductCode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return ProductCodeRegex().IsMatch(value);
    }

    /// <summary>
    /// テキストから hashtag を抽出します。
    /// </summary>
    /// <param name="value">抽出対象文字列。</param>
    /// <returns><c>#</c> を除いた hashtag 一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<string> ExtractHashtags(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return HashtagRegex()
            .Matches(value)
            .Select(match => match.Groups["tag"].Value)
            .ToList();
    }

    /// <summary>
    /// 複数の空白を 1 つの空白へ置換します。
    /// </summary>
    /// <param name="value">置換対象文字列。</param>
    /// <returns>空白を正規化した文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string NormalizeWhitespace(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WhitespaceRegex().Replace(value.Trim(), " ");
    }

    /// <summary>
    /// ユーザー入力をリテラル検索用の regex pattern に変換します。
    /// </summary>
    /// <param name="keyword">検索キーワード。</param>
    /// <returns>大文字小文字を無視するリテラル検索 regex。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="keyword"/> が <see langword="null"/> の場合。</exception>
    public static Regex CreateLiteralSearchRegex(string keyword)
    {
        ArgumentNullException.ThrowIfNull(keyword);

        return new Regex(
            Regex.Escape(keyword),
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(200));
    }

    /// <summary>
    /// timeout 付きで regex match を試行します。
    /// </summary>
    /// <param name="pattern">正規表現 pattern。</param>
    /// <param name="value">照合対象文字列。</param>
    /// <param name="timeout">match timeout。</param>
    /// <returns>match した場合は <see langword="true"/>。timeout 時は <see langword="false"/>。</returns>
    /// <exception cref="ArgumentException"><paramref name="pattern"/> が空白の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static bool IsMatchWithTimeout(string pattern, string value, TimeSpan timeout)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        ArgumentNullException.ThrowIfNull(value);

        try
        {
            return Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant, timeout);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    [GeneratedRegex(@"^[A-Z]{3}-\d{4}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex ProductCodeRegex();

    [GeneratedRegex(@"(?<!\w)#(?<tag>[A-Za-z0-9_]+)", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex HashtagRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex WhitespaceRegex();
}
