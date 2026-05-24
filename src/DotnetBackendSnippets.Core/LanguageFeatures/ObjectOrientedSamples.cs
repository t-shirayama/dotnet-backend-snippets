namespace DotnetBackendSnippets.LanguageFeatures;

public static class ObjectOrientedSamples
{
    public static decimal CalculateDiscountedAmount(decimal amount, DiscountPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return amount - policy.CalculateDiscount(amount);
    }

    public static decimal CalculateTotalWithTax(decimal amount, ITaxCalculator taxCalculator)
    {
        ArgumentNullException.ThrowIfNull(taxCalculator);

        return amount + taxCalculator.CalculateTax(amount);
    }
}

public abstract class DiscountPolicy
{
    public abstract decimal CalculateDiscount(decimal amount);
}

public sealed class PercentageDiscountPolicy : DiscountPolicy
{
    private readonly decimal rate;

    public PercentageDiscountPolicy(decimal rate)
    {
        if (rate is < 0m or > 1m)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be between 0 and 1.");
        }

        this.rate = rate;
    }

    public override decimal CalculateDiscount(decimal amount)
    {
        return amount * rate;
    }
}

public interface ITaxCalculator
{
    decimal CalculateTax(decimal amount);
}

public sealed class FixedRateTaxCalculator(decimal rate) : ITaxCalculator
{
    public decimal CalculateTax(decimal amount)
    {
        return amount * rate;
    }
}

public readonly record struct Money(decimal Amount, string Currency) : IComparable<Money>
{
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return left.Amount < right.Amount;
    }

    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);

        return left.Amount > right.Amount;
    }

    public int CompareTo(Money other)
    {
        EnsureSameCurrency(this, other);

        return Amount.CompareTo(other.Amount);
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (!string.Equals(left.Currency, right.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Money values must use the same currency.");
        }
    }
}

public sealed class HeaderBag
{
    private readonly List<KeyValuePair<string, string>> headers = [];

    public string this[string key]
    {
        get
        {
            var match = headers.FirstOrDefault(
                header => string.Equals(header.Key, key, StringComparison.OrdinalIgnoreCase));

            return match.Equals(default(KeyValuePair<string, string>))
                ? string.Empty
                : match.Value;
        }
    }

    public KeyValuePair<string, string> this[int index] => headers[index];

    public void Add(string key, string value)
    {
        headers.Add(KeyValuePair.Create(key, value));
    }
}
