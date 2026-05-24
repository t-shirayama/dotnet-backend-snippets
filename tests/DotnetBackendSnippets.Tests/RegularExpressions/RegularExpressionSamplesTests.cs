using DotnetBackendSnippets.RegularExpressions;

namespace DotnetBackendSnippets.Tests.RegularExpressions;

// テスト対象: Regular Expression Samples のスニペット動作を確認する。
public sealed class RegularExpressionSamplesTests
{
    // テスト意図: Is Product Code / Validates Expected Shape を確認する。
    [Theory]
    [InlineData("ABC-1234", true)]
    [InlineData("abc-1234", false)]
    [InlineData("ABC-12", false)]
    public void IsProductCode_ValidatesExpectedShape(string value, bool expected)
    {
        Assert.Equal(expected, RegularExpressionSamples.IsProductCode(value));
    }

    // テスト意図: Extract Hashtags / Returns Tags Without Hash を確認する。
    [Fact]
    public void ExtractHashtags_ReturnsTagsWithoutHash()
    {
        IReadOnlyList<string> tags = RegularExpressionSamples.ExtractHashtags("Ship #DotNet and #backend_2026");

        Assert.Equal(["DotNet", "backend_2026"], tags);
    }

    // テスト意図: Normalize Whitespace / Collapses Whitespace を確認する。
    [Fact]
    public void NormalizeWhitespace_CollapsesWhitespace()
    {
        string result = RegularExpressionSamples.NormalizeWhitespace("  A\tB\r\nC  ");

        Assert.Equal("A B C", result);
    }

    // テスト意図: Create Literal Search Regex / Escapes User Input を確認する。
    [Fact]
    public void CreateLiteralSearchRegex_EscapesUserInput()
    {
        var regex = RegularExpressionSamples.CreateLiteralSearchRegex("a+b");

        Assert.Matches(regex, "A+B");
        Assert.DoesNotMatch(regex, "aaab");
    }

    // テスト意図: Is Match With Timeout / Returns False / When Regex Times Out を確認する。
    [Fact]
    public void IsMatchWithTimeout_ReturnsFalse_WhenRegexTimesOut()
    {
        string value = new('a', 64);

        bool result = RegularExpressionSamples.IsMatchWithTimeout(
            "^(a+)+$",
            value + "!",
            TimeSpan.FromMilliseconds(1));

        Assert.False(result);
    }
}
