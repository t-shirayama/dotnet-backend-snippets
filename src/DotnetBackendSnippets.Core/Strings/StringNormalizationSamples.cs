using System.Globalization;
using System.Text;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    /// <summary>
    /// 文字列をトリムし、null の場合は空文字列を返します。
    /// </summary>
    /// <param name="value">トリムする文字列。</param>
    /// <returns>トリム後の文字列。<paramref name="value"/> が <see langword="null"/> の場合は空文字列。</returns>
    public static string TrimOrEmpty(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 通常の空白と全角空白を取り除きます。
    /// </summary>
    /// <param name="value">トリムする文字列。</param>
    /// <returns>通常の空白と全角空白を取り除いた文字列。<paramref name="value"/> が <see langword="null"/> の場合は空文字列。</returns>
    public static string TrimJapaneseWhitespace(string? value)
    {
        return value?.Trim().Trim('\u3000') ?? string.Empty;
    }

    /// <summary>
    /// 文字列を単一行に正規化します。
    /// </summary>
    /// <param name="value">正規化する文字列。</param>
    /// <returns>連続する空白を 1 つの空白にまとめた文字列。</returns>
    public static string NormalizeToSingleLine(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.NormalizeWhitespace(value);
    }

    /// <summary>
    /// 改行コードを指定した改行文字へ統一します。
    /// </summary>
    /// <param name="value">正規化する文字列。</param>
    /// <param name="newline">統一後に使用する改行文字。</param>
    /// <returns>改行コードを <paramref name="newline"/> に統一した文字列。</returns>
    public static string NormalizeLineEndings(string value, string newline = "\n")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(newline);

        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    /// <summary>
    /// 文字列を指定した Unicode 正規化形式へ変換します。
    /// </summary>
    /// <param name="value">正規化する文字列。</param>
    /// <param name="normalizationForm">使用する Unicode 正規化形式。</param>
    /// <returns>指定した Unicode 正規化形式に変換した文字列。</returns>
    public static string NormalizeUnicode(string value, NormalizationForm normalizationForm = NormalizationForm.FormC)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Normalize(normalizationForm);
    }

    /// <summary>
    /// 検索用にダイアクリティカルマークを除去します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <returns>ダイアクリティカルマークを除去した検索向け文字列。</returns>
    public static string RemoveDiacriticsForSearch(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// 連続する区切り文字を 1 つにまとめます。
    /// </summary>
    /// <param name="value">正規化する文字列。</param>
    /// <returns>連続するハイフンまたはアンダースコアを 1 つにまとめ、前後の区切り文字を取り除いた文字列。</returns>
    public static string CollapseSeparators(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SeparatorRegex().Replace(value, "$1").Trim('-', '_');
    }

    /// <summary>
    /// 空白文字列を null に変換します。
    /// </summary>
    /// <param name="value">判定する文字列。</param>
    /// <returns><paramref name="value"/> が空白のみの場合は <see langword="null"/>。それ以外は元の文字列。</returns>
    public static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>
    /// 検索用にかな文字を正規化します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <returns>互換文字を正規化し、カタカナをひらがなへ寄せた文字列。</returns>
    public static string NormalizeKanaForSearch(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Normalize(NormalizationForm.FormKC);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            builder.Append(character is >= '\u30A1' and <= '\u30F6'
                ? (char)(character - 0x60)
                : character);
        }

        return builder.ToString();
    }

    /// <summary>
    /// ファイル名として使いやすい文字列へ変換します。
    /// </summary>
    /// <param name="value">ファイル名に変換する文字列。</param>
    /// <returns>OS 非依存の禁止文字をハイフンへ置き換えた文字列。</returns>
    public static string ToSafeFileName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var invalid = new HashSet<char>(['<', '>', ':', '"', '/', '\\', '|', '?', '*']);
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            builder.Append(invalid.Contains(character) ? '-' : character);
        }

        return CollapseSeparators(builder.ToString().Trim());
    }
}
