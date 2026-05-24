using System.Globalization;

namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class DateAndTimeReverseLookupSamples
{
    private const string DateFormat = "yyyy-MM-dd";

    private const string TimeFormat = "HH:mm";

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
}

