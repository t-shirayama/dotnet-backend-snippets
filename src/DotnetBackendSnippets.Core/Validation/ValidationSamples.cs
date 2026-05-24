namespace DotnetBackendSnippets.Validation;

/// <summary>
/// ユーザー登録入力を表します。
/// </summary>
/// <param name="Email">メールアドレス。</param>
/// <param name="Password">パスワード。</param>
/// <param name="DisplayName">表示名。</param>
public sealed record UserRegistrationInput(string Email, string Password, string DisplayName);

/// <summary>
/// 入力検証の結果を表します。
/// </summary>
/// <param name="IsValid">検証に成功したかどうか。</param>
/// <param name="Errors">検証エラーの一覧。</param>
public sealed record ValidationResult(bool IsValid, IReadOnlyList<string> Errors);

/// <summary>
/// 入力検証の基本的な実装例を提供します。
/// </summary>
public static class ValidationSamples
{
    /// <summary>
    /// ユーザー登録入力を検証します。
    /// </summary>
    /// <param name="input">検証する入力値。</param>
    /// <returns>検証結果とエラー一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/> が <see langword="null"/> の場合。</exception>
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
