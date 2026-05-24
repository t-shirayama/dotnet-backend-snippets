# Data Protection / 秘密情報管理の基本

## 目的

ASP.NET Core Data Protection、secret provider 抽象化、必須 secret 読み取りを小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Security/DataProtectionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Security/DataProtectionSamplesTests.cs`

## 使い方

- `CreateProtector` は Data Protection provider と purpose から `IDataProtector` を作ります。
- `ProtectString` / `UnprotectString` は token などの短い文字列を保護・復元します。
- `ISecretProvider` は Key Vault、Secrets Manager、環境変数などの secret source を差し替える境界です。
- `GetRequiredSecretAsync` は secret 未設定を起動時に検出しやすい形で例外にします。

## メモ

- Data Protection の key ring は複数台で共有できる保存先に置き、環境ごとに分けます。
- purpose は用途ごとに分けます。別 purpose で保護した値は復元できないため、誤用を防げます。
- user-secrets はローカル開発用です。本番では Key Vault / Secrets Manager / orchestrator の secret 機構を使います。
- connection string や token をログに出す場合は必ずマスクします。

## 実務逆引き

- Data Protection で値を保護したい → `ProtectString`
- purpose を分けたい → `CreateProtector`
- secret store を抽象化したい → `ISecretProvider`
- 必須 secret を読みたい → `GetRequiredSecretAsync`
