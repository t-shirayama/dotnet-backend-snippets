namespace DotnetBackendSnippets.LanguageFeatures;

public static class TypeInferenceSamples
{
    public static IReadOnlyList<OrderLineSummary> BuildLineSummaries(IEnumerable<OrderLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var summaries = lines.Select(line => new OrderLineSummary(
            line.Sku,
            line.Quantity,
            line.UnitPrice,
            line.Quantity * line.UnitPrice));

        return summaries.ToList();
    }

    public static IReadOnlyDictionary<string, decimal> BuildTotalsBySku(IEnumerable<OrderLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        Dictionary<string, decimal> totals = [];

        foreach (var line in lines)
        {
            totals[line.Sku] = totals.GetValueOrDefault(line.Sku) + line.Quantity * line.UnitPrice;
        }

        return totals;
    }
}

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

public sealed record OrderLineSummary(string Sku, int Quantity, decimal UnitPrice, decimal LineTotal);
