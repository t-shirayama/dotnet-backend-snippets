using DotnetBackendSnippets.Configuration;
using Microsoft.Extensions.Configuration;

namespace DotnetBackendSnippets.Tests.Configuration;

public sealed class ConfigurationSamplesTests
{
    [Fact]
    public void ReadAppSettings_ReturnsSettings_WhenValuesExist()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["App:ServiceName"] = "Orders",
                ["App:RetryCount"] = "3",
            })
            .Build();

        var settings = ConfigurationSamples.ReadAppSettings(configuration);

        Assert.Equal(new AppSettings("Orders", 3), settings);
    }

    [Fact]
    public void ReadAppSettings_Throws_WhenRetryCountIsInvalid()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["App:ServiceName"] = "Orders",
                ["App:RetryCount"] = "invalid",
            })
            .Build();

        var exception = Assert.Throws<InvalidOperationException>(() => ConfigurationSamples.ReadAppSettings(configuration));

        Assert.Contains("RetryCount", exception.Message, StringComparison.Ordinal);
    }
}
