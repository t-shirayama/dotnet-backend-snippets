namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    /// <summary>
    /// 最初の区切り文字より前の部分を取得します。
    /// </summary>
    /// <param name="value">入力文字列。</param>
    /// <param name="separator">区切り文字。</param>
    /// <returns>区切り文字より前の部分。見つからない場合は元の文字列。</returns>
    public static string Before(string value, string separator)
    {
        return SplitAround(value, separator).Before;
    }

    /// <summary>
    /// 最初の区切り文字より後の部分を取得します。
    /// </summary>
    /// <param name="value">入力文字列。</param>
    /// <param name="separator">区切り文字。</param>
    /// <returns>区切り文字より後の部分。見つからない場合は空文字列。</returns>
    public static string After(string value, string separator)
    {
        return SplitAround(value, separator).After;
    }

    /// <summary>
    /// 最後の区切り文字より前の部分を取得します。
    /// </summary>
    /// <param name="value">入力文字列。</param>
    /// <param name="separator">区切り文字。</param>
    /// <returns>最後の区切り文字より前の部分。見つからない場合は元の文字列。</returns>
    public static string BeforeLast(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.LastIndexOf(separator, StringComparison.Ordinal);

        return index < 0 ? value : value[..index];
    }

    /// <summary>
    /// 最後の区切り文字より後の部分を取得します。
    /// </summary>
    /// <param name="value">入力文字列。</param>
    /// <param name="separator">区切り文字。</param>
    /// <returns>最後の区切り文字より後の部分。見つからない場合は空文字列。</returns>
    public static string AfterLast(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.LastIndexOf(separator, StringComparison.Ordinal);

        return index < 0 ? string.Empty : value[(index + separator.Length)..];
    }

    /// <summary>
    /// 文字列から数字だけを抽出します。
    /// </summary>
    /// <param name="value">入力文字列。</param>
    /// <returns>入力文字列に含まれる数字だけを連結した文字列。</returns>
    public static string ExtractDigits(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DigitsRegex().Replace(value, string.Empty);
    }

    /// <summary>
    /// ログ行から correlation id を抽出します。
    /// </summary>
    /// <param name="logLine">検索対象のログ行。</param>
    /// <returns>見つかった correlation id。見つからない場合は <see langword="null"/>。</returns>
    public static string? ExtractCorrelationId(string logLine)
    {
        ArgumentNullException.ThrowIfNull(logLine);

        var match = CorrelationIdRegex().Match(logLine);

        return match.Success ? match.Groups["value"].Value : null;
    }
}
