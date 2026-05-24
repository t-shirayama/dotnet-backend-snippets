namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// decimal の最小値と最大値を表します。
    /// </summary>
    /// <param name="Min">最小値。</param>
    /// <param name="Max">最大値。</param>
    public readonly record struct DecimalRange(decimal Min, decimal Max);

    /// <summary>
    /// 一覧表示上の開始位置と終了位置を表します。
    /// </summary>
    /// <param name="Start">表示開始位置。</param>
    /// <param name="End">表示終了位置。</param>
    public readonly record struct DisplayRange(int Start, int End);

    /// <summary>
    /// 手数料の内訳を表します。
    /// </summary>
    /// <param name="PercentageFee">割合で計算した手数料。</param>
    /// <param name="FixedFee">固定手数料。</param>
    /// <param name="AppliedFee">実際に適用した手数料。</param>
    public readonly record struct FeeBreakdown(decimal PercentageFee, decimal FixedFee, decimal AppliedFee);

    /// <summary>
    /// ページング用の offset と limit を表します。
    /// </summary>
    /// <param name="Offset">読み飛ばす件数。</param>
    /// <param name="Limit">取得する件数。</param>
    public readonly record struct OffsetLimit(int Offset, int Limit);

    /// <summary>
    /// 税抜、税額、税込の内訳を表します。
    /// </summary>
    /// <param name="NetAmount">税抜金額。</param>
    /// <param name="TaxAmount">税額。</param>
    /// <param name="GrossAmount">税込金額。</param>
    public readonly record struct TaxBreakdown(decimal NetAmount, decimal TaxAmount, decimal GrossAmount);
}

