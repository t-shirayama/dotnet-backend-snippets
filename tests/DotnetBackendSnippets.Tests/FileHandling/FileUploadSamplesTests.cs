using DotnetBackendSnippets.FileHandling;

namespace DotnetBackendSnippets.Tests.FileHandling;

// テスト対象: File Upload Samples のスニペット動作を確認する。
public sealed class FileUploadSamplesTests
{
    // テスト意図: Validate Upload / Returns Valid / When Metadata Matches Rules を確認する。
    [Fact]
    public void ValidateUpload_ReturnsValid_WhenMetadataMatchesRules()
    {
        var file = new UploadedFileMetadata("avatar.JPG", 512_000, "IMAGE/JPEG");
        var rules = new FileUploadRules(
            MaxBytes: 1_000_000,
            AllowedExtensions: [".jpg", ".png"],
            AllowedContentTypes: ["image/jpeg", "image/png"]);

        var result = FileUploadSamples.ValidateUpload(file, rules);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // テスト意図: Validate Upload Metadata / Returns Valid / When Metadata Matches Rules を確認する。
    [Fact]
    public void ValidateUploadMetadata_ReturnsValid_WhenMetadataMatchesRules()
    {
        var file = new UploadedFileMetadata("avatar.JPG", 512_000, "IMAGE/JPEG");
        var rules = new FileUploadRules(
            MaxBytes: 1_000_000,
            AllowedExtensions: [".jpg", ".png"],
            AllowedContentTypes: ["image/jpeg", "image/png"]);

        var result = FileUploadSamples.ValidateUploadMetadata(file, rules);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // テスト意図: Validate Upload / Reads Extension Without Depending On Os Path Rules を確認する。
    [Theory]
    [InlineData(@"C:\uploads\avatar.JPG")]
    [InlineData("/uploads/avatar.JPG")]
    public void ValidateUpload_ReadsExtensionWithoutDependingOnOsPathRules(string fileName)
    {
        var file = new UploadedFileMetadata(fileName, 512_000, "IMAGE/JPEG");
        var rules = new FileUploadRules(
            MaxBytes: 1_000_000,
            AllowedExtensions: [".jpg", ".png"],
            AllowedContentTypes: ["image/jpeg", "image/png"]);

        var result = FileUploadSamples.ValidateUpload(file, rules);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // テスト意図: Validate Upload / Returns Errors / When File Breaks Rules を確認する。
    [Fact]
    public void ValidateUpload_ReturnsErrors_WhenFileBreaksRules()
    {
        var file = new UploadedFileMetadata("payload.exe", 2_000_000, "application/octet-stream");
        var rules = new FileUploadRules(
            MaxBytes: 1_000_000,
            AllowedExtensions: [".jpg", ".png"],
            AllowedContentTypes: ["image/jpeg", "image/png"]);

        var result = FileUploadSamples.ValidateUpload(file, rules);

        Assert.False(result.IsValid);
        Assert.Contains("ファイルサイズは 1000000 bytes 以下にしてください。", result.Errors);
        Assert.Contains("許可されていない拡張子です。", result.Errors);
        Assert.Contains("許可されていない Content-Type です。", result.Errors);
    }

    // テスト意図: Validate Upload / Returns Error / When File Is Empty を確認する。
    [Fact]
    public void ValidateUpload_ReturnsError_WhenFileIsEmpty()
    {
        var file = new UploadedFileMetadata("empty.txt", 0, "text/plain");
        var rules = new FileUploadRules(
            MaxBytes: 100,
            AllowedExtensions: [".txt"],
            AllowedContentTypes: ["text/plain"]);

        var result = FileUploadSamples.ValidateUpload(file, rules);

        Assert.False(result.IsValid);
        Assert.Contains("空のファイルはアップロードできません。", result.Errors);
    }

    // テスト意図: Validate Upload / Returns Error / When File Name Is Empty を確認する。
    [Fact]
    public void ValidateUpload_ReturnsError_WhenFileNameIsEmpty()
    {
        var file = new UploadedFileMetadata("", 10, "text/plain");
        var rules = new FileUploadRules(
            MaxBytes: 100,
            AllowedExtensions: [".txt"],
            AllowedContentTypes: ["text/plain"]);

        var result = FileUploadSamples.ValidateUpload(file, rules);

        Assert.False(result.IsValid);
        Assert.Contains("ファイル名が空です。", result.Errors);
    }

    // テスト意図: Validate Upload / Throws / When Rule Collections Are Null を確認する。
    [Fact]
    public void ValidateUpload_Throws_WhenRuleCollectionsAreNull()
    {
        var file = new UploadedFileMetadata("report.pdf", 10, "application/pdf");
        var rules = new FileUploadRules(
            MaxBytes: 100,
            AllowedExtensions: null!,
            AllowedContentTypes: ["application/pdf"]);

        Assert.Throws<ArgumentNullException>(() => FileUploadSamples.ValidateUpload(file, rules));
    }

    // テスト意図: Normalize Extension / Adds Leading Dot And Lowercases を確認する。
    [Fact]
    public void NormalizeExtension_AddsLeadingDotAndLowercases()
    {
        var result = FileUploadSamples.NormalizeExtension(" PDF ");

        Assert.Equal(".pdf", result);
    }

    // テスト意図: Has Known File Signature / Checks File Header Bytes を確認する。
    [Theory]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, KnownFileType.Png, true)]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, KnownFileType.Jpeg, true)]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, KnownFileType.Pdf, true)]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, KnownFileType.Png, false)]
    public void HasKnownFileSignature_ChecksFileHeaderBytes(byte[] headerBytes, KnownFileType fileType, bool expected)
    {
        var result = FileUploadSamples.HasKnownFileSignature(headerBytes, fileType);

        Assert.Equal(expected, result);
    }

    // テスト意図: Validate Upload With Signature / Combines Metadata And Header Validation を確認する。
    [Fact]
    public void ValidateUploadWithSignature_CombinesMetadataAndHeaderValidation()
    {
        var file = new UploadedFileMetadata("avatar.png", 512_000, "image/png");
        var rules = new FileUploadRules(
            MaxBytes: 1_000_000,
            AllowedExtensions: [".png"],
            AllowedContentTypes: ["image/png"]);
        byte[] pdfHeader = [0x25, 0x50, 0x44, 0x46, 0x2D];

        var result = FileUploadSamples.ValidateUploadWithSignature(file, rules, pdfHeader, KnownFileType.Png);

        Assert.False(result.IsValid);
        Assert.Contains("ファイル内容のシグネチャが期待する形式と一致しません。", result.Errors);
    }

    // テスト意図: Create Server File Name / Uses Server Generated ID And Original Extension Only を確認する。
    [Fact]
    public void CreateServerFileName_UsesServerGeneratedIdAndOriginalExtensionOnly()
    {
        var fileId = Guid.Parse("c7f02e02-8db2-4d31-88d2-1f8a2f7a78a6");

        var result = FileUploadSamples.CreateServerFileName(@"..\unsafe\Report.PDF", fileId);

        Assert.Equal("c7f02e028db24d3188d21f8a2f7a78a6.pdf", result);
    }

    // テスト意図: Create Server File Name / Throws / When File Id Is Empty を確認する。
    [Fact]
    public void CreateServerFileName_Throws_WhenFileIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => FileUploadSamples.CreateServerFileName("report.pdf", Guid.Empty));

        Assert.Equal("fileId", exception.ParamName);
    }
}
