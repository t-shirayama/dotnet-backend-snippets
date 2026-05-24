namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値計算でよく使う実装例を提供します。
/// </summary>
public static class NumberSamples
{
    /// <summary>
    /// 値を指定範囲内に丸めます。
    /// </summary>
    /// <param name="value">対象の値。</param>
    /// <param name="min">最小値。</param>
    /// <param name="max">最大値。</param>
    /// <returns>範囲内に収めた値。</returns>
    /// <exception cref="ArgumentException"><paramref name="min"/> が <paramref name="max"/> より大きい場合。</exception>
    public static int Clamp(int value, int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(min));
        }

        return Math.Min(Math.Max(value, min), max);
    }

    /// <summary>
    /// 0 除算の場合に既定値を返して割り算します。
    /// </summary>
    /// <param name="numerator">分子。</param>
    /// <param name="denominator">分母。</param>
    /// <param name="defaultValue">分母が 0 の場合に返す値。</param>
    /// <returns>割り算の結果、または既定値。</returns>
    public static decimal DivideOrDefault(decimal numerator, decimal denominator, decimal defaultValue = 0m)
    {
        return denominator == 0m ? defaultValue : numerator / denominator;
    }

    /// <summary>
    /// 全体に対する割合をパーセントで計算します。
    /// </summary>
    /// <param name="part">部分の値。</param>
    /// <param name="whole">全体の値。</param>
    /// <param name="decimalPlaces">丸める小数点以下桁数。</param>
    /// <returns>パーセント値。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="decimalPlaces"/> が 0 未満の場合。</exception>
    public static decimal Percentage(decimal part, decimal whole, int decimalPlaces = 2)
    {
        if (decimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be zero or greater.");
        }

        if (whole == 0m)
        {
            return 0m;
        }

        return Math.Round(part / whole * 100m, decimalPlaces, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 金額を小数第 2 位までに丸めます。
    /// </summary>
    /// <param name="value">丸める金額。</param>
    /// <returns>丸めた金額。</returns>
    public static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 税抜金額に税率を適用し、金額として丸めます。
    /// </summary>
    /// <param name="netAmount">税抜金額。</param>
    /// <param name="taxRate">税率。10% は 0.10 を指定します。</param>
    /// <returns>税込金額。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="taxRate"/> が 0 未満の場合。</exception>
    public static decimal AddTax(decimal netAmount, decimal taxRate)
    {
        if (taxRate < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(taxRate), "Tax rate must be zero or greater.");
        }

        return RoundCurrency(netAmount * (1m + taxRate));
    }

    /// <summary>
    /// 値が指定範囲内にあるか判定します。
    /// </summary>
    /// <param name="value">対象の値。</param>
    /// <param name="min">最小値。</param>
    /// <param name="max">最大値。</param>
    /// <param name="inclusive">境界値を含めるかどうか。</param>
    /// <returns>範囲内なら <see langword="true"/>。</returns>
    /// <exception cref="ArgumentException"><paramref name="min"/> が <paramref name="max"/> より大きい場合。</exception>
    public static bool IsBetween(int value, int min, int max, bool inclusive = true)
    {
        if (min > max)
        {
            throw new ArgumentException("Minimum must be less than or equal to maximum.", nameof(min));
        }

        return inclusive
            ? value >= min && value <= max
            : value > min && value < max;
    }
}
