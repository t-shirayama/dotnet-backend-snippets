using DotnetBackendSnippets.TypeSystem;

namespace DotnetBackendSnippets.Tests.TypeSystem;

public sealed class TypeSystemSamplesTests
{
    [Fact]
    public void HasSameValue_UsesRecordValueEquality()
    {
        var first = new Money(100m, "JPY");
        var second = new Money(100m, "JPY");

        var result = TypeSystemSamples.HasSameValue(first, second);

        Assert.True(result);
    }

    [Fact]
    public void RequireNonNull_ReturnsValue_WhenValueExists()
    {
        var result = TypeSystemSamples.RequireNonNull("value", "sample");

        Assert.Equal("value", result);
    }

    [Fact]
    public void RequireNonNull_Throws_WhenValueIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => TypeSystemSamples.RequireNonNull(null, "sample"));

        Assert.Equal("sample", exception.ParamName);
    }

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

    [Fact]
    public void DescribeStatus_UsesPatternMatchingSwitch()
    {
        var result = TypeSystemSamples.DescribeStatus(OrderStatus.Cancelled);

        Assert.Equal("Order will not be processed.", result);
    }

    [Fact]
    public void CreateResult_ReturnsSuccessOrFailureRecord()
    {
        var success = TypeSystemSamples.CreateResult(succeeds: true, "ok");
        var failure = TypeSystemSamples.CreateResult(succeeds: false, "ignored");

        Assert.IsType<SuccessResult<string>>(success);
        Assert.IsType<FailureResult>(failure);
    }

    [Fact]
    public void FirstOrNone_ReturnsMaybeWithoutValue_WhenSourceIsEmpty()
    {
        var result = TypeSystemSamples.FirstOrNone<string>([]);

        Assert.False(result.HasValue);
        Assert.Null(result.Value);
    }
}
