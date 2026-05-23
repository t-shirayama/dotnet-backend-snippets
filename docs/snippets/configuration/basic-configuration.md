# Configuration の必須値読み取り

## 目的

アプリケーション設定から必須値を読み取り、設定漏れや不正な値を早めに検出するためのスニペットです。`ConfigurationBuilder` で作った設定にも、本番の設定にも同じ考え方を使えます。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Configuration/ConfigurationSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Configuration/ConfigurationSamplesTests.cs`

## 使い方

`ConfigurationSamples.ReadAppSettings(configuration)` を呼び出すと、`App:ServiceName` と `App:RetryCount` を読み取って `AppSettings` を返します。必須値がない場合や `RetryCount` が数値でない場合は例外を投げます。

## メモ

- 設定値は文字列として読まれるため、数値などは明示的に検証します。
- 起動時に設定を検証すると、実行中の予期しない失敗を減らせます。

## 実務逆引き

- 設定値を必須として読みたい → `GetRequiredValue`
- 設定セクションを型に変換したい → `ReadAppSettings`
- 数値設定を不正値として検出したい → `ReadAppSettings`
- 環境変数で設定を上書きしたい → `ConfigurationBuilder` の provider 順を調整する
- user secrets をローカル開発で使いたい → 追加候補
