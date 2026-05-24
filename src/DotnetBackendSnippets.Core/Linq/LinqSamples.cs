namespace DotnetBackendSnippets.Linq;

/// <summary>
/// 注文データを表します。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Category">注文カテゴリ。</param>
/// <param name="Amount">注文金額。</param>
public sealed record Order(int Id, int CustomerId, string Category, decimal Amount);

/// <summary>
/// 顧客データを表します。
/// </summary>
/// <param name="Id">顧客 ID。</param>
/// <param name="Name">顧客名。</param>
public sealed record Customer(int Id, string Name);

/// <summary>
/// 顧客ごとの注文集計を表します。
/// </summary>
/// <param name="CustomerName">顧客名。</param>
/// <param name="OrderCount">注文件数。</param>
/// <param name="TotalAmount">注文合計金額。</param>
public sealed record CustomerOrderSummary(string CustomerName, int OrderCount, decimal TotalAmount);

/// <summary>
/// LINQ を使った基本的なデータ操作例を提供します。
/// </summary>
public static class LinqSamples
{
    /// <summary>
    /// 指定金額以上の注文カテゴリを重複なしで取得します。
    /// </summary>
    /// <param name="orders">注文一覧。</param>
    /// <param name="minimumAmount">抽出する最小金額。</param>
    /// <returns>カテゴリ名の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<string> GetExpensiveOrderCategories(IEnumerable<Order> orders, decimal minimumAmount)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders
            .Where(order => order.Amount >= minimumAmount)
            .Select(order => order.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(category => category, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// 注文金額をカテゴリごとに合計します。
    /// </summary>
    /// <param name="orders">注文一覧。</param>
    /// <returns>カテゴリ名をキー、合計金額を値にした辞書。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyDictionary<string, decimal> SumAmountByCategory(IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders
            .GroupBy(order => order.Category, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Sum(order => order.Amount), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 金額の高い注文を指定件数取得します。
    /// </summary>
    /// <param name="orders">注文一覧。</param>
    /// <param name="count">取得件数。</param>
    /// <returns>金額降順の注文一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> が 0 未満の場合。</exception>
    public static IReadOnlyList<Order> TopOrders(IEnumerable<Order> orders, int count)
    {
        ArgumentNullException.ThrowIfNull(orders);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be zero or greater.");
        }

        return orders
            .OrderByDescending(order => order.Amount)
            .ThenBy(order => order.Id)
            .Take(count)
            .ToList();
    }

    /// <summary>
    /// シーケンスから指定ページの要素を取得します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">ページング対象のシーケンス。</param>
    /// <param name="pageNumber">1 始まりのページ番号。</param>
    /// <param name="pageSize">1 ページあたりの件数。</param>
    /// <returns>指定ページに含まれる要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> または <paramref name="pageSize"/> が 1 未満の場合。</exception>
    public static IReadOnlyList<T> Page<T>(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be one or greater.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be one or greater.");
        }

        var skipCount = ((long)pageNumber - 1) * pageSize;

        return source
            .SkipLong(skipCount)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// キー selector で重複を除外します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">重複判定に使うキーの型。</typeparam>
    /// <param name="source">重複を除外するシーケンス。</param>
    /// <param name="keySelector">重複判定キーを返す関数。</param>
    /// <returns>キーが重複しない要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<T> DistinctByKey<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return source.DistinctBy(keySelector).ToList();
    }

    /// <summary>
    /// GroupBy を使ってキー重複を除外します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">重複判定に使うキーの型。</typeparam>
    /// <param name="source">重複を除外するシーケンス。</param>
    /// <param name="keySelector">重複判定キーを返す関数。</param>
    /// <returns>各キーの先頭要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<T> DistinctByKeyWithGroupBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return source.GroupBy(keySelector).Select(group => group.First()).ToList();
    }

    /// <summary>
    /// 顧客と注文を左外部結合し、顧客ごとの注文数と合計金額を返します。
    /// </summary>
    /// <param name="customers">顧客一覧。</param>
    /// <param name="orders">注文一覧。</param>
    /// <returns>顧客ごとの注文集計一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="customers"/> または <paramref name="orders"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<CustomerOrderSummary> LeftJoinCustomerOrders(
        IEnumerable<Customer> customers,
        IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(customers);
        ArgumentNullException.ThrowIfNull(orders);

        return customers
            .GroupJoin(
                orders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, customerOrders) => new CustomerOrderSummary(
                    customer.Name,
                    customerOrders.Count(),
                    customerOrders.Sum(order => order.Amount)))
            .ToList();
    }

    /// <summary>
    /// 入れ子になったシーケンスを 1 つの一覧に平坦化します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">入れ子のシーケンス。</param>
    /// <returns>平坦化された要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="source"/> に <see langword="null"/> のシーケンスが含まれる場合。</exception>
    public static IReadOnlyList<T> Flatten<T>(IEnumerable<IEnumerable<T>> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var flattened = new List<T>();

        foreach (var items in source)
        {
            if (items is null)
            {
                throw new ArgumentException("Nested collections must not be null.", nameof(source));
            }

            flattened.AddRange(items);
        }

        return flattened;
    }

    private static IEnumerable<T> SkipLong<T>(this IEnumerable<T> source, long count)
    {
        foreach (var item in source)
        {
            if (count > 0)
            {
                count--;
                continue;
            }

            yield return item;
        }
    }
}
