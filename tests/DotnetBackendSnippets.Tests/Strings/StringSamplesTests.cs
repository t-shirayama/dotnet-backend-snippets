using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

public sealed class StringSamplesTests
{
    [Fact]
    public void NormalizeWhitespace_TrimsAndCollapsesWhitespace()
    {
        var result = StringSamples.NormalizeWhitespace("  Hello\r\n  C#\tWorld  ");

        Assert.Equal("Hello C# World", result);
    }

    [Fact]
    public void ToSlug_ReturnsLowerKebabCaseText()
    {
        var result = StringSamples.ToSlug("  C# Backend Snippets!!  ");

        Assert.Equal("c-backend-snippets", result);
    }

    [Fact]
    public void Truncate_AddsSuffix_WhenValueIsTooLong()
    {
        var result = StringSamples.Truncate("abcdefghij", 7);

        Assert.Equal("abcd...", result);
    }

    [Fact]
    public void Truncate_ReturnsShortSuffix_WhenMaxLengthIsShorterThanSuffix()
    {
        var result = StringSamples.Truncate("abcdef", 2);

        Assert.Equal("..", result);
    }

    [Fact]
    public void MaskMiddle_HidesMiddleCharacters()
    {
        var result = StringSamples.MaskMiddle("1234567890", visibleStart: 2, visibleEnd: 2);

        Assert.Equal("12******90", result);
    }

    [Fact]
    public void SplitLines_HandlesDifferentLineEndings()
    {
        var result = StringSamples.SplitLines("a\r\nb\nc\rd");

        Assert.Equal(["a", "b", "c", "d"], result);
    }

    [Fact]
    public void NormalizeKey_ReturnsUpperInvariantCollapsedKey()
    {
        var result = StringSamples.NormalizeKey("  customer   id ");

        Assert.Equal("CUSTOMER ID", result);
    }
}
