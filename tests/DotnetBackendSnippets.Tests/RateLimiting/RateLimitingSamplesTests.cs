using DotnetBackendSnippets.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.Tests.RateLimiting;

public sealed class RateLimitingSamplesTests
{
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
