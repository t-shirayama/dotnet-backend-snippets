using System.Net;
using Microsoft.AspNetCore.Http;

namespace DotnetBackendSnippets.HttpClientFactory;

/// <summary>
/// 外部 API 失敗の分類を表します。
/// </summary>
public enum ExternalApiFailureKind
{
    /// <summary>
    /// 失敗ではありません。
    /// </summary>
    None,

    /// <summary>
    /// 一時的な失敗です。
    /// </summary>
    Transient,

    /// <summary>
    /// 呼び出し側の修正が必要な失敗です。
    /// </summary>
    Client,

    /// <summary>
    /// 認証・認可の失敗です。
    /// </summary>
    Authentication,

    /// <summary>
    /// timeout です。
    /// </summary>
    Timeout,
}

/// <summary>
/// retry / backoff の設定を表します。
/// </summary>
/// <param name="MaxAttempts">最大試行回数。</param>
/// <param name="BaseDelay">指数 backoff の基準待機時間。</param>
/// <param name="MaxDelay">最大待機時間。</param>
public sealed record RetryBackoffOptions(int MaxAttempts, TimeSpan BaseDelay, TimeSpan MaxDelay);

/// <summary>
/// circuit breaker の状態を表します。
/// </summary>
public enum CircuitBreakerState
{
    /// <summary>
    /// 通常状態です。
    /// </summary>
    Closed,

    /// <summary>
    /// 呼び出しを遮断している状態です。
    /// </summary>
    Open,
}

/// <summary>
/// 外部 API の失敗分類、retry、circuit breaker、correlation id 伝播のサンプルです。
/// </summary>
public static class ExternalApiResilienceSamples
{
    /// <summary>
    /// HTTP レスポンスや例外を外部 API 失敗種別に分類します。
    /// </summary>
    /// <param name="statusCode">HTTP ステータスコード。</param>
    /// <param name="exception">発生した例外。</param>
    /// <returns>失敗種別。</returns>
    public static ExternalApiFailureKind ClassifyFailure(HttpStatusCode? statusCode, Exception? exception = null)
    {
        if (exception is TimeoutException or TaskCanceledException)
        {
            return ExternalApiFailureKind.Timeout;
        }

        if (statusCode is null || ((int)statusCode >= 200 && (int)statusCode <= 299))
        {
            return ExternalApiFailureKind.None;
        }

        int code = (int)statusCode.Value;

        return code switch
        {
            (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden => ExternalApiFailureKind.Authentication,
            (int)HttpStatusCode.RequestTimeout or (int)HttpStatusCode.TooManyRequests => ExternalApiFailureKind.Transient,
            >= 500 => ExternalApiFailureKind.Transient,
            >= 400 and < 500 => ExternalApiFailureKind.Client,
            _ => ExternalApiFailureKind.Transient,
        };
    }

    /// <summary>
    /// 指数 backoff の待機時間を計算します。
    /// </summary>
    /// <param name="attempt">1 始まりの試行回数。</param>
    /// <param name="options">retry / backoff 設定。</param>
    /// <returns>待機時間。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="attempt"/> または <paramref name="options"/> が不正な場合。</exception>
    public static TimeSpan CalculateExponentialBackoffDelay(int attempt, RetryBackoffOptions options)
    {
        ValidateRetryBackoffOptions(options);

        if (attempt < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(attempt), "Attempt must be one or greater.");
        }

        if (options.BaseDelay == TimeSpan.Zero || options.MaxDelay == TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        double multiplier = Math.Pow(2, attempt - 1);
        double calculatedMilliseconds = options.BaseDelay.TotalMilliseconds * multiplier;
        double milliseconds = double.IsInfinity(calculatedMilliseconds)
            ? options.MaxDelay.TotalMilliseconds
            : Math.Min(calculatedMilliseconds, options.MaxDelay.TotalMilliseconds);

        return TimeSpan.FromMilliseconds(milliseconds);
    }

    /// <summary>
    /// 一時的な失敗だけ retry しながら外部 API 呼び出しを実行します。
    /// </summary>
    /// <typeparam name="T">戻り値の型。</typeparam>
    /// <param name="operation">外部 API 呼び出し。</param>
    /// <param name="options">retry / backoff 設定。</param>
    /// <param name="delayAsync">待機処理の差し替え関数。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>外部 API 呼び出し結果。</returns>
    /// <exception cref="ArgumentNullException">必須引数が <see langword="null"/> の場合。</exception>
    public static async Task<ExternalApiResult<T>> ExecuteWithRetryAsync<T>(
        Func<CancellationToken, Task<ExternalApiResult<T>>> operation,
        RetryBackoffOptions options,
        Func<TimeSpan, CancellationToken, Task>? delayAsync = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ValidateRetryBackoffOptions(options);

        delayAsync ??= Task.Delay;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                ExternalApiResult<T> result = await operation(cancellationToken);
                ExternalApiFailureKind failureKind = ClassifyFailure(result.Error?.StatusCode);

                if (failureKind != ExternalApiFailureKind.Transient || attempt >= options.MaxAttempts)
                {
                    return result;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (ClassifyFailure(null, exception) is ExternalApiFailureKind.Timeout && attempt < options.MaxAttempts)
            {
            }

            TimeSpan delay = CalculateExponentialBackoffDelay(attempt, options);
            await delayAsync(delay, cancellationToken);
        }
    }

    private static void ValidateRetryBackoffOptions(RetryBackoffOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.MaxAttempts < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Max attempts must be positive.");
        }

        if (options.BaseDelay < TimeSpan.Zero || options.MaxDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Delays must be zero or greater.");
        }
    }
}

/// <summary>
/// 連続失敗回数で開く、テストしやすい circuit breaker です。
/// </summary>
public sealed class SimpleCircuitBreaker
{
    private readonly int failureThreshold;
    private int consecutiveFailures;

    /// <summary>
    /// <see cref="SimpleCircuitBreaker"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="failureThreshold">開くまでの連続失敗回数。</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="failureThreshold"/> が 1 未満の場合。</exception>
    public SimpleCircuitBreaker(int failureThreshold)
    {
        if (failureThreshold < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(failureThreshold), "Failure threshold must be positive.");
        }

        this.failureThreshold = failureThreshold;
    }

    /// <summary>
    /// 現在の circuit breaker 状態を取得します。
    /// </summary>
    /// <value>現在の状態。</value>
    public CircuitBreakerState State { get; private set; } = CircuitBreakerState.Closed;

    /// <summary>
    /// 呼び出し結果を記録します。
    /// </summary>
    /// <param name="isSuccess">成功した場合は <see langword="true"/>。</param>
    public void RecordResult(bool isSuccess)
    {
        if (isSuccess)
        {
            consecutiveFailures = 0;
            State = CircuitBreakerState.Closed;
            return;
        }

        consecutiveFailures++;

        if (consecutiveFailures >= failureThreshold)
        {
            State = CircuitBreakerState.Open;
        }
    }
}

/// <summary>
/// HTTP リクエストへ correlation id を伝播します。
/// </summary>
/// <param name="httpContextAccessor">現在の HTTP コンテキスト取得元。</param>
public sealed class CorrelationIdPropagationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? correlationId = httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(correlationId) && !request.Headers.Contains("X-Correlation-ID"))
        {
            request.Headers.TryAddWithoutValidation("X-Correlation-ID", correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
