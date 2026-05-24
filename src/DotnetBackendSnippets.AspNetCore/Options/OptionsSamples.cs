using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
/// <param name="optionsMonitor">設定変更を監視するオプションモニター。</param>
/// <exception cref="ArgumentNullException"><paramref name="optionsMonitor"/> が <see langword="null"/> の場合。</exception>
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

        services.AddSingleton<NotificationOptionsReader>();

        return services;
    }
}
