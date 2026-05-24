using DotnetBackendSnippets.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.Tests.DependencyInjection;

// テスト対象: Dependency Injection Samples のスニペット動作を確認する。
public sealed class DependencyInjectionSamplesTests
{
    // テスト意図: Add Greeting Service / Registers Greeting Service を確認する。
    [Fact]
    public void AddGreetingService_RegistersGreetingService()
    {
        var services = new ServiceCollection();

        services.AddGreetingService(" Hi ");

        using var provider = services.BuildServiceProvider();
        var greetingService = provider.GetRequiredService<IGreetingService>();

        Assert.Equal("Hi, Codex!", greetingService.CreateGreeting("Codex"));
    }

    // テスト意図: Add Greeting Service / Throws Argument Exception / When Prefix Is Blank を確認する。
    [Fact]
    public void AddGreetingService_ThrowsArgumentException_WhenPrefixIsBlank()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentException>(() => services.AddGreetingService(" "));

        Assert.Equal("prefix", exception.ParamName);
    }
}
