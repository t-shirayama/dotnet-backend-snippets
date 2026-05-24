namespace DotnetBackendSnippets.LanguageFeatures;

public static class PatternMatchingSamples
{
    public static string DescribeValue(object? value)
    {
        return value switch
        {
            null => "null",
            string { Length: 0 } => "empty string",
            string text => $"string:{text.Length}",
            int number when number >= 0 => "non-negative integer",
            int => "negative integer",
            IReadOnlyList<int> { Count: > 0 } numbers => $"integer list starting with {numbers[0]}",
            _ => "other",
        };
    }

    public static string DescribeNumbers(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        return numbers switch
        {
            [] => "empty",
            [var first, ..] => $"integer array starting with {first}",
        };
    }

    public static string ClassifyShipment(Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        return shipment switch
        {
            { DeliveredAt: not null } => "delivered",
            { TrackingNumber.Length: > 0 } => "in transit",
            _ => "preparing",
        };
    }

    public static string ClassifyPoint(Coordinate coordinate)
    {
        return coordinate switch
        {
            (0, 0) => "origin",
            (0, _) => "y-axis",
            (_, 0) => "x-axis",
            (> 0, > 0) => "quadrant-1",
            _ => "other",
        };
    }
}

public sealed record Shipment(string TrackingNumber, DateTimeOffset? DeliveredAt);

public readonly record struct Coordinate(int X, int Y);
