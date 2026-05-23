using Microsoft.Extensions.Logging;

namespace DotnetBackendSnippets.Tests.Logging;

public sealed record LogEntry(LogLevel Level, string Message);

public sealed class ListLogger : ILogger
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
        entries.Add(new LogEntry(logLevel, formatter(state, exception)));
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
