using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class TupleSamplesTests
{
    [Fact]
    public void TrySplitFullName_ReturnsNamedTuple()
    {
        var result = TupleSamples.TrySplitFullName("Ada Lovelace");

        Assert.True(result.Success);
        Assert.Equal("Ada", result.FirstName);
        Assert.Equal("Lovelace", result.LastName);
    }

    [Fact]
    public void CalculateSummary_ReturnsMultipleValues()
    {
        var (min, max, average) = TupleSamples.CalculateSummary([2, 4, 9]);

        Assert.Equal(2, min);
        Assert.Equal(9, max);
        Assert.Equal(5m, average);
    }

    [Fact]
    public void ToRangeRecord_ConvertsTupleToNamedDomainRecord()
    {
        var result = TupleSamples.ToRangeRecord((Min: 1, Max: 5));

        Assert.Equal(new NumericRange(1, 5), result);
    }
}
