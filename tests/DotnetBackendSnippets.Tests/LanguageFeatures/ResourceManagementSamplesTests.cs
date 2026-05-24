using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class ResourceManagementSamplesTests
{
    [Fact]
    public void UseDisposableResource_DisposesAfterAction()
    {
        var result = ResourceManagementSamples.UseDisposableResource(resource => resource.Write("used"));

        Assert.Equal(["opened", "used", "disposed"], result);
    }

    [Fact]
    public void ReadUntilBlankLine_YieldsOnlyBeforeBlankLine()
    {
        var result = ResourceManagementSamples.ReadUntilBlankLine(["a", "b", "", "c"]).ToList();

        Assert.Equal(["a", "b"], result);
    }
}
