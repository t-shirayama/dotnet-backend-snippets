using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed class LinqSamplesTests
{
    private static readonly Order[] Orders =
    [
        new(1, 1, "Books", 120m),
        new(2, 1, "Food", 50m),
        new(3, 2, "books", 80m),
        new(4, 3, "Tools", 200m),
    ];

    [Fact]
    public void GetExpensiveOrderCategories_FiltersProjectsAndDeduplicatesCategories()
    {
        var result = LinqSamples.GetExpensiveOrderCategories(Orders, minimumAmount: 100m);

        Assert.Equal(["Books", "Tools"], result);
    }

    [Fact]
    public void SumAmountByCategory_GroupsIgnoringCase()
    {
        var result = LinqSamples.SumAmountByCategory(Orders);

        Assert.Equal(200m, result["Books"]);
        Assert.Equal(50m, result["Food"]);
    }

    [Fact]
    public void TopOrders_ReturnsHighestAmountsThenLowestIds()
    {
        var result = LinqSamples.TopOrders(Orders, count: 2);

        Assert.Equal([4, 1], result.Select(order => order.Id));
    }

    [Fact]
    public void Page_ReturnsEmptyList_WhenPageIsOutOfRange()
    {
        var result = LinqSamples.Page([1, 2, 3], pageNumber: 3, pageSize: 2);

        Assert.Empty(result);
    }

    [Fact]
    public void DistinctByKey_KeepsFirstItemForEachKey()
    {
        var result = LinqSamples.DistinctByKey(Orders, order => order.CustomerId);

        Assert.Equal([1, 3, 4], result.Select(order => order.Id));
    }

    [Fact]
    public void LeftJoinCustomerOrders_IncludesCustomersWithoutOrders()
    {
        var customers = new[]
        {
            new Customer(1, "Alice"),
            new Customer(2, "Bob"),
            new Customer(99, "No Orders"),
        };

        var result = LinqSamples.LeftJoinCustomerOrders(customers, Orders);

        Assert.Equal(3, result.Count);
        Assert.Contains(result, summary => summary.CustomerName == "No Orders" && summary.OrderCount == 0 && summary.TotalAmount == 0m);
    }

    [Fact]
    public void Flatten_ReturnsSingleSequence()
    {
        var result = LinqSamples.Flatten(new[] { new[] { 1, 2 }, [], new[] { 3 } });

        Assert.Equal([1, 2, 3], result);
    }
}
