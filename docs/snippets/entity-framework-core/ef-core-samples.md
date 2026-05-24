# Entity Framework Core の基本

## 目的

Entity Framework Core でよく使う DbContext 登録、読み取り専用クエリ、ページング、projection、Include、transaction、soft delete、migration コマンド、optimistic concurrency、unique constraint、監査日時、SaveChanges interceptor、transaction retry を小さく確認するためのスニペットです。

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
- `UpdatePostTitleWithConcurrencyAsync` は `ConcurrencyStamp` を使って楽観的同時実行制御を行います。
- `ApplyAuditValues` は `ChangeTracker` から追加・更新された記事に `CreatedAt` / `UpdatedAt` を設定します。
- `IsUniqueConstraintViolation` は保存例外が代表的な一意制約違反かどうかを判定します。
- `AuditSaveChangesInterceptor` は `SaveChanges` 時に監査日時を設定します。
- `ExecuteInTransactionWithRetryAsync` は transaction 内処理を一時的な失敗だけ retry します。
- `CreateAddMigrationCommand` / `CreateApplyMigrationCommand` / `CreateMigrationBundleCommand` は shell 文字列ではなく引数配列として `dotnet ef` コマンドを組み立てます。

## メモ

- InMemory provider は速く、単純な repository や query filter のテストに向いています。ただし relational database ではないため、SQL、制約、transaction、照合順序の挙動は本番 DB と異なります。
- SQLite in-memory は relational database として動くため、外部プロセスなしで外部キー制約や SQL 変換に近い挙動を確認したいテストに向いています。
- このリポジトリのテストでは軽い確認に InMemory provider を使い、transaction など relational database 寄りの挙動は SQLite in-memory でも確認します。
- `AsNoTracking` は読み取り専用では有効ですが、取得した entity をそのまま更新する処理には向きません。
- 一覧 API では `Include` より projection を優先すると、取得列とレスポンス形状を明確にしやすくなります。
- optimistic concurrency は「最後に保存した人が勝つ」では困る更新画面や API で使います。競合時は再読み込みや差分確認を促すレスポンスに変換します。
- migration bundle はアプリ起動時に自動 migration したくない環境で、デプロイ手順として DB 更新を分けたい場合に向いています。
- 監査日時は実アプリでは `TimeProvider` などで現在時刻を注入すると、テストしやすくなります。
- provider 側に retry 実行戦略がある場合はそれを優先し、独自 retry は一時的な例外だけに限定します。

## 実務逆引き

- DbContext を DI に登録したい → `AddSampleBlogDbContext`
- InMemory provider で repository をテストしたい → `EfCoreSamplesTests`
- SQLite in-memory と InMemory provider を使い分けたい → メモの provider 方針
- `AsNoTracking` を使いたい → `GetRecentPostsReadOnlyAsync`
- pagination query を書きたい → `PagePostsAsync`
- `Include` と projection を使い分けたい → `GetBlogWithPostsAsync` / `GetBlogSummariesAsync`
- transaction を使いたい → `SoftDeletePostInTransactionAsync`
- soft delete の query filter を書きたい → `HasQueryFilter`
- optimistic concurrency を扱いたい → `UpdatePostTitleWithConcurrencyAsync`
- migration を追加・適用・bundle 化したい → `CreateAddMigrationCommand` / `CreateApplyMigrationCommand` / `CreateMigrationBundleCommand`
- unique constraint violation を判定したい → `IsUniqueConstraintViolation`
- CreatedAt / UpdatedAt を設定したい → `ApplyAuditValues`
- SaveChanges interceptor で監査カラムを設定したい → `AuditSaveChangesInterceptor`
- transaction retry を扱いたい → `ExecuteInTransactionWithRetryAsync`
