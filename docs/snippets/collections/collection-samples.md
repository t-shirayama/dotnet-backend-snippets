# コレクション操作

## 目的

辞書やリストを扱うときによく使う、カウント更新、重複検出、辞書マージ、必須キー取得、順序を保つ追加、固定サイズ分割をまとめます。

## 実装

`src/DotnetBackendSnippets.Core/Collections/CollectionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Collections/CollectionSamplesTests.cs`

## 使い方

- `IncrementCount` は辞書内のカウントを追加または加算します。
- `FindDuplicates` は `GroupBy` を使って2回以上出現した値を返します。
- `FindDuplicatesOnePass` は `HashSet<T>` を使い、一回走査で重複を検出します。
- `MergeDictionaries` は2つの辞書を結合し、同じキーは後の辞書を優先します。
- `GetRequired` は必須キーがない場合に例外を投げます。
- `AddIfMissing` は既に存在する値を追加せず、順序を保ちます。
- `ISet<T>` 向けの `AddIfMissing` は、セットの検索性能と比較方法をそのまま利用します。
- `ChunkBySize` はコレクションを指定件数ごとに分割します。

## メモ

- `Dictionary` のキーには `notnull` 制約を付けています。null キーを避けるためです。
- 順序が重要な場合は `List`、キー検索が重要な場合は `Dictionary` など、用途に合う型を選びます。
- 重複排除を主目的にする場合は、`List<T>` より `HashSet<T>` や `ISet<T>` が向いています。

## 実務逆引き

- 重複値を検出したい → `FindDuplicates` / `FindDuplicatesOnePass`
- 辞書のカウントを増やしたい → `IncrementCount`
- 辞書を後勝ちでマージしたい → `MergeDictionaries`
- 必須キーがない場合に早めに失敗させたい → `GetRequired`
- 重複を避けて追加したい → `AddIfMissing`
- 一定件数ごとに分割したい → `ChunkBySize`
