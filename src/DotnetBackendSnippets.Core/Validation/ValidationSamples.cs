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
/// フィールド単位の検証エラーを表します。
/// </summary>
/// <param name="Field">エラー対象フィールド。</param>
/// <param name="Code">ローカライズやクライアント分岐に使うエラーコード。</param>
/// <param name="Message">既定のエラーメッセージ。</param>
public sealed record ValidationError(string Field, string Code, string Message);

/// <summary>
/// コード付き検証エラーの結果を表します。
/// </summary>
/// <param name="IsValid">検証に成功したかどうか。</param>
/// <param name="Errors">フィールド別の検証エラー一覧。</param>
public sealed record ValidationReport(bool IsValid, IReadOnlyList<ValidationError> Errors);

/// <summary>
/// 注文作成リクエストを表します。
/// </summary>
/// <param name="CustomerId">顧客 ID。</param>
/// <param name="ShippingAddress">配送先住所。</param>
/// <param name="Lines">注文明細。</param>
public sealed record CreateOrderInput(
    string? CustomerId,
    AddressInput? ShippingAddress,
    IReadOnlyList<OrderLineInput>? Lines);

/// <summary>
/// 住所入力を表します。
/// </summary>
/// <param name="PostalCode">郵便番号。</param>
/// <param name="CountryCode">国コード。</param>
public sealed record AddressInput(string? PostalCode, string? CountryCode);

/// <summary>
/// 注文明細入力を表します。
/// </summary>
/// <param name="Sku">商品 SKU。</param>
/// <param name="Quantity">数量。</param>
public sealed record OrderLineInput(string? Sku, int Quantity);

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

    /// <summary>
    /// nested object、collection、cross-field を含む注文作成リクエストを検証します。
    /// </summary>
    /// <param name="input">検証する注文作成リクエスト。</param>
    /// <returns>コード付きの検証結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/> が <see langword="null"/> の場合。</exception>
    public static ValidationReport ValidateCreateOrder(CreateOrderInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var errors = new List<ValidationError>();

        AddRequiredError(errors, "customerId", input.CustomerId);

        if (input.ShippingAddress is null)
        {
            errors.Add(new ValidationError("shippingAddress", "required", "Shipping address is required."));
        }
        else
        {
            AddRequiredError(errors, "shippingAddress.postalCode", input.ShippingAddress.PostalCode);
            AddRequiredError(errors, "shippingAddress.countryCode", input.ShippingAddress.CountryCode);

            if (string.Equals(input.ShippingAddress.CountryCode, "JP", StringComparison.OrdinalIgnoreCase)
                && !IsJapanesePostalCode(input.ShippingAddress.PostalCode))
            {
                errors.Add(new ValidationError(
                    "shippingAddress.postalCode",
                    "postal_code.jp",
                    "Japanese postal code must be 7 digits."));
            }
        }

        if (input.Lines is null || input.Lines.Count == 0)
        {
            errors.Add(new ValidationError("lines", "required", "At least one order line is required."));
        }
        else
        {
            ValidateOrderLines(input.Lines, errors);
        }

        return new ValidationReport(errors.Count == 0, errors);
    }

    private static void ValidateOrderLines(IReadOnlyList<OrderLineInput> lines, List<ValidationError> errors)
    {
        var seenSkus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < lines.Count; index++)
        {
            OrderLineInput line = lines[index];
            string skuField = $"lines[{index}].sku";

            AddRequiredError(errors, skuField, line.Sku);

            if (!string.IsNullOrWhiteSpace(line.Sku) && !seenSkus.Add(line.Sku.Trim()))
            {
                errors.Add(new ValidationError(skuField, "duplicate", "SKU must be unique in an order."));
            }

            if (line.Quantity <= 0)
            {
                errors.Add(new ValidationError($"lines[{index}].quantity", "range", "Quantity must be positive."));
            }
        }
    }

    private static void AddRequiredError(List<ValidationError> errors, string field, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ValidationError(field, "required", $"{field} is required."));
        }
    }

    private static bool IsJapanesePostalCode(string? postalCode)
    {
        return postalCode is { Length: 7 } && postalCode.All(char.IsAsciiDigit);
    }
}
