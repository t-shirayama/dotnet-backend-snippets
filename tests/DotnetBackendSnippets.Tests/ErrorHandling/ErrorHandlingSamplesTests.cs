using DotnetBackendSnippets.ErrorHandling;

namespace DotnetBackendSnippets.Tests.ErrorHandling;

// テスト対象: Error Handling Samples のスニペット動作を確認する。
public sealed class ErrorHandlingSamplesTests
{
    // テスト意図: Try Parse Positive Int / Returns Success / When Value Is Positive Integer を確認する。
    [Fact]
    public void TryParsePositiveInt_ReturnsSuccess_WhenValueIsPositiveInteger()
    {
        var result = ErrorHandlingSamples.TryParsePositiveInt("42");

        Assert.True(result.Succeeded);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    // テスト意図: Try Parse Positive Int / Returns Failure / When Value Is Not Positive Integer を確認する。
    [Fact]
    public void TryParsePositiveInt_ReturnsFailure_WhenValueIsNotPositiveInteger()
    {
        var result = ErrorHandlingSamples.TryParsePositiveInt("0");

        Assert.False(result.Succeeded);
        Assert.Equal("Value must be positive.", result.Error);
    }

    // テスト意図: Throw If Invalid State / Throws / When State Is Invalid を確認する。
    [Fact]
    public void ThrowIfInvalidState_Throws_WhenStateIsInvalid()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => ErrorHandlingSamples.ThrowIfInvalidState(false));

        Assert.Contains("invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
