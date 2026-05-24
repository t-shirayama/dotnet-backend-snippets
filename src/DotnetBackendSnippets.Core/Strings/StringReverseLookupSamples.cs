using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    private const string TokenAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// 文字列をトリムし、null の場合は空文字列を返します。
    /// </summary>
    public static string TrimOrEmpty(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 通常の空白と全角空白を取り除きます。
    /// </summary>
    public static string TrimJapaneseWhitespace(string? value)
    {
        return value?.Trim().Trim('\u3000') ?? string.Empty;
    }

    /// <summary>
    /// 文字列を単一行に正規化します。
    /// </summary>
    public static string NormalizeToSingleLine(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.NormalizeWhitespace(value);
    }

    /// <summary>
    /// 改行コードを指定した改行文字へ統一します。
    /// </summary>
    public static string NormalizeLineEndings(string value, string newline = "\n")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(newline);

        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    /// <summary>
    /// 文字列を指定した Unicode 正規化形式へ変換します。
    /// </summary>
    public static string NormalizeUnicode(string value, NormalizationForm normalizationForm = NormalizationForm.FormC)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Normalize(normalizationForm);
    }

    /// <summary>
    /// 検索用にダイアクリティカルマークを除去します。
    /// </summary>
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
    public static string CollapseSeparators(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SeparatorRegex().Replace(value, "$1").Trim('-', '_');
    }

    /// <summary>
    /// 空白文字列を null に変換します。
    /// </summary>
    public static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

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
    /// URL パスセグメントとして安全にエンコードします。
    /// </summary>
    public static string EncodePathSegment(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Uri.EscapeDataString(value);
    }

    /// <summary>
    /// URL クエリ値として安全にエンコードします。
    /// </summary>
    public static string EncodeQueryValue(string value)
    {
        return UrlEncode(value);
    }

    /// <summary>
    /// ファイル名から拡張子を除いたスラッグを作成します。
    /// </summary>
    public static string FileNameToSlug(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return ToSlugOrDefault(Path.GetFileNameWithoutExtension(fileName));
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
    /// メールアドレスのローカル部をマスクします。
    /// </summary>
    public static string MaskEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        var atIndex = email.IndexOf('@', StringComparison.Ordinal);
        if (atIndex <= 0)
        {
            return StringSamples.MaskMiddle(email, 1, 1);
        }

        var localPart = email[..atIndex];
        var domain = email[atIndex..];

        return $"{StringSamples.MaskMiddle(localPart, 1, localPart.Length > 2 ? 1 : 0)}{domain}";
    }

    /// <summary>
    /// 電話番号の数字部分をマスクします。
    /// </summary>
    public static string MaskPhoneNumber(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var digits = ExtractDigits(value);

        return StringSamples.MaskMiddle(digits, 0, Math.Min(4, digits.Length));
    }

    /// <summary>
    /// カード番号を下 4 桁相当だけ残してマスクします。
    /// </summary>
    public static string MaskCardNumber(string value)
    {
        return MaskPhoneNumber(value);
    }

    /// <summary>
    /// 結合文字を考慮して文字列を切り詰めます。
    /// </summary>
    public static string TruncateTextElements(string value, int maxTextElements, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxTextElements < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxTextElements), "Max text elements must be zero or greater.");
        }

        var elementIndexes = StringInfo.ParseCombiningCharacters(value);
        if (elementIndexes.Length <= maxTextElements)
        {
            return value;
        }

        if (maxTextElements == 0)
        {
            return string.Empty;
        }

        var cutIndex = elementIndexes[maxTextElements];

        return value[..cutIndex] + suffix;
    }

    /// <summary>
    /// 秘密情報らしい代入値を伏せ字にします。
    /// </summary>
    public static string RedactSecrets(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SecretAssignmentRegex().Replace(value, match => $"{match.Groups[1].Value}=***");
    }

    /// <summary>
    /// 指定した JSON フィールドの値をマスクします。
    /// </summary>
    public static string MaskJsonFields(string json, ISet<string> fieldNames)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(fieldNames);

        using var document = JsonDocument.Parse(json);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteMaskedJson(document.RootElement, writer, fieldNames);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// 表示幅を考慮して文字列を切り詰めます。
    /// </summary>
    public static string TruncateByDisplayWidth(string value, int maxWidth, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxWidth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "Max width must be zero or greater.");
        }

        var builder = new StringBuilder();
        var currentWidth = 0;

        foreach (var rune in value.EnumerateRunes())
        {
            var width = rune.Value <= 0x7F ? 1 : 2;
            if (currentWidth + width > maxWidth)
            {
                return builder.Append(suffix).ToString();
            }

            builder.Append(rune);
            currentWidth += width;
        }

        return builder.ToString();
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
    /// カンマ区切りの値をトリムして分割します。
    /// </summary>
    public static IReadOnlyList<string> SplitCsvLikeValues(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// キーと値の行を辞書へ変換します。
    /// </summary>
    public static IReadOnlyDictionary<string, string> ParseKeyValueLines(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.SplitLines(value, removeEmptyLines: true)
            .Select(line => line.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 最初の区切り文字より前の部分を取得します。
    /// </summary>
    public static string Before(string value, string separator)
    {
        return SplitAround(value, separator).Before;
    }

    /// <summary>
    /// 最初の区切り文字より後の部分を取得します。
    /// </summary>
    public static string After(string value, string separator)
    {
        return SplitAround(value, separator).After;
    }

    /// <summary>
    /// 最後の区切り文字より前の部分を取得します。
    /// </summary>
    public static string BeforeLast(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.LastIndexOf(separator, StringComparison.Ordinal);

        return index < 0 ? value : value[..index];
    }

    /// <summary>
    /// 最後の区切り文字より後の部分を取得します。
    /// </summary>
    public static string AfterLast(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.LastIndexOf(separator, StringComparison.Ordinal);

        return index < 0 ? string.Empty : value[(index + separator.Length)..];
    }

    /// <summary>
    /// 文字列から数字だけを抽出します。
    /// </summary>
    public static string ExtractDigits(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DigitsRegex().Replace(value, string.Empty);
    }

    /// <summary>
    /// ログ行から correlation id を抽出します。
    /// </summary>
    public static string? ExtractCorrelationId(string logLine)
    {
        ArgumentNullException.ThrowIfNull(logLine);

        var match = CorrelationIdRegex().Match(logLine);

        return match.Success ? match.Groups["value"].Value : null;
    }

    /// <summary>
    /// 大文字小文字を無視して部分一致を判定します。
    /// </summary>
    public static bool ContainsIgnoreCase(string value, string search)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(search);

        return value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 大文字小文字を無視して前方一致を判定します。
    /// </summary>
    public static bool StartsWithIgnoreCase(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 大文字小文字を無視して後方一致を判定します。
    /// </summary>
    public static bool EndsWithIgnoreCase(string value, string suffix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// OrdinalIgnoreCase で文字列の等価性を判定します。
    /// </summary>
    public static bool EqualsOrdinalIgnoreCase(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 検索用にかな文字を正規化します。
    /// </summary>
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
    /// すべてのキーワードを含むかを判定します。
    /// </summary>
    public static bool ContainsAllKeywords(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.All(keyword => ContainsIgnoreCase(value, keyword));
    }

    /// <summary>
    /// いずれかのキーワードを含むかを判定します。
    /// </summary>
    public static bool ContainsAnyKeyword(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.Any(keyword => ContainsIgnoreCase(value, keyword));
    }

    /// <summary>
    /// 単語単位で含まれているかを判定します。
    /// </summary>
    public static bool ContainsWholeWord(string value, string word)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        return Regex.IsMatch(value, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    /// <summary>
    /// 正規表現パターンとして安全にエスケープします。
    /// </summary>
    public static string EscapeRegexPattern(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Regex.Escape(value);
    }

    /// <summary>
    /// HTML として安全にエンコードします。
    /// </summary>
    public static string HtmlEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.HtmlEncode(value);
    }

    /// <summary>
    /// JavaScript 文字列として安全にエンコードします。
    /// </summary>
    public static string JavaScriptStringEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return JavaScriptEncoder.Default.Encode(value);
    }

    /// <summary>
    /// URL 用に文字列をエンコードします。
    /// </summary>
    public static string UrlEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.UrlEncode(value);
    }

    /// <summary>
    /// URL エンコードされた文字列をデコードします。
    /// </summary>
    public static string UrlDecode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.UrlDecode(value);
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
    /// 正規表現用のエスケープ処理を行います。
    /// </summary>
    public static string RegexEscape(string value)
    {
        return EscapeRegexPattern(value);
    }

    /// <summary>
    /// SQL LIKE パターン用にワイルドカードをエスケープします。
    /// </summary>
    public static string EscapeSqlLikePattern(string value, char escapeCharacter = '\\')
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace(escapeCharacter.ToString(), new string(escapeCharacter, 2), StringComparison.Ordinal)
            .Replace("%", $"{escapeCharacter}%", StringComparison.Ordinal)
            .Replace("_", $"{escapeCharacter}_", StringComparison.Ordinal)
            .Replace("[", $"{escapeCharacter}[", StringComparison.Ordinal);
    }

    /// <summary>
    /// 制御文字をログなどで見える形にエスケープします。
    /// </summary>
    public static string EscapeControlCharacters(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);
    }

    /// <summary>
    /// CSV フィールドとして必要な場合にクォートします。
    /// </summary>
    public static string EscapeCsvField(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.IndexOfAny([',', '"', '\r', '\n']) < 0)
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    /// <summary>
    /// 値を CSV 行として結合します。
    /// </summary>
    public static string JoinCsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join(",", values.Select(EscapeCsvField));
    }

    /// <summary>
    /// 値を TSV 行として結合します。
    /// </summary>
    public static string JoinTsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join('\t', values.Select(value => value.Replace("\t", " ", StringComparison.Ordinal)));
    }

    /// <summary>
    /// 単一行ログ向けに制御文字を無害化します。
    /// </summary>
    public static string SanitizeForSingleLineLog(string value)
    {
        return EscapeControlCharacters(value);
    }

    /// <summary>
    /// 個人情報らしい文字列を伏せ字にします。
    /// </summary>
    public static string RedactPersonalData(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().Replace(value, "***@***");
    }

    /// <summary>
    /// 各行の先頭に空白インデントを付けます。
    /// </summary>
    public static string IndentLines(string value, int spaces)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (spaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(spaces), "Spaces must be zero or greater.");
        }

        var prefix = new string(' ', spaces);

        return string.Join('\n', StringSamples.SplitLines(value).Select(line => prefix + line));
    }

    /// <summary>
    /// 各行の先頭に指定プレフィックスを付けます。
    /// </summary>
    public static string PrefixLines(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return string.Join('\n', StringSamples.SplitLines(value).Select(line => prefix + line));
    }

    /// <summary>
    /// 必要な場合だけ左側にパディングします。
    /// </summary>
    public static string PadLeftSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadLeft(totalWidth);
    }

    /// <summary>
    /// 必要な場合だけ右側にパディングします。
    /// </summary>
    public static string PadRightSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadRight(totalWidth);
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
    /// ファイル名として使いやすい文字列へ変換します。
    /// </summary>
    public static string ToSafeFileName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var invalid = Path.GetInvalidFileNameChars().ToHashSet();
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            builder.Append(invalid.Contains(character) ? '-' : character);
        }

        return CollapseSeparators(builder.ToString().Trim());
    }

    /// <summary>
    /// オブジェクトストレージ用のキーを作成します。
    /// </summary>
    public static string BuildObjectKey(params string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        return string.Join('/', segments.Select(segment => StringSamples.ToUnicodeSlug(segment)));
    }

    /// <summary>
    /// キャッシュキー用の文字列を作成します。
    /// </summary>
    public static string BuildCacheKey(params string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        return string.Join(':', segments.Select(segment => StringSamples.NormalizeKey(segment).Replace(' ', ':').ToLowerInvariant()));
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
    /// 文字列が null または空白かを判定します。
    /// </summary>
    public static bool IsBlank(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// メールアドレスらしい形式かを判定します。
    /// </summary>
    public static bool IsEmailLike(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().IsMatch(value);
    }

    /// <summary>
    /// 絶対 URL として有効かを判定します。
    /// </summary>
    public static bool IsAbsoluteUrl(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    /// <summary>
    /// HTTPS の絶対 URL かを判定します。
    /// </summary>
    public static bool IsHttpsUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
    }

    /// <summary>
    /// GUID として解析できるかを判定します。
    /// </summary>
    public static bool IsGuid(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Guid.TryParse(value, out _);
    }

    /// <summary>
    /// 数字だけで構成されているかを判定します。
    /// </summary>
    public static bool IsDigitsOnly(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length > 0 && value.All(char.IsDigit);
    }

    /// <summary>
    /// ASCII スラッグ形式かを判定します。
    /// </summary>
    public static bool IsAsciiSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return AsciiSlugRegex().IsMatch(value);
    }

    /// <summary>
    /// 許可された拡張子かを判定します。
    /// </summary>
    public static bool HasAllowedExtension(string fileName, ISet<string> allowedExtensions)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(allowedExtensions);

        return allowedExtensions.Contains(Path.GetExtension(fileName));
    }

    /// <summary>
    /// 禁止語を含むかを判定します。
    /// </summary>
    public static bool ContainsBlockedWord(string value, ISet<string> blockedWords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(blockedWords);

        return blockedWords.Any(word => ContainsWholeWord(value, word));
    }

    /// <summary>
    /// 文字列が最大長以内かを検証します。
    /// </summary>
    public static void ValidateMaxLength(string value, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length must be zero or greater.");
        }

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"Value must be {maxLength} characters or fewer.", nameof(value));
        }
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
    public static string TruncateUtf8Bytes(string value, int maxBytes, string suffix = "")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxBytes), "Max bytes must be zero or greater.");
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

    private static (string Before, string After) SplitAround(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.IndexOf(separator, StringComparison.Ordinal);

        return index < 0
            ? (value, string.Empty)
            : (value[..index], value[(index + separator.Length)..]);
    }

    private static string CreateRandomFromAlphabet(int length, string alphabet)
    {
        if (length < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be one or greater.");
        }

        var bytes = RandomNumberGenerator.GetBytes(length);
        var builder = new StringBuilder(length);

        foreach (var value in bytes)
        {
            builder.Append(alphabet[value % alphabet.Length]);
        }

        return builder.ToString();
    }

    private static string CapitalizeInvariant(string value)
    {
        return value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
    }

    private static void WriteMaskedJson(JsonElement element, Utf8JsonWriter writer, ISet<string> fieldNames)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    if (fieldNames.Contains(property.Name))
                    {
                        writer.WriteStringValue("***");
                    }
                    else
                    {
                        WriteMaskedJson(property.Value, writer, fieldNames);
                    }
                }

                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteMaskedJson(item, writer, fieldNames);
                }

                writer.WriteEndArray();
                break;

            default:
                element.WriteTo(writer);
                break;
        }
    }

    [GeneratedRegex(@"([_-])\1+")]
    private static partial Regex SeparatorRegex();

    [GeneratedRegex(@"(?i)\b(api[_-]?key|access[_-]?token|password|secret)\s*=\s*[^\s;]+")]
    private static partial Regex SecretAssignmentRegex();

    [GeneratedRegex("[^0-9]")]
    private static partial Regex DigitsRegex();

    [GeneratedRegex(@"(?i)\bcorrelation[-_ ]?id[=: ]+(?<value>[a-z0-9-]+)")]
    private static partial Regex CorrelationIdRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex AsciiSlugRegex();

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex PascalBoundaryRegex();

    [GeneratedRegex(@"\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\}")]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex("[^A-Za-z0-9]+")]
    private static partial Regex NonEnvironmentNameRegex();
}
