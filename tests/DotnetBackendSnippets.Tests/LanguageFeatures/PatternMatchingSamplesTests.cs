using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Pattern Matching Samples のスニペット動作を確認する。
public sealed class PatternMatchingSamplesTests
{
    // テスト意図: Describe Value / Uses Type And Property Patterns を確認する。
    [Theory]
    [InlineData(null, "null")]
    [InlineData("", "empty string")]
    [InlineData("abc", "string:3")]
    [InlineData(1, "non-negative integer")]
    [InlineData(-1, "negative integer")]
    public void DescribeValue_UsesTypeAndPropertyPatterns(object? value, string expected)
    {
        var result = PatternMatchingSamples.DescribeValue(value);

        Assert.Equal(expected, result);
    }

    // テスト意図: Describe Value / Uses List Pattern を確認する。
    [Fact]
    public void DescribeValue_UsesListPattern()
    {
        var result = PatternMatchingSamples.DescribeNumbers([3, 4]);

        Assert.Equal("integer array starting with 3", result);
    }

    // テスト意図: Classify Shipment / Uses Property Patterns を確認する。
    [Theory]
    [InlineData("", null, "preparing")]
    [InlineData("TRACK", null, "in transit")]
    [InlineData("TRACK", "2026-05-24T00:00:00+09:00", "delivered")]
    public void ClassifyShipment_UsesPropertyPatterns(
        string trackingNumber,
        string? deliveredAt,
        string expected)
    {
        var shipment = new Shipment(
            trackingNumber,
            deliveredAt is null ? null : DateTimeOffset.Parse(deliveredAt));

        var result = PatternMatchingSamples.ClassifyShipment(shipment);

        Assert.Equal(expected, result);
    }

    // テスト意図: Classify Point / Uses Deconstruct Pattern を確認する。
    [Theory]
    [InlineData(0, 0, "origin")]
    [InlineData(0, 5, "y-axis")]
    [InlineData(5, 0, "x-axis")]
    [InlineData(1, 1, "quadrant-1")]
    [InlineData(-1, 1, "other")]
    public void ClassifyPoint_UsesDeconstructPattern(int x, int y, string expected)
    {
        var result = PatternMatchingSamples.ClassifyPoint(new Coordinate(x, y));

        Assert.Equal(expected, result);
    }
}
