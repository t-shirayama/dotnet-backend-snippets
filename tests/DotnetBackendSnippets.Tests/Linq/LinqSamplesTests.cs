using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト対象: LINQ Samples のスニペット動作を確認する。
public sealed class LinqSamplesTests
{
    private static readonly Order[] Orders =
    [
        new(1, 1, "Books", 120m),
        new(2, 1, "Food", 50m),
        new(3, 2, "books", 80m),
        new(4, 3, "Tools", 200m),
    ];

    // テスト意図: Get Expensive Order Categories / Filters Projects And Deduplicates Categories を確認する。
    [Fact]
    public void GetExpensiveOrderCategories_FiltersProjectsAndDeduplicatesCategories()
    {
        var result = LinqSamples.GetExpensiveOrderCategories(Orders, minimumAmount: 100m);

        Assert.Equal(["Books", "Tools"], result);
    }

    // テスト意図: Sum Amount By Category / Groups Ignoring Case を確認する。
    [Fact]
    public void SumAmountByCategory_GroupsIgnoringCase()
    {
        var result = LinqSamples.SumAmountByCategory(Orders);

        Assert.Equal(200m, result["Books"]);
        Assert.Equal(50m, result["Food"]);
    }

    // テスト意図: Top Orders / Returns Highest Amounts Then Lowest Ids を確認する。
    [Fact]
    public void TopOrders_ReturnsHighestAmountsThenLowestIds()
    {
        var result = LinqSamples.TopOrders(Orders, count: 2);

        Assert.Equal([4, 1], result.Select(order => order.Id));
    }

    // テスト意図: Page / Returns Empty List / When Page Is Out Of Range を確認する。
    [Fact]
    public void Page_ReturnsEmptyList_WhenPageIsOutOfRange()
    {
        var result = LinqSamples.Page([1, 2, 3], pageNumber: 3, pageSize: 2);

        Assert.Empty(result);
    }

    // テスト意図: Page / Does Not Overflow / When Skip Count Is Very Large を確認する。
    [Fact]
    public void Page_DoesNotOverflow_WhenSkipCountIsVeryLarge()
    {
        var result = LinqSamples.Page([1, 2, 3], pageNumber: int.MaxValue, pageSize: int.MaxValue);

        Assert.Empty(result);
    }

    // テスト意図: Distinct By Key / Keeps First Item For Each Key を確認する。
    [Fact]
    public void DistinctByKey_KeepsFirstItemForEachKey()
    {
        var result = LinqSamples.DistinctByKey(Orders, order => order.CustomerId);

        Assert.Equal([1, 3, 4], result.Select(order => order.Id));
    }

    // テスト意図: Distinct By Key With Group By / Keeps First Item For Each Key を確認する。
    [Fact]
    public void DistinctByKeyWithGroupBy_KeepsFirstItemForEachKey()
    {
        var result = LinqSamples.DistinctByKeyWithGroupBy(Orders, order => order.CustomerId);

        Assert.Equal([1, 3, 4], result.Select(order => order.Id));
    }

    // テスト意図: Left Join Customer Orders / Includes Customers Without Orders を確認する。
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

    // テスト意図: Flatten / Returns Single Sequence を確認する。
    [Fact]
    public void Flatten_ReturnsSingleSequence()
    {
        var result = LinqSamples.Flatten(new[] { new[] { 1, 2 }, [], new[] { 3 } });

        Assert.Equal([1, 2, 3], result);
    }

    // テスト意図: Flatten / Throws / When Nested Collection Is Null を確認する。
    [Fact]
    public void Flatten_Throws_WhenNestedCollectionIsNull()
    {
        IEnumerable<int>?[] source = [[1], null];

        var exception = Assert.Throws<ArgumentException>(() => LinqSamples.Flatten(source!));

        Assert.Equal("source", exception.ParamName);
    }
}
