# ファイルアップロード検証

## 目的

アップロードファイルのファイル名、サイズ、拡張子、Content-Type を、ASP.NET Core の型に依存しないメタデータとして検証します。

## 実装

`src/DotnetBackendSnippets.Core/FileHandling/FileUploadSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/FileHandling/FileUploadSamplesTests.cs`

## 使い方

- `UploadedFileMetadata` にファイル名、バイト数、Content-Type を入れます。
- `FileUploadRules` に最大サイズ、許可する拡張子、許可する Content-Type を設定します。
- `ValidateUpload` は検証結果とエラーメッセージ一覧を返します。
- `NormalizeExtension` は `.JPG` や `jpg` のような表記揺れを `.jpg` に揃えます。

## メモ

- Content-Type はクライアントから送られる値なので、単独では信用しません。早期 reject の補助として使い、重要な用途ではファイルの中身や保存後のスキャンも検討します。
- 拡張子だけで安全性は保証できません。保存先の分離、ランダムな保存名、実行権限のないディレクトリ、ウイルススキャンなどと組み合わせます。
- サイズ上限はアプリ側だけでなく、Web サーバーや reverse proxy 側にも設定します。
