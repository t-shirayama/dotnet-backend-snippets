using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

// テスト補助: Date And Time Reverse Lookup Samples の共有 fixture を定義する。
public sealed partial class DateAndTimeReverseLookupSamplesTests
{
    // テスト意図: Business Day Helpers / Skip Weekends And Holidays を確認する。
    [Fact]
    public void BusinessDayHelpers_SkipWeekendsAndHolidays()
    {
        var holidays = new HashSet<DateOnly> { new(2026, 5, 25) };

        Assert.False(DateAndTimeReverseLookupSamples.IsBusinessDay(new DateOnly(2026, 5, 25), holidays));
        Assert.Equal(new DateOnly(2026, 5, 26), DateAndTimeReverseLookupSamples.NextBusinessDay(new DateOnly(2026, 5, 22), holidays));
        Assert.Equal(new DateOnly(2026, 5, 22), DateAndTimeReverseLookupSamples.PreviousBusinessDay(new DateOnly(2026, 5, 26), holidays));
    }

    // テスト意図: Add Business Days / Supports Forward And Backward Calculation を確認する。
    [Fact]
    public void AddBusinessDays_SupportsForwardAndBackwardCalculation()
    {
        var holidays = new HashSet<DateOnly> { new(2026, 5, 25) };

        Assert.Equal(new DateOnly(2026, 5, 27), DateAndTimeReverseLookupSamples.AddBusinessDays(new DateOnly(2026, 5, 22), 2, holidays));
        Assert.Equal(new DateOnly(2026, 5, 22), DateAndTimeReverseLookupSamples.AddBusinessDays(new DateOnly(2026, 5, 27), -2, holidays));
    }

    // テスト意図: Count Business Days / Counts Half Open Range を確認する。
    [Fact]
    public void CountBusinessDays_CountsHalfOpenRange()
    {
        var holidays = new HashSet<DateOnly> { new(2026, 5, 25) };

        var result = DateAndTimeReverseLookupSamples.CountBusinessDays(
            new DateOnly(2026, 5, 22),
            new DateOnly(2026, 5, 29),
            holidays);

        Assert.Equal(4, result);
    }
}

