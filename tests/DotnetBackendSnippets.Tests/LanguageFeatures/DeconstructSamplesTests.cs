using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

public sealed class DeconstructSamplesTests
{
    [Fact]
    public void FormatCustomer_UsesCustomDeconstructOrder()
    {
        var result = DeconstructSamples.FormatCustomer(new CustomerName("太郎", "山田"));

        Assert.Equal("山田 太郎", result);
    }

    [Fact]
    public void FormatRecordProduct_UsesRecordDeconstruction()
    {
        var result = DeconstructSamples.FormatRecordProduct(new ProductCode("BOOK", 12));

        Assert.Equal("BOOK-0012", result);
    }
}
