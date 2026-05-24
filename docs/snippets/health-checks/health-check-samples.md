# Health Check / Readiness の基本

## 目的

liveness / readiness、DB や外部 API の依存チェック、degraded 状態、Kubernetes 向け endpoint path を小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/HealthChecks/HealthCheckSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/HealthChecks/HealthCheckSamplesTests.cs`

## 使い方

- `CreateKubernetesHealthEndpointPaths` は `/health/live` と `/health/ready` の path を作ります。
- `CreateLivenessResult` はプロセスが起動していることだけを見る healthy 結果を作ります。
- `CheckDependencyAsync` は DB や外部 API の probe を `HealthCheckResult` に変換します。
- `CreateReadinessResult` は複数依存先の状態から ready / degraded / unhealthy を決めます。

## メモ

- liveness は「プロセスを再起動すべきか」、readiness は「トラフィックを流してよいか」を分けて考えます。
- DB など必須依存先は unhealthy、任意の外部 API は degraded として扱うなど、依存先ごとに重みを決めます。
- readiness に重いチェックを入れすぎると監視自体が負荷になるため、timeout と頻度を設計します。

## 実務逆引き

- Kubernetes 向け endpoint を分けたい → `CreateKubernetesHealthEndpointPaths`
- liveness を返したい → `CreateLivenessResult`
- readiness を集約したい → `CreateReadinessResult`
- DB 接続チェックを差し込みたい → `DelegateDependencyProbe`
- 外部 API 依存を degraded にしたい → `CheckDependencyAsync`
