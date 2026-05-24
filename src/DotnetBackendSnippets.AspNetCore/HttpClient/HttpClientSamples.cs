using System.Net.Http.Json;

namespace DotnetBackendSnippets.HttpClient;

/// <summary>
/// メッセージ API のレスポンスを表します。
/// </summary>
/// <param name="Message">返されたメッセージ。</param>
public sealed record MessageResponse(string Message);

/// <summary>
/// メッセージ API を呼び出すクライアントです。
/// </summary>
/// <param name="httpClient">API 呼び出しに使う HTTP クライアント。</param>
public sealed class MessageApiClient(System.Net.Http.HttpClient httpClient)
{
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

        var body = await response.Content.ReadFromJsonAsync<MessageResponse>(cancellationToken);

        if (body is null || string.IsNullOrWhiteSpace(body.Message))
        {
            throw new InvalidOperationException("Response message is required.");
        }

        return body.Message;
    }
}
