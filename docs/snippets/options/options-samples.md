# Options パターンの登録と検証

## 目的

`IOptions<T>` で設定を型として扱い、起動時や利用時に設定漏れを検出するためのスニペットです。`IOptionsMonitor<T>` を使うと、現在の設定値を DI 経由で読み取れます。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Options/OptionsSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Options/OptionsSamplesTests.cs`

## 使い方

`services.AddNotificationOptions(configuration)` を呼び出すと、`Notifications` セクションを `NotificationOptions` に bind します。

`SenderEmail` は必須、`RetryCount` は `0` から `5` までとして検証します。利用側は `IOptions<NotificationOptions>`、`NotificationOptionsReader`、または scoped な `NotificationOptionsSnapshotReader` 経由で現在値を取得します。

`AddNamedNotificationOptions` は、同じ `NotificationOptions` 型を `email`、`sms` のような名前付き options として複数登録する例です。

## メモ

- `IOptions<T>` は設定値を型安全に読む基本形です。
- `IOptionsMonitor<T>` は現在値を読むための仕組みで、設定の再読み込みに対応する場面で使いやすいです。
- `IOptionsSnapshot<T>` は scoped service で request 単位の設定 snapshot を読みたい場合に使います。
- named options は同じ型の設定を用途別に複数持ちたい場合に使います。
- `ValidateOnStart()` を付けると、ホスト起動時に検証する設計へ寄せられます。

## 実務逆引き

- `IOptions<T>` を登録したい → `AddNotificationOptions`
- `IOptionsMonitor<T>` の現在値を読みたい → `NotificationOptionsReader`
- `IOptionsSnapshot<T>` を使い分けたい → `NotificationOptionsSnapshotReader`
- named options を登録したい → `AddNamedNotificationOptions`
- 設定を bind して検証したい → `AddNotificationOptions`
- 起動時に設定漏れを検出したい → `ValidateOnStart`
