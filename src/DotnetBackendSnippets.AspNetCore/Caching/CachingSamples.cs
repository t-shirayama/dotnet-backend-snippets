using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetBackendSnippets.Caching;

/// <summary>
/// キャッシュキー生成、メモリキャッシュ、分散キャッシュの実務サンプルです。
/// </summary>
public static class CachingSamples
{
    private sealed record CachedValue<T>(T? Value);

    /// <summary>
    /// segment を連結して読みやすいキャッシュキーを作成します。
    /// </summary>
    /// <param name="prefix">キーの用途を表す prefix。</param>
    /// <param name="segments">キーを構成する segment。</param>
    /// <returns><c>prefix:segment</c> 形式のキャッシュキー。</returns>
    /// <exception cref="ArgumentException"><paramref name="prefix"/> または segment が空白の場合。</exception>
    public static string BuildCacheKey(string prefix, params string[] segments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        ArgumentNullException.ThrowIfNull(segments);

        var parts = new List<string> { NormalizeSegment(prefix) };

        foreach (string segment in segments)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(segment);
            parts.Add(NormalizeSegment(segment));
        }

        return string.Join(':', parts);
    }

    /// <summary>
    /// absolute expiration と sliding expiration を持つ memory cache 設定を作成します。
    /// </summary>
    /// <param name="absoluteExpirationRelativeToNow">登録時点からの絶対有効期限。</param>
    /// <param name="slidingExpiration">アクセスごとに延長する有効期限。</param>
    /// <returns>メモリキャッシュ登録設定。</returns>
    public static MemoryCacheEntryOptions CreateMemoryCacheEntryOptions(
        TimeSpan? absoluteExpirationRelativeToNow,
        TimeSpan? slidingExpiration)
    {
        var options = new MemoryCacheEntryOptions();

        if (absoluteExpirationRelativeToNow is not null)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        if (slidingExpiration is not null)
        {
            options.SlidingExpiration = slidingExpiration;
        }

        return options;
    }

    /// <summary>
    /// null も含めて値をメモリキャッシュから取得、または作成します。
    /// </summary>
    /// <typeparam name="T">キャッシュする値の型。</typeparam>
    /// <param name="cache">メモリキャッシュ。</param>
    /// <param name="key">キャッシュキー。</param>
    /// <param name="factory">キャッシュミス時に値を作る処理。</param>
    /// <param name="options">キャッシュ登録設定。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>キャッシュ済みまたは作成した値。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cache"/>、<paramref name="factory"/>、<paramref name="options"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> が空白の場合。</exception>
    public static async Task<T?> GetOrCreateNullableAsync<T>(
        IMemoryCache cache,
        string key,
        Func<CancellationToken, Task<T?>> factory,
        MemoryCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(options);

        if (cache.TryGetValue(key, out object? cachedObject) && cachedObject is CachedValue<T> cached)
        {
            return cached.Value;
        }

        T? value = await factory(cancellationToken);
        cache.Set(key, new CachedValue<T>(value), options);
        return value;
    }

    /// <summary>
    /// JSON として分散キャッシュへ値を保存します。
    /// </summary>
    /// <typeparam name="T">保存する値の型。</typeparam>
    /// <param name="cache">分散キャッシュ。</param>
    /// <param name="key">キャッシュキー。</param>
    /// <param name="value">保存する値。</param>
    /// <param name="options">分散キャッシュ登録設定。</param>
    /// <param name="jsonOptions">JSON オプション。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>非同期処理を表すタスク。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cache"/>、<paramref name="options"/>、<paramref name="jsonOptions"/> が <see langword="null"/> の場合。</exception>
    public static Task SetJsonAsync<T>(
        IDistributedCache cache,
        string key,
        T value,
        DistributedCacheEntryOptions options,
        JsonSerializerOptions jsonOptions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(jsonOptions);

        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, jsonOptions);
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// 分散キャッシュから JSON として値を読み取ります。
    /// </summary>
    /// <typeparam name="T">読み取る値の型。</typeparam>
    /// <param name="cache">分散キャッシュ。</param>
    /// <param name="key">キャッシュキー。</param>
    /// <param name="jsonOptions">JSON オプション。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>キャッシュ値。存在しない場合は <see langword="null"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cache"/> または <paramref name="jsonOptions"/> が <see langword="null"/> の場合。</exception>
    public static async Task<T?> GetJsonAsync<T>(
        IDistributedCache cache,
        string key,
        JsonSerializerOptions jsonOptions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(jsonOptions);

        byte[]? bytes = await cache.GetAsync(key, cancellationToken);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, jsonOptions);
    }

    /// <summary>
    /// stale cache として使う値を作成します。
    /// </summary>
    /// <typeparam name="T">キャッシュする値の型。</typeparam>
    /// <param name="value">キャッシュ値。</param>
    /// <param name="freshUntil">通常利用できる期限。</param>
    /// <param name="staleUntil">再取得失敗時に stale として利用できる期限。</param>
    /// <returns>stale cache 用エントリ。</returns>
    /// <exception cref="ArgumentException"><paramref name="staleUntil"/> が <paramref name="freshUntil"/> より前の場合。</exception>
    public static StaleCacheEntry<T> CreateStaleCacheEntry<T>(
        T value,
        DateTimeOffset freshUntil,
        DateTimeOffset staleUntil)
    {
        if (staleUntil < freshUntil)
        {
            throw new ArgumentException("Stale expiration must be greater than or equal to fresh expiration.", nameof(staleUntil));
        }

        return new StaleCacheEntry<T>(value, freshUntil, staleUntil);
    }

    /// <summary>
    /// stale cache entry の状態を判定します。
    /// </summary>
    /// <typeparam name="T">キャッシュする値の型。</typeparam>
    /// <param name="entry">判定する stale cache entry。</param>
    /// <param name="now">現在時刻。</param>
    /// <returns>fresh、stale、expired のいずれか。</returns>
    public static StaleCacheState GetStaleCacheState<T>(StaleCacheEntry<T> entry, DateTimeOffset now)
    {
        if (now <= entry.FreshUntil)
        {
            return StaleCacheState.Fresh;
        }

        return now <= entry.StaleUntil ? StaleCacheState.Stale : StaleCacheState.Expired;
    }

    /// <summary>
    /// 外部 API レスポンス向けの cache key を作成します。
    /// </summary>
    /// <param name="apiName">外部 API 名。</param>
    /// <param name="resourceName">取得する resource 名。</param>
    /// <param name="resourceId">resource ID。</param>
    /// <returns>外部 API レスポンス向け cache key。</returns>
    /// <exception cref="ArgumentException">引数が空白の場合。</exception>
    public static string BuildExternalApiCacheKey(string apiName, string resourceName, string resourceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiName);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

        return BuildCacheKey("external-api", apiName, resourceName, resourceId);
    }

    /// <summary>
    /// cache hit / miss の比率を計算します。
    /// </summary>
    /// <param name="hits">cache hit 回数。</param>
    /// <param name="misses">cache miss 回数。</param>
    /// <returns>cache metrics の snapshot。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hits"/> または <paramref name="misses"/> が負数の場合。</exception>
    public static CacheMetricsSnapshot CreateCacheMetricsSnapshot(long hits, long misses)
    {
        if (hits < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hits), "Hits must be zero or greater.");
        }

        if (misses < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(misses), "Misses must be zero or greater.");
        }

        long total = checked(hits + misses);
        double hitRate = total == 0 ? 0d : (double)hits / total;

        return new CacheMetricsSnapshot(hits, misses, hitRate);
    }

    private static string NormalizeSegment(string value)
    {
        string normalized = value.Trim().ToLowerInvariant();
        Span<char> buffer = normalized.Length <= 256
            ? stackalloc char[normalized.Length]
            : new char[normalized.Length];

        var index = 0;
        var previousWasSeparator = false;

        foreach (char character in normalized)
        {
            if (char.IsAsciiLetterOrDigit(character))
            {
                buffer[index++] = character;
                previousWasSeparator = false;
            }
            else if (!previousWasSeparator)
            {
                buffer[index++] = '-';
                previousWasSeparator = true;
            }
        }

        string result = new string(buffer[..index]).Trim('-');
        return string.IsNullOrWhiteSpace(result)
            ? throw new ArgumentException("Cache key segment must contain at least one ASCII letter or digit.", nameof(value))
            : result;
    }
}

/// <summary>
/// stale cache の状態を表します。
/// </summary>
public enum StaleCacheState
{
    /// <summary>
    /// 通常のキャッシュ値として利用できます。
    /// </summary>
    Fresh,

    /// <summary>
    /// 再取得失敗時などに限定して利用する古い値です。
    /// </summary>
    Stale,

    /// <summary>
    /// 期限切れで利用しません。
    /// </summary>
    Expired,
}

/// <summary>
/// stale cache 用の値と期限を表します。
/// </summary>
/// <typeparam name="T">キャッシュする値の型。</typeparam>
/// <param name="Value">キャッシュ値。</param>
/// <param name="FreshUntil">通常利用できる期限。</param>
/// <param name="StaleUntil">stale として利用できる期限。</param>
public sealed record StaleCacheEntry<T>(T Value, DateTimeOffset FreshUntil, DateTimeOffset StaleUntil);

/// <summary>
/// cache metrics の snapshot を表します。
/// </summary>
/// <param name="Hits">cache hit 回数。</param>
/// <param name="Misses">cache miss 回数。</param>
/// <param name="HitRate">cache hit 率。</param>
public sealed record CacheMetricsSnapshot(long Hits, long Misses, double HitRate);

/// <summary>
/// cache stampede を避けるため、同じキーの factory 実行を同時に 1 つへ絞ります。
/// </summary>
public sealed class MemoryCacheStampedeGuard
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new(StringComparer.Ordinal);

    /// <summary>
    /// 同じキーへの同時アクセスを抑えながらメモリキャッシュから値を取得、または作成します。
    /// </summary>
    /// <typeparam name="T">キャッシュする値の型。</typeparam>
    /// <param name="cache">メモリキャッシュ。</param>
    /// <param name="key">キャッシュキー。</param>
    /// <param name="factory">キャッシュミス時に値を作る処理。</param>
    /// <param name="options">キャッシュ登録設定。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>キャッシュ済みまたは作成した値。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cache"/>、<paramref name="factory"/>、<paramref name="options"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> が空白の場合。</exception>
    public async Task<T?> GetOrCreateOnceAsync<T>(
        IMemoryCache cache,
        string key,
        Func<CancellationToken, Task<T?>> factory,
        MemoryCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(options);

        SemaphoreSlim semaphore = locks.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            return await CachingSamples.GetOrCreateNullableAsync(cache, key, factory, options, cancellationToken);
        }
        finally
        {
            semaphore.Release();
            locks.TryRemove(key, out _);
        }
    }
}
