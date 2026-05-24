using System.Globalization;

namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// 整数として解析し、失敗時は既定値を返します。
    /// </summary>
    /// <param name="value">解析する文字列。</param>
    /// <param name="defaultValue">解析に失敗した場合に返す値。</param>
    /// <returns>解析した整数、または既定値。</returns>
    public static int ParseIntOrDefault(string? value, int defaultValue = 0)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// InvariantCulture で decimal を解析します。
    /// </summary>
    /// <param name="value">解析する文字列。</param>
    /// <param name="result">解析できた decimal 値。</param>
    /// <returns>解析できた場合は true。</returns>
    public static bool TryParseDecimalInvariant(string? value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// null の decimal を既定値へ置き換えます。
    /// </summary>
    /// <param name="value">変換する nullable decimal。</param>
    /// <param name="defaultValue">値が null の場合に返す値。</param>
    /// <returns>元の値、または既定値。</returns>
    public static decimal DefaultIfNull(decimal? value, decimal defaultValue = 0m)
    {
        return value ?? defaultValue;
    }

    /// <summary>
    /// 整数が 1 以上であることを検証します。
    /// </summary>
    /// <param name="value">検証する整数。</param>
    /// <param name="parameterName">例外に使うパラメーター名。</param>
    /// <returns>検証済みの整数。</returns>
    public static int RequirePositiveInt(int value, string parameterName = "value")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be one or greater.");
        }

        return value;
    }

    /// <summary>
    /// decimal が 0 以上であることを検証します。
    /// </summary>
    /// <param name="value">検証する decimal 値。</param>
    /// <param name="parameterName">例外に使うパラメーター名。</param>
    /// <returns>検証済みの decimal 値。</returns>
    public static decimal RequireNonNegative(decimal value, string parameterName = "value")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (value < 0m)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Value must be zero or greater.");
        }

        return value;
    }

    /// <summary>
    /// 割合が 0 から 1 の範囲であることを検証します。
    /// </summary>
    /// <param name="rate">検証する割合。</param>
    /// <param name="parameterName">例外に使うパラメーター名。</param>
    /// <returns>検証済みの割合。</returns>
    public static decimal RequireFractionRate(decimal rate, string parameterName = "rate")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (rate is < 0m or > 1m)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Rate must be between 0 and 1.");
        }

        return rate;
    }

    /// <summary>
    /// 金額が返金を表す負数かを判定します。
    /// </summary>
    /// <param name="amount">判定する金額。</param>
    /// <returns>負数の場合は true。</returns>
    public static bool IsRefund(decimal amount)
    {
        return amount < 0m;
    }

    /// <summary>
    /// 金額が上限以下であることを検証します。
    /// </summary>
    /// <param name="amount">検証する金額。</param>
    /// <param name="maximumAmount">許可する最大金額。</param>
    /// <param name="parameterName">例外に使うパラメーター名。</param>
    /// <returns>検証済みの金額。</returns>
    public static decimal EnsureMaximumAmount(decimal amount, decimal maximumAmount, string parameterName = "amount")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

        if (amount > maximumAmount)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Amount exceeds the allowed maximum.");
        }

        return amount;
    }
}
