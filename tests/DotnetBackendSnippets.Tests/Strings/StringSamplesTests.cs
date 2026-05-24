using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

// テスト対象: String Samples のスニペット動作を確認する。
public sealed class StringSamplesTests
{
    // テスト意図: Normalize Whitespace / Trims And Collapses Whitespace を確認する。
    [Fact]
    public void NormalizeWhitespace_TrimsAndCollapsesWhitespace()
    {
        var result = StringSamples.NormalizeWhitespace("  Hello\r\n  C#\tWorld  ");

        Assert.Equal("Hello C# World", result);
    }

    // テスト意図: To Slug / Returns Lower Kebab Case Text を確認する。
    [Fact]
    public void ToSlug_ReturnsLowerKebabCaseText()
    {
        var result = StringSamples.ToSlug("  C# Backend Snippets!!  ");

        Assert.Equal("c-backend-snippets", result);
    }

    // テスト意図: To Ascii Slug / Returns Ascii Only Slug を確認する。
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

    // テスト意図: To Unicode Slug / Keeps Unicode Letters And Digits を確認する。
    [Fact]
    public void ToUnicodeSlug_KeepsUnicodeLettersAndDigits()
    {
        var result = StringSamples.ToUnicodeSlug("  日本語入力 Café 123 !! ");

        Assert.Equal("日本語入力-café-123", result);
    }

    // テスト意図: Truncate / Adds Suffix / When Value Is Too Long を確認する。
    [Fact]
    public void Truncate_AddsSuffix_WhenValueIsTooLong()
    {
        var result = StringSamples.Truncate("abcdefghij", 7);

        Assert.Equal("abcd...", result);
    }

    // テスト意図: Truncate / Returns Short Suffix / When Max Length Is Shorter Than Suffix を確認する。
    [Fact]
    public void Truncate_ReturnsShortSuffix_WhenMaxLengthIsShorterThanSuffix()
    {
        var result = StringSamples.Truncate("abcdef", 2);

        Assert.Equal("..", result);
    }

    // テスト意図: Truncate / Returns Empty / When Max Length Is Zero を確認する。
    [Fact]
    public void Truncate_ReturnsEmpty_WhenMaxLengthIsZero()
    {
        var result = StringSamples.Truncate("abcdef", 0);

        Assert.Equal(string.Empty, result);
    }

    // テスト意図: Truncate / Cuts Without Suffix / When Suffix Is Empty を確認する。
    [Fact]
    public void Truncate_CutsWithoutSuffix_WhenSuffixIsEmpty()
    {
        var result = StringSamples.Truncate("abcdef", 3, suffix: string.Empty);

        Assert.Equal("abc", result);
    }

    // テスト意図: Truncate / Throws / When Max Length Is Negative を確認する。
    [Fact]
    public void Truncate_Throws_WhenMaxLengthIsNegative()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => StringSamples.Truncate("abcdef", -1));

        Assert.Equal("maxLength", exception.ParamName);
    }

    // テスト意図: Mask Middle / Hides Middle Characters を確認する。
    [Fact]
    public void MaskMiddle_HidesMiddleCharacters()
    {
        var result = StringSamples.MaskMiddle("1234567890", visibleStart: 2, visibleEnd: 2);

        Assert.Equal("12******90", result);
    }

    // テスト意図: Mask Middle / Allows Zero Visible Start を確認する。
    [Fact]
    public void MaskMiddle_AllowsZeroVisibleStart()
    {
        var result = StringSamples.MaskMiddle("123456", visibleStart: 0, visibleEnd: 2);

        Assert.Equal("****56", result);
    }

    // テスト意図: Mask Middle / Allows Zero Visible End を確認する。
    [Fact]
    public void MaskMiddle_AllowsZeroVisibleEnd()
    {
        var result = StringSamples.MaskMiddle("123456", visibleStart: 2, visibleEnd: 0);

        Assert.Equal("12****", result);
    }

    // テスト意図: Mask Middle / Throws / When Visible Counts Are Negative を確認する。
    [Theory]
    [InlineData(-1, 0, "visibleStart")]
    [InlineData(0, -1, "visibleEnd")]
    public void MaskMiddle_Throws_WhenVisibleCountsAreNegative(int visibleStart, int visibleEnd, string expectedParamName)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => StringSamples.MaskMiddle("123456", visibleStart, visibleEnd));

        Assert.Equal(expectedParamName, exception.ParamName);
    }

    // テスト意図: Split Lines / Handles Different Line Endings を確認する。
    [Fact]
    public void SplitLines_HandlesDifferentLineEndings()
    {
        var result = StringSamples.SplitLines("a\r\nb\nc\rd");

        Assert.Equal(["a", "b", "c", "d"], result);
    }

    // テスト意図: Split Lines / Removes Empty Lines / When Requested を確認する。
    [Fact]
    public void SplitLines_RemovesEmptyLines_WhenRequested()
    {
        var result = StringSamples.SplitLines("a\r\n\r\nb\n\nc", removeEmptyLines: true);

        Assert.Equal(["a", "b", "c"], result);
    }

    // テスト意図: Normalize Key / Returns Upper Invariant Collapsed Key を確認する。
    [Fact]
    public void NormalizeKey_ReturnsUpperInvariantCollapsedKey()
    {
        var result = StringSamples.NormalizeKey("  customer   id ");

        Assert.Equal("CUSTOMER ID", result);
    }
}
