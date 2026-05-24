# スニペット索引

このディレクトリには、各スニペットの目的・実装・テスト・使い方をまとめています。

## 完成度ステータス

| ステータス | 意味 |
| --- | --- |
| Implemented | 実装・テスト・docs が揃っている |
| Tested | 実装・テストがあり、docs は簡易 |
| Planned | 追加予定 |
| Draft | 実験中 |

この索引でリンクしている各カテゴリは原則として Implemented です。ページ内で追加候補を扱う場合は、実装済みのメソッド名と候補を分けて記載します。

- [文字列操作の実務逆引き](strings/string-samples.md#実務逆引き)
- [数値処理の実務逆引き](numbers/number-samples.md#実務逆引き)
- [日付・時刻処理の実務逆引き](date-and-time/date-and-time-samples.md#実務逆引き)
- [LINQ 実務逆引き](linq/linq-samples.md#実務逆引き)

## 学習ルート

### 初学者向けおすすめ順

1. [文字列操作](strings/string-samples.md)
2. [数値計算](numbers/number-samples.md)
3. [日付操作](date-and-time/date-and-time-samples.md)
4. [コレクション操作](collections/collection-samples.md)
5. [LINQ 操作](linq/linq-samples.md)
6. [非同期処理](async/async-samples.md)
7. [C# 言語機能サンプル](language-features/language-feature-samples.md)

### 実務でよく使う順

1. [Configuration の必須値読み取り](configuration/basic-configuration.md)
2. [Dependency Injection のサービス登録](dependency-injection/basic-dependency-injection.md)
3. [Validation の基本](validation/basic-validation.md)
4. [Error Handling の基本](error-handling/basic-error-handling.md)
5. [Logging の基本](logging/basic-logging.md)
6. [Observability の基本](observability/observability-samples.md)

### Web API 開発セット

- [ASP.NET Core API の基本](api/api-samples.md)
- [Validation の基本](validation/basic-validation.md)
- [Options パターン](options/options-samples.md)
- [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
- [Observability の基本](observability/observability-samples.md)

### テスト技法セット

- [HttpClient のテスト可能な呼び出し](http-client/basic-http-client.md)
- [Configuration の必須値読み取り](configuration/basic-configuration.md)
- [Dependency Injection のサービス登録](dependency-injection/basic-dependency-injection.md)
- [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)

### セキュリティ注意セット

- [セキュリティ補助](security/security-samples.md)
- [ファイルアップロード検証](file-handling/file-upload-samples.md)
- [ASP.NET Core API の基本](api/api-samples.md)

### EF Core 実務セット

- [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
- [LINQ 操作](linq/linq-samples.md)
- [数値計算](numbers/number-samples.md)
- [日付操作](date-and-time/date-and-time-samples.md)

## バックエンド開発

- [ASP.NET Core API の基本](api/api-samples.md)
- [BackgroundService の worker loop](background-services/background-worker-loop.md)
- [Configuration の必須値読み取り](configuration/basic-configuration.md)
- [Dependency Injection のサービス登録](dependency-injection/basic-dependency-injection.md)
- [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
- [Logging の基本](logging/basic-logging.md)
- [Observability の基本](observability/observability-samples.md)
- [Options パターン](options/options-samples.md)
- [Validation の基本](validation/basic-validation.md)
- [HttpClient のテスト可能な呼び出し](http-client/basic-http-client.md)
- [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
- [Error Handling の基本](error-handling/basic-error-handling.md)
- [セキュリティ補助](security/security-samples.md)
- [ファイルアップロード検証](file-handling/file-upload-samples.md)
- [文字列ユーティリティ](utilities/string-utilities.md)

## C# 基礎

- [非同期処理](async/async-samples.md)
- [文字列操作](strings/string-samples.md)
- [数値計算](numbers/number-samples.md)
- [日付操作](date-and-time/date-and-time-samples.md)
- [LINQ 操作](linq/linq-samples.md)
- [コレクション操作](collections/collection-samples.md)
- [型システム](type-system/type-system-samples.md)
- [C# 言語機能サンプル](language-features/language-feature-samples.md)

## 計画・棚卸し

- [C# 言語機能の実装状況](language-features/csharp-language-feature-coverage.md)

## 逆引き

- 文字列の余分な空白を整えたい → [文字列操作: NormalizeWhitespace](strings/string-samples.md)
- 文字列処理の探し方を広く見たい → [文字列操作の実務逆引き](strings/string-samples.md#実務逆引き)
- URL 向けの文字列を作りたい → [文字列操作: ToAsciiSlug / ToUnicodeSlug](strings/string-samples.md)
- 長すぎる表示文字列を省略したい → [文字列操作: Truncate](strings/string-samples.md)
- ID やメールアドレスの一部を隠したい → [文字列操作: MaskMiddle](strings/string-samples.md)
- 数値処理の探し方を広く見たい → [数値処理の実務逆引き](numbers/number-samples.md#実務逆引き)
- 日付・時刻処理の探し方を広く見たい → [日付・時刻処理の実務逆引き](date-and-time/date-and-time-samples.md#実務逆引き)
- LINQ の探し方を広く見たい → [LINQ 実務逆引き](linq/linq-samples.md#実務逆引き)
- 非同期列挙をページングしたい → [非同期処理: PageAsync](async/async-samples.md)
- HttpClient を外部通信なしでテストしたい → [HttpClient のテスト可能な呼び出し](http-client/basic-http-client.md)
- typed HttpClient を登録したい → [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
- DI にサービスを登録したい → [Dependency Injection のサービス登録](dependency-injection/basic-dependency-injection.md)
- Options パターンを使いたい → [Options パターン](options/options-samples.md)
- 設定値を必須として読みたい → [Configuration の必須値読み取り](configuration/basic-configuration.md)
- ログ出力をテストしたい → [Logging の基本](logging/basic-logging.md)
- correlation id をログに入れたい → [Observability の基本](observability/observability-samples.md)
- 日付だけを扱いたい → [日付操作](date-and-time/date-and-time-samples.md)
- EF Core の読み取りクエリを書きたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
- ファイルアップロードを検証したい → [ファイルアップロード検証](file-handling/file-upload-samples.md)
- API や処理のエラーを値として扱いたい → [Error Handling の基本](error-handling/basic-error-handling.md)
- C# 言語機能の基本サンプルを見たい → [C# 言語機能サンプル](language-features/language-feature-samples.md)
- C# 言語機能の実装状況を確認したい → [C# 言語機能の実装状況](language-features/csharp-language-feature-coverage.md)

## 開発・運用の逆引き

- xUnit で単体テストを書きたい → `tests/DotnetBackendSnippets.Tests`
- `Theory` と `InlineData` で境界値テストを書きたい → 既存テスト群
- `HttpMessageHandler` を fake にしたい → [HttpClient のテスト可能な呼び出し](http-client/basic-http-client.md)
- DI / Configuration をテストで組み立てたい → [Configuration の必須値読み取り](configuration/basic-configuration.md) / [Dependency Injection のサービス登録](dependency-injection/basic-dependency-injection.md)
- GitHub Actions で restore / build / test / coverage を回したい → `/.github/workflows/dotnet-ci.yml`
- `dotnet format --verify-no-changes` を CI で確認したい → `/.github/workflows/format.yml`
- Markdown のリンクを CI で確認したい → `/.github/workflows/docs.yml`
- CodeQL で C# の静的解析をしたい → `/.github/workflows/codeql.yml`
- 警告をエラーとして扱いたい → `/Directory.Build.props`
- nullable reference types を全体で有効化したい → `/Directory.Build.props`
- `.editorconfig` でコードスタイルを揃えたい → `/.editorconfig`
- 新しいスニペットの実装・テスト・ドキュメントを同時に追加したい → [README の追加ルール](../../README.md) / [CONTRIBUTING](../../CONTRIBUTING.md)
