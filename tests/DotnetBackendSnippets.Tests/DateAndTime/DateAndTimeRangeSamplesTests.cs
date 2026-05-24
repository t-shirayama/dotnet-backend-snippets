using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed partial class DateAndTimeReverseLookupSamplesTests
{
    [Fact]
    public void SingleDayRange_ReturnsHalfOpenRange()
    {
        var result = DateAndTimeReverseLookupSamples.SingleDayRange(new DateOnly(2026, 5, 24));

        Assert.Equal(new DateOnly(2026, 5, 24), result.StartInclusive);
        Assert.Equal(new DateOnly(2026, 5, 25), result.EndExclusive);
    }

    [Fact]
    public void MonthRange_ReturnsMonthStartAndNextMonthStart()
    {
        var result = DateAndTimeReverseLookupSamples.MonthRange(new DateOnly(2024, 2, 15));

        Assert.Equal(new DateOnly(2024, 2, 1), result.StartInclusive);
        Assert.Equal(new DateOnly(2024, 3, 1), result.EndExclusive);
    }

    [Fact]
    public void QuarterRange_ReturnsQuarterStartAndNextQuarterStart()
    {
        var result = DateAndTimeReverseLookupSamples.QuarterRange(new DateOnly(2026, 5, 24));

        Assert.Equal(new DateOnly(2026, 4, 1), result.StartInclusive);
        Assert.Equal(new DateOnly(2026, 7, 1), result.EndExclusive);
    }

    [Theory]
    [InlineData(2026, 3, 31, 4, 2025, 4, 1)]
    [InlineData(2026, 4, 1, 4, 2026, 4, 1)]
    public void StartOfFiscalYear_UsesConfiguredStartMonth(
        int year,
        int month,
        int day,
        int fiscalYearStartMonth,
        int expectedYear,
        int expectedMonth,
        int expectedDay)
    {
        var result = DateAndTimeReverseLookupSamples.StartOfFiscalYear(new DateOnly(year, month, day), fiscalYearStartMonth);

        Assert.Equal(new DateOnly(expectedYear, expectedMonth, expectedDay), result);
    }

    [Fact]
    public void StartOfWeek_UsesRequestedFirstDay()
    {
        var result = DateAndTimeReverseLookupSamples.StartOfWeek(new DateOnly(2026, 5, 24), DayOfWeek.Monday);

        Assert.Equal(new DateOnly(2026, 5, 18), result);
    }

    [Fact]
    public void SearchRangeForInclusiveDates_UsesCustomLocalDayStart()
    {
        var timeZone = GetTokyoTimeZone();

        var result = DateAndTimeReverseLookupSamples.SearchRangeForInclusiveDates(
            new DateOnly(2026, 5, 24),
            new DateOnly(2026, 5, 24),
            new TimeOnly(6, 0),
            timeZone);

        Assert.Equal(new DateTimeOffset(2026, 5, 23, 21, 0, 0, TimeSpan.Zero), result.StartInclusive);
        Assert.Equal(new DateTimeOffset(2026, 5, 24, 21, 0, 0, TimeSpan.Zero), result.EndExclusive);
    }
}

