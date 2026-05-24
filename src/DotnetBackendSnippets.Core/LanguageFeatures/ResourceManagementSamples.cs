namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// using と yield を使ったリソース管理のサンプルを提供します。
/// </summary>
public static class ResourceManagementSamples
{
    /// <summary>
    /// disposable なリソースを using で扱い、イベント履歴を返します。
    /// </summary>
    /// <param name="action">リソースに対して実行する処理。</param>
    /// <returns>リソースのイベント履歴。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> が null です。</exception>
    public static IReadOnlyList<string> UseDisposableResource(Action<RecordingResource> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        using var resource = new RecordingResource();
        action(resource);

        return resource.Events;
    }

    /// <summary>
    /// 空行に到達するまで行を遅延列挙します。
    /// </summary>
    /// <param name="lines">入力行。</param>
    /// <returns>空行の前までの行。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lines"/> が null です。</exception>
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

/// <summary>
/// 書き込みと破棄の履歴を記録する disposable リソースです。
/// </summary>
public sealed class RecordingResource : IDisposable
{
    private readonly List<string> events = ["opened"];

    /// <summary>
    /// 記録されたイベント履歴を取得します。
    /// </summary>
    /// <value>イベント履歴。</value>
    public IReadOnlyList<string> Events => events;

    /// <summary>
    /// 破棄済みかどうかを取得します。
    /// </summary>
    /// <value>破棄済みの場合は true。</value>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// 値を書き込み履歴に追加します。
    /// </summary>
    /// <param name="value">記録する値。</param>
    /// <exception cref="ObjectDisposedException">リソースが破棄済みです。</exception>
    public void Write(string value)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);

        events.Add(value);
    }

    /// <summary>
    /// リソースを破棄し、破棄イベントを記録します。
    /// </summary>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            events.Add("disposed");
            IsDisposed = true;
        }
    }
}
