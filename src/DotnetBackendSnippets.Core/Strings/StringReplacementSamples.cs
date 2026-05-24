using System.Globalization;
using System.Text;
using System.Text.Json;

namespace DotnetBackendSnippets.Strings;

/// <summary>
/// 文字列処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class StringReverseLookupSamples
{
    /// <summary>
    /// メールアドレスのローカル部をマスクします。
    /// </summary>
    /// <param name="email">マスクするメールアドレス。</param>
    /// <returns>ローカル部を伏せ字にしたメールアドレス。</returns>
    public static string MaskEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        var atIndex = email.IndexOf('@', StringComparison.Ordinal);
        if (atIndex <= 0)
        {
            return StringSamples.MaskMiddle(email, 1, 1);
        }

        var localPart = email[..atIndex];
        var domain = email[atIndex..];

        return $"{StringSamples.MaskMiddle(localPart, 1, localPart.Length > 2 ? 1 : 0)}{domain}";
    }

    /// <summary>
    /// 電話番号の数字部分をマスクします。
    /// </summary>
    /// <param name="value">マスクする電話番号文字列。</param>
    /// <returns>数字部分の末尾 4 桁相当だけを残して伏せ字にした文字列。</returns>
    public static string MaskPhoneNumber(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var digits = ExtractDigits(value);

        return StringSamples.MaskMiddle(digits, 0, Math.Min(4, digits.Length));
    }

    /// <summary>
    /// カード番号を下 4 桁相当だけ残してマスクします。
    /// </summary>
    /// <param name="value">マスクするカード番号文字列。</param>
    /// <returns>数字部分の末尾 4 桁相当だけを残して伏せ字にした文字列。</returns>
    public static string MaskCardNumber(string value)
    {
        return MaskPhoneNumber(value);
    }

    /// <summary>
    /// 結合文字を考慮して文字列を切り詰めます。
    /// </summary>
    /// <param name="value">切り詰める文字列。</param>
    /// <param name="maxTextElements">残すテキスト要素数の最大値。</param>
    /// <param name="suffix">切り詰めた場合に末尾へ付ける文字列。</param>
    /// <returns>テキスト要素単位で切り詰めた文字列。</returns>
    public static string TruncateTextElements(string value, int maxTextElements, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxTextElements < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxTextElements), "Max text elements must be zero or greater.");
        }

        var elementIndexes = StringInfo.ParseCombiningCharacters(value);
        if (elementIndexes.Length <= maxTextElements)
        {
            return value;
        }

        if (maxTextElements == 0)
        {
            return string.Empty;
        }

        var cutIndex = elementIndexes[maxTextElements];

        return value[..cutIndex] + suffix;
    }

    /// <summary>
    /// 秘密情報らしい代入値を伏せ字にします。
    /// </summary>
    /// <param name="value">マスクする文字列。</param>
    /// <returns>秘密情報らしい代入値を伏せ字にした文字列。</returns>
    public static string RedactSecrets(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SecretAssignmentRegex().Replace(value, match => $"{match.Groups[1].Value}=***");
    }

    /// <summary>
    /// 指定した JSON フィールドの値をマスクします。
    /// </summary>
    /// <param name="json">マスク対象の JSON 文字列。</param>
    /// <param name="fieldNames">値をマスクするフィールド名。</param>
    /// <returns>指定フィールドの値をマスクした JSON 文字列。</returns>
    public static string MaskJsonFields(string json, ISet<string> fieldNames)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(fieldNames);

        using var document = JsonDocument.Parse(json);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteMaskedJson(document.RootElement, writer, fieldNames);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// 表示幅を考慮して文字列を切り詰めます。
    /// </summary>
    /// <param name="value">切り詰める文字列。</param>
    /// <param name="maxWidth">残す表示幅の最大値。</param>
    /// <param name="suffix">切り詰めた場合に末尾へ付ける文字列。</param>
    /// <returns>ASCII を幅 1、それ以外を幅 2 として切り詰めた文字列。</returns>
    public static string TruncateByDisplayWidth(string value, int maxWidth, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxWidth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "Max width must be zero or greater.");
        }

        var builder = new StringBuilder();
        var currentWidth = 0;

        foreach (var rune in value.EnumerateRunes())
        {
            var width = rune.Value <= 0x7F ? 1 : 2;
            if (currentWidth + width > maxWidth)
            {
                return builder.Append(suffix).ToString();
            }

            builder.Append(rune);
            currentWidth += width;
        }

        return builder.ToString();
    }
}
