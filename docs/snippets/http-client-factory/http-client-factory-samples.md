# HttpClientFactory の typed client

## 目的

`IHttpClientFactory` で typed client を登録し、bearer token、query string、失敗レスポンス、retry / backoff、circuit breaker、correlation id 伝播をテスト可能な形にするためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/HttpClientFactory/HttpClientFactorySamples.cs`

`src/DotnetBackendSnippets.AspNetCore/HttpClientFactory/ExternalApiResilienceSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/HttpClientFactory/HttpClientFactorySamplesTests.cs`

`tests/DotnetBackendSnippets.Tests/HttpClientFactory/ExternalApiResilienceSamplesTests.cs`

## 使い方

`services.AddProductApiClient(baseAddress, timeout)` を呼び出すと、`ProductApiClient` を typed client として登録します。

`BearerTokenHandler` は `IAccessTokenProvider` から token を取得し、`Authorization: Bearer ...` を付けて送信します。検索 API の path は `QueryStringSamples.BuildProductSearchPath()` で組み立てます。

`AddProductApiClient` は `IAccessTokenProvider` 自体は登録しません。実アプリでは Key Vault、token endpoint、テストでは fake provider など、用途に合わせて別途 `IAccessTokenProvider` を登録してから呼び出します。

失敗ステータスは例外にせず、`ExternalApiResult<ProductDto>` の `Error` として返します。

耐障害性の部品として、`ClassifyFailure` は HTTP status や timeout 例外を分類します。`ExecuteWithRetryAsync` は一時的な失敗だけ retry し、`CalculateExponentialBackoffDelay` で指数 backoff を計算します。`SimpleCircuitBreaker` は連続失敗で open にし、`CorrelationIdPropagationHandler` は受信リクエストの correlation id を外部 API 呼び出しへ伝播します。

## メモ

- typed client は、外部 API ごとの呼び出し処理をクラスに閉じ込められます。
- `DelegatingHandler` を使うと、認証ヘッダーなどの横断的な処理を分離できます。
- `GetProductAsync` は `productId` が 1 未満の場合に例外を投げ、API へ不正な ID を送らないようにします。
- テストでは primary handler を差し替え、外部通信なしでリクエスト内容を検証します。
- retry は 5xx、429、timeout のような一時的な失敗に限定します。400 や 401 / 403 は呼び出し側の修正や認証更新が必要なことが多いため、闇雲に retry しません。
- circuit breaker は障害中の外部 API に呼び続けて全体を巻き込むことを避けるための考え方です。

## 実務逆引き

- `HttpClient` を DI で登録したい → `AddProductApiClient`
- `IHttpClientFactory` で typed client を作りたい → `ProductApiClient`
- bearer token を付けて呼びたい → `BearerTokenHandler`
- query string を安全に組み立てたい → `QueryStringSamples.BuildProductSearchPath`
- 失敗レスポンスを例外ではなく結果型で扱いたい → `ExternalApiResult<T>`
- timeout を設定したい → `AddProductApiClient`
- 外部 API エラーを分類したい → `ClassifyFailure`
- retry / backoff を設定したい → `ExecuteWithRetryAsync` / `CalculateExponentialBackoffDelay`
- circuit breaker を扱いたい → `SimpleCircuitBreaker`
- correlation id を伝播したい → `CorrelationIdPropagationHandler`
