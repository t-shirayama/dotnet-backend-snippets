using DotnetBackendSnippets.RegularExpressions;

namespace DotnetBackendSnippets.Tests.RegularExpressions;

// テスト対象: Regular Expression Samples の実務パターン系スニペットを確認する。
public sealed class RegularExpressionPatternSamplesTests
{
    // テスト意図: Try Parse Log Entry / Extracts Named Groups を確認する。
    [Fact]
    public void TryParseLogEntry_ExtractsNamedGroups()
    {
        const string line = "2026-05-24T10:15:30+00:00 INFO [req-1234] Created order";

        var result = RegularExpressionSamples.TryParseLogEntry(line);

        Assert.NotNull(result);
        Assert.Equal(new DateTimeOffset(2026, 5, 24, 10, 15, 30, TimeSpan.Zero), result.Value.Timestamp);
        Assert.Equal("INFO", result.Value.Level);
        Assert.Equal("req-1234", result.Value.CorrelationId);
        Assert.Equal("Created order", result.Value.Message);
    }

    // テスト意図: Try Parse Log Entry / Returns Null / When Shape Does Not Match を確認する。
    [Fact]
    public void TryParseLogEntry_ReturnsNull_WhenShapeDoesNotMatch()
    {
        var result = RegularExpressionSamples.TryParseLogEntry("not a structured log line");

        Assert.Null(result);
    }

    // テスト意図: Redact Email User Names / Uses Match Evaluator を確認する。
    [Fact]
    public void RedactEmailUserNames_UsesMatchEvaluator()
    {
        string result = RegularExpressionSamples.RedactEmailUserNames("Contact alice@example.com and b@example.jp");

        Assert.Equal("Contact a***@example.com and b***@example.jp", result);
    }

    // テスト意図: Split Tokens / Handles Multiple Separators を確認する。
    [Fact]
    public void SplitTokens_HandlesMultipleSeparators()
    {
        IReadOnlyList<string> result = RegularExpressionSamples.SplitTokens("alpha, beta;gamma| delta  epsilon");

        Assert.Equal(["alpha", "beta", "gamma", "delta", "epsilon"], result);
    }

    // テスト意図: Is Safe Ascii Identifier / Uses Non Backtracking Friendly Pattern を確認する。
    [Theory]
    [InlineData("customer_id-2026", true)]
    [InlineData("1-customer", false)]
    [InlineData("ab", false)]
    [InlineData("customer id", false)]
    public void IsSafeAsciiIdentifier_UsesNonBacktrackingFriendlyPattern(string value, bool expected)
    {
        Assert.Equal(expected, RegularExpressionSamples.IsSafeAsciiIdentifier(value));
    }

    // テスト意図: Contains Only Unicode Letters Numbers And Hyphen / Accepts Japanese を確認する。
    [Theory]
    [InlineData("注文-2026", true)]
    [InlineData("Order-2026", true)]
    [InlineData("注文_2026", false)]
    [InlineData("注文 2026", false)]
    public void ContainsOnlyUnicodeLettersNumbersAndHyphen_AcceptsJapanese(string value, bool expected)
    {
        Assert.Equal(expected, RegularExpressionSamples.ContainsOnlyUnicodeLettersNumbersAndHyphen(value));
    }

    // テスト意図: Get Registered Pattern / Returns Pattern By Kind を確認する。
    [Fact]
    public void GetRegisteredPattern_ReturnsPatternByKind()
    {
        var slugRegex = RegularExpressionSamples.GetRegisteredPattern(RegularExpressionSamples.RegularExpressionPatternKind.Slug);
        var correlationIdRegex = RegularExpressionSamples.GetRegisteredPattern(RegularExpressionSamples.RegularExpressionPatternKind.CorrelationId);

        Assert.Matches(slugRegex, "order-history-2026");
        Assert.DoesNotMatch(slugRegex, "Order History");
        Assert.Matches(correlationIdRegex, "req-2026:05:24");
    }
}
