using System.Reflection;

namespace DotnetBackendSnippets.LanguageFeatures;

/// <summary>
/// デリゲート、イベント、属性の基本的な使い方を示します。
/// </summary>
public static class DelegateEventAttributeSamples
{
    /// <summary>
    /// 渡された割引計算デリゲートを金額に適用します。
    /// </summary>
    /// <param name="amount">割引前の金額。</param>
    /// <param name="calculator">割引計算デリゲート。</param>
    /// <returns>計算後の金額。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="calculator"/> が null です。</exception>
    public static decimal ApplyDiscount(decimal amount, DiscountCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);

        return calculator(amount);
    }

    /// <summary>
    /// 条件に一致する要素にだけ処理を実行します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="values">処理対象の要素。</param>
    /// <param name="predicate">対象を選ぶ条件。</param>
    /// <param name="action">一致した要素に行う処理。</param>
    /// <exception cref="ArgumentNullException">いずれかの引数が null です。</exception>
    public static void ForEachMatching<T>(IEnumerable<T> values, Func<T, bool> predicate, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var value in values.Where(predicate))
        {
            action(value);
        }
    }

    /// <summary>
    /// メンバーに付与されたスニペットタグ名を取得します。
    /// </summary>
    /// <param name="member">属性を読むメンバー情報。</param>
    /// <returns>付与されているタグ名の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="member"/> が null です。</exception>
    public static IReadOnlyList<string> GetSnippetTags(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return member
            .GetCustomAttributes<SnippetTagAttribute>()
            .Select(attribute => attribute.Name)
            .ToList();
    }

    /// <summary>
    /// 属性付きメソッドとして使う割引計算サンプルです。
    /// </summary>
    /// <param name="amount">割引前の金額。</param>
    /// <returns>10% 割引後の金額。</returns>
    [SnippetTag("delegate")]
    [SnippetTag("attribute")]
    public static decimal TaggedDiscount(decimal amount)
    {
        return amount * 0.9m;
    }
}

/// <summary>
/// 金額から割引後の金額を計算するデリゲートです。
/// </summary>
/// <param name="amount">割引前の金額。</param>
/// <returns>割引後の金額。</returns>
public delegate decimal DiscountCalculator(decimal amount);

/// <summary>
/// 進捗変更イベントを通知するサンプルクラスです。
/// </summary>
public sealed class ProgressReporter
{
    /// <summary>
    /// 進捗が報告されたときに発生します。
    /// </summary>
    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;

    /// <summary>
    /// 進捗率を検証してイベントで通知します。
    /// </summary>
    /// <param name="percent">0 から 100 までの進捗率。</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="percent"/> が範囲外です。</exception>
    public void Report(int percent)
    {
        if (percent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(percent), "Percent must be between 0 and 100.");
        }

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percent));
    }
}

/// <summary>
/// 進捗変更イベントのデータです。
/// </summary>
/// <param name="percent">0 から 100 までの進捗率。</param>
public sealed class ProgressChangedEventArgs(int percent) : EventArgs
{
    /// <summary>
    /// 進捗率を取得します。
    /// </summary>
    /// <value>0 から 100 までの進捗率。</value>
    public int Percent { get; } = percent;
}

/// <summary>
/// スニペットにタグ名を付ける属性です。
/// </summary>
/// <param name="name">タグ名。</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class SnippetTagAttribute(string name) : Attribute
{
    /// <summary>
    /// タグ名を取得します。
    /// </summary>
    /// <value>タグ名。</value>
    public string Name { get; } = name;
}
