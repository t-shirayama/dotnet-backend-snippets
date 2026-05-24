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
    /// カンマ区切りの値をトリムして分割します。
    /// </summary>
    public static IReadOnlyList<string> SplitCsvLikeValues(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// キーと値の行を辞書へ変換します。
    /// </summary>
    public static IReadOnlyDictionary<string, string> ParseKeyValueLines(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.SplitLines(value, removeEmptyLines: true)
            .Select(line => line.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 値を CSV 行として結合します。
    /// </summary>
    public static string JoinCsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join(",", values.Select(EscapeCsvField));
    }

    /// <summary>
    /// 値を TSV 行として結合します。
    /// </summary>
    public static string JoinTsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join('\t', values.Select(value => value.Replace("\t", " ", StringComparison.Ordinal)));
    }

    /// <summary>
    /// 各行の先頭に空白インデントを付けます。
    /// </summary>
    public static string IndentLines(string value, int spaces)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (spaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(spaces), "Spaces must be zero or greater.");
        }

        var prefix = new string(' ', spaces);

        return string.Join('\n', StringSamples.SplitLines(value).Select(line => prefix + line));
    }

    /// <summary>
    /// 各行の先頭に指定プレフィックスを付けます。
    /// </summary>
    public static string PrefixLines(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return string.Join('\n', StringSamples.SplitLines(value).Select(line => prefix + line));
    }

    /// <summary>
    /// 必要な場合だけ左側にパディングします。
    /// </summary>
    public static string PadLeftSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadLeft(totalWidth);
    }

    /// <summary>
    /// 必要な場合だけ右側にパディングします。
    /// </summary>
    public static string PadRightSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadRight(totalWidth);
    }
}
