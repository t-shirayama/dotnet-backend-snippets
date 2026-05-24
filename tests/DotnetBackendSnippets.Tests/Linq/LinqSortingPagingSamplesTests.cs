using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト補助: LINQ Reverse Lookup Samples の共有データを定義する。
public sealed partial class LinqReverseLookupSamplesTests
{
    // テスト意図: Sort Orders / Uses Requested Column And Stable Tie Breaker を確認する。
    [Fact]
    public void SortOrders_UsesRequestedColumnAndStableTieBreaker()
    {
        var result = LinqReverseLookupSamples.SortOrders(Orders, OrderSortColumn.Amount, SortDirection.Descending);

        Assert.Equal([4, 5, 1, 3, 2], result.Select(order => order.Id));
    }
}
