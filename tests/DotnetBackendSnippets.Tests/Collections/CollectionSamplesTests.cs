using DotnetBackendSnippets.Collections;

namespace DotnetBackendSnippets.Tests.Collections;

// テスト対象: Collection Samples のスニペット動作を確認する。
public sealed class CollectionSamplesTests
{
    // テスト意図: Increment Count / Adds Or Increments Dictionary Value を確認する。
    [Fact]
    public void IncrementCount_AddsOrIncrementsDictionaryValue()
    {
        var counts = new Dictionary<string, int>();

        CollectionSamples.IncrementCount(counts, "apple");
        CollectionSamples.IncrementCount(counts, "apple");

        Assert.Equal(2, counts["apple"]);
    }

    // テスト意図: Find Duplicates / Returns Values That Appear More Than Once を確認する。
    [Fact]
    public void FindDuplicates_ReturnsValuesThatAppearMoreThanOnce()
    {
        var result = CollectionSamples.FindDuplicates(["a", "b", "a", "c", "b"]);

        Assert.Equal(["a", "b"], result);
    }

    // テスト意図: Find Duplicates One Pass / Returns Values That Appear More Than Once を確認する。
    [Fact]
    public void FindDuplicatesOnePass_ReturnsValuesThatAppearMoreThanOnce()
    {
        var result = CollectionSamples.FindDuplicatesOnePass(["a", "b", "a", "c", "b", "a"]);

        Assert.Equal(["a", "b"], result);
    }

    // テスト意図: Merge Dictionaries / Uses Second Dictionary When Keys Overlap を確認する。
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

    // テスト意図: Get Required / Throws / When Key Is Missing を確認する。
    [Fact]
    public void GetRequired_Throws_WhenKeyIsMissing()
    {
        var dictionary = new Dictionary<string, int> { ["exists"] = 1 };

        var exception = Assert.Throws<KeyNotFoundException>(() => CollectionSamples.GetRequired(dictionary, "missing"));

        Assert.Contains("missing", exception.Message, StringComparison.Ordinal);
    }

    // テスト意図: Add If Missing / Preserves Order And Skips Existing Value を確認する。
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

    // テスト意図: Add If Missing / Adds To Set Using Set Lookup を確認する。
    [Fact]
    public void AddIfMissing_AddsToSetUsingSetLookup()
    {
        var values = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a" };

        var firstAdded = CollectionSamples.AddIfMissing(values, "B");
        var secondAdded = CollectionSamples.AddIfMissing(values, "b");

        Assert.True(firstAdded);
        Assert.False(secondAdded);
    }

    // テスト意図: Chunk By Size / Returns Fixed Size Chunks With Remainder を確認する。
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
