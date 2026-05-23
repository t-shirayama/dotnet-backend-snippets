using DotnetBackendSnippets.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Tests.BackgroundServices;

public sealed class BackgroundServiceSamplesTests
{
    [Fact]
    public async Task RunUntilCancelledAsync_ExecutesWorkerLoop_UntilCancellation()
    {
        using CancellationTokenSource cancellationTokenSource = new();
        CountingWorker worker = new(() => cancellationTokenSource.Cancel());
        TestLogger<BackgroundWorkerLoop> logger = new();
        BackgroundWorkerLoop loop = new(worker, logger, static (_, _) => Task.CompletedTask);
        BackgroundWorkerLoopOptions options = new()
        {
            Interval = TimeSpan.Zero,
        };

        await loop.RunUntilCancelledAsync(options, cancellationTokenSource.Token);

        Assert.Equal(3, worker.Count);
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public async Task RunOnceAsync_LogsException_AndContinuesWhenConfigured()
    {
        InvalidOperationException exception = new("failed");
        ThrowingWorker worker = new(exception);
        TestLogger<BackgroundWorkerLoop> logger = new();
        BackgroundWorkerLoop loop = new(worker, logger);

        bool shouldContinue = await loop.RunOnceAsync(continueOnError: true);

        Assert.True(shouldContinue);
        LogEntry entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Contains("Background worker iteration failed.", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunOnceAsync_RethrowsException_WhenContinueOnErrorIsFalse()
    {
        InvalidOperationException exception = new("failed");
        ThrowingWorker worker = new(exception);
        TestLogger<BackgroundWorkerLoop> logger = new();
        BackgroundWorkerLoop loop = new(worker, logger);

        InvalidOperationException actual = await Assert.ThrowsAsync<InvalidOperationException>(
            () => loop.RunOnceAsync(continueOnError: false));

        Assert.Same(exception, actual);
    }

    [Fact]
    public void AddBackgroundWorker_RegistersWorkerLoopHostedService()
    {
        ServiceCollection services = [];

        services.AddBackgroundWorker<NoOpWorker>(options =>
        {
            options.Interval = TimeSpan.FromSeconds(5);
            options.ContinueOnError = false;
        });

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.IsType<NoOpWorker>(provider.GetRequiredService<IBackgroundWorker>());
        Assert.IsType<WorkerLoopHostedService>(provider.GetRequiredService<IHostedService>());
    }

    private sealed class CountingWorker(Action onThirdExecution) : IBackgroundWorker
    {
        public int Count { get; private set; }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Count++;

            if (Count == 3)
            {
                onThirdExecution();
            }

            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingWorker(Exception exception) : IBackgroundWorker
    {
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            throw exception;
        }
    }

    private sealed class NoOpWorker : IBackgroundWorker
    {
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);

    private sealed class TestLogger<T> : ILogger<T>
    {
        private readonly List<LogEntry> entries = [];

        public IReadOnlyList<LogEntry> Entries => entries;

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
