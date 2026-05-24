using DotnetBackendSnippets.Testing;

namespace DotnetBackendSnippets.Tests.Testing;

// テスト対象: Testing Samples のスニペット動作を確認する。
public sealed class TestingSamplesTests
{
    // テスト意図: Reminder Service / Uses Fake Sender And Fixed Time Provider を確認する。
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

    // テスト意図: Reminder Service / Does Not Send Outside Business Hours を確認する。
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

    // テスト意図: Fixed Time Provider / Normalizes Offset To Utc を確認する。
    [Fact]
    public void FixedTimeProvider_NormalizesOffsetToUtc()
    {
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 5, 24, 19, 0, 0, TimeSpan.FromHours(9)));

        var result = timeProvider.GetUtcNow();

        Assert.Equal(TimeSpan.Zero, result.Offset);
        Assert.Equal(10, result.Hour);
    }

    // テスト意図: Throws Operation Canceled Async / Returns True / When Operation Observes Cancellation を確認する。
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
