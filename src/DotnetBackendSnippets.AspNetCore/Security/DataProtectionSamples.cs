using Microsoft.AspNetCore.DataProtection;

namespace DotnetBackendSnippets.Security;

/// <summary>
/// secret store から秘密情報を読むための抽象化です。
/// </summary>
public interface ISecretProvider
{
    /// <summary>
    /// 指定名の secret を取得します。
    /// </summary>
    /// <param name="name">secret 名。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>secret 値。</returns>
    Task<string?> GetSecretAsync(string name, CancellationToken cancellationToken = default);
}

/// <summary>
/// ASP.NET Core Data Protection と秘密情報管理のサンプルです。
/// </summary>
public static class DataProtectionSamples
{
    /// <summary>
    /// purpose を指定して保護用 protector を作成します。
    /// </summary>
    /// <param name="provider">Data Protection provider。</param>
    /// <param name="purpose">用途。</param>
    /// <returns>用途で分離された protector。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="purpose"/> が空白の場合。</exception>
    public static IDataProtector CreateProtector(IDataProtectionProvider provider, string purpose)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);

        return provider.CreateProtector(purpose);
    }

    /// <summary>
    /// Data Protection で文字列を保護します。
    /// </summary>
    /// <param name="protector">protector。</param>
    /// <param name="value">保護する値。</param>
    /// <returns>保護済み文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="protector"/> または <paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string ProtectString(IDataProtector protector, string value)
    {
        ArgumentNullException.ThrowIfNull(protector);
        ArgumentNullException.ThrowIfNull(value);

        return protector.Protect(value);
    }

    /// <summary>
    /// Data Protection で保護された文字列を復元します。
    /// </summary>
    /// <param name="protector">protector。</param>
    /// <param name="protectedValue">保護済み文字列。</param>
    /// <returns>復元した文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="protector"/> または <paramref name="protectedValue"/> が <see langword="null"/> の場合。</exception>
    public static string UnprotectString(IDataProtector protector, string protectedValue)
    {
        ArgumentNullException.ThrowIfNull(protector);
        ArgumentNullException.ThrowIfNull(protectedValue);

        return protector.Unprotect(protectedValue);
    }

    /// <summary>
    /// secret provider から必須 secret を取得します。
    /// </summary>
    /// <param name="secretProvider">secret provider。</param>
    /// <param name="name">secret 名。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>secret 値。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="secretProvider"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> が空白の場合。</exception>
    /// <exception cref="InvalidOperationException">secret が未設定の場合。</exception>
    public static async Task<string> GetRequiredSecretAsync(
        ISecretProvider secretProvider,
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(secretProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string? value = await secretProvider.GetSecretAsync(name, cancellationToken);

        return string.IsNullOrWhiteSpace(value)
            ? throw new InvalidOperationException($"Secret '{name}' is required.")
            : value;
    }
}
