namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class DateAndTimeReverseLookupSamples
{
    /// <summary>
    /// 1 日分の半開区間を作成します。
    /// </summary>
    /// <param name="date">対象日。</param>
    /// <returns>開始日を含み終了日を含まない日付範囲。</returns>
    public static DateOnlyRange SingleDayRange(DateOnly date)
    {
        return new DateOnlyRange(date, date.AddDays(1));
    }

    /// <summary>
    /// 指定日を含む月の半開区間を作成します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <returns>月初から翌月初までの日付範囲。</returns>
    public static DateOnlyRange MonthRange(DateOnly value)
    {
        var start = new DateOnly(value.Year, value.Month, 1);

        return new DateOnlyRange(start, start.AddMonths(1));
    }

    /// <summary>
    /// 指定日を含む四半期の半開区間を作成します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <returns>四半期の開始日から次四半期の開始日までの日付範囲。</returns>
    public static DateOnlyRange QuarterRange(DateOnly value)
    {
        var quarterStartMonth = ((value.Month - 1) / 3) * 3 + 1;
        var start = new DateOnly(value.Year, quarterStartMonth, 1);

        return new DateOnlyRange(start, start.AddMonths(3));
    }

    /// <summary>
    /// 指定日が属する会計年度の開始日を取得します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <param name="fiscalYearStartMonth">会計年度の開始月。</param>
    /// <returns>会計年度の開始日。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fiscalYearStartMonth"/> が 1 から 12 の範囲外です。</exception>
    public static DateOnly StartOfFiscalYear(DateOnly value, int fiscalYearStartMonth)
    {
        if (fiscalYearStartMonth is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(fiscalYearStartMonth), "Fiscal year start month must be between 1 and 12.");
        }

        var fiscalYear = value.Month >= fiscalYearStartMonth ? value.Year : value.Year - 1;

        return new DateOnly(fiscalYear, fiscalYearStartMonth, 1);
    }

    /// <summary>
    /// 指定日を含む週の開始日を取得します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <param name="firstDayOfWeek">週の先頭曜日。</param>
    /// <returns>週の開始日。</returns>
    public static DateOnly StartOfWeek(DateOnly value, DayOfWeek firstDayOfWeek)
    {
        var daysFromStart = ((int)value.DayOfWeek - (int)firstDayOfWeek + 7) % 7;

        return value.AddDays(-daysFromStart);
    }

    /// <summary>
    /// 日付指定の検索条件を UTC の半開日時範囲へ変換します。
    /// </summary>
    /// <param name="startInclusive">開始日。</param>
    /// <param name="endInclusive">終了日。</param>
    /// <param name="startOfDay">業務上の日付開始時刻。</param>
    /// <param name="timeZone">ローカル日付のタイムゾーン。</param>
    /// <returns>UTC の検索範囲。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="timeZone"/> が null です。</exception>
    /// <exception cref="ArgumentException"><paramref name="endInclusive"/> が開始日より前です。</exception>
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
}

