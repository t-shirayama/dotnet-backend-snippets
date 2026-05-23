using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed class NumberSamplesTests
{
    [Theory]
    [InlineData(5, 1, 10, 5)]
    [InlineData(-1, 1, 10, 1)]
    [InlineData(11, 1, 10, 10)]
    public void Clamp_ReturnsValueWithinRange(int value, int min, int max, int expected)
    {
        var result = NumberSamples.Clamp(value, min, max);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DivideOrDefault_ReturnsDefault_WhenDenominatorIsZero()
    {
        var result = NumberSamples.DivideOrDefault(10m, 0m, -1m);

        Assert.Equal(-1m, result);
    }

    [Fact]
    public void Percentage_ReturnsRoundedPercentage()
    {
        var result = NumberSamples.Percentage(1m, 3m, decimalPlaces: 1);

        Assert.Equal(33.3m, result);
    }

    [Fact]
    public void RoundCurrency_UsesAwayFromZeroRounding()
    {
        var result = NumberSamples.RoundCurrency(1.005m);

        Assert.Equal(1.01m, result);
    }

    [Fact]
    public void AddTax_ReturnsRoundedTaxIncludedAmount()
    {
        var result = NumberSamples.AddTax(100m, 0.1m);

        Assert.Equal(110m, result);
    }

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
