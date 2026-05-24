namespace DotnetBackendSnippets.Linq;

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class LinqReverseLookupSamples
{
    /// <summary>
    /// 同じキーでは後勝ちにして辞書を作成します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>後勝ちで作成した辞書。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyDictionary<TKey, T> ToDictionaryLastWins<T, TKey>(
        IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var dictionary = comparer is null
            ? new Dictionary<TKey, T>()
            : new Dictionary<TKey, T>(comparer);

        foreach (var item in source)
        {
            dictionary[keySelector(item)] = item;
        }

        return dictionary;
    }

    /// <summary>
    /// キーごとの要素一覧を持つ辞書を作成します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="source">入力シーケンス。</param>
    /// <param name="keySelector">キーを取り出す関数。</param>
    /// <param name="comparer">任意のキー比較器。</param>
    /// <returns>キーごとの要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> または <paramref name="keySelector"/> が null です。</exception>
    public static IReadOnlyDictionary<TKey, IReadOnlyList<T>> ToLookupDictionary<T, TKey>(
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

        return groups.ToDictionary(
            group => group.Key,
            group => (IReadOnlyList<T>)group.ToList(),
            comparer);
    }
}

