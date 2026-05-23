namespace DotnetBackendSnippets.ErrorHandling;

public sealed record OperationResult<T>(bool Succeeded, T? Value, string? Error)
{
    public static OperationResult<T> Success(T value) => new(true, value, null);

    public static OperationResult<T> Failure(string error) => new(false, default, error);
}

public static class ErrorHandlingSamples
{
    public static OperationResult<int> TryParsePositiveInt(string value)
    {
        if (!int.TryParse(value, out var number))
        {
            return OperationResult<int>.Failure("Value must be an integer.");
        }

        if (number <= 0)
        {
            return OperationResult<int>.Failure("Value must be positive.");
        }

        return OperationResult<int>.Success(number);
    }

    public static void ThrowIfInvalidState(bool isValid)
    {
        if (!isValid)
        {
            throw new InvalidOperationException("The current state is invalid.");
        }
    }
}
