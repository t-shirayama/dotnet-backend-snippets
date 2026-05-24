namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// 金額の合計を checked で計算します。
    /// </summary>
    public static decimal SumAmounts(IEnumerable<decimal> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        decimal total = 0m;
        foreach (var value in values)
        {
            total = checked(total + value);
        }

        return total;
    }

    /// <summary>
    /// 平均値を計算し、空の場合は既定値を返します。
    /// </summary>
    public static decimal AverageOrDefault(IEnumerable<decimal> values, decimal defaultValue = 0m)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.ToArray();

        return items.Length == 0 ? defaultValue : SumAmounts(items) / items.Length;
    }

    /// <summary>
    /// 最小値と最大値を取得し、空の場合は null を返します。
    /// </summary>
    public static DecimalRange? MinMaxOrNull(IEnumerable<decimal> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.ToArray();
        if (items.Length == 0)
        {
            return null;
        }

        return new DecimalRange(items.Min(), items.Max());
    }

    /// <summary>
    /// カテゴリごとに金額を合計します。
    /// </summary>
    public static IReadOnlyDictionary<string, decimal> SumByCategory<T>(
        IEnumerable<T> values,
        Func<T, string> categorySelector,
        Func<T, decimal> amountSelector,
        IEqualityComparer<string>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(categorySelector);
        ArgumentNullException.ThrowIfNull(amountSelector);

        return values
            .GroupBy(categorySelector, comparer ?? StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => SumAmounts(group.Select(amountSelector)), comparer ?? StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 重み付き平均を計算します。
    /// </summary>
    public static decimal WeightedAverage(IEnumerable<(decimal Value, decimal Weight)> values, decimal defaultValue = 0m)
    {
        ArgumentNullException.ThrowIfNull(values);

        decimal weightedTotal = 0m;
        decimal totalWeight = 0m;

        foreach (var (value, weight) in values)
        {
            RequireNonNegative(weight, nameof(values));
            weightedTotal = checked(weightedTotal + value * weight);
            totalWeight = checked(totalWeight + weight);
        }

        return totalWeight == 0m ? defaultValue : weightedTotal / totalWeight;
    }

    /// <summary>
    /// 中央値を計算します。
    /// </summary>
    public static decimal Median(IEnumerable<decimal> values)
    {
        var items = SortRequiredValues(values);
        var middle = items.Length / 2;

        return items.Length % 2 == 1
            ? items[middle]
            : (items[middle - 1] + items[middle]) / 2m;
    }

    /// <summary>
    /// 指定パーセンタイルの値を計算します。
    /// </summary>
    public static decimal Percentile(IEnumerable<decimal> values, decimal percentile)
    {
        if (percentile is < 0m or > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 100.");
        }

        var items = SortRequiredValues(values);
        var position = (items.Length - 1) * percentile / 100m;
        var lowerIndex = (int)Math.Floor(position);
        var upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex)
        {
            return items[lowerIndex];
        }

        var fraction = position - lowerIndex;

        return items[lowerIndex] + (items[upperIndex] - items[lowerIndex]) * fraction;
    }

    /// <summary>
    /// 指定範囲外の値を除いて平均を計算します。
    /// </summary>
    public static decimal AverageWithoutOutliers(
        IEnumerable<decimal> values,
        decimal minimumInclusive,
        decimal maximumInclusive,
        decimal defaultValue = 0m)
    {
        if (minimumInclusive > maximumInclusive)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(minimumInclusive));
        }

        ArgumentNullException.ThrowIfNull(values);

        return AverageOrDefault(values.Where(value => value >= minimumInclusive && value <= maximumInclusive), defaultValue);
    }
}

