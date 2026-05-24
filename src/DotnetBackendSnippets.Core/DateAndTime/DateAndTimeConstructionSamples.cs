namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class DateAndTimeReverseLookupSamples
{
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
}

