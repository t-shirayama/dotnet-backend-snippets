using DotnetBackendSnippets.Logging;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Tests.Logging;

public sealed class LoggingSamplesTests
{
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
}
