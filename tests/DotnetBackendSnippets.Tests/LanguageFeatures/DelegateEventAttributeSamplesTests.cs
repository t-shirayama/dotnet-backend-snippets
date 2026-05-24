using System.Reflection;
using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Delegate Event Attribute Samples のスニペット動作を確認する。
public sealed class DelegateEventAttributeSamplesTests
{
    // テスト意図: Apply Discount / Uses Delegate を確認する。
    [Fact]
    public void ApplyDiscount_UsesDelegate()
    {
        decimal TenPercentOff(decimal amount) => amount * 0.9m;

        var result = DelegateEventAttributeSamples.ApplyDiscount(100m, TenPercentOff);

        Assert.Equal(90m, result);
    }

    // テスト意図: For Each Matching / Combines Func And Action を確認する。
    [Fact]
    public void ForEachMatching_CombinesFuncAndAction()
    {
        List<int> captured = [];

        DelegateEventAttributeSamples.ForEachMatching([1, 2, 3, 4], value => value % 2 == 0, captured.Add);

        Assert.Equal([2, 4], captured);
    }

    // テスト意図: Progress Reporter / Raises Event を確認する。
    [Fact]
    public void ProgressReporter_RaisesEvent()
    {
        var reporter = new ProgressReporter();
        var reportedPercent = -1;

        reporter.ProgressChanged += (_, args) => reportedPercent = args.Percent;
        reporter.Report(25);

        Assert.Equal(25, reportedPercent);
    }

    // テスト意図: Get Snippet Tags / Reads Custom Attributes を確認する。
    [Fact]
    public void GetSnippetTags_ReadsCustomAttributes()
    {
        var method = typeof(DelegateEventAttributeSamples)
            .GetMethod(nameof(DelegateEventAttributeSamples.TaggedDiscount), BindingFlags.Public | BindingFlags.Static);

        var result = DelegateEventAttributeSamples.GetSnippetTags(method!);

        Assert.Equal(["delegate", "attribute"], result);
    }
}
