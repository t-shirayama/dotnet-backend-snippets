namespace DotnetBackendSnippets.LanguageFeatures;

public static class ByRefAndUnsafeSamples
{
    public static void Increment(ref int value)
    {
        value++;
    }

    public static decimal CalculateTotal(in Measurement measurement)
    {
        return measurement.UnitPrice * measurement.Quantity;
    }

    public static bool TryParsePositiveInt(string? value, out int result)
    {
        if (int.TryParse(value, out var parsed) && parsed > 0)
        {
            result = parsed;
            return true;
        }

        result = 0;
        return false;
    }

    public static int ReadFirstWithPinnedPointer(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length == 0)
        {
            throw new ArgumentException("Values must contain at least one item.", nameof(values));
        }

        unsafe
        {
            fixed (int* pointer = values)
            {
                return *pointer;
            }
        }
    }
}

public readonly record struct Measurement(decimal UnitPrice, int Quantity);
