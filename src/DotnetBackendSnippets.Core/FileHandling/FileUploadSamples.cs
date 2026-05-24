using System.Globalization;

namespace DotnetBackendSnippets.FileHandling;

/// <summary>
/// ファイルアップロード検証の実装例を提供します。
/// </summary>
public static class FileUploadSamples
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] PdfSignature = [0x25, 0x50, 0x44, 0x46, 0x2D];

    /// <summary>
    /// アップロードされたファイルのメタデータだけが指定ルールを満たすか検証します。
    /// </summary>
    /// <remarks>
    /// 既存互換用の入口です。ファイル内容の検証は行わないため、内容確認が必要な場合は
    /// <see cref="ValidateUploadWithSignature(UploadedFileMetadata, FileUploadRules, ReadOnlySpan{byte}, KnownFileType)"/>
    /// のようなシグネチャ検証と組み合わせます。
    /// </remarks>
    /// <param name="file">アップロードされたファイルのメタデータ。</param>
    /// <param name="rules">検証ルール。</param>
    /// <returns>検証結果とエラー一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> または <paramref name="rules"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="rules"/> の許可拡張子一覧または許可 Content-Type 一覧が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rules"/> の最大バイト数が 0 以下の場合。</exception>
    public static FileUploadValidationResult ValidateUpload(
        UploadedFileMetadata file,
        FileUploadRules rules)
    {
        return ValidateUploadMetadata(file, rules);
    }

    /// <summary>
    /// アップロードされたファイルのメタデータが指定ルールを満たすか検証します。
    /// </summary>
    /// <param name="file">アップロードされたファイルのメタデータ。</param>
    /// <param name="rules">検証ルール。</param>
    /// <returns>検証結果とエラー一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> または <paramref name="rules"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="rules"/> の許可拡張子一覧または許可 Content-Type 一覧が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rules"/> の最大バイト数が 0 以下の場合。</exception>
    public static FileUploadValidationResult ValidateUploadMetadata(
        UploadedFileMetadata file,
        FileUploadRules rules)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(rules.AllowedExtensions);
        ArgumentNullException.ThrowIfNull(rules.AllowedContentTypes);

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

        var extension = NormalizeExtension(GetExtensionPortable(file.FileName));
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
    /// アップロードされたファイルのメタデータとファイルシグネチャをまとめて検証します。
    /// </summary>
    /// <param name="file">アップロードされたファイルのメタデータ。</param>
    /// <param name="rules">検証ルール。</param>
    /// <param name="headerBytes">ファイル先頭から読み取ったバイト列。</param>
    /// <param name="expectedFileType">期待するファイル種別。</param>
    /// <returns>検証結果とエラー一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> または <paramref name="rules"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="rules"/> の許可拡張子一覧または許可 Content-Type 一覧が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rules"/> の最大バイト数または <paramref name="expectedFileType"/> が不正な場合。</exception>
    public static FileUploadValidationResult ValidateUploadWithSignature(
        UploadedFileMetadata file,
        FileUploadRules rules,
        ReadOnlySpan<byte> headerBytes,
        KnownFileType expectedFileType)
    {
        var metadataResult = ValidateUploadMetadata(file, rules);
        var errors = metadataResult.Errors.ToList();

        if (!HasKnownFileSignature(headerBytes, expectedFileType))
        {
            errors.Add("ファイル内容のシグネチャが期待する形式と一致しません。");
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

    /// <summary>
    /// ファイル先頭バイトが既知のファイルシグネチャと一致するかを判定します。
    /// </summary>
    /// <param name="headerBytes">ファイル先頭から読み取ったバイト列。</param>
    /// <param name="fileType">期待するファイル種別。</param>
    /// <returns>期待するファイル種別のシグネチャと一致する場合は true。</returns>
    public static bool HasKnownFileSignature(ReadOnlySpan<byte> headerBytes, KnownFileType fileType)
    {
        return fileType switch
        {
            KnownFileType.Png => headerBytes.StartsWith(PngSignature),
            KnownFileType.Jpeg => headerBytes.Length >= 3
                && headerBytes[0] == 0xFF
                && headerBytes[1] == 0xD8
                && headerBytes[2] == 0xFF,
            KnownFileType.Pdf => headerBytes.StartsWith(PdfSignature),
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), "Unknown file type."),
        };
    }

    /// <summary>
    /// 元ファイル名をそのまま使わず、サーバー側で生成した保存名を作成します。
    /// </summary>
    /// <param name="originalFileName">拡張子を取り出す元ファイル名。</param>
    /// <param name="fileId">保存名に使うサーバー側で生成した ID。</param>
    /// <returns>ID と正規化済み拡張子で構成した保存用ファイル名。</returns>
    public static string CreateServerFileName(string originalFileName, Guid fileId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);

        if (fileId == Guid.Empty)
        {
            throw new ArgumentException("File id must not be empty.", nameof(fileId));
        }

        var extension = NormalizeExtension(GetExtensionPortable(originalFileName));

        return $"{fileId:N}{extension}";
    }

    private static HashSet<string> NormalizeExtensions(IEnumerable<string> extensions)
    {
        return extensions
            .Select(NormalizeExtension)
            .Where(extension => extension.Length > 0)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static string GetExtensionPortable(string fileName)
    {
        var normalized = fileName.Replace('\\', '/');
        var lastSegmentStart = normalized.LastIndexOf('/') + 1;
        var lastSegment = normalized[lastSegmentStart..];
        var extensionStart = lastSegment.LastIndexOf('.');

        return extensionStart <= 0 || extensionStart == lastSegment.Length - 1
            ? string.Empty
            : lastSegment[extensionStart..];
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

/// <summary>
/// シグネチャ確認に使う既知のファイル種別です。
/// </summary>
public enum KnownFileType
{
    /// <summary>
    /// PNG 画像。
    /// </summary>
    Png,

    /// <summary>
    /// JPEG 画像。
    /// </summary>
    Jpeg,

    /// <summary>
    /// PDF 文書。
    /// </summary>
    Pdf,
}
