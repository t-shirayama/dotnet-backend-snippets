using DotnetBackendSnippets.Security;

namespace DotnetBackendSnippets.Tests.Security;

// テスト対象: Security Samples のスニペット動作を確認する。
public sealed class SecuritySamplesTests
{
    // テスト意図: Hash Password / Creates Hash That Verifies Original Password を確認する。
    [Fact]
    public void HashPassword_CreatesHashThatVerifiesOriginalPassword()
    {
        var hash = SecuritySamples.HashPassword("correct horse battery staple");

        Assert.True(SecuritySamples.VerifyPassword("correct horse battery staple", hash));
    }

    // テスト意図: Verify Password / Returns False / When Password Is Different を確認する。
    [Fact]
    public void VerifyPassword_ReturnsFalse_WhenPasswordIsDifferent()
    {
        var hash = SecuritySamples.HashPassword("expected-password");

        var result = SecuritySamples.VerifyPassword("wrong-password", hash);

        Assert.False(result);
    }

    // テスト意図: Verify Password / Returns False / When Hash Format Is Invalid を確認する。
    [Theory]
    [InlineData("")]
    [InlineData("not-a-password-hash")]
    [InlineData("PBKDF2-SHA256$100000$not-base64$also-not-base64")]
    public void VerifyPassword_ReturnsFalse_WhenHashFormatIsInvalid(string encodedHash)
    {
        var result = SecuritySamples.VerifyPassword("password", encodedHash);

        Assert.False(result);
    }

    // テスト意図: Are API Keys Equal / Returns True / When Keys Match を確認する。
    [Fact]
    public void AreApiKeysEqual_ReturnsTrue_WhenKeysMatch()
    {
        var result = SecuritySamples.AreApiKeysEqual("api-key-123", "api-key-123");

        Assert.True(result);
    }

    // テスト意図: Are API Keys Equal / Returns False / When Keys Do Not Match を確認する。
    [Fact]
    public void AreApiKeysEqual_ReturnsFalse_WhenKeysDoNotMatch()
    {
        var result = SecuritySamples.AreApiKeysEqual("api-key-123", "api-key-456");

        Assert.False(result);
    }

    // テスト意図: Find Potential Secrets / Finds Sensitive Configuration Keys を確認する。
    [Fact]
    public void FindPotentialSecrets_FindsSensitiveConfigurationKeys()
    {
        var values = new Dictionary<string, string?>
        {
            ["Logging:LogLevel:Default"] = "Information",
            ["ExternalApi:ApiKey"] = "sample-secret-value",
            ["Auth:Issuer"] = "https://example.test",
        };

        var result = SecuritySamples.FindPotentialSecrets(values);

        var finding = Assert.Single(result);
        Assert.Equal("ExternalApi:ApiKey", finding.Key);
    }

    // テスト意図: Find Potential Secrets / Finds Long Token Like Values を確認する。
    [Fact]
    public void FindPotentialSecrets_FindsLongTokenLikeValues()
    {
        var values = new Dictionary<string, string?>
        {
            ["Service:Endpoint"] = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6",
        };

        var result = SecuritySamples.FindPotentialSecrets(values);

        var finding = Assert.Single(result);
        Assert.Equal("Service:Endpoint", finding.Key);
    }

    // テスト意図: HTML Encode For Display / Escapes HTML Control Characters を確認する。
    [Fact]
    public void HtmlEncodeForDisplay_EscapesHtmlControlCharacters()
    {
        var result = SecuritySamples.HtmlEncodeForDisplay("<script>alert('x')</script>");

        Assert.Equal("&lt;script&gt;alert(&#39;x&#39;)&lt;/script&gt;", result);
    }

    // テスト意図: Quote SQL Identifier / Returns Bracket Quoted Identifier / When Name Is Whitelisted を確認する。
    [Fact]
    public void QuoteSqlIdentifier_ReturnsBracketQuotedIdentifier_WhenNameIsWhitelisted()
    {
        var result = SecuritySamples.QuoteSqlIdentifier("CustomerId");

        Assert.Equal("[CustomerId]", result);
    }

    // テスト意図: Quote SQL Identifier / Throws / When Identifier Contains SQL Syntax を確認する。
    [Fact]
    public void QuoteSqlIdentifier_Throws_WhenIdentifierContainsSqlSyntax()
    {
        var exception = Assert.Throws<ArgumentException>(() => SecuritySamples.QuoteSqlIdentifier("Users; DROP TABLE Users"));

        Assert.Equal("identifier", exception.ParamName);
    }
}
