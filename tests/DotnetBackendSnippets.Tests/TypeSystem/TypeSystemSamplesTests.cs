using DotnetBackendSnippets.TypeSystem;

namespace DotnetBackendSnippets.Tests.TypeSystem;

// テスト対象: Type System Samples のスニペット動作を確認する。
public sealed class TypeSystemSamplesTests
{
    // テスト意図: Has Same Value / Uses Record Value Equality を確認する。
    [Fact]
    public void HasSameValue_UsesRecordValueEquality()
    {
        var first = new Money(100m, "JPY");
        var second = new Money(100m, "JPY");

        var result = TypeSystemSamples.HasSameValue(first, second);

        Assert.True(result);
    }

    // テスト意図: Require Non Null / Returns Value / When Value Exists を確認する。
    [Fact]
    public void RequireNonNull_ReturnsValue_WhenValueExists()
    {
        var result = TypeSystemSamples.RequireNonNull("value", "sample");

        Assert.Equal("value", result);
    }

    // テスト意図: Require Non Null / Throws / When Value Is Null を確認する。
    [Fact]
    public void RequireNonNull_Throws_WhenValueIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => TypeSystemSamples.RequireNonNull(null, "sample"));

        Assert.Equal("sample", exception.ParamName);
    }

    // テスト意図: Try Parse Order Status / Parses Only Defined Enum Values を確認する。
    [Theory]
    [InlineData("submitted", OrderStatus.Submitted, true)]
    [InlineData("999", OrderStatus.Draft, false)]
    public void TryParseOrderStatus_ParsesOnlyDefinedEnumValues(
        string value,
        OrderStatus expectedStatus,
        bool expectedResult)
    {
        var result = TypeSystemSamples.TryParseOrderStatus(value, out var status);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedStatus, status);
    }

    // テスト意図: Describe Status / Uses Pattern Matching Switch を確認する。
    [Fact]
    public void DescribeStatus_UsesPatternMatchingSwitch()
    {
        var result = TypeSystemSamples.DescribeStatus(OrderStatus.Cancelled);

        Assert.Equal("Order will not be processed.", result);
    }

    // テスト意図: Create Result / Returns Success Or Failure Record を確認する。
    [Fact]
    public void CreateResult_ReturnsSuccessOrFailureRecord()
    {
        var success = TypeSystemSamples.CreateResult(succeeds: true, "ok");
        var failure = TypeSystemSamples.CreateResult(succeeds: false, "ignored");

        var successResult = Assert.IsType<Success<string>>(success);
        var failureResult = Assert.IsType<Failure<string>>(failure);

        Assert.Equal("ok", successResult.Value);
        Assert.Equal("Operation failed.", failureResult.Error);
    }

    // テスト意図: First Or None / Returns Maybe Without Value / When Source Is Empty を確認する。
    [Fact]
    public void FirstOrNone_ReturnsMaybeWithoutValue_WhenSourceIsEmpty()
    {
        var result = TypeSystemSamples.FirstOrNone<string>([]);

        Assert.False(result.HasValue);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    // テスト意図: First Or None / Returns Maybe With Value / For Value Types を確認する。
    [Fact]
    public void FirstOrNone_ReturnsMaybeWithValue_ForValueTypes()
    {
        var result = TypeSystemSamples.FirstOrNone([42, 100]);

        Assert.True(result.HasValue);
        Assert.Equal(42, result.Value);
    }
}
