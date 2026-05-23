using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.HttpClientFactory;

public interface IAccessTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public sealed class BearerTokenHandler(IAccessTokenProvider accessTokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

public sealed record ProductDto(int Id, string Name);

public sealed record ExternalApiError(HttpStatusCode StatusCode, string Message);

public sealed record ExternalApiResult<T>(T? Value, ExternalApiError? Error)
{
    public bool IsSuccess => Error is null;

    public static ExternalApiResult<T> Success(T value)
    {
        return new ExternalApiResult<T>(value, null);
    }

    public static ExternalApiResult<T> Failure(HttpStatusCode statusCode, string message)
    {
        return new ExternalApiResult<T>(default, new ExternalApiError(statusCode, message));
    }
}

public sealed class ProductApiClient(System.Net.Http.HttpClient httpClient)
{
    public async Task<ExternalApiResult<ProductDto>> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"products/{productId}", cancellationToken);

        return await ReadProductResponseAsync(response, cancellationToken);
    }

    public async Task<ExternalApiResult<ProductDto>> SearchProductAsync(
        string? keyword,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        var path = QueryStringSamples.BuildProductSearchPath(keyword, pageNumber);
        using var response = await httpClient.GetAsync(path, cancellationToken);

        return await ReadProductResponseAsync(response, cancellationToken);
    }

    private static async Task<ExternalApiResult<ProductDto>> ReadProductResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(errorBody) ? response.ReasonPhrase ?? "Request failed." : errorBody;

            return ExternalApiResult<ProductDto>.Failure(response.StatusCode, message);
        }

        var product = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);

        return product is null
            ? ExternalApiResult<ProductDto>.Failure(HttpStatusCode.InternalServerError, "Response body is required.")
            : ExternalApiResult<ProductDto>.Success(product);
    }
}

public static class QueryStringSamples
{
    public static string BuildProductSearchPath(string? keyword, int pageNumber)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than or equal to 1.");
        }

        var query = new Dictionary<string, string?>
        {
            ["page"] = pageNumber.ToString(CultureInfo.InvariantCulture),
        };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query["keyword"] = keyword;
        }

        return QueryHelpers.AddQueryString("products/search", query);
    }
}

public static class HttpClientFactorySamples
{
    public static IServiceCollection AddProductApiClient(
        this IServiceCollection services,
        Uri baseAddress,
        TimeSpan timeout)
    {
        services.AddTransient<BearerTokenHandler>();

        services
            .AddHttpClient<ProductApiClient>(client =>
            {
                client.BaseAddress = baseAddress;
                client.Timeout = timeout;
            })
            .AddHttpMessageHandler<BearerTokenHandler>();

        return services;
    }
}
