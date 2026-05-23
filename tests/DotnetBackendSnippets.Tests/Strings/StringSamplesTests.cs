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

    [Theory]
    [InlineData("", "")]
    [InlineData("!!!", "")]
    [InlineData("日本語入力", "")]
    [InlineData("Café au lait", "cafe-au-lait")]
    public void ToAsciiSlug_ReturnsAsciiOnlySlug(string value, string expected)
    {
        var result = StringSamples.ToAsciiSlug(value);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToUnicodeSlug_KeepsUnicodeLettersAndDigits()
    {
        var result = StringSamples.ToUnicodeSlug("  日本語入力 Café 123 !! ");

        Assert.Equal("日本語入力-café-123", result);
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
    public void Truncate_ReturnsEmpty_WhenMaxLengthIsZero()
    {
        var result = StringSamples.Truncate("abcdef", 0);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Truncate_CutsWithoutSuffix_WhenSuffixIsEmpty()
    {
        var result = StringSamples.Truncate("abcdef", 3, suffix: string.Empty);

        Assert.Equal("abc", result);
    }

    [Fact]
    public void Truncate_Throws_WhenMaxLengthIsNegative()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => StringSamples.Truncate("abcdef", -1));

        Assert.Equal("maxLength", exception.ParamName);
    }

    [Fact]
    public void MaskMiddle_HidesMiddleCharacters()
    {
        var result = StringSamples.MaskMiddle("1234567890", visibleStart: 2, visibleEnd: 2);

        Assert.Equal("12******90", result);
    }

    [Fact]
    public void MaskMiddle_AllowsZeroVisibleStart()
    {
        var result = StringSamples.MaskMiddle("123456", visibleStart: 0, visibleEnd: 2);

        Assert.Equal("****56", result);
    }

    [Fact]
    public void MaskMiddle_AllowsZeroVisibleEnd()
    {
        var result = StringSamples.MaskMiddle("123456", visibleStart: 2, visibleEnd: 0);

        Assert.Equal("12****", result);
    }

    [Theory]
    [InlineData(-1, 0, "visibleStart")]
    [InlineData(0, -1, "visibleEnd")]
    public void MaskMiddle_Throws_WhenVisibleCountsAreNegative(int visibleStart, int visibleEnd, string expectedParamName)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => StringSamples.MaskMiddle("123456", visibleStart, visibleEnd));

        Assert.Equal(expectedParamName, exception.ParamName);
    }

    [Fact]
    public void SplitLines_HandlesDifferentLineEndings()
    {
        var result = StringSamples.SplitLines("a\r\nb\nc\rd");

        Assert.Equal(["a", "b", "c", "d"], result);
    }

    [Fact]
    public void SplitLines_RemovesEmptyLines_WhenRequested()
    {
        var result = StringSamples.SplitLines("a\r\n\r\nb\n\nc", removeEmptyLines: true);

        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void NormalizeKey_ReturnsUpperInvariantCollapsedKey()
    {
        var result = StringSamples.NormalizeKey("  customer   id ");

        Assert.Equal("CUSTOMER ID", result);
    }
}
