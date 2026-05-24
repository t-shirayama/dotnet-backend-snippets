# net10.0 と EF Core 10 への移行

## 決定

このリポジトリでは Target Framework を `net10.0` に更新し、EF Core サンプルでは `Microsoft.EntityFrameworkCore` 10 系を採用します。

## 理由

- .NET 10 は LTS として、今後のスニペット利用者が再現しやすい基準になります。
- .NET 8 の EOL が近づいているため、学習用スニペットの標準環境を早めに移行します。
- EF Core 10 は `net10.0` 向けの stable release として提供されており、SQLite in-memory を含む既存テストで互換性を確認します。
- ASP.NET Core 依存パッケージも `net10.0` 向けに揃え、CI の SDK と Target Framework の説明を一致させます。

## 運用

- CI は .NET SDK `10.0.x` を使います。
- NuGet の patch / minor 更新は Dependabot と CI 結果を確認して取り込みます。
- .NET 11 / EF Core 11 などの次期 major 更新は、移行 PR として扱います。
- 方針が変わる場合は、この decision、README、CONTRIBUTING、CI の SDK 指定を同じ変更で更新します。
