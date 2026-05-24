using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

public sealed partial class StringReverseLookupSamplesTests
{
    [Fact]
    public void SplittingAndSearchHelpers_ParseAndCompareText()
    {
        Assert.Equal(["a", "b", "c"], StringReverseLookupSamples.SplitCsvLikeValues(" a, b,, c "));

        var values = StringReverseLookupSamples.ParseKeyValueLines("A=1\nB = 2");
        Assert.Equal("1", values["a"]);
        Assert.Equal("before", StringReverseLookupSamples.Before("before:after", ":"));
        Assert.Equal("after", StringReverseLookupSamples.After("before:after", ":"));
        Assert.Equal("a/b", StringReverseLookupSamples.BeforeLast("a/b/c", "/"));
        Assert.Equal("c", StringReverseLookupSamples.AfterLast("a/b/c", "/"));
        Assert.Equal("123", StringReverseLookupSamples.ExtractDigits("a1-b2-c3"));
        Assert.Equal("abc-123", StringReverseLookupSamples.ExtractCorrelationId("correlation-id=abc-123 completed"));
        Assert.True(StringReverseLookupSamples.ContainsIgnoreCase("Hello", "he"));
        Assert.True(StringReverseLookupSamples.StartsWithIgnoreCase("Hello", "he"));
        Assert.True(StringReverseLookupSamples.EndsWithIgnoreCase("Hello", "LO"));
        Assert.True(StringReverseLookupSamples.EqualsOrdinalIgnoreCase("A", "a"));
        Assert.Equal("かな", StringReverseLookupSamples.NormalizeKanaForSearch("カナ"));
        Assert.True(StringReverseLookupSamples.ContainsAllKeywords("C# backend snippets", ["backend", "SNIPPETS"]));
        Assert.True(StringReverseLookupSamples.ContainsAnyKeyword("C# backend snippets", ["frontend", "backend"]));
        Assert.True(StringReverseLookupSamples.ContainsWholeWord("hello backend", "backend"));
        Assert.Equal(@"\.\*", StringReverseLookupSamples.EscapeRegexPattern(".*"));
    }
}

