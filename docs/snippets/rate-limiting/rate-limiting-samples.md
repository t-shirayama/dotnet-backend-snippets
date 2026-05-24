# Rate Limiting の基本

## 目的

IP、ユーザー、エンドポイント単位の rate limit key、fixed window 判定、429 `ProblemDetails` をテスト可能な形で扱うためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/RateLimiting/RateLimitingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/RateLimiting/RateLimitingSamplesTests.cs`

## 使い方

- `FixedWindowRateLimiter` は指定 window 内のリクエスト回数を数えて許可・拒否を判定します。
- `ResolveRateLimitKey` は IP、user、endpoint のどれで制限するかを切り替えます。
- `CreateTooManyRequestsProblem` は 429 response 用の `ProblemDetails` を作ります。

## メモ

- サンプルの limiter はテストしやすいインメモリ実装です。複数台構成では ASP.NET Core rate limiting middleware、Redis などの共有ストア、API gateway の制限を検討します。
- 未認証ユーザーは user 単位で制限できないため、IP や anonymous key へフォールバックします。
- 429 では `Retry-After` ヘッダーや retry 秒数を返すとクライアントが再試行しやすくなります。

## 実務逆引き

- IP 単位で制限したい → `RateLimitKeyMode.IpAddress`
- user 単位で制限したい → `RateLimitKeyMode.User`
- endpoint 別 policy を作りたい → `RateLimitKeyMode.Endpoint`
- 429 response を作りたい → `CreateTooManyRequestsProblem`
- rate limit 判定を単体テストしたい → `FixedWindowRateLimiter`
