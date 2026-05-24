using System.Net.Http.Json;
using System.Text.Json;

namespace DotnetBackendSnippets.HttpClient;

/// <summary>
/// メッセージ API のレスポンスを表します。
/// </summary>
/// <param name="Message">返されたメッセージ。</param>
public sealed record MessageResponse(string Message);

/// <summary>
/// メッセージ API を呼び出すクライアントです。
/// </summary>
public sealed class MessageApiClient
{
    private readonly System.Net.Http.HttpClient httpClient;

    /// <summary>
    /// <see cref="MessageApiClient"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="httpClient">API 呼び出しに使う HTTP クライアント。</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> が <see langword="null"/> の場合。</exception>
    public MessageApiClient(System.Net.Http.HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        this.httpClient = httpClient;
    }

    /// <summary>
    /// メッセージ API からメッセージを取得します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>取得したメッセージ。</returns>
    /// <exception cref="HttpRequestException">HTTP リクエストが失敗した場合。</exception>
    /// <exception cref="InvalidOperationException">レスポンス本文が不正な場合。</exception>
    public async Task<string> GetMessageAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("message", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await ReadRequiredMessageAsync(response, cancellationToken);
    }

    /// <summary>
    /// メッセージ API からメッセージを取得し、404 の場合は <see langword="null"/> を返します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>取得したメッセージ、または 404 の場合は <see langword="null"/>。</returns>
    /// <exception cref="HttpRequestException">404 以外の HTTP リクエストが失敗した場合。</exception>
    /// <exception cref="InvalidOperationException">レスポンス本文が不正な場合。</exception>
    public async Task<string?> GetMessageOrNullAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("message", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await ReadRequiredMessageAsync(response, cancellationToken);
    }

    /// <summary>
    /// メッセージ API へ JSON を送信し、レスポンスのメッセージを取得します。
    /// </summary>
    /// <param name="message">送信するメッセージ。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>レスポンスから取得したメッセージ。</returns>
    /// <exception cref="ArgumentException"><paramref name="message"/> が空白の場合。</exception>
    /// <exception cref="HttpRequestException">HTTP リクエストが失敗した場合。</exception>
    /// <exception cref="InvalidOperationException">レスポンス本文が不正な場合。</exception>
    public async Task<string> PostMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        using var response = await httpClient.PostAsJsonAsync("message", new MessageResponse(message.Trim()), cancellationToken);
        response.EnsureSuccessStatusCode();

        return await ReadRequiredMessageAsync(response, cancellationToken);
    }

    private static async Task<string> ReadRequiredMessageAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        MessageResponse? body;

        try
        {
            body = await response.Content.ReadFromJsonAsync<MessageResponse>(cancellationToken);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("Response JSON is invalid.", exception);
        }

        if (body is null || string.IsNullOrWhiteSpace(body.Message))
        {
            throw new InvalidOperationException("Response message is required.");
        }

        return body.Message;
    }
}
