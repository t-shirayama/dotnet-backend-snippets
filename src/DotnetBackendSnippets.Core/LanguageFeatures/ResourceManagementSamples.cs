namespace DotnetBackendSnippets.LanguageFeatures;

public static class ResourceManagementSamples
{
    public static IReadOnlyList<string> UseDisposableResource(Action<RecordingResource> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        using var resource = new RecordingResource();
        action(resource);

        return resource.Events;
    }

    public static IEnumerable<string> ReadUntilBlankLine(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                yield break;
            }

            yield return line;
        }
    }
}

public sealed class RecordingResource : IDisposable
{
    private readonly List<string> events = ["opened"];

    public IReadOnlyList<string> Events => events;

    public bool IsDisposed { get; private set; }

    public void Write(string value)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        events.Add(value);
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            events.Add("disposed");
            IsDisposed = true;
        }
    }
}
