# ファイルアップロード検証

## 目的

アップロードファイルのファイル名、サイズ、拡張子、Content-Type を、ASP.NET Core の型に依存しないメタデータとして検証します。拡張子や Content-Type は偽装できるため、これはメタデータ検証です。

## 実装

`src/DotnetBackendSnippets.Core/FileHandling/FileUploadSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/FileHandling/FileUploadSamplesTests.cs`

## 使い方

- `UploadedFileMetadata` にファイル名、バイト数、Content-Type を入れます。
- `FileUploadRules` に最大サイズ、許可する拡張子、許可する Content-Type を設定します。
- `ValidateUpload` は検証結果とエラーメッセージ一覧を返します。
- `NormalizeExtension` は `.JPG` や `jpg` のような表記揺れを `.jpg` に揃えます。
- `HasKnownFileSignature` は PNG / JPEG / PDF のような代表的なファイル先頭バイトを確認します。
- `CreateServerFileName` は元ファイル名を保存名に使わず、サーバー側で生成した ID と拡張子から保存名を作ります。

## メモ

- Content-Type はクライアントから送られる値なので、単独では信用しません。早期 reject の補助として使い、重要な用途ではファイルシグネチャ確認や保存後のスキャンも検討します。
- 拡張子だけで安全性は保証できません。保存先の分離、サーバー側で生成した保存名、実行権限のないディレクトリ、ウイルススキャンなどと組み合わせます。
- magic number と呼ばれるファイル先頭バイトの確認も万能ではありません。許可するファイル種別に応じて、画像デコードや PDF パーサーなど実際の処理系で読めるかも確認します。
- サイズ上限はアプリ側だけでなく、Web サーバーや reverse proxy 側にも設定します。

## 実務逆引き

- file upload の拡張子・サイズ・MIME を検証したい → `ValidateUpload`
- 拡張子の表記揺れを揃えたい → `NormalizeExtension`
- 空ファイルを拒否したい → `ValidateUpload`
- 大きすぎるファイルを拒否したい → `FileUploadRules.MaxBytes`
- Content-Type を早期 reject に使いたい → `FileUploadRules.AllowedContentTypes`
- PNG / JPEG / PDF の先頭バイトを確認したい → `HasKnownFileSignature`
- 元ファイル名を保存名に使わないようにしたい → `CreateServerFileName`
