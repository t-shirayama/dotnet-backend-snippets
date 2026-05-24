namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// LINQ の検索、結合、集計のサンプルを提供します。
/// </summary>
public static class LinqAdvancedSamples
{
    /// <summary>
    /// 指定金額以上の注文が存在するかを確認します。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <param name="minimumAmount">判定する最小金額。</param>
    /// <returns>条件を満たす注文があれば true。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が null です。</exception>
    public static bool HasAnyHighValueOrder(IEnumerable<AdvancedOrder> orders, decimal minimumAmount)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders.Any(order => order.Amount >= minimumAmount);
    }

    /// <summary>
    /// 注文 ID に一致する注文を任意結果として検索します。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <param name="id">検索する注文 ID。</param>
    /// <returns>見つかった注文、または null。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が null です。</exception>
    public static AdvancedOrder? FindOptionalOrder(IEnumerable<AdvancedOrder> orders, int id)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders.FirstOrDefault(order => order.Id == id);
    }

    /// <summary>
    /// 注文 ID に一致する注文を必須結果として検索します。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <param name="id">検索する注文 ID。</param>
    /// <returns>見つかった注文。</returns>
    /// <exception cref="KeyNotFoundException">注文が見つかりません。</exception>
    public static AdvancedOrder FindRequiredOrder(IEnumerable<AdvancedOrder> orders, int id)
    {
        return FindOptionalOrder(orders, id)
            ?? throw new KeyNotFoundException($"Order '{id}' was not found.");
    }

    /// <summary>
    /// 顧客と注文を結合して表示行を作成します。
    /// </summary>
    /// <param name="customers">顧客の一覧。</param>
    /// <param name="orders">注文の一覧。</param>
    /// <returns>顧客名を含む注文行の一覧。</returns>
    /// <exception cref="ArgumentNullException">いずれかの引数が null です。</exception>
    public static IReadOnlyList<CustomerOrderRow> JoinCustomerOrders(
        IEnumerable<AdvancedCustomer> customers,
        IEnumerable<AdvancedOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(customers);
        ArgumentNullException.ThrowIfNull(orders);

        return customers
            .Join(
                orders,
                customer => customer.Id,
                order => order.CustomerId,
                (customer, order) => new CustomerOrderRow(customer.Name, order.Id, order.Amount))
            .ToList();
    }

    /// <summary>
    /// 注文件数、合計金額、最大金額を集計します。
    /// </summary>
    /// <param name="orders">注文の一覧。</param>
    /// <returns>注文の集計結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> が null です。</exception>
    public static OrderAggregate AggregateOrders(IEnumerable<AdvancedOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        var count = 0;
        var totalAmount = 0m;
        var maxAmount = 0m;

        foreach (var order in orders)
        {
            maxAmount = count == 0 ? order.Amount : Math.Max(maxAmount, order.Amount);
            totalAmount += order.Amount;
            count++;
        }

        return new OrderAggregate(count, totalAmount, maxAmount);
    }
}

/// <summary>
/// LINQ サンプル用の顧客です。
/// </summary>
/// <param name="Id">顧客 ID。</param>
/// <param name="Name">顧客名。</param>
public sealed record AdvancedCustomer(int Id, string Name);

/// <summary>
/// LINQ サンプル用の注文です。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Amount">注文金額。</param>
public sealed record AdvancedOrder(int Id, int CustomerId, decimal Amount);

/// <summary>
/// 顧客名を含む注文表示行です。
/// </summary>
/// <param name="CustomerName">顧客名。</param>
/// <param name="OrderId">注文 ID。</param>
/// <param name="Amount">注文金額。</param>
public sealed record CustomerOrderRow(string CustomerName, int OrderId, decimal Amount);

/// <summary>
/// 注文の集計結果です。
/// </summary>
/// <param name="Count">注文件数。</param>
/// <param name="TotalAmount">合計金額。</param>
/// <param name="MaxAmount">最大金額。</param>
public sealed record OrderAggregate(int Count, decimal TotalAmount, decimal MaxAmount);
