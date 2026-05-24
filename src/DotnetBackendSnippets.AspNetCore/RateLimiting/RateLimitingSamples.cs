using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.RateLimiting;

/// <summary>
/// rate limit のキー種別を表します。
/// </summary>
public enum RateLimitKeyMode
{
    /// <summary>
    /// IP アドレス単位で制限します。
    /// </summary>
    IpAddress,

    /// <summary>
    /// ユーザー ID 単位で制限します。
    /// </summary>
    User,

    /// <summary>
    /// エンドポイント名単位で制限します。
    /// </summary>
    Endpoint,
}

/// <summary>
/// rate limit の入力を表します。
/// </summary>
/// <param name="IpAddress">クライアント IP アドレス。</param>
/// <param name="UserId">認証済みユーザー ID。</param>
/// <param name="EndpointName">エンドポイント名。</param>
public sealed record RateLimitContext(string? IpAddress, string? UserId, string EndpointName);

/// <summary>
/// fixed window rate limit のルールを表します。
/// </summary>
/// <param name="PermitLimit">window 内で許可する回数。</param>
/// <param name="Window">window の長さ。</param>
/// <param name="KeyMode">キー種別。</param>
public sealed record FixedWindowRateLimitRule(int PermitLimit, TimeSpan Window, RateLimitKeyMode KeyMode);

/// <summary>
/// rate limit 判定結果を表します。
/// </summary>
/// <param name="IsAllowed">許可されたかどうか。</param>
/// <param name="Key">判定に使ったキー。</param>
/// <param name="Remaining">残り許可回数。</param>
/// <param name="RetryAfter">拒否時に再試行まで待つ時間。</param>
public sealed record RateLimitDecision(bool IsAllowed, string Key, int Remaining, TimeSpan RetryAfter);

/// <summary>
/// テストしやすい fixed window rate limiter です。
/// </summary>
public sealed class FixedWindowRateLimiter
{
    private readonly FixedWindowRateLimitRule rule;
    private readonly Func<DateTimeOffset> getUtcNow;
    private readonly object syncLock = new();
    private readonly Dictionary<string, WindowCounter> counters = new(StringComparer.Ordinal);

    /// <summary>
    /// <see cref="FixedWindowRateLimiter"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="rule">rate limit ルール。</param>
    /// <param name="getUtcNow">現在時刻を返す関数。</param>
    /// <exception cref="ArgumentOutOfRangeException">許可回数または window が不正な場合。</exception>
    public FixedWindowRateLimiter(FixedWindowRateLimitRule rule, Func<DateTimeOffset>? getUtcNow = null)
    {
        if (rule.PermitLimit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rule), "Permit limit must be positive.");
        }

        if (rule.Window <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(rule), "Window must be positive.");
        }

        this.rule = rule;
        this.getUtcNow = getUtcNow ?? (() => DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// 現在のリクエストを許可するか判定します。
    /// </summary>
    /// <param name="context">rate limit 入力。</param>
    /// <returns>判定結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> が <see langword="null"/> の場合。</exception>
    public RateLimitDecision Check(RateLimitContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string key = RateLimitingSamples.ResolveRateLimitKey(context, rule.KeyMode);
        DateTimeOffset now = getUtcNow();

        lock (syncLock)
        {
            if (!counters.TryGetValue(key, out WindowCounter? counter) || now >= counter.ResetAt)
            {
                counter = new WindowCounter(0, now.Add(rule.Window));
                counters[key] = counter;
            }

            if (counter.Count >= rule.PermitLimit)
            {
                return new RateLimitDecision(false, key, 0, counter.ResetAt - now);
            }

            counter.Count++;
            return new RateLimitDecision(true, key, rule.PermitLimit - counter.Count, TimeSpan.Zero);
        }
    }

    private sealed class WindowCounter(int count, DateTimeOffset resetAt)
    {
        public int Count { get; set; } = count;

        public DateTimeOffset ResetAt { get; } = resetAt;
    }
}

/// <summary>
/// rate limiting のキー設計と 429 応答のサンプルです。
/// </summary>
public static class RateLimitingSamples
{
    /// <summary>
    /// 入力とキー種別から rate limit key を作成します。
    /// </summary>
    /// <param name="context">rate limit 入力。</param>
    /// <param name="keyMode">キー種別。</param>
    /// <returns>rate limit key。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> が <see langword="null"/> の場合。</exception>
    public static string ResolveRateLimitKey(RateLimitContext context, RateLimitKeyMode keyMode)
    {
        ArgumentNullException.ThrowIfNull(context);

        return keyMode switch
        {
            RateLimitKeyMode.IpAddress => $"ip:{NormalizeOptionalKeyPart(context.IpAddress, "unknown")}",
            RateLimitKeyMode.User => string.IsNullOrWhiteSpace(context.UserId)
                ? $"anonymous:{NormalizeOptionalKeyPart(context.IpAddress, "unknown")}"
                : $"user:{context.UserId.Trim()}",
            RateLimitKeyMode.Endpoint => $"endpoint:{NormalizeRequiredKeyPart(context.EndpointName, nameof(context.EndpointName))}",
            _ => throw new ArgumentOutOfRangeException(nameof(keyMode), keyMode, "Unknown key mode."),
        };
    }

    /// <summary>
    /// rate limit 超過時の ProblemDetails を作成します。
    /// </summary>
    /// <param name="decision">拒否判定。</param>
    /// <returns>429 を表す ProblemDetails。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="decision"/> が <see langword="null"/> の場合。</exception>
    public static ProblemDetails CreateTooManyRequestsProblem(RateLimitDecision decision)
    {
        ArgumentNullException.ThrowIfNull(decision);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Too many requests.",
            Detail = "Rate limit exceeded. Retry after the specified number of seconds.",
            Type = "https://httpstatuses.com/429",
        };

        problem.Extensions["rateLimitKey"] = decision.Key;
        problem.Extensions["retryAfterSeconds"] = Math.Ceiling(decision.RetryAfter.TotalSeconds);
        return problem;
    }

    private static string NormalizeOptionalKeyPart(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static string NormalizeRequiredKeyPart(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
