namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// パターンマッチングの代表的な書き方を示します。
/// </summary>
public static class PatternMatchingSamples
{
    /// <summary>
    /// 値の型や形に応じて説明文字列を返します。
    /// </summary>
    /// <param name="value">判定対象の値。</param>
    /// <returns>値の分類を表す文字列。</returns>
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

    /// <summary>
    /// 配列パターンで整数配列を説明します。
    /// </summary>
    /// <param name="numbers">判定対象の整数配列。</param>
    /// <returns>配列の状態を表す文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="numbers"/> が null です。</exception>
    public static string DescribeNumbers(int[] numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        return numbers switch { [] => "empty", [var first, ..] => $"integer array starting with {first}" };
    }

    /// <summary>
    /// 配送情報の状態を分類します。
    /// </summary>
    /// <param name="shipment">配送情報。</param>
    /// <returns>配送状態を表す文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="shipment"/> が null です。</exception>
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

    /// <summary>
    /// 座標を原点、軸、象限などに分類します。
    /// </summary>
    /// <param name="coordinate">判定対象の座標。</param>
    /// <returns>座標の分類を表す文字列。</returns>
    public static string ClassifyPoint(Coordinate coordinate)
    {
        return coordinate switch
        {
            (0, 0) => "origin",
            (0, _) => "y-axis",
            (_, 0) => "x-axis",
            ( > 0, > 0) => "quadrant-1",
            _ => "other",
        };
    }
}

/// <summary>
/// 配送状態の分類に使う配送情報です。
/// </summary>
/// <param name="TrackingNumber">追跡番号。</param>
/// <param name="DeliveredAt">配送完了日時。</param>
public sealed record Shipment(string TrackingNumber, DateTimeOffset? DeliveredAt);

/// <summary>
/// 2 次元座標です。
/// </summary>
/// <param name="X">X 座標。</param>
/// <param name="Y">Y 座標。</param>
public readonly record struct Coordinate(int X, int Y);
