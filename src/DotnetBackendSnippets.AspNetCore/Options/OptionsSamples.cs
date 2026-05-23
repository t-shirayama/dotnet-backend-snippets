using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.Options;

public sealed class NotificationOptions
{
    public const string SectionName = "Notifications";

    public string SenderEmail { get; init; } = string.Empty;

    public int RetryCount { get; init; }
}

public sealed class NotificationOptionsReader(IOptionsMonitor<NotificationOptions> optionsMonitor)
{
    public NotificationOptions GetCurrent()
    {
        return optionsMonitor.CurrentValue;
    }
}

public static class OptionsSamples
{
    public static IServiceCollection AddNotificationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<NotificationOptions>()
            .Bind(configuration.GetSection(NotificationOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.SenderEmail), "SenderEmail is required.")
            .Validate(options => options.RetryCount is >= 0 and <= 5, "RetryCount must be between 0 and 5.")
            .ValidateOnStart();

        services.AddSingleton<NotificationOptionsReader>();

        return services;
    }
}
