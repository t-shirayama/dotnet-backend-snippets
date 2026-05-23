# 文字列操作

## 目的

文字列の空白整理、URL やキーに使いやすい形への変換、表示用の省略やマスクなど、業務コードでよく使う文字列処理をまとめます。

## 実装

`src/DotnetBackendSnippets.Core/Strings/StringSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Strings/StringSamplesTests.cs`

## 使い方

- `NormalizeWhitespace` は連続する空白や改行を1つの半角スペースにまとめます。
- `ToSlug` は互換用の名前で、`ToAsciiSlug` と同じ処理を行います。
- `ToAsciiSlug` はアクセント記号をできるだけ落としてから、ASCII の英数字以外を `-` に置き換えます。
- `ToUnicodeSlug` は日本語やアクセント付き文字などの Unicode 文字を残し、記号や空白を `-` に置き換えます。
- `Truncate` は長すぎる文字列を指定長に収め、末尾に省略記号を付けます。
- `MaskMiddle` は ID やメールアドレスの一部を隠すような表示に使えます。
- `SplitLines` は `\r\n`、`\n`、`\r` の違いを吸収して行に分割します。
- `NormalizeKey` は大文字小文字や余分な空白の違いを吸収した比較用キーを作ります。

## メモ

- ASCII 前提の URL や外部システム連携では `ToAsciiSlug`、日本語を含む表示向け URL では `ToUnicodeSlug` のように用途を分けます。
- マスク処理は表示用の補助であり、秘密情報そのものを安全に保存する仕組みではありません。

## 実務逆引き

バックエンド実務で毎回調べがちな文字列処理を、困りごとから探せる形でまとめます。
実装済みのものは既存スニペット名を記載し、未実装のものは「追加候補」としています。

### 正規化・空白

- 連続する空白・改行・タブを1つの半角スペースにまとめたい: 実装済み `NormalizeWhitespace`
- 先頭と末尾の空白を取り除きたい: 追加候補 `TrimOrEmpty`
- 全角スペースも含めて前後の空白を取り除きたい: 追加候補 `TrimJapaneseWhitespace`
- 改行を半角スペースに置き換えて1行にしたい: 追加候補 `NormalizeToSingleLine`
- CRLF / LF / CR を統一したい: 追加候補 `NormalizeLineEndings`
- Unicode 正規化 Form C にそろえたい: 追加候補 `NormalizeUnicode`
- アクセント記号を落として検索用文字列を作りたい: 追加候補 `RemoveDiacriticsForSearch`
- 連続するハイフンやアンダースコアを1つにまとめたい: 追加候補 `CollapseSeparators`
- null と空白だけの文字列を同じ扱いにしたい: 追加候補 `NullIfWhiteSpace`
- 表示用に余分な空白だけ整えて意味は変えたくない: 実装済み `NormalizeWhitespace`

### Slug・URL

- URL 向けに ASCII slug を作りたい: 実装済み `ToAsciiSlug`
- 日本語を残した Unicode slug を作りたい: 実装済み `ToUnicodeSlug`
- 互換用の slug メソッドを使いたい: 実装済み `ToSlug`
- slug が空になったときの fallback を入れたい: 追加候補 `ToSlugOrDefault`
- slug の最大長を制限したい: 追加候補 `TruncateSlug`
- URL path segment を安全にエンコードしたい: 追加候補 `EncodePathSegment`
- query string の値をエンコードしたい: 追加候補 `EncodeQueryValue`
- ファイル名から URL 向け slug を作りたい: 追加候補 `FileNameToSlug`
- slug の重複を避けるため連番を付けたい: 追加候補 `AppendSlugSuffix`
- 外部公開 URL と管理画面 URL で slug 方針を分けたい: 実装済み `ToAsciiSlug` / `ToUnicodeSlug`

### マスク・表示

- ID やトークンの中央だけ隠したい: 実装済み `MaskMiddle`
- メールアドレスのローカル部を一部だけ隠したい: 追加候補 `MaskEmail`
- 電話番号の下4桁だけ残したい: 追加候補 `MaskPhoneNumber`
- クレジットカード番号の末尾4桁だけ残したい: 追加候補 `MaskCardNumber`
- 長い文字列を指定文字数で省略したい: 実装済み `Truncate`
- サロゲートペアや絵文字を壊さず省略したい: 追加候補 `TruncateTextElements`
- ログ出力前に secret らしい値を伏せたい: 追加候補 `RedactSecrets`
- JSON の一部フィールドだけマスクしたい: 追加候補 `MaskJsonFields`
- 表示幅を考慮して省略したい: 追加候補 `TruncateByDisplayWidth`
- 空文字のときだけ代替表示にしたい: 追加候補 `DefaultIfEmpty`

### 分割・抽出

- OS 差を吸収して行単位に分割したい: 実装済み `SplitLines`
- 空行を除外して行単位に分割したい: 実装済み `SplitLines`
- カンマ区切りを空白 trim 付きで分割したい: 追加候補 `SplitCsvLikeValues`
- key=value 形式を辞書にしたい: 追加候補 `ParseKeyValueLines`
- 先頭の区切り文字より前だけ取りたい: 追加候補 `Before`
- 先頭の区切り文字より後だけ取りたい: 追加候補 `After`
- 最後の区切り文字より前だけ取りたい: 追加候補 `BeforeLast`
- 最後の区切り文字より後だけ取りたい: 追加候補 `AfterLast`
- 文字列から数字だけ抽出したい: 追加候補 `ExtractDigits`
- ログ行から correlation id を抜き出したい: 追加候補 `ExtractCorrelationId`

### 検索・比較

- 大文字小文字と余分な空白を無視する比較キーを作りたい: 実装済み `NormalizeKey`
- 大文字小文字を無視して含まれるか確認したい: 追加候補 `ContainsIgnoreCase`
- 前方一致を大小文字無視で判定したい: 追加候補 `StartsWithIgnoreCase`
- 後方一致を大小文字無視で判定したい: 追加候補 `EndsWithIgnoreCase`
- 文化依存しない安全な比較をしたい: 追加候補 `EqualsOrdinalIgnoreCase`
- 日本語検索向けにひらがな・カタカナ差を吸収したい: 追加候補 `NormalizeKanaForSearch`
- 複数キーワードをすべて含むか判定したい: 追加候補 `ContainsAllKeywords`
- 複数キーワードのどれかを含むか判定したい: 追加候補 `ContainsAnyKeyword`
- 単語境界を考慮して検索したい: 追加候補 `ContainsWholeWord`
- ユーザー入力の検索語を正規表現として安全に扱いたい: 追加候補 `EscapeRegexPattern`

### エンコード・エスケープ

- HTML 表示前にエスケープしたい: 追加候補 `HtmlEncode`
- JavaScript 文字列として安全に埋め込みたい: 追加候補 `JavaScriptStringEncode`
- URL query 用に値をエンコードしたい: 追加候補 `UrlEncode`
- URL query をデコードしたい: 追加候補 `UrlDecode`
- Base64 に変換したい: 追加候補 `ToBase64`
- Base64Url に変換したい: 追加候補 `ToBase64Url`
- Base64Url から戻したい: 追加候補 `FromBase64Url`
- 正規表現のメタ文字をエスケープしたい: 追加候補 `RegexEscape`
- SQL LIKE の `%` や `_` をエスケープしたい: 追加候補 `EscapeSqlLikePattern`
- ログ出力用に制御文字を可視化したい: 追加候補 `EscapeControlCharacters`

### CSV・ログ・テキスト出力

- CSV の1フィールドを安全にクォートしたい: 追加候補 `EscapeCsvField`
- CSV 1行を作りたい: 追加候補 `JoinCsvRow`
- TSV 1行を作りたい: 追加候補 `JoinTsvRow`
- ログ用に改行を `\n` 表記へ逃がしたい: 追加候補 `SanitizeForSingleLineLog`
- ログに出す最大文字数を制限したい: 実装済み `Truncate`
- ログ用に個人情報っぽい値をマスクしたい: 追加候補 `RedactPersonalData`
- 複数行メッセージにインデントを付けたい: 追加候補 `IndentLines`
- 箇条書き用に prefix を付けたい: 追加候補 `PrefixLines`
- 固定幅の表を作るために右詰めしたい: 追加候補 `PadLeftSafe`
- 固定幅の表を作るために左詰めしたい: 追加候補 `PadRightSafe`

### キー生成・識別子

- 設定キー比較用に表記ゆれを吸収したい: 実装済み `NormalizeKey`
- ユーザー入力から辞書キーを作りたい: 実装済み `NormalizeKey`
- ランダムな短いコードを作りたい: 追加候補 `CreateRandomCode`
- 数字だけのワンタイムコードを作りたい: 追加候補 `CreateNumericCode`
- URL 安全なランダムトークンを作りたい: 追加候補 `CreateUrlSafeToken`
- GUID を短い文字列表現にしたい: 追加候補 `ToShortGuid`
- ファイル保存用の安全な名前を作りたい: 追加候補 `ToSafeFileName`
- S3 や Blob Storage 用の object key を作りたい: 追加候補 `BuildObjectKey`
- Redis key を一貫した形式で作りたい: 追加候補 `BuildCacheKey`
- メッセージ重複排除用の安定ハッシュキーを作りたい: 追加候補 `CreateStableHashKey`

### 検証・判定

- null / empty / whitespace をまとめて判定したい: 追加候補 `IsBlank`
- メールアドレスらしい形式か軽く判定したい: 追加候補 `IsEmailLike`
- URL として妥当か判定したい: 追加候補 `IsAbsoluteUrl`
- HTTPS URL だけ許可したい: 追加候補 `IsHttpsUrl`
- UUID / GUID 形式か判定したい: 追加候補 `IsGuid`
- 数字だけか判定したい: 追加候補 `IsDigitsOnly`
- 英数字とハイフンだけ許可したい: 追加候補 `IsAsciiSlug`
- ファイル拡張子が許可リスト内か判定したい: 追加候補 `HasAllowedExtension`
- 禁止語を含むか判定したい: 追加候補 `ContainsBlockedWord`
- 最大長を超えた入力をエラーにしたい: 追加候補 `ValidateMaxLength`

### 変換・テンプレート

- snake_case を PascalCase に変換したい: 追加候補 `SnakeToPascalCase`
- PascalCase を camelCase に変換したい: 追加候補 `PascalToCamelCase`
- PascalCase を kebab-case に変換したい: 追加候補 `PascalToKebabCase`
- 簡単なテンプレート文字列に値を埋め込みたい: 追加候補 `RenderTemplate`
- `{name}` 形式の placeholder を抽出したい: 追加候補 `ExtractPlaceholders`
- 複数形の簡易表示を作りたい: 追加候補 `PluralizeSimple`
- バイト数表示を人間向けに整えたい: 追加候補 `FormatBytes`
- 文字列を UTF-8 byte 数で制限したい: 追加候補 `TruncateUtf8Bytes`
- 環境変数名向けに大文字スネークケースへ変換したい: 追加候補 `ToEnvironmentVariableName`
- 画面表示用と永続化用で文字列変換を分けたい: 実装済み `NormalizeWhitespace` / `NormalizeKey`
