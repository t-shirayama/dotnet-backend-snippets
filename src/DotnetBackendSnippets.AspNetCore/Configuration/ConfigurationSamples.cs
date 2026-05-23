using Microsoft.Extensions.Configuration;

namespace DotnetBackendSnippets.Configuration;

public sealed record AppSettings(string ServiceName, int RetryCount);

public static class ConfigurationSamples
{
    public static string GetRequiredValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' is required.");
        }

        return value;
    }

    public static AppSettings ReadAppSettings(IConfiguration configuration)
    {
        var section = configuration.GetSection("App");
        var serviceName = GetRequiredValue(section, "ServiceName");
        var retryCountValue = GetRequiredValue(section, "RetryCount");

        if (!int.TryParse(retryCountValue, out var retryCount) || retryCount < 0)
        {
            throw new InvalidOperationException("Configuration value 'App:RetryCount' must be a non-negative integer.");
        }

        return new AppSettings(serviceName, retryCount);
    }
}
