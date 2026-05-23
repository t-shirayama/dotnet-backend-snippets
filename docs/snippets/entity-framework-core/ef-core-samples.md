# Entity Framework Core の基本

## 目的

Entity Framework Core でよく使う DbContext 登録、読み取り専用クエリ、ページング、projection、Include、transaction、soft delete を小さく確認するためのスニペットです。

EF Core は .NET の ORM です。ORM は C# のオブジェクトとデータベースのテーブルを対応づけ、LINQ でクエリを書けるようにする仕組みです。

## 実装

`src/DotnetBackendSnippets.EntityFrameworkCore/EfCoreSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/EntityFrameworkCore/EfCoreSamplesTests.cs`

## 使い方

- `AddSampleBlogDbContext` は `DbContext` を DI に登録します。実アプリでは `UseSqlServer`、`UseNpgsql`、`UseSqlite` など、利用する provider を渡します。
- `GetRecentPostsReadOnlyAsync` は `AsNoTracking` と projection を使い、更新しない一覧取得を軽くします。
- `PagePostsAsync` は `OrderBy`、`Skip`、`Take` でページングします。大きすぎるページ番号でも `int` の掛け算でオーバーフローしないよう、スキップ件数を `long` で計算します。
- `GetBlogSummariesAsync` は `Include` せず、必要な列だけを DTO に射影します。一覧画面や API の軽いレスポンス向きです。
- `GetBlogWithPostsAsync` は `Include` で関連データを読み込みます。集約全体を扱う詳細画面や更新処理向きです。
- `SoftDeletePostInTransactionAsync` は transaction 内で `IsDeleted` を更新します。`HasQueryFilter` により通常のクエリから削除済みデータを隠します。

## メモ

- InMemory provider は速く、単純な repository や query filter のテストに向いています。ただし relational database ではないため、SQL、制約、transaction、照合順序の挙動は本番 DB と異なります。
- SQLite in-memory は relational database として動くため、外部プロセスなしで外部キー制約や SQL 変換に近い挙動を確認したいテストに向いています。
- このリポジトリのテストでは追加依存を最小限にするため、InMemory provider を使っています。SQL 生成や制約まで確認したいスニペットを追加する場合は、SQLite in-memory 用のテストを別途追加するのが安全です。
- `AsNoTracking` は読み取り専用では有効ですが、取得した entity をそのまま更新する処理には向きません。
- 一覧 API では `Include` より projection を優先すると、取得列とレスポンス形状を明確にしやすくなります。

## 実務逆引き

- DbContext を DI に登録したい → `AddSampleBlogDbContext`
- InMemory provider で repository をテストしたい → `EfCoreSamplesTests`
- SQLite in-memory と InMemory provider を使い分けたい → メモの provider 方針
- `AsNoTracking` を使いたい → `GetRecentPostsReadOnlyAsync`
- pagination query を書きたい → `PagePostsAsync`
- `Include` と projection を使い分けたい → `GetBlogWithPostsAsync` / `GetBlogSummariesAsync`
- transaction を使いたい → `SoftDeletePostInTransactionAsync`
- soft delete の query filter を書きたい → `HasQueryFilter`
- optimistic concurrency を扱いたい → 追加候補
- migration を追加・適用したい → 追加候補
