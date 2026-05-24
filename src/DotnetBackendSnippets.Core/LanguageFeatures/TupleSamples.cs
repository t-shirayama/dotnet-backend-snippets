namespace DotnetBackendSnippets.LanguageFeatures;

public static class TupleSamples
{
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

    public static NumericRange ToRangeRecord((int Min, int Max) range)
    {
        if (range.Min > range.Max)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(range));
        }

        return new NumericRange(range.Min, range.Max);
    }
}

public sealed record NumericRange(int Min, int Max);
