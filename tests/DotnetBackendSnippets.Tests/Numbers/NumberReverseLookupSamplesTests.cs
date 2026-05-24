using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed class NumberReverseLookupSamplesTests
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
    public void RoundBankers_UsesToEvenRounding()
    {
        var result = NumberReverseLookupSamples.RoundBankers(2.5m, 0);

        Assert.Equal(2m, result);
    }

    [Fact]
    public void RoundAwayFromZero_UsesAwayFromZeroRounding()
    {
        var result = NumberReverseLookupSamples.RoundAwayFromZero(2.5m, 0);

        Assert.Equal(3m, result);
    }

    [Theory]
    [InlineData(149, 100, 100)]
    [InlineData(150, 100, 200)]
    public void RoundToUnit_RoundsToNearestUnit(decimal value, decimal unit, decimal expected)
    {
        var result = NumberReverseLookupSamples.RoundToUnit(value, unit);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CeilingToUnit_RoundsUpToUnit()
    {
        var result = NumberReverseLookupSamples.CeilingToUnit(101m, 100m);

        Assert.Equal(200m, result);
    }

    [Fact]
    public void FloorToUnit_RoundsDownToUnit()
    {
        var result = NumberReverseLookupSamples.FloorToUnit(199m, 100m);

        Assert.Equal(100m, result);
    }

    [Fact]
    public void CalculateChangeRate_ReturnsRoundedPercentage()
    {
        var result = NumberReverseLookupSamples.CalculateChangeRate(150m, 120m, decimalPlaces: 1);

        Assert.Equal(25.0m, result);
    }

    [Fact]
    public void CalculateRatio_ReturnsZero_WhenWholeIsZero()
    {
        var result = NumberReverseLookupSamples.CalculateRatio(10m, 0m);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateCompositionPercentages_AdjustsRoundedValuesToTotalOneHundred()
    {
        var result = NumberReverseLookupSamples.CalculateCompositionPercentages([1m, 1m, 1m]);

        Assert.Equal([33.34m, 33.33m, 33.33m], result);
        Assert.Equal(100m, result.Sum());
    }

    [Fact]
    public void ApplyDiscountRate_ReturnsRoundedDiscountedAmount()
    {
        var result = NumberReverseLookupSamples.ApplyDiscountRate(1000m, 0.15m);

        Assert.Equal(850m, result);
    }

    [Fact]
    public void CalculateProfitMargin_ReturnsRoundedMarginPercentage()
    {
        var result = NumberReverseLookupSamples.CalculateProfitMargin(1000m, 700m);

        Assert.Equal(30m, result);
    }

    [Fact]
    public void CalculateTaxFromNet_ReturnsTaxBreakdown()
    {
        var result = NumberReverseLookupSamples.CalculateTaxFromNet(100m, 0.1m);

        Assert.Equal(new NumberReverseLookupSamples.TaxBreakdown(100m, 10m, 110m), result);
    }

    [Fact]
    public void CalculateTaxFromGross_ReturnsNetAndTaxAmount()
    {
        var result = NumberReverseLookupSamples.CalculateTaxFromGross(110m, 0.1m);

        Assert.Equal(new NumberReverseLookupSamples.TaxBreakdown(100m, 10m, 110m), result);
    }

    [Fact]
    public void CalculateTotalWithShipping_AddsShippingAndRounds()
    {
        var result = NumberReverseLookupSamples.CalculateTotalWithShipping(1000.005m, 250m);

        Assert.Equal(1250.01m, result);
    }

    [Fact]
    public void CalculateFee_AppliesMinimumFee()
    {
        var result = NumberReverseLookupSamples.CalculateFee(1000m, 0.029m, fixedFee: 30m, minimumFee: 100m);

        Assert.Equal(new NumberReverseLookupSamples.FeeBreakdown(29m, 30m, 100m), result);
    }

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
    public void MinorCurrencyUnits_RoundTripsDecimalAmount()
    {
        var minorUnits = NumberReverseLookupSamples.ToMinorCurrencyUnits(12.345m);
        var amount = NumberReverseLookupSamples.FromMinorCurrencyUnits(minorUnits);

        Assert.Equal(1235, minorUnits);
        Assert.Equal(12.35m, amount);
    }

    [Fact]
    public void ConvertCurrency_AppliesRateAndRounding()
    {
        var result = NumberReverseLookupSamples.ConvertCurrency(100m, 1.2345m, decimalPlaces: 2);

        Assert.Equal(123.45m, result);
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

    [Fact]
    public void CalculateSkip_ReturnsOffsetFromOneBasedPage()
    {
        var result = NumberReverseLookupSamples.CalculateSkip(pageNumber: 3, pageSize: 20);

        Assert.Equal(40, result);
    }

    [Fact]
    public void CalculateTotalPages_RoundsUpPartialPage()
    {
        var result = NumberReverseLookupSamples.CalculateTotalPages(totalCount: 101, pageSize: 20);

        Assert.Equal(6, result);
    }

    [Theory]
    [InlineData(5, 95, 20, true)]
    [InlineData(4, 95, 20, false)]
    public void IsLastPage_ChecksPageAgainstTotalCount(int pageNumber, int totalCount, int pageSize, bool expected)
    {
        var result = NumberReverseLookupSamples.IsLastPage(pageNumber, totalCount, pageSize);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToOffsetLimit_ReturnsOffsetLimitPair()
    {
        var result = NumberReverseLookupSamples.ToOffsetLimit(pageNumber: 2, pageSize: 50);

        Assert.Equal(new NumberReverseLookupSamples.OffsetLimit(50, 50), result);
    }

    [Fact]
    public void GetDisplayRange_ReturnsOneBasedRangeForCurrentPage()
    {
        var result = NumberReverseLookupSamples.GetDisplayRange(pageNumber: 3, pageSize: 20, totalCount: 55);

        Assert.Equal(new NumberReverseLookupSamples.DisplayRange(41, 55), result);
    }

    [Fact]
    public void ClampPageSize_ReturnsDefaultForInvalidPageSize()
    {
        var result = NumberReverseLookupSamples.ClampPageSize(pageSize: 0, maximumPageSize: 100, defaultPageSize: 20);

        Assert.Equal(20, result);
    }

    [Fact]
    public void SumAmounts_UsesCheckedDecimalAddition()
    {
        var result = NumberReverseLookupSamples.SumAmounts([100m, 250m, -50m]);

        Assert.Equal(300m, result);
    }

    [Fact]
    public void AverageOrDefault_ReturnsDefault_WhenCollectionIsEmpty()
    {
        var result = NumberReverseLookupSamples.AverageOrDefault([], defaultValue: -1m);

        Assert.Equal(-1m, result);
    }

    [Fact]
    public void MinMaxOrNull_ReturnsMinimumAndMaximum()
    {
        var result = NumberReverseLookupSamples.MinMaxOrNull([3m, 1m, 2m]);

        Assert.Equal(new NumberReverseLookupSamples.DecimalRange(1m, 3m), result);
    }

    [Fact]
    public void SumByCategory_GroupsAmountsCaseInsensitively()
    {
        SaleLine[] lines =
        [
            new("Books", 100m),
            new("books", 250m),
            new("Food", 50m),
        ];

        var result = NumberReverseLookupSamples.SumByCategory(lines, line => line.Category, line => line.Amount);

        Assert.Equal(350m, result["Books"]);
        Assert.Equal(50m, result["Food"]);
    }

    [Fact]
    public void WeightedAverage_ReturnsWeightedValue()
    {
        (decimal Value, decimal Weight)[] values = [(80m, 1m), (100m, 3m)];

        var result = NumberReverseLookupSamples.WeightedAverage(values);

        Assert.Equal(95m, result);
    }

    [Fact]
    public void Median_ReturnsMiddleValue()
    {
        var result = NumberReverseLookupSamples.Median([30m, 10m, 20m]);

        Assert.Equal(20m, result);
    }

    [Fact]
    public void Percentile_InterpolatesBetweenSortedValues()
    {
        var result = NumberReverseLookupSamples.Percentile([10m, 20m, 30m, 40m], 75m);

        Assert.Equal(32.5m, result);
    }

    [Fact]
    public void AverageWithoutOutliers_ExcludesValuesOutsideRange()
    {
        var result = NumberReverseLookupSamples.AverageWithoutOutliers([10m, 12m, 100m], 0m, 50m);

        Assert.Equal(11m, result);
    }

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

    private sealed record SaleLine(string Category, decimal Amount);
}
