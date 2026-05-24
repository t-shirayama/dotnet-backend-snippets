using Microsoft.AspNetCore.Http;

namespace DotnetBackendSnippets.OpenApi;

/// <summary>
/// API version を表します。
/// </summary>
/// <param name="Major">major version。</param>
/// <param name="Minor">minor version。</param>
public sealed record ApiVersion(int Major, int Minor)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Minor == 0 ? $"v{Major}" : $"v{Major}.{Minor}";
    }
}

/// <summary>
/// OpenAPI に出したいレスポンス情報を表します。
/// </summary>
/// <param name="StatusCode">HTTP ステータスコード。</param>
/// <param name="Description">レスポンス説明。</param>
/// <param name="BodyType">レスポンス body の型名。</param>
public sealed record OpenApiResponseMetadata(int StatusCode, string Description, string? BodyType = null);

/// <summary>
/// OpenAPI に出したい schema example を表します。
/// </summary>
/// <param name="Name">example 名。</param>
/// <param name="Json">example JSON。</param>
public sealed record OpenApiSchemaExample(string Name, string Json);

/// <summary>
/// OpenAPI に出したい認証要件を表します。
/// </summary>
/// <param name="Scheme">認証 scheme。</param>
/// <param name="Scopes">必要な scope。</param>
public sealed record OpenApiAuthRequirement(string Scheme, IReadOnlyList<string> Scopes);

/// <summary>
/// エンドポイント単位の OpenAPI metadata を表します。
/// </summary>
/// <param name="OperationId">operationId。</param>
/// <param name="Summary">短い説明。</param>
/// <param name="Description">詳細説明。</param>
/// <param name="Tags">タグ。</param>
/// <param name="Version">API version。</param>
/// <param name="Responses">レスポンス一覧。</param>
/// <param name="Auth">認証要件。</param>
/// <param name="Examples">schema example 一覧。</param>
public sealed record OpenApiEndpointMetadata(
    string OperationId,
    string Summary,
    string Description,
    IReadOnlyList<string> Tags,
    ApiVersion Version,
    IReadOnlyList<OpenApiResponseMetadata> Responses,
    OpenApiAuthRequirement? Auth,
    IReadOnlyList<OpenApiSchemaExample> Examples);

/// <summary>
/// OpenAPI metadata と API versioning のサンプルです。
/// </summary>
public static class OpenApiSamples
{
    /// <summary>
    /// 認証付き注文取得 API の OpenAPI metadata を作成します。
    /// </summary>
    /// <param name="version">API version。</param>
    /// <returns>OpenAPI に反映する metadata。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="version"/> が無効な場合。</exception>
    public static OpenApiEndpointMetadata DescribeGetOrderEndpoint(ApiVersion version)
    {
        ValidateApiVersion(version);

        return new OpenApiEndpointMetadata(
            OperationId: $"getOrder{version}",
            Summary: "Get an order.",
            Description: "Returns one order visible to the current authenticated user.",
            Tags: ["Orders"],
            Version: version,
            Responses:
            [
                new OpenApiResponseMetadata(StatusCodes.Status200OK, "Order was found.", "OrderResponse"),
                CreateProblemResponse(StatusCodes.Status401Unauthorized, "Authentication is required."),
                CreateProblemResponse(StatusCodes.Status403Forbidden, "The current user cannot access this order."),
                CreateProblemResponse(StatusCodes.Status404NotFound, "Order was not found."),
            ],
            Auth: CreateBearerAuthRequirement("orders:read"),
            Examples:
            [
                new OpenApiSchemaExample(
                    "OrderResponse",
                    """{"id":123,"status":"submitted","totalAmount":1200}"""),
            ]);
    }

    /// <summary>
    /// ProblemDetails レスポンスの metadata を作成します。
    /// </summary>
    /// <param name="statusCode">HTTP ステータスコード。</param>
    /// <param name="description">レスポンス説明。</param>
    /// <returns>ProblemDetails レスポンス metadata。</returns>
    /// <exception cref="ArgumentException"><paramref name="description"/> が空白の場合。</exception>
    public static OpenApiResponseMetadata CreateProblemResponse(int statusCode, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        return new OpenApiResponseMetadata(statusCode, description, "ProblemDetails");
    }

    /// <summary>
    /// Bearer token の認証要件を作成します。
    /// </summary>
    /// <param name="scopes">必要な scope。</param>
    /// <returns>Bearer 認証要件。</returns>
    public static OpenApiAuthRequirement CreateBearerAuthRequirement(params string[] scopes)
    {
        ArgumentNullException.ThrowIfNull(scopes);

        return new OpenApiAuthRequirement("Bearer", scopes);
    }

    /// <summary>
    /// API version を含む route pattern を作成します。
    /// </summary>
    /// <param name="resource">resource 名。</param>
    /// <param name="version">API version。</param>
    /// <returns><c>/api/v1/resource</c> 形式の route pattern。</returns>
    /// <exception cref="ArgumentException"><paramref name="resource"/> が空白の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="version"/> が無効な場合。</exception>
    public static string CreateVersionedRoute(string resource, ApiVersion version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        ValidateApiVersion(version);

        return $"/api/{version}/{resource.Trim().Trim('/')}";
    }

    /// <summary>
    /// API version が有効か検証します。
    /// </summary>
    /// <param name="version">検証する API version。</param>
    /// <exception cref="ArgumentOutOfRangeException">major が 1 未満、または minor が 0 未満の場合。</exception>
    public static void ValidateApiVersion(ApiVersion version)
    {
        if (version.Major < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Major version must be one or greater.");
        }

        if (version.Minor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Minor version must be zero or greater.");
        }
    }
}
