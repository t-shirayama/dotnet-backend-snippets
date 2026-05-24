namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// double と decimal の加算結果の違いを表します。
    /// </summary>
    /// <param name="DoubleSum">double で 0.1 + 0.2 を計算した結果。</param>
    /// <param name="DecimalSum">decimal で 0.1 + 0.2 を計算した結果。</param>
    /// <param name="DoubleEqualsExpected">double の結果が 0.3 と完全一致するかどうか。</param>
    /// <param name="DecimalEqualsExpected">decimal の結果が 0.3 と完全一致するかどうか。</param>
    public readonly record struct FloatingPointComparison(
        double DoubleSum,
        decimal DecimalSum,
        bool DoubleEqualsExpected,
        bool DecimalEqualsExpected);

    /// <summary>
    /// double を絶対許容誤差と相対許容誤差で比較します。
    /// </summary>
    /// <param name="left">比較する左辺。</param>
    /// <param name="right">比較する右辺。</param>
    /// <param name="absoluteTolerance">値が 0 に近い場合に使う絶対許容誤差。</param>
    /// <param name="relativeTolerance">値の大きさに比例させる相対許容誤差。</param>
    /// <returns>許容誤差内なら <see langword="true"/>。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="absoluteTolerance"/> または <paramref name="relativeTolerance"/> が不正な場合。</exception>
    public static bool AreNearlyEqual(
        double left,
        double right,
        double absoluteTolerance = 1e-12,
        double relativeTolerance = 1e-12)
    {
        ValidateTolerance(absoluteTolerance, nameof(absoluteTolerance));
        ValidateTolerance(relativeTolerance, nameof(relativeTolerance));

        if (double.IsNaN(left) || double.IsNaN(right))
        {
            return false;
        }

        if (left.Equals(right))
        {
            return true;
        }

        if (double.IsInfinity(left) || double.IsInfinity(right))
        {
            return false;
        }

        var difference = Math.Abs(left - right);
        if (difference <= absoluteTolerance)
        {
            return true;
        }

        var largestMagnitude = Math.Max(Math.Abs(left), Math.Abs(right));

        return difference <= largestMagnitude * relativeTolerance;
    }

    /// <summary>
    /// double が 0 に近い値かどうかを判定します。
    /// </summary>
    /// <param name="value">判定する値。</param>
    /// <param name="tolerance">0 とみなす許容誤差。</param>
    /// <returns>0 近傍なら <see langword="true"/>。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> が不正な場合。</exception>
    public static bool IsNearlyZero(double value, double tolerance = 1e-12)
    {
        ValidateTolerance(tolerance, nameof(tolerance));

        return !double.IsNaN(value) && Math.Abs(value) <= tolerance;
    }

    /// <summary>
    /// 分母が 0 近傍の場合に既定値を返して double の割り算をします。
    /// </summary>
    /// <param name="numerator">分子。</param>
    /// <param name="denominator">分母。</param>
    /// <param name="defaultValue">分母が 0 近傍、または結果が有限でない場合に返す値。</param>
    /// <param name="zeroTolerance">0 とみなす許容誤差。</param>
    /// <returns>割り算の結果、または既定値。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="zeroTolerance"/> が不正な場合。</exception>
    public static double DivideOrDefault(double numerator, double denominator, double defaultValue = 0d, double zeroTolerance = 1e-12)
    {
        if (IsNearlyZero(denominator, zeroTolerance))
        {
            return defaultValue;
        }

        var result = numerator / denominator;

        return double.IsFinite(result) ? result : defaultValue;
    }

    /// <summary>
    /// Neumaier の補償和で double の累積誤差を抑えて合計します。
    /// </summary>
    /// <param name="values">合計する値のシーケンス。</param>
    /// <returns>Neumaier の補償和で計算した合計。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> が <see langword="null"/> の場合。</exception>
    public static double SumWithCompensation(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var sum = 0d;
        var compensation = 0d;

        foreach (var value in values)
        {
            var next = sum + value;
            compensation += Math.Abs(sum) >= Math.Abs(value)
                ? (sum - next) + value
                : (value - next) + sum;
            sum = next;
        }

        return sum + compensation;
    }

    /// <summary>
    /// double を有効桁数で丸めます。
    /// </summary>
    /// <param name="value">丸める値。</param>
    /// <param name="significantDigits">有効桁数。</param>
    /// <param name="midpointRounding">中間値の丸め方法。</param>
    /// <returns>指定した有効桁数へ丸めた値。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="significantDigits"/> が 1 から 15 の範囲外の場合。</exception>
    public static double RoundToSignificantDigits(
        double value,
        int significantDigits,
        MidpointRounding midpointRounding = MidpointRounding.ToEven)
    {
        if (significantDigits is < 1 or > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(significantDigits), "Significant digits must be between 1 and 15.");
        }

        if (value == 0d || !double.IsFinite(value))
        {
            return value;
        }

        var scale = Math.Pow(10d, Math.Floor(Math.Log10(Math.Abs(value))) + 1 - significantDigits);

        return Math.Round(value / scale, 0, midpointRounding) * scale;
    }

    /// <summary>
    /// double と decimal で 0.1 + 0.2 を計算し、完全一致比較の違いを示します。
    /// </summary>
    /// <returns>double と decimal の加算結果と 0.3 への完全一致結果。</returns>
    public static FloatingPointComparison CompareDoubleAndDecimalAddition()
    {
        var doubleSum = 0.1d + 0.2d;
        var decimalSum = 0.1m + 0.2m;

        return new FloatingPointComparison(
            doubleSum,
            decimalSum,
            doubleSum == 0.3d,
            decimalSum == 0.3m);
    }

    private static void ValidateTolerance(double tolerance, string parameterName)
    {
        if (double.IsNaN(tolerance) || double.IsInfinity(tolerance) || tolerance < 0d)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Tolerance must be zero or greater and finite.");
        }
    }
}
