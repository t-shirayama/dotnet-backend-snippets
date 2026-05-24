using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

// テスト対象: String Reverse Lookup Samples のスニペット動作を確認する。
public sealed partial class StringReverseLookupSamplesTests
{
    // テスト意図: HTML JavaScript URL And Base64 Helpers / Escape External Boundaries を確認する。
    [Fact]
    public void HtmlJavaScriptUrlAndBase64Helpers_EscapeExternalBoundaries()
    {
        Assert.Equal("&lt;b&gt;", StringReverseLookupSamples.HtmlEncode("<b>"));
        Assert.Contains("\\u003C", StringReverseLookupSamples.JavaScriptStringEncode("<script>"));
        Assert.Equal("a+b", StringReverseLookupSamples.UrlEncode("a b"));
        Assert.Equal("a b", StringReverseLookupSamples.UrlDecode("a+b"));
        Assert.Equal("44OG44K544OI", StringReverseLookupSamples.ToBase64("テスト"));
        Assert.Equal("hello", StringReverseLookupSamples.FromBase64Url(StringReverseLookupSamples.ToBase64Url("hello")));
    }

    // テスト意図: SQL CSV And TSV Helpers / Escape Delimited Text を確認する。
    [Fact]
    public void SqlCsvAndTsvHelpers_EscapeDelimitedText()
    {
        Assert.Equal(@"\%", StringReverseLookupSamples.EscapeSqlLikePattern("%"));
        Assert.Equal("\"a,b\"", StringReverseLookupSamples.EscapeCsvField("a,b"));
        Assert.Equal("a,\"b,c\"", StringReverseLookupSamples.JoinCsvRow(["a", "b,c"]));
        Assert.Equal("a\tb c", StringReverseLookupSamples.JoinTsvRow(["a", "b\tc"]));
    }

    // テスト意図: Log And Personal Data Helpers / Sanitize Sensitive Output を確認する。
    [Fact]
    public void LogAndPersonalDataHelpers_SanitizeSensitiveOutput()
    {
        Assert.Equal("a\\nb", StringReverseLookupSamples.EscapeControlCharacters("a\nb"));
        Assert.Equal("a\\nb", StringReverseLookupSamples.SanitizeForSingleLineLog("a\nb"));
        Assert.Equal("***@***", StringReverseLookupSamples.RedactPersonalData("email=user@example.com"));
    }

    // テスト意図: Line Formatting Helpers / Add Indent Prefix And Padding を確認する。
    [Fact]
    public void LineFormattingHelpers_AddIndentPrefixAndPadding()
    {
        Assert.Equal("  a\n  b", StringReverseLookupSamples.IndentLines("a\nb", 2));
        Assert.Equal("- a\n- b", StringReverseLookupSamples.PrefixLines("a\nb", "- "));
        Assert.Equal("  x", StringReverseLookupSamples.PadLeftSafe("x", 3));
        Assert.Equal("x  ", StringReverseLookupSamples.PadRightSafe("x", 3));
    }
}
