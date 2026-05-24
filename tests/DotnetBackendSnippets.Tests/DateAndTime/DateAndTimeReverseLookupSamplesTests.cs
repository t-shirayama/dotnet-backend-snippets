using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed class DateAndTimeReverseLookupSamplesTests
{
    [Fact]
    public void ToDateOnly_ReturnsDatePart()
    {
        var result = DateAndTimeReverseLookupSamples.ToDateOnly(new DateTime(2026, 5, 24, 23, 59, 59));

        Assert.Equal(new DateOnly(2026, 5, 24), result);
    }

    [Fact]
    public void CombineDateAndTime_AppliesRequestedKind()
    {
        var result = DateAndTimeReverseLookupSamples.CombineDateAndTime(
            new DateOnly(2026, 5, 24),
            new TimeOnly(9, 30),
            DateTimeKind.Utc);

        Assert.Equal(new DateTime(2026, 5, 24, 9, 30, 0, DateTimeKind.Utc), result);
    }

    [Fact]
    public void CurrentDate_UsesTimeProviderAndTimeZone()
    {
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 5, 24, 15, 30, 0, TimeSpan.Zero));

        var result = DateAndTimeReverseLookupSamples.CurrentDate(timeProvider, GetTokyoTimeZone());

        Assert.Equal(new DateOnly(2026, 5, 25), result);
    }

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
    public void BusinessDayHelpers_SkipWeekendsAndHolidays()
    {
        var holidays = new HashSet<DateOnly> { new(2026, 5, 25) };

        Assert.False(DateAndTimeReverseLookupSamples.IsBusinessDay(new DateOnly(2026, 5, 25), holidays));
        Assert.Equal(new DateOnly(2026, 5, 26), DateAndTimeReverseLookupSamples.NextBusinessDay(new DateOnly(2026, 5, 22), holidays));
        Assert.Equal(new DateOnly(2026, 5, 22), DateAndTimeReverseLookupSamples.PreviousBusinessDay(new DateOnly(2026, 5, 26), holidays));
    }

    [Fact]
    public void AddBusinessDays_SupportsForwardAndBackwardCalculation()
    {
        var holidays = new HashSet<DateOnly> { new(2026, 5, 25) };

        Assert.Equal(new DateOnly(2026, 5, 27), DateAndTimeReverseLookupSamples.AddBusinessDays(new DateOnly(2026, 5, 22), 2, holidays));
        Assert.Equal(new DateOnly(2026, 5, 22), DateAndTimeReverseLookupSamples.AddBusinessDays(new DateOnly(2026, 5, 27), -2, holidays));
    }

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

    [Fact]
    public void LocalDateRangeToUtcRange_ConvertsInclusiveLocalDatesToUtcHalfOpenRange()
    {
        var timeZone = GetTokyoTimeZone();

        var result = DateAndTimeReverseLookupSamples.LocalDateRangeToUtcRange(
            new DateOnly(2026, 5, 24),
            new DateOnly(2026, 5, 24),
            timeZone);

        Assert.Equal(new DateTimeOffset(2026, 5, 23, 15, 0, 0, TimeSpan.Zero), result.StartInclusive);
        Assert.Equal(new DateTimeOffset(2026, 5, 24, 15, 0, 0, TimeSpan.Zero), result.EndExclusive);
    }

    [Fact]
    public void NormalizeToUtc_PreservesInstant()
    {
        var result = DateAndTimeReverseLookupSamples.NormalizeToUtc(new DateTimeOffset(2026, 5, 24, 9, 0, 0, TimeSpan.FromHours(9)));

        Assert.Equal(new DateTimeOffset(2026, 5, 24, 0, 0, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void UtcToLocalDate_ReturnsDateInRequestedTimeZone()
    {
        var timeZone = GetTokyoTimeZone();

        var result = DateAndTimeReverseLookupSamples.UtcToLocalDate(
            new DateTimeOffset(2026, 5, 24, 15, 30, 0, TimeSpan.Zero),
            timeZone);

        Assert.Equal(new DateOnly(2026, 5, 25), result);
    }

    [Fact]
    public void ExpirationHelpers_UseNowPassedByCaller()
    {
        var now = new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);
        var expiresAt = now.AddMinutes(30);

        Assert.False(DateAndTimeReverseLookupSamples.IsExpired(expiresAt, now));
        Assert.Equal(TimeSpan.FromMinutes(30), DateAndTimeReverseLookupSamples.RemainingTime(expiresAt, now));
        Assert.Equal(TimeSpan.Zero, DateAndTimeReverseLookupSamples.RemainingTime(expiresAt, expiresAt.AddSeconds(1)));
    }

    [Fact]
    public void IsWithinPeriod_UsesHalfOpenRange()
    {
        var validFrom = new DateTimeOffset(2026, 5, 24, 9, 0, 0, TimeSpan.Zero);
        var validUntil = new DateTimeOffset(2026, 5, 24, 18, 0, 0, TimeSpan.Zero);

        Assert.True(DateAndTimeReverseLookupSamples.IsWithinPeriod(validFrom, validFrom, validUntil));
        Assert.False(DateAndTimeReverseLookupSamples.IsWithinPeriod(validUntil, validFrom, validUntil));
    }

    [Theory]
    [InlineData("2026-05-24", true, 2026, 5, 24)]
    [InlineData("05/24/2026", false, 1, 1, 1)]
    public void TryParseIsoDate_AcceptsOnlyFixedFormat(string value, bool expected, int year, int month, int day)
    {
        var result = DateAndTimeReverseLookupSamples.TryParseIsoDate(value, out var date);

        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.Equal(new DateOnly(year, month, day), date);
        }
    }

    [Theory]
    [InlineData("09:30", true, 9, 30)]
    [InlineData("9:30", false, 0, 0)]
    public void TryParseHourMinute_AcceptsOnlyFixedFormat(string value, bool expected, int hour, int minute)
    {
        var result = DateAndTimeReverseLookupSamples.TryParseHourMinute(value, out var time);

        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.Equal(new TimeOnly(hour, minute), time);
        }
    }

    [Fact]
    public void TryFindTimeZone_ReturnsFalseForUnknownId()
    {
        var result = DateAndTimeReverseLookupSamples.TryFindTimeZone("Unknown/TimeZone", out var timeZone);

        Assert.False(result);
        Assert.Null(timeZone);
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

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return utcNow;
        }
    }

    private static TimeZoneInfo GetTokyoTimeZone()
    {
        foreach (var id in new[] { "Tokyo Standard Time", "Asia/Tokyo" })
        {
            if (DateAndTimeReverseLookupSamples.TryFindTimeZone(id, out var timeZone))
            {
                return timeZone!;
            }
        }

        throw new InvalidOperationException("Tokyo time zone was not found.");
    }
}
