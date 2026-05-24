using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed partial class LinqReverseLookupSamplesTests
{
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
}
