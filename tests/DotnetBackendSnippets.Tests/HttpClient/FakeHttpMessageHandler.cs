using System.Net;

namespace DotnetBackendSnippets.Tests.HttpClient;

public sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(response);
    }

    public static FakeHttpMessageHandler Json(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new FakeHttpMessageHandler(new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
        });
    }
}
