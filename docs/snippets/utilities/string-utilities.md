# 文字列ユーティリティ

## 目的

よく使う文字列処理を小さなユーティリティとして切り出し、テストで挙動を固定するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.Core/Utilities/StringUtilities.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Utilities/StringUtilitiesTests.cs`

## 使い方

`StringUtilities.NormalizeWhitespace(value)` は前後の空白を取り除き、連続する空白や改行を1つの半角スペースにまとめます。`RequireNonEmpty(value, parameterName)` は空文字や空白だけの値を拒否します。

## メモ

- 小さなユーティリティでもテストを用意すると、後から安心して再利用できます。
- 引数チェックでは `ArgumentException` や `ArgumentNullException` など標準例外を使うと呼び出し側が扱いやすくなります。
