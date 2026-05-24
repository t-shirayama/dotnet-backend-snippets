using DotnetBackendSnippets.Numbers;

namespace DotnetBackendSnippets.Tests.Numbers;

// テスト補助: Number Reverse Lookup Samples の共有型を定義する。
public sealed partial class NumberReverseLookupSamplesTests
{
    // テスト意図: Calculate Skip / Returns Offset From One Based Page を確認する。
    [Fact]
    public void CalculateSkip_ReturnsOffsetFromOneBasedPage()
    {
        var result = NumberReverseLookupSamples.CalculateSkip(pageNumber: 3, pageSize: 20);

        Assert.Equal(40, result);
    }

    // テスト意図: Calculate Total Pages / Rounds Up Partial Page を確認する。
    [Fact]
    public void CalculateTotalPages_RoundsUpPartialPage()
    {
        var result = NumberReverseLookupSamples.CalculateTotalPages(totalCount: 101, pageSize: 20);

        Assert.Equal(6, result);
    }

    // テスト意図: Is Last Page / Checks Page Against Total Count を確認する。
    [Theory]
    [InlineData(5, 95, 20, true)]
    [InlineData(4, 95, 20, false)]
    public void IsLastPage_ChecksPageAgainstTotalCount(int pageNumber, int totalCount, int pageSize, bool expected)
    {
        var result = NumberReverseLookupSamples.IsLastPage(pageNumber, totalCount, pageSize);

        Assert.Equal(expected, result);
    }

    // テスト意図: To Offset Limit / Returns Offset Limit Pair を確認する。
    [Fact]
    public void ToOffsetLimit_ReturnsOffsetLimitPair()
    {
        var result = NumberReverseLookupSamples.ToOffsetLimit(pageNumber: 2, pageSize: 50);

        Assert.Equal(new NumberReverseLookupSamples.OffsetLimit(50, 50), result);
    }

    // テスト意図: Get Display Range / Returns One Based Range For Current Page を確認する。
    [Fact]
    public void GetDisplayRange_ReturnsOneBasedRangeForCurrentPage()
    {
        var result = NumberReverseLookupSamples.GetDisplayRange(pageNumber: 3, pageSize: 20, totalCount: 55);

        Assert.Equal(new NumberReverseLookupSamples.DisplayRange(41, 55), result);
    }

    // テスト意図: Clamp Page Size / Returns Default For Invalid Page Size を確認する。
    [Fact]
    public void ClampPageSize_ReturnsDefaultForInvalidPageSize()
    {
        var result = NumberReverseLookupSamples.ClampPageSize(pageSize: 0, maximumPageSize: 100, defaultPageSize: 20);

        Assert.Equal(20, result);
    }
}

