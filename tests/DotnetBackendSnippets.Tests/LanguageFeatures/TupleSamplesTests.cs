using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Tuple Samples のスニペット動作を確認する。
public sealed class TupleSamplesTests
{
    // テスト意図: Try Split Full Name / Returns Named Tuple を確認する。
    [Fact]
    public void TrySplitFullName_ReturnsNamedTuple()
    {
        var result = TupleSamples.TrySplitFullName("Ada Lovelace");

        Assert.True(result.Success);
        Assert.Equal("Ada", result.FirstName);
        Assert.Equal("Lovelace", result.LastName);
    }

    // テスト意図: Calculate Summary / Returns Multiple Values を確認する。
    [Fact]
    public void CalculateSummary_ReturnsMultipleValues()
    {
        var (min, max, average) = TupleSamples.CalculateSummary([2, 4, 9]);

        Assert.Equal(2, min);
        Assert.Equal(9, max);
        Assert.Equal(5m, average);
    }

    // テスト意図: Calculate Summary / Returns Decimal Average / Without Floating Point Rounding を確認する。
    [Fact]
    public void CalculateSummary_ReturnsDecimalAverage_WithoutFloatingPointRounding()
    {
        var result = TupleSamples.CalculateSummary([1, 2]);

        Assert.Equal(1.5m, result.Average);
    }

    // テスト意図: To Range Record / Converts Tuple To Named Domain Record を確認する。
    [Fact]
    public void ToRangeRecord_ConvertsTupleToNamedDomainRecord()
    {
        var result = TupleSamples.ToRangeRecord((Min: 1, Max: 5));

        Assert.Equal(new NumericRange(1, 5), result);
    }
}
