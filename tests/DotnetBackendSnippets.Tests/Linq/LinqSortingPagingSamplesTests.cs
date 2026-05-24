using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed partial class LinqReverseLookupSamplesTests
{
    [Fact]
    public void SortOrders_UsesRequestedColumnAndStableTieBreaker()
    {
        var result = LinqReverseLookupSamples.SortOrders(Orders, OrderSortColumn.Amount, SortDirection.Descending);

        Assert.Equal([4, 5, 1, 3, 2], result.Select(order => order.Id));
    }
}
