using DotnetBackendSnippets.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.Tests.DependencyInjection;

public sealed class DependencyInjectionSamplesTests
{
    [Fact]
    public void AddGreetingService_RegistersGreetingService()
    {
        var services = new ServiceCollection();

        services.AddGreetingService("Hi");

        using var provider = services.BuildServiceProvider();
        var greetingService = provider.GetRequiredService<IGreetingService>();

        Assert.Equal("Hi, Codex!", greetingService.CreateGreeting("Codex"));
    }
}
