using System.Runtime.CompilerServices;
using DotnetBackendSnippets.Async;

namespace DotnetBackendSnippets.Tests.Async;

public sealed class AsyncSamplesTests
{
    [Fact]
    public async Task PageAsync_ReturnsRequestedPage()
    {
        IReadOnlyList<int> result = await AsyncSamples.PageAsync(ToAsync([1, 2, 3, 4, 5]), pageNumber: 2, pageSize: 2);

        Assert.Equal([3, 4], result);
    }

    [Fact]
    public async Task PageAsync_ReturnsEmptyList_WhenPageIsOutOfRange()
    {
        IReadOnlyList<int> result = await AsyncSamples.PageAsync(ToAsync([1, 2, 3]), pageNumber: 3, pageSize: 2);

        Assert.Empty(result);
    }

    [Fact]
    public async Task PageAsync_DoesNotOverflow_WhenSkipCountIsVeryLarge()
    {
        IReadOnlyList<int> result = await AsyncSamples.PageAsync(ToAsync([1, 2, 3]), pageNumber: int.MaxValue, pageSize: int.MaxValue);

        Assert.Empty(result);
    }

    [Fact]
    public async Task PageAsync_Throws_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => AsyncSamples.PageAsync(ToAsync([1, 2, 3]), pageNumber: 1, pageSize: 2, cancellationTokenSource.Token));
    }

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

    [Fact]
    public async Task DelayAsync_Throws_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => AsyncSamples.DelayAsync(TimeSpan.FromMinutes(1), cancellationTokenSource.Token));
    }

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
