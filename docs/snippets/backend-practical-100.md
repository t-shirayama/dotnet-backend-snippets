# バックエンド実務で毎回ググるやつ100選

## 目的

バックエンド開発中に何度も調べがちな処理を、困りごとから探せる逆引きリストとしてまとめます。

このページは、既存スニペットへの導線と、今後追加したいスニペット候補を兼ねます。実装済みの項目は該当ドキュメントへリンクし、未実装の項目は追加候補として残します。

## 文字列・数値・日付・コレクション

1. 文字列の余分な空白を1つにまとめたい → [NormalizeWhitespace](strings/string-samples.md)
2. URL に使う ASCII slug を作りたい → [ToAsciiSlug](strings/string-samples.md)
3. 日本語を残した slug を作りたい → [ToUnicodeSlug](strings/string-samples.md)
4. 長すぎる文字列を末尾省略したい → [Truncate](strings/string-samples.md)
5. ID やメールアドレスの中央を隠したい → [MaskMiddle](strings/string-samples.md)
6. 改行コードの違いを吸収して行分割したい → [SplitLines](strings/string-samples.md)
7. 比較用のキーを正規化したい → [NormalizeKey](strings/string-samples.md)
8. 金額や割合を丸めたい → [数値計算](numbers/number-samples.md)
9. 月初・月末・年齢を計算したい → [日付操作](date-and-time/date-and-time-samples.md)
10. 重複値を検出したい → [FindDuplicates / FindDuplicatesOnePass](collections/collection-samples.md)

## LINQ・列挙・非同期

11. 条件に合う値だけ抽出して並び替えたい → [GetExpensiveOrderCategories](linq/linq-samples.md)
12. グループごとに合計したい → [SumAmountByCategory](linq/linq-samples.md)
13. 上位 N 件を取りたい → [TopOrders](linq/linq-samples.md)
14. ページングしたい → [Page](linq/linq-samples.md)
15. キー重複を除いて最初の要素だけ残したい → [DistinctByKey](linq/linq-samples.md)
16. 左外部結合したい → [LeftJoinCustomerOrders](linq/linq-samples.md)
17. 入れ子のコレクションを平坦化したい → [Flatten](linq/linq-samples.md)
18. `IAsyncEnumerable<T>` をページングしたい → [PageAsync](async/async-samples.md)
19. `Task.WhenAll` で複数処理を安全に待ちたい → [WhenAllSettledAsync](async/async-samples.md)
20. キャンセル可能な非同期メソッドを書きたい → [DelayAsync / ProcessSequentiallyAsync](async/async-samples.md)

## Configuration・Options・DI

21. 設定値を必須として読みたい → [GetRequiredValue](configuration/basic-configuration.md)
22. 設定セクションを型に変換したい → [ReadAppSettings](configuration/basic-configuration.md)
23. `IOptions<T>` を登録したい → [Options パターン](options/options-samples.md)
24. `IOptionsSnapshot<T>` と `IOptionsMonitor<T>` を使い分けたい → [Options パターン](options/options-samples.md)
25. 環境変数で設定を上書きしたい → [Options パターン](options/options-samples.md)
26. user secrets をローカル開発で使いたい → 追加候補
27. DI に singleton/scoped/transient を登録したい → [Dependency Injection のサービス登録](dependency-injection/basic-dependency-injection.md)
28. keyed service を登録したい → 追加候補
29. HostedService を DI で動かしたい → [BackgroundService の worker loop](background-services/background-worker-loop.md)
30. テスト用に DI 登録を差し替えたい → 追加候補

## Logging・Observability

31. 構造化ログを出したい → [Logging の基本](logging/basic-logging.md)
32. ログ出力をテストしたい → [ListLogger](logging/basic-logging.md)
33. 例外付きログを出したい → [Observability の基本](observability/observability-samples.md)
34. `BeginScope` でリクエスト単位の情報を持たせたい → [Observability の基本](observability/observability-samples.md)
35. correlation id をログに入れたい → [Observability の基本](observability/observability-samples.md)
36. health check を追加したい → [Observability の基本](observability/observability-samples.md)
37. OpenTelemetry の trace を追加したい → 追加候補
38. メトリクスをカウンターで出したい → 追加候補
39. slow query や遅い処理をログに出したい → [Observability の基本](observability/observability-samples.md)
40. 本番ではログレベルを設定で変えたい → 追加候補

## HttpClient・外部 API

41. `HttpClient` を DI で登録したい → [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
42. `IHttpClientFactory` で typed client を作りたい → [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
43. 外部通信なしで `HttpClient` をテストしたい → [HttpClient のテスト可能な呼び出し](http-client/basic-http-client.md)
44. JSON レスポンスを読みたい → [MessageApiClient](http-client/basic-http-client.md)
45. timeout を設定したい → 追加候補
46. retry/backoff を設定したい → 追加候補
47. bearer token を付けて呼びたい → [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
48. query string を安全に組み立てたい → [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
49. `EnsureSuccessStatusCode` の失敗を扱いたい → [HttpClientFactory の基本](http-client-factory/http-client-factory-samples.md)
50. API レスポンスの null や不正 JSON を検証したい → 追加候補

## ASP.NET Core API

51. Minimal API の endpoint を定義したい → [ASP.NET Core API の基本](api/api-samples.md)
52. Controller で `ActionResult<T>` を返したい → [ASP.NET Core API の基本](api/api-samples.md)
53. `ProblemDetails` でエラー応答を統一したい → [ASP.NET Core API の基本](api/api-samples.md)
54. global exception handler を追加したい → [ASP.NET Core API の基本](api/api-samples.md)
55. request body を DTO として受けたい → [ASP.NET Core API の基本](api/api-samples.md)
56. route parameter と query parameter を受けたい → [ASP.NET Core API の基本](api/api-samples.md)
57. endpoint filter を使いたい → [ASP.NET Core API の基本](api/api-samples.md)
58. middleware を作りたい → [ASP.NET Core API の基本](api/api-samples.md)
59. CORS を設定したい → [ASP.NET Core API の基本](api/api-samples.md)
60. Swagger/OpenAPI を設定したい → 追加候補

## Validation・Error Handling

61. DTO の必須項目を検証したい → [Validation の基本](validation/basic-validation.md)
62. 複数の検証エラーをまとめて返したい → [ValidationResult](validation/basic-validation.md)
63. 例外ではなく結果型で失敗を返したい → [OperationResult](error-handling/basic-error-handling.md)
64. 正の整数だけ parse したい → [TryParsePositiveInt](error-handling/basic-error-handling.md)
65. 不正な状態なら例外を投げたい → [ThrowIfInvalidState](error-handling/basic-error-handling.md)
66. 成功・失敗を型で表したい → [Result&lt;T&gt;](type-system/type-system-samples.md)
67. 値がない可能性を `Maybe<T>` で表したい → [Maybe&lt;T&gt;](type-system/type-system-samples.md)
68. FluentValidation を DI と組み合わせたい → 追加候補
69. validation error を `ProblemDetails` に変換したい → [ASP.NET Core API の基本](api/api-samples.md)
70. domain exception を HTTP status に変換したい → [ASP.NET Core API の基本](api/api-samples.md)

## EF Core・データアクセス

71. DbContext を DI に登録したい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
72. SQLite in-memory で repository をテストしたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
73. InMemory provider と SQLite in-memory を使い分けたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
74. `AsNoTracking` を使いたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
75. pagination query を書きたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
76. `Include` と projection を使い分けたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
77. optimistic concurrency を扱いたい → 追加候補
78. transaction を使いたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)
79. migration を追加・適用したい → 追加候補
80. soft delete の query filter を書きたい → [Entity Framework Core の基本](entity-framework-core/ef-core-samples.md)

## 認証・認可・セキュリティ

81. JWT bearer authentication を設定したい → 追加候補
82. policy based authorization を設定したい → 追加候補
83. role と claim を使い分けたい → 追加候補
84. current user id を取得したい → 追加候補
85. password hash を検証したい → [セキュリティ補助](security/security-samples.md)
86. API key authentication を実装したい → [セキュリティ補助](security/security-samples.md)
87. rate limiting を設定したい → 追加候補
88. secret を設定ファイルに直書きしないようにしたい → [セキュリティ補助](security/security-samples.md)
89. input を HTML/SQL 用途に応じて扱いたい → [セキュリティ補助](security/security-samples.md)
90. file upload の拡張子・サイズ・MIME を検証したい → [ファイルアップロード検証](file-handling/file-upload-samples.md)

## テスト・CI・運用

91. xUnit で単体テストを書きたい → 既存テスト群
92. `Theory` と `InlineData` で境界値テストを書きたい → 既存テスト群
93. `HttpMessageHandler` を fake にしたい → [HttpClient のテスト可能な呼び出し](http-client/basic-http-client.md)
94. DI/Configuration をテストで組み立てたい → [Configuration](configuration/basic-configuration.md) / [Dependency Injection](dependency-injection/basic-dependency-injection.md)
95. GitHub Actions で restore/build/test/format を回したい → `/.github/workflows/dotnet.yml`
96. `dotnet format --verify-no-changes` を CI で確認したい → `/.github/workflows/dotnet.yml`
97. 警告をエラーとして扱いたい → `/Directory.Build.props`
98. nullable reference types を全体で有効化したい → `/Directory.Build.props`
99. `.editorconfig` でコードスタイルを揃えたい → `/.editorconfig`
100. 新しいスニペットの実装・テスト・ドキュメントを同時に追加したい → [README の追加ルール](../../README.md)

## メモ

- 「追加候補」は、今後 `src`、`tests`、`docs/snippets` の3点セットで増やす対象です。
- 実務でよく使う順に、Configuration、HttpClient、ASP.NET Core API、EF Core、認証認可を優先して実装していくと効果が出やすいです。
