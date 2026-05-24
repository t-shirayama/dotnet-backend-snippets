using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Object Oriented Samples のスニペット動作を確認する。
public sealed class ObjectOrientedSamplesTests
{
    // テスト意図: Calculate Discounted Amount / Uses Abstract Base Class Override を確認する。
    [Fact]
    public void CalculateDiscountedAmount_UsesAbstractBaseClassOverride()
    {
        var policy = new PercentageDiscountPolicy(0.2m);

        var result = ObjectOrientedSamples.CalculateDiscountedAmount(100m, policy);

        Assert.Equal(80m, result);
    }

    // テスト意図: Calculate Total With Tax / Uses Delegated Tax Calculator を確認する。
    [Fact]
    public void CalculateTotalWithTax_UsesDelegatedTaxCalculator()
    {
        var result = ObjectOrientedSamples.CalculateTotalWithTax(100m, new FixedRateTaxCalculator(0.1m));

        Assert.Equal(110m, result);
    }

    // テスト意図: Money / Adds Only Same Currency Values を確認する。
    [Fact]
    public void Money_AddsOnlySameCurrencyValues()
    {
        var result = new Money(100m, "JPY") + new Money(50m, "jpy");

        Assert.Equal(new Money(150m, "JPY"), result);
    }

    // テスト意図: Money / Throws / When Currencies Differ を確認する。
    [Fact]
    public void Money_Throws_WhenCurrenciesDiffer()
    {
        Assert.Throws<InvalidOperationException>(() => new Money(100m, "JPY") + new Money(1m, "USD"));
    }

    // テスト意図: Header Bag / Supports String And Int Indexers を確認する。
    [Fact]
    public void HeaderBag_SupportsStringAndIntIndexers()
    {
        var headers = new HeaderBag();
        headers.Add("X-Request-Id", "abc");

        Assert.Equal("abc", headers["x-request-id"]);
        Assert.Equal(KeyValuePair.Create("X-Request-Id", "abc"), headers[0]);
    }

    // テスト意図: Header Bag / Rejects Blank Header Names を確認する。
    [Fact]
    public void HeaderBag_RejectsBlankHeaderNames()
    {
        var headers = new HeaderBag();

        Assert.Throws<ArgumentException>(() => headers.Add("", "abc"));
        Assert.Throws<ArgumentException>(() => headers[" "]);
    }
}
