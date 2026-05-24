namespace DotnetBackendSnippets.DtoMapping;

/// <summary>
/// 注文作成リクエスト DTO を表します。
/// </summary>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Lines">注文明細 DTO。</param>
/// <param name="IsAdmin">過剰投稿の例として受け取っても domain へ渡さない値。</param>
public sealed record CreateOrderRequestDto(string CustomerId, IReadOnlyList<CreateOrderLineRequestDto> Lines, bool IsAdmin);

/// <summary>
/// 注文明細リクエスト DTO を表します。
/// </summary>
/// <param name="Sku">商品 SKU。</param>
/// <param name="Quantity">数量。</param>
public sealed record CreateOrderLineRequestDto(string Sku, int Quantity);

/// <summary>
/// 注文作成 command を表します。
/// </summary>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="RequestedByUserId">操作ユーザー ID。</param>
/// <param name="Lines">注文明細 command。</param>
public sealed record CreateOrderCommand(string CustomerId, string RequestedByUserId, IReadOnlyList<CreateOrderLineCommand> Lines);

/// <summary>
/// 注文明細 command を表します。
/// </summary>
/// <param name="Sku">商品 SKU。</param>
/// <param name="Quantity">数量。</param>
public sealed record CreateOrderLineCommand(string Sku, int Quantity);

/// <summary>
/// 注文 domain entity を表します。
/// </summary>
public sealed class OrderEntity
{
    /// <summary>
    /// 注文 ID を取得または設定します。
    /// </summary>
    /// <value>永続化済み注文 ID。</value>
    public int Id { get; set; }

    /// <summary>
    /// 顧客 ID を取得または設定します。
    /// </summary>
    /// <value>注文者を表す ID。</value>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 注文状態を取得または設定します。
    /// </summary>
    /// <value>domain 側の状態文字列。</value>
    public string Status { get; set; } = "draft";

    /// <summary>
    /// 注文明細を取得します。
    /// </summary>
    /// <value>注文に含まれる明細。</value>
    public List<OrderLineEntity> Lines { get; } = [];
}

/// <summary>
/// 注文明細 domain entity を表します。
/// </summary>
public sealed class OrderLineEntity
{
    /// <summary>
    /// 商品 SKU を取得または設定します。
    /// </summary>
    /// <value>商品を表す SKU。</value>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// 数量を取得または設定します。
    /// </summary>
    /// <value>注文数量。</value>
    public int Quantity { get; set; }
}

/// <summary>
/// 注文レスポンス DTO を表します。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Status">注文状態。</param>
/// <param name="Lines">注文明細レスポンス。</param>
public sealed record OrderResponseDto(int Id, string CustomerId, string Status, IReadOnlyList<OrderLineResponseDto> Lines);

/// <summary>
/// v2 の注文レスポンス DTO を表します。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="Status">注文状態。</param>
/// <param name="LineCount">注文明細数。</param>
/// <param name="Links">関連リンク。</param>
public sealed record OrderResponseV2Dto(
    int Id,
    string CustomerId,
    string Status,
    int LineCount,
    IReadOnlyDictionary<string, string> Links);

/// <summary>
/// 注文明細レスポンス DTO を表します。
/// </summary>
/// <param name="Sku">商品 SKU。</param>
/// <param name="Quantity">数量。</param>
public sealed record OrderLineResponseDto(string Sku, int Quantity);

/// <summary>
/// 顧客プロフィール entity を表します。
/// </summary>
public sealed class CustomerProfileEntity
{
    /// <summary>
    /// 表示名を取得または設定します。
    /// </summary>
    /// <value>顧客向け表示名。</value>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 電話番号を取得または設定します。
    /// </summary>
    /// <value>連絡先電話番号。</value>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 管理者かどうかを取得または設定します。
    /// </summary>
    /// <value>権限に関わるため request DTO から直接更新しない値。</value>
    public bool IsAdmin { get; set; }
}

/// <summary>
/// 顧客プロフィール部分更新 DTO を表します。
/// </summary>
/// <param name="DisplayName">新しい表示名。null の場合は更新しません。</param>
/// <param name="PhoneNumber">新しい電話番号。null の場合は更新しません。</param>
/// <param name="IsAdmin">過剰投稿の例として無視する値。</param>
public sealed record UpdateCustomerProfileRequestDto(string? DisplayName, string? PhoneNumber, bool? IsAdmin);

/// <summary>
/// PATCH で未指定と null 指定を区別する値を表します。
/// </summary>
/// <typeparam name="T">値の型。</typeparam>
/// <param name="IsSpecified">リクエストに値が含まれているかどうか。</param>
/// <param name="Value">指定された値。</param>
public readonly record struct OptionalPatchValue<T>(bool IsSpecified, T? Value)
{
    /// <summary>
    /// 未指定の値を作成します。
    /// </summary>
    /// <returns>未指定を表す値。</returns>
    public static OptionalPatchValue<T> Unspecified() => new(false, default);

    /// <summary>
    /// 指定済みの値を作成します。
    /// </summary>
    /// <param name="value">指定された値。</param>
    /// <returns>指定済みを表す値。</returns>
    public static OptionalPatchValue<T> Specified(T? value) => new(true, value);
}

/// <summary>
/// 顧客プロフィールの optional patch DTO を表します。
/// </summary>
/// <param name="DisplayName">表示名の optional patch 値。</param>
/// <param name="PhoneNumber">電話番号の optional patch 値。</param>
public sealed record OptionalCustomerProfilePatchDto(
    OptionalPatchValue<string> DisplayName,
    OptionalPatchValue<string?> PhoneNumber);

/// <summary>
/// DTO と domain object / entity の手書き mapping サンプルです。
/// </summary>
public static class DtoMappingSamples
{
    /// <summary>
    /// request DTO を domain command に変換します。
    /// </summary>
    /// <param name="request">注文作成リクエスト DTO。</param>
    /// <param name="currentUserId">操作ユーザー ID。</param>
    /// <returns>domain 層へ渡す command。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> または明細コレクション内の値が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="currentUserId"/> が空白の場合。</exception>
    public static CreateOrderCommand ToCommand(CreateOrderRequestDto request, string currentUserId)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Lines);
        ArgumentException.ThrowIfNullOrWhiteSpace(currentUserId);

        return new CreateOrderCommand(
            request.CustomerId,
            currentUserId,
            request.Lines.Select(ToCommandLine).ToList());
    }

    /// <summary>
    /// entity を API レスポンス DTO に変換します。
    /// </summary>
    /// <param name="order">注文 entity。</param>
    /// <returns>API レスポンス DTO。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="order"/> が <see langword="null"/> の場合。</exception>
    public static OrderResponseDto ToResponse(OrderEntity order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new OrderResponseDto(
            order.Id,
            order.CustomerId,
            order.Status,
            order.Lines.Select(ToResponseLine).ToList());
    }

    /// <summary>
    /// 部分更新 DTO を entity に反映します。
    /// </summary>
    /// <param name="profile">更新対象 entity。</param>
    /// <param name="request">部分更新 DTO。</param>
    /// <exception cref="ArgumentNullException"><paramref name="profile"/> または <paramref name="request"/> が <see langword="null"/> の場合。</exception>
    public static void ApplyProfilePatch(CustomerProfileEntity profile, UpdateCustomerProfileRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(request);

        if (request.DisplayName is not null)
        {
            profile.DisplayName = request.DisplayName.Trim();
        }

        if (request.PhoneNumber is not null)
        {
            profile.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim();
        }
    }

    /// <summary>
    /// optional patch DTO を entity に反映します。
    /// </summary>
    /// <param name="profile">更新対象 entity。</param>
    /// <param name="request">optional patch DTO。</param>
    /// <exception cref="ArgumentNullException"><paramref name="profile"/> または <paramref name="request"/> が <see langword="null"/> の場合。</exception>
    public static void ApplyOptionalProfilePatch(CustomerProfileEntity profile, OptionalCustomerProfilePatchDto request)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(request);

        if (request.DisplayName.IsSpecified && !string.IsNullOrWhiteSpace(request.DisplayName.Value))
        {
            profile.DisplayName = request.DisplayName.Value.Trim();
        }

        if (request.PhoneNumber.IsSpecified)
        {
            profile.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber.Value)
                ? null
                : request.PhoneNumber.Value.Trim();
        }
    }

    /// <summary>
    /// entity を v2 API レスポンス DTO に変換します。
    /// </summary>
    /// <param name="order">注文 entity。</param>
    /// <param name="basePath">注文 API の base path。</param>
    /// <returns>v2 API レスポンス DTO。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="order"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="basePath"/> が空白の場合。</exception>
    public static OrderResponseV2Dto ToV2Response(OrderEntity order, string basePath)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentException.ThrowIfNullOrWhiteSpace(basePath);

        string trimmedBasePath = basePath.TrimEnd('/');

        return new OrderResponseV2Dto(
            order.Id,
            order.CustomerId,
            order.Status,
            order.Lines.Count,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["self"] = $"{trimmedBasePath}/{order.Id}",
                ["customer"] = $"/customers/{order.CustomerId}",
            });
    }

    private static CreateOrderLineCommand ToCommandLine(CreateOrderLineRequestDto line)
    {
        ArgumentNullException.ThrowIfNull(line);

        return new CreateOrderLineCommand(line.Sku, line.Quantity);
    }

    private static OrderLineResponseDto ToResponseLine(OrderLineEntity line)
    {
        ArgumentNullException.ThrowIfNull(line);

        return new OrderLineResponseDto(line.Sku, line.Quantity);
    }
}
