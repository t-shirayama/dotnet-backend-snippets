using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    private const string TokenAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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

    private static string GetFileNameWithoutExtensionPortable(string path)
    {
        var fileName = GetLastPathSegmentPortable(path);
        var extensionStart = fileName.LastIndexOf('.');

        return extensionStart <= 0 ? fileName : fileName[..extensionStart];
    }

    private static string GetExtensionPortable(string path)
    {
        var fileName = GetLastPathSegmentPortable(path);
        var extensionStart = fileName.LastIndexOf('.');

        return extensionStart <= 0 || extensionStart == fileName.Length - 1
            ? string.Empty
            : fileName[extensionStart..];
    }

    private static string GetLastPathSegmentPortable(string path)
    {
        var normalized = path.Replace('\\', '/');
        var separatorIndex = normalized.LastIndexOf('/');

        return separatorIndex < 0 ? normalized : normalized[(separatorIndex + 1)..];
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
