using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

public sealed partial class NumberReverseLookupSamplesTests
{
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
}

