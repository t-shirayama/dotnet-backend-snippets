using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class LinqAdvancedSamplesTests
{
    [Fact]
    public void HasAnyHighValueOrder_ReturnsTrue_WhenAnyOrderMatches()
    {
        var orders = CreateOrders();

        var result = LinqAdvancedSamples.HasAnyHighValueOrder(orders, minimumAmount: 150m);

        Assert.True(result);
    }

    [Fact]
    public void FindOptionalOrder_ReturnsNull_WhenMissing()
    {
        var result = LinqAdvancedSamples.FindOptionalOrder(CreateOrders(), id: 99);

        Assert.Null(result);
    }

    [Fact]
    public void FindRequiredOrder_Throws_WhenMissing()
    {
        Assert.Throws<KeyNotFoundException>(() => LinqAdvancedSamples.FindRequiredOrder(CreateOrders(), id: 99));
    }

    [Fact]
    public void JoinCustomerOrders_ReturnsOnlyMatchedRows()
    {
        AdvancedCustomer[] customers =
        [
            new(1, "A"),
            new(2, "B"),
            new(3, "C"),
        ];

        var result = LinqAdvancedSamples.JoinCustomerOrders(customers, CreateOrders());

        Assert.Equal(
            [
                new CustomerOrderRow("A", 10, 100m),
                new CustomerOrderRow("A", 11, 200m),
                new CustomerOrderRow("B", 12, 50m),
            ],
            result);
    }

    [Fact]
    public void AggregateOrders_ReturnsStateObject()
    {
        var result = LinqAdvancedSamples.AggregateOrders(CreateOrders());

        Assert.Equal(new OrderAggregate(3, 350m, 200m), result);
    }

    private static AdvancedOrder[] CreateOrders()
    {
        return
        [
            new(10, 1, 100m),
            new(11, 1, 200m),
            new(12, 2, 50m),
        ];
    }
}
