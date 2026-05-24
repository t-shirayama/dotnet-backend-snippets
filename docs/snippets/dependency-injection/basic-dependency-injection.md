# Dependency Injection のサービス登録

## 目的

`ServiceCollection` にサービスを登録し、依存関係を解決する基本パターンを確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/DependencyInjection/DependencyInjectionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/DependencyInjection/DependencyInjectionSamplesTests.cs`

## 使い方

`services.AddGreetingService("Hi")` を呼び出すと、`IGreetingService` が DI コンテナに登録されます。`BuildServiceProvider()` 後に `GetRequiredService<IGreetingService>()` で取得できます。

`AddLifetimeExampleServices` は singleton / scoped / transient の lifetime 差を確認するための登録例です。`AddReportFormatters` は keyed service で `plain` と `markdown` の実装を切り替える例です。`ReplaceGreetingService` はテスト用実装への差し替えを示します。

## メモ

- サンプルではシンプルな singleton 登録を使っています。
- 実務では状態を持つサービスや外部リソースを扱う場合、lifetime の選択に注意してください。
- scoped service は request や処理単位の状態、transient service は軽量で状態を共有しない処理、singleton service は不変または thread-safe な共有処理に寄せます。
- keyed service は複数実装の切り替えに便利ですが、key 文字列の管理が散らばらないように注意します。

## 実務逆引き

- DI にサービスを登録したい → `AddGreetingService`
- singleton / scoped / transient を使い分けたい → `AddLifetimeExampleServices`
- keyed service を登録したい → `AddReportFormatters`
- テスト用に DI 登録を差し替えたい → `ReplaceGreetingService`
- HostedService を DI で動かしたい → [BackgroundService 向け worker ループ](../background-services/background-worker-loop.md)
