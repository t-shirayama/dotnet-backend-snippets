using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

public sealed class DateAndTimeSamplesTests
{
    [Fact]
    public void StartOfDay_ReturnsMidnight()
    {
        var result = DateAndTimeSamples.StartOfDay(new DateTime(2026, 5, 24, 13, 45, 10));

        Assert.Equal(new DateTime(2026, 5, 24), result);
    }

    [Fact]
    public void EndOfDay_ReturnsLastTickOfDay()
    {
        var result = DateAndTimeSamples.EndOfDay(new DateTime(2026, 5, 24, 13, 45, 10));

        Assert.Equal(new DateTime(2026, 5, 24, 23, 59, 59, 999).AddTicks(9999), result);
    }

    [Fact]
    public void StartOfMonthAndEndOfMonth_HandleMonthEnd()
    {
        var value = new DateOnly(2024, 2, 15);

        Assert.Equal(new DateOnly(2024, 2, 1), DateAndTimeSamples.StartOfMonth(value));
        Assert.Equal(new DateOnly(2024, 2, 29), DateAndTimeSamples.EndOfMonth(value));
    }

    [Theory]
    [InlineData(2026, 5, 23, true)]
    [InlineData(2026, 5, 25, false)]
    public void IsWeekend_ReturnsWhetherDateIsWeekend(int year, int month, int day, bool expected)
    {
        var result = DateAndTimeSamples.IsWeekend(new DateOnly(year, month, day));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateAge_SubtractsOne_WhenBirthdayHasNotOccurredThisYear()
    {
        var result = DateAndTimeSamples.CalculateAge(
            new DateOnly(2000, 6, 1),
            new DateOnly(2026, 5, 24));

        Assert.Equal(25, result);
    }

    [Fact]
    public void CalculateAge_ReturnsAge_WhenBirthdayHasOccurredThisYear()
    {
        var result = DateAndTimeSamples.CalculateAge(
            new DateOnly(2000, 5, 1),
            new DateOnly(2026, 5, 24));

        Assert.Equal(26, result);
    }

    [Fact]
    public void CalculateAge_TreatsLeapDayBirthdayAsFebruaryTwentyEighthInNonLeapYear()
    {
        var result = DateAndTimeSamples.CalculateAge(
            new DateOnly(2000, 2, 29),
            new DateOnly(2026, 2, 28));

        Assert.Equal(26, result);
    }

    [Fact]
    public void DaysBetween_ReturnsSignedDayDifference()
    {
        var result = DateAndTimeSamples.DaysBetween(
            new DateOnly(2026, 5, 24),
            new DateOnly(2026, 5, 31));

        Assert.Equal(7, result);
    }
}
