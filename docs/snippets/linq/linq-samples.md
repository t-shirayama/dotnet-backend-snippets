# LINQ 操作

## 目的

LINQ を使ったフィルタ、射影、グルーピング、並び替え、ページング、重複排除、左外部結合、フラット化の基本パターンをまとめます。LINQ はコレクションを SQL のような感覚で扱える C# の機能です。

## 実装

`src/DotnetBackendSnippets/Linq/LinqSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Linq/LinqSamplesTests.cs`

## 使い方

- `GetExpensiveOrderCategories` は条件に合う注文を抽出し、カテゴリ名だけを重複なしで返します。
- `SumAmountByCategory` はカテゴリごとに注文金額を合計します。
- `TopOrders` は金額が大きい順に上位 N 件を返します。
- `Page` はページ番号と件数から一部だけを取り出します。
- `DistinctByKey` は指定したキーが同じ要素のうち、最初の要素だけを残します。
- `LeftJoinCustomerOrders` は注文がない顧客も残して集計します。
- `Flatten` は入れ子のコレクションを1つのコレクションにします。

## メモ

- LINQ は読みやすい反面、遅延実行や複数回列挙に注意が必要です。このスニペットでは戻り値を `ToList` などで確定しています。
- データベース向け LINQ とメモリ上の LINQ では使えるメソッドや性能特性が違う場合があります。
