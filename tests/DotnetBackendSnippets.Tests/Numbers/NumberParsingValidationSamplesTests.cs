using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト補助: Number Reverse Lookup Samples の共有型を定義する。
public sealed partial class NumberReverseLookupSamplesTests
{
    // テスト意図: Parse Int Or Default / Returns Fallback / When Value Is Invalid を確認する。
    [Fact]
    public void ParseIntOrDefault_ReturnsFallback_WhenValueIsInvalid()
    {
        var result = NumberReverseLookupSamples.ParseIntOrDefault("not-a-number", defaultValue: 25);

        Assert.Equal(25, result);
    }

    // テスト意図: Try Parse Decimal Invariant / Parses Comma Separated Invariant Value を確認する。
    [Fact]
    public void TryParseDecimalInvariant_ParsesCommaSeparatedInvariantValue()
    {
        var parsed = NumberReverseLookupSamples.TryParseDecimalInvariant("1,234.56", out var result);

        Assert.True(parsed);
        Assert.Equal(1234.56m, result);
    }

    // テスト意図: Default If Null / Returns Fallback / When Value Is Null を確認する。
    [Fact]
    public void DefaultIfNull_ReturnsFallback_WhenValueIsNull()
    {
        var result = NumberReverseLookupSamples.DefaultIfNull(null, 10m);

        Assert.Equal(10m, result);
    }

    // テスト意図: Require Positive Int / Throws / When Value Is Zero を確認する。
    [Fact]
    public void RequirePositiveInt_Throws_WhenValueIsZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => NumberReverseLookupSamples.RequirePositiveInt(0, "pageNumber"));

        Assert.Equal("pageNumber", exception.ParamName);
    }

    // テスト意図: Require Fraction Rate / Throws / When Rate Is Greater Than One を確認する。
    [Fact]
    public void RequireFractionRate_Throws_WhenRateIsGreaterThanOne()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => NumberReverseLookupSamples.RequireFractionRate(1.2m, "discountRate"));

        Assert.Equal("discountRate", exception.ParamName);
    }

    // テスト意図: Is Refund / Returns True / When Amount Is Negative を確認する。
    [Fact]
    public void IsRefund_ReturnsTrue_WhenAmountIsNegative()
    {
        var result = NumberReverseLookupSamples.IsRefund(-100m);

        Assert.True(result);
    }

    // テスト意図: Ensure Maximum Amount / Throws / When Amount Exceeds Maximum を確認する。
    [Fact]
    public void EnsureMaximumAmount_Throws_WhenAmountExceedsMaximum()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => NumberReverseLookupSamples.EnsureMaximumAmount(101m, 100m, "amount"));

        Assert.Equal("amount", exception.ParamName);
    }
}

