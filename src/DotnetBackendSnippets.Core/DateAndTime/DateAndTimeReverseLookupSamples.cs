using System.Globalization;

namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻で逆引きしやすい実務向けサンプルを提供します。
/// </summary>
public static class DateAndTimeReverseLookupSamples
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string TimeFormat = "HH:mm";

    /// <summary>
    /// <see cref="DateTime"/> から日付部分だけを取り出します。
    /// </summary>
    /// <param name="value">変換元の日時。</param>
    /// <returns>日付部分。</returns>
    public static DateOnly ToDateOnly(DateTime value)
    {
        return DateOnly.FromDateTime(value);
    }

    /// <summary>
    /// <see cref="DateOnly"/> と <see cref="TimeOnly"/> を結合して日時を作成します。
    /// </summary>
    /// <param name="date">日付。</param>
    /// <param name="time">時刻。</param>
    /// <param name="kind">作成する日時の種類。</param>
    /// <returns>結合された日時。</returns>
    public static DateTime CombineDateAndTime(DateOnly date, TimeOnly time, DateTimeKind kind = DateTimeKind.Unspecified)
    {
        return DateTime.SpecifyKind(date.ToDateTime(time), kind);
    }

    /// <summary>
    /// 指定タイムゾーンでの現在日付を取得します。
    /// </summary>
    /// <param name="timeProvider">現在時刻を提供するオブジェクト。</param>
    /// <param name="timeZone">変換先のタイムゾーン。</param>
    /// <returns>タイムゾーン変換後の現在日付。</returns>
    /// <exception cref="ArgumentNullException">いずれかの引数が null です。</exception>
    public static DateOnly CurrentDate(TimeProvider timeProvider, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(timeZone);

        var localNow = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);

        return DateOnly.FromDateTime(localNow.DateTime);
    }

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
    /// 指定日が営業日かどうかを判定します。
    /// </summary>
    /// <param name="value">対象日。</param>
    /// <param name="holidays">任意の休日一覧。</param>
    /// <returns>週末でも休日でもない場合は true。</returns>
    public static bool IsBusinessDay(DateOnly value, ISet<DateOnly>? holidays = null)
    {
        return !DateAndTimeSamples.IsWeekend(value) && holidays?.Contains(value) != true;
    }

    /// <summary>
    /// 指定日の翌営業日を取得します。
    /// </summary>
    /// <param name="value">基準日。</param>
    /// <param name="holidays">任意の休日一覧。</param>
    /// <returns>翌営業日。</returns>
    public static DateOnly NextBusinessDay(DateOnly value, ISet<DateOnly>? holidays = null)
    {
        var candidate = value.AddDays(1);
        while (!IsBusinessDay(candidate, holidays))
        {
            candidate = candidate.AddDays(1);
        }

        return candidate;
    }

    /// <summary>
    /// 指定日の前営業日を取得します。
    /// </summary>
    /// <param name="value">基準日。</param>
    /// <param name="holidays">任意の休日一覧。</param>
    /// <returns>前営業日。</returns>
    public static DateOnly PreviousBusinessDay(DateOnly value, ISet<DateOnly>? holidays = null)
    {
        var candidate = value.AddDays(-1);
        while (!IsBusinessDay(candidate, holidays))
        {
            candidate = candidate.AddDays(-1);
        }

        return candidate;
    }

    /// <summary>
    /// 指定した営業日数だけ日付を進めます。
    /// </summary>
    /// <param name="value">基準日。</param>
    /// <param name="days">加算する営業日数。負数で過去方向。</param>
    /// <param name="holidays">任意の休日一覧。</param>
    /// <returns>営業日加算後の日付。</returns>
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

    /// <summary>
    /// 半開区間内の営業日数を数えます。
    /// </summary>
    /// <param name="startInclusive">開始日。</param>
    /// <param name="endExclusive">終了日。</param>
    /// <param name="holidays">任意の休日一覧。</param>
    /// <returns>営業日数。</returns>
    /// <exception cref="ArgumentException"><paramref name="endExclusive"/> が開始日より前です。</exception>
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

    /// <summary>
    /// ローカル日付の範囲を UTC の日時範囲へ変換します。
    /// </summary>
    /// <param name="startInclusive">開始日。</param>
    /// <param name="endInclusive">終了日。</param>
    /// <param name="timeZone">ローカル日付のタイムゾーン。</param>
    /// <returns>UTC の半開日時範囲。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="timeZone"/> が null です。</exception>
    /// <exception cref="ArgumentException"><paramref name="endInclusive"/> が開始日より前です。</exception>
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

    /// <summary>
    /// 日時オフセットを UTC に正規化します。
    /// </summary>
    /// <param name="value">変換元の日時オフセット。</param>
    /// <returns>UTC の日時オフセット。</returns>
    public static DateTimeOffset NormalizeToUtc(DateTimeOffset value)
    {
        return value.ToUniversalTime();
    }

    /// <summary>
    /// UTC の日時から指定タイムゾーンの日付を取得します。
    /// </summary>
    /// <param name="utcValue">UTC の日時。</param>
    /// <param name="timeZone">変換先のタイムゾーン。</param>
    /// <returns>ローカル日付。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="timeZone"/> が null です。</exception>
    public static DateOnly UtcToLocalDate(DateTimeOffset utcValue, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var localValue = TimeZoneInfo.ConvertTime(utcValue.ToUniversalTime(), timeZone);

        return DateOnly.FromDateTime(localValue.DateTime);
    }

    /// <summary>
    /// 有効期限が切れているかを判定します。
    /// </summary>
    /// <param name="expiresAt">有効期限。</param>
    /// <param name="now">判定時刻。</param>
    /// <returns>期限切れの場合は true。</returns>
    public static bool IsExpired(DateTimeOffset expiresAt, DateTimeOffset now)
    {
        return now >= expiresAt;
    }

    /// <summary>
    /// 有効期限までの残り時間を取得します。
    /// </summary>
    /// <param name="expiresAt">有効期限。</param>
    /// <param name="now">現在時刻。</param>
    /// <returns>残り時間。期限切れの場合は 0。</returns>
    public static TimeSpan RemainingTime(DateTimeOffset expiresAt, DateTimeOffset now)
    {
        return expiresAt <= now ? TimeSpan.Zero : expiresAt - now;
    }

    /// <summary>
    /// 指定日時が有効期間内かどうかを判定します。
    /// </summary>
    /// <param name="value">判定対象の日時。</param>
    /// <param name="validFrom">有効開始日時。</param>
    /// <param name="validUntil">有効終了日時。</param>
    /// <returns>有効期間内の場合は true。</returns>
    /// <exception cref="ArgumentException"><paramref name="validUntil"/> が開始日時より前です。</exception>
    public static bool IsWithinPeriod(DateTimeOffset value, DateTimeOffset validFrom, DateTimeOffset validUntil)
    {
        if (validUntil < validFrom)
        {
            throw new ArgumentException("Valid until must be greater than or equal to valid from.", nameof(validUntil));
        }

        return validFrom <= value && value < validUntil;
    }

    /// <summary>
    /// ISO 形式の日付文字列を解析します。
    /// </summary>
    /// <param name="value">解析する文字列。</param>
    /// <param name="date">解析された日付。</param>
    /// <returns>解析できた場合は true。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が null です。</exception>
    public static bool TryParseIsoDate(string value, out DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    /// <summary>
    /// 時分形式の文字列を解析します。
    /// </summary>
    /// <param name="value">解析する文字列。</param>
    /// <param name="time">解析された時刻。</param>
    /// <returns>解析できた場合は true。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が null です。</exception>
    public static bool TryParseHourMinute(string value, out TimeOnly time)
    {
        ArgumentNullException.ThrowIfNull(value);

        return TimeOnly.TryParseExact(value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
    }

    /// <summary>
    /// タイムゾーン ID からタイムゾーンを検索します。
    /// </summary>
    /// <param name="timeZoneId">タイムゾーン ID。</param>
    /// <param name="timeZone">見つかったタイムゾーン。</param>
    /// <returns>見つかった場合は true。</returns>
    /// <exception cref="ArgumentException"><paramref name="timeZoneId"/> が null または空白です。</exception>
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

/// <summary>
/// DateOnly の半開区間です。
/// </summary>
/// <param name="StartInclusive">開始日。</param>
/// <param name="EndExclusive">終了日。</param>
public readonly record struct DateOnlyRange(DateOnly StartInclusive, DateOnly EndExclusive);

/// <summary>
/// DateTimeOffset の半開区間です。
/// </summary>
/// <param name="StartInclusive">開始日時。</param>
/// <param name="EndExclusive">終了日時。</param>
public readonly record struct DateTimeOffsetRange(DateTimeOffset StartInclusive, DateTimeOffset EndExclusive);
