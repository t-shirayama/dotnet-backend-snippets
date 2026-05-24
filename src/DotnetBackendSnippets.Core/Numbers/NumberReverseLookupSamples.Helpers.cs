namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
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

