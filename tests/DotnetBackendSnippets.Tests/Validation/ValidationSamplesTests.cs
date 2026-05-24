using DotnetBackendSnippets.Validation;

namespace DotnetBackendSnippets.Tests.Validation;

public sealed class ValidationSamplesTests
{
    [Fact]
    public void ValidateUserRegistration_ReturnsValid_WhenInputIsValid()
    {
        var input = new UserRegistrationInput("user@example.com", "password123", "User");

        var result = ValidationSamples.ValidateUserRegistration(input);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateUserRegistration_ReturnsErrors_WhenInputIsInvalid()
    {
        var input = new UserRegistrationInput("invalid", "short", "");

        var result = ValidationSamples.ValidateUserRegistration(input);

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
    }

    [Fact]
    public void ValidateCreateOrder_ReturnsValid_WhenNestedObjectsAndLinesAreValid()
    {
        var input = new CreateOrderInput(
            "customer-1",
            new AddressInput("1000001", "JP"),
            [new OrderLineInput("SKU-1", 2)]);

        ValidationReport result = ValidationSamples.ValidateCreateOrder(input);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateCreateOrder_ReturnsFieldCodes_ForNestedCollectionAndCrossFieldErrors()
    {
        var input = new CreateOrderInput(
            "",
            new AddressInput("100-0001", "JP"),
            [
                new OrderLineInput("SKU-1", 0),
                new OrderLineInput("sku-1", 1),
            ]);

        ValidationReport result = ValidationSamples.ValidateCreateOrder(input);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error is { Field: "customerId", Code: "required" });
        Assert.Contains(result.Errors, error => error is { Field: "shippingAddress.postalCode", Code: "postal_code.jp" });
        Assert.Contains(result.Errors, error => error is { Field: "lines[0].quantity", Code: "range" });
        Assert.Contains(result.Errors, error => error is { Field: "lines[1].sku", Code: "duplicate" });
    }
}
