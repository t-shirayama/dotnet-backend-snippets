using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class TypeInferenceSamplesTests
{
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
