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

        int count = items.TryGetNonEnumeratedCount(out int knownCount)
            ? knownCount
            : items.Count();

        logger.LogInformation("Processed {Count} items.", count);

        return count;
    }

    /// <summary>
    /// 例外を処理済みとして warning log に出力します。
    /// </summary>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <param name="exception">記録する例外。</param>
    /// <param name="operationName">失敗した処理名。</param>
    /// <returns>ログに使った event id。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> または <paramref name="exception"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="operationName"/> が空白の場合。</exception>
    public static EventId LogHandledException(ILogger logger, Exception exception, string operationName)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

        var eventId = new EventId(1001, "HandledException");
        logger.LogWarning(eventId, exception, "Handled exception in {OperationName}.", operationName.Trim());

        return eventId;
    }

    /// <summary>
    /// 処理時間が閾値以上なら warning、未満なら debug でログに出力します。
    /// </summary>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <param name="operationName">処理名。</param>
    /// <param name="elapsed">実際の処理時間。</param>
    /// <param name="threshold">遅い処理として扱う閾値。</param>
    /// <returns>閾値以上の場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="operationName"/> が空白の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="elapsed"/> または <paramref name="threshold"/> が負数の場合。</exception>
    public static bool LogOperationDuration(
        ILogger logger,
        string operationName,
        TimeSpan elapsed,
        TimeSpan threshold)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

        if (elapsed < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(elapsed), "Elapsed time must be zero or greater.");
        }

        if (threshold < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold must be zero or greater.");
        }

        var isSlow = elapsed >= threshold;
        var trimmedOperationName = operationName.Trim();

        if (isSlow)
        {
            logger.LogWarning(
                "Slow operation {OperationName} took {ElapsedMilliseconds} ms. Threshold is {ThresholdMilliseconds} ms.",
                trimmedOperationName,
                elapsed.TotalMilliseconds,
                threshold.TotalMilliseconds);
        }
        else
        {
            logger.LogDebug(
                "Operation {OperationName} took {ElapsedMilliseconds} ms.",
                trimmedOperationName,
                elapsed.TotalMilliseconds);
        }

        return isSlow;
    }
}
