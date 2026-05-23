namespace DotnetBackendSnippets.TypeSystem;

public enum OrderStatus
{
    Draft,
    Submitted,
    Cancelled,
}

public sealed record Money(decimal Amount, string Currency);

public abstract record Result<T>;

public sealed record Success<T>(T Value) : Result<T>;

public sealed record Failure<T>(string Error) : Result<T>;

public readonly record struct Maybe<T>
{
    private readonly T _value;

    public Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    public bool HasValue { get; }

    public T Value => HasValue
        ? _value
        : throw new InvalidOperationException("Maybe does not contain a value.");

    public static Maybe<T> None => default;
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

    public static Result<string> CreateResult(bool succeeds, string value)
    {
        return succeeds
            ? new Success<string>(value)
            : new Failure<string>("Operation failed.");
    }

    public static Maybe<T> FirstOrNone<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        using var enumerator = source.GetEnumerator();

        return enumerator.MoveNext()
            ? new Maybe<T>(enumerator.Current)
            : Maybe<T>.None;
    }
}
