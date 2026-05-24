using DotnetBackendSnippets.Testing;

namespace DotnetBackendSnippets.Tests.Testing;

public sealed class TestingSamplesTests
{
    [Fact]
    public async Task ReminderService_UsesFakeSenderAndFixedTimeProvider()
    {
        var sender = new FakeNotificationSender();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 5, 24, 10, 0, 0, TimeSpan.Zero));
        var service = new ReminderService(sender, timeProvider);

        bool sent = await service.SendDuringBusinessHoursAsync("user-1", "Check report");

        Assert.True(sent);
        Assert.Equal(("user-1", "Check report"), Assert.Single(sender.SentMessages));
    }

    [Fact]
    public async Task ReminderService_DoesNotSendOutsideBusinessHours()
    {
        var sender = new FakeNotificationSender();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 5, 24, 20, 0, 0, TimeSpan.Zero));
        var service = new ReminderService(sender, timeProvider);

        bool sent = await service.SendDuringBusinessHoursAsync("user-1", "Check report");

        Assert.False(sent);
        Assert.Empty(sender.SentMessages);
    }

    [Fact]
    public async Task ThrowsOperationCanceledAsync_ReturnsTrue_WhenOperationObservesCancellation()
    {
        bool result = await TestingSamples.ThrowsOperationCanceledAsync(token =>
        {
            token.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        });

        Assert.True(result);
    }
}
