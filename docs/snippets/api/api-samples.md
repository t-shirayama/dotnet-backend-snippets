# ASP.NET Core API の小さな実務ヘルパー

## 目的

Minimal API や Controller でよく使う、DTO、結果型、`ProblemDetails`、validation error、route/query、CORS policy 名を小さく分けて確認できるスニペットです。

HTTP サーバを立てずに単体テストできるよう、`WebApplicationFactory` や追加 NuGet に依存しない静的メソッドとして実装しています。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Api/ApiSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Api/ApiSamplesTests.cs`

## 使い方

`ApiSamples.DescribeCreateTodoEndpoint()` は、Minimal API の `MapPost` に相当する endpoint 情報を返します。`Pattern`、`Name`、`CorsPolicyName`、`OperationId`、`Tags` をまとめておくと、ルーティング、CORS、OpenAPI のメタデータを同じ名前で管理しやすくなります。

`ApiSamples.CreateTodo(request, nextId)` は、request body 用 DTO の `CreateTodoRequest` を受け取り、成功時は `TodoResponse`、失敗時は `ValidationProblemDetails` を持つ `ApiEndpointResult<TodoResponse>` を返します。

Controller で使う場合は `ApiSamples.ToActionResult(result)` で `ActionResult<T>` に変換できます。Minimal API で使う場合は `StatusCode`、`Value`、`Problem` を見て `Results.Created` や `Results.Problem` に渡す形にできます。

`ApiSamples.ReadTodoRouteAndQuery(id, status, pageNumber, pageSize)` は、route parameter と query parameter をまとめて検証します。`id` は正の値、`status` は `open` または `done`、page は `int` overflow を避けるために `long` で `Skip` を計算してから検証します。

`ApiSamples.CreateProblem(...)` は RFC 7807 形式の `ProblemDetails` を作ります。`ApiSamples.CreateValidationProblem(errors)` はフィールドごとの validation error を `ValidationProblemDetails` に変換します。

`ApiSamples.MapDomainException(exception)` は domain exception を HTTP status 付きの `ProblemDetails` に変換します。業務ルール違反を例外で表す場合でも、API の外側では一貫したエラー応答に寄せられます。

`CorsPolicyNames` は CORS policy 名を文字列定数として集約します。`ApiSamples.IsKnownCorsPolicy(policyName)` で、typo した policy 名をテストで検出できます。

## メモ

- `ProblemDetails` は API エラー応答の標準的な形です。`Status`、`Title`、`Detail`、`Type`、`Instance` を揃えるとクライアント側で扱いやすくなります。
- validation error は例外ではなく `ValidationProblemDetails` に変換すると、複数フィールドのエラーをまとめて返せます。
- middleware や endpoint filter の実体はフレームワークに登録しますが、判定ロジックは `RequireHeader` や `ShouldShortCircuitForMaintenance` のように分けると単体テストしやすくなります。
- Swagger/OpenAPI 用の追加パッケージは使わず、`OperationId`、`Summary`、`Tags` のようなメタデータだけをサンプル化しています。
