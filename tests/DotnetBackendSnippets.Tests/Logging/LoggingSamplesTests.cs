using DotnetBackendSnippets.Logging;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Tests.Logging;

// テスト対象: Logging Samples のスニペット動作を確認する。
public sealed class LoggingSamplesTests
{
    // テスト意図: Count Processed Items / Returns Count / And Writes Log を確認する。
    [Fact]
    public void CountProcessedItems_ReturnsCount_AndWritesLog()
    {
        var logger = new ListLogger();

        var count = LoggingSamples.CountProcessedItems(["a", "b", "c"], logger);

        Assert.Equal(3, count);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("3", entry.Message, StringComparison.Ordinal);
    }

    // テスト意図: Log Handled Exception / Writes Warning With Event Id を確認する。
    [Fact]
    public void LogHandledException_WritesWarningWithEventId()
    {
        var logger = new ListLogger();
        var exception = new InvalidOperationException("failed");

        var eventId = LoggingSamples.LogHandledException(logger, exception, " Import ");

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal(eventId, entry.EventId);
        Assert.Same(exception, entry.Exception);
        Assert.Contains("Import", entry.Message, StringComparison.Ordinal);
    }

    // テスト意図: Log Operation Duration / Writes Warning / When Slow を確認する。
    [Fact]
    public void LogOperationDuration_WritesWarning_WhenSlow()
    {
        var logger = new ListLogger();

        bool isSlow = LoggingSamples.LogOperationDuration(
            logger,
            "Export",
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(100));

        Assert.True(isSlow);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains("Export", entry.Message, StringComparison.Ordinal);
        Assert.Contains("250", entry.Message, StringComparison.Ordinal);
    }

    // テスト意図: Log Operation Duration / Writes Debug / When Fast を確認する。
    [Fact]
    public void LogOperationDuration_WritesDebug_WhenFast()
    {
        var logger = new ListLogger();

        bool isSlow = LoggingSamples.LogOperationDuration(
            logger,
            "Export",
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(100));

        Assert.False(isSlow);
        Assert.Equal(LogLevel.Debug, Assert.Single(logger.Entries).Level);
    }
}
