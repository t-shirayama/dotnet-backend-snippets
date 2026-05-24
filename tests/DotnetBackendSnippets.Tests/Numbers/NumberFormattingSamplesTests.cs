using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト補助: Number Reverse Lookup Samples の共有型を定義する。
public sealed partial class NumberReverseLookupSamplesTests
{
    // テスト意図: Format Thousands / Uses Invariant Separators を確認する。
    [Fact]
    public void FormatThousands_UsesInvariantSeparators()
    {
        var result = NumberReverseLookupSamples.FormatThousands(1234.56m, decimalPlaces: 2);

        Assert.Equal("1,234.56", result);
    }

    // テスト意図: Format Fixed Decimal / Always Keeps Requested Decimal Places を確認する。
    [Fact]
    public void FormatFixedDecimal_AlwaysKeepsRequestedDecimalPlaces()
    {
        var result = NumberReverseLookupSamples.FormatFixedDecimal(12m, decimalPlaces: 2);

        Assert.Equal("12.00", result);
    }

    // テスト意図: Format Currency Code / Includes Uppercase Currency Code を確認する。
    [Fact]
    public void FormatCurrencyCode_IncludesUppercaseCurrencyCode()
    {
        var result = NumberReverseLookupSamples.FormatCurrencyCode(1234m, "jpy", decimalPlaces: 0);

        Assert.Equal("JPY 1,234", result);
    }

    // テスト意図: Trim Trailing Zeros / Removes Unnecessary Decimal Zeros を確認する。
    [Fact]
    public void TrimTrailingZeros_RemovesUnnecessaryDecimalZeros()
    {
        var result = NumberReverseLookupSamples.TrimTrailingZeros(12.3400m);

        Assert.Equal("12.34", result);
    }

    // テスト意図: Format Percent / Converts Ratio To Percentage Text を確認する。
    [Fact]
    public void FormatPercent_ConvertsRatioToPercentageText()
    {
        var result = NumberReverseLookupSamples.FormatPercent(0.1234m, decimalPlaces: 2);

        Assert.Equal("12.34%", result);
    }

    // テスト意図: Format Accounting / Uses Parentheses For Negative Values を確認する。
    [Fact]
    public void FormatAccounting_UsesParenthesesForNegativeValues()
    {
        var result = NumberReverseLookupSamples.FormatAccounting(-1234.5m, decimalPlaces: 1);

        Assert.Equal("(1,234.5)", result);
    }

    // テスト意図: Format With Unit / Appends Unit Text を確認する。
    [Fact]
    public void FormatWithUnit_AppendsUnitText()
    {
        var result = NumberReverseLookupSamples.FormatWithUnit(10m, "kg");

        Assert.Equal("10 kg", result);
    }

    // テスト意図: Format File Size / Uses Binary Units を確認する。
    [Fact]
    public void FormatFileSize_UsesBinaryUnits()
    {
        var result = NumberReverseLookupSamples.FormatFileSize(1536);

        Assert.Equal("1.5 KB", result);
    }

    // テスト意図: Format Duration / Uses Milliseconds For Short Durations を確認する。
    [Fact]
    public void FormatDuration_UsesMillisecondsForShortDurations()
    {
        var result = NumberReverseLookupSamples.FormatDuration(TimeSpan.FromMilliseconds(12.345), decimalPlaces: 1);

        Assert.Equal("12.3 ms", result);
    }
}

