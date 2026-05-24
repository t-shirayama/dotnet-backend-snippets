# Deployment / Docker の基本

## 目的

Dockerfile、docker compose、non-root 実行、environment variables、secret 注入、graceful shutdown を小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Deployment/DeploymentSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Deployment/DeploymentSamplesTests.cs`

## 使い方

- `CreateAspNetCoreDockerfile` は ASP.NET Core runtime image、non-root user、`ASPNETCORE_URLS`、`ENTRYPOINT` を含む Dockerfile を生成します。
- `CreateComposeService` は image と environment variables を持つ docker compose service 断片を生成します。
- `CreateGracefulShutdownOptions` は `HostOptions.ShutdownTimeout` を設定します。

## メモ

- Docker image では root 実行を避け、アプリ専用 user を使います。
- secret は image に焼き込まず、environment variables、secret store、orchestrator の secret 機構から注入します。
- graceful shutdown timeout は Kubernetes の termination grace period やロードバランサーの drain 設計と合わせます。
- structured logging の出力先はコンテナでは標準出力を基本にし、収集基盤側で集約します。

## 実務逆引き

- non-root Dockerfile を作りたい → `CreateAspNetCoreDockerfile`
- docker compose に環境変数を渡したい → `CreateComposeService`
- secret を環境変数から注入したい → `CreateComposeService`
- graceful shutdown を設定したい → `CreateGracefulShutdownOptions`
