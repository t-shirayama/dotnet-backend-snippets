namespace DotnetBackendSnippets.Numbers;

/// <summary>
/// 数値処理で逆引きしやすい実務向けサンプルを分類別に提供します。
/// </summary>
public static partial class NumberReverseLookupSamples
{
    /// <summary>
    /// ページ番号とページサイズから skip 件数を計算します。
    /// </summary>
    public static int CalculateSkip(int pageNumber, int pageSize)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));
        RequirePositiveInt(pageSize, nameof(pageSize));

        return checked((pageNumber - 1) * pageSize);
    }

    /// <summary>
    /// 総件数とページサイズから総ページ数を計算します。
    /// </summary>
    public static int CalculateTotalPages(int totalCount, int pageSize)
    {
        RequireNonNegative(totalCount, nameof(totalCount));
        RequirePositiveInt(pageSize, nameof(pageSize));

        return totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (decimal)pageSize);
    }

    /// <summary>
    /// 指定ページが最終ページかを判定します。
    /// </summary>
    public static bool IsLastPage(int pageNumber, int totalCount, int pageSize)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));

        var totalPages = CalculateTotalPages(totalCount, pageSize);

        return pageNumber >= Math.Max(1, totalPages);
    }

    /// <summary>
    /// ページ指定を offset と limit に変換します。
    /// </summary>
    public static OffsetLimit ToOffsetLimit(int pageNumber, int pageSize)
    {
        return new OffsetLimit(CalculateSkip(pageNumber, pageSize), pageSize);
    }

    /// <summary>
    /// 一覧表示上の開始位置と終了位置を計算します。
    /// </summary>
    public static DisplayRange GetDisplayRange(int pageNumber, int pageSize, int totalCount)
    {
        RequirePositiveInt(pageNumber, nameof(pageNumber));
        RequirePositiveInt(pageSize, nameof(pageSize));
        RequireNonNegative(totalCount, nameof(totalCount));

        if (totalCount == 0)
        {
            return new DisplayRange(0, 0);
        }

        var skip = checked((long)(pageNumber - 1) * pageSize);
        if (skip >= totalCount)
        {
            return new DisplayRange(0, 0);
        }

        return new DisplayRange((int)skip + 1, (int)Math.Min(skip + pageSize, totalCount));
    }

    /// <summary>
    /// ページサイズを既定値と上限の範囲に丸めます。
    /// </summary>
    public static int ClampPageSize(int pageSize, int maximumPageSize, int defaultPageSize = 20)
    {
        RequirePositiveInt(maximumPageSize, nameof(maximumPageSize));
        RequirePositiveInt(defaultPageSize, nameof(defaultPageSize));

        if (pageSize < 1)
        {
            return Math.Min(defaultPageSize, maximumPageSize);
        }

        return Math.Min(pageSize, maximumPageSize);
    }
}

