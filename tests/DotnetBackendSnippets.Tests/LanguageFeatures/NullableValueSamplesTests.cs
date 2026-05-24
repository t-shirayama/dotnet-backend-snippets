using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class NullableValueSamplesTests
{
    [Fact]
    public void QuantityOrDefault_UsesFallback_WhenValueIsNull()
    {
        var result = NullableValueSamples.QuantityOrDefault(null, defaultQuantity: 10);

        Assert.Equal(10, result);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(0, null)]
    [InlineData(-1, null)]
    [InlineData(3, 3)]
    public void NormalizePositiveQuantity_ReturnsNull_ForMissingOrInvalidValues(int? value, int? expected)
    {
        var result = NullableValueSamples.NormalizePositiveQuantity(value);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryGetPositiveQuantity_ReturnsOutValue_WhenPositive()
    {
        var success = NullableValueSamples.TryGetPositiveQuantity(5, out var value);

        Assert.True(success);
        Assert.Equal(5, value);
    }
}
