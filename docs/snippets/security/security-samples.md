# セキュリティ補助

## 目的

パスワードハッシュ検証、API key 比較、secret らしき設定値の検出、HTML 表示用エンコード、SQL 識別子のホワイトリスト検証など、バックエンドで毎回確認しがちな安全寄りの小処理をまとめます。

## 実装

`src/DotnetBackendSnippets.Core/Security/SecuritySamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Security/SecuritySamplesTests.cs`

## 使い方

- `HashPassword` は PBKDF2-SHA256 で salt 付きハッシュ文字列を作ります。追加 NuGet は使いません。既定反復回数はサンプル値なので、実運用では環境性能と年次の推奨に合わせて見直します。
- `VerifyPassword` は保存済みハッシュと入力パスワードを検証します。不正なハッシュ形式は例外ではなく `false` として扱います。
- `AreApiKeysEqual` は API key を SHA-256 で固定長にしてから定数時間比較します。
- `FindPotentialSecrets` は `ApiKey`、`Password`、`Token` などのキー名や、長いランダム値に見える設定値を検出します。
- `HtmlEncodeForDisplay` はユーザー入力を HTML に表示する直前にエンコードします。
- `QuoteSqlIdentifier` はテーブル名や列名のような SQL 識別子を単純な許可リストで検証して `[]` で囲みます。

## メモ

- パスワードは復号できる形で保存せず、salt 付きの一方向ハッシュとして保存します。
- PBKDF2 の反復回数は固定の正解ではありません。ログイン遅延、サーバー性能、利用フレームワークの推奨値を見ながら、移行できる保存形式で定期的に引き上げます。
- API key の検証では、通常は保存側にも平文ではなくハッシュを持たせます。このサンプルは比較部分だけを小さく切り出しています。
- secret 検出は静的チェックの補助です。誤検知や検知漏れはあるため、CI の secret scanning やレビューと併用します。
- SQL の値は文字列連結せず、必ず ADO.NET や ORM のパラメータを使います。`QuoteSqlIdentifier` は値ではなく、どうしても動的に選ぶ必要がある列名などの識別子向けです。

## 実務逆引き

- password hash を検証したい → `HashPassword` / `VerifyPassword`
- API key を定数時間で比較したい → `AreApiKeysEqual`
- secret を設定ファイルに直書きしていないか見たい → `FindPotentialSecrets`
- HTML 表示用に入力をエンコードしたい → `HtmlEncodeForDisplay`
- SQL 識別子をホワイトリスト検証したい → `QuoteSqlIdentifier`
- JWT bearer authentication を設定したい → 追加候補
- policy based authorization を設定したい → 追加候補
- role と claim を使い分けたい → 追加候補
- current user id を取得したい → 追加候補
- rate limiting を設定したい → 追加候補
