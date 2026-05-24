using DotnetBackendSnippets.Linq;

namespace DotnetBackendSnippets.Tests.Linq;

// テスト補助: LINQ Reverse Lookup Samples の共有データを定義する。
public sealed partial class LinqReverseLookupSamplesTests
{
    // テスト意図: Count By Key / Groups Using Comparer を確認する。
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

    // テスト意図: Latest Per Key / Returns Newest Item Per Group を確認する。
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

    // テスト意図: Find Duplicate Keys / Returns Keys Appearing More Than Once を確認する。
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
