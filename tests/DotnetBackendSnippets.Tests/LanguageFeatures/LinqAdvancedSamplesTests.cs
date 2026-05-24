using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: LINQ Advanced Samples のスニペット動作を確認する。
public sealed class LinqAdvancedSamplesTests
{
    // テスト意図: Has Any High Value Order / Returns True / When Any Order Matches を確認する。
    [Fact]
    public void HasAnyHighValueOrder_ReturnsTrue_WhenAnyOrderMatches()
    {
        var orders = CreateOrders();

        var result = LinqAdvancedSamples.HasAnyHighValueOrder(orders, minimumAmount: 150m);

        Assert.True(result);
    }

    // テスト意図: Find Optional Order / Returns Null / When Missing を確認する。
    [Fact]
    public void FindOptionalOrder_ReturnsNull_WhenMissing()
    {
        var result = LinqAdvancedSamples.FindOptionalOrder(CreateOrders(), id: 99);

        Assert.Null(result);
    }

    // テスト意図: Find Required Order / Throws / When Missing を確認する。
    [Fact]
    public void FindRequiredOrder_Throws_WhenMissing()
    {
        Assert.Throws<KeyNotFoundException>(() => LinqAdvancedSamples.FindRequiredOrder(CreateOrders(), id: 99));
    }

    // テスト意図: Join Customer Orders / Returns Only Matched Rows を確認する。
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

    // テスト意図: Aggregate Orders / Returns State Object を確認する。
    [Fact]
    public void AggregateOrders_ReturnsStateObject()
    {
        var result = LinqAdvancedSamples.AggregateOrders(CreateOrders());

        Assert.Equal(new OrderAggregate(3, 350m, 200m), result);
    }

    // テスト意図: Aggregate Orders / Keeps Negative Maximum / When All Amounts Are Negative を確認する。
    [Fact]
    public void AggregateOrders_KeepsNegativeMaximum_WhenAllAmountsAreNegative()
    {
        AdvancedOrder[] orders =
        [
            new(10, 1, -100m),
            new(11, 1, -50m),
        ];

        var result = LinqAdvancedSamples.AggregateOrders(orders);

        Assert.Equal(new OrderAggregate(2, -150m, -50m), result);
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
