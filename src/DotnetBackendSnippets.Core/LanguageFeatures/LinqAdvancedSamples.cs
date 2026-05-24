namespace DotnetBackendSnippets.LanguageFeatures;

public static class LinqAdvancedSamples
{
    public static bool HasAnyHighValueOrder(IEnumerable<AdvancedOrder> orders, decimal minimumAmount)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders.Any(order => order.Amount >= minimumAmount);
    }

    public static AdvancedOrder? FindOptionalOrder(IEnumerable<AdvancedOrder> orders, int id)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders.FirstOrDefault(order => order.Id == id);
    }

    public static AdvancedOrder FindRequiredOrder(IEnumerable<AdvancedOrder> orders, int id)
    {
        return FindOptionalOrder(orders, id)
            ?? throw new KeyNotFoundException($"Order '{id}' was not found.");
    }

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

    public static OrderAggregate AggregateOrders(IEnumerable<AdvancedOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        return orders.Aggregate(
            new OrderAggregate(0, 0m, 0m),
            (current, order) => new OrderAggregate(
                current.Count + 1,
                current.TotalAmount + order.Amount,
                Math.Max(current.MaxAmount, order.Amount)));
    }
}

public sealed record AdvancedCustomer(int Id, string Name);

public sealed record AdvancedOrder(int Id, int CustomerId, decimal Amount);

public sealed record CustomerOrderRow(string CustomerName, int OrderId, decimal Amount);

public sealed record OrderAggregate(int Count, decimal TotalAmount, decimal MaxAmount);
