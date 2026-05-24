using System.Reflection;

namespace DotnetBackendSnippets.LanguageFeatures;

public static class DelegateEventAttributeSamples
{
    public static decimal ApplyDiscount(decimal amount, DiscountCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);

        return calculator(amount);
    }

    public static void ForEachMatching<T>(IEnumerable<T> values, Func<T, bool> predicate, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var value in values.Where(predicate))
        {
            action(value);
        }
    }

    public static IReadOnlyList<string> GetSnippetTags(MemberInfo member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return member
            .GetCustomAttributes<SnippetTagAttribute>()
            .Select(attribute => attribute.Name)
            .ToList();
    }

    [SnippetTag("delegate")]
    [SnippetTag("attribute")]
    public static decimal TaggedDiscount(decimal amount)
    {
        return amount * 0.9m;
    }
}

public delegate decimal DiscountCalculator(decimal amount);

public sealed class ProgressReporter
{
    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;

    public void Report(int percent)
    {
        if (percent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(percent), "Percent must be between 0 and 100.");
        }

        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percent));
    }
}

public sealed class ProgressChangedEventArgs(int percent) : EventArgs
{
    public int Percent { get; } = percent;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class SnippetTagAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
