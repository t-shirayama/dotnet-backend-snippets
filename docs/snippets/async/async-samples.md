# 非同期処理

## 目的

バックエンド実務でよく使う非同期処理の基本パターンをまとめます。対象は `IAsyncEnumerable<T>` のページング、`Task.WhenAll` の成功・失敗集約、`CancellationToken` を尊重する遅延と処理です。

## 実装

`src/DotnetBackendSnippets.Core/Async/AsyncSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Async/AsyncSamplesTests.cs`

## 使い方

- `PageAsync` は `IAsyncEnumerable<T>` から指定ページだけを読み取ります。全件をメモリに載せず、必要な件数を取得した時点で列挙を止めます。
- `WhenAllSettledAsync` は複数の `Task` をまとめて待ち、成功した結果と失敗した例外を分けて返します。1件の失敗で他の結果を捨てたくない場合に使います。
- `DelayAsync` は `Task.Delay` に `CancellationToken` を渡す最小例です。待機中にキャンセルされたら `TaskCanceledException` が発生します。
- `ProcessSequentiallyAsync` は各要素の処理前に `CancellationToken` を確認し、処理関数にも同じトークンを渡します。

## メモ

- `PageAsync` はページ番号とページサイズから計算するスキップ件数を `long` で扱い、極端に大きい値でも `int` の掛け算でオーバーフローしないようにしています。
- `WhenAllSettledAsync` は各処理を内部で捕捉してから `Task.WhenAll` するため、最後まで待って集約できます。呼び出し元で即時キャンセルしたい処理は、別途 `CancellationToken` を受け取る設計にしてください。
- `CancellationToken` はメソッドの引数に受け取るだけでなく、`Task.Delay`、非同期列挙、下位の処理関数へ渡すことが重要です。

## 実務逆引き

- `IAsyncEnumerable<T>` をページングしたい → `PageAsync`
- `Task.WhenAll` で複数処理を安全に待ちたい → `WhenAllSettledAsync`
- キャンセル可能な遅延処理を書きたい → `DelayAsync`
- `CancellationToken` を下位処理へ渡したい → `ProcessSequentiallyAsync`
- 大きいページ番号でも overflow を避けたい → `PageAsync`
