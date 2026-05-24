using DotnetBackendSnippets.Utilities;

namespace DotnetBackendSnippets.Tests.Utilities;

// テスト対象: String Utilities のスニペット動作を確認する。
public sealed class StringUtilitiesTests
{
    // テスト意図: Normalize Whitespace / Trims And Collapses Whitespace を確認する。
    [Fact]
    public void NormalizeWhitespace_TrimsAndCollapsesWhitespace()
    {
        var result = StringUtilities.NormalizeWhitespace("  Hello\r\n   .NET\tBackend  ");

        Assert.Equal("Hello .NET Backend", result);
    }

    // テスト意図: Require Non Empty / Throws / When Value Is Blank を確認する。
    [Fact]
    public void RequireNonEmpty_Throws_WhenValueIsBlank()
    {
        var exception = Assert.Throws<ArgumentException>(() => StringUtilities.RequireNonEmpty(" ", "name"));

        Assert.Equal("name", exception.ParamName);
    }
}
