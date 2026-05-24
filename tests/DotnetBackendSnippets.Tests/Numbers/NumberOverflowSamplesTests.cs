using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト補助: Number Reverse Lookup Samples の共有型を定義する。
public sealed partial class NumberReverseLookupSamplesTests
{
    // テスト意図: Try Add Int32 / Returns False / When Overflow Would Occur を確認する。
    [Fact]
    public void TryAddInt32_ReturnsFalse_WhenOverflowWouldOccur()
    {
        var added = NumberReverseLookupSamples.TryAddInt32(int.MaxValue, 1, out var result);

        Assert.False(added);
        Assert.Equal(0, result);
    }

    // テスト意図: Try Multiply Int32 / Returns False / When Overflow Would Occur を確認する。
    [Fact]
    public void TryMultiplyInt32_ReturnsFalse_WhenOverflowWouldOccur()
    {
        var multiplied = NumberReverseLookupSamples.TryMultiplyInt32(int.MaxValue, 2, out var result);

        Assert.False(multiplied);
        Assert.Equal(0, result);
    }

    // テスト意図: Try Multiply Decimal / Returns False / When Overflow Would Occur を確認する。
    [Fact]
    public void TryMultiplyDecimal_ReturnsFalse_WhenOverflowWouldOccur()
    {
        var multiplied = NumberReverseLookupSamples.TryMultiplyDecimal(decimal.MaxValue, 2m, out var result);

        Assert.False(multiplied);
        Assert.Equal(0m, result);
    }

    // テスト意図: Can Multiply Without Exceeding / Returns False / When Result Exceeds Limit を確認する。
    [Fact]
    public void CanMultiplyWithoutExceeding_ReturnsFalse_WhenResultExceedsLimit()
    {
        var result = NumberReverseLookupSamples.CanMultiplyWithoutExceeding(11m, 10m, maximumAbsoluteValue: 100m);

        Assert.False(result);
    }

    // テスト意図: Can Multiply Without Exceeding / Handles Minimum Decimal Without Abs Overflow を確認する。
    [Fact]
    public void CanMultiplyWithoutExceeding_HandlesMinimumDecimalWithoutAbsOverflow()
    {
        var result = NumberReverseLookupSamples.CanMultiplyWithoutExceeding(decimal.MinValue, 1m, maximumAbsoluteValue: 0m);

        Assert.False(result);
    }

    // テスト意図: Big Multiply / Returns Long Result を確認する。
    [Fact]
    public void BigMultiply_ReturnsLongResult()
    {
        var result = NumberReverseLookupSamples.BigMultiply(int.MaxValue, 2);

        Assert.Equal(4294967294L, result);
    }

    // テスト意図: Is Finite / Detects Nan And Infinity を確認する。
    [Theory]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(1.23d, true)]
    public void IsFinite_DetectsNanAndInfinity(double value, bool expected)
    {
        var result = NumberReverseLookupSamples.IsFinite(value);

        Assert.Equal(expected, result);
    }
}
