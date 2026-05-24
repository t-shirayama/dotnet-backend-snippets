namespace DotnetBackendSnippets.Linq;

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class LinqReverseLookupSamples
{
    /// <summary>
    /// 親要素と子要素の組み合わせから結果を作成します。
    /// </summary>
    /// <typeparam name="TParent">親要素の型。</typeparam>
    /// <typeparam name="TChild">子要素の型。</typeparam>
    /// <typeparam name="TResult">結果の型。</typeparam>
    /// <param name="source">親要素の入力シーケンス。</param>
    /// <param name="childrenSelector">親から子要素を取り出す関数。</param>
    /// <param name="resultSelector">親と子から結果を作る関数。</param>
    /// <returns>平坦化された結果一覧。</returns>
    /// <exception cref="ArgumentNullException">必須引数が null です。</exception>
    public static IReadOnlyList<TResult> SelectManyWithParent<TParent, TChild, TResult>(
        IEnumerable<TParent> source,
        Func<TParent, IEnumerable<TChild>?> childrenSelector,
        Func<TParent, TChild, TResult> resultSelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(childrenSelector);
        ArgumentNullException.ThrowIfNull(resultSelector);

        var result = new List<TResult>();

        foreach (var parent in source)
        {
            var children = childrenSelector(parent);
            if (children is null)
            {
                continue;
            }

            result.AddRange(children.Select(child => resultSelector(parent, child)));
        }

        return result;
    }
}

