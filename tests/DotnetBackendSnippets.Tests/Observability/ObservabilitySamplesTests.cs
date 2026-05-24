using DotnetBackendSnippets.Observability;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Tests.Observability;

// テスト対象: Observability Samples のスニペット動作を確認する。
public sealed class ObservabilitySamplesTests
{
    // テスト意図: Log Operation Error / Writes Exception And Structured Message を確認する。
    [Fact]
    public void LogOperationError_WritesExceptionAndStructuredMessage()
    {
        CapturingLogger logger = new();
        InvalidOperationException exception = new("database timeout");

        ObservabilitySamples.LogOperationError(logger, "SaveOrder", exception, "corr-123");

        CapturedLogEntry entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Contains("SaveOrder", entry.Message, StringComparison.Ordinal);
        Assert.Contains("corr-123", entry.Message, StringComparison.Ordinal);
    }

    // テスト意図: Begin Operation Scope / Adds Correlation And Operation State を確認する。
    [Fact]
    public void BeginOperationScope_AddsCorrelationAndOperationState()
    {
        CapturingLogger logger = new();

        using (ObservabilitySamples.BeginOperationScope(logger, "corr-123", "ImportOrders", "user-1"))
        {
            logger.LogInformation("inside scope");
        }

        CapturedLogEntry entry = Assert.Single(logger.Entries);
        IReadOnlyDictionary<string, object> scope =
            Assert.IsAssignableFrom<IReadOnlyDictionary<string, object>>(Assert.Single(entry.Scopes));
        Assert.Equal("corr-123", scope["CorrelationId"]);
        Assert.Equal("ImportOrders", scope["OperationName"]);
        Assert.Equal("user-1", scope["UserId"]);
    }

    // テスト意図: Get Or Create Correlation ID / Returns Trimmed Header Value を確認する。
    [Fact]
    public void GetOrCreateCorrelationId_ReturnsTrimmedHeaderValue()
    {
        HeaderDictionary headers = new()
        {
            [ObservabilitySamples.CorrelationIdHeaderName] = " corr-123 ",
        };

        string correlationId = ObservabilitySamples.GetOrCreateCorrelationId(headers);

        Assert.Equal("corr-123", correlationId);
    }

    // テスト意図: Get Or Create Correlation ID / Creates Value / When Header Is Missing を確認する。
    [Fact]
    public void GetOrCreateCorrelationId_CreatesValue_WhenHeaderIsMissing()
    {
        HeaderDictionary headers = [];

        string correlationId = ObservabilitySamples.GetOrCreateCorrelationId(headers);

        Assert.Equal(32, correlationId.Length);
        Assert.True(Guid.TryParseExact(correlationId, "N", out _));
    }

    // テスト意図: Create Health Check Result / Returns Healthy Result With Data を確認する。
    [Fact]
    public void CreateHealthCheckResult_ReturnsHealthyResultWithData()
    {
        TimeSpan elapsed = TimeSpan.FromMilliseconds(12);

        HealthCheckResult result = ObservabilitySamples.CreateHealthCheckResult(
            "database",
            isHealthy: true,
            elapsed);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("database", result.Data["Dependency"]);
        Assert.Equal(12d, result.Data["ElapsedMilliseconds"]);
    }

    // テスト意図: Create Health Check Result / Throws Argument Out Of Range Exception / When Elapsed Is Negative を確認する。
    [Fact]
    public void CreateHealthCheckResult_ThrowsArgumentOutOfRangeException_WhenElapsedIsNegative()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            ObservabilitySamples.CreateHealthCheckResult("database", isHealthy: true, TimeSpan.FromMilliseconds(-1)));

        Assert.Equal("elapsed", exception.ParamName);
    }

    // テスト意図: Measure Async / Returns Value And Elapsed Time を確認する。
    [Fact]
    public async Task MeasureAsync_ReturnsValueAndElapsedTime()
    {
        TimedOperationResult<string> result = await ObservabilitySamples.MeasureAsync(
            static _ => Task.FromResult("ok"));

        Assert.Equal("ok", result.Value);
        Assert.True(result.Elapsed >= TimeSpan.Zero);
    }

    // テスト意図: Measure And Log Async / Logs Warning / When Threshold Is Exceeded を確認する。
    [Fact]
    public async Task MeasureAndLogAsync_LogsWarning_WhenThresholdIsExceeded()
    {
        CapturingLogger logger = new();

        TimedOperationResult<string> result = await ObservabilitySamples.MeasureAndLogAsync(
            logger,
            "RefreshCache",
            static _ => Task.FromResult("ok"),
            TimeSpan.Zero);

        Assert.Equal("ok", result.Value);
        CapturedLogEntry entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains("RefreshCache", entry.Message, StringComparison.Ordinal);
    }

    // テスト意図: Measure And Log Async / Throws Argument Out Of Range Exception / When Threshold Is Negative を確認する。
    [Fact]
    public async Task MeasureAndLogAsync_ThrowsArgumentOutOfRangeException_WhenThresholdIsNegative()
    {
        CapturingLogger logger = new();

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            ObservabilitySamples.MeasureAndLogAsync(
                logger,
                "RefreshCache",
                static _ => Task.FromResult("ok"),
                TimeSpan.FromMilliseconds(-1)));

        Assert.Equal("slowThreshold", exception.ParamName);
    }

    private sealed record CapturedLogEntry(
        LogLevel Level,
        string Message,
        Exception? Exception,
        IReadOnlyList<object> Scopes);

    private sealed class CapturingLogger : ILogger
    {
        private readonly List<CapturedLogEntry> entries = [];
        private readonly List<object> scopes = [];

        public IReadOnlyList<CapturedLogEntry> Entries => entries;

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            scopes.Add(state);
            return new Scope(this, state);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            entries.Add(new CapturedLogEntry(
                logLevel,
                formatter(state, exception),
                exception,
                scopes.ToArray()));
        }

        private sealed class Scope(CapturingLogger logger, object state) : IDisposable
        {
            public void Dispose()
            {
                logger.scopes.Remove(state);
            }
        }
    }
}
