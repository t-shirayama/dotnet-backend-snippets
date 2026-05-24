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
    public static int ParseIntOrDefault(string? value, int defaultValue = 0)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    /// <summary>
    /// InvariantCulture で decimal を解析します。
    /// </summary>
    public static bool TryParseDecimalInvariant(string? value, out decimal result)
    {
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// null の decimal を既定値へ置き換えます。
    /// </summary>
    public static decimal DefaultIfNull(decimal? value, decimal defaultValue = 0m)
    {
        return value ?? defaultValue;
    }

    /// <summary>
    /// 整数が 1 以上であることを検証します。
    /// </summary>
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
    public static bool IsRefund(decimal amount)
    {
        return amount < 0m;
    }

    /// <summary>
    /// 金額が上限以下であることを検証します。
    /// </summary>
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

