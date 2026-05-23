using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.BackgroundServices;

public interface IBackgroundWorker
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

public sealed class BackgroundWorkerLoopOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);

    public bool ContinueOnError { get; set; } = true;
}

public sealed class BackgroundWorkerLoop
{
    private readonly IBackgroundWorker worker;
    private readonly ILogger<BackgroundWorkerLoop> logger;
    private readonly Func<TimeSpan, CancellationToken, Task> delayAsync;

    public BackgroundWorkerLoop(
        IBackgroundWorker worker,
        ILogger<BackgroundWorkerLoop> logger,
        Func<TimeSpan, CancellationToken, Task>? delayAsync = null)
    {
        this.worker = worker ?? throw new ArgumentNullException(nameof(worker));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.delayAsync = delayAsync ?? Task.Delay;
    }

    public async Task RunUntilCancelledAsync(
        BackgroundWorkerLoopOptions options,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Interval must be zero or greater.");
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            bool shouldContinue = await RunOnceAsync(options.ContinueOnError, cancellationToken);

            if (!shouldContinue || cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await delayAsync(options.Interval, cancellationToken);
        }
    }

    public async Task<bool> RunOnceAsync(bool continueOnError, CancellationToken cancellationToken = default)
    {
        try
        {
            await worker.ExecuteAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        catch (Exception exception) when (continueOnError)
        {
            logger.LogError(exception, "Background worker iteration failed.");
            return true;
        }
    }
}

public sealed class WorkerLoopHostedService(
    BackgroundWorkerLoop loop,
    IOptions<BackgroundWorkerLoopOptions> options)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return loop.RunUntilCancelledAsync(options.Value, stoppingToken);
    }
}

public static class BackgroundServiceSamples
{
    public static IServiceCollection AddBackgroundWorker<TWorker>(
        this IServiceCollection services,
        Action<BackgroundWorkerLoopOptions>? configureOptions = null)
        where TWorker : class, IBackgroundWorker
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLogging();
        services.AddSingleton<IBackgroundWorker, TWorker>();
        services.AddSingleton<BackgroundWorkerLoop>();
        services.AddHostedService<WorkerLoopHostedService>();

        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.AddOptions<BackgroundWorkerLoopOptions>();
        }

        return services;
    }
}
