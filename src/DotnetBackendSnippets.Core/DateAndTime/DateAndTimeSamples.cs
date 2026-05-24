namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻の基本操作例を提供します。
/// </summary>
public static class DateAndTimeSamples
{
    /// <summary>
    /// 指定日時の日始まりを返します。
    /// </summary>
    /// <param name="value">対象日時。</param>
    /// <returns>同じ日付の 00:00:00。</returns>
    public static DateTime StartOfDay(DateTime value)
    {
        return value.Date;
    }

    /// <summary>
    /// 指定日時の日終わりを返します。
    /// </summary>
    /// <param name="value">対象日時。</param>
    /// <returns>同じ日付の最後の tick。</returns>
    public static DateTime EndOfDay(DateTime value)
    {
        return value.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// 指定日が属する月の初日を返します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <returns>同じ年月の 1 日。</returns>
    public static DateOnly StartOfMonth(DateOnly value)
    {
        return new DateOnly(value.Year, value.Month, 1);
    }

    /// <summary>
    /// 指定日が属する月の末日を返します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <returns>同じ年月の最終日。</returns>
    public static DateOnly EndOfMonth(DateOnly value)
    {
        return StartOfMonth(value).AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// 指定日が週末かどうかを判定します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <returns>土曜日または日曜日なら <see langword="true"/>。</returns>
    public static bool IsWeekend(DateOnly value)
    {
        return value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    /// <summary>
    /// 生年月日と基準日から満年齢を計算します。
    /// </summary>
    /// <param name="birthDate">生年月日。</param>
    /// <param name="today">基準日。</param>
    /// <returns>基準日時点の満年齢。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="birthDate"/> が <paramref name="today"/> より未来の場合。</exception>
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

    /// <summary>
    /// 2 つの日付の差分日数を返します。
    /// </summary>
    /// <param name="start">開始日。</param>
    /// <param name="end">終了日。</param>
    /// <returns><paramref name="end"/> から <paramref name="start"/> を引いた日数。</returns>
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
