using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Observability;

public sealed record TimedOperationResult<T>(T Value, TimeSpan Elapsed);

public static class ObservabilitySamples
{
    public const string CorrelationIdHeaderName = "X-Correlation-ID";

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

    public static IDisposable? BeginOperationScope(
        ILogger logger,
        string correlationId,
        string operationName,
        string? userId = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        return logger.BeginScope(CreateScopeState(correlationId, operationName, userId));
    }

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

    public static HealthCheckResult CreateHealthCheckResult(
        string dependencyName,
        bool isHealthy,
        TimeSpan elapsed,
        Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dependencyName);

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
