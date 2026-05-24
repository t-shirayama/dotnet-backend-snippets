using DotnetBackendSnippets.HttpClient;

namespace DotnetBackendSnippets.Tests.HttpClient;

// テスト対象: HTTP Client Samples のスニペット動作を確認する。
public sealed class HttpClientSamplesTests
{
    // テスト意図: Get Message Async / Returns Message / Without Real HTTP Call を確認する。
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

    // テスト意図: Get Message Async / Throws Invalid Operation Exception / When Json Is Invalid を確認する。
    [Fact]
    public async Task GetMessageAsync_ThrowsInvalidOperationException_WhenJsonIsInvalid()
    {
        var handler = FakeHttpMessageHandler.Json("""{"message":""");
        using var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/"),
        };
        var client = new MessageApiClient(httpClient);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetMessageAsync());

        Assert.Equal("Response JSON is invalid.", exception.Message);
    }

    // テスト意図: Get Message Async / Throws Invalid Operation Exception / When Message Is Missing を確認する。
    [Fact]
    public async Task GetMessageAsync_ThrowsInvalidOperationException_WhenMessageIsMissing()
    {
        var handler = FakeHttpMessageHandler.Json("""{"message":" "}""");
        using var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/"),
        };
        var client = new MessageApiClient(httpClient);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetMessageAsync());

        Assert.Equal("Response message is required.", exception.Message);
    }

    // テスト意図: Get Message Or Null Async / Returns Null / When Not Found を確認する。
    [Fact]
    public async Task GetMessageOrNullAsync_ReturnsNull_WhenNotFound()
    {
        var handler = FakeHttpMessageHandler.Json("""{"message":"missing"}""", System.Net.HttpStatusCode.NotFound);
        using var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/"),
        };
        var client = new MessageApiClient(httpClient);

        string? message = await client.GetMessageOrNullAsync();

        Assert.Null(message);
    }

    // テスト意図: Post Message Async / Sends Json Body / And Returns Response Message を確認する。
    [Fact]
    public async Task PostMessageAsync_SendsJsonBody_AndReturnsResponseMessage()
    {
        var handler = FakeHttpMessageHandler.Json("""{"message":"accepted"}""", System.Net.HttpStatusCode.Created);
        using var httpClient = new System.Net.Http.HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/"),
        };
        var client = new MessageApiClient(httpClient);

        string message = await client.PostMessageAsync(" hello ");
        string requestBody = await handler.LastRequest!.Content!.ReadAsStringAsync();

        Assert.Equal("accepted", message);
        Assert.Equal(System.Net.Http.HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal(new Uri("https://example.test/message"), handler.LastRequest.RequestUri);
        Assert.Contains("\"message\":\"hello\"", requestBody, StringComparison.Ordinal);
    }
}
