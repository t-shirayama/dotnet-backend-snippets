using DotnetBackendSnippets.DtoMapping;

namespace DotnetBackendSnippets.Tests.DtoMapping;

// テスト対象: DTO Mapping Samples のスニペット動作を確認する。
public sealed class DtoMappingSamplesTests
{
    // テスト意図: To Command / Maps Request DTO And Ignores Over Posted Values を確認する。
    [Fact]
    public void ToCommand_MapsRequestDtoAndIgnoresOverPostedValues()
    {
        var request = new CreateOrderRequestDto(
            "customer-1",
            [new CreateOrderLineRequestDto("SKU-1", 2)],
            IsAdmin: true);

        CreateOrderCommand command = DtoMappingSamples.ToCommand(request, "user-1");

        Assert.Equal("customer-1", command.CustomerId);
        Assert.Equal("user-1", command.RequestedByUserId);
        Assert.Equal(new CreateOrderLineCommand("SKU-1", 2), Assert.Single(command.Lines));
    }

    // テスト意図: To Command / Throws / When Nested Line DTO Is Null を確認する。
    [Fact]
    public void ToCommand_Throws_WhenNestedLineDtoIsNull()
    {
        var request = new CreateOrderRequestDto("customer-1", [null!], IsAdmin: false);

        var exception = Assert.Throws<ArgumentNullException>(() => DtoMappingSamples.ToCommand(request, "user-1"));

        Assert.Equal("line", exception.ParamName);
    }

    // テスト意図: To Response / Maps Nested Entity To Response DTO を確認する。
    [Fact]
    public void ToResponse_MapsNestedEntityToResponseDto()
    {
        var order = new OrderEntity
        {
            Id = 10,
            CustomerId = "customer-1",
            Status = "submitted",
        };
        order.Lines.Add(new OrderLineEntity { Sku = "SKU-1", Quantity = 2 });

        OrderResponseDto response = DtoMappingSamples.ToResponse(order);

        Assert.Equal(10, response.Id);
        Assert.Equal("submitted", response.Status);
        Assert.Equal(new OrderLineResponseDto("SKU-1", 2), Assert.Single(response.Lines));
    }

    // テスト意図: To Response / Throws / When Nested Line Entity Is Null を確認する。
    [Fact]
    public void ToResponse_Throws_WhenNestedLineEntityIsNull()
    {
        var order = new OrderEntity();
        order.Lines.Add(null!);

        var exception = Assert.Throws<ArgumentNullException>(() => DtoMappingSamples.ToResponse(order));

        Assert.Equal("line", exception.ParamName);
    }

    // テスト意図: Apply Profile Patch / Updates Only Allowed Non Null Fields を確認する。
    [Fact]
    public void ApplyProfilePatch_UpdatesOnlyAllowedNonNullFields()
    {
        var profile = new CustomerProfileEntity
        {
            DisplayName = "Alice",
            PhoneNumber = "000",
            IsAdmin = false,
        };
        var request = new UpdateCustomerProfileRequestDto(
            DisplayName: "  Alice Smith  ",
            PhoneNumber: "",
            IsAdmin: true);

        DtoMappingSamples.ApplyProfilePatch(profile, request);

        Assert.Equal("Alice Smith", profile.DisplayName);
        Assert.Null(profile.PhoneNumber);
        Assert.False(profile.IsAdmin);
    }

    // テスト意図: Apply Optional Profile Patch / Distinguishes Unspecified And Null を確認する。
    [Fact]
    public void ApplyOptionalProfilePatch_DistinguishesUnspecifiedAndNull()
    {
        var profile = new CustomerProfileEntity
        {
            DisplayName = "Alice",
            PhoneNumber = "090-0000-0000",
        };
        var request = new OptionalCustomerProfilePatchDto(
            OptionalPatchValue<string>.Unspecified(),
            OptionalPatchValue<string?>.Specified(null));

        DtoMappingSamples.ApplyOptionalProfilePatch(profile, request);

        Assert.Equal("Alice", profile.DisplayName);
        Assert.Null(profile.PhoneNumber);
    }

    // テスト意図: To V2 Response / Adds Versioned Shape And Links を確認する。
    [Fact]
    public void ToV2Response_AddsVersionedShapeAndLinks()
    {
        var order = new OrderEntity
        {
            Id = 100,
            CustomerId = "customer-1",
            Status = "submitted",
        };
        order.Lines.Add(new OrderLineEntity { Sku = "SKU-1", Quantity = 2 });
        order.Lines.Add(new OrderLineEntity { Sku = "SKU-2", Quantity = 1 });

        OrderResponseV2Dto response = DtoMappingSamples.ToV2Response(order, "/api/orders/");

        Assert.Equal(2, response.LineCount);
        Assert.Equal("/api/orders/100", response.Links["self"]);
        Assert.Equal("/customers/customer-1", response.Links["customer"]);
    }
}
