namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class DateAndTimeReverseLookupSamples
{
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

