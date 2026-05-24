using DotnetBackendSnippets.DateAndTime;

namespace DotnetBackendSnippets.Tests.DateAndTime;

// テスト補助: Date And Time Reverse Lookup Samples の共有 fixture を定義する。
public sealed partial class DateAndTimeReverseLookupSamplesTests
{
    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return utcNow;
        }
    }

    private static TimeZoneInfo GetTokyoTimeZone()
    {
        foreach (var id in new[] { "Tokyo Standard Time", "Asia/Tokyo" })
        {
            if (DateAndTimeReverseLookupSamples.TryFindTimeZone(id, out var timeZone))
            {
                return timeZone!;
            }
        }

        throw new InvalidOperationException("Tokyo time zone was not found.");
    }
}

