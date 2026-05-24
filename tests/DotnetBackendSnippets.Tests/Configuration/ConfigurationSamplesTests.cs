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

    [Fact]
    public void CreateEnvironmentSettingsFiles_ReturnsBaseAndEnvironmentSpecificNames()
    {
        var files = ConfigurationSamples.CreateEnvironmentSettingsFiles("Production");

        Assert.Equal("appsettings.json", files.BaseFile);
        Assert.Equal("appsettings.Production.json", files.EnvironmentFile);
    }

    [Fact]
    public void CreateEnvironmentVariablePrefix_NormalizesApplicationName()
    {
        string prefix = ConfigurationSamples.CreateEnvironmentVariablePrefix("Orders.Api");

        Assert.Equal("ORDERSAPI_", prefix);
    }

    [Fact]
    public void GetRequiredConnectionString_ReturnsConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Orders"] = "Data Source=orders.db",
            })
            .Build();

        string connectionString = ConfigurationSamples.GetRequiredConnectionString(configuration, "Orders");

        Assert.Equal("Data Source=orders.db", connectionString);
    }

    [Fact]
    public void RedactSecretSettings_MasksSecretValues()
    {
        Dictionary<string, string?> settings = new(StringComparer.Ordinal)
        {
            ["ApiKey"] = "secret-value",
            ["ServiceName"] = "Orders",
        };

        IReadOnlyDictionary<string, string?> redacted = ConfigurationSamples.RedactSecretSettings(settings);

        Assert.Equal("***", redacted["ApiKey"]);
        Assert.Equal("Orders", redacted["ServiceName"]);
    }
}
