using DotnetBackendSnippets.Deployment;

namespace DotnetBackendSnippets.Tests.Deployment;

// テスト対象: Deployment Samples のスニペット動作を確認する。
public sealed class DeploymentSamplesTests
{
    // テスト意図: Create ASP.NET Core Dockerfile / Uses Non Root User And Port を確認する。
    [Fact]
    public void CreateAspNetCoreDockerfile_UsesNonRootUserAndPort()
    {
        string dockerfile = DeploymentSamples.CreateAspNetCoreDockerfile(new DockerfileSpec("Orders.Api", 8080, 12345));

        Assert.Contains("USER appuser", dockerfile, StringComparison.Ordinal);
        Assert.Contains("EXPOSE 8080", dockerfile, StringComparison.Ordinal);
        Assert.Contains("Orders.Api.dll", dockerfile, StringComparison.Ordinal);
        Assert.DoesNotContain("\r\n", dockerfile, StringComparison.Ordinal);
    }

    // テスト意図: Create Compose Service / Includes Environment Variables を確認する。
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
                ["Quoted"] = "value \"with\" quote",
            });

        Assert.Contains("image: orders-api:latest", compose, StringComparison.Ordinal);
        Assert.Contains("ASPNETCORE_ENVIRONMENT: \"Production\"", compose, StringComparison.Ordinal);
        Assert.Contains("ConnectionStrings__Default: \"${ORDERS_CONNECTION_STRING}\"", compose, StringComparison.Ordinal);
        Assert.Contains("Quoted: \"value \\\"with\\\" quote\"", compose, StringComparison.Ordinal);
        Assert.DoesNotContain("\r\n", compose, StringComparison.Ordinal);
    }

    // テスト意図: Create Compose Service / Escapes Backslash Environment Values を確認する。
    [Fact]
    public void CreateComposeService_EscapesBackslashEnvironmentValues()
    {
        string compose = DeploymentSamples.CreateComposeService(
            "api",
            "orders-api:latest",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Json__Path"] = "C:\\data\\orders",
            });

        Assert.Contains("Json__Path: \"C:\\\\data\\\\orders\"", compose, StringComparison.Ordinal);
    }

    // テスト意図: Create ASP.NET Core Dockerfile / Throws Argument Exception / When Assembly Name Is Unsafe を確認する。
    [Fact]
    public void CreateAspNetCoreDockerfile_ThrowsArgumentException_WhenAssemblyNameIsUnsafe()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            DeploymentSamples.CreateAspNetCoreDockerfile(new DockerfileSpec("Orders.Api\"")));

        Assert.Equal("spec", exception.ParamName);
    }

    // テスト意図: Create Graceful Shutdown Options / Returns Host Options を確認する。
    [Fact]
    public void CreateGracefulShutdownOptions_ReturnsHostOptions()
    {
        var options = DeploymentSamples.CreateGracefulShutdownOptions(TimeSpan.FromSeconds(30));

        Assert.Equal(TimeSpan.FromSeconds(30), options.ShutdownTimeout);
    }
}
