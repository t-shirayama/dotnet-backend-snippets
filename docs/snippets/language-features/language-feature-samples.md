# C# 言語機能サンプル

## 目的

C# の基本文法や言語機能を、バックエンド実装で再利用しやすい小さな例としてまとめます。型推論、nullable 値型、タプル、継承、演算子オーバーロード、コレクション、LINQ、パターンマッチング、デリゲート、イベント、属性、リソース管理、by-ref、unsafe の隔離を扱います。

## 実装

- `src/DotnetBackendSnippets.Core/LanguageFeatures/TypeInferenceSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/NullableValueSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/TupleSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/DeconstructSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/ObjectOrientedSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/AdvancedCollectionSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/LinqAdvancedSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/PatternMatchingSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/DelegateEventAttributeSamples.cs`
- `src/DotnetBackendSnippets.Core/LanguageFeatures/ResourceManagementSamples.cs`
- `src/DotnetBackendSnippets.UnsafeSamples/LanguageFeatures/ByRefAndUnsafeSamples.cs`

## テスト

- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/TypeInferenceSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/NullableValueSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/TupleSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/DeconstructSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/ObjectOrientedSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/AdvancedCollectionSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/LinqAdvancedSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/PatternMatchingSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/DelegateEventAttributeSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/ResourceManagementSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/ByRefAndUnsafeSamplesTests.cs`

## 使い方

- `TypeInferenceSamples` は `var` と明示型の使い分けを、LINQ projection と辞書の境界で示します。
- `NullableValueSamples` は `int?` の `GetValueOrDefault`、`Value`、`HasValue`、`is null` の使い分けを示します。
- `TupleSamples` は名前付き tuple、複数戻り値、tuple から record への変換を扱います。
- `DeconstructSamples` は record と独自型の `Deconstruct` を使った分解を扱います。
- `ObjectOrientedSamples` は `abstract` / `sealed`、委譲、演算子オーバーロード、インデクサをまとめます。
- `AdvancedCollectionSamples` は配列、`Queue<T>`、`Stack<T>` の基本操作を扱います。
- `LinqAdvancedSamples` は `Any`、`FirstOrDefault`、内部結合、`Aggregate` を扱います。
- `PatternMatchingSamples` は型パターン、プロパティパターン、リストパターン、デコンストラクトパターンを扱います。
- `DelegateEventAttributeSamples` は `delegate`、`Func<T>`、`Action<T>`、`event`、独自属性の読み取りを扱います。
- `ResourceManagementSamples` は `using var` と `yield return` の基本を扱います。
- `ByRefAndUnsafeSamples` は `ref`、`in`、`out` と、unsafe ブロックを安全な公開メソッドの内側へ閉じ込める例を扱います。

## メモ

- tuple は小さな内部処理では便利ですが、API 境界や意味を持つ値には record を使うと読みやすくなります。
- 継承は差し替え可能な振る舞いを表したい場合に絞り、単純な計算の差し替えは委譲を優先します。
- unsafe はプロジェクトで許可が必要です。このリポジトリでは `DotnetBackendSnippets.UnsafeSamples` だけで unsafe を許可し、通常の `DotnetBackendSnippets.Core` には unsafe 許可を付けません。
