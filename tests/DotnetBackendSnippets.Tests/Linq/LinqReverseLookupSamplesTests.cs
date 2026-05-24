using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed class LinqReverseLookupSamplesTests
{
    private static readonly Order[] Orders =
    [
        new(1, 1, "Books", 120m),
        new(2, 1, "Food", 50m),
        new(3, 2, "books", 80m),
        new(4, 3, "Tools", 200m),
        new(5, 2, "Tools", 200m),
    ];

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

    [Fact]
    public void SortOrders_UsesRequestedColumnAndStableTieBreaker()
    {
        var result = LinqReverseLookupSamples.SortOrders(Orders, OrderSortColumn.Amount, SortDirection.Descending);

        Assert.Equal([4, 5, 1, 3, 2], result.Select(order => order.Id));
    }

    [Fact]
    public void CountByKey_GroupsUsingComparer()
    {
        var result = LinqReverseLookupSamples.CountByKey(
            Orders,
            order => order.Category,
            StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, result["Books"]);
        Assert.Equal(2, result["Tools"]);
    }

    [Fact]
    public void LatestPerKey_ReturnsNewestItemPerGroup()
    {
        var tickets = new[]
        {
            new SupportTicket(1, 10, new DateTimeOffset(2026, 5, 20, 9, 0, 0, TimeSpan.Zero)),
            new SupportTicket(2, 10, new DateTimeOffset(2026, 5, 21, 9, 0, 0, TimeSpan.Zero)),
            new SupportTicket(3, 20, new DateTimeOffset(2026, 5, 19, 9, 0, 0, TimeSpan.Zero)),
        };

        var result = LinqReverseLookupSamples.LatestPerKey(
            tickets,
            ticket => ticket.CustomerId,
            ticket => ticket.CreatedAt);

        Assert.Equal([2, 3], result.Select(ticket => ticket.Id));
    }

    [Fact]
    public void FindDuplicateKeys_ReturnsKeysAppearingMoreThanOnce()
    {
        var result = LinqReverseLookupSamples.FindDuplicateKeys(
            Orders,
            order => order.Category,
            StringComparer.OrdinalIgnoreCase);

        Assert.Equal(["Books", "Tools"], result);
    }

    [Fact]
    public void ToDictionaryLastWins_DoesNotThrowOnDuplicateKeys()
    {
        var result = LinqReverseLookupSamples.ToDictionaryLastWins(
            Orders,
            order => order.CustomerId);

        Assert.Equal(2, result[1].Id);
        Assert.Equal(5, result[2].Id);
    }

    [Fact]
    public void ToLookupDictionary_KeepsMultipleItemsPerKey()
    {
        var result = LinqReverseLookupSamples.ToLookupDictionary(
            Orders,
            order => order.CustomerId);

        Assert.Equal([1, 2], result[1].Select(order => order.Id));
        Assert.Equal([3, 5], result[2].Select(order => order.Id));
    }

    [Fact]
    public void SelectManyWithParent_SkipsNullChildrenAndKeepsParentValues()
    {
        var carts = new[]
        {
            new Cart("cart-1", [new CartItem("book"), new CartItem("pen")]),
            new Cart("cart-2", null),
            new Cart("cart-3", [new CartItem("mug")]),
        };

        var result = LinqReverseLookupSamples.SelectManyWithParent(
            carts,
            cart => cart.Items,
            (cart, item) => $"{cart.Id}:{item.Sku}");

        Assert.Equal(["cart-1:book", "cart-1:pen", "cart-3:mug"], result);
    }

    private sealed record UserSummary(string Id, string DisplayName);

    private sealed record SupportTicket(int Id, int CustomerId, DateTimeOffset CreatedAt);

    private sealed record Cart(string Id, IReadOnlyList<CartItem>? Items);

    private sealed record CartItem(string Sku);
}
