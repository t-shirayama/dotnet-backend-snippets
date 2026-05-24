using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

// テスト対象: String Reverse Lookup Samples のスニペット動作を確認する。
public sealed partial class StringReverseLookupSamplesTests
{
    // テスト意図: Normalization Helpers / Handle Whitespace Unicode And Separators を確認する。
    [Fact]
    public void NormalizationHelpers_HandleWhitespaceUnicodeAndSeparators()
    {
        Assert.Equal(string.Empty, StringReverseLookupSamples.TrimOrEmpty(null));
        Assert.Equal("value", StringReverseLookupSamples.TrimJapaneseWhitespace("\u3000 value \u3000"));
        Assert.Equal("A B C", StringReverseLookupSamples.NormalizeToSingleLine(" A\r\n  B\tC "));
        Assert.Equal("a|b|c", StringReverseLookupSamples.NormalizeLineEndings("a\r\nb\rc", "|"));
        Assert.Equal("é", StringReverseLookupSamples.NormalizeUnicode("e\u0301"));
        Assert.Equal("Cafe", StringReverseLookupSamples.RemoveDiacriticsForSearch("Café"));
        Assert.Equal("a-b_c", StringReverseLookupSamples.CollapseSeparators("a---b___c"));
        Assert.Null(StringReverseLookupSamples.NullIfWhiteSpace("   "));
    }
}

