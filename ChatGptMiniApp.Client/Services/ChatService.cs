using System.Net.Http.Json;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace ChatGptMiniApp.Client.Services;

public class ChatService(HttpClient httpClient, ILocalStorageService localStorage)
{
    public async Task StreamMessage(Guid chatId, string userMessage, Action<string> onMessageReceived, Action<string> onChatIdReceived)
    {
        var token =await localStorage.GetItemAsStringAsync("token");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/chat/{chatId}/stream")
        {
            Content = JsonContent.Create(userMessage)
        };

        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            switch (string.IsNullOrWhiteSpace(line))
            {
                case false when line.StartsWith("data:"):
                {
                    var message = line.Substring("data:".Length);
                    onMessageReceived(message);

                    break;
                }
                case false when line.StartsWith("chatid:"):
                {
                    var message = line.Substring("chatid:".Length);
                    onChatIdReceived(message);

                    break;
                }
            }
        }
    }
}