# CONTRIBUTING

このリポジトリは、C# / .NET バックエンド開発で使うスニペットを、実装・テスト・説明ドキュメントのセットで管理します。

## スニペット追加基準

### 1スニペットの目安

- 1つの困りごとに絞る。
- 実装は小さく保つ。
- 実務上の注意点を最低1つ書く。
- 正常系、異常系、境界値のテストを入れる。
- 外部サービスや現在時刻に依存する処理は、差し替え可能な形にする。
- OS 依存のパス、改行、カルチャ、タイムゾーン挙動を暗黙に使わない。

### ドキュメントに必ず書くこと

- 何を解決するか。
- 使ってよい場面。
- 使わない方がよい場面。
- 対応する実装ファイルとテストファイル。
- セキュリティ、性能、運用上の注意点。

### テスト観点

- 正常系: 代表的な入力で期待結果を返す。
- 異常系: null、不正値、範囲外、許可されない値を確認する。
- 境界値: 空、0、最大長、最小長、区切り直前直後などを確認する。
- 実務差分: OS 非依存、カルチャ非依存、時刻非依存が必要な箇所は明示的に確認する。

### 粒度の目安

- 小さいカテゴリは1ファイルにまとめる。
- 1ファイルが大きくなったら、用途別の partial class に分割する。
- 分割しても公開クラス名とメソッド名はむやみに変えない。
- 便利メソッドを増やすより、困りごとから探せる名前と docs を優先する。

## レビュー前チェック

```bash
dotnet restore
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
dotnet format --verify-no-changes --no-restore
python scripts/check_markdown_links.py
```

ローカルに .NET 8 runtime がない環境では、通常の `dotnet test --no-build --configuration Release` が実行できない場合があります。その場合も CI は `.github/workflows/dotnet-ci.yml` で .NET SDK 8.0.x をセットアップして確認します。
