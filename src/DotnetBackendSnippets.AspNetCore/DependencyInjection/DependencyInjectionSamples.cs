using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.DependencyInjection;

/// <summary>
/// あいさつ文を作成するサービスを表します。
/// </summary>
public interface IGreetingService
{
    /// <summary>
    /// 名前を使ったあいさつ文を作成します。
    /// </summary>
    /// <param name="name">あいさつ対象の名前。</param>
    /// <returns>作成したあいさつ文。</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> が空白の場合。</exception>
    string CreateGreeting(string name);
}

/// <summary>
/// 固定の接頭辞であいさつ文を作成します。
/// </summary>
/// <param name="prefix">あいさつ文の接頭辞。</param>
public sealed class GreetingService(string prefix) : IGreetingService
{
    /// <inheritdoc />
    public string CreateGreeting(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        return $"{prefix}, {name}!";
    }
}

/// <summary>
/// DI 登録のサンプルです。
/// </summary>
public static class DependencyInjectionSamples
{
    /// <summary>
    /// あいさつサービスを DI コンテナーに登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="prefix">あいさつ文の接頭辞。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddGreetingService(this IServiceCollection services, string prefix = "Hello")
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IGreetingService>(_ => new GreetingService(prefix));
        return services;
    }
}
