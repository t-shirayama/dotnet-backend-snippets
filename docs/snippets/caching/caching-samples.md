# Caching の基本

## 目的

`IMemoryCache`、分散キャッシュ、キャッシュキー設計、有効期限、null cache、stampede 対策を小さく確認するためのスニペットです。

## 実装

`src/DotnetBackendSnippets.AspNetCore/Caching/CachingSamples.cs`

## テスト

`tests/DotnetBackendSnippets.Tests/Caching/CachingSamplesTests.cs`

## 使い方

- `BuildCacheKey` は prefix と segment から読みやすいキーを作ります。
- `CreateMemoryCacheEntryOptions` は absolute expiration と sliding expiration をまとめます。
- `GetOrCreateNullableAsync` は `null` の結果もキャッシュし、存在しないデータへの繰り返し問い合わせを減らします。
- `MemoryCacheStampedeGuard.GetOrCreateOnceAsync` は同じキーの同時 factory 実行を 1 回に絞ります。
- `SetJsonAsync` / `GetJsonAsync` は `IDistributedCache` に JSON として値を保存・取得します。

## メモ

- キャッシュキーは人間が読めることと衝突しにくいことを優先し、prefix に用途やバージョンを含めると運用しやすくなります。
- null cache は DB や外部 API に「存在しない値」を何度も問い合わせる負荷を減らします。ただし短めの TTL にします。
- stampede 対策は単一プロセス内の多重実行を抑えます。複数台構成では分散 lock や短い stale cache も検討します。
- 分散キャッシュへ保存する DTO は、互換性を意識して JSON 形式や有効期限を設計します。

## 実務逆引き

- cache key を組み立てたい → `BuildCacheKey`
- absolute / sliding expiration を設定したい → `CreateMemoryCacheEntryOptions`
- null をキャッシュしたい → `GetOrCreateNullableAsync`
- stampede を抑えたい → `MemoryCacheStampedeGuard`
- distributed cache に JSON を保存したい → `SetJsonAsync` / `GetJsonAsync`
