using System.Net.Http.Json;

namespace DotnetBackendSnippets.HttpClient;

public sealed record MessageResponse(string Message);

public sealed class MessageApiClient(System.Net.Http.HttpClient httpClient)
{
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
