using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed partial class DateAndTimeReverseLookupSamplesTests
{
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
}

