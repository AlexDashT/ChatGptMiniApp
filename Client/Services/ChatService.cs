using System.Net.Http.Json;

namespace ChatGptMiniApp.Client.Services;

public class ChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task StreamMessage(int chatId, string userMessage, Action<string> onMessageReceived)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/chat/{chatId}/stream")
        {
            Content = JsonContent.Create(userMessage)
        };

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data:"))
            {
                var message = line.Substring("data:".Length);
                onMessageReceived(message);
            }
        }
    }
}