namespace DotnetBackendSnippets.Async;

public abstract record TaskOutcome<T>(int Index);

public sealed record TaskSucceeded<T>(int Index, T Value) : TaskOutcome<T>(Index);

public sealed record TaskFailed<T>(int Index, Exception Exception) : TaskOutcome<T>(Index);

public sealed record TaskBatchResult<T>(
    IReadOnlyList<TaskSucceeded<T>> Successes,
    IReadOnlyList<TaskFailed<T>> Failures)
{
    public bool IsSuccessful => Failures.Count == 0;
}

public static class AsyncSamples
{
    public static async Task<IReadOnlyList<T>> PageAsync<T>(
        IAsyncEnumerable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be one or greater.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be one or greater.");
        }

        var skipCount = ((long)pageNumber - 1) * pageSize;
        var items = new List<T>();

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (skipCount > 0)
            {
                skipCount--;
                continue;
            }

            items.Add(item);

            if (items.Count == pageSize)
            {
                break;
            }
        }

        return items;
    }

    public static async Task<TaskBatchResult<T>> WhenAllSettledAsync<T>(
        IEnumerable<Func<Task<T>>> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);

        var tasks = operations
            .Select((operation, index) =>
            {
                if (operation is null)
                {
                    throw new ArgumentException("Operations must not contain null.", nameof(operations));
                }

                return CaptureAsync(operation, index);
            })
            .ToArray();

        TaskOutcome<T>[] outcomes = await Task.WhenAll(tasks).ConfigureAwait(false);

        return new TaskBatchResult<T>(
            outcomes.OfType<TaskSucceeded<T>>().ToList(),
            outcomes.OfType<TaskFailed<T>>().ToList());
    }

    public static Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (delay < TimeSpan.Zero && delay != Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be zero, positive, or infinite.");
        }

        return Task.Delay(delay, cancellationToken);
    }

    public static async Task<IReadOnlyList<TResult>> ProcessSequentiallyAsync<T, TResult>(
        IEnumerable<T> source,
        Func<T, CancellationToken, ValueTask<TResult>> processor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(processor);

        var results = new List<TResult>();

        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            results.Add(await processor(item, cancellationToken).ConfigureAwait(false));
        }

        return results;
    }

    private static async Task<TaskOutcome<T>> CaptureAsync<T>(Func<Task<T>> operation, int index)
    {
        try
        {
            T value = await operation().ConfigureAwait(false);
            return new TaskSucceeded<T>(index, value);
        }
        catch (Exception exception)
        {
            return new TaskFailed<T>(index, exception);
        }
    }
}
