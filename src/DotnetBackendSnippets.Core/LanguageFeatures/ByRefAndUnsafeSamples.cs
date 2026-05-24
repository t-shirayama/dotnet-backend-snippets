namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// ref、in、out、unsafe の基本的な使い方を示します。
/// </summary>
public static class ByRefAndUnsafeSamples
{
    /// <summary>
    /// 参照渡しされた整数を 1 増やします。
    /// </summary>
    /// <param name="value">更新対象の整数。</param>
    public static void Increment(ref int value)
    {
        value++;
    }

    /// <summary>
    /// 読み取り専用参照の測定値から合計金額を計算します。
    /// </summary>
    /// <param name="measurement">単価と数量を持つ測定値。</param>
    /// <returns>単価と数量を掛けた合計。</returns>
    public static decimal CalculateTotal(in Measurement measurement)
    {
        return measurement.UnitPrice * measurement.Quantity;
    }

    /// <summary>
    /// 正の整数として解析できるかを確認します。
    /// </summary>
    /// <param name="value">解析する文字列。</param>
    /// <param name="result">解析できた正の整数。</param>
    /// <returns>正の整数として解析できた場合は true。</returns>
    public static bool TryParsePositiveInt(string? value, out int result)
    {
        if (int.TryParse(value, out var parsed) && parsed > 0)
        {
            result = parsed;
            return true;
        }

        result = 0;
        return false;
    }

    /// <summary>
    /// 固定ポインターを使って配列の先頭要素を読み取ります。
    /// </summary>
    /// <param name="values">読み取り対象の整数配列。</param>
    /// <returns>配列の先頭要素。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> が null です。</exception>
    /// <exception cref="ArgumentException"><paramref name="values"/> が空です。</exception>
    public static int ReadFirstWithPinnedPointer(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length == 0)
        {
            throw new ArgumentException("Values must contain at least one item.", nameof(values));
        }

        unsafe
        {
            fixed (int* pointer = values)
            {
                return *pointer;
            }
        }
    }
}

/// <summary>
/// 単価と数量を表す測定値です。
/// </summary>
/// <param name="UnitPrice">単価。</param>
/// <param name="Quantity">数量。</param>
public readonly record struct Measurement(decimal UnitPrice, int Quantity);
