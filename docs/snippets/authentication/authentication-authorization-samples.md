# Authentication / Authorization の基本

## 目的

ASP.NET Core で JWT Bearer、Cookie 認証、Policy-based Authorization、claims からの current user 取得、認可失敗の `ProblemDetails` 変換を小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Authentication/AuthenticationAuthorizationSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Authentication/AuthenticationAuthorizationSamplesTests.cs`

## 使い方

- `AddJwtBearerAuthentication` は issuer、audience、署名検証キーを指定して JWT Bearer 認証を登録します。
- `CreateJwtToken` はテストやサンプル用に署名済み JWT を発行します。
- `AddCookieAuthentication` はログインパス、アクセス拒否パス、HttpOnly cookie などを設定します。
- `AddSampleAuthorizationPolicies` は role ベースの `admin-only` と claim ベースの `tenant-member` を登録します。
- `GetRequiredUserId` は `ClaimTypes.NameIdentifier` または `sub` claim から user id を取得します。
- `CanAccessTenant` は `tenant_id` claim と対象 tenant id を比較します。
- `HasScope` / `HasPermission` は scope claim と permission claim を判定します。
- `PermissionAuthorizationHandler` は permission claim を ASP.NET Core の authorization handler として検証します。
- `CreateForbiddenProblem` は認可失敗を API レスポンスに変換しやすい `ProblemDetails` にします。

## メモ

- JWT の署名キー、issuer、audience は環境ごとに設定し、ソースコードへ秘密値を直書きしないでください。
- role は大まかな権限、claim は tenant、scope、permission のような属性や細かい権限に向いています。
- 認可ロジックは handler や helper に閉じ込め、`ClaimsPrincipal` を直接組み立てた単体テストで確認すると安全です。
- 本番ではトークン失効、clock skew、鍵ローテーション、Cookie の `Secure` / `SameSite` 設計も確認してください。

## 実務逆引き

- JWT Bearer を設定したい → `AddJwtBearerAuthentication`
- JWT を発行したい → `CreateJwtToken`
- Cookie 認証を設定したい → `AddCookieAuthentication`
- role と claim のポリシーを分けたい → `AddSampleAuthorizationPolicies`
- scope / permission を判定したい → `HasScope`, `HasPermission`
- permission 用の認可 handler をテストしたい → `PermissionAuthorizationHandler`
- current user id を取得したい → `GetRequiredUserId`
- tenant アクセス可否をテストしたい → `CanAccessTenant`
- 認可失敗を `ProblemDetails` にしたい → `CreateForbiddenProblem`
