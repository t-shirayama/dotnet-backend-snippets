using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.HttpClientFactory;

/// <summary>
/// 外部 API 用のアクセストークンを取得します。
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// アクセストークンを非同期に取得します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>アクセストークン。</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// HTTP リクエストに Bearer トークンを追加します。
/// </summary>
public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider accessTokenProvider;

    /// <summary>
    /// <see cref="BearerTokenHandler"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="accessTokenProvider">アクセストークンの取得元。</param>
    /// <exception cref="ArgumentNullException"><paramref name="accessTokenProvider"/> が <see langword="null"/> の場合。</exception>
    public BearerTokenHandler(IAccessTokenProvider accessTokenProvider)
    {
        this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
    }

    /// <inheritdoc />
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

/// <summary>
/// 外部商品 API の商品データを表します。
/// </summary>
/// <param name="Id">商品 ID。</param>
/// <param name="Name">商品名。</param>
public sealed record ProductDto(int Id, string Name);

/// <summary>
/// 外部 API のエラー情報を表します。
/// </summary>
/// <param name="StatusCode">HTTP ステータスコード。</param>
/// <param name="Message">エラーメッセージ。</param>
public sealed record ExternalApiError(HttpStatusCode StatusCode, string Message);

/// <summary>
/// 外部 API 呼び出しの成功または失敗を表します。
/// </summary>
/// <typeparam name="T">成功時の値の型。</typeparam>
/// <param name="Value">成功時の値。</param>
/// <param name="Error">失敗時のエラー情報。</param>
public sealed record ExternalApiResult<T>(T? Value, ExternalApiError? Error)
{
    /// <summary>
    /// 呼び出しが成功したかどうかを取得します。
    /// </summary>
    /// <value>エラーがない場合は <see langword="true"/>。</value>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// 成功結果を作成します。
    /// </summary>
    /// <param name="value">成功時の値。</param>
    /// <returns>成功を表す結果。</returns>
    public static ExternalApiResult<T> Success(T value)
    {
        return new ExternalApiResult<T>(value, null);
    }

    /// <summary>
    /// 失敗結果を作成します。
    /// </summary>
    /// <param name="statusCode">HTTP ステータスコード。</param>
    /// <param name="message">エラーメッセージ。</param>
    /// <returns>失敗を表す結果。</returns>
    public static ExternalApiResult<T> Failure(HttpStatusCode statusCode, string message)
    {
        return new ExternalApiResult<T>(default, new ExternalApiError(statusCode, message));
    }
}

/// <summary>
/// 外部商品 API を呼び出すクライアントです。
/// </summary>
public sealed class ProductApiClient
{
    private readonly System.Net.Http.HttpClient httpClient;

    /// <summary>
    /// <see cref="ProductApiClient"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="httpClient">API 呼び出しに使う HTTP クライアント。</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> が <see langword="null"/> の場合。</exception>
    public ProductApiClient(System.Net.Http.HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// 商品 ID を指定して商品を取得します。
    /// </summary>
    /// <param name="productId">商品 ID。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>商品取得結果。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="productId"/> が 1 未満の場合。</exception>
    public async Task<ExternalApiResult<ProductDto>> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        if (productId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(productId), "Product id must be greater than or equal to 1.");
        }

        using var response = await httpClient.GetAsync($"products/{productId}", cancellationToken);

        return await ReadProductResponseAsync(response, cancellationToken);
    }

    /// <summary>
    /// キーワードとページ番号で商品を検索します。
    /// </summary>
    /// <param name="keyword">検索キーワード。</param>
    /// <param name="pageNumber">ページ番号。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>商品検索結果。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> が 1 未満の場合。</exception>
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

/// <summary>
/// クエリ文字列の組み立てサンプルです。
/// </summary>
public static class QueryStringSamples
{
    /// <summary>
    /// 商品検索 API のパスを作成します。
    /// </summary>
    /// <param name="keyword">検索キーワード。</param>
    /// <param name="pageNumber">ページ番号。</param>
    /// <returns>クエリ文字列付きの相対パス。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> が 1 未満の場合。</exception>
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

/// <summary>
/// HttpClientFactory 登録のサンプルです。
/// </summary>
public static class HttpClientFactorySamples
{
    /// <summary>
    /// 商品 API クライアントを DI コンテナーに登録します。
    /// </summary>
    /// <remarks>
    /// <see cref="BearerTokenHandler"/> が利用する <see cref="IAccessTokenProvider"/> は、このメソッドの呼び出し前に別途登録してください。
    /// </remarks>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="baseAddress">外部 API のベースアドレス。</param>
    /// <param name="timeout">HTTP リクエストのタイムアウト。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> または <paramref name="baseAddress"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="baseAddress"/> が絶対 URI ではない場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> が 0 以下の場合。</exception>
    public static IServiceCollection AddProductApiClient(
        this IServiceCollection services,
        Uri baseAddress,
        TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(baseAddress);

        if (!baseAddress.IsAbsoluteUri)
        {
            throw new ArgumentException("Base address must be an absolute URI.", nameof(baseAddress));
        }

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be positive.");
        }

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
