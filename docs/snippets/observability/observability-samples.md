# Observability の基本

## 目的

ログ、correlation id、health check、処理時間計測を、小さくテストできる形で扱うためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Observability/ObservabilitySamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Observability/ObservabilitySamplesTests.cs`

## 使い方

例外付きログは `LogOperationError` に `ILogger`、操作名、例外、任意の correlation id を渡します。

```csharp
try
{
    await handler.HandleAsync(cancellationToken);
}
catch (Exception exception)
{
    ObservabilitySamples.LogOperationError(logger, "HandleMessage", exception, correlationId);
    throw;
}
```

`BeginScope` に渡す状態は `CreateScopeState` または `BeginOperationScope` で作ります。

```csharp
using var scope = ObservabilitySamples.BeginOperationScope(
    logger,
    correlationId,
    "ImportOrders",
    userId);
```

HTTP ヘッダーから correlation id を取得する場合は `GetOrCreateCorrelationId` を使います。

```csharp
string correlationId = ObservabilitySamples.GetOrCreateCorrelationId(httpContext.Request.Headers);
```

health check の結果は `CreateHealthCheckResult` で組み立てます。

```csharp
HealthCheckResult result = ObservabilitySamples.CreateHealthCheckResult(
    "database",
    isHealthy: true,
    elapsed: TimeSpan.FromMilliseconds(12));
```

処理時間を計測してログに出す場合は `MeasureAndLogAsync` を使います。

```csharp
TimedOperationResult<string> result = await ObservabilitySamples.MeasureAndLogAsync(
    logger,
    "RefreshCache",
    cancellationToken => cache.RefreshAsync(cancellationToken),
    slowThreshold: TimeSpan.FromSeconds(1),
    cancellationToken);
```

## メモ

- correlation id はリクエストやジョブをまたいでログを追うための識別子です。
- `BeginScope` の状態は構造化ログのプロパティとして扱えるよう、辞書で渡しています。
- `MeasureAndLogAsync` はしきい値以上なら warning、それ未満なら information としてログを出します。
- OpenTelemetry やメトリクス基盤を追加する場合も、まず操作名、correlation id、処理時間をそろえておくと移行しやすくなります。
