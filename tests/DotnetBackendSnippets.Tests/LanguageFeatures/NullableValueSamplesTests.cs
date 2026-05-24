using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Nullable Value Samples のスニペット動作を確認する。
public sealed class NullableValueSamplesTests
{
    // テスト意図: Quantity Or Default / Uses Fallback / When Value Is Null を確認する。
    [Fact]
    public void QuantityOrDefault_UsesFallback_WhenValueIsNull()
    {
        var result = NullableValueSamples.QuantityOrDefault(null, defaultQuantity: 10);

        Assert.Equal(10, result);
    }

    // テスト意図: Normalize Positive Quantity / Returns Null / For Missing Or Invalid Values を確認する。
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

    // テスト意図: Try Get Positive Quantity / Returns Out Value / When Positive を確認する。
    [Fact]
    public void TryGetPositiveQuantity_ReturnsOutValue_WhenPositive()
    {
        var success = NullableValueSamples.TryGetPositiveQuantity(5, out var value);

        Assert.True(success);
        Assert.Equal(5, value);
    }
}
