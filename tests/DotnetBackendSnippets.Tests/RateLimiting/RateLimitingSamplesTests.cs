using DotnetBackendSnippets.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.Tests.RateLimiting;

// テスト対象: Rate Limiting Samples のスニペット動作を確認する。
public sealed class RateLimitingSamplesTests
{
    // テスト意図: Fixed Window Rate Limiter / Rejects Requests After Limit Until Window Resets を確認する。
    [Fact]
    public void FixedWindowRateLimiter_RejectsRequestsAfterLimitUntilWindowResets()
    {
        var now = new DateTimeOffset(2026, 5, 24, 10, 0, 0, TimeSpan.Zero);
        var limiter = new FixedWindowRateLimiter(
            new FixedWindowRateLimitRule(2, TimeSpan.FromMinutes(1), RateLimitKeyMode.User),
            () => now);
        var context = new RateLimitContext("127.0.0.1", "user-1", "GetOrders");

        Assert.True(limiter.Check(context).IsAllowed);
        RateLimitDecision second = limiter.Check(context);
        RateLimitDecision third = limiter.Check(context);

        Assert.True(second.IsAllowed);
        Assert.Equal(0, second.Remaining);
        Assert.False(third.IsAllowed);
        Assert.Equal(TimeSpan.FromMinutes(1), third.RetryAfter);

        now = now.AddMinutes(1);
        Assert.True(limiter.Check(context).IsAllowed);
    }

    // テスト意図: Fixed Window Rate Limiter / Removes Expired Counters を確認する。
    [Fact]
    public void FixedWindowRateLimiter_RemovesExpiredCounters()
    {
        var now = new DateTimeOffset(2026, 5, 24, 10, 0, 0, TimeSpan.Zero);
        var limiter = new FixedWindowRateLimiter(
            new FixedWindowRateLimitRule(2, TimeSpan.FromMinutes(1), RateLimitKeyMode.User),
            () => now);

        limiter.Check(new RateLimitContext("127.0.0.1", "user-1", "GetOrders"));
        limiter.Check(new RateLimitContext("127.0.0.2", "user-2", "GetOrders"));
        Assert.Equal(2, limiter.ActiveKeyCount);

        now = now.AddMinutes(2);
        limiter.Check(new RateLimitContext("127.0.0.3", "user-3", "GetOrders"));

        Assert.Equal(1, limiter.ActiveKeyCount);
    }

    // テスト意図: Resolve Rate Limit Key / Returns Expected Key を確認する。
    [Theory]
    [InlineData(RateLimitKeyMode.IpAddress, "ip:127.0.0.1")]
    [InlineData(RateLimitKeyMode.User, "user:user-1")]
    [InlineData(RateLimitKeyMode.Endpoint, "endpoint:GetOrders")]
    public void ResolveRateLimitKey_ReturnsExpectedKey(RateLimitKeyMode keyMode, string expected)
    {
        var context = new RateLimitContext("127.0.0.1", "user-1", "GetOrders");

        string key = RateLimitingSamples.ResolveRateLimitKey(context, keyMode);

        Assert.Equal(expected, key);
    }

    // テスト意図: Resolve Rate Limit Key / Normalizes Blank Optional Parts And Rejects Blank Endpoint を確認する。
    [Fact]
    public void ResolveRateLimitKey_NormalizesBlankOptionalPartsAndRejectsBlankEndpoint()
    {
        var anonymousContext = new RateLimitContext(" ", " ", "GetOrders");
        var blankEndpointContext = new RateLimitContext("127.0.0.1", "user-1", " ");

        Assert.Equal("ip:unknown", RateLimitingSamples.ResolveRateLimitKey(anonymousContext, RateLimitKeyMode.IpAddress));
        Assert.Equal("anonymous:unknown", RateLimitingSamples.ResolveRateLimitKey(anonymousContext, RateLimitKeyMode.User));
        Assert.Throws<ArgumentException>(
            () => RateLimitingSamples.ResolveRateLimitKey(blankEndpointContext, RateLimitKeyMode.Endpoint));
    }

    // テスト意図: Create Too Many Requests Problem / Returns 429 Problem Details を確認する。
    [Fact]
    public void CreateTooManyRequestsProblem_Returns429ProblemDetails()
    {
        var decision = new RateLimitDecision(false, "user:user-1", 0, TimeSpan.FromSeconds(30));

        ProblemDetails problem = RateLimitingSamples.CreateTooManyRequestsProblem(decision);

        Assert.Equal(StatusCodes.Status429TooManyRequests, problem.Status);
        Assert.Equal("user:user-1", problem.Extensions["rateLimitKey"]);
        Assert.Equal(30d, problem.Extensions["retryAfterSeconds"]);
    }
}
