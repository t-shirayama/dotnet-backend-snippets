using System.Text;
using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

public sealed class StringReverseLookupSamplesTests
{
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

    [Fact]
    public void SlugAndUrlHelpers_CreateSafeValues()
    {
        Assert.Equal("item", StringReverseLookupSamples.ToSlugOrDefault("!!!"));
        Assert.Equal("hello", StringReverseLookupSamples.TruncateSlug("hello-world", 5));
        Assert.Equal("a%2Fb", StringReverseLookupSamples.EncodePathSegment("a/b"));
        Assert.Equal("a+b", StringReverseLookupSamples.EncodeQueryValue("a b"));
        Assert.Equal("monthly-report", StringReverseLookupSamples.FileNameToSlug("Monthly Report.xlsx"));
        Assert.Equal("monthly-report-2", StringReverseLookupSamples.AppendSlugSuffix("monthly-report", 2));
    }

    [Fact]
    public void MaskingAndRedactionHelpers_HideSensitiveValues()
    {
        Assert.Equal("a***e@example.com", StringReverseLookupSamples.MaskEmail("alice@example.com"));
        Assert.Equal("******7890", StringReverseLookupSamples.MaskPhoneNumber("03-1234-7890"));
        Assert.Equal("************1111", StringReverseLookupSamples.MaskCardNumber("4111 1111 1111 1111"));
        Assert.Equal("a😀...", StringReverseLookupSamples.TruncateTextElements("a😀b", 2));
        Assert.Equal("password=***", StringReverseLookupSamples.RedactSecrets("password=secret"));
        Assert.Equal("""{"name":"Ada","password":"***"}""", StringReverseLookupSamples.MaskJsonFields("""{"name":"Ada","password":"secret"}""", new HashSet<string> { "password" }));
        Assert.Equal("abc...", StringReverseLookupSamples.TruncateByDisplayWidth("abc日本語", 3));
        Assert.Equal("n/a", StringReverseLookupSamples.DefaultIfEmpty(string.Empty, "n/a"));
    }

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

    [Fact]
    public void KeyValidationAndTemplateHelpers_CreateStableIdentifiers()
    {
        Assert.Equal(8, StringReverseLookupSamples.CreateRandomCode(8).Length);
        Assert.Matches("^[0-9]{6}$", StringReverseLookupSamples.CreateNumericCode(6));
        Assert.NotEmpty(StringReverseLookupSamples.CreateUrlSafeToken(8));
        Assert.NotEmpty(StringReverseLookupSamples.ToShortGuid(Guid.Parse("00112233-4455-6677-8899-aabbccddeeff")));
        Assert.Equal("a-b.txt", StringReverseLookupSamples.ToSafeFileName("a:b.txt"));
        Assert.Equal("tenant/customer-file", StringReverseLookupSamples.BuildObjectKey("Tenant", "Customer File"));
        Assert.Equal("tenant:customer:id", StringReverseLookupSamples.BuildCacheKey("Tenant", "Customer Id"));
        Assert.Equal(64, StringReverseLookupSamples.CreateStableHashKey("value").Length);
        Assert.True(StringReverseLookupSamples.IsBlank(" "));
        Assert.True(StringReverseLookupSamples.IsEmailLike("user@example.com"));
        Assert.True(StringReverseLookupSamples.IsAbsoluteUrl("https://example.com"));
        Assert.True(StringReverseLookupSamples.IsHttpsUrl("https://example.com"));
        Assert.True(StringReverseLookupSamples.IsGuid("00112233-4455-6677-8899-aabbccddeeff"));
        Assert.True(StringReverseLookupSamples.IsDigitsOnly("123"));
        Assert.True(StringReverseLookupSamples.IsAsciiSlug("backend-snippets"));
        Assert.True(StringReverseLookupSamples.HasAllowedExtension("a.txt", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".txt" }));
        Assert.True(StringReverseLookupSamples.ContainsBlockedWord("bad value", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "bad" }));
        StringReverseLookupSamples.ValidateMaxLength("abc", 3);
        Assert.Equal("CustomerId", StringReverseLookupSamples.SnakeToPascalCase("customer_id"));
        Assert.Equal("customerId", StringReverseLookupSamples.PascalToCamelCase("CustomerId"));
        Assert.Equal("customer-id", StringReverseLookupSamples.PascalToKebabCase("CustomerId"));
        Assert.Equal("Hello Ada", StringReverseLookupSamples.RenderTemplate("Hello {name}", new Dictionary<string, string> { ["name"] = "Ada" }));
        Assert.Equal(["name"], StringReverseLookupSamples.ExtractPlaceholders("Hello {name}"));
        Assert.Equal("items", StringReverseLookupSamples.PluralizeSimple("item", 2));
        Assert.Equal("1 KB", StringReverseLookupSamples.FormatBytes(1024));
        Assert.True(Encoding.UTF8.GetByteCount(StringReverseLookupSamples.TruncateUtf8Bytes("abcdef", 4)) <= 4);
        Assert.Equal("CUSTOMER_ID", StringReverseLookupSamples.ToEnvironmentVariableName("CustomerId"));
    }
}
