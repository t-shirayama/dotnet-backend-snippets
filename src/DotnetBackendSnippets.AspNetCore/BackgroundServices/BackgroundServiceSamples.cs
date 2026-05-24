using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.BackgroundServices;

/// <summary>
/// 定期実行されるバックグラウンド処理を表します。
/// </summary>
public interface IBackgroundWorker
{
    /// <summary>
    /// 1 回分のバックグラウンド処理を実行します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// バックグラウンド処理ループの設定です。
/// </summary>
public sealed class BackgroundWorkerLoopOptions
{
    /// <summary>
    /// ループ間隔を取得または設定します。
    /// </summary>
    /// <value>各実行の後に待機する時間。</value>
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// 例外発生後も継続するかを取得または設定します。
    /// </summary>
    /// <value>継続する場合は <see langword="true"/>。</value>
    public bool ContinueOnError { get; set; } = true;
}

/// <summary>
/// バックグラウンド処理を一定間隔で実行します。
/// </summary>
public sealed class BackgroundWorkerLoop
{
    private readonly IBackgroundWorker worker;
    private readonly ILogger<BackgroundWorkerLoop> logger;
    private readonly Func<TimeSpan, CancellationToken, Task> delayAsync;

    /// <summary>
    /// <see cref="BackgroundWorkerLoop"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="worker">実行するワーカー。</param>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <param name="delayAsync">待機処理の差し替え関数。</param>
    /// <exception cref="ArgumentNullException"><paramref name="worker"/> または <paramref name="logger"/> が <see langword="null"/> の場合。</exception>
    public BackgroundWorkerLoop(
        IBackgroundWorker worker,
        ILogger<BackgroundWorkerLoop> logger,
        Func<TimeSpan, CancellationToken, Task>? delayAsync = null)
    {
        this.worker = worker ?? throw new ArgumentNullException(nameof(worker));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.delayAsync = delayAsync ?? Task.Delay;
    }

    /// <summary>
    /// キャンセルされるまでワーカーを繰り返し実行します。
    /// </summary>
    /// <param name="options">ループ設定。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException">間隔が負の値の場合。</exception>
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

    /// <summary>
    /// ワーカーを 1 回実行します。
    /// </summary>
    /// <param name="continueOnError">例外発生時に継続可能として扱うかどうか。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>次の実行を続けてよい場合は <see langword="true"/>。</returns>
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

/// <summary>
/// <see cref="BackgroundWorkerLoop"/> をホストサービスとして実行します。
/// </summary>
/// <param name="loop">実行するループ。</param>
/// <param name="options">ループ設定のオプション。</param>
public sealed class WorkerLoopHostedService(
    BackgroundWorkerLoop loop,
    IOptions<BackgroundWorkerLoopOptions> options)
    : BackgroundService
{
    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return loop.RunUntilCancelledAsync(options.Value, stoppingToken);
    }
}

/// <summary>
/// バックグラウンドサービス登録のサンプルです。
/// </summary>
public static class BackgroundServiceSamples
{
    /// <summary>
    /// ワーカーとホストサービスを DI コンテナーに登録します。
    /// </summary>
    /// <typeparam name="TWorker">登録するワーカー型。</typeparam>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="configureOptions">ループ設定を変更する処理。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
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
