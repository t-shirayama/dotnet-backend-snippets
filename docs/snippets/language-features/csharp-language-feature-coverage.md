# C# 言語機能の実装状況

## 目的

C# の基本文法・言語機能について、スニペットとして整理した実装状況をまとめます。

実装・テスト・使い方は [C# 言語機能サンプル](language-feature-samples.md) を参照してください。

## 型システム

| 項目 | 状況 | 実装 |
|---|---|---|
| `var` / 型推論 | 実装済み | `TypeInferenceSamples` |
| Nullable 値型 `int?` | 実装済み | `NullableValueSamples` |
| タプル | 実装済み | `TupleSamples` |
| デコンストラクト | 実装済み | `DeconstructSamples` |

## OOP

| 項目 | 状況 | 実装 |
|---|---|---|
| クラスと継承 | 実装済み | `ObjectOrientedSamples` |
| `abstract` / `sealed` | 実装済み | `DiscountPolicy`, `PercentageDiscountPolicy` |
| 演算子オーバーロード | 実装済み | `Money` |
| インデクサ | 実装済み | `HeaderBag` |

## コレクション

| 項目 | 状況 | 実装 |
|---|---|---|
| 配列 `int[]` | 実装済み | `AdvancedCollectionSamples.CreateFixedLengthBuffer` |
| `Queue<T>` | 実装済み | `AdvancedCollectionSamples.ProcessQueue` |
| `Stack<T>` | 実装済み | `AdvancedCollectionSamples.PopUndoStack` |

## LINQ

| 項目 | 状況 | 実装 |
|---|---|---|
| `Any` | 実装済み | `LinqAdvancedSamples.HasAnyHighValueOrder` |
| `First` / `FirstOrDefault` | 実装済み | `FindOptionalOrder`, `FindRequiredOrder` |
| 通常の `Join` | 実装済み | `JoinCustomerOrders` |
| `Aggregate` | 実装済み | `AggregateOrders` |

## パターンマッチング

| 項目 | 状況 | 実装 |
|---|---|---|
| `is` パターン | 実装済み | `PatternMatchingSamples.DescribeValue` |
| リストパターン | 実装済み | `PatternMatchingSamples.DescribeNumbers` |
| デコンストラクトパターン | 実装済み | `PatternMatchingSamples.ClassifyPoint` |

## デリゲート・イベント・属性

| 項目 | 状況 | 実装 |
|---|---|---|
| ラムダ式 / デリゲート | 実装済み | `DelegateEventAttributeSamples` |
| イベント | 実装済み | `ProgressReporter` |
| 属性 `Attribute` | 実装済み | `SnippetTagAttribute` |

## リソース管理・イテレータ

| 項目 | 状況 | 実装 |
|---|---|---|
| `using` / `Dispose` | 実装済み | `ResourceManagementSamples.UseDisposableResource` |
| `yield return` | 実装済み | `ResourceManagementSamples.ReadUntilBlankLine` |

## by-ref・unsafe

`ByRefAndUnsafeSamples` は `DotnetBackendSnippets.UnsafeSamples` に分離し、unsafe 許可を通常の Core サンプルへ広げないようにしています。

| 項目 | 状況 | 実装 |
|---|---|---|
| `ref` パラメータ | 実装済み | `ByRefAndUnsafeSamples.Increment` |
| `in` パラメータ | 実装済み | `CalculateTotal` |
| `out` パラメータ | 実装済み | `TryParsePositiveInt` |
| `unsafe` / ポインタ | 実装済み | `ReadFirstWithPinnedPointer` |
