using DotnetBackendSnippets.HttpClient;

namespace DotnetBackendSnippets.Tests.HttpClient;

public sealed class HttpClientSamplesTests
{
    [Fact]
    public async Task GetMessageAsync_ReturnsMessage_WithoutRealHttpCall()
    {
        var handler = FakeHttpMessageHandler.Json("""{"message":"hello"}""");
        using var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/"),
        };
        var client = new MessageApiClient(httpClient);

        var message = await client.GetMessageAsync();

        Assert.Equal("hello", message);
        Assert.Equal(new Uri("https://example.test/message"), handler.LastRequest?.RequestUri);
    }
}
