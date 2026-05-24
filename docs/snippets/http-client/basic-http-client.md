# HttpClient のテスト可能な呼び出し

## 目的

外部 API を呼び出す処理を、実通信なしでテストできる形にするためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/HttpClient/HttpClientSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/HttpClient/HttpClientSamplesTests.cs`

## 使い方

`MessageApiClient` に `HttpClient` を渡し、`GetMessageAsync()` を呼び出します。レスポンス JSON の `message` を読み取って文字列として返します。

`GetMessageOrNullAsync` は 404 を例外ではなく未取得として扱う例です。`PostMessageAsync` は JSON request body を送り、レスポンス JSON の必須項目を検証します。

## メモ

- テストでは `FakeHttpMessageHandler` を使い、ネットワークへ接続しません。
- `EnsureSuccessStatusCode()` により、失敗ステータスは例外として扱います。
- 不正 JSON は `JsonException` のまま外へ漏らさず、呼び出し側に分かりやすい `InvalidOperationException` へ変換しています。
- 404 を業務上の「未取得」として扱うか、HTTP エラーとして扱うかはメソッド単位で分けます。

## 実務逆引き

- 外部通信なしで `HttpClient` をテストしたい → `FakeHttpMessageHandler`
- JSON レスポンスを読みたい → `MessageApiClient`
- 失敗ステータスを例外として扱いたい → `EnsureSuccessStatusCode`
- typed client と bearer token を使いたい → [HttpClientFactory の typed client](../http-client-factory/http-client-factory-samples.md)
- API レスポンスの null や不正 JSON を検証したい → `GetMessageAsync`
- 404 を null として扱いたい → `GetMessageOrNullAsync`
- JSON request body を送信したい → `PostMessageAsync`
