namespace DotnetBackendSnippets.Numbers;

public static class NumberSamples
{
    public static int Clamp(int value, int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(min));
        }

        return Math.Min(Math.Max(value, min), max);
    }

    public static decimal DivideOrDefault(decimal numerator, decimal denominator, decimal defaultValue = 0m)
    {
        return denominator == 0m ? defaultValue : numerator / denominator;
    }

    public static decimal Percentage(decimal part, decimal whole, int decimalPlaces = 2)
    {
        if (decimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be zero or greater.");
        }

        if (whole == 0m)
        {
            return 0m;
        }

        return Math.Round(part / whole * 100m, decimalPlaces, MidpointRounding.AwayFromZero);
    }

    public static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal AddTax(decimal netAmount, decimal taxRate)
    {
        if (taxRate < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(taxRate), "Tax rate must be zero or greater.");
        }

        return RoundCurrency(netAmount * (1m + taxRate));
    }

    public static bool IsBetween(int value, int min, int max, bool inclusive = true)
    {
        if (min > max)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(min));
        }

        return inclusive
            ? value >= min && value <= max
            : value > min && value < max;
    }
}
