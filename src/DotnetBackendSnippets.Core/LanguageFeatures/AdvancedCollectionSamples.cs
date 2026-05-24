namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// 配列、キュー、スタックなどのコレクション機能のサンプルを提供します。
/// </summary>
public static class AdvancedCollectionSamples
{
    /// <summary>
    /// 指定した長さと初期値で固定長の配列を作成します。
    /// </summary>
    /// <param name="length">作成する配列の長さ。</param>
    /// <param name="initialValue">各要素に設定する初期値。</param>
    /// <returns>初期化済みの整数配列。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> が 0 未満です。</exception>
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

    /// <summary>
    /// キューを使ってジョブを先入れ先出し順に処理します。
    /// </summary>
    /// <param name="jobs">処理対象のジョブ名。</param>
    /// <returns>処理されたジョブ名の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="jobs"/> が null です。</exception>
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

    /// <summary>
    /// スタックを使って操作を後入れ先出し順に取り出します。
    /// </summary>
    /// <param name="actions">積み上げる操作名。</param>
    /// <returns>取り出された操作名の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="actions"/> が null です。</exception>
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
