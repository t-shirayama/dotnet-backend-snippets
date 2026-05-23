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
}
