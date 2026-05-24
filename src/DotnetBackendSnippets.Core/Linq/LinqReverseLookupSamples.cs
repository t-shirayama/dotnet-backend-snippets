namespace DotnetBackendSnippets.Linq;

public enum OrderSortColumn
{
    Id,
    CustomerId,
    Category,
    Amount,
}

public enum SortDirection
{
    Ascending,
    Descending,
}

public sealed record OrderSearchCriteria(
    int? CustomerId = null,
    string? Category = null,
    decimal? MinimumAmount = null,
    decimal? MaximumAmount = null);

public sealed record OrderListItem(int Id, int CustomerId, string Category, decimal Amount);

public static class LinqReverseLookupSamples
{
    public static IReadOnlyList<T> WhereIf<T>(IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        return (condition ? source.Where(predicate) : source).ToList();
    }

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

    public static IReadOnlyList<OrderListItem> ToOrderListItems(IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders
            .Select(order => new OrderListItem(order.Id, order.CustomerId, order.Category, order.Amount))
            .ToList();
    }

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
