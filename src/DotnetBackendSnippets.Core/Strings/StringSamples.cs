using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Strings;

public static partial class StringSamples
{
    public static string NormalizeWhitespace(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WhitespaceRegex().Replace(value.Trim(), " ");
    }

    public static string ToSlug(string value)
    {
        return ToAsciiSlug(value);
    }

    public static string ToAsciiSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = RemoveDiacritics(value).Trim().ToLower(CultureInfo.InvariantCulture);
        var slug = NonAlphaNumericRegex().Replace(normalized, "-");

        return DuplicateHyphenRegex().Replace(slug, "-").Trim('-');
    }

    public static string ToUnicodeSlug(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Trim().ToLower(CultureInfo.InvariantCulture);
        var slug = NonLetterOrDigitRegex().Replace(normalized, "-");

        return DuplicateHyphenRegex().Replace(slug, "-").Trim('-');
    }

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

    public static IReadOnlyList<string> SplitLines(string value, bool removeEmptyLines = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n', removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }

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
