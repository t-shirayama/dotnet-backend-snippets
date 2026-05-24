namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// null 許容値型の基本的な扱い方を示します。
/// </summary>
public static class NullableValueSamples
{
    /// <summary>
    /// 数量が null の場合に既定値へ置き換えます。
    /// </summary>
    /// <param name="quantity">入力数量。</param>
    /// <param name="defaultQuantity">null の場合に使う既定値。</param>
    /// <returns>数量または既定値。</returns>
    public static int QuantityOrDefault(int? quantity, int defaultQuantity = 0)
    {
        return quantity.GetValueOrDefault(defaultQuantity);
    }

    /// <summary>
    /// 正の数量だけを残し、それ以外を null に正規化します。
    /// </summary>
    /// <param name="quantity">入力数量。</param>
    /// <returns>正の数量、または null。</returns>
    public static int? NormalizePositiveQuantity(int? quantity)
    {
        if (quantity is null)
        {
            return null;
        }

        return quantity.Value > 0 ? quantity.Value : null;
    }

    /// <summary>
    /// 正の数量を取り出せるかを確認します。
    /// </summary>
    /// <param name="quantity">入力数量。</param>
    /// <param name="value">取得できた正の数量。</param>
    /// <returns>正の数量を取得できた場合は true。</returns>
    public static bool TryGetPositiveQuantity(int? quantity, out int value)
    {
        if (quantity.HasValue && quantity.Value > 0)
        {
            value = quantity.Value;
            return true;
        }

        value = 0;
        return false;
    }
}
