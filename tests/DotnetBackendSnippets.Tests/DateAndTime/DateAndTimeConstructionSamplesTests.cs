using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed partial class DateAndTimeReverseLookupSamplesTests
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
}

