using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed partial class NumberReverseLookupSamplesTests
{
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
}

