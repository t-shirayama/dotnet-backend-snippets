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
    /// 最初の区切り文字より前の部分を取得します。
    /// </summary>
    public static string Before(string value, string separator)
    {
        return SplitAround(value, separator).Before;
    }

    /// <summary>
    /// 最初の区切り文字より後の部分を取得します。
    /// </summary>
    public static string After(string value, string separator)
    {
        return SplitAround(value, separator).After;
    }

    /// <summary>
    /// 最後の区切り文字より前の部分を取得します。
    /// </summary>
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
    public static string ExtractDigits(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DigitsRegex().Replace(value, string.Empty);
    }

    /// <summary>
    /// ログ行から correlation id を抽出します。
    /// </summary>
    public static string? ExtractCorrelationId(string logLine)
    {
        ArgumentNullException.ThrowIfNull(logLine);

        var match = CorrelationIdRegex().Match(logLine);

        return match.Success ? match.Groups["value"].Value : null;
    }
}
