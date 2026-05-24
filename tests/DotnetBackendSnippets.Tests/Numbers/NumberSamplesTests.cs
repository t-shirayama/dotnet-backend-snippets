using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト対象: Number Samples のスニペット動作を確認する。
public sealed class NumberSamplesTests
{
    // テスト意図: Clamp / Returns Value Within Range を確認する。
    [Theory]
    [InlineData(5, 1, 10, 5)]
    [InlineData(-1, 1, 10, 1)]
    [InlineData(11, 1, 10, 10)]
    public void Clamp_ReturnsValueWithinRange(int value, int min, int max, int expected)
    {
        var result = NumberSamples.Clamp(value, min, max);

        Assert.Equal(expected, result);
    }

    // テスト意図: Divide Or Default / Returns Default / When Denominator Is Zero を確認する。
    [Fact]
    public void DivideOrDefault_ReturnsDefault_WhenDenominatorIsZero()
    {
        var result = NumberSamples.DivideOrDefault(10m, 0m, -1m);

        Assert.Equal(-1m, result);
    }

    // テスト意図: Percentage / Returns Rounded Percentage を確認する。
    [Fact]
    public void Percentage_ReturnsRoundedPercentage()
    {
        var result = NumberSamples.Percentage(1m, 3m, decimalPlaces: 1);

        Assert.Equal(33.3m, result);
    }

    // テスト意図: Round Currency / Uses Away From Zero Rounding を確認する。
    [Fact]
    public void RoundCurrency_UsesAwayFromZeroRounding()
    {
        var result = NumberSamples.RoundCurrency(1.005m);

        Assert.Equal(1.01m, result);
    }

    // テスト意図: Add Tax / Returns Rounded Tax Included Amount を確認する。
    [Fact]
    public void AddTax_ReturnsRoundedTaxIncludedAmount()
    {
        var result = NumberSamples.AddTax(100m, 0.1m);

        Assert.Equal(110m, result);
    }

    // テスト意図: Is Between / Handles Inclusive And Exclusive Ranges を確認する。
    [Theory]
    [InlineData(10, 10, 20, true, true)]
    [InlineData(10, 10, 20, false, false)]
    [InlineData(15, 10, 20, false, true)]
    public void IsBetween_HandlesInclusiveAndExclusiveRanges(
        int value,
        int min,
        int max,
        bool inclusive,
        bool expected)
    {
        var result = NumberSamples.IsBetween(value, min, max, inclusive);

        Assert.Equal(expected, result);
    }
}
