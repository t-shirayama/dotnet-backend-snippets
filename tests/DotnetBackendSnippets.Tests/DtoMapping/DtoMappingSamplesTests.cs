using DotnetBackendSnippets.DtoMapping;

namespace DotnetBackendSnippets.Tests.DtoMapping;

public sealed class DtoMappingSamplesTests
{
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
}
