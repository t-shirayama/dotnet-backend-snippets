using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト補助: Number Reverse Lookup Samples の共有型を定義する。
public sealed partial class NumberReverseLookupSamplesTests
{
    // テスト意図: Are Nearly Equal / Accepts Floating Point Rounding Difference を確認する。
    [Fact]
    public void AreNearlyEqual_AcceptsFloatingPointRoundingDifference()
    {
        var left = 0.1d + 0.2d;

        bool result = NumberReverseLookupSamples.AreNearlyEqual(left, 0.3d);

        Assert.True(result);
        Assert.False(left == 0.3d);
    }

    // テスト意図: Are Nearly Equal / Uses Relative Tolerance For Large Values を確認する。
    [Fact]
    public void AreNearlyEqual_UsesRelativeToleranceForLargeValues()
    {
        bool result = NumberReverseLookupSamples.AreNearlyEqual(
            1_000_000_000d,
            1_000_000_001d,
            absoluteTolerance: 0d,
            relativeTolerance: 1e-8);

        Assert.True(result);
    }

    // テスト意図: Are Nearly Equal / Rejects NaN を確認する。
    [Fact]
    public void AreNearlyEqual_RejectsNaN()
    {
        bool result = NumberReverseLookupSamples.AreNearlyEqual(double.NaN, double.NaN);

        Assert.False(result);
    }

    // テスト意図: Are Nearly Equal / Rejects Different Infinity Values を確認する。
    [Fact]
    public void AreNearlyEqual_RejectsDifferentInfinityValues()
    {
        bool result = NumberReverseLookupSamples.AreNearlyEqual(double.PositiveInfinity, double.MaxValue);

        Assert.False(result);
    }

    // テスト意図: Is Nearly Zero / Uses Tolerance を確認する。
    [Theory]
    [InlineData(1e-13, true)]
    [InlineData(1e-6, false)]
    public void IsNearlyZero_UsesTolerance(double value, bool expected)
    {
        Assert.Equal(expected, NumberReverseLookupSamples.IsNearlyZero(value));
    }

    // テスト意図: Divide Or Default / Returns Default / When Denominator Is Nearly Zero を確認する。
    [Fact]
    public void DivideOrDefault_ReturnsDefault_WhenDenominatorIsNearlyZero()
    {
        double result = NumberReverseLookupSamples.DivideOrDefault(10d, 1e-13, defaultValue: -1d);

        Assert.Equal(-1d, result);
    }

    // テスト意図: Sum With Compensation / Reduces Cumulative Error を確認する。
    [Fact]
    public void SumWithCompensation_ReducesCumulativeError()
    {
        double naive = new[] { 1e16, 1d, -1e16 }.Sum();

        double compensated = NumberReverseLookupSamples.SumWithCompensation([1e16, 1d, -1e16]);

        Assert.Equal(0d, naive);
        Assert.Equal(1d, compensated);
    }

    // テスト意図: Sum With Compensation / Keeps Small Repeated Values Close To Expected を確認する。
    [Fact]
    public void SumWithCompensation_KeepsSmallRepeatedValuesCloseToExpected()
    {
        var values = Enumerable.Repeat(0.1d, 10);

        double result = NumberReverseLookupSamples.SumWithCompensation(values);

        Assert.True(NumberReverseLookupSamples.AreNearlyEqual(result, 1d));
    }

    // テスト意図: Round To Significant Digits / Rounds By Significant Digits を確認する。
    [Theory]
    [InlineData(12345.6789, 3, 12300d)]
    [InlineData(0.012345, 2, 0.012d)]
    [InlineData(-9876.5, 2, -9900d)]
    public void RoundToSignificantDigits_RoundsBySignificantDigits(double value, int significantDigits, double expected)
    {
        double result = NumberReverseLookupSamples.RoundToSignificantDigits(value, significantDigits);

        Assert.Equal(expected, result, precision: 12);
    }

    // テスト意図: Compare Double And Decimal Addition / Shows Decimal Exactness For Base Ten を確認する。
    [Fact]
    public void CompareDoubleAndDecimalAddition_ShowsDecimalExactnessForBaseTen()
    {
        var result = NumberReverseLookupSamples.CompareDoubleAndDecimalAddition();

        Assert.False(result.DoubleEqualsExpected);
        Assert.True(result.DecimalEqualsExpected);
        Assert.True(NumberReverseLookupSamples.AreNearlyEqual(result.DoubleSum, 0.3d));
        Assert.Equal(0.3m, result.DecimalSum);
    }
}
