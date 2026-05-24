# Regular Expressions の基本

## 目的

正規表現で入力検証、抽出、置換、リテラル検索、timeout、基本 pattern 管理を扱うためのスニペットです。

## 実装

- `src/DotnetBackendSnippets.Core/RegularExpressions/RegularExpressionSamples.cs`
- `src/DotnetBackendSnippets.Core/RegularExpressions/RegularExpressionPatternSamples.cs`

## テスト

- `tests/DotnetBackendSnippets.Tests/RegularExpressions/RegularExpressionSamplesTests.cs`
- `tests/DotnetBackendSnippets.Tests/RegularExpressions/RegularExpressionPatternSamplesTests.cs`

## 使い方

- `IsProductCode` は `GeneratedRegex` でコンパイル済みの検証 regex を使います。
- `ExtractHashtags` は named group で hashtag を抽出します。
- `NormalizeWhitespace` は連続空白を 1 つに置換します。
- `CreateLiteralSearchRegex` はユーザー入力を `Regex.Escape` してリテラル検索にします。
- `IsMatchWithTimeout` は timeout 付きで照合し、`RegexMatchTimeoutException` を失敗として扱います。
- `TryParseLogEntry` は named group で timestamp、level、correlation id、message を抽出します。
- `RedactEmailUserNames` は `MatchEvaluator` でメールアドレスの user 名部分だけをマスクします。
- `SplitTokens` は comma、semicolon、pipe、空白など複数の区切り文字を扱います。
- `IsSafeAsciiIdentifier` は `RegexOptions.NonBacktracking` を使える単純な検証 pattern の例です。
- `ContainsOnlyUnicodeLettersNumbersAndHyphen` は日本語など Unicode 文字種を含む許可文字チェックです。
- `GetRegisteredPattern` は用途別に固定 pattern を選ぶ registry 的な例です。

## メモ

- 正規表現は文字列の形を確認するには便利ですが、メールや URL の RFC 完全検証を regex だけで行うのは避けます。実務では `MailAddress`、`Uri`、ドメイン固有の validation と組み合わせます。
- ユーザー入力を regex pattern として直接連結しないでください。検索語として使う場合は `Regex.Escape` します。
- 外部入力に対する regex は timeout を設定し、ReDoS のリスクを下げます。
- 高頻度に使う固定 pattern は `GeneratedRegex` で生成しておくと、初期化や実行の意図が明確になります。
- .NET 8 では、条件に合う単純な検証 pattern に `RegexOptions.NonBacktracking` を使うと、バックトラッキング由来の性能リスクを下げやすくなります。
- 日本語や絵文字を含む単語境界は `\b` だけでは期待通りにならないことがあります。

## 実務逆引き

- 入力形式を regex で検証したい → `IsProductCode`
- 文字列から token を抽出したい → `ExtractHashtags`
- ログ行から named group で値を取り出したい → `TryParseLogEntry`
- regex で置換したい → `NormalizeWhitespace`
- match 内容を使って置換したい → `RedactEmailUserNames`
- 複数の区切り文字で分割したい → `SplitTokens`
- ユーザー入力を安全に検索したい → `CreateLiteralSearchRegex`
- regex timeout を設定したい → `IsMatchWithTimeout`
- NonBacktracking で検証したい → `IsSafeAsciiIdentifier`
- Unicode 文字種を許可文字にしたい → `ContainsOnlyUnicodeLettersNumbersAndHyphen`
- 用途別の固定 pattern を管理したい → `GetRegisteredPattern`
