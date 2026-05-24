using System.Reflection;
using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class DelegateEventAttributeSamplesTests
{
    [Fact]
    public void ApplyDiscount_UsesDelegate()
    {
        decimal TenPercentOff(decimal amount) => amount * 0.9m;

        var result = DelegateEventAttributeSamples.ApplyDiscount(100m, TenPercentOff);

        Assert.Equal(90m, result);
    }

    [Fact]
    public void ForEachMatching_CombinesFuncAndAction()
    {
        List<int> captured = [];

        DelegateEventAttributeSamples.ForEachMatching([1, 2, 3, 4], value => value % 2 == 0, captured.Add);

        Assert.Equal([2, 4], captured);
    }

    [Fact]
    public void ProgressReporter_RaisesEvent()
    {
        var reporter = new ProgressReporter();
        var reportedPercent = -1;

        reporter.ProgressChanged += (_, args) => reportedPercent = args.Percent;
        reporter.Report(25);

        Assert.Equal(25, reportedPercent);
    }

    [Fact]
    public void GetSnippetTags_ReadsCustomAttributes()
    {
        var method = typeof(DelegateEventAttributeSamples)
            .GetMethod(nameof(DelegateEventAttributeSamples.TaggedDiscount), BindingFlags.Public | BindingFlags.Static);

        var result = DelegateEventAttributeSamples.GetSnippetTags(method!);

        Assert.Equal(["delegate", "attribute"], result);
    }
}
