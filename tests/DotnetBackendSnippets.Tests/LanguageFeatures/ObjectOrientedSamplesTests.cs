using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class ObjectOrientedSamplesTests
{
    [Fact]
    public void CalculateDiscountedAmount_UsesAbstractBaseClassOverride()
    {
        var policy = new PercentageDiscountPolicy(0.2m);

        var result = ObjectOrientedSamples.CalculateDiscountedAmount(100m, policy);

        Assert.Equal(80m, result);
    }

    [Fact]
    public void CalculateTotalWithTax_UsesDelegatedTaxCalculator()
    {
        var result = ObjectOrientedSamples.CalculateTotalWithTax(100m, new FixedRateTaxCalculator(0.1m));

        Assert.Equal(110m, result);
    }

    [Fact]
    public void Money_AddsOnlySameCurrencyValues()
    {
        var result = new Money(100m, "JPY") + new Money(50m, "jpy");

        Assert.Equal(new Money(150m, "JPY"), result);
    }

    [Fact]
    public void Money_Throws_WhenCurrenciesDiffer()
    {
        Assert.Throws<InvalidOperationException>(() => new Money(100m, "JPY") + new Money(1m, "USD"));
    }

    [Fact]
    public void HeaderBag_SupportsStringAndIntIndexers()
    {
        var headers = new HeaderBag();
        headers.Add("X-Request-Id", "abc");

        Assert.Equal("abc", headers["x-request-id"]);
        Assert.Equal(KeyValuePair.Create("X-Request-Id", "abc"), headers[0]);
    }
}
