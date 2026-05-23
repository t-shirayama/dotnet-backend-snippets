# LINQ 操作

## 目的

LINQ を使ったフィルタ、射影、グルーピング、並び替え、ページング、重複排除、左外部結合、フラット化の基本パターンをまとめます。LINQ はコレクションを SQL のような感覚で扱える C# の機能です。

## 実装

`src/DotnetBackendSnippets.Core/Linq/LinqSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Linq/LinqSamplesTests.cs`

## 使い方

- `GetExpensiveOrderCategories` は条件に合う注文を抽出し、カテゴリ名だけを重複なしで返します。
- `SumAmountByCategory` はカテゴリごとに注文金額を合計します。
- `TopOrders` は金額が大きい順に上位 N 件を返します。
- `Page` はページ番号と件数から一部だけを取り出します。
- `DistinctByKey` は .NET 標準の `DistinctBy` を使い、指定したキーが同じ要素のうち最初の要素だけを残します。
- `DistinctByKeyWithGroupBy` は `GroupBy` で同じ処理を書く学習用の比較例です。
- `LeftJoinCustomerOrders` は注文がない顧客も残して集計します。
- `Flatten` は入れ子のコレクションを1つのコレクションにします。

## メモ

- LINQ は読みやすい反面、遅延実行や複数回列挙に注意が必要です。このスニペットでは戻り値を `ToList` などで確定しています。
- データベース向け LINQ とメモリ上の LINQ では使えるメソッドや性能特性が違う場合があります。
- `Page` は大きいページ番号でも `int` の掛け算でオーバーフローしないよう、スキップ件数を `long` で計算します。
- `Flatten` は内側のコレクションが `null` でない前提です。`null` が混ざる場合は呼び出し側で空コレクションに変換するか、事前に検証します。

## 実務逆引き

業務バックエンドで毎回調べがちな LINQ の使い方を、目的から探せる形でまとめます。

実装済みの項目は [LINQ 操作](linq-samples.md) と [`LinqSamples.cs`](../../../src/DotnetBackendSnippets.Core/Linq/LinqSamples.cs) の名前を示します。未実装の項目は「追加候補」として扱います。

### 絞り込みと条件分岐

- 金額が一定以上の注文カテゴリを取り出したい: 実装済み `GetExpensiveOrderCategories`
- 有効なユーザーだけを取り出したい: 追加候補 `Where(user => user.IsActive)`
- null ではない値だけを処理したい: 追加候補 `Where(value => value is not null)`
- 条件があるときだけ `Where` を足したい: 追加候補 `WhereIf`
- 複数条件を読みやすく分けたい: 追加候補 predicate メソッド化
- 日付範囲で検索したい: 追加候補 `from <= value && value < to`
- ID 一覧に含まれるデータだけ取りたい: 追加候補 `ids.Contains(entity.Id)`
- 大文字小文字を無視して検索したい: 追加候補 `StringComparer` と `StringComparison`
- 空文字や空白を除外したい: 追加候補 `string.IsNullOrWhiteSpace`
- 条件に一致するものが1件でもあるか確認したい: 追加候補 `Any`

### 射影と DTO 変換

- エンティティから DTO に変換したい: 追加候補 `Select`
- 必要な列だけ取り出したい: 追加候補 projection
- 匿名型で一時的に形を変えたい: 追加候補 `new { ... }`
- record DTO に詰め替えたい: 追加候補 `Select(x => new Response(...))`
- インデックス付きで変換したい: 追加候補 `Select((item, index) => ...)`
- null 許容のプロパティを既定値にしたい: 追加候補 `??`
- ネストした値を安全に取り出したい: 追加候補 null 条件演算子
- 文字列を整形して返したい: 追加候補 `Trim` と `Normalize`
- enum を表示名に変換したい: 追加候補 `Select` 内の mapping
- DTO 変換を再利用したい: 追加候補 expression / mapper メソッド

### 並び替えとページング

- 金額が大きい順に上位 N 件を取りたい: 実装済み `TopOrders`
- ページ番号と件数で一部だけ取りたい: 実装済み `Page`
- 同値の場合に ID 昇順で安定させたい: 実装済み `TopOrders`
- 動的に並び替え項目を変えたい: 追加候補 sort key switch
- 昇順と降順を切り替えたい: 追加候補 `OrderBy` / `OrderByDescending`
- 複数キーで並び替えたい: 追加候補 `ThenBy`
- 最新順に表示したい: 追加候補 `OrderByDescending(x => x.CreatedAt)`
- ページング前に必ず並び替えたい: 追加候補 stable paging
- オフセットページングの上限を設けたい: 追加候補 max page size
- keyset pagination を使いたい: 追加候補 `Where(x => x.Id > lastId).Take(size)`

### 集計と GroupBy

- カテゴリごとの合計金額を出したい: 実装済み `SumAmountByCategory`
- ステータスごとの件数を数えたい: 追加候補 `GroupBy` + `Count`
- 月ごとの売上を集計したい: 追加候補 `GroupBy(x => new { x.Date.Year, x.Date.Month })`
- 顧客ごとの最新注文を取りたい: 追加候補 `GroupBy` + `MaxBy`
- グループごとの平均値を出したい: 追加候補 `Average`
- グループごとの最小値と最大値を出したい: 追加候補 `Min` / `Max`
- 空の集合でも安全に合計したい: 追加候補 `DefaultIfEmpty`
- 集計結果を辞書にしたい: 実装済み `SumAmountByCategory`
- 複合キーで集計したい: 追加候補 anonymous key
- `GroupBy(...).First()` の意味を明確にしたい: 実装済み `DistinctByKeyWithGroupBy`

### 重複排除と集合演算

- キーが同じ要素を重複排除したい: 実装済み `DistinctByKey`
- `GroupBy` で重複排除の仕組みを見たい: 実装済み `DistinctByKeyWithGroupBy`
- 大文字小文字を無視して文字列を重複排除したい: 追加候補 `Distinct(StringComparer.OrdinalIgnoreCase)`
- 重複している値だけを取りたい: 追加候補 `GroupBy` + `Where(g => g.Count() > 1)`
- 2つの一覧の共通部分を取りたい: 追加候補 `Intersect`
- 2つの一覧の差分を取りたい: 追加候補 `Except`
- 2つの一覧を重複なしで結合したい: 追加候補 `Union`
- 既存 ID と新規 ID の差分を出したい: 追加候補 `HashSet<T>`
- 重複検出を一回走査で行いたい: 追加候補 `HashSet<T>.Add`
- 独自 comparer で比較したい: 追加候補 `IEqualityComparer<T>`

### Join と関連データ

- 顧客ごとの注文数と合計金額を出したい: 実装済み `LeftJoinCustomerOrders`
- 注文がない顧客も残したい: 実装済み `LeftJoinCustomerOrders`
- 内部結合で一致するデータだけ取りたい: 追加候補 `Join`
- 左外部結合を書きたい: 追加候補 `GroupJoin` + `DefaultIfEmpty`
- 親子データをまとめて DTO にしたい: 追加候補 `GroupJoin`
- ID から名前を引きたい: 追加候補 `ToDictionary` + lookup
- 複合キーで join したい: 追加候補 anonymous key
- join 後に集計したい: 実装済み `LeftJoinCustomerOrders`
- N+1 を避けて関連データを使いたい: 追加候補 EF Core projection
- `Lookup<TKey, T>` を作りたい: 追加候補 `ToLookup`

### SelectMany と入れ子データ

- 入れ子のコレクションを平坦化したい: 実装済み `Flatten`
- 注文一覧から明細一覧を取り出したい: 追加候補 `SelectMany(order => order.Items)`
- 親の情報を残して子を展開したい: 追加候補 `SelectMany` の result selector
- null の内側コレクションを扱いたい: 追加候補 null を空配列に変換
- タグ一覧を1つの一覧にしたい: 追加候補 `SelectMany(x => x.Tags)`
- ネストした配列を DTO に変換したい: 追加候補 `SelectMany` + `Select`
- 階層データを再帰的に展開したい: 追加候補 recursive iterator
- カンマ区切り文字列を分割して平坦化したい: 追加候補 `Split` + `SelectMany`
- 親ごとの子件数を数えたい: 追加候補 `Select(x => new { Count = x.Children.Count })`
- `Flatten` の null 方針を明示したい: 実装済み `Flatten`

### 辞書化と materialize

- カテゴリごとの合計を辞書で返したい: 実装済み `SumAmountByCategory`
- ID をキーにして辞書化したい: 追加候補 `ToDictionary(x => x.Id)`
- キー重複時に最後の値を採用したい: 追加候補 `GroupBy` + `Last`
- キー重複時に例外にしたくない: 追加候補 safe dictionary helper
- 大文字小文字を無視する辞書にしたい: 追加候補 `StringComparer.OrdinalIgnoreCase`
- 1キーに複数値を持たせたい: 追加候補 `ToLookup`
- 遅延実行を確定したい: 追加候補 `ToList`
- 何度も列挙する前に配列化したい: 追加候補 `ToArray`
- 件数だけ必要なときは materialize しない: 追加候補 `Count`
- `IEnumerable<T>` と `IReadOnlyList<T>` の境界を決めたい: 追加候補 API design

### 遅延実行と性能注意

- LINQ がいつ実行されるか知りたい: 追加候補 deferred execution sample
- 同じ query を複数回列挙したくない: 追加候補 materialize once
- `Where` の順番で無駄な処理を減らしたい: 追加候補 cheap filter first
- `Any` と `Count() > 0` を使い分けたい: 追加候補 existence check
- `First` と `FirstOrDefault` を使い分けたい: 追加候補 not found handling
- `Single` と `First` を使い分けたい: 追加候補 uniqueness guarantee
- 大量データで `Contains` を速くしたい: 追加候補 `HashSet<T>`
- `OrderBy` 後の `Take` で上位件数を取る: 実装済み `TopOrders`
- ページングの skip 数 overflow を避けたい: 実装済み `Page`
- LINQ を読みやすく分割したい: 追加候補 named intermediate query

### EF Core LINQ 注意点

- DB 側で実行される LINQ とメモリ上 LINQ を区別したい: 追加候補 `IQueryable<T>` / `IEnumerable<T>`
- `AsNoTracking` で読み取り専用 query にしたい: 追加候補 EF Core query sample
- DTO projection で必要列だけ読む: 追加候補 `Select`
- `Include` と projection を使い分けたい: 追加候補 EF Core include sample
- `Contains` が SQL の `IN` になる場面を知りたい: 追加候補 EF Core translation sample
- DB で変換できないメソッドを避けたい: 追加候補 translation boundary
- `ToList` の位置で SQL が変わることを確認したい: 追加候補 materialization timing
- ページングは `OrderBy` とセットにしたい: 追加候補 EF Core paging sample
- 大文字小文字検索の照合順序に注意したい: 追加候補 collation sample
- N+1 を避ける query にしたい: 追加候補 projection / split query
