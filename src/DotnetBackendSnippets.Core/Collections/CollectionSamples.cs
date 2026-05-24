namespace DotnetBackendSnippets.Collections;

/// <summary>
/// コレクション操作でよく使う実装例を提供します。
/// </summary>
public static class CollectionSamples
{
    /// <summary>
    /// 指定キーの件数を 1 増やし、存在しない場合は 1 で追加します。
    /// </summary>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <param name="counts">件数を保持する辞書。</param>
    /// <param name="key">増加対象のキー。</param>
    /// <exception cref="ArgumentNullException"><paramref name="counts"/> が <see langword="null"/> の場合。</exception>
    public static void IncrementCount<TKey>(IDictionary<TKey, int> counts, TKey key)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(counts);

        counts[key] = counts.TryGetValue(key, out var current) ? current + 1 : 1;
    }

    /// <summary>
    /// 入力内で重複している要素を抽出します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">確認対象の要素一覧。</param>
    /// <returns>重複している要素の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<T> FindDuplicates<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source
            .GroupBy(item => item)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();
    }

    /// <summary>
    /// 1 回の走査で重複要素を抽出します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">確認対象の要素一覧。</param>
    /// <returns>初めて重複した順の重複要素一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
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

    /// <summary>
    /// 2 つの辞書を結合し、後勝ちで値を上書きします。
    /// </summary>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <typeparam name="TValue">値の型。</typeparam>
    /// <param name="first">基準になる辞書。</param>
    /// <param name="second">上書きする辞書。</param>
    /// <returns>結合後の新しい辞書。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> または <paramref name="second"/> が <see langword="null"/> の場合。</exception>
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

    /// <summary>
    /// 必須キーの値を取得し、存在しなければ例外を投げます。
    /// </summary>
    /// <typeparam name="TKey">キーの型。</typeparam>
    /// <typeparam name="TValue">値の型。</typeparam>
    /// <param name="dictionary">参照する辞書。</param>
    /// <param name="key">取得するキー。</param>
    /// <returns>キーに対応する値。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="KeyNotFoundException"><paramref name="key"/> が存在しない場合。</exception>
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

    /// <summary>
    /// コレクションに値がなければ追加します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="collection">追加対象のコレクション。</param>
    /// <param name="value">追加する値。</param>
    /// <returns>追加した場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> が <see langword="null"/> の場合。</exception>
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

    /// <summary>
    /// セットに値がなければ追加します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="set">追加対象のセット。</param>
    /// <param name="value">追加する値。</param>
    /// <returns>追加した場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="set"/> が <see langword="null"/> の場合。</exception>
    public static bool AddIfMissing<T>(ISet<T> set, T value)
    {
        ArgumentNullException.ThrowIfNull(set);

        return set.Add(value);
    }

    /// <summary>
    /// 要素一覧を指定サイズごとのチャンクに分割します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">分割対象の要素一覧。</param>
    /// <param name="size">1 チャンクあたりの最大件数。</param>
    /// <returns>チャンクの一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> が 1 未満の場合。</exception>
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
