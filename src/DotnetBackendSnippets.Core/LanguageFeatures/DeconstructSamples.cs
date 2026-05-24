namespace DotnetBackendSnippets.LanguageFeatures;

public static class DeconstructSamples
{
    public static string FormatCustomer(CustomerName customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var (lastName, firstName) = customer;

        return $"{lastName} {firstName}";
    }

    public static string FormatRecordProduct(ProductCode product)
    {
        var (category, number) = product;

        return $"{category}-{number:0000}";
    }
}

public sealed class CustomerName
{
    public CustomerName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public void Deconstruct(out string lastName, out string firstName)
    {
        lastName = LastName;
        firstName = FirstName;
    }
}

public sealed record ProductCode(string Category, int Number);
