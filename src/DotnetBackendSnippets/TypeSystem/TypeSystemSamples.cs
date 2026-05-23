namespace DotnetBackendSnippets.TypeSystem;

public enum OrderStatus
{
    Draft,
    Submitted,
    Cancelled,
}

public sealed record Money(decimal Amount, string Currency);

public sealed record SuccessResult<T>(T Value);

public sealed record FailureResult(string Error);

public readonly record struct Maybe<T>(T? Value)
{
    public bool HasValue => Value is not null;
}

public static class TypeSystemSamples
{
    public static bool HasSameValue(Money first, Money second)
    {
        return first == second;
    }

    public static string RequireNonNull(string? value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    public static bool TryParseOrderStatus(string value, out OrderStatus status)
    {
        if (Enum.TryParse(value, ignoreCase: true, out status) && Enum.IsDefined(status))
        {
            return true;
        }

        status = default;
        return false;
    }

    public static string DescribeStatus(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Draft => "Order is still editable.",
            OrderStatus.Submitted => "Order is waiting for processing.",
            OrderStatus.Cancelled => "Order will not be processed.",
            _ => "Unknown order status.",
        };
    }

    public static object CreateResult(bool succeeds, string value)
    {
        return succeeds
            ? new SuccessResult<string>(value)
            : new FailureResult("Operation failed.");
    }

    public static Maybe<T> FirstOrNone<T>(IEnumerable<T> source)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(source);

        return new Maybe<T>(source.FirstOrDefault());
    }
}
