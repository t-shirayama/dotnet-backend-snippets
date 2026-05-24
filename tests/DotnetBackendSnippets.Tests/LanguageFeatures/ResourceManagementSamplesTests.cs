using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Resource Management Samples のスニペット動作を確認する。
public sealed class ResourceManagementSamplesTests
{
    // テスト意図: Use Disposable Resource / Disposes After Action を確認する。
    [Fact]
    public void UseDisposableResource_DisposesAfterAction()
    {
        var result = ResourceManagementSamples.UseDisposableResource(resource => resource.Write("used"));

        Assert.Equal(["opened", "used", "disposed"], result);
    }

    // テスト意図: Read Until Blank Line / Yields Only Before Blank Line を確認する。
    [Fact]
    public void ReadUntilBlankLine_YieldsOnlyBeforeBlankLine()
    {
        var result = ResourceManagementSamples.ReadUntilBlankLine(["a", "b", "", "c"]).ToList();

        Assert.Equal(["a", "b"], result);
    }
}
