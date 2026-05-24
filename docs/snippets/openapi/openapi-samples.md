# OpenAPI / API 仕様の基本

## 目的

OpenAPI に出したい endpoint metadata、summary / description、response type、auth 要件、schema example、API versioning を小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/OpenApi/OpenApiSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/OpenApi/OpenApiSamplesTests.cs`

## 使い方

- `DescribeGetOrderEndpoint` は認証付き API の operationId、説明、タグ、レスポンス、example、認証要件をまとめます。
- `CreateProblemResponse` は 4xx / 5xx の `ProblemDetails` レスポンス metadata を作ります。
- `CreateBearerAuthRequirement` は Bearer token と scope を表す認証要件を作ります。
- `CreateVersionedRoute` は `/api/v1/orders` のような versioned route を作ります。

## メモ

- 実アプリではこの metadata を Minimal API の `.WithName()`、`.WithSummary()`、`.Produces<T>()`、`.RequireAuthorization()` などに対応づけます。
- OpenAPI の operationId はクライアント生成で使われるため、変更を破壊的変更として扱います。
- auth 付き API は 401 / 403 のレスポンスも明示すると、クライアント側の分岐が書きやすくなります。
- API versioning は URL、header、media type など複数方式があります。チームの互換性方針に合わせて統一します。

## 実務逆引き

- endpoint summary / description を管理したい → `OpenApiEndpointMetadata`
- response type を列挙したい → `OpenApiResponseMetadata`
- auth 付き API の OpenAPI 情報を持ちたい → `CreateBearerAuthRequirement`
- schema example を用意したい → `OpenApiSchemaExample`
- API versioning 付き route を作りたい → `CreateVersionedRoute`
