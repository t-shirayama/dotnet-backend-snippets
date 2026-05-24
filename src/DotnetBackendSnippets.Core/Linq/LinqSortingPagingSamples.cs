namespace DotnetBackendSnippets.Linq;

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class LinqReverseLookupSamples
{
    /// <summary>
    /// 指定列と方向で注文を並び替えます。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <param name="sortColumn">並び替え列。</param>
    /// <param name="sortDirection">並び替え方向。</param>
    /// <returns>並び替え済みの注文一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が null です。</exception>
    /// <exception cref="ArgumentOutOfRangeException">並び替えオプションが未定義です。</exception>
    public static IReadOnlyList<Order> SortOrders(
        IEnumerable<Order> orders,
        OrderSortColumn sortColumn,
        SortDirection sortDirection)
    {
        ArgumentNullException.ThrowIfNull(orders);

        if (!Enum.IsDefined(sortColumn))
        {
            throw new ArgumentOutOfRangeException(nameof(sortColumn), "Sort column is not supported.");
        }

        if (!Enum.IsDefined(sortDirection))
        {
            throw new ArgumentOutOfRangeException(nameof(sortDirection), "Sort direction is not supported.");
        }

        var sorted = (sortColumn, sortDirection) switch
        {
            (OrderSortColumn.Id, SortDirection.Ascending) => orders.OrderBy(order => order.Id),
            (OrderSortColumn.Id, SortDirection.Descending) => orders.OrderByDescending(order => order.Id),
            (OrderSortColumn.CustomerId, SortDirection.Ascending) => orders.OrderBy(order => order.CustomerId),
            (OrderSortColumn.CustomerId, SortDirection.Descending) => orders.OrderByDescending(order => order.CustomerId),
            (OrderSortColumn.Category, SortDirection.Ascending) => orders.OrderBy(order => order.Category, StringComparer.OrdinalIgnoreCase),
            (OrderSortColumn.Category, SortDirection.Descending) => orders.OrderByDescending(order => order.Category, StringComparer.OrdinalIgnoreCase),
            (OrderSortColumn.Amount, SortDirection.Ascending) => orders.OrderBy(order => order.Amount),
            (OrderSortColumn.Amount, SortDirection.Descending) => orders.OrderByDescending(order => order.Amount),
            _ => throw new InvalidOperationException("Sort options were validated but not handled."),
        };

        return sortColumn == OrderSortColumn.Id
            ? sorted.ToList()
            : sorted.ThenBy(order => order.Id).ToList();
    }
}

