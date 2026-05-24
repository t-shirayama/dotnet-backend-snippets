using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Logging;

/// <summary>
/// ログ出力のサンプルです。
/// </summary>
public static class LoggingSamples
{
    /// <summary>
    /// 処理済み件数を数えてログに出力します。
    /// </summary>
    /// <param name="items">処理済み項目。</param>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <returns>処理済み件数。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> または <paramref name="logger"/> が <see langword="null"/> の場合。</exception>
    public static int CountProcessedItems(IEnumerable<string> items, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(logger);

        var count = items.Count();
        logger.LogInformation("Processed {Count} items.", count);

        return count;
    }
}
