using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed partial class DateAndTimeReverseLookupSamplesTests
{
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
    public void TryFindTimeZone_ReturnsFalseForUnknownId()
    {
        var result = DateAndTimeReverseLookupSamples.TryFindTimeZone("Unknown/TimeZone", out var timeZone);

        Assert.False(result);
        Assert.Null(timeZone);
    }
}

