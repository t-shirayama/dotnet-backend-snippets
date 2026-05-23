using DotnetBackendSnippets.Observability;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Tests.Observability;

public sealed class ObservabilitySamplesTests
{
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

    [Fact]
    public void GetOrCreateCorrelationId_CreatesValue_WhenHeaderIsMissing()
    {
        HeaderDictionary headers = [];

        string correlationId = ObservabilitySamples.GetOrCreateCorrelationId(headers);

        Assert.Equal(32, correlationId.Length);
        Assert.True(Guid.TryParseExact(correlationId, "N", out _));
    }

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

    [Fact]
    public async Task MeasureAsync_ReturnsValueAndElapsedTime()
    {
        TimedOperationResult<string> result = await ObservabilitySamples.MeasureAsync(
            static _ => Task.FromResult("ok"));

        Assert.Equal("ok", result.Value);
        Assert.True(result.Elapsed >= TimeSpan.Zero);
    }

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
