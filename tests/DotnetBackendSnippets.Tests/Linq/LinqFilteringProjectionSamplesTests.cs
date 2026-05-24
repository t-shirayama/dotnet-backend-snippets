using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト補助: LINQ Reverse Lookup Samples の共有データを定義する。
public sealed partial class LinqReverseLookupSamplesTests
{
    // テスト意図: Where If / Applies Predicate / When Condition Is True を確認する。
    [Fact]
    public void WhereIf_AppliesPredicate_WhenConditionIsTrue()
    {
        var result = LinqReverseLookupSamples.WhereIf(Orders, condition: true, order => order.Amount >= 100m);

        Assert.Equal([1, 4, 5], result.Select(order => order.Id));
    }

    // テスト意図: Where If / Returns All Items / When Condition Is False を確認する。
    [Fact]
    public void WhereIf_ReturnsAllItems_WhenConditionIsFalse()
    {
        var result = LinqReverseLookupSamples.WhereIf(Orders, condition: false, order => order.Amount >= 100m);

        Assert.Equal([1, 2, 3, 4, 5], result.Select(order => order.Id));
    }

    // テスト意図: Search Orders / Applies Optional Filters Ignoring Category Case を確認する。
    [Fact]
    public void SearchOrders_AppliesOptionalFiltersIgnoringCategoryCase()
    {
        var criteria = new OrderSearchCriteria(CustomerId: 2, Category: "BOOKS", MaximumAmount: 100m);

        var result = LinqReverseLookupSamples.SearchOrders(Orders, criteria);

        var order = Assert.Single(result);
        Assert.Equal(3, order.Id);
    }

    // テスト意図: Search Orders / Throws / When Amount Range Is Invalid を確認する。
    [Fact]
    public void SearchOrders_Throws_WhenAmountRangeIsInvalid()
    {
        var criteria = new OrderSearchCriteria(MinimumAmount: 200m, MaximumAmount: 100m);

        var exception = Assert.Throws<ArgumentException>(() => LinqReverseLookupSamples.SearchOrders(Orders, criteria));

        Assert.Equal("criteria", exception.ParamName);
    }

    // テスト意図: Filter By Ids / Uses Hash Set Lookup With Comparer を確認する。
    [Fact]
    public void FilterByIds_UsesHashSetLookupWithComparer()
    {
        var users = new[]
        {
            new UserSummary("ALICE", "Alice"),
            new UserSummary("bob", "Bob"),
            new UserSummary("carol", "Carol"),
        };

        var result = LinqReverseLookupSamples.FilterByIds(
            users,
            ["alice", "CAROL"],
            user => user.Id,
            StringComparer.OrdinalIgnoreCase);

        Assert.Equal(["Alice", "Carol"], result.Select(user => user.DisplayName));
    }

    // テスト意図: To Order List Items / Projects Order DTO を確認する。
    [Fact]
    public void ToOrderListItems_ProjectsOrderDto()
    {
        var result = LinqReverseLookupSamples.ToOrderListItems(Orders);

        Assert.Contains(new OrderListItem(1, 1, "Books", 120m), result);
    }
}
