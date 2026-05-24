using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed partial class NumberReverseLookupSamplesTests
{
    [Fact]
    public void ParseIntOrDefault_ReturnsFallback_WhenValueIsInvalid()
    {
        var result = NumberReverseLookupSamples.ParseIntOrDefault("not-a-number", defaultValue: 25);

        Assert.Equal(25, result);
    }

    [Fact]
    public void TryParseDecimalInvariant_ParsesCommaSeparatedInvariantValue()
    {
        var parsed = NumberReverseLookupSamples.TryParseDecimalInvariant("1,234.56", out var result);

        Assert.True(parsed);
        Assert.Equal(1234.56m, result);
    }

    [Fact]
    public void DefaultIfNull_ReturnsFallback_WhenValueIsNull()
    {
        var result = NumberReverseLookupSamples.DefaultIfNull(null, 10m);

        Assert.Equal(10m, result);
    }

    [Fact]
    public void RequirePositiveInt_Throws_WhenValueIsZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => NumberReverseLookupSamples.RequirePositiveInt(0, "pageNumber"));

        Assert.Equal("pageNumber", exception.ParamName);
    }

    [Fact]
    public void RequireFractionRate_Throws_WhenRateIsGreaterThanOne()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => NumberReverseLookupSamples.RequireFractionRate(1.2m, "discountRate"));

        Assert.Equal("discountRate", exception.ParamName);
    }

    [Fact]
    public void IsRefund_ReturnsTrue_WhenAmountIsNegative()
    {
        var result = NumberReverseLookupSamples.IsRefund(-100m);

        Assert.True(result);
    }

    [Fact]
    public void EnsureMaximumAmount_Throws_WhenAmountExceedsMaximum()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => NumberReverseLookupSamples.EnsureMaximumAmount(101m, 100m, "amount"));

        Assert.Equal("amount", exception.ParamName);
    }
}

