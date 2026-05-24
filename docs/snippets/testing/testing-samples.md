# Testing の基本

## 目的

Arrange / Act / Assert、fake、time provider、cancellation token、例外系テストの考え方を小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.Core/Testing/TestingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Testing/TestingSamplesTests.cs`

## 使い方

- `CreateArrangeActAssertNotes` はテストで何を準備し、何を実行し、何を確認するかを分けて記録します。
- `ReminderService` は外部送信を `INotificationSender` に分け、時刻を `TimeProvider` から受け取るテスト対象です。
- `FakeNotificationSender` は送信結果をメモリに残す fake です。
- `FixedTimeProvider` は時刻依存ロジックを固定時刻でテストするための provider です。UTC 以外の offset は UTC に正規化します。
- `ThrowsOperationCanceledAsync` は cancellation token に反応する処理かどうかを確認しやすくします。

## メモ

- fake は動く軽量実装、stub は決まった値を返す差し替え、mock は呼び出し検証を主目的にするものとして使い分けます。
- 時刻や乱数、外部通信は直接呼ばず、差し替え可能にするとテストが安定します。
- snapshot test は大きな出力の差分確認には便利ですが、意図しない変更を見逃さないレビュー運用が必要です。
- integration test は最小構成から始め、DB や外部依存を増やすほど実行時間と保守コストを意識します。

## 実務逆引き

- AAA の形を揃えたい → `TestCaseNotes`
- fake を使いたい → `FakeNotificationSender`
- time provider をテストしたい → `FixedTimeProvider`
- cancellation token をテストしたい → `ThrowsOperationCanceledAsync`
- 外部送信を差し替えたい → `INotificationSender`
