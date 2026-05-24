using System.Text;
using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

public sealed partial class StringReverseLookupSamplesTests
{
    [Fact]
    public void SlugAndUrlHelpers_CreateSafeValues()
    {
        Assert.Equal("item", StringReverseLookupSamples.ToSlugOrDefault("!!!"));
        Assert.Equal("hello", StringReverseLookupSamples.TruncateSlug("hello-world", 5));
        Assert.Equal("a%2Fb", StringReverseLookupSamples.EncodePathSegment("a/b"));
        Assert.Equal("a+b", StringReverseLookupSamples.EncodeQueryValue("a b"));
        Assert.Equal("monthly-report", StringReverseLookupSamples.FileNameToSlug("Monthly Report.xlsx"));
        Assert.Equal("monthly-report", StringReverseLookupSamples.FileNameToSlug(@"C:\temp\Monthly Report.xlsx"));
        Assert.Equal("monthly-report", StringReverseLookupSamples.FileNameToSlug("/tmp/Monthly Report.xlsx"));
        Assert.Equal("monthly-report-2", StringReverseLookupSamples.AppendSlugSuffix("monthly-report", 2));
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
        Assert.Throws<ArgumentException>(() => StringReverseLookupSamples.BuildObjectKey("Tenant", "!!!"));
        Assert.Throws<ArgumentException>(() => StringReverseLookupSamples.BuildObjectKey("Tenant", " "));
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
        Assert.True(StringReverseLookupSamples.HasAllowedExtension(@"C:\temp\a.txt", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".txt" }));
        Assert.True(StringReverseLookupSamples.HasAllowedExtension("/tmp/a.txt", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".txt" }));
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
