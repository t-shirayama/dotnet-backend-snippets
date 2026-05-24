# 文字列操作

## 目的

文字列の空白整理、URL やキーに使いやすい形への変換、表示用の省略やマスクなど、業務コードでよく使う文字列処理をまとめます。

## 実装

- `src/DotnetBackendSnippets.Core/Strings/StringSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringInspectionSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringExtractionSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringNormalizationSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringSplitJoinSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringValidationSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringConversionSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringEscapingSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringReplacementSamples.cs`
- `src/DotnetBackendSnippets.Core/Strings/StringReverseLookupSamples.Helpers.cs`

## テスト

- `tests/DotnetBackendSnippets.Tests/Strings/StringSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/Strings/String*SamplesTests.cs`

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
- 逆引き用の `StringReverseLookupSamples` は partial class として分類別ファイルに分割しています。

## 実務逆引き

バックエンド実務で毎回調べがちな文字列処理を、困りごとから探せる形でまとめます。
実装済みのものは既存スニペット名を記載し、逆引き項目は実装済みメソッド名を記載しています。

### 正規化・空白

- 連続する空白・改行・タブを1つの半角スペースにまとめたい: 実装済み `NormalizeWhitespace`
- 先頭と末尾の空白を取り除きたい: 実装済み `TrimOrEmpty`
- 全角スペースも含めて前後の空白を取り除きたい: 実装済み `TrimJapaneseWhitespace`
- 改行を半角スペースに置き換えて1行にしたい: 実装済み `NormalizeToSingleLine`
- CRLF / LF / CR を統一したい: 実装済み `NormalizeLineEndings`
- Unicode 正規化 Form C にそろえたい: 実装済み `NormalizeUnicode`
- アクセント記号を落として検索用文字列を作りたい: 実装済み `RemoveDiacriticsForSearch`
- 連続するハイフンやアンダースコアを1つにまとめたい: 実装済み `CollapseSeparators`
- null と空白だけの文字列を同じ扱いにしたい: 実装済み `NullIfWhiteSpace`
- 表示用に余分な空白だけ整えて意味は変えたくない: 実装済み `NormalizeWhitespace`

### Slug・URL

- URL 向けに ASCII slug を作りたい: 実装済み `ToAsciiSlug`
- 日本語を残した Unicode slug を作りたい: 実装済み `ToUnicodeSlug`
- 互換用の slug メソッドを使いたい: 実装済み `ToSlug`
- slug が空になったときの fallback を入れたい: 実装済み `ToSlugOrDefault`
- slug の最大長を制限したい: 実装済み `TruncateSlug`
- URL path segment を安全にエンコードしたい: 実装済み `EncodePathSegment`
- query string の値をエンコードしたい: 実装済み `EncodeQueryValue`
- ファイル名から URL 向け slug を作りたい: 実装済み `FileNameToSlug`
- slug の重複を避けるため連番を付けたい: 実装済み `AppendSlugSuffix`
- 外部公開 URL と管理画面 URL で slug 方針を分けたい: 実装済み `ToAsciiSlug` / `ToUnicodeSlug`

### マスク・表示

- ID やトークンの中央だけ隠したい: 実装済み `MaskMiddle`
- メールアドレスのローカル部を一部だけ隠したい: 実装済み `MaskEmail`
- 電話番号の下4桁だけ残したい: 実装済み `MaskPhoneNumber`
- クレジットカード番号の末尾4桁だけ残したい: 実装済み `MaskCardNumber`
- 長い文字列を指定文字数で省略したい: 実装済み `Truncate`
- サロゲートペアや絵文字を壊さず省略したい: 実装済み `TruncateTextElements`
- ログ出力前に secret らしい値を伏せたい: 実装済み `RedactSecrets`
- JSON の一部フィールドだけマスクしたい: 実装済み `MaskJsonFields`
- 表示幅を考慮して省略したい: 実装済み `TruncateByDisplayWidth`
- 空文字のときだけ代替表示にしたい: 実装済み `DefaultIfEmpty`

### 分割・抽出

- OS 差を吸収して行単位に分割したい: 実装済み `SplitLines`
- 空行を除外して行単位に分割したい: 実装済み `SplitLines`
- カンマ区切りを空白 trim 付きで分割したい: 実装済み `SplitCsvLikeValues`
- key=value 形式を辞書にしたい: 実装済み `ParseKeyValueLines`
- 先頭の区切り文字より前だけ取りたい: 実装済み `Before`
- 先頭の区切り文字より後だけ取りたい: 実装済み `After`
- 最後の区切り文字より前だけ取りたい: 実装済み `BeforeLast`
- 最後の区切り文字より後だけ取りたい: 実装済み `AfterLast`
- 文字列から数字だけ抽出したい: 実装済み `ExtractDigits`
- ログ行から correlation id を抜き出したい: 実装済み `ExtractCorrelationId`

### 検索・比較

- 大文字小文字と余分な空白を無視する比較キーを作りたい: 実装済み `NormalizeKey`
- 大文字小文字を無視して含まれるか確認したい: 実装済み `ContainsIgnoreCase`
- 前方一致を大小文字無視で判定したい: 実装済み `StartsWithIgnoreCase`
- 後方一致を大小文字無視で判定したい: 実装済み `EndsWithIgnoreCase`
- 文化依存しない安全な比較をしたい: 実装済み `EqualsOrdinalIgnoreCase`
- 日本語検索向けにひらがな・カタカナ差を吸収したい: 実装済み `NormalizeKanaForSearch`
- 複数キーワードをすべて含むか判定したい: 実装済み `ContainsAllKeywords`。空のキーワード一覧は LINQ の `All` と同じく `true` です。
- 複数キーワードのどれかを含むか判定したい: 実装済み `ContainsAnyKeyword`
- 単語境界を考慮して検索したい: 実装済み `ContainsWholeWord`。英数字ベースの単語境界向けです。
- ユーザー入力の検索語を正規表現として安全に扱いたい: 実装済み `EscapeRegexPattern`

### エンコード・エスケープ

- HTML 表示前にエスケープしたい: 実装済み `HtmlEncode`
- JavaScript 文字列として安全に埋め込みたい: 実装済み `JavaScriptStringEncode`
- URL query 用に値をエンコードしたい: 実装済み `UrlEncode`
- URL query をデコードしたい: 実装済み `UrlDecode`
- Base64 に変換したい: 実装済み `ToBase64`
- Base64Url に変換したい: 実装済み `ToBase64Url`
- Base64Url から戻したい: 実装済み `FromBase64Url`
- 正規表現のメタ文字をエスケープしたい: 実装済み `RegexEscape`
- SQL LIKE の `%` や `_` をエスケープしたい: 実装済み `EscapeSqlLikePattern`
- ログ出力用に制御文字を可視化したい: 実装済み `EscapeControlCharacters`

### CSV・ログ・テキスト出力

- CSV の1フィールドを安全にクォートしたい: 実装済み `EscapeCsvField`
- CSV 1行を作りたい: 実装済み `JoinCsvRow`
- TSV 1行を作りたい: 実装済み `JoinTsvRow`
- ログ用に改行を `\n` 表記へ逃がしたい: 実装済み `SanitizeForSingleLineLog`
- ログに出す最大文字数を制限したい: 実装済み `Truncate`
- ログ用に個人情報っぽい値をマスクしたい: 実装済み `RedactPersonalData`
- 複数行メッセージにインデントを付けたい: 実装済み `IndentLines`
- 箇条書き用に prefix を付けたい: 実装済み `PrefixLines`
- 固定幅の表を作るために右詰めしたい: 実装済み `PadLeftSafe`
- 固定幅の表を作るために左詰めしたい: 実装済み `PadRightSafe`

### キー生成・識別子

- 設定キー比較用に表記ゆれを吸収したい: 実装済み `NormalizeKey`
- ユーザー入力から辞書キーを作りたい: 実装済み `NormalizeKey`
- ランダムな短いコードを作りたい: 実装済み `CreateRandomCode`
- 数字だけのワンタイムコードを作りたい: 実装済み `CreateNumericCode`
- URL 安全なランダムトークンを作りたい: 実装済み `CreateUrlSafeToken`
- GUID を短い文字列表現にしたい: 実装済み `ToShortGuid`
- ファイル保存用の安全な名前を作りたい: 実装済み `ToSafeFileName`
- S3 や Blob Storage 用の object key を作りたい: 実装済み `BuildObjectKey`。空や記号だけの segment は例外にします。
- Redis key を一貫した形式で作りたい: 実装済み `BuildCacheKey`
- メッセージ重複排除用の安定ハッシュキーを作りたい: 実装済み `CreateStableHashKey`

### 検証・判定

- null / empty / whitespace をまとめて判定したい: 実装済み `IsBlank`
- メールアドレスらしい形式か軽く判定したい: 実装済み `IsEmailLike`
- URL として妥当か判定したい: 実装済み `IsAbsoluteUrl`
- HTTPS URL だけ許可したい: 実装済み `IsHttpsUrl`
- UUID / GUID 形式か判定したい: 実装済み `IsGuid`
- 数字だけか判定したい: 実装済み `IsDigitsOnly`
- 英数字とハイフンだけ許可したい: 実装済み `IsAsciiSlug`
- ファイル拡張子が許可リスト内か判定したい: 実装済み `HasAllowedExtension`
- 禁止語を含むか判定したい: 実装済み `ContainsBlockedWord`
- 最大長を超えた入力をエラーにしたい: 実装済み `ValidateMaxLength`

### 変換・テンプレート

- snake_case を PascalCase に変換したい: 実装済み `SnakeToPascalCase`
- PascalCase を camelCase に変換したい: 実装済み `PascalToCamelCase`
- PascalCase を kebab-case に変換したい: 実装済み `PascalToKebabCase`
- 簡単なテンプレート文字列に値を埋め込みたい: 実装済み `RenderTemplate`
- `{name}` 形式の placeholder を抽出したい: 実装済み `ExtractPlaceholders`
- 複数形の簡易表示を作りたい: 実装済み `PluralizeSimple`
- バイト数表示を人間向けに整えたい: 実装済み `FormatBytes`
- 文字列を UTF-8 byte 数で制限したい: 実装済み `TruncateUtf8Bytes`
- 環境変数名向けに大文字スネークケースへ変換したい: 実装済み `ToEnvironmentVariableName`
- 画面表示用と永続化用で文字列変換を分けたい: 実装済み `NormalizeWhitespace` / `NormalizeKey`
