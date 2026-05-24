namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// Int32 の加算がオーバーフローしないかを確認します。
    /// </summary>
    /// <param name="left">左辺の値。</param>
    /// <param name="right">右辺の値。</param>
    /// <param name="result">計算できた場合の加算結果。</param>
    /// <returns>オーバーフローせずに加算できた場合は true。</returns>
    public static bool TryAddInt32(int left, int right, out int result)
    {
        try
        {
            result = checked(left + right);
            return true;
        }
        catch (OverflowException)
        {
            result = 0;
            return false;
        }
    }

    /// <summary>
    /// Int32 の乗算がオーバーフローしないかを確認します。
    /// </summary>
    /// <param name="left">左辺の値。</param>
    /// <param name="right">右辺の値。</param>
    /// <param name="result">計算できた場合の乗算結果。</param>
    /// <returns>オーバーフローせずに乗算できた場合は true。</returns>
    public static bool TryMultiplyInt32(int left, int right, out int result)
    {
        try
        {
            result = checked(left * right);
            return true;
        }
        catch (OverflowException)
        {
            result = 0;
            return false;
        }
    }

    /// <summary>
    /// decimal の乗算がオーバーフローしないかを確認します。
    /// </summary>
    /// <param name="left">左辺の値。</param>
    /// <param name="right">右辺の値。</param>
    /// <param name="result">計算できた場合の乗算結果。</param>
    /// <returns>オーバーフローせずに乗算できた場合は true。</returns>
    public static bool TryMultiplyDecimal(decimal left, decimal right, out decimal result)
    {
        try
        {
            result = checked(left * right);
            return true;
        }
        catch (OverflowException)
        {
            result = 0m;
            return false;
        }
    }

    /// <summary>
    /// 乗算結果が指定した絶対値上限を超えないかを判定します。
    /// </summary>
    /// <param name="left">左辺の値。</param>
    /// <param name="right">右辺の値。</param>
    /// <param name="maximumAbsoluteValue">許可する乗算結果の絶対値上限。</param>
    /// <returns>乗算でき、かつ絶対値上限以下の場合は true。</returns>
    public static bool CanMultiplyWithoutExceeding(decimal left, decimal right, decimal maximumAbsoluteValue)
    {
        RequireNonNegative(maximumAbsoluteValue, nameof(maximumAbsoluteValue));

        return TryMultiplyDecimal(left, right, out var result) && Math.Abs(result) <= maximumAbsoluteValue;
    }

    /// <summary>
    /// 2 つの Int32 を Int64 として乗算します。
    /// </summary>
    /// <param name="left">左辺の値。</param>
    /// <param name="right">右辺の値。</param>
    /// <returns>Int64 としての乗算結果。</returns>
    public static long BigMultiply(int left, int right)
    {
        return Math.BigMul(left, right);
    }

    /// <summary>
    /// double が有限値かを判定します。
    /// </summary>
    /// <param name="value">判定する double 値。</param>
    /// <returns>NaN でも無限大でもない場合は true。</returns>
    public static bool IsFinite(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
