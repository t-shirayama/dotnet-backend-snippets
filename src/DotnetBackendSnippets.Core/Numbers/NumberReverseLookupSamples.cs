using System.Globalization;

namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを提供します。
/// </summary>
public static class NumberReverseLookupSamples
{
    /// <summary>
    /// decimal の最小値と最大値を表します。
    /// </summary>
    /// <param name="Min">最小値。</param>
    /// <param name="Max">最大値。</param>
    public readonly record struct DecimalRange(decimal Min, decimal Max);

    /// <summary>
    /// 一覧表示上の開始位置と終了位置を表します。
    /// </summary>
    /// <param name="Start">表示開始位置。</param>
    /// <param name="End">表示終了位置。</param>
    public readonly record struct DisplayRange(int Start, int End);

    /// <summary>
    /// 手数料の内訳を表します。
    /// </summary>
    /// <param name="PercentageFee">割合で計算した手数料。</param>
    /// <param name="FixedFee">固定手数料。</param>
    /// <param name="AppliedFee">実際に適用した手数料。</param>
    public readonly record struct FeeBreakdown(decimal PercentageFee, decimal FixedFee, decimal AppliedFee);

    /// <summary>
    /// ページング用の offset と limit を表します。
    /// </summary>
    /// <param name="Offset">読み飛ばす件数。</param>
    /// <param name="Limit">取得する件数。</param>
    public readonly record struct OffsetLimit(int Offset, int Limit);

    /// <summary>
    /// 税抜、税額、税込の内訳を表します。
    /// </summary>
    /// <param name="NetAmount">税抜金額。</param>
    /// <param name="TaxAmount">税額。</param>
    /// <param name="GrossAmount">税込金額。</param>
    public readonly record struct TaxBreakdown(decimal NetAmount, decimal TaxAmount, decimal GrossAmount);

    /// <summary>
    /// 整数として解析し、失敗時は既定値を返します。
    /// </summary>
    public static int ParseIntOrDefault(string? value, int defaultValue = 0)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// InvariantCulture で decimal を解析します。
    /// </summary>
    public static bool TryParseDecimalInvariant(string? value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// null の decimal を既定値へ置き換えます。
    /// </summary>
    public static decimal DefaultIfNull(decimal? value, decimal defaultValue = 0m)
    {
        return value ?? defaultValue;
    }

    /// <summary>
    /// 整数が 1 以上であることを検証します。
    /// </summary>
    public static int RequirePositiveInt(int value, string parameterName = "value")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be one or greater.");
        }

        return value;
    }

    /// <summary>
    /// decimal が 0 以上であることを検証します。
    /// </summary>
    public static decimal RequireNonNegative(decimal value, string parameterName = "value")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (value < 0m)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be zero or greater.");
        }

        return value;
    }

    /// <summary>
    /// 割合が 0 から 1 の範囲であることを検証します。
    /// </summary>
    public static decimal RequireFractionRate(decimal rate, string parameterName = "rate")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (rate is < 0m or > 1m)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Rate must be between 0 and 1.");
        }

        return rate;
    }

    /// <summary>
    /// 中間値を 0 から遠い方向へ丸めます。
    /// </summary>
    public static decimal RoundAwayFromZero(decimal value, int decimalPlaces)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 中間値を最近接偶数へ丸めます。
    /// </summary>
    public static decimal RoundBankers(decimal value, int decimalPlaces)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return Math.Round(value, decimalPlaces, MidpointRounding.ToEven);
    }

    /// <summary>
    /// 指定単位へ丸めます。
    /// </summary>
    public static decimal RoundToUnit(
        decimal value,
        decimal unit,
        MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
    {
        if (unit <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unit), "Unit must be greater than zero.");
        }

        return Math.Round(value / unit, 0, midpointRounding) * unit;
    }

    /// <summary>
    /// 指定単位へ切り上げます。
    /// </summary>
    public static decimal CeilingToUnit(decimal value, decimal unit)
    {
        if (unit <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unit), "Unit must be greater than zero.");
        }

        return Math.Ceiling(value / unit) * unit;
    }

    /// <summary>
    /// 指定単位へ切り下げます。
    /// </summary>
    public static decimal FloorToUnit(decimal value, decimal unit)
    {
        if (unit <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unit), "Unit must be greater than zero.");
        }

        return Math.Floor(value / unit) * unit;
    }

    /// <summary>
    /// 前回値からの変化率を計算します。
    /// </summary>
    public static decimal CalculateChangeRate(
        decimal current,
        decimal previous,
        int decimalPlaces = 2,
        decimal defaultWhenPreviousIsZero = 0m)
    {
        ValidateDecimalPlaces(decimalPlaces);

        if (previous == 0m)
        {
            return defaultWhenPreviousIsZero;
        }

        return RoundAwayFromZero((current - previous) / previous * 100m, decimalPlaces);
    }

    /// <summary>
    /// 全体に対する比率を計算します。
    /// </summary>
    public static decimal CalculateRatio(decimal part, decimal whole, int decimalPlaces = 4)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return whole == 0m ? 0m : RoundAwayFromZero(part / whole, decimalPlaces);
    }

    /// <summary>
    /// 構成比率の合計が 100 になるよう計算します。
    /// </summary>
    public static IReadOnlyList<decimal> CalculateCompositionPercentages(IEnumerable<decimal> values, int decimalPlaces = 2)
    {
        ArgumentNullException.ThrowIfNull(values);
        ValidateDecimalPlaces(decimalPlaces);

        var items = values.ToArray();
        if (items.Length == 0)
        {
            return Array.Empty<decimal>();
        }

        if (items.Any(value => value < 0m))
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Values must be zero or greater.");
        }

        var total = items.Sum();
        if (total == 0m)
        {
            return items.Select(_ => 0m).ToArray();
        }

        var rounded = items
            .Select(value => RoundAwayFromZero(value / total * 100m, decimalPlaces))
            .ToArray();
        var difference = 100m - rounded.Sum();

        if (difference != 0m)
        {
            var largestIndex = Array.IndexOf(items, items.Max());
            rounded[largestIndex] += difference;
        }

        return rounded;
    }

    /// <summary>
    /// 割引率を適用した金額を計算します。
    /// </summary>
    public static decimal ApplyDiscountRate(decimal amount, decimal discountRate, int decimalPlaces = 2)
    {
        RequireNonNegative(amount, nameof(amount));
        RequireFractionRate(discountRate, nameof(discountRate));

        return RoundAwayFromZero(amount * (1m - discountRate), decimalPlaces);
    }

    /// <summary>
    /// 利益率を計算します。
    /// </summary>
    public static decimal CalculateProfitMargin(decimal revenue, decimal cost, int decimalPlaces = 2)
    {
        ValidateDecimalPlaces(decimalPlaces);

        if (revenue == 0m)
        {
            return 0m;
        }

        return RoundAwayFromZero((revenue - cost) / revenue * 100m, decimalPlaces);
    }

    /// <summary>
    /// 税抜金額から税額と税込金額を計算します。
    /// </summary>
    public static TaxBreakdown CalculateTaxFromNet(decimal netAmount, decimal taxRate, int decimalPlaces = 2)
    {
        RequireNonNegative(netAmount, nameof(netAmount));
        RequireNonNegative(taxRate, nameof(taxRate));

        var taxAmount = RoundAwayFromZero(netAmount * taxRate, decimalPlaces);
        var grossAmount = RoundAwayFromZero(netAmount + taxAmount, decimalPlaces);

        return new TaxBreakdown(RoundAwayFromZero(netAmount, decimalPlaces), taxAmount, grossAmount);
    }

    /// <summary>
    /// 税込金額から税抜金額と税額を計算します。
    /// </summary>
    public static TaxBreakdown CalculateTaxFromGross(decimal grossAmount, decimal taxRate, int decimalPlaces = 2)
    {
        RequireNonNegative(grossAmount, nameof(grossAmount));
        RequireNonNegative(taxRate, nameof(taxRate));

        var netAmount = RoundAwayFromZero(grossAmount / (1m + taxRate), decimalPlaces);
        var taxAmount = RoundAwayFromZero(grossAmount - netAmount, decimalPlaces);

        return new TaxBreakdown(netAmount, taxAmount, RoundAwayFromZero(grossAmount, decimalPlaces));
    }

    /// <summary>
    /// 小計と送料から合計金額を計算します。
    /// </summary>
    public static decimal CalculateTotalWithShipping(decimal subtotal, decimal shipping, int decimalPlaces = 2)
    {
        RequireNonNegative(subtotal, nameof(subtotal));
        RequireNonNegative(shipping, nameof(shipping));

        return RoundAwayFromZero(subtotal + shipping, decimalPlaces);
    }

    /// <summary>
    /// 割合手数料、固定手数料、最低手数料を考慮して手数料を計算します。
    /// </summary>
    public static FeeBreakdown CalculateFee(
        decimal amount,
        decimal rate,
        decimal fixedFee = 0m,
        decimal minimumFee = 0m,
        int decimalPlaces = 2)
    {
        RequireNonNegative(amount, nameof(amount));
        RequireNonNegative(rate, nameof(rate));
        RequireNonNegative(fixedFee, nameof(fixedFee));
        RequireNonNegative(minimumFee, nameof(minimumFee));

        var percentageFee = RoundAwayFromZero(amount * rate, decimalPlaces);
        var calculatedFee = RoundAwayFromZero(percentageFee + fixedFee, decimalPlaces);
        var appliedFee = Math.Max(calculatedFee, minimumFee);

        return new FeeBreakdown(percentageFee, RoundAwayFromZero(fixedFee, decimalPlaces), RoundAwayFromZero(appliedFee, decimalPlaces));
    }

    /// <summary>
    /// 桁区切り付きの数値文字列に整形します。
    /// </summary>
    public static string FormatThousands(decimal value, int decimalPlaces = 0, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 固定小数点の数値文字列に整形します。
    /// </summary>
    public static string FormatFixedDecimal(decimal value, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return value.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 通貨コード付きの金額文字列に整形します。
    /// </summary>
    public static string FormatCurrencyCode(
        decimal value,
        string currencyCode,
        int decimalPlaces = 2,
        IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        ValidateDecimalPlaces(decimalPlaces);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{currencyCode.ToUpperInvariant()} {value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)}");
    }

    /// <summary>
    /// 通貨金額を最小通貨単位へ変換します。
    /// </summary>
    public static long ToMinorCurrencyUnits(decimal amount, int fractionDigits = 2)
    {
        ValidateDecimalPlaces(fractionDigits);

        var multiplier = PowerOfTen(fractionDigits);

        return checked((long)RoundAwayFromZero(amount * multiplier, 0));
    }

    /// <summary>
    /// 最小通貨単位から通貨金額へ変換します。
    /// </summary>
    public static decimal FromMinorCurrencyUnits(long minorUnits, int fractionDigits = 2)
    {
        ValidateDecimalPlaces(fractionDigits);

        return minorUnits / PowerOfTen(fractionDigits);
    }

    /// <summary>
    /// 為替レートを使って金額を換算します。
    /// </summary>
    public static decimal ConvertCurrency(decimal amount, decimal exchangeRate, int decimalPlaces = 2)
    {
        RequireNonNegative(exchangeRate, nameof(exchangeRate));

        return RoundAwayFromZero(amount * exchangeRate, decimalPlaces);
    }

    /// <summary>
    /// 金額が返金を表す負数かを判定します。
    /// </summary>
    public static bool IsRefund(decimal amount)
    {
        return amount < 0m;
    }

    /// <summary>
    /// 金額が上限以下であることを検証します。
    /// </summary>
    public static decimal EnsureMaximumAmount(decimal amount, decimal maximumAmount, string parameterName = "amount")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (amount > maximumAmount)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Amount exceeds the allowed maximum.");
        }

        return amount;
    }

    /// <summary>
    /// ページ番号とページサイズから skip 件数を計算します。
    /// </summary>
    public static int CalculateSkip(int pageNumber, int pageSize)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));
        RequirePositiveInt(pageSize, nameof(pageSize));

        return checked((pageNumber - 1) * pageSize);
    }

    /// <summary>
    /// 総件数とページサイズから総ページ数を計算します。
    /// </summary>
    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        RequireNonNegative(totalCount, nameof(totalCount));
        RequirePositiveInt(pageSize, nameof(pageSize));

        return totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (decimal)pageSize);
    }

    /// <summary>
    /// 指定ページが最終ページかを判定します。
    /// </summary>
    public static bool IsLastPage(int pageNumber, int totalCount, int pageSize)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));

        var totalPages = CalculateTotalPages(totalCount, pageSize);

        return pageNumber >= Math.Max(1, totalPages);
    }

    /// <summary>
    /// ページ指定を offset と limit に変換します。
    /// </summary>
    public static OffsetLimit ToOffsetLimit(int pageNumber, int pageSize)
    {
        return new OffsetLimit(CalculateSkip(pageNumber, pageSize), pageSize);
    }

    /// <summary>
    /// 一覧表示上の開始位置と終了位置を計算します。
    /// </summary>
    public static DisplayRange GetDisplayRange(int pageNumber, int pageSize, int totalCount)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));
        RequirePositiveInt(pageSize, nameof(pageSize));
        RequireNonNegative(totalCount, nameof(totalCount));

        if (totalCount == 0)
        {
            return new DisplayRange(0, 0);
        }

        var skip = checked((long)(pageNumber - 1) * pageSize);
        if (skip >= totalCount)
        {
            return new DisplayRange(0, 0);
        }

        return new DisplayRange((int)skip + 1, (int)Math.Min(skip + pageSize, totalCount));
    }

    /// <summary>
    /// ページサイズを既定値と上限の範囲に丸めます。
    /// </summary>
    public static int ClampPageSize(int pageSize, int maximumPageSize, int defaultPageSize = 20)
    {
        RequirePositiveInt(maximumPageSize, nameof(maximumPageSize));
        RequirePositiveInt(defaultPageSize, nameof(defaultPageSize));

        if (pageSize < 1)
        {
            return Math.Min(defaultPageSize, maximumPageSize);
        }

        return Math.Min(pageSize, maximumPageSize);
    }

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

    /// <summary>
    /// Int32 の加算がオーバーフローしないかを確認します。
    /// </summary>
    public static bool TryAddInt32(int left, int right, out int result)
    {
        try
        {
            result = checked(left + right);
            return true;
        }
        catch (OverflowException)
        {
            result = 0;
            return false;
        }
    }

    /// <summary>
    /// Int32 の乗算がオーバーフローしないかを確認します。
    /// </summary>
    public static bool TryMultiplyInt32(int left, int right, out int result)
    {
        try
        {
            result = checked(left * right);
            return true;
        }
        catch (OverflowException)
        {
            result = 0;
            return false;
        }
    }

    /// <summary>
    /// decimal の乗算がオーバーフローしないかを確認します。
    /// </summary>
    public static bool TryMultiplyDecimal(decimal left, decimal right, out decimal result)
    {
        try
        {
            result = checked(left * right);
            return true;
        }
        catch (OverflowException)
        {
            result = 0m;
            return false;
        }
    }

    /// <summary>
    /// 乗算結果が指定した絶対値上限を超えないかを判定します。
    /// </summary>
    public static bool CanMultiplyWithoutExceeding(decimal left, decimal right, decimal maximumAbsoluteValue)
    {
        RequireNonNegative(maximumAbsoluteValue, nameof(maximumAbsoluteValue));

        return TryMultiplyDecimal(left, right, out var result) && Math.Abs(result) <= maximumAbsoluteValue;
    }

    /// <summary>
    /// 2 つの Int32 を Int64 として乗算します。
    /// </summary>
    public static long BigMultiply(int left, int right)
    {
        return Math.BigMul(left, right);
    }

    /// <summary>
    /// double が有限値かを判定します。
    /// </summary>
    public static bool IsFinite(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// decimal の末尾ゼロを省いた文字列に整形します。
    /// </summary>
    public static string TrimTrailingZeros(decimal value, IFormatProvider? provider = null)
    {
        return value.ToString("0.############################", provider ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 比率をパーセント文字列に整形します。
    /// </summary>
    public static string FormatPercent(decimal ratio, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return (ratio * 100m).ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture) + "%";
    }

    /// <summary>
    /// 負数を括弧で表す会計形式に整形します。
    /// </summary>
    public static string FormatAccounting(decimal value, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        var absoluteValue = Math.Abs(value).ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);

        return value < 0m ? $"({absoluteValue})" : absoluteValue;
    }

    /// <summary>
    /// 単位付きの数値文字列に整形します。
    /// </summary>
    public static string FormatWithUnit(decimal value, string unit, int decimalPlaces = 0, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);
        ValidateDecimalPlaces(decimalPlaces);

        return $"{value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} {unit}";
    }

    /// <summary>
    /// ファイルサイズを読みやすい単位付き文字列に整形します。
    /// </summary>
    public static string FormatFileSize(long bytes, int decimalPlaces = 2)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes must be zero or greater.");
        }

        ValidateDecimalPlaces(decimalPlaces);

        string[] units = ["B", "KB", "MB", "GB", "TB"];
        var value = (decimal)bytes;
        var unitIndex = 0;

        while (value >= 1024m && unitIndex < units.Length - 1)
        {
            value /= 1024m;
            unitIndex++;
        }

        return $"{TrimTrailingZeros(RoundAwayFromZero(value, decimalPlaces))} {units[unitIndex]}";
    }

    /// <summary>
    /// 時間間隔をミリ秒または秒の文字列に整形します。
    /// </summary>
    public static string FormatDuration(TimeSpan duration, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        if (duration.TotalSeconds < 1d)
        {
            return $"{duration.TotalMilliseconds.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} ms";
        }

        return $"{duration.TotalSeconds.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} sec";
    }

    private static decimal[] SortRequiredValues(IEnumerable<decimal> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.Order().ToArray();
        if (items.Length == 0)
        {
            throw new ArgumentException("At least one value is required.", nameof(values));
        }

        return items;
    }

    private static decimal PowerOfTen(int exponent)
    {
        var result = 1m;

        for (var i = 0; i < exponent; i++)
        {
            result *= 10m;
        }

        return result;
    }

    private static void ValidateDecimalPlaces(int decimalPlaces)
    {
        if (decimalPlaces is < 0 or > 28)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be between 0 and 28.");
        }
    }
}
