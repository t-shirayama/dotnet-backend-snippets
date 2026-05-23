using System.Globalization;

namespace DotnetBackendSnippets.FileHandling;

public static class FileUploadSamples
{
    public static FileUploadValidationResult ValidateUpload(
        UploadedFileMetadata file,
        FileUploadRules rules)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(rules);

        if (rules.MaxBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rules), "Max bytes must be greater than zero.");
        }

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(file.FileName))
        {
            errors.Add("ファイル名が空です。");
        }

        if (file.Length <= 0)
        {
            errors.Add("空のファイルはアップロードできません。");
        }
        else if (file.Length > rules.MaxBytes)
        {
            errors.Add($"ファイルサイズは {rules.MaxBytes.ToString(CultureInfo.InvariantCulture)} bytes 以下にしてください。");
        }

        var extension = NormalizeExtension(Path.GetExtension(file.FileName));
        var allowedExtensions = NormalizeExtensions(rules.AllowedExtensions);
        if (allowedExtensions.Count > 0 && !allowedExtensions.Contains(extension))
        {
            errors.Add("許可されていない拡張子です。");
        }

        var contentType = NormalizeContentType(file.ContentType);
        var allowedContentTypes = NormalizeContentTypes(rules.AllowedContentTypes);
        if (allowedContentTypes.Count > 0 && !allowedContentTypes.Contains(contentType))
        {
            errors.Add("許可されていない Content-Type です。");
        }

        return new FileUploadValidationResult(errors.Count == 0, errors);
    }

    public static string NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return string.Empty;
        }

        var trimmed = extension.Trim().ToLowerInvariant();

        return trimmed.StartsWith('.') ? trimmed : $".{trimmed}";
    }

    private static HashSet<string> NormalizeExtensions(IEnumerable<string> extensions)
    {
        return extensions
            .Select(NormalizeExtension)
            .Where(extension => extension.Length > 0)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static string NormalizeContentType(string? contentType)
    {
        return contentType?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    private static HashSet<string> NormalizeContentTypes(IEnumerable<string> contentTypes)
    {
        return contentTypes
            .Select(NormalizeContentType)
            .Where(contentType => contentType.Length > 0)
            .ToHashSet(StringComparer.Ordinal);
    }
}

public sealed record UploadedFileMetadata(string FileName, long Length, string? ContentType);

public sealed record FileUploadRules(
    long MaxBytes,
    IReadOnlyCollection<string> AllowedExtensions,
    IReadOnlyCollection<string> AllowedContentTypes);

public sealed record FileUploadValidationResult(bool IsValid, IReadOnlyList<string> Errors);
