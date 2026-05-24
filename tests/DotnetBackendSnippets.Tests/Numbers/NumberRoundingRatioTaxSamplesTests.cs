using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed partial class NumberReverseLookupSamplesTests
{
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
}

