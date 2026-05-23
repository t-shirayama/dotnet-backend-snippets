namespace DotnetBackendSnippets.DateAndTime;

public static class DateAndTimeSamples
{
    public static DateTime StartOfDay(DateTime value)
    {
        return value.Date;
    }

    public static DateTime EndOfDay(DateTime value)
    {
        return value.Date.AddDays(1).AddTicks(-1);
    }

    public static DateOnly StartOfMonth(DateOnly value)
    {
        return new DateOnly(value.Year, value.Month, 1);
    }

    public static DateOnly EndOfMonth(DateOnly value)
    {
        return StartOfMonth(value).AddMonths(1).AddDays(-1);
    }

    public static bool IsWeekend(DateOnly value)
    {
        return value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public static int CalculateAge(DateOnly birthDate, DateOnly today)
    {
        if (birthDate > today)
        {
            throw new ArgumentOutOfRangeException(nameof(birthDate), "Birth date must not be in the future.");
        }

        var age = today.Year - birthDate.Year;
        var birthdayThisYear = GetBirthdayInYear(birthDate, today.Year);

        return today < birthdayThisYear ? age - 1 : age;
    }

    public static int DaysBetween(DateOnly start, DateOnly end)
    {
        return end.DayNumber - start.DayNumber;
    }

    private static DateOnly GetBirthdayInYear(DateOnly birthDate, int year)
    {
        if (birthDate.Month == 2 && birthDate.Day == 29 && !DateTime.IsLeapYear(year))
        {
            return new DateOnly(year, 2, 28);
        }

        return new DateOnly(year, birthDate.Month, birthDate.Day);
    }
}
