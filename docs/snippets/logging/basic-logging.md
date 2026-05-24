# Logging の基本

## 目的

`ILogger` を受け取る処理でログを出しつつ、戻り値もテストできる形にするためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Logging/LoggingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Logging/LoggingSamplesTests.cs`

## 使い方

`LoggingSamples.CountProcessedItems(items, logger)` に処理対象と `ILogger` を渡すと、件数をログに出して件数を返します。

`LogHandledException` は処理済みの例外を event id 付きで warning log に残します。`LogOperationDuration` は閾値以上の遅い処理を warning、閾値未満を debug で出し分けます。

## メモ

- テストでは `ListLogger` を使い、実際のログ出力先に依存しないようにしています。
- ログは戻り値の代わりではないため、重要な結果は戻り値や例外で検証できるようにします。
- 例外ログでは exception オブジェクトを `ILogger` に渡し、message 文字列だけに潰さないようにします。
- 処理時間ログは閾値を引数や設定値にして、テストで遅い・速いの両方を確認できる形にします。

## 実務逆引き

- 構造化ログを出したい → `CountProcessedItems`
- ログ出力をテストしたい → `ListLogger`
- 例外付きログを出したい → `LogHandledException`
- 遅い処理だけ warning にしたい → `LogOperationDuration`
- correlation id をログに入れたい → [Observability の基本](../observability/observability-samples.md)
- 本番ではログレベルを設定で変えたい → 追加候補
