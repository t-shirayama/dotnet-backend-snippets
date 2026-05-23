# Validation の基本

## 目的

入力値を検証し、正常系と異常系の両方をテストしやすい形で結果を返すためのスニペットです。

## 実装

`src/DotnetBackendSnippets.Core/Validation/ValidationSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Validation/ValidationSamplesTests.cs`

## 使い方

`ValidationSamples.ValidateUserRegistration(input)` にユーザー登録入力を渡すと、`IsValid` とエラーメッセージ一覧を持つ `ValidationResult` を返します。

## メモ

- 例外ではなく結果オブジェクトで返すと、画面や API レスポンスにエラーを変換しやすくなります。
- 複数のエラーをまとめて返すと、利用者が一度に修正しやすくなります。

## 実務逆引き

- DTO の必須項目を検証したい → `ValidateUserRegistration`
- 複数の検証エラーをまとめて返したい → `ValidationResult`
- validation error を API 応答に変換したい → [ASP.NET Core API の小さな実務ヘルパー](../api/api-samples.md)
- FluentValidation を DI と組み合わせたい → 追加候補
