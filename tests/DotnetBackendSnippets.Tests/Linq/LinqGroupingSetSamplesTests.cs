using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

public sealed partial class LinqReverseLookupSamplesTests
{
    [Fact]
    public void CountByKey_GroupsUsingComparer()
    {
        var result = LinqReverseLookupSamples.CountByKey(
            Orders,
            order => order.Category,
            StringComparer.OrdinalIgnoreCase);

        Assert.Equal(2, result["Books"]);
        Assert.Equal(2, result["Tools"]);
    }

    [Fact]
    public void LatestPerKey_ReturnsNewestItemPerGroup()
    {
        var tickets = new[]
        {
            new SupportTicket(1, 10, new DateTimeOffset(2026, 5, 20, 9, 0, 0, TimeSpan.Zero)),
            new SupportTicket(2, 10, new DateTimeOffset(2026, 5, 21, 9, 0, 0, TimeSpan.Zero)),
            new SupportTicket(3, 20, new DateTimeOffset(2026, 5, 19, 9, 0, 0, TimeSpan.Zero)),
        };

        var result = LinqReverseLookupSamples.LatestPerKey(
            tickets,
            ticket => ticket.CustomerId,
            ticket => ticket.CreatedAt);

        Assert.Equal([2, 3], result.Select(ticket => ticket.Id));
    }

    [Fact]
    public void FindDuplicateKeys_ReturnsKeysAppearingMoreThanOnce()
    {
        var result = LinqReverseLookupSamples.FindDuplicateKeys(
            Orders,
            order => order.Category,
            StringComparer.OrdinalIgnoreCase);

        Assert.Equal(["Books", "Tools"], result);
    }
}
