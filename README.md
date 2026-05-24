# dotnet-backend-snippets

C# / .NET のバックエンド開発でよく使う実装パターンを、**テストで動作確認できるスニペット集** として管理するリポジトリです。

各スニペットは、原則として次の3点をセットで用意します。

1. 実装コード
2. テストコード
3. 説明ドキュメント

Target Framework は `net8.0` です。

通常の純粋な C# サンプルは `DotnetBackendSnippets.Core` に置きます。unsafe を扱う `ByRefAndUnsafeSamples` だけは `DotnetBackendSnippets.UnsafeSamples` に分け、unsafe 許可を通常スニペットへ広げないようにしています。

## 目的

- バックエンド開発で再利用しやすい C# / .NET のコード例を整理する
- `dotnet test` でスニペットの動作を確認できる状態にする
- 実装の使いどころ、注意点、テスト観点を日本語ドキュメントとして残す
- 学習用にも実務の下書きにも使いやすい、小さく読みやすいサンプルを保つ

## スニペット追加時のルール

新しいスニペットを追加する場合は、原則として以下を同時に追加します。

- 実装コード: `src/DotnetBackendSnippets.Core/<Category>/` または依存に応じた `src/DotnetBackendSnippets.*<Category>/`
- テストコード: `tests/DotnetBackendSnippets.Tests/<Category>/`
- 説明ドキュメント: `docs/snippets/<category>/`

例:

```text
src/DotnetBackendSnippets.AspNetCore/Logging/LoggingSamples.cs
tests/DotnetBackendSnippets.Tests/Logging/LoggingSamplesTests.cs
docs/snippets/logging/basic-logging.md
```

純粋な C# のサンプルは `DotnetBackendSnippets.Core` に置き、ASP.NET Core / Microsoft.Extensions 系のサンプルは `DotnetBackendSnippets.AspNetCore` に分けます。Entity Framework Core のサンプルは `DotnetBackendSnippets.EntityFrameworkCore` に置きます。unsafe が必要なサンプルは `DotnetBackendSnippets.UnsafeSamples` に分けます。

## 追加済みカテゴリ

- API
- Async / Await
- Background Services
- Collections
- Configuration
- Date and Time
- Dependency Injection
- Entity Framework Core
- Error Handling
- File Handling
- HttpClient
- HttpClientFactory
- LINQ
- Logging
- Language Features
- Numbers
- Observability
- Options
- Security
- Strings
- Type System
- Unsafe Samples
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

スニペットを困りごとから探す場合は、[スニペット索引](docs/snippets/README.md) の逆引きも参照してください。

基礎カテゴリを深掘りする逆引きとして、[文字列操作の実務逆引き](docs/snippets/strings/string-samples.md#実務逆引き)、[数値処理の実務逆引き](docs/snippets/numbers/number-samples.md#実務逆引き)、[日付・時刻処理の実務逆引き](docs/snippets/date-and-time/date-and-time-samples.md#実務逆引き)、[LINQ 実務逆引き](docs/snippets/linq/linq-samples.md#実務逆引き)、[C# 言語機能サンプル](docs/snippets/language-features/language-feature-samples.md) も用意しています。

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

- Authentication
- Authorization
- Docker
- DTO Mapping
- Repository Pattern
- Regular Expressions
- Caching
- Serialization

## ライセンス

MIT License です。コード例は業務コード、社内資料、学習用途などで再利用できます。詳しくは [LICENSE](LICENSE) を確認してください。
