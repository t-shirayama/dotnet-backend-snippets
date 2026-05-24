# BackgroundService 向け worker ループ

## 目的

`HostedService` / `BackgroundService` に渡しやすい worker ループと、channel-based queue を DI とテストの両方で扱いやすい形にします。

## 実装

`src/DotnetBackendSnippets.AspNetCore/BackgroundServices/BackgroundServiceSamples.cs`

`src/DotnetBackendSnippets.AspNetCore/BackgroundServices/BackgroundQueueSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/BackgroundServices/BackgroundServiceSamplesTests.cs`

## 使い方

定期実行したい処理は `IBackgroundWorker` に実装します。

```csharp
public sealed class CleanupWorker : IBackgroundWorker
{
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

DI には `AddBackgroundWorker<TWorker>` で登録します。

```csharp
services.AddBackgroundWorker<CleanupWorker>(options =>
{
    options.Interval = TimeSpan.FromMinutes(5);
    options.ContinueOnError = true;
});
```

テストでは `BackgroundWorkerLoop.RunOnceAsync` を使うと、ループ全体を起動せずに1回分の処理だけ検証できます。

一時的な非同期処理をキューに積む場合は `ChannelBackgroundJobQueue.EnqueueAsync` を使います。`QueuedBackgroundJobProcessor` は scoped service provider を作ってジョブを実行し、失敗時は `MaxAttempts` まで retry します。最大試行回数を超えたジョブは `IBackgroundJobPoisonHandler` に渡します。

## メモ

- 例外が発生した場合、`ContinueOnError = true` ならログを出して次のループに進みます。
- `OperationCanceledException` は停止要求として扱い、正常にループを抜けます。
- 実処理は `IBackgroundWorker` に寄せると、`BackgroundService` のライフサイクルと業務ロジックを分けてテストできます。
- channel-based queue はプロセス内キューです。永続化や複数台処理が必要な場合は Azure Queue、SQS、RabbitMQ などの外部 queue を検討します。
- ジョブは idempotent にして、retry されても二重更新にならないようにします。
- scoped service を使う処理は processor 側で scope を作ると、DbContext などの lifetime を守れます。

## 実務逆引き

- HostedService を DI で動かしたい → `AddBackgroundWorker<TWorker>`
- worker ループを1回だけテストしたい → `RunOnceAsync`
- 例外後も次のループへ進めたい → `ContinueOnError`
- 停止要求をキャンセルとして扱いたい → `OperationCanceledException` の分岐
- channel-based queue を使いたい → `ChannelBackgroundJobQueue`
- scoped service を background job で使いたい → `QueuedBackgroundJobProcessor`
- retry と poison message を扱いたい → `BackgroundJob.MaxAttempts` / `IBackgroundJobPoisonHandler`
