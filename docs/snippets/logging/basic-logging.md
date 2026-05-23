# Logging の基本

## 目的

`ILogger` を受け取る処理でログを出しつつ、戻り値もテストできる形にするためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Logging/LoggingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Logging/LoggingSamplesTests.cs`

## 使い方

`LoggingSamples.CountProcessedItems(items, logger)` に処理対象と `ILogger` を渡すと、件数をログに出して件数を返します。

## メモ

- テストでは `ListLogger` を使い、実際のログ出力先に依存しないようにしています。
- ログは戻り値の代わりではないため、重要な結果は戻り値や例外で検証できるようにします。
