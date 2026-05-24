namespace DotnetBackendSnippets.Async;

/// <summary>
/// 複数タスクの実行結果を表す基底型です。
/// </summary>
/// <typeparam name="T">タスクが返す値の型。</typeparam>
/// <param name="Index">元の操作一覧における位置。</param>
public abstract record TaskOutcome<T>(int Index);

/// <summary>
/// 成功したタスクの結果を表します。
/// </summary>
/// <typeparam name="T">成功値の型。</typeparam>
/// <param name="Index">元の操作一覧における位置。</param>
/// <param name="Value">タスクが返した値。</param>
public sealed record TaskSucceeded<T>(int Index, T Value) : TaskOutcome<T>(Index);

/// <summary>
/// 失敗したタスクの結果を表します。
/// </summary>
/// <typeparam name="T">失敗したタスクが返す予定だった値の型。</typeparam>
/// <param name="Index">元の操作一覧における位置。</param>
/// <param name="Exception">タスクで発生した例外。</param>
public sealed record TaskFailed<T>(int Index, Exception Exception) : TaskOutcome<T>(Index);

/// <summary>
/// 複数タスクを完了まで待機した結果をまとめます。
/// </summary>
/// <typeparam name="T">タスクが返す値の型。</typeparam>
/// <param name="Successes">成功したタスクの結果一覧。</param>
/// <param name="Failures">失敗したタスクの結果一覧。</param>
public sealed record TaskBatchResult<T>(
    IReadOnlyList<TaskSucceeded<T>> Successes,
    IReadOnlyList<TaskFailed<T>> Failures)
{
    /// <summary>
    /// すべてのタスクが成功したかどうかを取得します。
    /// </summary>
    /// <value>失敗が 0 件なら <see langword="true"/>。</value>
    public bool IsSuccessful => Failures.Count == 0;
}

/// <summary>
/// 非同期処理でよく使う実装例を提供します。
/// </summary>
public static class AsyncSamples
{
    /// <summary>
    /// 非同期シーケンスから指定ページの要素を取得します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">ページング対象の非同期シーケンス。</param>
    /// <param name="pageNumber">1 始まりのページ番号。</param>
    /// <param name="pageSize">1 ページあたりの件数。</param>
    /// <param name="cancellationToken">キャンセル通知用トークン。</param>
    /// <returns>指定ページに含まれる要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> または <paramref name="pageSize"/> が 1 未満の場合。</exception>
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

    /// <summary>
    /// 複数の非同期操作をすべて完了まで待機し、成功と失敗を分けて返します。
    /// </summary>
    /// <typeparam name="T">各操作が返す値の型。</typeparam>
    /// <param name="operations">実行する非同期操作の一覧。</param>
    /// <returns>成功結果と失敗結果を含むバッチ結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="operations"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="operations"/> に <see langword="null"/> の操作が含まれる場合。</exception>
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

    /// <summary>
    /// キャンセル可能な待機タスクを作成します。
    /// </summary>
    /// <param name="delay">待機時間。</param>
    /// <param name="cancellationToken">キャンセル通知用トークン。</param>
    /// <returns>待機を表すタスク。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delay"/> が負数で、無限待機でもない場合。</exception>
    public static Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (delay < TimeSpan.Zero && delay != Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be zero, positive, or infinite.");
        }

        return Task.Delay(delay, cancellationToken);
    }

    /// <summary>
    /// 各要素を順番に非同期処理し、結果を入力順で返します。
    /// </summary>
    /// <typeparam name="T">入力要素の型。</typeparam>
    /// <typeparam name="TResult">処理結果の型。</typeparam>
    /// <param name="source">処理対象の要素一覧。</param>
    /// <param name="processor">各要素を処理する関数。</param>
    /// <param name="cancellationToken">キャンセル通知用トークン。</param>
    /// <returns>各要素の処理結果一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="processor"/> が <see langword="null"/> の場合。</exception>
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
