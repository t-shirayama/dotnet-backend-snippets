using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotnetBackendSnippets.Serialization;

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
    /// 確定済み状態です。
    /// </summary>
    Submitted,

    /// <summary>
    /// キャンセル済み状態です。
    /// </summary>
    Cancelled,
}

/// <summary>
/// API で返す注文 DTO を表します。
/// </summary>
/// <param name="Id">注文 ID。</param>
/// <param name="CustomerName">顧客名。</param>
/// <param name="Status">注文状態。</param>
/// <param name="CreatedAt">作成日時。</param>
/// <param name="InternalNote">内部メモ。null の場合は JSON に出力しません。</param>
public sealed record OrderDto(
    Guid Id,
    string CustomerName,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    string? InternalNote);

/// <summary>
/// 未知の JSON フィールドを保持できるリクエストを表します。
/// </summary>
public sealed class FlexibleOrderRequest
{
    /// <summary>
    /// 顧客名を取得または設定します。
    /// </summary>
    /// <value>注文者の名前。</value>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 未知の JSON フィールドを取得します。
    /// </summary>
    /// <value>後方互換性確認やログ用に残す追加フィールド。</value>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// <see cref="System.Text.Json"/> を使うシリアライズサンプルです。
/// </summary>
public static class SerializationSamples
{
    /// <summary>
    /// Web API 向けの JSON オプションを作成します。
    /// </summary>
    /// <returns>camelCase、文字列 enum、null 省略を設定した JSON オプション。</returns>
    public static JsonSerializerOptions CreateWebApiJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };

        options.Converters.Add(new JsonStringEnumConverter<OrderStatus>(JsonNamingPolicy.CamelCase));
        return options;
    }

    /// <summary>
    /// 注文 DTO を Web API 向け JSON に変換します。
    /// </summary>
    /// <param name="order">変換する注文 DTO。</param>
    /// <returns>JSON 文字列。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="order"/> が <see langword="null"/> の場合。</exception>
    public static string SerializeOrder(OrderDto order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return JsonSerializer.Serialize(order, CreateWebApiJsonOptions());
    }

    /// <summary>
    /// JSON から注文 DTO を読み取ります。
    /// </summary>
    /// <param name="json">読み取る JSON。</param>
    /// <returns>復元した注文 DTO。</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> が空白の場合。</exception>
    /// <exception cref="JsonException">JSON が注文 DTO として不正な場合。</exception>
    public static OrderDto DeserializeOrder(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        return JsonSerializer.Deserialize<OrderDto>(json, CreateWebApiJsonOptions())
            ?? throw new JsonException("JSON did not contain an order.");
    }

    /// <summary>
    /// 未知のフィールドを保持しながらリクエスト JSON を読み取ります。
    /// </summary>
    /// <param name="json">読み取る JSON。</param>
    /// <returns>既知フィールドと未知フィールドを持つリクエスト。</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> が空白の場合。</exception>
    /// <exception cref="JsonException">JSON がリクエストとして不正な場合。</exception>
    public static FlexibleOrderRequest DeserializeFlexibleOrderRequest(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        return JsonSerializer.Deserialize<FlexibleOrderRequest>(json, CreateWebApiJsonOptions())
            ?? throw new JsonException("JSON did not contain an order request.");
    }

    /// <summary>
    /// snake_case を使う外部 API 向け JSON オプションを作成します。
    /// </summary>
    /// <returns>snake_case lower のプロパティ名を使う JSON オプション。</returns>
    public static JsonSerializerOptions CreateSnakeCaseJsonOptions()
    {
        var options = CreateWebApiJsonOptions();
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        return options;
    }
}
