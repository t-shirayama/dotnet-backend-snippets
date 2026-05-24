using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト補助: Number Reverse Lookup Samples の共有型を定義する。
public sealed partial class NumberReverseLookupSamplesTests
{
    // テスト意図: Round Bankers / Uses To Even Rounding を確認する。
    [Fact]
    public void RoundBankers_UsesToEvenRounding()
    {
        var result = NumberReverseLookupSamples.RoundBankers(2.5m, 0);

        Assert.Equal(2m, result);
    }

    // テスト意図: Round Away From Zero / Uses Away From Zero Rounding を確認する。
    [Fact]
    public void RoundAwayFromZero_UsesAwayFromZeroRounding()
    {
        var result = NumberReverseLookupSamples.RoundAwayFromZero(2.5m, 0);

        Assert.Equal(3m, result);
    }

    // テスト意図: Round To Unit / Rounds To Nearest Unit を確認する。
    [Theory]
    [InlineData(149, 100, 100)]
    [InlineData(150, 100, 200)]
    public void RoundToUnit_RoundsToNearestUnit(decimal value, decimal unit, decimal expected)
    {
        var result = NumberReverseLookupSamples.RoundToUnit(value, unit);

        Assert.Equal(expected, result);
    }

    // テスト意図: Ceiling To Unit / Rounds Up To Unit を確認する。
    [Fact]
    public void CeilingToUnit_RoundsUpToUnit()
    {
        var result = NumberReverseLookupSamples.CeilingToUnit(101m, 100m);

        Assert.Equal(200m, result);
    }

    // テスト意図: Floor To Unit / Rounds Down To Unit を確認する。
    [Fact]
    public void FloorToUnit_RoundsDownToUnit()
    {
        var result = NumberReverseLookupSamples.FloorToUnit(199m, 100m);

        Assert.Equal(100m, result);
    }

    // テスト意図: Calculate Change Rate / Returns Rounded Percentage を確認する。
    [Fact]
    public void CalculateChangeRate_ReturnsRoundedPercentage()
    {
        var result = NumberReverseLookupSamples.CalculateChangeRate(150m, 120m, decimalPlaces: 1);

        Assert.Equal(25.0m, result);
    }

    // テスト意図: Calculate Ratio / Returns Zero / When Whole Is Zero を確認する。
    [Fact]
    public void CalculateRatio_ReturnsZero_WhenWholeIsZero()
    {
        var result = NumberReverseLookupSamples.CalculateRatio(10m, 0m);

        Assert.Equal(0m, result);
    }

    // テスト意図: Calculate Composition Percentages / Adjusts Rounded Values To Total One Hundred を確認する。
    [Fact]
    public void CalculateCompositionPercentages_AdjustsRoundedValuesToTotalOneHundred()
    {
        var result = NumberReverseLookupSamples.CalculateCompositionPercentages([1m, 1m, 1m]);

        Assert.Equal([33.34m, 33.33m, 33.33m], result);
        Assert.Equal(100m, result.Sum());
    }

    // テスト意図: Apply Discount Rate / Returns Rounded Discounted Amount を確認する。
    [Fact]
    public void ApplyDiscountRate_ReturnsRoundedDiscountedAmount()
    {
        var result = NumberReverseLookupSamples.ApplyDiscountRate(1000m, 0.15m);

        Assert.Equal(850m, result);
    }

    // テスト意図: Calculate Profit Margin / Returns Rounded Margin Percentage を確認する。
    [Fact]
    public void CalculateProfitMargin_ReturnsRoundedMarginPercentage()
    {
        var result = NumberReverseLookupSamples.CalculateProfitMargin(1000m, 700m);

        Assert.Equal(30m, result);
    }

    // テスト意図: Calculate Tax From Net / Returns Tax Breakdown を確認する。
    [Fact]
    public void CalculateTaxFromNet_ReturnsTaxBreakdown()
    {
        var result = NumberReverseLookupSamples.CalculateTaxFromNet(100m, 0.1m);

        Assert.Equal(new NumberReverseLookupSamples.TaxBreakdown(100m, 10m, 110m), result);
    }

    // テスト意図: Calculate Tax From Gross / Returns Net And Tax Amount を確認する。
    [Fact]
    public void CalculateTaxFromGross_ReturnsNetAndTaxAmount()
    {
        var result = NumberReverseLookupSamples.CalculateTaxFromGross(110m, 0.1m);

        Assert.Equal(new NumberReverseLookupSamples.TaxBreakdown(100m, 10m, 110m), result);
    }

    // テスト意図: Calculate Total With Shipping / Adds Shipping And Rounds を確認する。
    [Fact]
    public void CalculateTotalWithShipping_AddsShippingAndRounds()
    {
        var result = NumberReverseLookupSamples.CalculateTotalWithShipping(1000.005m, 250m);

        Assert.Equal(1250.01m, result);
    }

    // テスト意図: Calculate Fee / Applies Minimum Fee を確認する。
    [Fact]
    public void CalculateFee_AppliesMinimumFee()
    {
        var result = NumberReverseLookupSamples.CalculateFee(1000m, 0.029m, fixedFee: 30m, minimumFee: 100m);

        Assert.Equal(new NumberReverseLookupSamples.FeeBreakdown(29m, 30m, 100m), result);
    }

    // テスト意図: Minor Currency Units / Round Trips Decimal Amount を確認する。
    [Fact]
    public void MinorCurrencyUnits_RoundTripsDecimalAmount()
    {
        var minorUnits = NumberReverseLookupSamples.ToMinorCurrencyUnits(12.345m);
        var amount = NumberReverseLookupSamples.FromMinorCurrencyUnits(minorUnits);

        Assert.Equal(1235, minorUnits);
        Assert.Equal(12.35m, amount);
    }

    // テスト意図: Convert Currency / Applies Rate And Rounding を確認する。
    [Fact]
    public void ConvertCurrency_AppliesRateAndRounding()
    {
        var result = NumberReverseLookupSamples.ConvertCurrency(100m, 1.2345m, decimalPlaces: 2);

        Assert.Equal(123.45m, result);
    }
}

