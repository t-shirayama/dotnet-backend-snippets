using System.Globalization;
using System.Text.RegularExpressions;

namespace DotnetBackendSnippets.RegularExpressions;

/// <summary>
/// 正規表現の抽出、検証、置換、timeout のサンプルです。
/// </summary>
public static partial class RegularExpressionSamples
{
    private static readonly Regex SafeAsciiIdentifierRegex = new(
        @"^[A-Za-z][A-Za-z0-9_-]{2,31}$",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
        TimeSpan.FromMilliseconds(200));

    /// <summary>
    /// サンプルとして登録している正規表現 pattern の種類を表します。
    /// </summary>
    public enum RegularExpressionPatternKind
    {
        /// <summary>
        /// <c>ABC-1234</c> 形式の商品コード。
        /// </summary>
        ProductCode,

        /// <summary>
        /// URL slug などに使う英小文字、数字、hyphen の並び。
        /// </summary>
        Slug,

        /// <summary>
        /// HTTP header やログに載せる correlation id。
        /// </summary>
        CorrelationId,
    }

    /// <summary>
    /// ログ行から抽出した要素を表します。
    /// </summary>
    /// <param name="Timestamp">ログの時刻。</param>
    /// <param name="Level">ログレベル。</param>
    /// <param name="Message">ログメッセージ。</param>
    /// <param name="CorrelationId">ログに含まれる correlation id。</param>
    public readonly record struct ParsedLogEntry(
        DateTimeOffset Timestamp,
        string Level,
        string Message,
        string? CorrelationId);

    /// <summary>
    /// named group を使って構造化ログ風の 1 行を解析します。
    /// </summary>
    /// <param name="line">解析するログ行。</param>
    /// <returns>解析できた場合はログ要素。それ以外は <see langword="null"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="line"/> が <see langword="null"/> の場合。</exception>
    public static ParsedLogEntry? TryParseLogEntry(string line)
    {
        ArgumentNullException.ThrowIfNull(line);

        var match = LogEntryRegex().Match(line);
        if (!match.Success ||
            !DateTimeOffset.TryParse(
                match.Groups["timestamp"].Value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var timestamp))
        {
            return null;
        }

        var correlationId = match.Groups["correlationId"];

        return new ParsedLogEntry(
            timestamp,
            match.Groups["level"].Value,
            match.Groups["message"].Value,
            correlationId.Success ? correlationId.Value : null);
    }

    /// <summary>
    /// MatchEvaluator を使ってメールアドレスの user 名部分をマスクします。
    /// </summary>
    /// <param name="value">マスク対象文字列。</param>
    /// <returns>メールアドレスの user 名部分をマスクした文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string RedactEmailUserNames(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return EmailRegex().Replace(value, match => $"{match.Groups["first"].Value}***{match.Groups["domain"].Value}");
    }

    /// <summary>
    /// comma、semicolon、pipe、空白で文字列を分割します。
    /// </summary>
    /// <param name="value">分割対象文字列。</param>
    /// <returns>空要素を除外した token 一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static IReadOnlyList<string> SplitTokens(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return TokenSeparatorRegex()
            .Split(value.Trim())
            .Where(token => token.Length > 0)
            .ToArray();
    }

    /// <summary>
    /// 英字で始まり、英数字、hyphen、underscore だけを含む識別子か検証します。
    /// </summary>
    /// <param name="value">検証する文字列。</param>
    /// <returns>安全な ASCII 識別子の場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static bool IsSafeAsciiIdentifier(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return SafeAsciiIdentifierRegex.IsMatch(value);
    }

    /// <summary>
    /// Unicode の文字、数字、hyphen だけで構成されるか検証します。
    /// </summary>
    /// <param name="value">検証する文字列。</param>
    /// <returns>Unicode の文字、数字、hyphen だけの場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static bool ContainsOnlyUnicodeLettersNumbersAndHyphen(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return UnicodeLettersNumbersHyphenRegex().IsMatch(value);
    }

    /// <summary>
    /// 用途名から固定 pattern の正規表現を取得します。
    /// </summary>
    /// <param name="kind">取得する正規表現の種類。</param>
    /// <returns>指定した用途の正規表現。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> が未定義の場合。</exception>
    public static Regex GetRegisteredPattern(RegularExpressionPatternKind kind)
    {
        return kind switch
        {
            RegularExpressionPatternKind.ProductCode => ProductCodeRegex(),
            RegularExpressionPatternKind.Slug => SlugRegex(),
            RegularExpressionPatternKind.CorrelationId => CorrelationIdRegex(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), "Unknown regular expression pattern kind."),
        };
    }

    [GeneratedRegex(@"^(?<timestamp>\S+)\s+(?<level>[A-Z]+)(?:\s+\[(?<correlationId>[A-Za-z0-9_.:-]+)\])?\s+(?<message>.+)$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex LogEntryRegex();

    [GeneratedRegex(@"(?<first>[A-Za-z0-9._%+-])(?<rest>[A-Za-z0-9._%+-]*)(?<domain>@[A-Za-z0-9.-]+\.[A-Za-z]{2,})", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"[\s,;|]+", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex TokenSeparatorRegex();

    [GeneratedRegex(@"^[\p{L}\p{N}-]+$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex UnicodeLettersNumbersHyphenRegex();

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex SlugRegex();

    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9_.:-]{7,127}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex CorrelationIdRegex();
}
