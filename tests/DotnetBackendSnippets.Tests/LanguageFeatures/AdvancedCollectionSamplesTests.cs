using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Advanced Collection Samples のスニペット動作を確認する。
public sealed class AdvancedCollectionSamplesTests
{
    // テスト意図: Create Fixed Length Buffer / Returns Array Empty / For Zero Length を確認する。
    [Fact]
    public void CreateFixedLengthBuffer_ReturnsArrayEmpty_ForZeroLength()
    {
        var result = AdvancedCollectionSamples.CreateFixedLengthBuffer(0, initialValue: 1);

        Assert.Same(Array.Empty<int>(), result);
    }

    // テスト意図: Create Fixed Length Buffer / Fills Fixed Length Array を確認する。
    [Fact]
    public void CreateFixedLengthBuffer_FillsFixedLengthArray()
    {
        var result = AdvancedCollectionSamples.CreateFixedLengthBuffer(3, initialValue: 7);

        Assert.Equal([7, 7, 7], result);
    }

    // テスト意図: Process Queue / Returns Fifo Order を確認する。
    [Fact]
    public void ProcessQueue_ReturnsFifoOrder()
    {
        var result = AdvancedCollectionSamples.ProcessQueue(["first", "second", "third"]);

        Assert.Equal(["first", "second", "third"], result);
    }

    // テスト意図: Pop Undo Stack / Returns Lifo Order を確認する。
    [Fact]
    public void PopUndoStack_ReturnsLifoOrder()
    {
        var result = AdvancedCollectionSamples.PopUndoStack(["create", "edit", "publish"]);

        Assert.Equal(["publish", "edit", "create"], result);
    }
}
