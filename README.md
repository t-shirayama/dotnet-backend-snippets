# dotnet-backend-snippets

[![dotnet-ci](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/dotnet-ci.yml)
[![format](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/format.yml/badge.svg)](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/format.yml)
[![docs](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/docs.yml/badge.svg)](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/docs.yml)
[![codeql](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/codeql.yml/badge.svg)](https://github.com/t-shirayama/dotnet-backend-snippets/actions/workflows/codeql.yml)

C# / .NET のバックエンド開発でよく使う実装パターンを、**テストで動作確認できるスニペット集** として管理するリポジトリです。

このリポジトリは NuGet ライブラリではありません。業務コードや学習用メモへ写しやすい小さなサンプルを、実装・テスト・説明のセットで保守します。

各スニペットは、原則として次の3点をセットで用意します。

1. 実装コード
2. テストコード
3. 説明ドキュメント

Target Framework は `net10.0` です。CI では .NET SDK `10.0.x` を使います。Target Framework や主要パッケージのメジャー更新は、互換性とサンプルの再現性を確認する移行作業として扱います。

通常の純粋な C# サンプルは `DotnetBackendSnippets.Core` に置きます。unsafe を扱う `ByRefAndUnsafeSamples` だけは `DotnetBackendSnippets.UnsafeSamples` に分け、unsafe 許可を通常スニペットへ広げないようにしています。

## 目的

- バックエンド開発で再利用しやすい C# / .NET のコード例を整理する
- `dotnet test` でスニペットの動作を確認できる状態にする
- 実装の使いどころ、注意点、テスト観点を日本語ドキュメントとして残す
- 学習用にも実務の下書きにも使いやすい、小さく読みやすいサンプルを保つ

## 成熟度と利用上の注意

- サンプルの安定度: 実装・テスト・docs が揃ったものを `Implemented` として扱います。
- 破壊的変更: 学習用スニペット集なので、より安全で実務的な形へ直すために public API を変えることがあります。
- 実務利用: そのまま貼り付けず、認証、認可、ログ、例外、監査、性能、脅威モデルに合わせて調整してください。
- テスト方針: 正常系、異常系、境界値を中心に xUnit で確認します。外部通信や外部 DB は原則使いません。
- カバレッジ方針: CI で coverage artifact を出しますが、数値の達成率より「実務上の分岐と境界がテストされていること」を優先します。

## 品質チェック

このリポジトリでは GitHub Actions で次を確認しています。

- restore / build / test / coverage
- NuGet パッケージの脆弱性チェック
- `dotnet format`
- Markdown リンクチェック
- スニペットの実装・テスト・docs 棚卸しチェック
- CodeQL による静的解析

`main` へ取り込む変更は、上記の CI が通っている状態を前提にします。GitHub 側の branch protection では、`dotnet-ci`、`format`、`docs`、`codeql` を必須チェックにする運用を推奨します。

## バージョン管理方針

- 現在は `net10.0` LTS を基準にします。
- .NET 11 への移行や `<TargetFrameworks>` によるマルチターゲット化は、個別の issue / PR で検討します。
- EF Core 10 は `net10.0` と併用する方針として採用済みです。理由は [net10.0 と EF Core 10 への移行](docs/decisions/net10-ef-core-10.md) に残しています。
- テスト系パッケージの一部は major version が新しいものを採用しています。理由は [テスト系パッケージの major version 採用](docs/decisions/test-tooling-major-versions.md) に残しています。
- NuGet の patch / minor 更新は、Dependabot と CI で確認して取り込みます。
- xUnit v3、EF Core 11 などのメジャー更新は、テスト実行方法や対象 .NET バージョンへの影響を確認してから移行します。
- GitHub Actions の更新は、CI が通るものから早めに取り込みます。

## はじめに見るおすすめ順

初見の場合は、まずよく使う小さな処理から見ると全体像をつかみやすいです。

1. [Strings](docs/snippets/strings/string-samples.md): 正規化、マスク、エスケープ、キー生成
2. [Numbers](docs/snippets/numbers/number-samples.md): 丸め、割合、ページング、金額計算
3. [Date and Time](docs/snippets/date-and-time/date-and-time-samples.md): 範囲、営業日、UTC / タイムゾーン
4. [LINQ](docs/snippets/linq/linq-samples.md): 絞り込み、集計、Join、ページング
5. [HttpClient](docs/snippets/http-client/basic-http-client.md): 外部通信をテスト可能にする基本形

目的別に探す場合は [スニペット索引](docs/snippets/README.md) の学習ルートと逆引きを参照してください。

## スニペット追加時のルール

詳しい追加基準は [CONTRIBUTING.md](CONTRIBUTING.md) も参照してください。

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

namespace は、現時点では探しやすさを優先して `DotnetBackendSnippets.<Category>` を基本にします。依存境界はプロジェクトで分け、NuGet 化や大きな再利用を検討する段階で `DotnetBackendSnippets.AspNetCore.<Category>` のような階層化を移行 issue として扱います。

## 追加済みカテゴリ

- API
- Authentication / Authorization
- Async / Await
- Background Services
- Caching
- Collections
- Configuration
- Date and Time
- Data Protection
- Dependency Injection
- Deployment
- DTO Mapping
- Entity Framework Core
- Error Handling
- File Handling
- Health Checks
- HttpClient
- HttpClientFactory
- LINQ
- Logging
- Language Features
- Numbers
- Observability
- OpenAPI
- Options
- Regular Expressions
- Repository Pattern
- Rate Limiting
- Security
- Serialization
- Strings
- Type System
- Testing
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

## ライセンス

MIT License です。コード例は業務コード、社内資料、学習用途などで再利用できます。詳しくは [LICENSE](LICENSE) を確認してください。
