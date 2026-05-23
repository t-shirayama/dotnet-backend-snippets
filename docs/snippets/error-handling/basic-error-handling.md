# Error Handling の基本

## 目的

例外を投げるべき場面と、失敗結果として返す場面を分けて扱うためのスニペットです。

## 実装

`src/DotnetBackendSnippets.Core/ErrorHandling/ErrorHandlingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/ErrorHandling/ErrorHandlingSamplesTests.cs`

## 使い方

`TryParsePositiveInt(value)` は、正の整数に変換できた場合は成功結果を返し、失敗した場合はエラーメッセージを持つ失敗結果を返します。`ThrowIfInvalidState(false)` は不正な状態を例外で知らせます。

## メモ

- 入力エラーなど想定できる失敗は結果オブジェクトで返すと扱いやすくなります。
- 不正な状態やプログラム上の前提違反は例外で早めに検出します。
