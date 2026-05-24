namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// Int32 の加算がオーバーフローしないかを確認します。
    /// </summary>
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
    public static bool CanMultiplyWithoutExceeding(decimal left, decimal right, decimal maximumAbsoluteValue)
    {
        RequireNonNegative(maximumAbsoluteValue, nameof(maximumAbsoluteValue));

        return TryMultiplyDecimal(left, right, out var result) && Math.Abs(result) <= maximumAbsoluteValue;
    }

    /// <summary>
    /// 2 つの Int32 を Int64 として乗算します。
    /// </summary>
    public static long BigMultiply(int left, int right)
    {
        return Math.BigMul(left, right);
    }

    /// <summary>
    /// double が有限値かを判定します。
    /// </summary>
    public static bool IsFinite(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }
}

