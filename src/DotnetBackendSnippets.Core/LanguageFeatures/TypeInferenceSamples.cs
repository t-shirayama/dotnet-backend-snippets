namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// 型推論を使った明細処理のサンプルを提供します。
/// </summary>
public static class TypeInferenceSamples
{
    /// <summary>
    /// 注文明細から行ごとの集計を作成します。
    /// </summary>
    /// <param name="lines">注文明細の一覧。</param>
    /// <returns>明細ごとの集計一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lines"/> が null です。</exception>
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

    /// <summary>
    /// SKU ごとの合計金額を作成します。
    /// </summary>
    /// <param name="lines">注文明細の一覧。</param>
    /// <returns>SKU をキーにした合計金額。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lines"/> が null です。</exception>
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

/// <summary>
/// 注文明細です。
/// </summary>
/// <param name="Sku">商品 SKU。</param>
/// <param name="Quantity">数量。</param>
/// <param name="UnitPrice">単価。</param>
public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

/// <summary>
/// 注文明細の集計行です。
/// </summary>
/// <param name="Sku">商品 SKU。</param>
/// <param name="Quantity">数量。</param>
/// <param name="UnitPrice">単価。</param>
/// <param name="LineTotal">行合計。</param>
public sealed record OrderLineSummary(string Sku, int Quantity, decimal UnitPrice, decimal LineTotal);
