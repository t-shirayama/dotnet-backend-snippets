namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
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
}

