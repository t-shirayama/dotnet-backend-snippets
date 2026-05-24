using DotnetBackendSnippets.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.Tests.Options;

// テスト対象: Options Samples のスニペット動作を確認する。
public sealed class OptionsSamplesTests
{
    // テスト意図: Add Notification Options / Registers Options / When Configuration Is Valid を確認する。
    [Fact]
    public void AddNotificationOptions_RegistersOptions_WhenConfigurationIsValid()
    {
        var configuration = CreateConfiguration("noreply@example.test", "3");
        var services = new ServiceCollection();

        services.AddNotificationOptions(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<NotificationOptions>>().Value;

        Assert.Equal("noreply@example.test", options.SenderEmail);
        Assert.Equal(3, options.RetryCount);
    }

    // テスト意図: Add Notification Options / Throws Validation Exception / When Configuration Is Invalid を確認する。
    [Fact]
    public void AddNotificationOptions_ThrowsValidationException_WhenConfigurationIsInvalid()
    {
        var configuration = CreateConfiguration("", "9");
        var services = new ServiceCollection();

        services.AddNotificationOptions(configuration);

        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<NotificationOptions>>().Value);
    }

    // テスト意図: Notification Options Reader / Returns Current Options / From Options Monitor を確認する。
    [Fact]
    public void NotificationOptionsReader_ReturnsCurrentOptions_FromOptionsMonitor()
    {
        var configuration = CreateConfiguration("alerts@example.test", "1");
        var services = new ServiceCollection();

        services.AddNotificationOptions(configuration);

        using var provider = services.BuildServiceProvider();
        var reader = provider.GetRequiredService<NotificationOptionsReader>();

        var current = reader.GetCurrent();

        Assert.Equal("alerts@example.test", current.SenderEmail);
        Assert.Equal(1, current.RetryCount);
    }

    // テスト意図: Add Notification Options / Throws Argument Null Exception / When Services Is Null を確認する。
    [Fact]
    public void AddNotificationOptions_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        IConfiguration configuration = CreateConfiguration("alerts@example.test", "1");

        var exception = Assert.Throws<ArgumentNullException>(() => OptionsSamples.AddNotificationOptions(null!, configuration));

        Assert.Equal("services", exception.ParamName);
    }

    private static IConfiguration CreateConfiguration(string senderEmail, string retryCount)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Notifications:SenderEmail"] = senderEmail,
                ["Notifications:RetryCount"] = retryCount,
            })
            .Build();
    }
}
