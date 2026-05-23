# C# 言語機能の追加候補

## 目的

C# の基本文法・言語機能について、現在のスニペット集で薄い項目や未カバーの項目を洗い出します。

このページは実装済みスニペットではなく、今後 `src`、`tests`、`docs` の3点セットで追加する候補リストです。既存コード内に自然に登場しているだけの項目も、学習用スニペットとして整理されていない場合は「薄い」とします。

## 型システム

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| `var` / 型推論 | 薄い | 明示型と `var` の使い分け、型が明らかな場面、匿名型で必要になる場面 |
| Nullable 値型 `int?` | 薄い | `HasValue`、`Value`、`GetValueOrDefault`、`??`、`is null` の使い分け |
| タプル | 薄い | 名前付き tuple、複数戻り値、tuple deconstruction、戻り値を record にすべき場面 |
| デコンストラクト | 薄い | `Deconstruct` メソッド、record / tuple / 独自型の分解 |

既存で比較的カバーできているもの:

- ジェネリクス `T` → [型システム](../type-system/type-system-samples.md)
- `record` 型 → [型システム](../type-system/type-system-samples.md)
- nullable reference types → [型システム](../type-system/type-system-samples.md)

## OOP

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| クラスと継承 | 薄い | base class、virtual / override、継承より委譲を選ぶ例 |
| `abstract` / `sealed` | 薄い | 抽象基底クラス、`sealed` で継承を閉じる理由、record/class での使い分け |
| 演算子オーバーロード | 未カバー | `Money` や `Quantity` の `+`、`==`、比較演算子の例 |
| インデクサ | 未カバー | `this[int index]`、`this[string key]`、読み取り専用インデクサ |

既存で比較的カバーできているもの:

- インターフェース → `IGreetingService`、`IBackgroundWorker`、`IAccessTokenProvider`
- プロパティ `get` / `set` → EF Core entity、Options
- 拡張メソッド → DI 登録系の `Add...` メソッド

## コレクション

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| 配列 `int[]` | 薄い | 配列初期化、固定長、`Array.Empty<T>()`、`Span<T>` へ渡す前提 |
| `Queue<T>` | 未カバー | FIFO のジョブ処理、`Enqueue` / `Dequeue` / `TryDequeue` |
| `Stack<T>` | 未カバー | LIFO の戻る操作、`Push` / `Pop` / `TryPop` |

既存で比較的カバーできているもの:

- `List<T>`、`Dictionary<TKey,TValue>`、`HashSet<T>` → [コレクション操作](../collections/collection-samples.md)

## LINQ

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| `Any` | 薄い | 存在確認、`Count() > 0` との違い、早期終了 |
| `First` / `FirstOrDefault` | 薄い | 見つからない場合の扱い、`Single` との使い分け |
| 通常の `Join` | 薄い | 内部結合、複合キー join、join 後の DTO 変換 |
| `Aggregate` | 未カバー | 合計以外の畳み込み、文字列連結、集計状態オブジェクト |

既存で比較的カバーできているもの:

- `Where` / `Select` / `GroupBy` / `OrderBy` / `Count` / `GroupJoin` → [LINQ 操作](../linq/linq-samples.md)

## パターンマッチング

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| `is` パターン | 薄い | 型パターン、プロパティパターン、リストパターン、null 判定 |
| デコンストラクトパターン | 薄い | tuple / record / 独自 `Deconstruct` と `switch` の組み合わせ |

既存で比較的カバーできているもの:

- `switch` 式 → [型システム](../type-system/type-system-samples.md)

## デリゲート・イベント・属性

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| ラムダ式 / デリゲート | 薄い | `Func<T>`、`Action<T>`、独自 `delegate`、コールバック設計 |
| イベント | 未カバー | `event EventHandler<TEventArgs>`、購読と解除、イベントを使いすぎない設計 |
| 属性 `Attribute` | 薄い | 独自属性、属性の読み取り、`AttributeUsage` |

既存で自然に登場しているもの:

- ラムダ式 → LINQ、Async、Observability のテスト
- 属性 → xUnit の `[Fact]` / `[Theory]`、`[GeneratedRegex]`

## リソース管理・イテレータ

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| `using` / `Dispose` | 薄い | `IDisposable` 実装、`using` statement、`using var`、`await using` |
| `yield return` | 薄い | 遅延評価 iterator、途中終了、例外タイミング、`IEnumerable<T>` の注意点 |

既存で自然に登場しているもの:

- `using var` / `await using` → HttpClient / EF Core tests
- `yield return` → LINQ の `SkipLong`、Async tests

## by-ref・unsafe

| 項目 | 状況 | 追加したいスニペット |
|---|---|---|
| `ref` パラメータ | 未カバー | 呼び出し元の変数を書き換える例、使いどころを限定する説明 |
| `in` パラメータ | 未カバー | 大きい struct の読み取り専用参照渡し、過剰利用を避ける説明 |
| `out` パラメータ | 薄い | `Try...` パターン、戻り値との使い分け |
| `unsafe` / ポインタ | 未カバー | 原則使わない方針、必要になる場面、`unsafe` を隔離する設計 |

既存で自然に登場しているもの:

- `out` → `TryParseOrderStatus`、設定値の parse、security helper

## 追加時の推奨カテゴリ

今後実装する場合は、次のように分けると既存構成と合わせやすいです。

- `src/DotnetBackendSnippets.Core/LanguageFeatures/`
- `tests/DotnetBackendSnippets.Tests/LanguageFeatures/`
- `docs/snippets/language-features/`

候補ファイル:

- `TypeInferenceSamples`
- `NullableValueSamples`
- `TupleSamples`
- `ObjectOrientedSamples`
- `AdvancedCollectionSamples`
- `LinqAdvancedSamples`
- `PatternMatchingSamples`
- `DelegateEventAttributeSamples`
- `ResourceManagementSamples`
- `ByRefAndUnsafeSamples`

## 優先順位

1. `var` / nullable 値型 / tuple / deconstruct
2. OOP: 継承、`abstract`、`sealed`、演算子オーバーロード、インデクサ
3. `Queue<T>` / `Stack<T>` / `Join` / `Aggregate`
4. delegate / event / custom attribute
5. `ref` / `in` / `unsafe`
