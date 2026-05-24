namespace DotnetBackendSnippets.DateAndTime;

/// <summary>
/// 日付と時刻で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class DateAndTimeReverseLookupSamples
{
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
}

