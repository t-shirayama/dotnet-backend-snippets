using System.Runtime.CompilerServices;
using DotnetBackendSnippets.Async;

namespace DotnetBackendSnippets.Tests.Async;

// テスト対象: Async Samples のスニペット動作を確認する。
public sealed class AsyncSamplesTests
{
    // テスト意図: Page Async / Returns Requested Page を確認する。
    [Fact]
    public async Task PageAsync_ReturnsRequestedPage()
    {
        IReadOnlyList<int> result = await AsyncSamples.PageAsync(ToAsync([1, 2, 3, 4, 5]), pageNumber: 2, pageSize: 2);

        Assert.Equal([3, 4], result);
    }

    // テスト意図: Page Async / Returns Empty List / When Page Is Out Of Range を確認する。
    [Fact]
    public async Task PageAsync_ReturnsEmptyList_WhenPageIsOutOfRange()
    {
        IReadOnlyList<int> result = await AsyncSamples.PageAsync(ToAsync([1, 2, 3]), pageNumber: 3, pageSize: 2);

        Assert.Empty(result);
    }

    // テスト意図: Page Async / Does Not Overflow / When Skip Count Is Very Large を確認する。
    [Fact]
    public async Task PageAsync_DoesNotOverflow_WhenSkipCountIsVeryLarge()
    {
        IReadOnlyList<int> result = await AsyncSamples.PageAsync(ToAsync([1, 2, 3]), pageNumber: int.MaxValue, pageSize: int.MaxValue);

        Assert.Empty(result);
    }

    // テスト意図: Page Async / Throws / When Cancellation Is Requested を確認する。
    [Fact]
    public async Task PageAsync_Throws_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => AsyncSamples.PageAsync(ToAsync([1, 2, 3]), pageNumber: 1, pageSize: 2, cancellationTokenSource.Token));
    }

    // テスト意図: When All Settled Async / Collects Successes And Failures を確認する。
    [Fact]
    public async Task WhenAllSettledAsync_CollectsSuccessesAndFailures()
    {
        var result = await AsyncSamples.WhenAllSettledAsync<int>(
        [
            () => Task.FromResult(10),
            () => throw new InvalidOperationException("failed"),
            () => Task.FromResult(30),
        ]);

        Assert.False(result.IsSuccessful);
        Assert.Equal([10, 30], result.Successes.Select(success => success.Value));
        Assert.Equal([0, 2], result.Successes.Select(success => success.Index));

        TaskFailed<int> failure = Assert.Single(result.Failures);
        Assert.Equal(1, failure.Index);
        Assert.IsType<InvalidOperationException>(failure.Exception);
    }

    // テスト意図: When All Settled Async / Returns Successful Result / When All Operations Succeed を確認する。
    [Fact]
    public async Task WhenAllSettledAsync_ReturnsSuccessfulResult_WhenAllOperationsSucceed()
    {
        var result = await AsyncSamples.WhenAllSettledAsync<string>(
        [
            () => Task.FromResult("first"),
            () => Task.FromResult("second"),
        ]);

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Failures);
        Assert.Equal(["first", "second"], result.Successes.Select(success => success.Value));
    }

    // テスト意図: Delay Async / Throws / When Cancellation Is Requested を確認する。
    [Fact]
    public async Task DelayAsync_Throws_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => AsyncSamples.DelayAsync(TimeSpan.FromMinutes(1), cancellationTokenSource.Token));
    }

    // テスト意図: Process Sequentially Async / Returns Processed Results を確認する。
    [Fact]
    public async Task ProcessSequentiallyAsync_ReturnsProcessedResults()
    {
        IReadOnlyList<string> result = await AsyncSamples.ProcessSequentiallyAsync(
            [1, 2, 3],
            (item, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ValueTask.FromResult($"item-{item}");
            });

        Assert.Equal(["item-1", "item-2", "item-3"], result);
    }

    // テスト意図: Process Sequentially Async / Throws / When Cancellation Is Requested を確認する。
    [Fact]
    public async Task ProcessSequentiallyAsync_Throws_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => AsyncSamples.ProcessSequentiallyAsync(
                [1, 2, 3],
                (item, _) => ValueTask.FromResult(item),
                cancellationTokenSource.Token));
    }

    private static async IAsyncEnumerable<T> ToAsync<T>(
        IEnumerable<T> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return item;
        }
    }
}
