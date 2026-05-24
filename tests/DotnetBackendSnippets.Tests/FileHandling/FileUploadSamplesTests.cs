using DotnetBackendSnippets.FileHandling;

namespace DotnetBackendSnippets.Tests.FileHandling;

public sealed class FileUploadSamplesTests
{
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

    [Fact]
    public void NormalizeExtension_AddsLeadingDotAndLowercases()
    {
        var result = FileUploadSamples.NormalizeExtension(" PDF ");

        Assert.Equal(".pdf", result);
    }

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

    [Fact]
    public void CreateServerFileName_UsesServerGeneratedIdAndOriginalExtensionOnly()
    {
        var fileId = Guid.Parse("c7f02e02-8db2-4d31-88d2-1f8a2f7a78a6");

        var result = FileUploadSamples.CreateServerFileName(@"..\unsafe\Report.PDF", fileId);

        Assert.Equal("c7f02e028db24d3188d21f8a2f7a78a6.pdf", result);
    }
}
