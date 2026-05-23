# HttpClientFactory の typed client

## 目的

`IHttpClientFactory` で typed client を登録し、bearer token、query string、失敗レスポンスの扱いをテスト可能な形にするためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/HttpClientFactory/HttpClientFactorySamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/HttpClientFactory/HttpClientFactorySamplesTests.cs`

## 使い方

`services.AddProductApiClient(baseAddress, timeout)` を呼び出すと、`ProductApiClient` を typed client として登録します。

`BearerTokenHandler` は `IAccessTokenProvider` から token を取得し、`Authorization: Bearer ...` を付けて送信します。検索 API の path は `QueryStringSamples.BuildProductSearchPath()` で組み立てます。

失敗ステータスは例外にせず、`ExternalApiResult<ProductDto>` の `Error` として返します。

## メモ

- typed client は、外部 API ごとの呼び出し処理をクラスに閉じ込められます。
- `DelegatingHandler` を使うと、認証ヘッダーなどの横断的な処理を分離できます。
- テストでは primary handler を差し替え、外部通信なしでリクエスト内容を検証します。
