using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.Utilities;

public static partial class StringUtilities
{
    public static string NormalizeWhitespace(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return WhitespaceRegex().Replace(value.Trim(), " ");
    }

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
