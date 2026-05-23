namespace DotnetBackendSnippets.Validation;

public sealed record UserRegistrationInput(string Email, string Password, string DisplayName);

public sealed record ValidationResult(bool IsValid, IReadOnlyList<string> Errors);

public static class ValidationSamples
{
    public static ValidationResult ValidateUserRegistration(UserRegistrationInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(input.Email) || !input.Email.Contains("@", StringComparison.Ordinal))
        {
            errors.Add("Email must contain '@'.");
        }

        if (string.IsNullOrWhiteSpace(input.Password) || input.Password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters.");
        }

        if (string.IsNullOrWhiteSpace(input.DisplayName))
        {
            errors.Add("Display name is required.");
        }

        return new ValidationResult(errors.Count == 0, errors);
    }
}
