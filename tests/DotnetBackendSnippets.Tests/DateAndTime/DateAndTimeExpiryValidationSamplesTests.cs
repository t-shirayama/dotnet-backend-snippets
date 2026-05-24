using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed partial class DateAndTimeReverseLookupSamplesTests
{
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
}

