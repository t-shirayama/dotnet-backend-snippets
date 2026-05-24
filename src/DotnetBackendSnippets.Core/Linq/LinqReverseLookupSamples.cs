namespace DotnetBackendSnippets.Linq;

/// <summary>
/// 注文一覧の並び替え列です。
/// </summary>
public enum OrderSortColumn
{
    /// <summary>
    /// 注文 ID で並び替えます。
    /// </summary>
    Id,

    /// <summary>
    /// 顧客 ID で並び替えます。
    /// </summary>
    CustomerId,

    /// <summary>
    /// カテゴリで並び替えます。
    /// </summary>
    Category,

    /// <summary>
    /// 金額で並び替えます。
    /// </summary>
    Amount,
}

/// <summary>
/// 並び替え方向です。
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// 昇順で並び替えます。
    /// </summary>
    Ascending,

    /// <summary>
    /// 降順で並び替えます。
    /// </summary>
    Descending,
}

/// <summary>
/// 注文検索条件です。
/// </summary>
/// <param name="CustomerId">絞り込む顧客 ID。</param>
/// <param name="Category">絞り込むカテゴリ。</param>
/// <param name="MinimumAmount">最小金額。</param>
/// <param name="MaximumAmount">最大金額。</param>
public sealed record OrderSearchCriteria(
    int? CustomerId = null,
    string? Category = null,
    decimal? MinimumAmount = null,
    decimal? MaximumAmount = null);

/// <summary>
/// 注文一覧に表示する項目です。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Category">カテゴリ。</param>
/// <param name="Amount">金額。</param>
public sealed record OrderListItem(int Id, int CustomerId, string Category, decimal Amount);

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを提供します。
/// </summary>
public static class LinqReverseLookupSamples
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

    /// <summary>
    /// キーごとの件数を数えます。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>キーごとの件数。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyDictionary<TKey, int> CountByKey<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups.ToDictionary(group => group.Key, group => group.Count(), comparer);
    }

    /// <summary>
    /// キーごとに最新の要素を取得します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="timestampSelector">比較する日時を取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>キーごとの最新要素。</returns>
    /// <exception cref="ArgumentNullException">必須引数が null です。</exception>
    public static IReadOnlyList<T> LatestPerKey<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<T, DateTimeOffset> timestampSelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(timestampSelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups.Select(group => group.MaxBy(timestampSelector)!).ToList();
    }

    /// <summary>
    /// 重複しているキーを取得します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>重複キーの一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyList<TKey> FindDuplicateKeys<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
    }

    /// <summary>
    /// 同じキーでは後勝ちにして辞書を作成します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>後勝ちで作成した辞書。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyDictionary<TKey, T> ToDictionaryLastWins<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var dictionary = comparer is null
            ? new Dictionary<TKey, T>()
            : new Dictionary<TKey, T>(comparer);

        foreach (var item in source)
        {
            dictionary[keySelector(item)] = item;
        }

        return dictionary;
    }

    /// <summary>
    /// キーごとの要素一覧を持つ辞書を作成します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>キーごとの要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyDictionary<TKey, IReadOnlyList<T>> ToLookupDictionary<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups.ToDictionary(
            group => group.Key,
            group => (IReadOnlyList<T>)group.ToList(),
            comparer);
    }

    /// <summary>
    /// 親要素と子要素の組み合わせから結果を作成します。
    /// </summary>
    /// <typeparam name="TParent">親要素の型。</typeparam>
    /// <typeparam name="TChild">子要素の型。</typeparam>
    /// <typeparam name="TResult">結果の型。</typeparam>
    /// <param name="source">親要素の入力シーケンス。</param>
    /// <param name="childrenSelector">親から子要素を取り出す関数。</param>
    /// <param name="resultSelector">親と子から結果を作る関数。</param>
    /// <returns>平坦化された結果一覧。</returns>
    /// <exception cref="ArgumentNullException">必須引数が null です。</exception>
    public static IReadOnlyList<TResult> SelectManyWithParent<TParent, TChild, TResult>(
        IEnumerable<TParent> source,
        Func<TParent, IEnumerable<TChild>?> childrenSelector,
        Func<TParent, TChild, TResult> resultSelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(childrenSelector);
        ArgumentNullException.ThrowIfNull(resultSelector);

        var result = new List<TResult>();

        foreach (var parent in source)
        {
            var children = childrenSelector(parent);
            if (children is null)
            {
                continue;
            }

            result.AddRange(children.Select(child => resultSelector(parent, child)));
        }

        return result;
    }
}
