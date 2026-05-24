using DotnetBackendSnippets.OpenApi;
using Microsoft.AspNetCore.Http;

namespace DotnetBackendSnippets.Tests.OpenApi;

public sealed class OpenApiSamplesTests
{
    [Fact]
    public void DescribeGetOrderEndpoint_ReturnsMetadataForAuthenticatedEndpoint()
    {
        OpenApiEndpointMetadata metadata = OpenApiSamples.DescribeGetOrderEndpoint(new ApiVersion(1, 0));

        Assert.Equal("getOrderv1", metadata.OperationId);
        Assert.Equal("Orders", Assert.Single(metadata.Tags));
        Assert.Equal("Bearer", metadata.Auth?.Scheme);
        Assert.Equal("orders:read", Assert.Single(metadata.Auth!.Scopes));
        Assert.Contains(metadata.Responses, response => response.StatusCode == StatusCodes.Status200OK);
        Assert.Contains(metadata.Responses, response => response is { StatusCode: StatusCodes.Status403Forbidden, BodyType: "ProblemDetails" });
        Assert.NotEmpty(metadata.Examples);
    }

    [Theory]
    [InlineData(1, 0, "/api/v1/orders")]
    [InlineData(2, 1, "/api/v2.1/orders")]
    public void CreateVersionedRoute_IncludesApiVersion(int major, int minor, string expected)
    {
        string route = OpenApiSamples.CreateVersionedRoute("/orders/", new ApiVersion(major, minor));

        Assert.Equal(expected, route);
    }
}
