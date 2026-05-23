using DotnetBackendSnippets.Utilities;

namespace DotnetBackendSnippets.Tests.Utilities;

public sealed class StringUtilitiesTests
{
    [Fact]
    public void NormalizeWhitespace_TrimsAndCollapsesWhitespace()
    {
        var result = StringUtilities.NormalizeWhitespace("  Hello\r\n   .NET\tBackend  ");

        Assert.Equal("Hello .NET Backend", result);
    }

    [Fact]
    public void RequireNonEmpty_Throws_WhenValueIsBlank()
    {
        var exception = Assert.Throws<ArgumentException>(() => StringUtilities.RequireNonEmpty(" ", "name"));

        Assert.Equal("name", exception.ParamName);
    }
}
