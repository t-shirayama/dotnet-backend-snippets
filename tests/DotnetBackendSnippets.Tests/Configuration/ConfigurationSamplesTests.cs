using DotnetBackendSnippets.Configuration;
using Microsoft.Extensions.Configuration;

namespace DotnetBackendSnippets.Tests.Configuration;

// テスト対象: Configuration Samples のスニペット動作を確認する。
public sealed class ConfigurationSamplesTests
{
    // テスト意図: Read App Settings / Returns Settings / When Values Exist を確認する。
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

    // テスト意図: Read App Settings / Throws / When Retry Count Is Invalid を確認する。
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

    // テスト意図: Create Environment Settings Files / Returns Base And Environment Specific Names を確認する。
    [Fact]
    public void CreateEnvironmentSettingsFiles_ReturnsBaseAndEnvironmentSpecificNames()
    {
        var files = ConfigurationSamples.CreateEnvironmentSettingsFiles("Production");

        Assert.Equal("appsettings.json", files.BaseFile);
        Assert.Equal("appsettings.Production.json", files.EnvironmentFile);
    }

    // テスト意図: Create Environment Variable Prefix / Normalizes Application Name を確認する。
    [Fact]
    public void CreateEnvironmentVariablePrefix_NormalizesApplicationName()
    {
        string prefix = ConfigurationSamples.CreateEnvironmentVariablePrefix("Orders.Api");

        Assert.Equal("ORDERSAPI_", prefix);
    }

    // テスト意図: Get Required Connection String / Returns Connection String を確認する。
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

    // テスト意図: Redact Secret Settings / Masks Secret Values を確認する。
    [Fact]
    public void RedactSecretSettings_MasksSecretValues()
    {
        Dictionary<string, string?> settings = new(StringComparer.Ordinal)
        {
            ["ApiKey"] = "secret-value",
            ["External:Api-Key"] = "header-secret",
            ["Database:Connection-String"] = "Data Source=orders.db",
            ["ServiceName"] = "Orders",
        };

        IReadOnlyDictionary<string, string?> redacted = ConfigurationSamples.RedactSecretSettings(settings);

        Assert.Equal("***", redacted["ApiKey"]);
        Assert.Equal("***", redacted["External:Api-Key"]);
        Assert.Equal("***", redacted["Database:Connection-String"]);
        Assert.Equal("Orders", redacted["ServiceName"]);
    }

    // テスト意図: Get Required Value / Throws Argument Exception / When Key Is Blank を確認する。
    [Fact]
    public void GetRequiredValue_ThrowsArgumentException_WhenKeyIsBlank()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<ArgumentException>(() => ConfigurationSamples.GetRequiredValue(configuration, " "));

        Assert.Equal("key", exception.ParamName);
    }
}
