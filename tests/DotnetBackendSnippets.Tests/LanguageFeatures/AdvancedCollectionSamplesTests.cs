using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class AdvancedCollectionSamplesTests
{
    [Fact]
    public void CreateFixedLengthBuffer_ReturnsArrayEmpty_ForZeroLength()
    {
        var result = AdvancedCollectionSamples.CreateFixedLengthBuffer(0, initialValue: 1);

        Assert.Same(Array.Empty<int>(), result);
    }

    [Fact]
    public void CreateFixedLengthBuffer_FillsFixedLengthArray()
    {
        var result = AdvancedCollectionSamples.CreateFixedLengthBuffer(3, initialValue: 7);

        Assert.Equal([7, 7, 7], result);
    }

    [Fact]
    public void ProcessQueue_ReturnsFifoOrder()
    {
        var result = AdvancedCollectionSamples.ProcessQueue(["first", "second", "third"]);

        Assert.Equal(["first", "second", "third"], result);
    }

    [Fact]
    public void PopUndoStack_ReturnsLifoOrder()
    {
        var result = AdvancedCollectionSamples.PopUndoStack(["create", "edit", "publish"]);

        Assert.Equal(["publish", "edit", "create"], result);
    }
}
