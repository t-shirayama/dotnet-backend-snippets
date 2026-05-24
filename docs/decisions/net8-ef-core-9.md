# net8.0 と EF Core 9 の併用

## 決定

このリポジトリでは Target Framework を `net8.0` のまま維持しつつ、EF Core サンプルでは `Microsoft.EntityFrameworkCore` 9 系を採用します。

## 理由

- `net8.0` は LTS として、スニペット利用者が再現しやすい基準にします。
- EF Core 9 は `net8.0` から利用でき、SQLite in-memory を含む既存テストで動作確認しています。
- EF Core の実務サンプルでは、最新の stable minor / patch を使ってクエリ、concurrency、migration 補助、repository pattern を検証します。

## 運用

- .NET SDK / Target Framework のメジャー更新は別作業として扱います。
- EF Core 10 など、対象 TFM や挙動に影響する更新は CI と代表サンプルを確認してから取り込みます。
- 方針が変わる場合は、この decision と README のバージョン管理方針を同時に更新します。
