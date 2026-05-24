using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DotnetBackendSnippets.Options;

/// <summary>
/// 通知機能の設定です。
/// </summary>
public sealed class NotificationOptions
{
    /// <summary>
    /// 設定セクション名です。
    /// </summary>
    public const string SectionName = "Notifications";

    /// <summary>
    /// 送信元メールアドレスを取得します。
    /// </summary>
    /// <value>通知メールの From に使うメールアドレス。</value>
    public string SenderEmail { get; init; } = string.Empty;

    /// <summary>
    /// リトライ回数を取得します。
    /// </summary>
    /// <value>通知送信失敗時のリトライ回数。</value>
    public int RetryCount { get; init; }
}

/// <summary>
/// 現在の通知設定を読み取ります。
/// </summary>
public sealed class NotificationOptionsReader
{
    private readonly IOptionsMonitor<NotificationOptions> optionsMonitor;

    /// <summary>
    /// <see cref="NotificationOptionsReader"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="optionsMonitor">設定変更を監視するオプションモニター。</param>
    /// <exception cref="ArgumentNullException"><paramref name="optionsMonitor"/> が <see langword="null"/> の場合。</exception>
    public NotificationOptionsReader(IOptionsMonitor<NotificationOptions> optionsMonitor)
    {
        ArgumentNullException.ThrowIfNull(optionsMonitor);

        this.optionsMonitor = optionsMonitor;
    }

    /// <summary>
    /// 現在の通知設定を取得します。
    /// </summary>
    /// <returns>現在の通知設定。</returns>
    public NotificationOptions GetCurrent()
    {
        return optionsMonitor.CurrentValue;
    }

    /// <summary>
    /// 名前付き通知設定を取得します。
    /// </summary>
    /// <param name="name">名前付き options の名前。</param>
    /// <returns>指定名の通知設定。</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> が空白の場合。</exception>
    public NotificationOptions GetNamed(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return optionsMonitor.Get(name);
    }
}

/// <summary>
/// scoped な処理で通知設定の snapshot を読み取ります。
/// </summary>
public sealed class NotificationOptionsSnapshotReader
{
    private readonly IOptionsSnapshot<NotificationOptions> optionsSnapshot;

    /// <summary>
    /// <see cref="NotificationOptionsSnapshotReader"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="optionsSnapshot">スコープごとの options snapshot。</param>
    /// <exception cref="ArgumentNullException"><paramref name="optionsSnapshot"/> が <see langword="null"/> の場合。</exception>
    public NotificationOptionsSnapshotReader(IOptionsSnapshot<NotificationOptions> optionsSnapshot)
    {
        ArgumentNullException.ThrowIfNull(optionsSnapshot);

        this.optionsSnapshot = optionsSnapshot;
    }

    /// <summary>
    /// 現在スコープの通知設定を取得します。
    /// </summary>
    /// <returns>現在スコープの通知設定。</returns>
    public NotificationOptions GetCurrent()
    {
        return optionsSnapshot.Value;
    }
}

/// <summary>
/// Options パターン登録のサンプルです。
/// </summary>
public static class OptionsSamples
{
    /// <summary>
    /// 通知設定を DI コンテナーに登録して検証します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="configuration">設定ソース。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> または <paramref name="configuration"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddNotificationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<NotificationOptions>()
            .Bind(configuration.GetSection(NotificationOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.SenderEmail), "SenderEmail is required.")
            .Validate(options => options.RetryCount is >= 0 and <= 5, "RetryCount must be between 0 and 5.")
            .ValidateOnStart();

        services.TryAddReaders();

        return services;
    }

    /// <summary>
    /// 名前付き通知設定を DI コンテナーに登録して検証します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="configuration">設定ソース。</param>
    /// <param name="name">名前付き options の名前。</param>
    /// <param name="sectionPath">bind する設定セクションのパス。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> または <paramref name="configuration"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> または <paramref name="sectionPath"/> が空白の場合。</exception>
    public static IServiceCollection AddNamedNotificationOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        string name,
        string sectionPath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionPath);

        services
            .AddOptions<NotificationOptions>(name)
            .Bind(configuration.GetSection(sectionPath))
            .Validate(options => !string.IsNullOrWhiteSpace(options.SenderEmail), "SenderEmail is required.")
            .Validate(options => options.RetryCount is >= 0 and <= 5, "RetryCount must be between 0 and 5.")
            .ValidateOnStart();

        services.TryAddReaders();

        return services;
    }

    private static void TryAddReaders(this IServiceCollection services)
    {
        services.TryAddSingleton<NotificationOptionsReader>();
        services.TryAddScoped<NotificationOptionsSnapshotReader>();
    }
}
