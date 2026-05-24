using System.Globalization;

namespace DotnetBackendSnippets.FileHandling;

/// <summary>
/// ファイルアップロード検証の実装例を提供します。
/// </summary>
public static class FileUploadSamples
{
    /// <summary>
    /// アップロードされたファイルが指定ルールを満たすか検証します。
    /// </summary>
    /// <param name="file">アップロードされたファイルのメタデータ。</param>
    /// <param name="rules">検証ルール。</param>
    /// <returns>検証結果とエラー一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> または <paramref name="rules"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rules"/> の最大バイト数が 0 以下の場合。</exception>
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

    /// <summary>
    /// 拡張子を小文字のドット付き形式に正規化します。
    /// </summary>
    /// <param name="extension">正規化する拡張子。</param>
    /// <returns>正規化後の拡張子。空白の場合は空文字列。</returns>
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

/// <summary>
/// アップロードされたファイルの基本情報を表します。
/// </summary>
/// <param name="FileName">ファイル名。</param>
/// <param name="Length">ファイルサイズのバイト数。</param>
/// <param name="ContentType">Content-Type ヘッダーの値。</param>
public sealed record UploadedFileMetadata(string FileName, long Length, string? ContentType);

/// <summary>
/// ファイルアップロードの検証ルールを表します。
/// </summary>
/// <param name="MaxBytes">許可する最大ファイルサイズ。</param>
/// <param name="AllowedExtensions">許可する拡張子一覧。</param>
/// <param name="AllowedContentTypes">許可する Content-Type 一覧。</param>
public sealed record FileUploadRules(
    long MaxBytes,
    IReadOnlyCollection<string> AllowedExtensions,
    IReadOnlyCollection<string> AllowedContentTypes);

/// <summary>
/// ファイルアップロード検証の結果を表します。
/// </summary>
/// <param name="IsValid">検証に成功したかどうか。</param>
/// <param name="Errors">検証エラーの一覧。</param>
public sealed record FileUploadValidationResult(bool IsValid, IReadOnlyList<string> Errors);
