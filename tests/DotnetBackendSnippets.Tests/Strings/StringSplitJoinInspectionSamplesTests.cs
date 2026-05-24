using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

// テスト対象: String Reverse Lookup Samples のスニペット動作を確認する。
public sealed partial class StringReverseLookupSamplesTests
{
    // テスト意図: CSV And Key Value Helpers / Parse Structured Text を確認する。
    [Fact]
    public void CsvAndKeyValueHelpers_ParseStructuredText()
    {
        Assert.Equal(["a", "b", "c"], StringReverseLookupSamples.SplitCsvLikeValues(" a, b,, c "));

        var values = StringReverseLookupSamples.ParseKeyValueLines("A=1\nB = 2");
        Assert.Equal("1", values["a"]);
    }

    // テスト意図: Text Extraction Helpers / Return Surrounding Parts And IDs を確認する。
    [Fact]
    public void TextExtractionHelpers_ReturnSurroundingPartsAndIds()
    {
        Assert.Equal("before", StringReverseLookupSamples.Before("before:after", ":"));
        Assert.Equal("after", StringReverseLookupSamples.After("before:after", ":"));
        Assert.Equal("a/b", StringReverseLookupSamples.BeforeLast("a/b/c", "/"));
        Assert.Equal("c", StringReverseLookupSamples.AfterLast("a/b/c", "/"));
        Assert.Equal("123", StringReverseLookupSamples.ExtractDigits("a1-b2-c3"));
        Assert.Equal("abc-123", StringReverseLookupSamples.ExtractCorrelationId("correlation-id=abc-123 completed"));
    }

    // テスト意図: Search And Comparison Helpers / Use Ordinal Ignore Case And Keyword Rules を確認する。
    [Fact]
    public void SearchAndComparisonHelpers_UseOrdinalIgnoreCaseAndKeywordRules()
    {
        Assert.True(StringReverseLookupSamples.ContainsIgnoreCase("Hello", "he"));
        Assert.True(StringReverseLookupSamples.StartsWithIgnoreCase("Hello", "he"));
        Assert.True(StringReverseLookupSamples.EndsWithIgnoreCase("Hello", "LO"));
        Assert.True(StringReverseLookupSamples.EqualsOrdinalIgnoreCase("A", "a"));
        Assert.Equal("かな", StringReverseLookupSamples.NormalizeKanaForSearch("カナ"));
        Assert.True(StringReverseLookupSamples.ContainsAllKeywords("C# backend snippets", ["backend", "SNIPPETS"]));
        Assert.True(StringReverseLookupSamples.ContainsAllKeywords("C# backend snippets", []));
        Assert.True(StringReverseLookupSamples.ContainsAnyKeyword("C# backend snippets", ["frontend", "backend"]));
        Assert.True(StringReverseLookupSamples.ContainsWholeWord("hello backend", "backend"));
    }

    // テスト意図: Regex Helper / Escapes User Pattern Literals を確認する。
    [Fact]
    public void RegexHelper_EscapesUserPatternLiterals()
    {
        Assert.Equal(@"\.\*", StringReverseLookupSamples.EscapeRegexPattern(".*"));
    }
}
