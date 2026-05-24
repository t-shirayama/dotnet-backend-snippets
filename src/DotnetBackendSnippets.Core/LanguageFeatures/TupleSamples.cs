namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// 名前付きタプルとレコードへの変換例を提供します。
/// </summary>
public static class TupleSamples
{
    /// <summary>
    /// 氏名文字列を名と姓に分割します。
    /// </summary>
    /// <param name="value">分割する氏名。</param>
    /// <returns>分割結果と成功可否を含むタプル。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が null です。</exception>
    public static (bool Success, string FirstName, string LastName) TrySplitFullName(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return parts.Length switch
        {
            >= 2 => (true, parts[0], parts[^1]),
            _ => (false, string.Empty, string.Empty),
        };
    }

    /// <summary>
    /// 整数列の最小値、最大値、平均を計算します。
    /// </summary>
    /// <param name="values">集計対象の整数列。</param>
    /// <returns>最小値、最大値、平均のタプル。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> が null です。</exception>
    /// <exception cref="ArgumentException"><paramref name="values"/> が空です。</exception>
    public static (int Min, int Max, decimal Average) CalculateSummary(IEnumerable<int> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.ToList();
        if (items.Count == 0)
        {
            throw new ArgumentException("Values must contain at least one item.", nameof(values));
        }

        return (items.Min(), items.Max(), (decimal)items.Average());
    }

    /// <summary>
    /// タプルの範囲をレコードへ変換します。
    /// </summary>
    /// <param name="range">最小値と最大値のタプル。</param>
    /// <returns>数値範囲レコード。</returns>
    /// <exception cref="ArgumentException">最小値が最大値を超えています。</exception>
    public static NumericRange ToRangeRecord((int Min, int Max) range)
    {
        if (range.Min > range.Max)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(range));
        }

        return new NumericRange(range.Min, range.Max);
    }
}

/// <summary>
/// 最小値と最大値を表す数値範囲です。
/// </summary>
/// <param name="Min">最小値。</param>
/// <param name="Max">最大値。</param>
public sealed record NumericRange(int Min, int Max);
