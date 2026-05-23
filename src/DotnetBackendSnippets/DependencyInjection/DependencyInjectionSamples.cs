using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.DependencyInjection;

public interface IGreetingService
{
    string CreateGreeting(string name);
}

public sealed class GreetingService(string prefix) : IGreetingService
{
    public string CreateGreeting(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        return $"{prefix}, {name}!";
    }
}

public static class DependencyInjectionSamples
{
    public static IServiceCollection AddGreetingService(this IServiceCollection services, string prefix = "Hello")
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IGreetingService>(_ => new GreetingService(prefix));
        return services;
    }
}
