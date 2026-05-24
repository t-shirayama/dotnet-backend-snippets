using System.Globalization;

namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// 桁区切り付きの数値文字列に整形します。
    /// </summary>
    public static string FormatThousands(decimal value, int decimalPlaces = 0, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 固定小数点の数値文字列に整形します。
    /// </summary>
    public static string FormatFixedDecimal(decimal value, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return value.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 通貨コード付きの金額文字列に整形します。
    /// </summary>
    public static string FormatCurrencyCode(
        decimal value,
        string currencyCode,
        int decimalPlaces = 2,
        IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        ValidateDecimalPlaces(decimalPlaces);

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{currencyCode.ToUpperInvariant()} {value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)}");
    }

    /// <summary>
    /// decimal の末尾ゼロを省いた文字列に整形します。
    /// </summary>
    public static string TrimTrailingZeros(decimal value, IFormatProvider? provider = null)
    {
        return value.ToString("0.############################", provider ?? CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 比率をパーセント文字列に整形します。
    /// </summary>
    public static string FormatPercent(decimal ratio, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        return (ratio * 100m).ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture) + "%";
    }

    /// <summary>
    /// 負数を括弧で表す会計形式に整形します。
    /// </summary>
    public static string FormatAccounting(decimal value, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        var absoluteValue = Math.Abs(value).ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture);

        return value < 0m ? $"({absoluteValue})" : absoluteValue;
    }

    /// <summary>
    /// 単位付きの数値文字列に整形します。
    /// </summary>
    public static string FormatWithUnit(decimal value, string unit, int decimalPlaces = 0, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unit);
        ValidateDecimalPlaces(decimalPlaces);

        return $"{value.ToString($"N{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} {unit}";
    }

    /// <summary>
    /// ファイルサイズを読みやすい単位付き文字列に整形します。
    /// </summary>
    public static string FormatFileSize(long bytes, int decimalPlaces = 2)
    {
        if (bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes must be zero or greater.");
        }

        ValidateDecimalPlaces(decimalPlaces);

        string[] units = ["B", "KB", "MB", "GB", "TB"];
        var value = (decimal)bytes;
        var unitIndex = 0;

        while (value >= 1024m && unitIndex < units.Length - 1)
        {
            value /= 1024m;
            unitIndex++;
        }

        return $"{TrimTrailingZeros(RoundAwayFromZero(value, decimalPlaces))} {units[unitIndex]}";
    }

    /// <summary>
    /// 時間間隔をミリ秒または秒の文字列に整形します。
    /// </summary>
    public static string FormatDuration(TimeSpan duration, int decimalPlaces = 2, IFormatProvider? provider = null)
    {
        ValidateDecimalPlaces(decimalPlaces);

        if (duration.TotalSeconds < 1d)
        {
            return $"{duration.TotalMilliseconds.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} ms";
        }

        return $"{duration.TotalSeconds.ToString($"F{decimalPlaces}", provider ?? CultureInfo.InvariantCulture)} sec";
    }
}

