using DotnetBackendSnippets.ErrorHandling;

namespace DotnetBackendSnippets.Tests.ErrorHandling;

public sealed class ErrorHandlingSamplesTests
{
    [Fact]
    public void TryParsePositiveInt_ReturnsSuccess_WhenValueIsPositiveInteger()
    {
        var result = ErrorHandlingSamples.TryParsePositiveInt("42");

        Assert.True(result.Succeeded);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void TryParsePositiveInt_ReturnsFailure_WhenValueIsNotPositiveInteger()
    {
        var result = ErrorHandlingSamples.TryParsePositiveInt("0");

        Assert.False(result.Succeeded);
        Assert.Equal("Value must be positive.", result.Error);
    }

    [Fact]
    public void ThrowIfInvalidState_Throws_WhenStateIsInvalid()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => ErrorHandlingSamples.ThrowIfInvalidState(false));

        Assert.Contains("invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
