using System.Globalization;

namespace DotnetBackendSnippets.DateAndTime;

public static class DateAndTimeReverseLookupSamples
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string TimeFormat = "HH:mm";

    public static DateOnly ToDateOnly(DateTime value)
    {
        return DateOnly.FromDateTime(value);
    }

    public static DateTime CombineDateAndTime(DateOnly date, TimeOnly time, DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return DateTime.SpecifyKind(date.ToDateTime(time), kind);
    }

    public static DateOnly CurrentDate(TimeProvider timeProvider, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(timeZone);

        var localNow = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);

        return DateOnly.FromDateTime(localNow.DateTime);
    }

    public static DateOnlyRange SingleDayRange(DateOnly date)
    {
        return new DateOnlyRange(date, date.AddDays(1));
    }

    public static DateOnlyRange MonthRange(DateOnly value)
    {
        var start = new DateOnly(value.Year, value.Month, 1);

        return new DateOnlyRange(start, start.AddMonths(1));
    }

    public static DateOnlyRange QuarterRange(DateOnly value)
    {
        var quarterStartMonth = ((value.Month - 1) / 3) * 3 + 1;
        var start = new DateOnly(value.Year, quarterStartMonth, 1);

        return new DateOnlyRange(start, start.AddMonths(3));
    }

    public static DateOnly StartOfFiscalYear(DateOnly value, int fiscalYearStartMonth)
    {
        if (fiscalYearStartMonth is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(fiscalYearStartMonth), "Fiscal year start month must be between 1 and 12.");
        }

        var fiscalYear = value.Month >= fiscalYearStartMonth ? value.Year : value.Year - 1;

        return new DateOnly(fiscalYear, fiscalYearStartMonth, 1);
    }

    public static DateOnly StartOfWeek(DateOnly value, DayOfWeek firstDayOfWeek)
    {
        var daysFromStart = ((int)value.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

        return value.AddDays(-daysFromStart);
    }

    public static bool IsBusinessDay(DateOnly value, ISet<DateOnly>? holidays = null)
    {
        return !DateAndTimeSamples.IsWeekend(value) && holidays?.Contains(value) != true;
    }

    public static DateOnly NextBusinessDay(DateOnly value, ISet<DateOnly>? holidays = null)
    {
        var candidate = value.AddDays(1);
        while (!IsBusinessDay(candidate, holidays))
        {
            candidate = candidate.AddDays(1);
        }

        return candidate;
    }

    public static DateOnly PreviousBusinessDay(DateOnly value, ISet<DateOnly>? holidays = null)
    {
        var candidate = value.AddDays(-1);
        while (!IsBusinessDay(candidate, holidays))
        {
            candidate = candidate.AddDays(-1);
        }

        return candidate;
    }

    public static DateOnly AddBusinessDays(DateOnly value, int days, ISet<DateOnly>? holidays = null)
    {
        if (days == 0)
        {
            return value;
        }

        var remaining = Math.Abs(days);
        var step = days > 0 ? 1 : -1;
        var candidate = value;

        while (remaining > 0)
        {
            candidate = candidate.AddDays(step);
            if (IsBusinessDay(candidate, holidays))
            {
                remaining--;
            }
        }

        return candidate;
    }

    public static int CountBusinessDays(DateOnly startInclusive, DateOnly endExclusive, ISet<DateOnly>? holidays = null)
    {
        if (endExclusive < startInclusive)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.", nameof(endExclusive));
        }

        var count = 0;
        for (var date = startInclusive; date < endExclusive; date = date.AddDays(1))
        {
            if (IsBusinessDay(date, holidays))
            {
                count++;
            }
        }

        return count;
    }

    public static DateTimeOffsetRange LocalDateRangeToUtcRange(DateOnly startInclusive, DateOnly endInclusive, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        if (endInclusive < startInclusive)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.", nameof(endInclusive));
        }

        var endExclusive = endInclusive.AddDays(1);

        return new DateTimeOffsetRange(
            LocalDateStartToUtc(startInclusive, timeZone),
            LocalDateStartToUtc(endExclusive, timeZone));
    }

    public static DateTimeOffset NormalizeToUtc(DateTimeOffset value)
    {
        return value.ToUniversalTime();
    }

    public static DateOnly UtcToLocalDate(DateTimeOffset utcValue, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var localValue = TimeZoneInfo.ConvertTime(utcValue.ToUniversalTime(), timeZone);

        return DateOnly.FromDateTime(localValue.DateTime);
    }

    public static bool IsExpired(DateTimeOffset expiresAt, DateTimeOffset now)
    {
        return now >= expiresAt;
    }

    public static TimeSpan RemainingTime(DateTimeOffset expiresAt, DateTimeOffset now)
    {
        return expiresAt <= now ? TimeSpan.Zero : expiresAt - now;
    }

    public static bool IsWithinPeriod(DateTimeOffset value, DateTimeOffset validFrom, DateTimeOffset validUntil)
    {
        if (validUntil < validFrom)
        {
            throw new ArgumentException("Valid until must be greater than or equal to valid from.", nameof(validUntil));
        }

        return validFrom <= value && value < validUntil;
    }

    public static bool TryParseIsoDate(string value, out DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    public static bool TryParseHourMinute(string value, out TimeOnly time)
    {
        ArgumentNullException.ThrowIfNull(value);

        return TimeOnly.TryParseExact(value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
    }

    public static bool TryFindTimeZone(string timeZoneId, out TimeZoneInfo? timeZone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);

        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            timeZone = null;
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            timeZone = null;
            return false;
        }
    }

    public static DateTimeOffsetRange SearchRangeForInclusiveDates(DateOnly startInclusive, DateOnly endInclusive, TimeOnly startOfDay, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        if (endInclusive < startInclusive)
        {
            throw new ArgumentException("End date must be greater than or equal to start date.", nameof(endInclusive));
        }

        var start = LocalDateTimeToUtc(startInclusive, startOfDay, timeZone);
        var endExclusive = LocalDateTimeToUtc(endInclusive.AddDays(1), startOfDay, timeZone);

        return new DateTimeOffsetRange(start, endExclusive);
    }

    private static DateTimeOffset LocalDateStartToUtc(DateOnly date, TimeZoneInfo timeZone)
    {
        return LocalDateTimeToUtc(date, TimeOnly.MinValue, timeZone);
    }

    private static DateTimeOffset LocalDateTimeToUtc(DateOnly date, TimeOnly time, TimeZoneInfo timeZone)
    {
        var localDateTime = date.ToDateTime(time, DateTimeKind.Unspecified);

        if (timeZone.IsInvalidTime(localDateTime))
        {
            throw new ArgumentException("The local date and time does not exist in the specified time zone.", nameof(date));
        }

        return new DateTimeOffset(localDateTime, timeZone.GetUtcOffset(localDateTime)).ToUniversalTime();
    }
}

public readonly record struct DateOnlyRange(DateOnly StartInclusive, DateOnly EndExclusive);

public readonly record struct DateTimeOffsetRange(DateTimeOffset StartInclusive, DateTimeOffset EndExclusive);
