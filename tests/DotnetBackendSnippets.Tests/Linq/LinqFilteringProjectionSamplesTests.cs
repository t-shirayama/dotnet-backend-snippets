using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed partial class LinqReverseLookupSamplesTests
{
    [Fact]
    public void WhereIf_AppliesPredicate_WhenConditionIsTrue()
    {
        var result = LinqReverseLookupSamples.WhereIf(Orders, condition: true, order => order.Amount >= 100m);

        Assert.Equal([1, 4, 5], result.Select(order => order.Id));
    }

    [Fact]
    public void WhereIf_ReturnsAllItems_WhenConditionIsFalse()
    {
        var result = LinqReverseLookupSamples.WhereIf(Orders, condition: false, order => order.Amount >= 100m);

        Assert.Equal([1, 2, 3, 4, 5], result.Select(order => order.Id));
    }

    [Fact]
    public void SearchOrders_AppliesOptionalFiltersIgnoringCategoryCase()
    {
        var criteria = new OrderSearchCriteria(CustomerId: 2, Category: "BOOKS", MaximumAmount: 100m);

        var result = LinqReverseLookupSamples.SearchOrders(Orders, criteria);

        var order = Assert.Single(result);
        Assert.Equal(3, order.Id);
    }

    [Fact]
    public void SearchOrders_Throws_WhenAmountRangeIsInvalid()
    {
        var criteria = new OrderSearchCriteria(MinimumAmount: 200m, MaximumAmount: 100m);

        var exception = Assert.Throws<ArgumentException>(() => LinqReverseLookupSamples.SearchOrders(Orders, criteria));

        Assert.Equal("criteria", exception.ParamName);
    }

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

    [Fact]
    public void ToOrderListItems_ProjectsOrderDto()
    {
        var result = LinqReverseLookupSamples.ToOrderListItems(Orders);

        Assert.Contains(new OrderListItem(1, 1, "Books", 120m), result);
    }
}
