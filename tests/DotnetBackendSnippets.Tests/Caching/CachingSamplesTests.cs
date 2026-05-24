using System.Text.Json;
using DotnetBackendSnippets.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetBackendSnippets.Tests.Caching;

// テスト対象: Caching Samples のスニペット動作を確認する。
public sealed class CachingSamplesTests
{
    // テスト意図: Build Cache Key / Normalizes Segments を確認する。
    [Fact]
    public void BuildCacheKey_NormalizesSegments()
    {
        string key = CachingSamples.BuildCacheKey("Product Search", "Tenant A", "Page 1");

        Assert.Equal("product-search:tenant-a:page-1", key);
    }

    // テスト意図: Memory Cache Stampede Guard / Rejects Blank Key Before Lock Lookup を確認する。
    [Fact]
    public async Task MemoryCacheStampedeGuard_RejectsBlankKeyBeforeLockLookup()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var guard = new MemoryCacheStampedeGuard();

        await Assert.ThrowsAsync<ArgumentException>(
            () => guard.GetOrCreateOnceAsync(
                cache,
                " ",
                _ => Task.FromResult<string?>("created"),
                new MemoryCacheEntryOptions()));
    }

    // テスト意図: Get Or Create Nullable Async / Caches Null Value を確認する。
    [Fact]
    public async Task GetOrCreateNullableAsync_CachesNullValue()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var calls = 0;

        string? first = await CachingSamples.GetOrCreateNullableAsync<string>(
            cache,
            "missing-user",
            _ =>
            {
                calls++;
                return Task.FromResult<string?>(null);
            },
            new MemoryCacheEntryOptions());
        string? second = await CachingSamples.GetOrCreateNullableAsync<string>(
            cache,
            "missing-user",
            _ =>
            {
                calls++;
                return Task.FromResult<string?>("created");
            },
            new MemoryCacheEntryOptions());

        Assert.Null(first);
        Assert.Null(second);
        Assert.Equal(1, calls);
    }

    // テスト意図: Memory Cache Stampede Guard / Runs Factory Once / For Concurrent Misses を確認する。
    [Fact]
    public async Task MemoryCacheStampedeGuard_RunsFactoryOnce_ForConcurrentMisses()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var guard = new MemoryCacheStampedeGuard();
        var calls = 0;

        Task<string?>[] tasks = Enumerable.Range(0, 5)
            .Select(_ => guard.GetOrCreateOnceAsync(
                cache,
                "expensive",
                async _ =>
                {
                    Interlocked.Increment(ref calls);
                    await Task.Delay(50);
                    return "cached";
                },
                new MemoryCacheEntryOptions()))
            .ToArray();

        string?[] results = await Task.WhenAll(tasks);

        Assert.All(results, result => Assert.Equal("cached", result));
        Assert.Equal(1, calls);
    }

    // テスト意図: Distributed Cache JSON Helpers / Round Trip JSON を確認する。
    [Fact]
    public async Task DistributedCacheJsonHelpers_RoundTripJson()
    {
        var cache = new InMemoryDistributedCache();
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var entryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        };
        var value = new CachedUser("user-1", "Alice");

        await CachingSamples.SetJsonAsync(cache, "user:user-1", value, entryOptions, options);
        CachedUser? actual = await CachingSamples.GetJsonAsync<CachedUser>(cache, "user:user-1", options);

        Assert.Equal(value, actual);
    }

    private sealed record CachedUser(string Id, string Name);

    private sealed class InMemoryDistributedCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> values = new(StringComparer.Ordinal);

        public byte[]? Get(string key)
        {
            return values.GetValueOrDefault(key);
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return Task.FromResult(Get(key));
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            values.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            values[key] = value;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value, options);
            return Task.CompletedTask;
        }
    }
}
