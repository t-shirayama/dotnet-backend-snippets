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
