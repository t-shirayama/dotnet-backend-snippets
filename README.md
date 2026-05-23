# dotnet-backend-snippets

C# / .NET のバックエンド開発でよく使う実装パターンを、**テストで動作確認できるスニペット集** として管理するリポジトリです。

各スニペットは、原則として次の3点をセットで用意します。

1. 実装コード
2. テストコード
3. 説明ドキュメント

Target Framework は `net8.0` です。

## 目的

- バックエンド開発で再利用しやすい C# / .NET のコード例を整理する
- `dotnet test` でスニペットの動作を確認できる状態にする
- 実装の使いどころ、注意点、テスト観点を日本語ドキュメントとして残す
- 学習用にも実務の下書きにも使いやすい、小さく読みやすいサンプルを保つ

## ディレクトリ構成

```text
dotnet-backend-snippets/
  src/
    DotnetBackendSnippets/
      Collections/
      Configuration/
      DateAndTime/
      DependencyInjection/
      ErrorHandling/
      HttpClient/
      Linq/
      Logging/
      Numbers/
      Strings/
      TypeSystem/
      Utilities/
      Validation/
  tests/
    DotnetBackendSnippets.Tests/
      Collections/
      Configuration/
      DateAndTime/
      DependencyInjection/
      ErrorHandling/
      HttpClient/
      Linq/
      Logging/
      Numbers/
      Strings/
      TypeSystem/
      Utilities/
      Validation/
  docs/
    snippets/
      collections/
      configuration/
      date-and-time/
      dependency-injection/
      error-handling/
      http-client/
      linq/
      logging/
      numbers/
      strings/
      type-system/
      utilities/
      validation/
      README.md
  .gitignore
  dotnet-backend-snippets.sln
  AGENTS.md
  README.md
```

## スニペット追加時のルール

新しいスニペットを追加する場合は、原則として以下を同時に追加します。

- 実装コード: `src/DotnetBackendSnippets/<Category>/`
- テストコード: `tests/DotnetBackendSnippets.Tests/<Category>/`
- 説明ドキュメント: `docs/snippets/<category>/`

例:

```text
src/DotnetBackendSnippets/Logging/LoggingSamples.cs
tests/DotnetBackendSnippets.Tests/Logging/LoggingSamplesTests.cs
docs/snippets/logging/basic-logging.md
```

## 追加済みカテゴリ

- Collections
- Configuration
- Date and Time
- Dependency Injection
- Error Handling
- HttpClient
- LINQ
- Logging
- Numbers
- Strings
- Type System
- Utilities
- Validation

## テスト実行方法

```bash
dotnet restore
dotnet build
dotnet test
```

フォーマット確認:

```bash
dotnet format --verify-no-changes
```

特定のテストプロジェクトだけを確認する場合:

```bash
dotnet test tests/DotnetBackendSnippets.Tests/DotnetBackendSnippets.Tests.csproj
```

## ドキュメント構成

各スニペットの説明ドキュメントは、次の構成を基本にします。

```markdown
# スニペット名

## 目的

## 実装

## テスト

## 使い方

## メモ
```

`実装` と `テスト` には、対象ファイルへの相対パスを記載します。

## 今後追加したいカテゴリ

- ASP.NET Core Web API
- Minimal API
- Middleware
- Entity Framework Core
- Authentication
- Authorization
- Background Services
- Docker
- DTO Mapping
- Repository Pattern
- Async / Await
- File I/O
- Regular Expressions
- Caching
- Serialization

## ライセンス

このリポジトリは、個人の学習・開発メモとして利用します。
