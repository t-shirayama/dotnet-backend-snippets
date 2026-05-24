using System.Text.Json;
using DotnetBackendSnippets.Serialization;

namespace DotnetBackendSnippets.Tests.Serialization;

// テスト対象: Serialization Samples のスニペット動作を確認する。
public sealed class SerializationSamplesTests
{
    // テスト意図: Serialize Order / Uses Camel Case String Enum And Omits Null を確認する。
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

    // テスト意図: Deserialize Order / Reads Case Insensitive Properties And String Enum を確認する。
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

    // テスト意図: Deserialize Flexible Order Request / Keeps Unknown Fields を確認する。
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

    // テスト意図: Create Snake Case JSON Options / Uses Snake Case Lower を確認する。
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

    // テスト意図: Payment Method Polymorphism / Round Trips Derived Type を確認する。
    [Fact]
    public void PaymentMethodPolymorphism_RoundTripsDerivedType()
    {
        string json = SerializationSamples.SerializePaymentMethod(new CardPaymentMethodDto("1234"));

        PaymentMethodDto result = SerializationSamples.DeserializePaymentMethod(json);

        Assert.Contains("\"type\":\"card\"", json, StringComparison.Ordinal);
        var card = Assert.IsType<CardPaymentMethodDto>(result);
        Assert.Equal("1234", card.Last4);
    }

    // テスト意図: Serialize Versioned Envelope / Includes API Version を確認する。
    [Fact]
    public void SerializeVersionedEnvelope_IncludesApiVersion()
    {
        var order = new OrderDto(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Alice",
            OrderStatus.Draft,
            new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero),
            null);

        string json = SerializationSamples.SerializeVersionedEnvelope(order, apiVersion: 2);

        Assert.Contains("\"apiVersion\":2", json, StringComparison.Ordinal);
        Assert.Contains("\"customerName\":\"Alice\"", json, StringComparison.Ordinal);
    }

    // テスト意図: Serialize Order With Source Generation / Uses Generated Context を確認する。
    [Fact]
    public void SerializeOrderWithSourceGeneration_UsesGeneratedContext()
    {
        var order = new OrderDto(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Alice",
            OrderStatus.Cancelled,
            new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero),
            null);

        string json = SerializationSamples.SerializeOrderWithSourceGeneration(order);

        Assert.Contains("\"status\":\"Cancelled\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("internalNote", json, StringComparison.Ordinal);
    }
}
