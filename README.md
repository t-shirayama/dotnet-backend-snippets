# dotnet-backend-snippets

C# / .NET のバックエンド開発で使うスニペット集です。ASP.NET Core、Entity Framework Core、Dependency Injection、Configuration、Logging、HttpClient、Background Service、テストなどの実装パターンを整理します。

このリポジトリでは、単なるコード片ではなく **動作確認できるスニペット** として管理することを重視します。各スニペットは、可能な限り実装コード・テストコード・説明ドキュメントをセットで用意します。

## 方針

- 実行可能、またはテストで検証できる形でコード例を管理する
- スニペットごとに目的、使いどころ、注意点を記録する
- 将来の .NET バージョンアップや実装変更に追従しやすい構成にする
- README は利用者向けの概要、`AGENTS.md` は coding agent 向けの作業ルールとして分ける

## 扱う内容

- ASP.NET Core Web API
- Controller / Minimal API / Middleware
- Dependency Injection
- Configuration
- Logging
- Error Handling
- Validation
- Entity Framework Core
- Authentication / Authorization
- HttpClient
- Background Service
- Unit Test / Integration Test
- Docker
- Utilities

## 想定ディレクトリ構成

```text
dotnet-backend-snippets/
  src/
    DotnetBackendSnippets/
      <Category>/
  tests/
    DotnetBackendSnippets.Tests/
      <Category>/
  docs/
    snippets/
      <category>/
  AGENTS.md
  README.md
```

カテゴリ例:

- `AspNetCore`
- `DependencyInjection`
- `Configuration`
- `Logging`
- `ErrorHandling`
- `Validation`
- `EntityFrameworkCore`
- `Authentication`
- `Authorization`
- `HttpClient`
- `BackgroundServices`
- `Utilities`

## スニペットの追加単位

スニペットは、原則として以下の3点をセットで追加します。

1. 実装コード
2. テストコード
3. 説明ドキュメント

例:

```text
src/DotnetBackendSnippets/Logging/LoggingSamples.cs
tests/DotnetBackendSnippets.Tests/Logging/LoggingSamplesTests.cs
docs/snippets/logging/basic-logging.md
```

## ドキュメント構成

各スニペットの説明ドキュメントは、できるだけ以下の形式で整理します。

```markdown
# スニペット名

## 目的

このスニペットをいつ、何のために使うのかを説明します。

## 実装

対象の実装コードへのリンクを書きます。

## テスト

対象のテストコードへのリンクを書きます。

## 使い方

利用例や呼び出し例を書きます。

## メモ

- 注意点
- よくあるミス
- 関連する補足
```

## テスト実行

ソリューションまたはプロジェクトを追加した後は、可能な範囲で以下を実行します。

```bash
dotnet restore
dotnet build
dotnet test
```

特定のテストプロジェクトだけを確認する場合:

```bash
dotnet test tests/DotnetBackendSnippets.Tests/DotnetBackendSnippets.Tests.csproj
```

## ライセンス

このリポジトリは、個人の学習・開発メモとして利用します。
