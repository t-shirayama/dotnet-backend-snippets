namespace DotnetBackendSnippets.Collections;

public static class CollectionSamples
{
    public static void IncrementCount<TKey>(IDictionary<TKey, int> counts, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(counts);

        counts[key] = counts.TryGetValue(key, out var current) ? current + 1 : 1;
    }

    public static IReadOnlyList<T> FindDuplicates<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source
            .GroupBy(item => item)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
    }

    public static IReadOnlyList<T> FindDuplicatesOnePass<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var seen = new HashSet<T>();
        var duplicates = new HashSet<T>();
        var result = new List<T>();

        foreach (var item in source)
        {
            if (!seen.Add(item) && duplicates.Add(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue> first,
        IReadOnlyDictionary<TKey, TValue> second)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        var merged = new Dictionary<TKey, TValue>(first);

        foreach (var (key, value) in second)
        {
            merged[key] = value;
        }

        return merged;
    }

    public static TValue GetRequired<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        if (!dictionary.TryGetValue(key, out var value))
        {
            throw new KeyNotFoundException($"Required key '{key}' was not found.");
        }

        return value;
    }

    public static bool AddIfMissing<T>(ICollection<T> collection, T value)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (collection.Contains(value))
        {
            return false;
        }

        collection.Add(value);
        return true;
    }

    public static bool AddIfMissing<T>(ISet<T> set, T value)
    {
        ArgumentNullException.ThrowIfNull(set);

        return set.Add(value);
    }

    public static IReadOnlyList<IReadOnlyList<T>> ChunkBySize<T>(IEnumerable<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Chunk size must be one or greater.");
        }

        return source
            .Select((item, index) => new { item, index })
            .GroupBy(pair => pair.index / size)
            .Select(group => (IReadOnlyList<T>)group.Select(pair => pair.item).ToList())
            .ToList();
    }
}
