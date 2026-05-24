using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed partial class NumberReverseLookupSamplesTests
{
    [Fact]
    public void TryAddInt32_ReturnsFalse_WhenOverflowWouldOccur()
    {
        var added = NumberReverseLookupSamples.TryAddInt32(int.MaxValue, 1, out var result);

        Assert.False(added);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryMultiplyInt32_ReturnsFalse_WhenOverflowWouldOccur()
    {
        var multiplied = NumberReverseLookupSamples.TryMultiplyInt32(int.MaxValue, 2, out var result);

        Assert.False(multiplied);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryMultiplyDecimal_ReturnsFalse_WhenOverflowWouldOccur()
    {
        var multiplied = NumberReverseLookupSamples.TryMultiplyDecimal(decimal.MaxValue, 2m, out var result);

        Assert.False(multiplied);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CanMultiplyWithoutExceeding_ReturnsFalse_WhenResultExceedsLimit()
    {
        var result = NumberReverseLookupSamples.CanMultiplyWithoutExceeding(11m, 10m, maximumAbsoluteValue: 100m);

        Assert.False(result);
    }

    [Fact]
    public void BigMultiply_ReturnsLongResult()
    {
        var result = NumberReverseLookupSamples.BigMultiply(int.MaxValue, 2);

        Assert.Equal(4294967294L, result);
    }

    [Theory]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(1.23d, true)]
    public void IsFinite_DetectsNanAndInfinity(double value, bool expected)
    {
        var result = NumberReverseLookupSamples.IsFinite(value);

        Assert.Equal(expected, result);
    }
}

