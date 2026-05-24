using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
public sealed class GreetingService : IGreetingService
{
    private readonly string prefix;

    /// <summary>
    /// <see cref="GreetingService"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="prefix">あいさつ文の接頭辞。</param>
    /// <exception cref="ArgumentException"><paramref name="prefix"/> が空白の場合。</exception>
    public GreetingService(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        this.prefix = prefix.Trim();
    }

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
/// レポートの表示形式を整形するサービスを表します。
/// </summary>
public interface IReportFormatter
{
    /// <summary>
    /// タイトルを指定形式へ整形します。
    /// </summary>
    /// <param name="title">整形するタイトル。</param>
    /// <returns>整形済みのタイトル。</returns>
    /// <exception cref="ArgumentException"><paramref name="title"/> が空白の場合。</exception>
    string FormatTitle(string title);
}

/// <summary>
/// plain text のレポートタイトルを作成します。
/// </summary>
public sealed class PlainTextReportFormatter : IReportFormatter
{
    /// <inheritdoc />
    public string FormatTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return title.Trim();
    }
}

/// <summary>
/// Markdown のレポートタイトルを作成します。
/// </summary>
public sealed class MarkdownReportFormatter : IReportFormatter
{
    /// <inheritdoc />
    public string FormatTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return $"# {title.Trim()}";
    }
}

/// <summary>
/// アプリケーション単位で共有する ID を提供します。
/// </summary>
public sealed class ApplicationIdProvider
{
    /// <summary>
    /// アプリケーション ID を取得します。
    /// </summary>
    /// <value>プロセス内で共有する ID。</value>
    public Guid ApplicationId { get; } = Guid.NewGuid();
}

/// <summary>
/// リクエストや処理スコープ単位で共有する状態です。
/// </summary>
public sealed class RequestState
{
    /// <summary>
    /// スコープ ID を取得します。
    /// </summary>
    /// <value>DI scope ごとに作成される ID。</value>
    public Guid ScopeId { get; } = Guid.NewGuid();
}

/// <summary>
/// 解決されるたびに新しい操作 ID を作成します。
/// </summary>
public sealed class OperationIdFactory
{
    /// <summary>
    /// 操作 ID を取得します。
    /// </summary>
    /// <value>インスタンスごとに作成される ID。</value>
    public Guid OperationId { get; } = Guid.NewGuid();
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
    /// <exception cref="ArgumentException"><paramref name="prefix"/> が空白の場合。</exception>
    public static IServiceCollection AddGreetingService(this IServiceCollection services, string prefix = "Hello")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        services.AddSingleton<IGreetingService>(_ => new GreetingService(prefix));
        return services;
    }

    /// <summary>
    /// singleton、scoped、transient の lifetime 例を登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddLifetimeExampleServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ApplicationIdProvider>();
        services.AddScoped<RequestState>();
        services.AddTransient<OperationIdFactory>();

        return services;
    }

    /// <summary>
    /// keyed service として複数のレポート整形サービスを登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddReportFormatters(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddKeyedSingleton<IReportFormatter, PlainTextReportFormatter>("plain");
        services.AddKeyedSingleton<IReportFormatter, MarkdownReportFormatter>("markdown");

        return services;
    }

    /// <summary>
    /// テスト用などに <see cref="IGreetingService"/> の登録を差し替えます。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="greetingService">差し替えるサービスインスタンス。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> または <paramref name="greetingService"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection ReplaceGreetingService(
        this IServiceCollection services,
        IGreetingService greetingService)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(greetingService);

        services.RemoveAll<IGreetingService>();
        services.AddSingleton(greetingService);

        return services;
    }
}
