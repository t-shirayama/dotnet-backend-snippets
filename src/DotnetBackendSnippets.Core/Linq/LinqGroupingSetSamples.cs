namespace DotnetBackendSnippets.Linq;

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class LinqReverseLookupSamples
{
    /// <summary>
    /// キーごとの件数を数えます。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>キーごとの件数。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyDictionary<TKey, int> CountByKey<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups.ToDictionary(group => group.Key, group => group.Count(), comparer);
    }

    /// <summary>
    /// キーごとに最新の要素を取得します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="timestampSelector">比較する日時を取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>キーごとの最新要素。</returns>
    /// <exception cref="ArgumentNullException">必須引数が null です。</exception>
    public static IReadOnlyList<T> LatestPerKey<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<T, DateTimeOffset> timestampSelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(timestampSelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups.Select(group => group.MaxBy(timestampSelector)!).ToList();
    }

    /// <summary>
    /// 重複しているキーを取得します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>重複キーの一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyList<TKey> FindDuplicateKeys<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var groups = comparer is null
            ? source.GroupBy(keySelector)
            : source.GroupBy(keySelector, comparer);

        return groups
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
    }
}

