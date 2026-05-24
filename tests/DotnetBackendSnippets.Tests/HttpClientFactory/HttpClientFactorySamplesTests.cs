using System.Net;
using DotnetBackendSnippets.HttpClientFactory;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.Tests.HttpClientFactory;

// テスト対象: HTTP Client Factory Samples のスニペット動作を確認する。
public sealed class HttpClientFactorySamplesTests
{
    // テスト意図: Add Product API Client / Registers Typed Client / With Bearer Token Handler を確認する。
    [Fact]
    public async Task AddProductApiClient_RegistersTypedClient_WithBearerTokenHandler()
    {
        var handler = TestHttpMessageHandler.Json("""{"id":10,"name":"Notebook"}""");
        var services = new ServiceCollection();

        services.AddSingleton<IAccessTokenProvider>(new FixedAccessTokenProvider("test-token"));
        services.AddProductApiClient(new Uri("https://api.example.test/"), TimeSpan.FromSeconds(10));
        services.AddHttpClient<ProductApiClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ProductApiClient>();

        var result = await client.GetProductAsync(10);

        Assert.True(result.IsSuccess);
        Assert.Equal(new ProductDto(10, "Notebook"), result.Value);
        Assert.Equal(new Uri("https://api.example.test/products/10"), handler.LastRequest?.RequestUri);
        Assert.Equal("Bearer", handler.LastRequest?.Headers.Authorization?.Scheme);
        Assert.Equal("test-token", handler.LastRequest?.Headers.Authorization?.Parameter);
    }

    // テスト意図: Get Product Async / Rejects Invalid Product Id を確認する。
    [Fact]
    public async Task GetProductAsync_RejectsInvalidProductId()
    {
        var client = new ProductApiClient(new System.Net.Http.HttpClient());

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.GetProductAsync(0));

        Assert.Equal("productId", exception.ParamName);
    }

    // テスト意図: Build Product Search Path / Adds Encoded Query String を確認する。
    [Fact]
    public void BuildProductSearchPath_AddsEncodedQueryString()
    {
        var path = QueryStringSamples.BuildProductSearchPath("green tea", 2);

        Assert.Equal("products/search?page=2&keyword=green%20tea", path);
    }

    // テスト意図: Add Product API Client / Rejects Invalid Configuration を確認する。
    [Fact]
    public void AddProductApiClient_RejectsInvalidConfiguration()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentException>(
            () => services.AddProductApiClient(new Uri("/relative", UriKind.Relative), TimeSpan.FromSeconds(10)));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => services.AddProductApiClient(new Uri("https://api.example.test/"), TimeSpan.Zero));
    }

    // テスト意図: Search Product Async / Returns Failure Result / When Response Is Not Successful を確認する。
    [Fact]
    public async Task SearchProductAsync_ReturnsFailureResult_WhenResponseIsNotSuccessful()
    {
        var handler = TestHttpMessageHandler.Text("not found", HttpStatusCode.NotFound);
        var services = new ServiceCollection();

        services.AddSingleton<IAccessTokenProvider>(new FixedAccessTokenProvider("test-token"));
        services.AddProductApiClient(new Uri("https://api.example.test/"), TimeSpan.FromSeconds(10));
        services.AddHttpClient<ProductApiClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ProductApiClient>();

        var result = await client.SearchProductAsync("missing", 1);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(HttpStatusCode.NotFound, result.Error?.StatusCode);
        Assert.Equal("not found", result.Error?.Message);
    }

    private sealed class FixedAccessTokenProvider(string token) : IAccessTokenProvider
    {
        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(token);
        }
    }

    private sealed class TestHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(response);
        }

        public static TestHttpMessageHandler Json(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new TestHttpMessageHandler(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
            });
        }

        public static TestHttpMessageHandler Text(string text, HttpStatusCode statusCode)
        {
            return new TestHttpMessageHandler(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(text),
            });
        }
    }
}
