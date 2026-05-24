namespace DotnetBackendSnippets.LanguageFeatures;

public static class AdvancedCollectionSamples
{
    public static int[] CreateFixedLengthBuffer(int length, int initialValue)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be zero or greater.");
        }

        if (length == 0)
        {
            return Array.Empty<int>();
        }

        var buffer = new int[length];
        Array.Fill(buffer, initialValue);

        return buffer;
    }

    public static IReadOnlyList<string> ProcessQueue(IEnumerable<string> jobs)
    {
        ArgumentNullException.ThrowIfNull(jobs);

        var queue = new Queue<string>(jobs);
        List<string> processed = [];

        while (queue.TryDequeue(out var job))
        {
            processed.Add(job);
        }

        return processed;
    }

    public static IReadOnlyList<string> PopUndoStack(IEnumerable<string> actions)
    {
        ArgumentNullException.ThrowIfNull(actions);

        var stack = new Stack<string>();
        foreach (var action in actions)
        {
            stack.Push(action);
        }

        List<string> undoOrder = [];
        while (stack.TryPop(out var action))
        {
            undoOrder.Add(action);
        }

        return undoOrder;
    }
}
