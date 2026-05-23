# BackgroundService 向け worker ループ

## 目的

`HostedService` / `BackgroundService` に渡しやすい worker ループを、DI とテストの両方で扱いやすい形にします。

## 実装

`src/DotnetBackendSnippets.AspNetCore/BackgroundServices/BackgroundServiceSamples.cs`

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

## メモ

- 例外が発生した場合、`ContinueOnError = true` ならログを出して次のループに進みます。
- `OperationCanceledException` は停止要求として扱い、正常にループを抜けます。
- 実処理は `IBackgroundWorker` に寄せると、`BackgroundService` のライフサイクルと業務ロジックを分けてテストできます。
