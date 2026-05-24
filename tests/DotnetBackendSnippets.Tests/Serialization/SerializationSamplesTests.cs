using System.Text.Json;
using DotnetBackendSnippets.Serialization;

namespace DotnetBackendSnippets.Tests.Serialization;

public sealed class SerializationSamplesTests
{
    [Fact]
    public void SerializeOrder_UsesCamelCaseStringEnumAndOmitsNull()
    {
        var order = new OrderDto(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Alice",
            OrderStatus.Submitted,
            new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero),
            InternalNote: null);

        string json = SerializationSamples.SerializeOrder(order);

        Assert.Contains("\"customerName\":\"Alice\"", json, StringComparison.Ordinal);
        Assert.Contains("\"status\":\"submitted\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("internalNote", json, StringComparison.Ordinal);
    }

    [Fact]
    public void DeserializeOrder_ReadsCaseInsensitivePropertiesAndStringEnum()
    {
        const string json = """
            {
              "ID": "11111111-1111-1111-1111-111111111111",
              "CUSTOMERNAME": "Alice",
              "STATUS": "submitted",
              "CREATEDAT": "2026-05-24T12:00:00+00:00"
            }
            """;

        OrderDto order = SerializationSamples.DeserializeOrder(json);

        Assert.Equal("Alice", order.CustomerName);
        Assert.Equal(OrderStatus.Submitted, order.Status);
    }

    [Fact]
    public void DeserializeFlexibleOrderRequest_KeepsUnknownFields()
    {
        const string json = """
            {
              "customerName": "Alice",
              "legacyCode": "L-001"
            }
            """;

        FlexibleOrderRequest request = SerializationSamples.DeserializeFlexibleOrderRequest(json);

        Assert.Equal("Alice", request.CustomerName);
        Assert.Equal("L-001", request.ExtensionData["legacyCode"].GetString());
    }

    [Fact]
    public void CreateSnakeCaseJsonOptions_UsesSnakeCaseLower()
    {
        var order = new OrderDto(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Alice",
            OrderStatus.Draft,
            new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero),
            "note");

        string json = JsonSerializer.Serialize(order, SerializationSamples.CreateSnakeCaseJsonOptions());

        Assert.Contains("\"customer_name\":\"Alice\"", json, StringComparison.Ordinal);
        Assert.Contains("\"internal_note\":\"note\"", json, StringComparison.Ordinal);
    }
}
