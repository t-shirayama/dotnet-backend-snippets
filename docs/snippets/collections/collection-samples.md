# コレクション操作

## 目的

辞書やリストを扱うときによく使う、カウント更新、重複検出、辞書マージ、必須キー取得、順序を保つ追加、固定サイズ分割をまとめます。

## 実装

`src/DotnetBackendSnippets/Collections/CollectionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Collections/CollectionSamplesTests.cs`

## 使い方

- `IncrementCount` は辞書内のカウントを追加または加算します。
- `FindDuplicates` は2回以上出現した値を返します。
- `MergeDictionaries` は2つの辞書を結合し、同じキーは後の辞書を優先します。
- `GetRequired` は必須キーがない場合に例外を投げます。
- `AddIfMissing` は既に存在する値を追加せず、順序を保ちます。
- `ChunkBySize` はコレクションを指定件数ごとに分割します。

## メモ

- `Dictionary` のキーには `notnull` 制約を付けています。null キーを避けるためです。
- 順序が重要な場合は `List`、キー検索が重要な場合は `Dictionary` など、用途に合う型を選びます。
