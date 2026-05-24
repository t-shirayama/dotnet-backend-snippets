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

    [Fact]
    public async Task ChannelBackgroundJobQueue_DequeuesQueuedJob()
    {
        var queue = new ChannelBackgroundJobQueue(
            Microsoft.Extensions.Options.Options.Create(new BackgroundJobQueueOptions { Capacity = 2 }));
        var job = new BackgroundJob("job-1", static (_, _) => ValueTask.CompletedTask);

        await queue.EnqueueAsync(job);
        BackgroundJob actual = await queue.DequeueAsync();

        Assert.Same(job, actual);
    }

    [Fact]
    public async Task QueuedBackgroundJobProcessor_RetriesFailedJob_AndUsesScopedService()
    {
        var queue = new ChannelBackgroundJobQueue(
            Microsoft.Extensions.Options.Options.Create(new BackgroundJobQueueOptions()));
        var services = new ServiceCollection();
        services.AddScoped<ScopedCounter>();
        using ServiceProvider provider = services.BuildServiceProvider();
        var poisonHandler = new RecordingPoisonHandler();
        TestLogger<QueuedBackgroundJobProcessor> logger = new();
        var processor = new QueuedBackgroundJobProcessor(
            queue,
            provider.GetRequiredService<IServiceScopeFactory>(),
            poisonHandler,
            logger);
        var attempts = 0;
        await queue.EnqueueAsync(new BackgroundJob(
            "retry-job",
            (serviceProvider, _) =>
            {
                attempts++;
                ScopedCounter counter = serviceProvider.GetRequiredService<ScopedCounter>();
                counter.Count++;

                if (attempts == 1)
                {
                    throw new InvalidOperationException("temporary");
                }

                return ValueTask.CompletedTask;
            }));

        await processor.ProcessNextAsync();

        Assert.Equal(2, attempts);
        Assert.Empty(poisonHandler.Jobs);
        Assert.Contains(logger.Entries, entry => entry.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task QueuedBackgroundJobProcessor_SendsPoisonMessage_WhenRetriesAreExhausted()
    {
        var queue = new ChannelBackgroundJobQueue(
            Microsoft.Extensions.Options.Options.Create(new BackgroundJobQueueOptions()));
        var services = new ServiceCollection();
        using ServiceProvider provider = services.BuildServiceProvider();
        var poisonHandler = new RecordingPoisonHandler();
        TestLogger<QueuedBackgroundJobProcessor> logger = new();
        var processor = new QueuedBackgroundJobProcessor(
            queue,
            provider.GetRequiredService<IServiceScopeFactory>(),
            poisonHandler,
            logger);

        await queue.EnqueueAsync(new BackgroundJob(
            "poison-job",
            static (_, _) => throw new InvalidOperationException("permanent"),
            MaxAttempts: 2));

        await processor.ProcessNextAsync();

        BackgroundJob job = Assert.Single(poisonHandler.Jobs);
        Assert.Equal("poison-job", job.Id);
        Assert.Contains(logger.Entries, entry => entry.Level == LogLevel.Error);
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

    private sealed class ScopedCounter
    {
        public int Count { get; set; }
    }

    private sealed class RecordingPoisonHandler : IBackgroundJobPoisonHandler
    {
        private readonly List<BackgroundJob> jobs = [];

        public IReadOnlyList<BackgroundJob> Jobs => jobs;

        public ValueTask HandleAsync(BackgroundJob job, Exception exception, CancellationToken cancellationToken)
        {
            jobs.Add(job);
            return ValueTask.CompletedTask;
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
