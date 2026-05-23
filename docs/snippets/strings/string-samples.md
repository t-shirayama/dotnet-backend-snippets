# 文字列操作

## 目的

文字列の空白整理、URL やキーに使いやすい形への変換、表示用の省略やマスクなど、業務コードでよく使う文字列処理をまとめます。

## 実装

`src/DotnetBackendSnippets/Strings/StringSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Strings/StringSamplesTests.cs`

## 使い方

- `NormalizeWhitespace` は連続する空白や改行を1つの半角スペースにまとめます。
- `ToSlug` は英数字以外を `-` に置き換え、URL の一部に使いやすい小文字文字列を作ります。
- `Truncate` は長すぎる文字列を指定長に収め、末尾に省略記号を付けます。
- `MaskMiddle` は ID やメールアドレスの一部を隠すような表示に使えます。
- `SplitLines` は `\r\n`、`\n`、`\r` の違いを吸収して行に分割します。
- `NormalizeKey` は大文字小文字や余分な空白の違いを吸収した比較用キーを作ります。

## メモ

- slug はここでは ASCII の英数字を前提にした最小実装です。
- マスク処理は表示用の補助であり、秘密情報そのものを安全に保存する仕組みではありません。
