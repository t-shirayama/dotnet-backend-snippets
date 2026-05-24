using DotnetBackendSnippets.Deployment;

namespace DotnetBackendSnippets.Tests.Deployment;

public sealed class DeploymentSamplesTests
{
    [Fact]
    public void CreateAspNetCoreDockerfile_UsesNonRootUserAndPort()
    {
        string dockerfile = DeploymentSamples.CreateAspNetCoreDockerfile(new DockerfileSpec("Orders.Api", 8080, 12345));

        Assert.Contains("USER appuser", dockerfile, StringComparison.Ordinal);
        Assert.Contains("EXPOSE 8080", dockerfile, StringComparison.Ordinal);
        Assert.Contains("Orders.Api.dll", dockerfile, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateComposeService_IncludesEnvironmentVariables()
    {
        string compose = DeploymentSamples.CreateComposeService(
            "api",
            "orders-api:latest",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Production",
                ["ConnectionStrings__Default"] = "${ORDERS_CONNECTION_STRING}",
            });

        Assert.Contains("image: orders-api:latest", compose, StringComparison.Ordinal);
        Assert.Contains("ASPNETCORE_ENVIRONMENT: \"Production\"", compose, StringComparison.Ordinal);
        Assert.Contains("ConnectionStrings__Default: \"${ORDERS_CONNECTION_STRING}\"", compose, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateGracefulShutdownOptions_ReturnsHostOptions()
    {
        var options = DeploymentSamples.CreateGracefulShutdownOptions(TimeSpan.FromSeconds(30));

        Assert.Equal(TimeSpan.FromSeconds(30), options.ShutdownTimeout);
    }
}
