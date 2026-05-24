using System.Security.Cryptography;
using System.Text;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    /// <summary>
    /// 文字列を ASCII スラッグへ変換し、空の場合は既定値を返します。
    /// </summary>
    public static string ToSlugOrDefault(string value, string fallback = "item")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(fallback);

        var slug = StringSamples.ToAsciiSlug(value);

        return string.IsNullOrEmpty(slug) ? fallback : slug;
    }

    /// <summary>
    /// スラッグを指定長に切り詰めます。
    /// </summary>
    public static string TruncateSlug(string slug, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(slug);

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length must be zero or greater.");
        }

        return StringSamples.ToAsciiSlug(StringSamples.Truncate(slug, maxLength, suffix: string.Empty));
    }

    /// <summary>
    /// ファイル名から拡張子を除いたスラッグを作成します。
    /// </summary>
    public static string FileNameToSlug(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return ToSlugOrDefault(GetFileNameWithoutExtensionPortable(fileName));
    }

    /// <summary>
    /// スラッグに連番サフィックスを付けます。
    /// </summary>
    public static string AppendSlugSuffix(string slug, int suffix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        if (suffix < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(suffix), "Suffix must be one or greater.");
        }

        return $"{slug}-{suffix}";
    }

    /// <summary>
    /// 空文字列を指定した既定値へ置き換えます。
    /// </summary>
    public static string DefaultIfEmpty(string? value, string fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);

        return string.IsNullOrEmpty(value) ? fallback : value;
    }

    /// <summary>
    /// UTF-8 文字列を Base64 へ変換します。
    /// </summary>
    public static string ToBase64(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// UTF-8 文字列を Base64Url へ変換します。
    /// </summary>
    public static string ToBase64Url(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// Base64Url 文字列を UTF-8 文字列へ戻します。
    /// </summary>
    public static string FromBase64Url(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var base64 = value.Replace('-', '+').Replace('_', '/');
        var padding = (4 - base64.Length % 4) % 4;
        base64 = base64.PadRight(base64.Length + padding, '=');

        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }

    /// <summary>
    /// 英数字のランダムコードを作成します。
    /// </summary>
    public static string CreateRandomCode(int length)
    {
        return CreateRandomFromAlphabet(length, TokenAlphabet);
    }

    /// <summary>
    /// 数字だけのランダムコードを作成します。
    /// </summary>
    public static string CreateNumericCode(int length)
    {
        return CreateRandomFromAlphabet(length, "0123456789");
    }

    /// <summary>
    /// URL で扱いやすいランダムトークンを作成します。
    /// </summary>
    public static string CreateUrlSafeToken(int byteLength = 32)
    {
        if (byteLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(byteLength), "Byte length must be one or greater.");
        }

        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// GUID を短い URL 安全文字列へ変換します。
    /// </summary>
    public static string ToShortGuid(Guid value)
    {
        return Convert.ToBase64String(value.ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// オブジェクトストレージ用のキーを作成します。
    /// </summary>
    public static string BuildObjectKey(params string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        var normalizedSegments = new List<string>(segments.Length);

        for (var index = 0; index < segments.Length; index++)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(segments[index], $"segments[{index}]");

            var normalizedSegment = StringSamples.ToUnicodeSlug(segments[index]);
            if (string.IsNullOrEmpty(normalizedSegment))
            {
                throw new ArgumentException("Object key segments must contain at least one letter or digit.", nameof(segments));
            }

            normalizedSegments.Add(normalizedSegment);
        }

        return string.Join('/', normalizedSegments);
    }

    /// <summary>
    /// キャッシュキー用の文字列を作成します。
    /// </summary>
    public static string BuildCacheKey(params string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        var normalizedSegments = new List<string>(segments.Length);

        for (var index = 0; index < segments.Length; index++)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(segments[index], $"segments[{index}]");

            normalizedSegments.Add(StringSamples.NormalizeKey(segments[index]).Replace(' ', ':').ToLowerInvariant());
        }

        return string.Join(':', normalizedSegments);
    }

    /// <summary>
    /// 文字列から安定した SHA-256 ハッシュキーを作成します。
    /// </summary>
    public static string CreateStableHashKey(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }

    /// <summary>
    /// snake_case を PascalCase に変換します。
    /// </summary>
    public static string SnakeToPascalCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return string.Concat(value.Split('_', StringSplitOptions.RemoveEmptyEntries).Select(CapitalizeInvariant));
    }

    /// <summary>
    /// PascalCase を camelCase に変換します。
    /// </summary>
    public static string PascalToCamelCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// PascalCase を kebab-case に変換します。
    /// </summary>
    public static string PascalToKebabCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return PascalBoundaryRegex().Replace(value, "$1-$2").ToLowerInvariant();
    }

    /// <summary>
    /// プレースホルダー付きテンプレートを値で置き換えます。
    /// </summary>
    public static string RenderTemplate(string template, IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(values);

        return PlaceholderRegex().Replace(template, match =>
            values.TryGetValue(match.Groups["name"].Value, out var value) ? value : match.Value);
    }

    /// <summary>
    /// テンプレート内のプレースホルダー名を抽出します。
    /// </summary>
    public static IReadOnlyList<string> ExtractPlaceholders(string template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return PlaceholderRegex().Matches(template).Select(match => match.Groups["name"].Value).Distinct().ToList();
    }

    /// <summary>
    /// 件数に応じて単純な複数形へ変換します。
    /// </summary>
    public static string PluralizeSimple(string singular, int count, string? plural = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(singular);

        return count == 1 ? singular : plural ?? singular + "s";
    }

    /// <summary>
    /// バイト数を読みやすい単位付き文字列に整形します。
    /// </summary>
    public static string FormatBytes(long bytes)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes must be zero or greater.");
        }

        string[] units = ["B", "KB", "MB", "GB", "TB"];
        var value = (decimal)bytes;
        var unitIndex = 0;

        while (value >= 1024m && unitIndex < units.Length - 1)
        {
            value /= 1024m;
            unitIndex++;
        }

        return $"{value:0.##} {units[unitIndex]}";
    }

    /// <summary>
    /// UTF-8 バイト数を超えないよう文字列を切り詰めます。
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="suffix"/> の UTF-8 バイト数が <paramref name="maxBytes"/> を超える場合。</exception>
    public static string TruncateUtf8Bytes(string value, int maxBytes, string suffix = "")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "Max bytes must be zero or greater.");
        }

        if (Encoding.UTF8.GetByteCount(suffix) > maxBytes)
        {
            throw new ArgumentException("Suffix byte length must be less than or equal to max bytes.", nameof(suffix));
        }

        var builder = new StringBuilder();
        foreach (var rune in value.EnumerateRunes())
        {
            var candidate = builder.ToString() + rune + suffix;
            if (Encoding.UTF8.GetByteCount(candidate) > maxBytes)
            {
                return builder + suffix;
            }

            builder.Append(rune);
        }

        return builder.ToString();
    }

    /// <summary>
    /// 環境変数名として使いやすい形式へ変換します。
    /// </summary>
    public static string ToEnvironmentVariableName(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var snake = PascalBoundaryRegex().Replace(value, "$1_$2");

        return NonEnvironmentNameRegex().Replace(snake, "_").Trim('_').ToUpperInvariant();
    }
}
