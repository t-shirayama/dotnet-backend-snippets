using DotnetBackendSnippets.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetBackendSnippets.Tests.HealthChecks;

// テスト対象: Health Check Samples のスニペット動作を確認する。
public sealed class HealthCheckSamplesTests
{
    // テスト意図: Create Kubernetes Health Endpoint Paths / Returns Live And Ready Paths を確認する。
    [Fact]
    public void CreateKubernetesHealthEndpointPaths_ReturnsLiveAndReadyPaths()
    {
        HealthEndpointPaths paths = HealthCheckSamples.CreateKubernetesHealthEndpointPaths("/healthz/");

        Assert.Equal("/healthz/live", paths.Liveness);
        Assert.Equal("/healthz/ready", paths.Readiness);
    }

    // テスト意図: Create Kubernetes Health Endpoint Paths / Handles Root Base Path Without Double Slash を確認する。
    [Fact]
    public void CreateKubernetesHealthEndpointPaths_HandlesRootBasePathWithoutDoubleSlash()
    {
        HealthEndpointPaths paths = HealthCheckSamples.CreateKubernetesHealthEndpointPaths("/");

        Assert.Equal("/live", paths.Liveness);
        Assert.Equal("/ready", paths.Readiness);
    }

    // テスト意図: Create Readiness Result / Returns Degraded / When Optional Dependency Is Degraded を確認する。
    [Fact]
    public void CreateReadinessResult_ReturnsDegraded_WhenOptionalDependencyIsDegraded()
    {
        Dictionary<string, HealthCheckResult> dependencies = new(StringComparer.Ordinal)
        {
            ["database"] = HealthCheckResult.Healthy(),
            ["external-api"] = HealthCheckResult.Degraded(),
        };

        HealthCheckResult result = HealthCheckSamples.CreateReadinessResult(dependencies);

        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Equal("Degraded", result.Data["external-api"]);
    }

    // テスト意図: Check Dependency Async / Returns Unhealthy / When Required Probe Fails を確認する。
    [Fact]
    public async Task CheckDependencyAsync_ReturnsUnhealthy_WhenRequiredProbeFails()
    {
        var probe = new DelegateDependencyProbe("database", _ => Task.FromResult(false));

        HealthCheckResult result = await HealthCheckSamples.CheckDependencyAsync(probe);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("database", result.Description, StringComparison.Ordinal);
    }

    // テスト意図: Delegate Dependency Probe / Rejects Invalid Delegate を確認する。
    [Fact]
    public void DelegateDependencyProbe_RejectsInvalidDelegate()
    {
        Assert.Throws<ArgumentException>(() => new DelegateDependencyProbe(" ", _ => Task.FromResult(true)));
        Assert.Throws<ArgumentNullException>(() => new DelegateDependencyProbe("database", null!));
    }
}
