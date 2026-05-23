using DotnetBackendSnippets.Security;

namespace DotnetBackendSnippets.Tests.Security;

public sealed class SecuritySamplesTests
{
    [Fact]
    public void HashPassword_CreatesHashThatVerifiesOriginalPassword()
    {
        var hash = SecuritySamples.HashPassword("correct horse battery staple");

        Assert.True(SecuritySamples.VerifyPassword("correct horse battery staple", hash));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_WhenPasswordIsDifferent()
    {
        var hash = SecuritySamples.HashPassword("expected-password");

        var result = SecuritySamples.VerifyPassword("wrong-password", hash);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-password-hash")]
    [InlineData("PBKDF2-SHA256$100000$not-base64$also-not-base64")]
    public void VerifyPassword_ReturnsFalse_WhenHashFormatIsInvalid(string encodedHash)
    {
        var result = SecuritySamples.VerifyPassword("password", encodedHash);

        Assert.False(result);
    }

    [Fact]
    public void AreApiKeysEqual_ReturnsTrue_WhenKeysMatch()
    {
        var result = SecuritySamples.AreApiKeysEqual("api-key-123", "api-key-123");

        Assert.True(result);
    }

    [Fact]
    public void AreApiKeysEqual_ReturnsFalse_WhenKeysDoNotMatch()
    {
        var result = SecuritySamples.AreApiKeysEqual("api-key-123", "api-key-456");

        Assert.False(result);
    }

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

    [Fact]
    public void HtmlEncodeForDisplay_EscapesHtmlControlCharacters()
    {
        var result = SecuritySamples.HtmlEncodeForDisplay("<script>alert('x')</script>");

        Assert.Equal("&lt;script&gt;alert(&#39;x&#39;)&lt;/script&gt;", result);
    }

    [Fact]
    public void QuoteSqlIdentifier_ReturnsBracketQuotedIdentifier_WhenNameIsWhitelisted()
    {
        var result = SecuritySamples.QuoteSqlIdentifier("CustomerId");

        Assert.Equal("[CustomerId]", result);
    }

    [Fact]
    public void QuoteSqlIdentifier_Throws_WhenIdentifierContainsSqlSyntax()
    {
        var exception = Assert.Throws<ArgumentException>(() => SecuritySamples.QuoteSqlIdentifier("Users; DROP TABLE Users"));

        Assert.Equal("identifier", exception.ParamName);
    }
}
