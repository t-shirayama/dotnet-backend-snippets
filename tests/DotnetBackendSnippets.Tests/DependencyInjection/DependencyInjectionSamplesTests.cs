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

    // テスト意図: Add Lifetime Example Services / Registers Expected Lifetimes を確認する。
    [Fact]
    public void AddLifetimeExampleServices_RegistersExpectedLifetimes()
    {
        var services = new ServiceCollection();

        services.AddLifetimeExampleServices();

        using var provider = services.BuildServiceProvider();
        var singleton1 = provider.GetRequiredService<ApplicationIdProvider>();
        var singleton2 = provider.GetRequiredService<ApplicationIdProvider>();
        var transient1 = provider.GetRequiredService<OperationIdFactory>();
        var transient2 = provider.GetRequiredService<OperationIdFactory>();

        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();
        var scoped1a = scope1.ServiceProvider.GetRequiredService<RequestState>();
        var scoped1b = scope1.ServiceProvider.GetRequiredService<RequestState>();
        var scoped2 = scope2.ServiceProvider.GetRequiredService<RequestState>();

        Assert.Same(singleton1, singleton2);
        Assert.NotEqual(transient1.OperationId, transient2.OperationId);
        Assert.Same(scoped1a, scoped1b);
        Assert.NotEqual(scoped1a.ScopeId, scoped2.ScopeId);
    }

    // テスト意図: Add Report Formatters / Resolves Keyed Services を確認する。
    [Fact]
    public void AddReportFormatters_ResolvesKeyedServices()
    {
        var services = new ServiceCollection();

        services.AddReportFormatters();

        using var provider = services.BuildServiceProvider();
        var plain = provider.GetRequiredKeyedService<IReportFormatter>("plain");
        var markdown = provider.GetRequiredKeyedService<IReportFormatter>("markdown");

        Assert.Equal("Weekly Report", plain.FormatTitle(" Weekly Report "));
        Assert.Equal("# Weekly Report", markdown.FormatTitle(" Weekly Report "));
    }

    // テスト意図: Replace Greeting Service / Replaces Existing Registration を確認する。
    [Fact]
    public void ReplaceGreetingService_ReplacesExistingRegistration()
    {
        var services = new ServiceCollection();

        services
            .AddGreetingService("Hello")
            .ReplaceGreetingService(new FakeGreetingService());

        using var provider = services.BuildServiceProvider();
        var greetingService = provider.GetRequiredService<IGreetingService>();

        Assert.Equal("TEST: Codex", greetingService.CreateGreeting("Codex"));
    }

    private sealed class FakeGreetingService : IGreetingService
    {
        public string CreateGreeting(string name)
        {
            return $"TEST: {name}";
        }
    }
}
