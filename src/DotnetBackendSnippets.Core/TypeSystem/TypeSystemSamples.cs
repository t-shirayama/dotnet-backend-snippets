namespace DotnetBackendSnippets.TypeSystem;

/// <summary>
/// 注文状態を表します。
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 下書き状態です。
    /// </summary>
    Draft,

    /// <summary>
    /// 送信済み状態です。
    /// </summary>
    Submitted,

    /// <summary>
    /// キャンセル済み状態です。
    /// </summary>
    Cancelled,
}

/// <summary>
/// 通貨付き金額を表します。
/// </summary>
/// <param name="Amount">金額。</param>
/// <param name="Currency">通貨コード。</param>
public sealed record Money(decimal Amount, string Currency);

/// <summary>
/// 成功または失敗を表す結果型の基底です。
/// </summary>
/// <typeparam name="T">成功時の値の型。</typeparam>
public abstract record Result<T>;

/// <summary>
/// 成功結果を表します。
/// </summary>
/// <typeparam name="T">成功値の型。</typeparam>
/// <param name="Value">成功時の値。</param>
public sealed record Success<T>(T Value) : Result<T>;

/// <summary>
/// 失敗結果を表します。
/// </summary>
/// <typeparam name="T">成功時の値の型。</typeparam>
/// <param name="Error">エラーメッセージ。</param>
public sealed record Failure<T>(string Error) : Result<T>;

/// <summary>
/// 値が存在する場合と存在しない場合を表します。
/// </summary>
/// <typeparam name="T">保持する値の型。</typeparam>
public readonly record struct Maybe<T>
{
    private readonly T _value;

    /// <summary>
    /// 値を持つ <see cref="Maybe{T}"/> を作成します。
    /// </summary>
    /// <param name="value">保持する値。</param>
    public Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>
    /// 値を保持しているかどうかを取得します。
    /// </summary>
    /// <value>値がある場合は <see langword="true"/>。</value>
    public bool HasValue { get; }

    /// <summary>
    /// 保持している値を取得します。
    /// </summary>
    /// <value>保持している値。</value>
    /// <exception cref="InvalidOperationException">値を保持していない場合。</exception>
    public T Value => HasValue
        ? _value
        : throw new InvalidOperationException("Maybe does not contain a value.");

    /// <summary>
    /// 値を持たない <see cref="Maybe{T}"/> を取得します。
    /// </summary>
    /// <value>値なしを表すインスタンス。</value>
    public static Maybe<T> None => default;
}

/// <summary>
/// C# の型システムに関する基本例を提供します。
/// </summary>
public static class TypeSystemSamples
{
    /// <summary>
    /// 2 つの金額値が同じかどうかを判定します。
    /// </summary>
    /// <param name="first">比較元の金額。</param>
    /// <param name="second">比較先の金額。</param>
    /// <returns>同じ値なら <see langword="true"/>。</returns>
    public static bool HasSameValue(Money first, Money second)
    {
        return first == second;
    }

    /// <summary>
    /// 値が null でないことを保証して返します。
    /// </summary>
    /// <param name="value">確認する値。</param>
    /// <param name="parameterName">例外に設定するパラメーター名。</param>
    /// <returns>null でない文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> が <see langword="null"/> の場合。</exception>
    public static string RequireNonNull(string? value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    /// <summary>
    /// 文字列を注文状態に変換します。
    /// </summary>
    /// <param name="value">変換する文字列。</param>
    /// <param name="status">変換できた注文状態。</param>
    /// <returns>変換できた場合は <see langword="true"/>。</returns>
    public static bool TryParseOrderStatus(string value, out OrderStatus status)
    {
        if (Enum.TryParse(value, ignoreCase: true, out status) && Enum.IsDefined(status))
        {
            return true;
        }

        status = default;
        return false;
    }

    /// <summary>
    /// 注文状態を説明文に変換します。
    /// </summary>
    /// <param name="status">説明する注文状態。</param>
    /// <returns>状態の説明文。</returns>
    public static string DescribeStatus(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Draft => "Order is still editable.",
            OrderStatus.Submitted => "Order is waiting for processing.",
            OrderStatus.Cancelled => "Order will not be processed.",
            _ => "Unknown order status.",
        };
    }

    /// <summary>
    /// 成功フラグに応じて結果型を作成します。
    /// </summary>
    /// <param name="succeeds">成功結果にするかどうか。</param>
    /// <param name="value">成功時の値。</param>
    /// <returns>成功または失敗の結果。</returns>
    public static Result<string> CreateResult(bool succeeds, string value)
    {
        return succeeds
            ? new Success<string>(value)
            : new Failure<string>("Operation failed.");
    }

    /// <summary>
    /// 先頭要素を取得し、存在しない場合は値なしを返します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">取得対象のシーケンス。</param>
    /// <returns>先頭要素を含む <see cref="Maybe{T}"/>、または値なし。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> が <see langword="null"/> の場合。</exception>
    public static Maybe<T> FirstOrNone<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        using var enumerator = source.GetEnumerator();

        return enumerator.MoveNext()
            ? new Maybe<T>(enumerator.Current)
            : Maybe<T>.None;
    }
}
