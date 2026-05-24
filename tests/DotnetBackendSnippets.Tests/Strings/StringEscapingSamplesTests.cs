using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

public sealed partial class StringReverseLookupSamplesTests
{
    [Fact]
    public void EncodingAndOutputHelpers_EscapeForCommonBoundaries()
    {
        Assert.Equal("&lt;b&gt;", StringReverseLookupSamples.HtmlEncode("<b>"));
        Assert.Contains("\\u003C", StringReverseLookupSamples.JavaScriptStringEncode("<script>"));
        Assert.Equal("a+b", StringReverseLookupSamples.UrlEncode("a b"));
        Assert.Equal("a b", StringReverseLookupSamples.UrlDecode("a+b"));
        Assert.Equal("44OG44K544OI", StringReverseLookupSamples.ToBase64("テスト"));
        Assert.Equal("hello", StringReverseLookupSamples.FromBase64Url(StringReverseLookupSamples.ToBase64Url("hello")));
        Assert.Equal(@"\%", StringReverseLookupSamples.EscapeSqlLikePattern("%"));
        Assert.Equal("a\\nb", StringReverseLookupSamples.EscapeControlCharacters("a\nb"));
        Assert.Equal("\"a,b\"", StringReverseLookupSamples.EscapeCsvField("a,b"));
        Assert.Equal("a,\"b,c\"", StringReverseLookupSamples.JoinCsvRow(["a", "b,c"]));
        Assert.Equal("a\tb c", StringReverseLookupSamples.JoinTsvRow(["a", "b\tc"]));
        Assert.Equal("a\\nb", StringReverseLookupSamples.SanitizeForSingleLineLog("a\nb"));
        Assert.Equal("***@***", StringReverseLookupSamples.RedactPersonalData("email=user@example.com"));
        Assert.Equal("  a\n  b", StringReverseLookupSamples.IndentLines("a\nb", 2));
        Assert.Equal("- a\n- b", StringReverseLookupSamples.PrefixLines("a\nb", "- "));
        Assert.Equal("  x", StringReverseLookupSamples.PadLeftSafe("x", 3));
        Assert.Equal("x  ", StringReverseLookupSamples.PadRightSafe("x", 3));
    }
}

