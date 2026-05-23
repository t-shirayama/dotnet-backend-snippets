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
}
