namespace DotnetBackendSnippets.DateAndTime;

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

