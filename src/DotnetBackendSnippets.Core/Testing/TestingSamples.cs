namespace DotnetBackendSnippets.Testing;

/// <summary>
/// テストの Arrange / Act / Assert を記録する小さな構造です。
/// </summary>
/// <param name="Arrange">前提条件。</param>
/// <param name="Act">実行する操作。</param>
/// <param name="Assert">確認する期待結果。</param>
public sealed record TestCaseNotes(string Arrange, string Act, string Assert);

/// <summary>
/// 通知送信を抽象化するテスト対象インターフェースです。
/// </summary>
public interface INotificationSender
{
    /// <summary>
    /// 通知を送信します。
    /// </summary>
    /// <param name="userId">ユーザー ID。</param>
    /// <param name="message">通知メッセージ。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    Task SendAsync(string userId, string message, CancellationToken cancellationToken);
}

/// <summary>
/// テストしやすい通知サービスです。
/// </summary>
public sealed class ReminderService
{
    private readonly INotificationSender sender;
    private readonly TimeProvider timeProvider;

    /// <summary>
    /// <see cref="ReminderService"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="sender">通知送信先。</param>
    /// <param name="timeProvider">時刻 provider。</param>
    /// <exception cref="ArgumentNullException"><paramref name="sender"/> または <paramref name="timeProvider"/> が <see langword="null"/> の場合。</exception>
    public ReminderService(INotificationSender sender, TimeProvider timeProvider)
    {
        this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <summary>
    /// 業務時間内だけ通知を送信します。
    /// </summary>
    /// <param name="userId">ユーザー ID。</param>
    /// <param name="message">通知メッセージ。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>送信した場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentException"><paramref name="userId"/> または <paramref name="message"/> が空白の場合。</exception>
    public async Task<bool> SendDuringBusinessHoursAsync(
        string userId,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        int hour = timeProvider.GetUtcNow().Hour;

        if (hour is < 9 or >= 18)
        {
            return false;
        }

        await sender.SendAsync(userId, message, cancellationToken);
        return true;
    }
}

/// <summary>
/// fake として使う通知 sender です。
/// </summary>
public sealed class FakeNotificationSender : INotificationSender
{
    private readonly List<(string UserId, string Message)> sentMessages = [];

    /// <summary>
    /// 送信済みメッセージを取得します。
    /// </summary>
    /// <value>送信先ユーザー ID とメッセージ。</value>
    public IReadOnlyList<(string UserId, string Message)> SentMessages => sentMessages;

    /// <inheritdoc />
    public Task SendAsync(string userId, string message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        sentMessages.Add((userId, message));
        return Task.CompletedTask;
    }
}

/// <summary>
/// テストで固定時刻を返す TimeProvider です。
/// </summary>
public sealed class FixedTimeProvider : TimeProvider
{
    private DateTimeOffset utcNow;

    /// <summary>
    /// <see cref="FixedTimeProvider"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="utcNow">固定する日時。UTC 以外の offset は UTC に正規化します。</param>
    public FixedTimeProvider(DateTimeOffset utcNow)
    {
        this.utcNow = utcNow.ToUniversalTime();
    }

    /// <summary>
    /// 固定時刻を変更します。
    /// </summary>
    /// <param name="value">新しい日時。UTC 以外の offset は UTC に正規化します。</param>
    public void SetUtcNow(DateTimeOffset value)
    {
        utcNow = value.ToUniversalTime();
    }

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow()
    {
        return utcNow;
    }
}

/// <summary>
/// テスト補助サンプルです。
/// </summary>
public static class TestingSamples
{
    /// <summary>
    /// AAA 形式のテストメモを作成します。
    /// </summary>
    /// <param name="arrange">前提条件。</param>
    /// <param name="act">実行する操作。</param>
    /// <param name="assert">確認する期待結果。</param>
    /// <returns>AAA 形式のテストメモ。</returns>
    /// <exception cref="ArgumentException">いずれかの引数が空白の場合。</exception>
    public static TestCaseNotes CreateArrangeActAssertNotes(string arrange, string act, string assert)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(arrange);
        ArgumentException.ThrowIfNullOrWhiteSpace(act);
        ArgumentException.ThrowIfNullOrWhiteSpace(assert);

        return new TestCaseNotes(arrange, act, assert);
    }

    /// <summary>
    /// 操作がキャンセルに反応するかを確認しやすい形で実行します。
    /// </summary>
    /// <param name="operation">キャンセル済み token を受け取る操作。</param>
    /// <returns>キャンセル例外が発生した場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="operation"/> が <see langword="null"/> の場合。</exception>
    public static async Task<bool> ThrowsOperationCanceledAsync(Func<CancellationToken, Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        try
        {
            await operation(cancellationTokenSource.Token);
            return false;
        }
        catch (OperationCanceledException)
        {
            return true;
        }
    }
}
