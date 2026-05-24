using DotnetBackendSnippets.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetBackendSnippets.Tests.HealthChecks;

public sealed class HealthCheckSamplesTests
{
    [Fact]
    public void CreateKubernetesHealthEndpointPaths_ReturnsLiveAndReadyPaths()
    {
        HealthEndpointPaths paths = HealthCheckSamples.CreateKubernetesHealthEndpointPaths("/healthz/");

        Assert.Equal("/healthz/live", paths.Liveness);
        Assert.Equal("/healthz/ready", paths.Readiness);
    }

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

    [Fact]
    public async Task CheckDependencyAsync_ReturnsUnhealthy_WhenRequiredProbeFails()
    {
        var probe = new DelegateDependencyProbe("database", _ => Task.FromResult(false));

        HealthCheckResult result = await HealthCheckSamples.CheckDependencyAsync(probe);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("database", result.Description, StringComparison.Ordinal);
    }
}
