namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    /// <summary>
    /// カンマ区切りの値をトリムして分割します。
    /// </summary>
    /// <param name="value">カンマ区切りの入力文字列。</param>
    /// <returns>空要素を除外し、各要素をトリムした文字列リスト。</returns>
    public static IReadOnlyList<string> SplitCsvLikeValues(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// キーと値の行を辞書へ変換します。
    /// </summary>
    /// <param name="value">キーと値を <c>=</c> で区切った行を含む文字列。</param>
    /// <returns>キーと値の行から作成した、大文字小文字を区別しない辞書。</returns>
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
    /// <param name="values">CSV フィールドとして結合する値。</param>
    /// <returns>必要なフィールドをエスケープして結合した CSV 行。</returns>
    public static string JoinCsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join(",", values.Select(EscapeCsvField));
    }

    /// <summary>
    /// 値を TSV 行として結合します。
    /// </summary>
    /// <param name="values">TSV フィールドとして結合する値。</param>
    /// <returns>タブを空白に置き換えて結合した TSV 行。</returns>
    public static string JoinTsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join('\t', values.Select(value => value.Replace("\t", " ", StringComparison.Ordinal)));
    }

    /// <summary>
    /// 各行の先頭に空白インデントを付けます。
    /// </summary>
    /// <param name="value">インデントする複数行文字列。</param>
    /// <param name="spaces">各行の先頭に付ける空白数。</param>
    /// <returns>各行に指定した数の空白を付けた文字列。</returns>
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
    /// <param name="value">プレフィックスを付ける複数行文字列。</param>
    /// <param name="prefix">各行の先頭に付ける文字列。</param>
    /// <returns>各行に <paramref name="prefix"/> を付けた文字列。</returns>
    public static string PrefixLines(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return string.Join('\n', StringSamples.SplitLines(value).Select(line => prefix + line));
    }

    /// <summary>
    /// 必要な場合だけ左側にパディングします。
    /// </summary>
    /// <param name="value">パディングする文字列。</param>
    /// <param name="totalWidth">必要な全体幅。</param>
    /// <returns>必要な場合だけ左側に空白を追加した文字列。</returns>
    public static string PadLeftSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadLeft(totalWidth);
    }

    /// <summary>
    /// 必要な場合だけ右側にパディングします。
    /// </summary>
    /// <param name="value">パディングする文字列。</param>
    /// <param name="totalWidth">必要な全体幅。</param>
    /// <returns>必要な場合だけ右側に空白を追加した文字列。</returns>
    public static string PadRightSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadRight(totalWidth);
    }
}
