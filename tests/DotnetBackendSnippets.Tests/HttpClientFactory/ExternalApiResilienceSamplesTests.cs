using System.Net;
using DotnetBackendSnippets.HttpClientFactory;
using Microsoft.AspNetCore.Http;

namespace DotnetBackendSnippets.Tests.HttpClientFactory;

public sealed class ExternalApiResilienceSamplesTests
{
    [Theory]
    [InlineData(HttpStatusCode.InternalServerError, ExternalApiFailureKind.Transient)]
    [InlineData(HttpStatusCode.TooManyRequests, ExternalApiFailureKind.Transient)]
    [InlineData(HttpStatusCode.BadRequest, ExternalApiFailureKind.Client)]
    [InlineData(HttpStatusCode.Unauthorized, ExternalApiFailureKind.Authentication)]
    public void ClassifyFailure_ReturnsExpectedKind(HttpStatusCode statusCode, ExternalApiFailureKind expected)
    {
        Assert.Equal(expected, ExternalApiResilienceSamples.ClassifyFailure(statusCode));
    }

    [Fact]
    public void CalculateExponentialBackoffDelay_CapsAtMaxDelay()
    {
        var options = new RetryBackoffOptions(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));

        Assert.Equal(TimeSpan.FromSeconds(1), ExternalApiResilienceSamples.CalculateExponentialBackoffDelay(1, options));
        Assert.Equal(TimeSpan.FromSeconds(2), ExternalApiResilienceSamples.CalculateExponentialBackoffDelay(2, options));
        Assert.Equal(TimeSpan.FromSeconds(3), ExternalApiResilienceSamples.CalculateExponentialBackoffDelay(3, options));
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_RetriesTransientFailure()
    {
        var attempts = 0;

        ExternalApiResult<ProductDto> result = await ExternalApiResilienceSamples.ExecuteWithRetryAsync(
            _ =>
            {
                attempts++;

                return Task.FromResult(attempts == 1
                    ? ExternalApiResult<ProductDto>.Failure(HttpStatusCode.ServiceUnavailable, "try later")
                    : ExternalApiResult<ProductDto>.Success(new ProductDto(1, "Notebook")));
            },
            new RetryBackoffOptions(3, TimeSpan.Zero, TimeSpan.Zero),
            static (_, _) => Task.CompletedTask);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void SimpleCircuitBreaker_OpensAfterThreshold()
    {
        var breaker = new SimpleCircuitBreaker(failureThreshold: 2);

        breaker.RecordResult(isSuccess: false);
        Assert.Equal(CircuitBreakerState.Closed, breaker.State);

        breaker.RecordResult(isSuccess: false);
        Assert.Equal(CircuitBreakerState.Open, breaker.State);

        breaker.RecordResult(isSuccess: true);
        Assert.Equal(CircuitBreakerState.Closed, breaker.State);
    }

    [Fact]
    public async Task CorrelationIdPropagationHandler_AddsHeaderFromHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = "corr-123";
        var handler = new CorrelationIdPropagationHandler(new HttpContextAccessor { HttpContext = context })
        {
            InnerHandler = new RecordingHandler(),
        };
        var invoker = new HttpMessageInvoker(handler);

        using HttpResponseMessage response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "https://api.example.test/products"),
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var recordingHandler = Assert.IsType<RecordingHandler>(handler.InnerHandler);
        Assert.Equal("corr-123", recordingHandler.LastRequest?.Headers.GetValues("X-Correlation-ID").Single());
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
