# Dependency Injection のサービス登録

## 目的

`ServiceCollection` にサービスを登録し、依存関係を解決する基本パターンを確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/DependencyInjection/DependencyInjectionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/DependencyInjection/DependencyInjectionSamplesTests.cs`

## 使い方

`services.AddGreetingService("Hi")` を呼び出すと、`IGreetingService` が DI コンテナに登録されます。`BuildServiceProvider()` 後に `GetRequiredService<IGreetingService>()` で取得できます。

## メモ

- サンプルではシンプルな singleton 登録を使っています。
- 実務では状態を持つサービスや外部リソースを扱う場合、lifetime の選択に注意してください。

## 実務逆引き

- DI にサービスを登録したい → `AddGreetingService`
- singleton / scoped / transient を使い分けたい → 追加候補
- keyed service を登録したい → 追加候補
- テスト用に DI 登録を差し替えたい → 追加候補
- HostedService を DI で動かしたい → [BackgroundService 向け worker ループ](../background-services/background-worker-loop.md)
