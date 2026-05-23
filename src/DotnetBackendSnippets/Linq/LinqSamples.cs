namespace DotnetBackendSnippets.Linq;

public sealed record Order(int Id, int CustomerId, string Category, decimal Amount);

public sealed record Customer(int Id, string Name);

public sealed record CustomerOrderSummary(string CustomerName, int OrderCount, decimal TotalAmount);

public static class LinqSamples
{
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

    public static IReadOnlyDictionary<string, decimal> SumAmountByCategory(IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders
            .GroupBy(order => order.Category, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Sum(order => order.Amount), StringComparer.OrdinalIgnoreCase);
    }

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

        return source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public static IReadOnlyList<T> DistinctByKey<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return source
            .GroupBy(keySelector)
            .Select(group => group.First())
            .ToList();
    }

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

    public static IReadOnlyList<T> Flatten<T>(IEnumerable<IEnumerable<T>> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.SelectMany(items => items).ToList();
    }
}
