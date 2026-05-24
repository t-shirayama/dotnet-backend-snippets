using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.BackgroundServices;

/// <summary>
/// キュー投入されたバックグラウンドジョブを表します。
/// </summary>
/// <param name="Id">ジョブ ID。</param>
/// <param name="ExecuteAsync">scoped service provider と cancellation token を受け取る処理。</param>
/// <param name="MaxAttempts">最大試行回数。</param>
public sealed record BackgroundJob(
    string Id,
    Func<IServiceProvider, CancellationToken, ValueTask> ExecuteAsync,
    int MaxAttempts = 3);

/// <summary>
/// poison message を処理するためのインターフェースです。
/// </summary>
public interface IBackgroundJobPoisonHandler
{
    /// <summary>
    /// 最大試行回数を超えたジョブを処理します。
    /// </summary>
    /// <param name="job">失敗したジョブ。</param>
    /// <param name="exception">最後に発生した例外。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    ValueTask HandleAsync(BackgroundJob job, Exception exception, CancellationToken cancellationToken);
}

/// <summary>
/// バックグラウンドジョブキューの設定です。
/// </summary>
public sealed class BackgroundJobQueueOptions
{
    /// <summary>
    /// キュー容量を取得または設定します。
    /// </summary>
    /// <value>bounded channel の最大件数。</value>
    public int Capacity { get; set; } = 100;
}

/// <summary>
/// channel-based のバックグラウンドジョブキューです。
/// </summary>
public sealed class ChannelBackgroundJobQueue
{
    private readonly Channel<BackgroundJob> channel;

    /// <summary>
    /// <see cref="ChannelBackgroundJobQueue"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="options">キュー設定。</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException">容量が 1 未満の場合。</exception>
    public ChannelBackgroundJobQueue(IOptions<BackgroundJobQueueOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Value.Capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Queue capacity must be positive.");
        }

        channel = Channel.CreateBounded<BackgroundJob>(new BoundedChannelOptions(options.Value.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        });
    }

    /// <summary>
    /// ジョブをキューに投入します。
    /// </summary>
    /// <param name="job">投入するジョブ。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="job"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException">ジョブ ID が空白、または最大試行回数が 1 未満の場合。</exception>
    public async ValueTask EnqueueAsync(BackgroundJob job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);
        ArgumentException.ThrowIfNullOrWhiteSpace(job.Id);

        if (job.MaxAttempts < 1)
        {
            throw new ArgumentException("Max attempts must be positive.", nameof(job));
        }

        await channel.Writer.WriteAsync(job, cancellationToken);
    }

    /// <summary>
    /// 次のジョブを取り出します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>取り出したジョブ。</returns>
    public ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return channel.Reader.ReadAsync(cancellationToken);
    }
}

/// <summary>
/// キューからジョブを取り出して scoped service provider 内で実行します。
/// </summary>
public sealed class QueuedBackgroundJobProcessor
{
    private readonly ChannelBackgroundJobQueue queue;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IBackgroundJobPoisonHandler poisonHandler;
    private readonly ILogger<QueuedBackgroundJobProcessor> logger;

    /// <summary>
    /// <see cref="QueuedBackgroundJobProcessor"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="queue">ジョブキュー。</param>
    /// <param name="scopeFactory">scope 作成ファクトリ。</param>
    /// <param name="poisonHandler">poison message handler。</param>
    /// <param name="logger">ログ出力に使うロガー。</param>
    public QueuedBackgroundJobProcessor(
        ChannelBackgroundJobQueue queue,
        IServiceScopeFactory scopeFactory,
        IBackgroundJobPoisonHandler poisonHandler,
        ILogger<QueuedBackgroundJobProcessor> logger)
    {
        this.queue = queue ?? throw new ArgumentNullException(nameof(queue));
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        this.poisonHandler = poisonHandler ?? throw new ArgumentNullException(nameof(poisonHandler));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// キューから 1 件取り出して retry つきで実行します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    public async Task ProcessNextAsync(CancellationToken cancellationToken = default)
    {
        BackgroundJob job = await queue.DequeueAsync(cancellationToken);

        for (var attempt = 1; attempt <= job.MaxAttempts; attempt++)
        {
            try
            {
                using IServiceScope scope = scopeFactory.CreateScope();
                await job.ExecuteAsync(scope.ServiceProvider, cancellationToken);
                return;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (attempt < job.MaxAttempts)
            {
                logger.LogWarning(exception, "Background job {JobId} failed on attempt {Attempt}.", job.Id, attempt);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Background job {JobId} moved to poison handler.", job.Id);
                await poisonHandler.HandleAsync(job, exception, cancellationToken);
            }
        }
    }
}

/// <summary>
/// キュー処理を hosted service として実行します。
/// </summary>
/// <param name="processor">ジョブ処理器。</param>
public sealed class QueuedBackgroundJobHostedService(QueuedBackgroundJobProcessor processor) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await processor.ProcessNextAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}

/// <summary>
/// poison message をログだけに流す既定実装です。
/// </summary>
/// <param name="logger">ログ出力に使うロガー。</param>
public sealed class LoggingPoisonHandler(ILogger<LoggingPoisonHandler> logger) : IBackgroundJobPoisonHandler
{
    /// <inheritdoc />
    public ValueTask HandleAsync(BackgroundJob job, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Background job {JobId} failed permanently.", job.Id);
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// channel-based queue の DI 登録サンプルです。
/// </summary>
public static class BackgroundQueueSamples
{
    /// <summary>
    /// バックグラウンドジョブキューと hosted service を登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="configureOptions">キュー設定を変更する処理。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddBackgroundJobQueue(
        this IServiceCollection services,
        Action<BackgroundJobQueueOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLogging();
        services.AddSingleton<ChannelBackgroundJobQueue>();
        services.AddSingleton<QueuedBackgroundJobProcessor>();
        services.AddSingleton<IBackgroundJobPoisonHandler, LoggingPoisonHandler>();
        services.AddHostedService<QueuedBackgroundJobHostedService>();

        if (configureOptions is null)
        {
            services.AddOptions<BackgroundJobQueueOptions>();
        }
        else
        {
            services.Configure(configureOptions);
        }

        return services;
    }
}
