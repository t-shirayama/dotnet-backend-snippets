using DotnetBackendSnippets.LanguageFeatures;

namespace DotnetBackendSnippets.Tests.LanguageFeatures;

// テスト対象: Deconstruct Samples のスニペット動作を確認する。
public sealed class DeconstructSamplesTests
{
    // テスト意図: Format Customer / Uses Custom Deconstruct Order を確認する。
    [Fact]
    public void FormatCustomer_UsesCustomDeconstructOrder()
    {
        var result = DeconstructSamples.FormatCustomer(new CustomerName("太郎", "山田"));

        Assert.Equal("山田 太郎", result);
    }

    // テスト意図: Format Record Product / Uses Record Deconstruction を確認する。
    [Fact]
    public void FormatRecordProduct_UsesRecordDeconstruction()
    {
        var result = DeconstructSamples.FormatRecordProduct(new ProductCode("BOOK", 12));

        Assert.Equal("BOOK-0012", result);
    }
}
