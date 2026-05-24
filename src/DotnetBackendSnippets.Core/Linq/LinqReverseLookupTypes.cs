namespace DotnetBackendSnippets.Linq;

/// <summary>
/// 注文一覧の並び替え列です。
/// </summary>
public enum OrderSortColumn
{
    /// <summary>
    /// 注文 ID で並び替えます。
    /// </summary>
    Id,

    /// <summary>
    /// 顧客 ID で並び替えます。
    /// </summary>
    CustomerId,

    /// <summary>
    /// カテゴリで並び替えます。
    /// </summary>
    Category,

    /// <summary>
    /// 金額で並び替えます。
    /// </summary>
    Amount,
}

/// <summary>
/// 並び替え方向です。
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// 昇順で並び替えます。
    /// </summary>
    Ascending,

    /// <summary>
    /// 降順で並び替えます。
    /// </summary>
    Descending,
}

/// <summary>
/// 注文検索条件です。
/// </summary>
/// <param name="CustomerId">絞り込む顧客 ID。</param>
/// <param name="Category">絞り込むカテゴリ。</param>
/// <param name="MinimumAmount">最小金額。</param>
/// <param name="MaximumAmount">最大金額。</param>
public sealed record OrderSearchCriteria(
    int? CustomerId = null,
    string? Category = null,
    decimal? MinimumAmount = null,
    decimal? MaximumAmount = null);

/// <summary>
/// 注文一覧に表示する項目です。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Category">カテゴリ。</param>
/// <param name="Amount">金額。</param>
public sealed record OrderListItem(int Id, int CustomerId, string Category, decimal Amount);

/// <summary>
/// LINQ で逆引きしやすい実務向けサンプルを提供します。

