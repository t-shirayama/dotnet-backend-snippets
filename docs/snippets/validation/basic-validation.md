# Validation の基本

## 目的

入力値を検証し、正常系と異常系の両方をテストしやすい形で結果を返すためのスニペットです。

## 実装

`src/DotnetBackendSnippets/Validation/ValidationSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Validation/ValidationSamplesTests.cs`

## 使い方

`ValidationSamples.ValidateUserRegistration(input)` にユーザー登録入力を渡すと、`IsValid` とエラーメッセージ一覧を持つ `ValidationResult` を返します。

## メモ

- 例外ではなく結果オブジェクトで返すと、画面や API レスポンスにエラーを変換しやすくなります。
- 複数のエラーをまとめて返すと、利用者が一度に修正しやすくなります。
