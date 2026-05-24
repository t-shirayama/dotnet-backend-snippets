using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Strings;

public static partial class StringReverseLookupSamples
{
    private const string TokenAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string TrimOrEmpty(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    public static string TrimJapaneseWhitespace(string? value)
    {
        return value?.Trim().Trim('\u3000') ?? string.Empty;
    }

    public static string NormalizeToSingleLine(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.NormalizeWhitespace(value);
    }

    public static string NormalizeLineEndings(string value, string newline = "\n")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(newline);

        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    public static string NormalizeUnicode(string value, NormalizationForm normalizationForm = NormalizationForm.FormC)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Normalize(normalizationForm);
    }

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

    public static string CollapseSeparators(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SeparatorRegex().Replace(value, "$1").Trim('-', '_');
    }

    public static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static string ToSlugOrDefault(string value, string fallback = "item")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(fallback);

        var slug = StringSamples.ToAsciiSlug(value);

        return string.IsNullOrEmpty(slug) ? fallback : slug;
    }

    public static string TruncateSlug(string slug, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(slug);

        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Max length must be zero or greater.");
        }

        return StringSamples.ToAsciiSlug(StringSamples.Truncate(slug, maxLength, suffix: string.Empty));
    }

    public static string EncodePathSegment(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Uri.EscapeDataString(value);
    }

    public static string EncodeQueryValue(string value)
    {
        return UrlEncode(value);
    }

    public static string FileNameToSlug(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return ToSlugOrDefault(Path.GetFileNameWithoutExtension(fileName));
    }

    public static string AppendSlugSuffix(string slug, int suffix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        if (suffix < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(suffix), "Suffix must be one or greater.");
        }

        return $"{slug}-{suffix}";
    }

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

    public static string MaskPhoneNumber(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var digits = ExtractDigits(value);

        return StringSamples.MaskMiddle(digits, 0, Math.Min(4, digits.Length));
    }

    public static string MaskCardNumber(string value)
    {
        return MaskPhoneNumber(value);
    }

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

    public static string RedactSecrets(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SecretAssignmentRegex().Replace(value, match => $"{match.Groups[1].Value}=***");
    }

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

    public static string DefaultIfEmpty(string? value, string fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);

        return string.IsNullOrEmpty(value) ? fallback : value;
    }

    public static IReadOnlyList<string> SplitCsvLikeValues(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    public static IReadOnlyDictionary<string, string> ParseKeyValueLines(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return StringSamples.SplitLines(value, removeEmptyLines: true)
            .Select(line => line.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.OrdinalIgnoreCase);
    }

    public static string Before(string value, string separator)
    {
        return SplitAround(value, separator).Before;
    }

    public static string After(string value, string separator)
    {
        return SplitAround(value, separator).After;
    }

    public static string BeforeLast(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.LastIndexOf(separator, StringComparison.Ordinal);

        return index < 0 ? value : value[..index];
    }

    public static string AfterLast(string value, string separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrEmpty(separator);

        var index = value.LastIndexOf(separator, StringComparison.Ordinal);

        return index < 0 ? string.Empty : value[(index + separator.Length)..];
    }

    public static string ExtractDigits(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DigitsRegex().Replace(value, string.Empty);
    }

    public static string? ExtractCorrelationId(string logLine)
    {
        ArgumentNullException.ThrowIfNull(logLine);

        var match = CorrelationIdRegex().Match(logLine);

        return match.Success ? match.Groups["value"].Value : null;
    }

    public static bool ContainsIgnoreCase(string value, string search)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(search);

        return value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    public static bool StartsWithIgnoreCase(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    public static bool EndsWithIgnoreCase(string value, string suffix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    public static bool EqualsOrdinalIgnoreCase(string? left, string? right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

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

    public static bool ContainsAllKeywords(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.All(keyword => ContainsIgnoreCase(value, keyword));
    }

    public static bool ContainsAnyKeyword(string value, IEnumerable<string> keywords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(keywords);

        return keywords.Any(keyword => ContainsIgnoreCase(value, keyword));
    }

    public static bool ContainsWholeWord(string value, string word)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(word);

        return Regex.IsMatch(value, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    public static string EscapeRegexPattern(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Regex.Escape(value);
    }

    public static string HtmlEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.HtmlEncode(value);
    }

    public static string JavaScriptStringEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return JavaScriptEncoder.Default.Encode(value);
    }

    public static string UrlEncode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.UrlEncode(value);
    }

    public static string UrlDecode(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WebUtility.UrlDecode(value);
    }

    public static string ToBase64(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    public static string ToBase64Url(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static string FromBase64Url(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var base64 = value.Replace('-', '+').Replace('_', '/');
        var padding = (4 - base64.Length % 4) % 4;
        base64 = base64.PadRight(base64.Length + padding, '=');

        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }

    public static string RegexEscape(string value)
    {
        return EscapeRegexPattern(value);
    }

    public static string EscapeSqlLikePattern(string value, char escapeCharacter = '\\')
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace(escapeCharacter.ToString(), new string(escapeCharacter, 2), StringComparison.Ordinal)
            .Replace("%", $"{escapeCharacter}%", StringComparison.Ordinal)
            .Replace("_", $"{escapeCharacter}_", StringComparison.Ordinal)
            .Replace("[", $"{escapeCharacter}[", StringComparison.Ordinal);
    }

    public static string EscapeControlCharacters(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);
    }

    public static string EscapeCsvField(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.IndexOfAny([',', '"', '\r', '\n']) < 0)
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    public static string JoinCsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join(",", values.Select(EscapeCsvField));
    }

    public static string JoinTsvRow(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return string.Join('\t', values.Select(value => value.Replace("\t", " ", StringComparison.Ordinal)));
    }

    public static string SanitizeForSingleLineLog(string value)
    {
        return EscapeControlCharacters(value);
    }

    public static string RedactPersonalData(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().Replace(value, "***@***");
    }

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

    public static string PrefixLines(string value, string prefix)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);

        return string.Join('\n', StringSamples.SplitLines(value).Select(line => prefix + line));
    }

    public static string PadLeftSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadLeft(totalWidth);
    }

    public static string PadRightSafe(string value, int totalWidth)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length >= totalWidth ? value : value.PadRight(totalWidth);
    }

    public static string CreateRandomCode(int length)
    {
        return CreateRandomFromAlphabet(length, TokenAlphabet);
    }

    public static string CreateNumericCode(int length)
    {
        return CreateRandomFromAlphabet(length, "0123456789");
    }

    public static string CreateUrlSafeToken(int byteLength = 32)
    {
        if (byteLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(byteLength), "Byte length must be one or greater.");
        }

        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static string ToShortGuid(Guid value)
    {
        return Convert.ToBase64String(value.ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

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

    public static string BuildObjectKey(params string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        return string.Join('/', segments.Select(segment => StringSamples.ToUnicodeSlug(segment)));
    }

    public static string BuildCacheKey(params string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        return string.Join(':', segments.Select(segment => StringSamples.NormalizeKey(segment).Replace(' ', ':').ToLowerInvariant()));
    }

    public static string CreateStableHashKey(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }

    public static bool IsBlank(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsEmailLike(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().IsMatch(value);
    }

    public static bool IsAbsoluteUrl(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    public static bool IsHttpsUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
    }

    public static bool IsGuid(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Guid.TryParse(value, out _);
    }

    public static bool IsDigitsOnly(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length > 0 && value.All(char.IsDigit);
    }

    public static bool IsAsciiSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return AsciiSlugRegex().IsMatch(value);
    }

    public static bool HasAllowedExtension(string fileName, ISet<string> allowedExtensions)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(allowedExtensions);

        return allowedExtensions.Contains(Path.GetExtension(fileName));
    }

    public static bool ContainsBlockedWord(string value, ISet<string> blockedWords)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(blockedWords);

        return blockedWords.Any(word => ContainsWholeWord(value, word));
    }

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

    public static string SnakeToPascalCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return string.Concat(value.Split('_', StringSplitOptions.RemoveEmptyEntries).Select(CapitalizeInvariant));
    }

    public static string PascalToCamelCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string PascalToKebabCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return PascalBoundaryRegex().Replace(value, "$1-$2").ToLowerInvariant();
    }

    public static string RenderTemplate(string template, IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(values);

        return PlaceholderRegex().Replace(template, match =>
            values.TryGetValue(match.Groups["name"].Value, out var value) ? value : match.Value);
    }

    public static IReadOnlyList<string> ExtractPlaceholders(string template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return PlaceholderRegex().Matches(template).Select(match => match.Groups["name"].Value).Distinct().ToList();
    }

    public static string PluralizeSimple(string singular, int count, string? plural = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(singular);

        return count == 1 ? singular : plural ?? singular + "s";
    }

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
