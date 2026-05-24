using DotnetBackendSnippets.RegularExpressions;

namespace DotnetBackendSnippets.Tests.RegularExpressions;

public sealed class RegularExpressionSamplesTests
{
    [Theory]
    [InlineData("ABC-1234", true)]
    [InlineData("abc-1234", false)]
    [InlineData("ABC-12", false)]
    public void IsProductCode_ValidatesExpectedShape(string value, bool expected)
    {
        Assert.Equal(expected, RegularExpressionSamples.IsProductCode(value));
    }

    [Fact]
    public void ExtractHashtags_ReturnsTagsWithoutHash()
    {
        IReadOnlyList<string> tags = RegularExpressionSamples.ExtractHashtags("Ship #DotNet and #backend_2026");

        Assert.Equal(["DotNet", "backend_2026"], tags);
    }

    [Fact]
    public void NormalizeWhitespace_CollapsesWhitespace()
    {
        string result = RegularExpressionSamples.NormalizeWhitespace("  A\tB\r\nC  ");

        Assert.Equal("A B C", result);
    }

    [Fact]
    public void CreateLiteralSearchRegex_EscapesUserInput()
    {
        var regex = RegularExpressionSamples.CreateLiteralSearchRegex("a+b");

        Assert.Matches(regex, "A+B");
        Assert.DoesNotMatch(regex, "aaab");
    }

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
