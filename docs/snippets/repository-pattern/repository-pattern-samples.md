# Repository Pattern の基本

## 目的

EF Core と組み合わせる repository pattern の使いどころを確認するためのスニペットです。DbSet の CRUD をそのまま包むのではなく、業務固有の query や永続化先の差し替えが必要な場合に限定する考え方を示します。

## 実装

`src/DotnetBackendSnippets.EntityFrameworkCore/RepositoryPatternSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/EntityFrameworkCore/RepositoryPatternSamplesTests.cs`

## 使い方

- `IBlogPostRepository` は記事に関する業務操作だけを公開します。
- `EfCoreBlogPostRepository` は `SampleBlogDbContext` を使って公開記事の取得と記事追加を行います。
- `ShouldUseRepository` は repository 抽象化を導入するかどうかの判断例です。

## メモ

- EF Core の `DbContext` 自体が Unit of Work と Repository に近い役割を持ちます。
- 単純な CRUD を包むだけの repository は、LINQ、Include、transaction、変更追跡の意図を隠しやすくなります。
- 業務固有の query、永続化先の差し替え、テスト用 fake が必要な境界では repository が役立ちます。

## 実務逆引き

- EF Core と repository pattern を組み合わせたい → `EfCoreBlogPostRepository`
- repository を導入すべきか判断したい → `ShouldUseRepository`
- DbSet の CRUD を包むだけの抽象化を避けたい → このページのメモ
