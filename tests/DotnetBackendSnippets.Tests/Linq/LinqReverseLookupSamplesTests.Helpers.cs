using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト補助: LINQ Reverse Lookup Samples の共有データを定義する。
public sealed partial class LinqReverseLookupSamplesTests
{
    private static readonly Order[] Orders =
    [
        new(1, 1, "Books", 120m),
        new(2, 1, "Food", 50m),
        new(3, 2, "books", 80m),
        new(4, 3, "Tools", 200m),
        new(5, 2, "Tools", 200m),
    ];

    private sealed record UserSummary(string Id, string DisplayName);

    private sealed record SupportTicket(int Id, int CustomerId, DateTimeOffset CreatedAt);

    private sealed record Cart(string Id, IReadOnlyList<CartItem>? Items);

    private sealed record CartItem(string Sku);
}
