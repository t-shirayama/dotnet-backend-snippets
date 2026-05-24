namespace DotnetBackendSnippets.Linq;

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class LinqReverseLookupSamples
{
    /// <summary>
    /// 条件が true のときだけ Where を適用します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="condition">フィルターを適用するかどうか。</param>
    /// <param name="predicate">フィルター条件。</param>
    /// <returns>条件適用後の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="predicate"/> が null です。</exception>
    public static IReadOnlyList<T> WhereIf<T>(IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        return (condition ? source.Where(predicate) : source).ToList();
    }

    /// <summary>
    /// 注文を検索条件で絞り込みます。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <param name="criteria">検索条件。</param>
    /// <returns>条件に一致した注文一覧。</returns>
    /// <exception cref="ArgumentNullException">いずれかの引数が null です。</exception>
    /// <exception cref="ArgumentException">最小金額が最大金額を超えています。</exception>
    public static IReadOnlyList<Order> SearchOrders(IEnumerable<Order> orders, OrderSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(criteria);

        if (criteria.MinimumAmount is not null
            && criteria.MaximumAmount is not null
            && criteria.MinimumAmount > criteria.MaximumAmount)
        {
            throw new ArgumentException("Minimum amount must be less than or equal to maximum amount.", nameof(criteria));
        }

        var query = orders;

        if (criteria.CustomerId is not null)
        {
            query = query.Where(order => order.CustomerId == criteria.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Category))
        {
            query = query.Where(order => string.Equals(order.Category, criteria.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.MinimumAmount is not null)
        {
            query = query.Where(order => order.Amount >= criteria.MinimumAmount.Value);
        }

        if (criteria.MaximumAmount is not null)
        {
            query = query.Where(order => order.Amount <= criteria.MaximumAmount.Value);
        }

        return query.ToList();
    }

    /// <summary>
    /// ID 一覧に一致する要素だけを抽出します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="ids">抽出対象の ID。</param>
    /// <param name="keySelector">要素からキーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>ID に一致した要素一覧。</returns>
    /// <exception cref="ArgumentNullException">必須引数が null です。</exception>
    public static IReadOnlyList<T> FilterByIds<T, TKey>(
        IEnumerable<T> source,
        IEnumerable<TKey> ids,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentNullException.ThrowIfNull(keySelector);

        var idSet = comparer is null ? ids.ToHashSet() : ids.ToHashSet(comparer);
        if (idSet.Count == 0)
        {
            return [];
        }

        return source.Where(item => idSet.Contains(keySelector(item))).ToList();
    }

    /// <summary>
    /// 注文を一覧表示用の項目に変換します。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <returns>一覧表示用項目の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が null です。</exception>
    public static IReadOnlyList<OrderListItem> ToOrderListItems(IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders
            .Select(order => new OrderListItem(order.Id, order.CustomerId, order.Category, order.Amount))
            .ToList();
    }
}

