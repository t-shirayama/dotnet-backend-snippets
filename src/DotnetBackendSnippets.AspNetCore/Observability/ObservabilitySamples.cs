using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Observability;

/// <summary>
/// 実行結果と経過時間を表します。
/// </summary>
/// <typeparam name="T">実行結果の型。</typeparam>
/// <param name="Value">実行結果。</param>
/// <param name="Elapsed">経過時間。</param>
public sealed record TimedOperationResult<T>(T Value, TimeSpan Elapsed);

/// <summary>
/// ログ、相関 ID、ヘルスチェックのサンプルです。
/// </summary>
public static class ObservabilitySamples
{
    /// <summary>
    /// 相関 ID を受け渡す HTTP ヘッダー名です。
    /// </summary>
    public const string CorrelationIdHeaderName = "X-Correlation-ID";

    /// <summary>
    /// 操作失敗を相関 ID とともにログ出力します。
    /// </summary>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <param name="operationName">操作名。</param>
    /// <param name="exception">発生した例外。</param>
    /// <param name="correlationId">相関 ID。</param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> または <paramref name="exception"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="operationName"/> が空白の場合。</exception>
    public static void LogOperationError(
        ILogger logger,
        string operationName,
        Exception exception,
        string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        ArgumentNullException.ThrowIfNull(exception);

        logger.LogError(
            exception,
            "Operation {OperationName} failed. CorrelationId: {CorrelationId}",
            operationName,
            correlationId ?? "none");
    }

    /// <summary>
    /// ログスコープに渡す状態を作成します。
    /// </summary>
    /// <param name="correlationId">相関 ID。</param>
    /// <param name="operationName">操作名。</param>
    /// <param name="userId">ユーザー ID。</param>
    /// <returns>ログスコープの状態。</returns>
    /// <exception cref="ArgumentException"><paramref name="correlationId"/> または <paramref name="operationName"/> が空白の場合。</exception>
    public static IReadOnlyDictionary<string, object> CreateScopeState(
        string correlationId,
        string operationName,
        string? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);

        Dictionary<string, object> state = new(StringComparer.Ordinal)
        {
            ["CorrelationId"] = correlationId,
            ["OperationName"] = operationName,
        };

        if (!string.IsNullOrWhiteSpace(userId))
        {
            state["UserId"] = userId;
        }

        return state;
    }

    /// <summary>
    /// 操作用のログスコープを開始します。
    /// </summary>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <param name="correlationId">相関 ID。</param>
    /// <param name="operationName">操作名。</param>
    /// <param name="userId">ユーザー ID。</param>
    /// <returns>スコープを終了するためのオブジェクト。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="correlationId"/> または <paramref name="operationName"/> が空白の場合。</exception>
    public static IDisposable? BeginOperationScope(
        ILogger logger,
        string correlationId,
        string operationName,
        string? userId = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        return logger.BeginScope(CreateScopeState(correlationId, operationName, userId));
    }

    /// <summary>
    /// ヘッダーから相関 ID を取得し、なければ新しく作成します。
    /// </summary>
    /// <param name="headers">HTTP ヘッダー。</param>
    /// <param name="headerName">相関 ID のヘッダー名。</param>
    /// <returns>取得または作成した相関 ID。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="headers"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="headerName"/> が空白の場合。</exception>
    public static string GetOrCreateCorrelationId(
        IHeaderDictionary headers,
        string headerName = CorrelationIdHeaderName)
    {
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentException.ThrowIfNullOrWhiteSpace(headerName);

        if (headers.TryGetValue(headerName, out var values))
        {
            string? value = values.FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 依存先のヘルスチェック結果を作成します。
    /// </summary>
    /// <param name="dependencyName">依存先名。</param>
    /// <param name="isHealthy">正常かどうか。</param>
    /// <param name="elapsed">確認にかかった時間。</param>
    /// <param name="exception">異常時の例外。</param>
    /// <returns>ヘルスチェック結果。</returns>
    /// <exception cref="ArgumentException"><paramref name="dependencyName"/> が空白の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="elapsed"/> が負の値の場合。</exception>
    public static HealthCheckResult CreateHealthCheckResult(
        string dependencyName,
        bool isHealthy,
        TimeSpan elapsed,
        Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dependencyName);

        if (elapsed < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(elapsed), "Elapsed time must be zero or greater.");
        }

        Dictionary<string, object> data = new(StringComparer.Ordinal)
        {
            ["Dependency"] = dependencyName,
            ["ElapsedMilliseconds"] = elapsed.TotalMilliseconds,
        };

        if (isHealthy)
        {
            return HealthCheckResult.Healthy($"{dependencyName} is healthy.", data);
        }

        return HealthCheckResult.Unhealthy($"{dependencyName} is unhealthy.", exception, data);
    }

    /// <summary>
    /// 非同期処理の実行時間を計測します。
    /// </summary>
    /// <typeparam name="T">実行結果の型。</typeparam>
    /// <param name="operation">計測する非同期処理。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>実行結果と経過時間。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="operation"/> が <see langword="null"/> の場合。</exception>
    public static async Task<TimedOperationResult<T>> MeasureAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        long startedAt = Stopwatch.GetTimestamp();
        T value = await operation(cancellationToken);
        TimeSpan elapsed = Stopwatch.GetElapsedTime(startedAt);

        return new TimedOperationResult<T>(value, elapsed);
    }

    /// <summary>
    /// 非同期処理の実行時間を計測してログ出力します。
    /// </summary>
    /// <typeparam name="T">実行結果の型。</typeparam>
    /// <param name="logger">ログ出力に使うロガー。</param>
    /// <param name="operationName">操作名。</param>
    /// <param name="operation">計測する非同期処理。</param>
    /// <param name="slowThreshold">低速とみなすしきい値。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>実行結果と経過時間。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> または <paramref name="operation"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="operationName"/> が空白の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="slowThreshold"/> が負の値の場合。</exception>
    public static async Task<TimedOperationResult<T>> MeasureAndLogAsync<T>(
        ILogger logger,
        string operationName,
        Func<CancellationToken, Task<T>> operation,
        TimeSpan slowThreshold,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        ArgumentNullException.ThrowIfNull(operation);

        if (slowThreshold < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(slowThreshold), "Slow threshold must be zero or greater.");
        }

        TimedOperationResult<T> result = await MeasureAsync(operation, cancellationToken);

        if (result.Elapsed >= slowThreshold)
        {
            logger.LogWarning(
                "Operation {OperationName} took {ElapsedMilliseconds} ms.",
                operationName,
                result.Elapsed.TotalMilliseconds);
        }
        else
        {
            logger.LogInformation(
                "Operation {OperationName} took {ElapsedMilliseconds} ms.",
                operationName,
                result.Elapsed.TotalMilliseconds);
        }

        return result;
    }
}
