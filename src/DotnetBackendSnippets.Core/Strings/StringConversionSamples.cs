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
    /// <param name="value">スラッグ化する文字列。</param>
    /// <param name="fallback">スラッグ化後に空になる場合の既定値。</param>
    /// <returns>ASCII スラッグ。空になる場合は <paramref name="fallback"/>。</returns>
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
    /// <param name="slug">切り詰めるスラッグ。</param>
    /// <param name="maxLength">最大文字数。</param>
    /// <returns>指定長以内に切り詰め、ASCII スラッグとして正規化した文字列。</returns>
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
    /// <param name="fileName">スラッグ化するファイル名。</param>
    /// <returns>拡張子を除いたファイル名から作成した ASCII スラッグ。</returns>
    public static string FileNameToSlug(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return ToSlugOrDefault(GetFileNameWithoutExtensionPortable(fileName));
    }

    /// <summary>
    /// スラッグに連番サフィックスを付けます。
    /// </summary>
    /// <param name="slug">元のスラッグ。</param>
    /// <param name="suffix">付与する 1 以上の連番。</param>
    /// <returns><paramref name="slug"/> に連番サフィックスを付けた文字列。</returns>
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
    /// <param name="value">判定する文字列。</param>
    /// <param name="fallback">空文字列または <see langword="null"/> の場合に返す文字列。</param>
    /// <returns><paramref name="value"/> が空文字列または <see langword="null"/> の場合は <paramref name="fallback"/>。それ以外は元の文字列。</returns>
    public static string DefaultIfEmpty(string? value, string fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);

        return string.IsNullOrEmpty(value) ? fallback : value;
    }

    /// <summary>
    /// UTF-8 文字列を Base64 へ変換します。
    /// </summary>
    /// <param name="value">Base64 へ変換する文字列。</param>
    /// <returns>UTF-8 バイト列を Base64 表現にした文字列。</returns>
    public static string ToBase64(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// UTF-8 文字列を Base64Url へ変換します。
    /// </summary>
    /// <param name="value">Base64Url へ変換する文字列。</param>
    /// <returns>UTF-8 バイト列を Base64Url 表現にした文字列。</returns>
    public static string ToBase64Url(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return ToBase64UrlString(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Base64Url 文字列を UTF-8 文字列へ戻します。
    /// </summary>
    /// <param name="value">Base64Url 形式の文字列。</param>
    /// <returns>デコードした UTF-8 文字列。</returns>
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
    /// <param name="length">作成するコードの文字数。</param>
    /// <returns>英数字で構成されたランダムコード。</returns>
    public static string CreateRandomCode(int length)
    {
        return CreateRandomFromAlphabet(length, TokenAlphabet);
    }

    /// <summary>
    /// 数字だけのランダムコードを作成します。
    /// </summary>
    /// <param name="length">作成するコードの文字数。</param>
    /// <returns>数字だけで構成されたランダムコード。</returns>
    public static string CreateNumericCode(int length)
    {
        return CreateRandomFromAlphabet(length, "0123456789");
    }

    /// <summary>
    /// URL で扱いやすいランダムトークンを作成します。
    /// </summary>
    /// <param name="byteLength">乱数として生成するバイト数。</param>
    /// <returns>URL で扱いやすい Base64Url 形式のランダムトークン。</returns>
    public static string CreateUrlSafeToken(int byteLength = 32)
    {
        if (byteLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(byteLength), "Byte length must be one or greater.");
        }

        return ToBase64UrlString(RandomNumberGenerator.GetBytes(byteLength));
    }

    /// <summary>
    /// GUID を短い URL 安全文字列へ変換します。
    /// </summary>
    /// <param name="value">変換する GUID。</param>
    /// <returns>GUID バイト列を Base64Url 形式にした短い文字列。</returns>
    public static string ToShortGuid(Guid value)
    {
        return ToBase64UrlString(value.ToByteArray());
    }

    /// <summary>
    /// オブジェクトストレージ用のキーを作成します。
    /// </summary>
    /// <param name="segments">キーを構成するセグメント。</param>
    /// <returns>各セグメントをスラッグ化して <c>/</c> で結合したオブジェクトキー。</returns>
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
    /// 人間が読みやすいキー向けで、区切り文字から元の segment を厳密に復元する用途は想定していません。
    /// </summary>
    /// <param name="segments">キーを構成するセグメント。</param>
    /// <returns>各セグメントを正規化し、空白と segment 境界を <c>:</c> で表したキャッシュキー。</returns>
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
    /// <param name="value">ハッシュ化する文字列。</param>
    /// <returns>SHA-256 ハッシュを小文字の 16 進文字列にしたキー。</returns>
    public static string CreateStableHashKey(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }

    /// <summary>
    /// snake_case を PascalCase に変換します。
    /// </summary>
    /// <param name="value">変換する snake_case 文字列。</param>
    /// <returns>PascalCase に変換した文字列。</returns>
    public static string SnakeToPascalCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return string.Concat(value.Split('_', StringSplitOptions.RemoveEmptyEntries).Select(CapitalizeInvariant));
    }

    /// <summary>
    /// PascalCase を camelCase に変換します。
    /// </summary>
    /// <param name="value">変換する PascalCase 文字列。</param>
    /// <returns>camelCase に変換した文字列。</returns>
    public static string PascalToCamelCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// PascalCase を kebab-case に変換します。
    /// </summary>
    /// <param name="value">変換する PascalCase 文字列。</param>
    /// <returns>kebab-case に変換した文字列。</returns>
    public static string PascalToKebabCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return PascalBoundaryRegex().Replace(value, "$1-$2").ToLowerInvariant();
    }

    /// <summary>
    /// プレースホルダー付きテンプレートを値で置き換えます。
    /// </summary>
    /// <param name="template">置換対象のテンプレート文字列。</param>
    /// <param name="values">プレースホルダー名と置換値の辞書。</param>
    /// <returns>一致したプレースホルダーを辞書の値で置き換えた文字列。</returns>
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
    /// <param name="template">検索対象のテンプレート文字列。</param>
    /// <returns>テンプレート内に含まれる重複なしのプレースホルダー名リスト。</returns>
    public static IReadOnlyList<string> ExtractPlaceholders(string template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return PlaceholderRegex().Matches(template).Select(match => match.Groups["name"].Value).Distinct().ToList();
    }

    /// <summary>
    /// 件数に応じて単純な複数形へ変換します。
    /// </summary>
    /// <param name="singular">単数形の単語。</param>
    /// <param name="count">件数。</param>
    /// <param name="plural">明示的に使用する複数形。未指定の場合は末尾に <c>s</c> を付けます。</param>
    /// <returns><paramref name="count"/> が 1 の場合は単数形、それ以外は複数形。</returns>
    public static string PluralizeSimple(string singular, int count, string? plural = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(singular);

        return count == 1 ? singular : plural ?? singular + "s";
    }

    /// <summary>
    /// バイト数を読みやすい単位付き文字列に整形します。
    /// </summary>
    /// <param name="bytes">整形するバイト数。</param>
    /// <returns>B、KB、MB、GB、TB のいずれかの単位を付けた文字列。</returns>
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
    /// <param name="value">切り詰める文字列。</param>
    /// <param name="maxBytes">許可する UTF-8 バイト数の最大値。</param>
    /// <param name="suffix">切り詰めた場合に末尾へ付ける文字列。</param>
    /// <returns>UTF-8 バイト数が <paramref name="maxBytes"/> 以下になるよう切り詰めた文字列。</returns>
    /// <exception cref="ArgumentException"><paramref name="suffix"/> の UTF-8 バイト数が <paramref name="maxBytes"/> を超える場合。</exception>
    public static string TruncateUtf8Bytes(string value, int maxBytes, string suffix = "")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "Max bytes must be zero or greater.");
        }

        var suffixByteCount = Encoding.UTF8.GetByteCount(suffix);
        if (suffixByteCount > maxBytes)
        {
            throw new ArgumentException("Suffix byte length must be less than or equal to max bytes.", nameof(suffix));
        }

        var builder = new StringBuilder();
        var currentByteCount = 0;

        foreach (var rune in value.EnumerateRunes())
        {
            var runeByteCount = rune.Utf8SequenceLength;
            if (currentByteCount + runeByteCount + suffixByteCount > maxBytes)
            {
                return builder.Append(suffix).ToString();
            }

            builder.Append(rune);
            currentByteCount += runeByteCount;
        }

        return builder.ToString();
    }

    /// <summary>
    /// 環境変数名として使いやすい形式へ変換します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <returns>英数字とアンダースコアで構成された大文字の環境変数名。</returns>
    public static string ToEnvironmentVariableName(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var snake = PascalBoundaryRegex().Replace(value, "$1_$2");

        return NonEnvironmentNameRegex().Replace(snake, "_").Trim('_').ToUpperInvariant();
    }
}
