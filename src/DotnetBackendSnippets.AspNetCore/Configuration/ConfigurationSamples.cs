using Microsoft.Extensions.Configuration;

namespace DotnetBackendSnippets.Configuration;

/// <summary>
/// アプリケーション設定を表します。
/// </summary>
/// <param name="ServiceName">サービス名。</param>
/// <param name="RetryCount">リトライ回数。</param>
public sealed record AppSettings(string ServiceName, int RetryCount);

/// <summary>
/// 設定値の読み取りサンプルです。
/// </summary>
public static class ConfigurationSamples
{
    /// <summary>
    /// 必須の設定値を取得します。
    /// </summary>
    /// <param name="configuration">設定ソース。</param>
    /// <param name="key">取得するキー。</param>
    /// <returns>空ではない設定値。</returns>
    /// <exception cref="InvalidOperationException">設定値が未指定の場合。</exception>
    public static string GetRequiredValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' is required.");
        }

        return value;
    }

    /// <summary>
    /// App セクションからアプリケーション設定を読み取ります。
    /// </summary>
    /// <param name="configuration">設定ソース。</param>
    /// <returns>読み取ったアプリケーション設定。</returns>
    /// <exception cref="InvalidOperationException">必須値がない、または形式が不正な場合。</exception>
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
