using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed partial class NumberReverseLookupSamplesTests
{
    [Fact]
    public void FormatThousands_UsesInvariantSeparators()
    {
        var result = NumberReverseLookupSamples.FormatThousands(1234.56m, decimalPlaces: 2);

        Assert.Equal("1,234.56", result);
    }

    [Fact]
    public void FormatFixedDecimal_AlwaysKeepsRequestedDecimalPlaces()
    {
        var result = NumberReverseLookupSamples.FormatFixedDecimal(12m, decimalPlaces: 2);

        Assert.Equal("12.00", result);
    }

    [Fact]
    public void FormatCurrencyCode_IncludesUppercaseCurrencyCode()
    {
        var result = NumberReverseLookupSamples.FormatCurrencyCode(1234m, "jpy", decimalPlaces: 0);

        Assert.Equal("JPY 1,234", result);
    }

    [Fact]
    public void TrimTrailingZeros_RemovesUnnecessaryDecimalZeros()
    {
        var result = NumberReverseLookupSamples.TrimTrailingZeros(12.3400m);

        Assert.Equal("12.34", result);
    }

    [Fact]
    public void FormatPercent_ConvertsRatioToPercentageText()
    {
        var result = NumberReverseLookupSamples.FormatPercent(0.1234m, decimalPlaces: 2);

        Assert.Equal("12.34%", result);
    }

    [Fact]
    public void FormatAccounting_UsesParenthesesForNegativeValues()
    {
        var result = NumberReverseLookupSamples.FormatAccounting(-1234.5m, decimalPlaces: 1);

        Assert.Equal("(1,234.5)", result);
    }

    [Fact]
    public void FormatWithUnit_AppendsUnitText()
    {
        var result = NumberReverseLookupSamples.FormatWithUnit(10m, "kg");

        Assert.Equal("10 kg", result);
    }

    [Fact]
    public void FormatFileSize_UsesBinaryUnits()
    {
        var result = NumberReverseLookupSamples.FormatFileSize(1536);

        Assert.Equal("1.5 KB", result);
    }

    [Fact]
    public void FormatDuration_UsesMillisecondsForShortDurations()
    {
        var result = NumberReverseLookupSamples.FormatDuration(TimeSpan.FromMilliseconds(12.345), decimalPlaces: 1);

        Assert.Equal("12.3 ms", result);
    }
}

