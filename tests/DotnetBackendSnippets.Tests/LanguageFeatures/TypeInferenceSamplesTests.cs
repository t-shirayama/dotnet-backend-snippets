using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Type Inference Samples のスニペット動作を確認する。
public sealed class TypeInferenceSamplesTests
{
    // テスト意図: Build Line Summaries / Uses Projection With Inferred Anonymous Shape Internally を確認する。
    [Fact]
    public void BuildLineSummaries_UsesProjectionWithInferredAnonymousShapeInternally()
    {
        OrderLine[] lines =
        [
            new("A-001", 2, 120m),
            new("B-001", 1, 50m),
        ];

        var result = TypeInferenceSamples.BuildLineSummaries(lines);

        Assert.Equal(
            [
                new OrderLineSummary("A-001", 2, 120m, 240m),
                new OrderLineSummary("B-001", 1, 50m, 50m),
            ],
            result);
    }

    // テスト意図: Build Totals By Sku / Uses Explicit Dictionary Type At API Boundary を確認する。
    [Fact]
    public void BuildTotalsBySku_UsesExplicitDictionaryTypeAtApiBoundary()
    {
        OrderLine[] lines =
        [
            new("A-001", 2, 120m),
            new("A-001", 1, 120m),
            new("B-001", 1, 50m),
        ];

        var result = TypeInferenceSamples.BuildTotalsBySku(lines);

        Assert.Equal(360m, result["A-001"]);
        Assert.Equal(50m, result["B-001"]);
    }
}
