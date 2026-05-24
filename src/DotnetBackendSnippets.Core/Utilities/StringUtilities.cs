using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Utilities;

/// <summary>
/// 汎用的な文字列ユーティリティを提供します。
/// </summary>
public static partial class StringUtilities
{
    /// <summary>
    /// 連続する空白を 1 つの半角スペースに正規化します。
    /// </summary>
    /// <param name="value">正規化する文字列。</param>
    /// <returns>前後の空白を除去し、内部空白をまとめた文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string NormalizeWhitespace(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WhitespaceRegex().Replace(value.Trim(), " ");
    }

    /// <summary>
    /// 文字列が空でないことを検証して返します。
    /// </summary>
    /// <param name="value">検証する文字列。</param>
    /// <param name="parameterName">例外に設定するパラメーター名。</param>
    /// <returns>空でない文字列。</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> が null、空文字、または空白のみの場合。</exception>
    public static string RequireNonEmpty(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
