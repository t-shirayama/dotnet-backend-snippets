using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Logging;

public static class LoggingSamples
{
    public static int CountProcessedItems(IEnumerable<string> items, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(logger);

        var count = items.Count();
        logger.LogInformation("Processed {Count} items.", count);

        return count;
    }
}
