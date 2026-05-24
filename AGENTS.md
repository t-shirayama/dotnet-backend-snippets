# AGENTS.md

## 役割

このリポジトリは `dotnet-backend-snippets` です。C# / .NET のバックエンド開発で使うスニペットを、実装・テスト・説明ドキュメントのセットで管理します。

Codex は、単なるメモではなく **テストで動作確認できるスニペット集** として保守してください。利用者がそのまま実務判断の入口にできるよう、挙動・制約・注意点をコードとドキュメントの両方で明確にします。

## 作業前の確認

- `README.md`、`CONTRIBUTING.md`、`docs/snippets/README.md`、対象カテゴリの docs / tests / 実装を先に確認する。
- 既存のディレクトリ構成、命名規則、partial class の分割方針、ドキュメントの書き方に合わせる。
- 変更範囲は依頼内容に必要な最小限にする。不要な抽象化、大きなリファクタリング、無関係な整形は避ける。
- 作業中に未コミット変更を見つけた場合は、ユーザーの変更として扱い、勝手に戻さない。

## 配置ルール

新しいスニペットは、原則として以下をセットで追加します。

1. 実装コード
2. テストコード
3. 説明ドキュメント
4. 必要な索引更新

実装プロジェクトは依存関係で分けます。

- 純粋な C# / BCL のスニペット: `src/DotnetBackendSnippets.Core/<Category>/`
- ASP.NET Core / Microsoft.Extensions / HTTP / Hosting / Options / Data Protection など: `src/DotnetBackendSnippets.AspNetCore/<Category>/`
- EF Core のスニペット: `src/DotnetBackendSnippets.EntityFrameworkCore/<Category>/`
- unsafe / byref など通常コードから隔離したいスニペット: `src/DotnetBackendSnippets.UnsafeSamples/<Category>/`
- テスト: `tests/DotnetBackendSnippets.Tests/<Category>/`
- ドキュメント: `docs/snippets/<category>/`

肥大化したカテゴリは、公開クラス名を維持したまま `partial class` で用途別ファイルに分割します。利用者の呼び出し API を壊さず、検索性と保守性を上げることを優先してください。

## 実装方針

- C# の標準的な命名規則に従う。型・メンバーは PascalCase、ローカル変数と引数は camelCase。
- 非同期メソッドには `Async` サフィックスを付ける。
- `var` は型が明らかな場合に使う。
- サンプル用途でも、実務のバックエンド開発で再利用しやすい形にする。
- 例外処理、null、非同期処理、DI、設定、ログは実運用を意識する。
- 外部サービスに依存する処理は、インターフェースやテスト用実装を使って検証可能にする。
- 警告、不要な `using`、秘密情報の直書きを残さない。
- OS 依存の挙動を避ける。パス区切り、改行、ファイル名禁止文字、カルチャ、タイムゾーンは Windows / Linux / macOS の差を意識して実装・テストする。
- 現在時刻に依存する処理は、可能な限り `TimeProvider`、`DateTimeOffset`、または注入可能な時計を使う。
- HTTP ヘッダー、識別子、拡張子など、大文字小文字の扱いが実務上決まっているものは comparer やコメントで明示する。
- セキュリティ系スニペットは「これだけで本番安全」と誤解されないよう、メタデータ検証、秘密情報、暗号パラメータ、ログマスキングなどの前提を明記する。

## XML コメント

公開 API には XML ドキュメントコメントを付けます。

- public な型・メソッド・プロパティには `<summary>` を書く。
- 引数がある場合は全て `<param>` に書く。
- ジェネリック型引数がある場合は `<typeparam>` を書く。
- 戻り値がある場合は `<returns>` を書く。`void` には不要。
- 例外を意図して投げる場合は `<exception>` を書く。
- プロパティや定数は、必要に応じて `<value>` または `<summary>` で用途を明確にする。
- コメントは事実を短く書き、実装をなぞるだけの冗長な説明は避ける。

## テスト方針

- スニペットには対応するテストを追加する。
- 通常のロジックは単体テストで確認する。
- ASP.NET Core Web API は、必要に応じて結合テストを使う。
- EF Core は用途に応じて InMemory provider と SQLite in-memory を使い分ける。SQL 変換、制約、リレーション、トランザクションに関わるものは SQLite in-memory を優先する。
- HttpClient はテスト用 `HttpMessageHandler` などを使い、外部通信を避ける。
- DI / Configuration は `ServiceCollection` / `ConfigurationBuilder` で検証する。
- テスト名は、何を確認しているか分かる名前にする。複数機能をまとめすぎず、失敗時に原因を追いやすくする。
- テストクラスには `テスト対象`、共有 fixture / helper だけの partial ファイルには `テスト補助`、各 `[Fact]` / `[Theory]` の直前には `テスト意図` のコメントを置き、何を確認するテストか一目で分かるようにする。
- 正常系だけでなく、異常系、境界値、OS 非依存性、カルチャ非依存性も必要に応じて確認する。

## ドキュメント方針

日本語で簡潔に書き、専門用語には短い補足を入れてください。各スニペットの説明ドキュメントは、できるだけ次の構成にします。

```markdown
# スニペット名

## 目的
## 実装
## テスト
## 使い方
## メモ
```

- `実装` と `テスト` には対象ファイルへの相対パスを記載する。
- 使ってよい場面、使わない方がよい場面、実務上の注意点を必要に応じて書く。
- 新しいカテゴリを追加した場合は `README.md`、`docs/snippets/README.md`、カテゴリ docs の索引を確認する。
- docs に「実装済み」と書く場合は、実装・テスト・リンクが揃っていることを確認する。未実装なら Planned / TODO として扱う。

## 依存関係と CI

- パッケージバージョンは `Directory.Packages.props` を優先して管理する。
- Dependabot PR は CI が通っていることを確認してからマージする。後続 PR に包含された古い PR は、状況を確認して close する。
- Target Framework は README / CONTRIBUTING の方針と合わせる。変更する場合は docs、CI、テスト環境への影響も更新する。
- CI で確認している内容と README / docs の説明がずれないようにする。
- 脆弱性チェックや CodeQL の指摘は、サンプル用途でも軽視しない。

## 確認コマンド

変更後は、可能な範囲で次を実行します。

```bash
dotnet restore
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
dotnet format --verify-no-changes --no-restore
python scripts/check_markdown_links.py
```

パッケージを変更した場合:

```bash
dotnet list package --vulnerable --include-transitive
```

ローカルに .NET 8 ランタイムがない環境でテスト確認だけ行う場合は、一時的に `DOTNET_ROLL_FORWARD=Major` を使ってもよいです。ただし、CI は .NET 8 SDK / runtime で通ることを基準にします。

実行できない場合は、理由と代替確認内容を最終報告に書いてください。

## Hooks / Skills の考え方

- このリポジトリ固有の運用ルールは、まず `AGENTS.md` と `CONTRIBUTING.md` に集約する。
- 同じ「実装・テスト・docs セットの .NET スニペット運用」を他のリポジトリでも使う場合は、Codex skill として切り出す価値がある。
- ローカルで取りこぼしを減らす場合は `.githooks` の pre-push hook を使い、Release build / test / format / Markdown link check を push 前に確認する。
- ネットワークに依存しやすい脆弱性チェックは、ローカル hook ではなく CI 側での確認を基本にする。

## 優先順位

迷った場合は、次の順に優先します。

1. テストで動作確認できること
2. 実務で再利用しやすいこと
3. 説明が分かりやすいこと
4. 既存構成と一貫していること
5. 将来メンテナンスしやすいこと
