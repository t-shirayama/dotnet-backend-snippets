# Regular Expressions の基本

## 目的

正規表現で入力検証、抽出、置換、リテラル検索、timeout を扱うためのスニペットです。

## 実装

`src/DotnetBackendSnippets.Core/RegularExpressions/RegularExpressionSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/RegularExpressions/RegularExpressionSamplesTests.cs`

## 使い方

- `IsProductCode` は `GeneratedRegex` でコンパイル済みの検証 regex を使います。
- `ExtractHashtags` は named group で hashtag を抽出します。
- `NormalizeWhitespace` は連続空白を 1 つに置換します。
- `CreateLiteralSearchRegex` はユーザー入力を `Regex.Escape` してリテラル検索にします。
- `IsMatchWithTimeout` は timeout 付きで照合し、`RegexMatchTimeoutException` を失敗として扱います。

## メモ

- ユーザー入力を regex pattern として直接連結しないでください。検索語として使う場合は `Regex.Escape` します。
- 外部入力に対する regex は timeout を設定し、ReDoS のリスクを下げます。
- 高頻度に使う固定 pattern は `GeneratedRegex` で生成しておくと、初期化や実行の意図が明確になります。
- 日本語や絵文字を含む単語境界は `\b` だけでは期待通りにならないことがあります。

## 実務逆引き

- 入力形式を regex で検証したい → `IsProductCode`
- 文字列から token を抽出したい → `ExtractHashtags`
- regex で置換したい → `NormalizeWhitespace`
- ユーザー入力を安全に検索したい → `CreateLiteralSearchRegex`
- regex timeout を設定したい → `IsMatchWithTimeout`
