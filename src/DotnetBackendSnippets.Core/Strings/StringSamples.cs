using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理でよく使う実装例を提供します。
/// </summary>
public static partial class StringSamples
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
    /// 文字列を ASCII スラッグに変換します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <returns>URL などで使いやすい ASCII スラッグ。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string ToSlug(string value)
    {
        return ToAsciiSlug(value);
    }

    /// <summary>
    /// 発音記号を除去して ASCII スラッグを作成します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <returns>ASCII 文字だけで構成したスラッグ。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string ToAsciiSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = RemoveDiacritics(value).Trim().ToLower(CultureInfo.InvariantCulture);
        var slug = NonAlphaNumericRegex().Replace(normalized, "-");

        return DuplicateHyphenRegex().Replace(slug, "-").Trim('-');
    }

    /// <summary>
    /// Unicode の文字と数字を残したスラッグを作成します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <returns>Unicode 文字を含められるスラッグ。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string ToUnicodeSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Trim().ToLower(CultureInfo.InvariantCulture);
        var slug = NonLetterOrDigitRegex().Replace(normalized, "-");

        return DuplicateHyphenRegex().Replace(slug, "-").Trim('-');
    }

    /// <summary>
    /// 文字列を指定長に切り詰めます。
    /// </summary>
    /// <param name="value">対象文字列。</param>
    /// <param name="maxLength">最大長。</param>
    /// <param name="suffix">切り詰め時に末尾へ付ける文字列。</param>
    /// <returns>切り詰め後の文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> または <paramref name="suffix"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> が 0 未満の場合。</exception>
    public static string Truncate(string value, int maxLength, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length must be zero or greater.");
        }

        if (value.Length <= maxLength)
        {
            return value;
        }

        if (maxLength == 0)
        {
            return string.Empty;
        }

        if (suffix.Length >= maxLength)
        {
            return suffix[..maxLength];
        }

        return string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);
    }

    /// <summary>
    /// 先頭と末尾を残して中央部分をマスクします。
    /// </summary>
    /// <param name="value">マスクする文字列。</param>
    /// <param name="visibleStart">先頭に残す文字数。</param>
    /// <param name="visibleEnd">末尾に残す文字数。</param>
    /// <param name="mask">マスクに使う文字。</param>
    /// <returns>マスク後の文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="visibleStart"/> または <paramref name="visibleEnd"/> が 0 未満の場合。</exception>
    public static string MaskMiddle(string value, int visibleStart, int visibleEnd, char mask = '*')
    {
        ArgumentNullException.ThrowIfNull(value);

        if (visibleStart < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(visibleStart), "Visible count must be zero or greater.");
        }

        if (visibleEnd < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(visibleEnd), "Visible count must be zero or greater.");
        }

        if (value.Length <= visibleStart + visibleEnd)
        {
            return new string(mask, value.Length);
        }

        var maskedLength = value.Length - visibleStart - visibleEnd;
        var prefix = value[..visibleStart];
        var suffix = visibleEnd == 0 ? string.Empty : value[^visibleEnd..];

        return $"{prefix}{new string(mask, maskedLength)}{suffix}";
    }

    /// <summary>
    /// 改行コードの違いを吸収して行ごとに分割します。
    /// </summary>
    /// <param name="value">分割する文字列。</param>
    /// <param name="removeEmptyLines">空行を除外するかどうか。</param>
    /// <returns>行の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<string> SplitLines(string value, bool removeEmptyLines = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n', removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }

    /// <summary>
    /// キー比較用に空白を正規化し、大文字へ変換します。
    /// </summary>
    /// <param name="value">正規化するキー文字列。</param>
    /// <returns>正規化後のキー。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string NormalizeKey(string value)
    {
        return NormalizeWhitespace(value).ToUpperInvariant();
    }

    private static string RemoveDiacritics(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(capacity: normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex(@"[^\p{L}\p{Nd}]+")]
    private static partial Regex NonLetterOrDigitRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex DuplicateHyphenRegex();
}
