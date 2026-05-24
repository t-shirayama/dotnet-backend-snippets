namespace DotnetBackendSnippets.Linq;

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class LinqReverseLookupSamples
{
    private static IEnumerable<IGrouping<TKey, T>> GroupByKey<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
        where TKey : notnull
    {
        return comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);
    }
}
