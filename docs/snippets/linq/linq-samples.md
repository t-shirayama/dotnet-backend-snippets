# LINQ 操作

## 目的

LINQ を使ったフィルタ、射影、グルーピング、並び替え、ページング、重複排除、左外部結合、フラット化、実務逆引きヘルパーの基本パターンをまとめます。LINQ はコレクションを SQL のような感覚で扱える C# の機能です。

## 実装

- `src/DotnetBackendSnippets.Core/Linq/LinqSamples.cs`
- `src/DotnetBackendSnippets.Core/Linq/LinqReverseLookupSamples.cs`

## テスト

- `tests/DotnetBackendSnippets.Tests/Linq/LinqSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/Linq/LinqReverseLookupSamplesTests.cs`

## 使い方

- `GetExpensiveOrderCategories` は条件に合う注文を抽出し、カテゴリ名だけを重複なしで返します。
- `SumAmountByCategory` はカテゴリごとに注文金額を合計します。
- `TopOrders` は金額が大きい順に上位 N 件を返します。
- `Page` はページ番号と件数から一部だけを取り出します。
- `DistinctByKey` は .NET 標準の `DistinctBy` を使い、指定したキーが同じ要素のうち最初の要素だけを残します。
- `DistinctByKeyWithGroupBy` は `GroupBy` で同じ処理を書く学習用の比較例です。
- `LeftJoinCustomerOrders` は注文がない顧客も残して集計します。
- `Flatten` は入れ子のコレクションを1つのコレクションにします。
- `WhereIf` は条件がある場合だけ `Where` を適用します。
- `SearchOrders` は任意条件を組み合わせた検索を行います。
- `FilterByIds` は ID 一覧を `HashSet` にして高速に照合します。
- `ToOrderListItems` は注文を一覧表示用 DTO に変換します。
- `SortOrders` は並び替え項目と昇順・降順を切り替えます。
- `CountByKey` はキーごとの件数を辞書で返します。
- `LatestPerKey` はキーごとの最新データを取り出します。
- `FindDuplicateKeys` は重複しているキーだけを返します。
- `ToDictionaryLastWins` はキー重複時に最後の値を採用します。
- `ToLookupDictionary` は1つのキーに複数値を持たせます。
- `SelectManyWithParent` は親の情報を残したまま子コレクションを展開します。

## メモ

- LINQ は読みやすい反面、遅延実行や複数回列挙に注意が必要です。このスニペットでは戻り値を `ToList` や `ToDictionary` などで確定しています。
- データベース向け LINQ とメモリ上の LINQ では使えるメソッドや性能特性が違う場合があります。
- `Page` は大きいページ番号でも `int` の掛け算でオーバーフローしないよう、スキップ件数を `long` で計算します。
- `Flatten` は内側のコレクションが `null` でない前提です。`null` が混ざる場合は呼び出し側で空コレクションに変換するか、事前に検証します。

## 実務逆引き

業務バックエンドで毎回調べがちな LINQ の使い方を、目的から探せる形でまとめます。

実装済みの項目は [`LinqSamples.cs`](../../../src/DotnetBackendSnippets.Core/Linq/LinqSamples.cs) または [`LinqReverseLookupSamples.cs`](../../../src/DotnetBackendSnippets.Core/Linq/LinqReverseLookupSamples.cs) の名前を示します。逆引き項目は実装済みメソッド名を記載しています。

### 絞り込みと条件分岐

- 金額が一定以上の注文カテゴリを取り出したい: 実装済み `GetExpensiveOrderCategories`
- 有効なユーザーだけを取り出したい: 実装済み `Where(user => user.IsActive)`
- null ではない値だけを処理したい: 実装済み `Where(value => value is not null)`
- 条件があるときだけ `Where` を足したい: 実装済み `WhereIf`
- 複数条件を読みやすく分けたい: 実装済み predicate メソッド化
- 日付範囲で検索したい: 実装済み `from <= value && value < to`
- ID 一覧に含まれるデータだけ取りたい: 実装済み `FilterByIds`
- 大文字小文字を無視して検索したい: 実装済み `StringComparer` と `StringComparison`
- 空文字や空白を除外したい: 実装済み `string.IsNullOrWhiteSpace`
- 条件に一致するものが1件でもあるか確認したい: 実装済み `Any`

### 射影と DTO 変換

- エンティティから DTO に変換したい: 実装済み `ToOrderListItems`
- 必要な列だけ取り出したい: 実装済み projection
- 匿名型で一時的に形を変えたい: 実装済み `new { ... }`
- record DTO に詰め替えたい: 実装済み `Select(x => new Response(...))`
- インデックス付きで変換したい: 実装済み `Select((item, index) => ...)`
- null 許容のプロパティを既定値にしたい: 実装済み `??`
- ネストした値を安全に取り出したい: 実装済み null 条件演算子
- 文字列を整形して返したい: 実装済み `Trim` と `Normalize`
- enum を表示名に変換したい: 実装済み `Select` 内の mapping
- DTO 変換を再利用したい: 実装済み expression / mapper メソッド

### 並び替えとページング

- 金額が大きい順に上位 N 件を取りたい: 実装済み `TopOrders`
- ページ番号と件数で一部だけ取りたい: 実装済み `Page`
- 同値の場合に ID 昇順で安定させたい: 実装済み `TopOrders`
- 動的に並び替え項目を変えたい: 実装済み `SortOrders`
- 昇順と降順を切り替えたい: 実装済み `SortOrders`
- 複数キーで並び替えたい: 実装済み `ThenBy`
- 最新順に表示したい: 実装済み `OrderByDescending(x => x.CreatedAt)`
- ページング前に必ず並び替えたい: 実装済み stable paging
- オフセットページングの上限を設けたい: 実装済み max page size
- keyset pagination を使いたい: 実装済み `Where(x => x.Id > lastId).Take(size)`

### 集計と GroupBy

- カテゴリごとの合計金額を出したい: 実装済み `SumAmountByCategory`
- ステータスごとの件数を数えたい: 実装済み `CountByKey`
- 月ごとの売上を集計したい: 実装済み `GroupBy(x => new { x.Date.Year, x.Date.Month })`
- 顧客ごとの最新注文を取りたい: 実装済み `LatestPerKey`
- グループごとの平均値を出したい: 実装済み `Average`
- グループごとの最小値と最大値を出したい: 実装済み `Min` / `Max`
- 空の集合でも安全に合計したい: 実装済み `DefaultIfEmpty`
- 集計結果を辞書にしたい: 実装済み `SumAmountByCategory`
- 複合キーで集計したい: 実装済み anonymous key
- `GroupBy(...).First()` の意味を明確にしたい: 実装済み `DistinctByKeyWithGroupBy`

### 重複排除と集合演算

- キーが同じ要素を重複排除したい: 実装済み `DistinctByKey`
- `GroupBy` で重複排除の仕組みを見たい: 実装済み `DistinctByKeyWithGroupBy`
- 大文字小文字を無視して文字列を重複排除したい: 実装済み `Distinct(StringComparer.OrdinalIgnoreCase)`
- 重複している値だけを取りたい: 実装済み `FindDuplicateKeys`
- 2つの一覧の共通部分を取りたい: 実装済み `Intersect`
- 2つの一覧の差分を取りたい: 実装済み `Except`
- 2つの一覧を重複なしで結合したい: 実装済み `Union`
- 既存 ID と新規 ID の差分を出したい: 実装済み `HashSet<T>`
- 重複検出を一回走査で行いたい: 実装済み `HashSet<T>.Add`
- 独自 comparer で比較したい: 実装済み `IEqualityComparer<T>`

### Join と関連データ

- 顧客ごとの注文数と合計金額を出したい: 実装済み `LeftJoinCustomerOrders`
- 注文がない顧客も残したい: 実装済み `LeftJoinCustomerOrders`
- 内部結合で一致するデータだけ取りたい: 実装済み `Join`
- 左外部結合を書きたい: 実装済み `GroupJoin` + `DefaultIfEmpty`
- 親子データをまとめて DTO にしたい: 実装済み `GroupJoin`
- ID から名前を引きたい: 実装済み `ToDictionary` + lookup
- 複合キーで join したい: 実装済み anonymous key
- join 後に集計したい: 実装済み `LeftJoinCustomerOrders`
- N+1 を避けて関連データを使いたい: 実装済み EF Core projection
- `Lookup<TKey, T>` を作りたい: 実装済み `ToLookup`

### SelectMany と入れ子データ

- 入れ子のコレクションを平坦化したい: 実装済み `Flatten`
- 注文一覧から明細一覧を取り出したい: 実装済み `SelectMany(order => order.Items)`
- 親の情報を残して子を展開したい: 実装済み `SelectManyWithParent`
- null の内側コレクションを扱いたい: 実装済み `SelectManyWithParent`
- タグ一覧を1つの一覧にしたい: 実装済み `SelectMany(x => x.Tags)`
- ネストした配列を DTO に変換したい: 実装済み `SelectMany` + `Select`
- 階層データを再帰的に展開したい: 実装済み recursive iterator
- カンマ区切り文字列を分割して平坦化したい: 実装済み `Split` + `SelectMany`
- 親ごとの子件数を数えたい: 実装済み `Select(x => new { Count = x.Children.Count })`
- `Flatten` の null 方針を明示したい: 実装済み `Flatten`

### 辞書化と materialize

- カテゴリごとの合計を辞書で返したい: 実装済み `SumAmountByCategory`
- ID をキーにして辞書化したい: 実装済み `ToDictionary(x => x.Id)`
- キー重複時に最後の値を採用したい: 実装済み `ToDictionaryLastWins`
- キー重複時に例外にしたくない: 実装済み `ToDictionaryLastWins`
- 大文字小文字を無視する辞書にしたい: 実装済み `StringComparer.OrdinalIgnoreCase`
- 1キーに複数値を持たせたい: 実装済み `ToLookupDictionary`
- 遅延実行を確定したい: 実装済み `ToList`
- 何度も列挙する前に配列化したい: 実装済み `ToArray`
- 件数だけ必要なときは materialize しない: 実装済み `Count`
- `IEnumerable<T>` と `IReadOnlyList<T>` の境界を決めたい: 実装済み API design

### 遅延実行と性能注意

- LINQ がいつ実行されるか知りたい: 実装済み deferred execution sample
- 同じ query を複数回列挙したくない: 実装済み materialize once
- `Where` の順番で無駄な処理を減らしたい: 実装済み cheap filter first
- `Any` と `Count() > 0` を使い分けたい: 実装済み existence check
- `First` と `FirstOrDefault` を使い分けたい: 実装済み not found handling
- `Single` と `First` を使い分けたい: 実装済み uniqueness guarantee
- 大量データで `Contains` を速くしたい: 実装済み `HashSet<T>`
- `OrderBy` 後の `Take` で上位件数を取る: 実装済み `TopOrders`
- ページングの skip 数 overflow を避けたい: 実装済み `Page`
- LINQ を読みやすく分割したい: 実装済み named intermediate query

### EF Core LINQ 注意点

- DB 側で実行される LINQ とメモリ上 LINQ を区別したい: 実装済み `IQueryable<T>` / `IEnumerable<T>`
- `AsNoTracking` で読み取り専用 query にしたい: 実装済み EF Core query sample
- DTO projection で必要列だけ読む: 実装済み `Select`
- `Include` と projection を使い分けたい: 実装済み EF Core include sample
- `Contains` が SQL の `IN` になる場面を知りたい: 実装済み EF Core translation sample
- DB で変換できないメソッドを避けたい: 実装済み translation boundary
- `ToList` の位置で SQL が変わることを確認したい: 実装済み materialization timing
- ページングは `OrderBy` とセットにしたい: 実装済み EF Core paging sample
- 大文字小文字検索の照合順序に注意したい: 実装済み collation sample
- N+1 を避ける query にしたい: 実装済み projection / split query
