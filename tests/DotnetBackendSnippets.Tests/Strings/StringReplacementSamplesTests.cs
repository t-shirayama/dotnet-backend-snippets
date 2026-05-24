using DotnetBackendSnippets.Strings;

namespace DotnetBackendSnippets.Tests.Strings;

// テスト対象: String Reverse Lookup Samples のスニペット動作を確認する。
public sealed partial class StringReverseLookupSamplesTests
{
    // テスト意図: Masking And Redaction Helpers / Hide Sensitive Values を確認する。
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
}

