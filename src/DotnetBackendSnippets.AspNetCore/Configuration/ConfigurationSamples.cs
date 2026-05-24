using Microsoft.Extensions.Configuration;

namespace DotnetBackendSnippets.Configuration;

/// <summary>
/// アプリケーション設定を表します。
/// </summary>
/// <param name="ServiceName">サービス名。</param>
/// <param name="RetryCount">リトライ回数。</param>
public sealed record AppSettings(string ServiceName, int RetryCount);

/// <summary>
/// 環境別設定ファイル名を表します。
/// </summary>
/// <param name="BaseFile">共通設定ファイル。</param>
/// <param name="EnvironmentFile">環境別設定ファイル。</param>
public sealed record EnvironmentSettingsFiles(string BaseFile, string EnvironmentFile);

/// <summary>
/// 設定値の読み取りサンプルです。
/// </summary>
public static class ConfigurationSamples
{
    private static readonly string[] SecretKeyFragments =
    [
        "apikey",
        "connectionstring",
        "password",
        "privatekey",
        "secret",
        "token",
    ];

    /// <summary>
    /// 必須の設定値を取得します。
    /// </summary>
    /// <param name="configuration">設定ソース。</param>
    /// <param name="key">取得するキー。</param>
    /// <returns>空ではない設定値。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configuration"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> が空白の場合。</exception>
    /// <exception cref="InvalidOperationException">設定値が未指定の場合。</exception>
    public static string GetRequiredValue(IConfiguration configuration, string key)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

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
    /// <exception cref="ArgumentNullException"><paramref name="configuration"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="InvalidOperationException">必須値がない、または形式が不正な場合。</exception>
    public static AppSettings ReadAppSettings(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection("App");
        var serviceName = GetRequiredValue(section, "ServiceName");
        var retryCountValue = GetRequiredValue(section, "RetryCount");

        if (!int.TryParse(retryCountValue, out var retryCount) || retryCount < 0)
        {
            throw new InvalidOperationException("Configuration value 'App:RetryCount' must be a non-negative integer.");
        }

        return new AppSettings(serviceName, retryCount);
    }

    /// <summary>
    /// 環境別 appsettings ファイル名を作成します。
    /// </summary>
    /// <param name="environmentName">環境名。</param>
    /// <returns>共通設定ファイルと環境別設定ファイル。</returns>
    /// <exception cref="ArgumentException"><paramref name="environmentName"/> が空白の場合。</exception>
    public static EnvironmentSettingsFiles CreateEnvironmentSettingsFiles(string environmentName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environmentName);

        return new EnvironmentSettingsFiles("appsettings.json", $"appsettings.{environmentName.Trim()}.json");
    }

    /// <summary>
    /// 環境変数 provider 用の prefix を作成します。
    /// </summary>
    /// <param name="applicationName">アプリケーション名。</param>
    /// <returns><c>APPNAME_</c> 形式の prefix。</returns>
    /// <exception cref="ArgumentException"><paramref name="applicationName"/> が空白の場合。</exception>
    public static string CreateEnvironmentVariablePrefix(string applicationName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationName);

        string normalized = new(applicationName
            .Where(char.IsAsciiLetterOrDigit)
            .Select(char.ToUpperInvariant)
            .ToArray());

        return string.IsNullOrWhiteSpace(normalized)
            ? throw new ArgumentException("Application name must contain at least one ASCII letter or digit.", nameof(applicationName))
            : $"{normalized}_";
    }

    /// <summary>
    /// 必須の connection string を取得します。
    /// </summary>
    /// <param name="configuration">設定ソース。</param>
    /// <param name="name">connection string 名。</param>
    /// <returns>connection string。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configuration"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> が空白の場合。</exception>
    /// <exception cref="InvalidOperationException">connection string が未指定の場合。</exception>
    public static string GetRequiredConnectionString(IConfiguration configuration, string name)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string? value = configuration.GetConnectionString(name);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Connection string '{name}' is required.");
        }

        return value;
    }

    /// <summary>
    /// ログ出力前に secret らしい設定値をマスクします。
    /// </summary>
    /// <param name="settings">設定キーと値。</param>
    /// <returns>secret らしい値をマスクした設定一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="settings"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyDictionary<string, string?> RedactSecretSettings(IReadOnlyDictionary<string, string?> settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return settings.ToDictionary(
            pair => pair.Key,
            pair => IsSecretKey(pair.Key) && !string.IsNullOrEmpty(pair.Value) ? "***" : pair.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsSecretKey(string key)
    {
        string normalized = new(key
            .Where(char.IsAsciiLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray());

        return SecretKeyFragments.Any(fragment => normalized.Contains(fragment, StringComparison.Ordinal));
    }
}
