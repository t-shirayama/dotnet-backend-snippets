# Options パターンの登録と検証

## 目的

`IOptions<T>` で設定を型として扱い、起動時や利用時に設定漏れを検出するためのスニペットです。`IOptionsMonitor<T>` を使うと、現在の設定値を DI 経由で読み取れます。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Options/OptionsSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Options/OptionsSamplesTests.cs`

## 使い方

`services.AddNotificationOptions(configuration)` を呼び出すと、`Notifications` セクションを `NotificationOptions` に bind します。

`SenderEmail` は必須、`RetryCount` は `0` から `5` までとして検証します。利用側は `IOptions<NotificationOptions>` または `NotificationOptionsReader` 経由で現在値を取得します。

## メモ

- `IOptions<T>` は設定値を型安全に読む基本形です。
- `IOptionsMonitor<T>` は現在値を読むための仕組みで、設定の再読み込みに対応する場面で使いやすいです。
- `ValidateOnStart()` を付けると、ホスト起動時に検証する設計へ寄せられます。
