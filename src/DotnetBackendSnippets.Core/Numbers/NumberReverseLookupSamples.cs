using System.Globalization;

namespace DotnetBackendSnippets.Numbers;

public static class NumberReverseLookupSamples
{
    public readonly record struct DecimalRange(decimal Min, decimal Max);

    public readonly record struct DisplayRange(int Start, int End);

    public readonly record struct FeeBreakdown(decimal PercentageFee, decimal FixedFee, decimal AppliedFee);

    public readonly record struct OffsetLimit(int Offset, int Limit);

    public readonly record struct TaxBreakdown(decimal NetAmount, decimal TaxAmount, decimal GrossAmount);

    public static int ParseIntOrDefault(string? value, int defaultValue = 0)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static bool TryParseDecimalInvariant(string? value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
    }

    public static decimal DefaultIfNull(decimal? value, decimal defaultValue = 0m)
    {
        return value ?? defaultValue;
    }

    public static int RequirePositiveInt(int value, string parameterName = "value")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be one or greater.");
        }

        return value;
    }

    public static decimal RequireNonNegative(decimal value, string parameterName = "value")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (value < 0m)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be zero or greater.");
        }

        return value;
    }

    public static decimal RequireFractionRate(decimal rate, string parameterName = "rate")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (rate is < 0m or > 1m)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Rate must be between 0 and 1.");
        }

        return rate;
    }

    public static decimal RoundAwayFromZero(decimal value, int decimalPlaces)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
    }

    public static decimal RoundBankers(decimal value, int decimalPlaces)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return Math.Round(value, decimalPlaces, MidpointRounding.ToEven);
    }

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

    public static decimal CeilingToUnit(decimal value, decimal unit)
    {
        if (unit <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unit), "Unit must be greater than zero.");
        }

        return Math.Ceiling(value / unit) * unit;
    }

    public static decimal FloorToUnit(decimal value, decimal unit)
    {
        if (unit <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unit), "Unit must be greater than zero.");
        }

        return Math.Floor(value / unit) * unit;
    }

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

    public static decimal CalculateRatio(decimal part, decimal whole, int decimalPlaces = 4)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return whole == 0m ? 0m : RoundAwayFromZero(part / whole, decimalPlaces);
    }

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

    public static decimal ApplyDiscountRate(decimal amount, decimal discountRate, int decimalPlaces = 2)
    {
        RequireNonNegative(amount, nameof(amount));
        RequireFractionRate(discountRate, nameof(discountRate));

        return RoundAwayFromZero(amount * (1m - discountRate), decimalPlaces);
    }

    public static decimal CalculateProfitMargin(decimal revenue, decimal cost, int decimalPlaces = 2)
    {
        ValidateDecimalPlaces(decimalPlaces);

        if (revenue == 0m)
        {
            return 0m;
        }

        return RoundAwayFromZero((revenue - cost) / revenue * 100m, decimalPlaces);
    }

    public static TaxBreakdown CalculateTaxFromNet(decimal netAmount, decimal taxRate, int decimalPlaces = 2)
    {
        RequireNonNegative(netAmount, nameof(netAmount));
        RequireNonNegative(taxRate, nameof(taxRate));

        var taxAmount = RoundAwayFromZero(netAmount * taxRate, decimalPlaces);
        var grossAmount = RoundAwayFromZero(netAmount + taxAmount, decimalPlaces);

        return new TaxBreakdown(RoundAwayFromZero(netAmount, decimalPlaces), taxAmount, grossAmount);
    }

    public static TaxBreakdown CalculateTaxFromGross(decimal grossAmount, decimal taxRate, int decimalPlaces = 2)
    {
        RequireNonNegative(grossAmount, nameof(grossAmount));
        RequireNonNegative(taxRate, nameof(taxRate));

        var netAmount = RoundAwayFromZero(grossAmount / (1m + taxRate), decimalPlaces);
        var taxAmount = RoundAwayFromZero(grossAmount - netAmount, decimalPlaces);

        return new TaxBreakdown(netAmount, taxAmount, RoundAwayFromZero(grossAmount, decimalPlaces));
    }

    public static decimal CalculateTotalWithShipping(decimal subtotal, decimal shipping, int decimalPlaces = 2)
    {
        RequireNonNegative(subtotal, nameof(subtotal));
        RequireNonNegative(shipping, nameof(shipping));

        return RoundAwayFromZero(subtotal + shipping, decimalPlaces);
    }

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

    public static string FormatThousands(decimal value, int decimalPlaces = 0, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);
    }

    public static string FormatFixedDecimal(decimal value, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return value.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);
    }

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

    public static long ToMinorCurrencyUnits(decimal amount, int fractionDigits = 2)
    {
        ValidateDecimalPlaces(fractionDigits);

        var multiplier = PowerOfTen(fractionDigits);

        return checked((long)RoundAwayFromZero(amount * multiplier, 0));
    }

    public static decimal FromMinorCurrencyUnits(long minorUnits, int fractionDigits = 2)
    {
        ValidateDecimalPlaces(fractionDigits);

        return minorUnits / PowerOfTen(fractionDigits);
    }

    public static decimal ConvertCurrency(decimal amount, decimal exchangeRate, int decimalPlaces = 2)
    {
        RequireNonNegative(exchangeRate, nameof(exchangeRate));

        return RoundAwayFromZero(amount * exchangeRate, decimalPlaces);
    }

    public static bool IsRefund(decimal amount)
    {
        return amount < 0m;
    }

    public static decimal EnsureMaximumAmount(decimal amount, decimal maximumAmount, string parameterName = "amount")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (amount > maximumAmount)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Amount exceeds the allowed maximum.");
        }

        return amount;
    }

    public static int CalculateSkip(int pageNumber, int pageSize)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));
        RequirePositiveInt(pageSize, nameof(pageSize));

        return checked((pageNumber - 1) * pageSize);
    }

    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        RequireNonNegative(totalCount, nameof(totalCount));
        RequirePositiveInt(pageSize, nameof(pageSize));

        return totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (decimal)pageSize);
    }

    public static bool IsLastPage(int pageNumber, int totalCount, int pageSize)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));

        var totalPages = CalculateTotalPages(totalCount, pageSize);

        return pageNumber >= Math.Max(1, totalPages);
    }

    public static OffsetLimit ToOffsetLimit(int pageNumber, int pageSize)
    {
        return new OffsetLimit(CalculateSkip(pageNumber, pageSize), pageSize);
    }

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

    public static decimal AverageOrDefault(IEnumerable<decimal> values, decimal defaultValue = 0m)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.ToArray();

        return items.Length == 0 ? defaultValue : SumAmounts(items) / items.Length;
    }

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

    public static decimal Median(IEnumerable<decimal> values)
    {
        var items = SortRequiredValues(values);
        var middle = items.Length / 2;

        return items.Length % 2 == 1
            ? items[middle]
            : (items[middle - 1] + items[middle]) / 2m;
    }

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

    public static bool CanMultiplyWithoutExceeding(decimal left, decimal right, decimal maximumAbsoluteValue)
    {
        RequireNonNegative(maximumAbsoluteValue, nameof(maximumAbsoluteValue));

        return TryMultiplyDecimal(left, right, out var result) && Math.Abs(result) <= maximumAbsoluteValue;
    }

    public static long BigMultiply(int left, int right)
    {
        return Math.BigMul(left, right);
    }

    public static bool IsFinite(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }

    public static string TrimTrailingZeros(decimal value, IFormatProvider? provider = null)
    {
        return value.ToString("0.############################", provider ?? CultureInfo.InvariantCulture);
    }

    public static string FormatPercent(decimal ratio, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return (ratio * 100m).ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture) + "%";
    }

    public static string FormatAccounting(decimal value, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        var absoluteValue = Math.Abs(value).ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);

        return value < 0m ? $"({absoluteValue})" : absoluteValue;
    }

    public static string FormatWithUnit(decimal value, string unit, int decimalPlaces = 0, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);
        ValidateDecimalPlaces(decimalPlaces);

        return $"{value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} {unit}";
    }

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
