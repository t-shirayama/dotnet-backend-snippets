namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// 継承、インターフェース、演算子などのオブジェクト指向機能を示します。
/// </summary>
public static class ObjectOrientedSamples
{
    /// <summary>
    /// 割引ポリシーを使って割引後の金額を計算します。
    /// </summary>
    /// <param name="amount">割引前の金額。</param>
    /// <param name="policy">適用する割引ポリシー。</param>
    /// <returns>割引後の金額。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="policy"/> が null です。</exception>
    public static decimal CalculateDiscountedAmount(decimal amount, DiscountPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return amount - policy.CalculateDiscount(amount);
    }

    /// <summary>
    /// 税額計算器を使って税込金額を計算します。
    /// </summary>
    /// <param name="amount">税抜金額。</param>
    /// <param name="taxCalculator">税額計算器。</param>
    /// <returns>税込金額。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="taxCalculator"/> が null です。</exception>
    public static decimal CalculateTotalWithTax(decimal amount, ITaxCalculator taxCalculator)
    {
        ArgumentNullException.ThrowIfNull(taxCalculator);

        return amount + taxCalculator.CalculateTax(amount);
    }
}

/// <summary>
/// 割引計算の基底ポリシーです。
/// </summary>
public abstract class DiscountPolicy
{
    /// <summary>
    /// 指定金額に対する割引額を計算します。
    /// </summary>
    /// <param name="amount">割引前の金額。</param>
    /// <returns>割引額。</returns>
    public abstract decimal CalculateDiscount(decimal amount);
}

/// <summary>
/// 割引率で割引額を計算するポリシーです。
/// </summary>
public sealed class PercentageDiscountPolicy : DiscountPolicy
{
    private readonly decimal rate;

    /// <summary>
    /// <see cref="PercentageDiscountPolicy"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="rate">0 から 1 までの割引率。</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rate"/> が範囲外です。</exception>
    public PercentageDiscountPolicy(decimal rate)
    {
        if (rate is < 0m or > 1m)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be between 0 and 1.");
        }

        this.rate = rate;
    }

    /// <summary>
    /// 割引率に基づいて割引額を計算します。
    /// </summary>
    /// <param name="amount">割引前の金額。</param>
    /// <returns>割引額。</returns>
    public override decimal CalculateDiscount(decimal amount)
    {
        return amount * rate;
    }
}

/// <summary>
/// 税額計算器の契約です。
/// </summary>
public interface ITaxCalculator
{
    /// <summary>
    /// 指定金額に対する税額を計算します。
    /// </summary>
    /// <param name="amount">税抜金額。</param>
    /// <returns>税額。</returns>
    decimal CalculateTax(decimal amount);
}

/// <summary>
/// 固定税率で税額を計算する実装です。
/// </summary>
/// <param name="rate">税率。</param>
public sealed class FixedRateTaxCalculator(decimal rate) : ITaxCalculator
{
    /// <summary>
    /// 固定税率で税額を計算します。
    /// </summary>
    /// <param name="amount">税抜金額。</param>
    /// <returns>税額。</returns>
    public decimal CalculateTax(decimal amount)
    {
        return amount * rate;
    }
}

/// <summary>
/// 金額と通貨をまとめた値オブジェクトです。
/// </summary>
/// <param name="Amount">金額。</param>
/// <param name="Currency">通貨コード。</param>
public readonly record struct Money(decimal Amount, string Currency) : IComparable<Money>
{
    /// <summary>
    /// 同じ通貨の金額を加算します。
    /// </summary>
    /// <param name="left">左辺の金額。</param>
    /// <param name="right">右辺の金額。</param>
    /// <returns>加算後の金額。</returns>
    /// <exception cref="InvalidOperationException">通貨が一致しません。</exception>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// 同じ通貨の金額を小なり比較します。
    /// </summary>
    /// <param name="left">左辺の金額。</param>
    /// <param name="right">右辺の金額。</param>
    /// <returns>左辺が右辺より小さい場合は true。</returns>
    /// <exception cref="InvalidOperationException">通貨が一致しません。</exception>
    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return left.Amount < right.Amount;
    }

    /// <summary>
    /// 同じ通貨の金額を大なり比較します。
    /// </summary>
    /// <param name="left">左辺の金額。</param>
    /// <param name="right">右辺の金額。</param>
    /// <returns>左辺が右辺より大きい場合は true。</returns>
    /// <exception cref="InvalidOperationException">通貨が一致しません。</exception>
    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return left.Amount > right.Amount;
    }

    /// <summary>
    /// 同じ通貨の金額を比較します。
    /// </summary>
    /// <param name="other">比較対象の金額。</param>
    /// <returns>現在の値と比較対象の順序。</returns>
    /// <exception cref="InvalidOperationException">通貨が一致しません。</exception>
    public int CompareTo(Money other)
    {
        EnsureSameCurrency(this, other);

        return Amount.CompareTo(other.Amount);
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (!string.Equals(left.Currency, right.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Money values must use the same currency.");
        }
    }
}

/// <summary>
/// ヘッダー名で値を取得できる簡易コレクションです。
/// </summary>
public sealed class HeaderBag
{
    private readonly List<KeyValuePair<string, string>> headers = [];

    /// <summary>
    /// ヘッダー名から値を取得します。
    /// </summary>
    /// <param name="key">ヘッダー名。</param>
    /// <returns>一致した値。見つからない場合は空文字列。</returns>
    /// <exception cref="ArgumentException"><paramref name="key"/> が空白の場合。</exception>
    public string this[string key]
    {
        get
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var index = headers.FindIndex(
                header => string.Equals(header.Key, key, StringComparison.OrdinalIgnoreCase));

            return index < 0 ? string.Empty : headers[index].Value;
        }
    }

    /// <summary>
    /// 追加順のインデックスでヘッダーを取得します。
    /// </summary>
    /// <param name="index">取得するインデックス。</param>
    /// <returns>ヘッダーのキーと値。</returns>
    public KeyValuePair<string, string> this[int index] => headers[index];

    /// <summary>
    /// ヘッダーを追加します。
    /// </summary>
    /// <param name="key">ヘッダー名。</param>
    /// <param name="value">ヘッダー値。</param>
    /// <exception cref="ArgumentException"><paramref name="key"/> が空白の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public void Add(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        headers.Add(KeyValuePair.Create(key, value));
    }
}
