# Dependency Injection のサービス登録

## 目的

`ServiceCollection` にサービスを登録し、依存関係を解決する基本パターンを確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets/DependencyInjection/DependencyInjectionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/DependencyInjection/DependencyInjectionSamplesTests.cs`

## 使い方

`services.AddGreetingService("Hi")` を呼び出すと、`IGreetingService` が DI コンテナに登録されます。`BuildServiceProvider()` 後に `GetRequiredService<IGreetingService>()` で取得できます。

## メモ

- サンプルではシンプルな singleton 登録を使っています。
- 実務では状態を持つサービスや外部リソースを扱う場合、lifetime の選択に注意してください。
