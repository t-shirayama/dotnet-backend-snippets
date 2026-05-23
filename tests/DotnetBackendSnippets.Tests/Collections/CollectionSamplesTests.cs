using DotnetBackendSnippets.Collections;

namespace DotnetBackendSnippets.Tests.Collections;

public sealed class CollectionSamplesTests
{
    [Fact]
    public void IncrementCount_AddsOrIncrementsDictionaryValue()
    {
        var counts = new Dictionary<string, int>();

        CollectionSamples.IncrementCount(counts, "apple");
        CollectionSamples.IncrementCount(counts, "apple");

        Assert.Equal(2, counts["apple"]);
    }

    [Fact]
    public void FindDuplicates_ReturnsValuesThatAppearMoreThanOnce()
    {
        var result = CollectionSamples.FindDuplicates(["a", "b", "a", "c", "b"]);

        Assert.Equal(["a", "b"], result);
    }

    [Fact]
    public void MergeDictionaries_UsesSecondDictionaryWhenKeysOverlap()
    {
        var first = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        var second = new Dictionary<string, int> { ["b"] = 20, ["c"] = 30 };

        var result = CollectionSamples.MergeDictionaries(first, second);

        Assert.Equal(1, result["a"]);
        Assert.Equal(20, result["b"]);
        Assert.Equal(30, result["c"]);
    }

    [Fact]
    public void GetRequired_Throws_WhenKeyIsMissing()
    {
        var dictionary = new Dictionary<string, int> { ["exists"] = 1 };

        var exception = Assert.Throws<KeyNotFoundException>(() => CollectionSamples.GetRequired(dictionary, "missing"));

        Assert.Contains("missing", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AddIfMissing_PreservesOrderAndSkipsExistingValue()
    {
        var values = new List<string> { "a", "b" };

        var firstAdded = CollectionSamples.AddIfMissing(values, "c");
        var secondAdded = CollectionSamples.AddIfMissing(values, "b");

        Assert.True(firstAdded);
        Assert.False(secondAdded);
        Assert.Equal(["a", "b", "c"], values);
    }

    [Fact]
    public void ChunkBySize_ReturnsFixedSizeChunksWithRemainder()
    {
        var result = CollectionSamples.ChunkBySize([1, 2, 3, 4, 5], size: 2);

        Assert.Collection(
            result,
            chunk => Assert.Equal([1, 2], chunk),
            chunk => Assert.Equal([3, 4], chunk),
            chunk => Assert.Equal([5], chunk));
    }
}
