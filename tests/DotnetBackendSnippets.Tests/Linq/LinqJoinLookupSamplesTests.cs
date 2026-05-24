using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト補助: LINQ Reverse Lookup Samples の共有データを定義する。
public sealed partial class LinqReverseLookupSamplesTests
{
    // テスト意図: To Dictionary Last Wins / Does Not Throw On Duplicate Keys を確認する。
    [Fact]
    public void ToDictionaryLastWins_DoesNotThrowOnDuplicateKeys()
    {
        var result = LinqReverseLookupSamples.ToDictionaryLastWins(
            Orders,
            order => order.CustomerId);

        Assert.Equal(2, result[1].Id);
        Assert.Equal(5, result[2].Id);
    }

    // テスト意図: To Lookup Dictionary / Keeps Multiple Items Per Key を確認する。
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
