# 型システム

## 目的

record、nullable、enum、pattern matching、generic な小さな結果型など、C# の型システムを使って安全に表現する基本パターンをまとめます。

## 実装

`src/DotnetBackendSnippets.Core/TypeSystem/TypeSystemSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/TypeSystem/TypeSystemSamplesTests.cs`

## 使い方

- `Money` は record です。record は値が同じなら等しいと判定しやすい型です。
- `RequireNonNull` は nullable な値を非 null として扱う前に検証します。
- `TryParseOrderStatus` は文字列から enum を安全に変換します。enum は名前付き定数の集合です。
- `DescribeStatus` は pattern matching の `switch` で状態ごとの説明を返します。
- `Result<T>`、`Success<T>`、`Failure<T>` は成功と失敗を型で表現する例です。generic は型を引数のように扱う仕組みです。
- `Maybe<T>` は値があるかないかを明示する小さな型です。

## メモ

- nullable reference types を有効にすると、null の可能性をコンパイル時に見つけやすくなります。
- 型で状態を表すと、文字列や bool だけで扱うよりも間違いを減らせます。
- `Maybe<T>` は参照型だけでなく、`int` や `DateOnly` などの値型にも使えます。
