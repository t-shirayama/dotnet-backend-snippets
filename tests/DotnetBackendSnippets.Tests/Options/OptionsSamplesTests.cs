using DotnetBackendSnippets.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.Tests.Options;

public sealed class OptionsSamplesTests
{
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

    [Fact]
    public void AddNotificationOptions_ThrowsValidationException_WhenConfigurationIsInvalid()
    {
        var configuration = CreateConfiguration("", "9");
        var services = new ServiceCollection();

        services.AddNotificationOptions(configuration);

        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<NotificationOptions>>().Value);
    }

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
