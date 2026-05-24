using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: By Ref And Unsafe Samples のスニペット動作を確認する。
public sealed class ByRefAndUnsafeSamplesTests
{
    // テスト意図: Increment / Updates Caller Variable / With Ref Parameter を確認する。
    [Fact]
    public void Increment_UpdatesCallerVariable_WithRefParameter()
    {
        var value = 1;

        ByRefAndUnsafeSamples.Increment(ref value);

        Assert.Equal(2, value);
    }

    // テスト意図: Calculate Total / Uses Read Only In Parameter を確認する。
    [Fact]
    public void CalculateTotal_UsesReadOnlyInParameter()
    {
        var measurement = new Measurement(12.5m, 4);

        var result = ByRefAndUnsafeSamples.CalculateTotal(in measurement);

        Assert.Equal(50m, result);
    }

    // テスト意図: Try Parse Positive Int / Returns Out Value を確認する。
    [Theory]
    [InlineData("5", true, 5)]
    [InlineData("0", false, 0)]
    [InlineData("abc", false, 0)]
    public void TryParsePositiveInt_ReturnsOutValue(string value, bool expectedSuccess, int expectedResult)
    {
        var success = ByRefAndUnsafeSamples.TryParsePositiveInt(value, out var result);

        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedResult, result);
    }

    // テスト意図: Read First With Pinned Pointer / Isolates Unsafe Code Behind Safe Method を確認する。
    [Fact]
    public void ReadFirstWithPinnedPointer_IsolatesUnsafeCodeBehindSafeMethod()
    {
        var result = ByRefAndUnsafeSamples.ReadFirstWithPinnedPointer([10, 20, 30]);

        Assert.Equal(10, result);
    }
}
