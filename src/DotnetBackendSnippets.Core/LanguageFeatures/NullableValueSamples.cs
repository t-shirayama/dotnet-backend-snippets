namespace DotnetBackendSnippets.LanguageFeatures;

public static class NullableValueSamples
{
    public static int QuantityOrDefault(int? quantity, int defaultQuantity = 0)
    {
        return quantity.GetValueOrDefault(defaultQuantity);
    }

    public static int? NormalizePositiveQuantity(int? quantity)
    {
        if (quantity is null)
        {
            return null;
        }

        return quantity.Value > 0 ? quantity.Value : null;
    }

    public static bool TryGetPositiveQuantity(int? quantity, out int value)
    {
        if (quantity.HasValue && quantity.Value > 0)
        {
            value = quantity.Value;
            return true;
        }

        value = 0;
        return false;
    }
}
