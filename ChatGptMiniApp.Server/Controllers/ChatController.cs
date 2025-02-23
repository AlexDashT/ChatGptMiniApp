using ChatGptMiniApp.Server.Core.Interfaces;
using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using System.ClientModel;
using System.Security.Claims;
using System.Text;

namespace ChatGptMiniApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController
        (IChatRepository chatRepository, IMessageRepository messageRepository, IUserRepository userRepository)
        : ControllerBase
    {
        private readonly string? _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        [Authorize]
        [HttpPost("{chatId}/stream")]
        public async Task StreamMessage(Guid chatId, [FromBody] string userMessage)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await userRepository.GetUserByEmailAsync(email);

            Response.ContentType = "text/event-stream";
            Chat chat = await chatRepository.GetChatByIdAsync(chatId);

            if (chat == null)
            {
                String title = userMessage[..(userMessage.Length>50?50:userMessage.Length)];
                chat = await chatRepository.CreateChatAsync(user.Id, title, chatId);
            }
            var messages = new List<ChatMessage>();

            foreach (Message message in chat.Messages.OrderBy(m=>m.SentAt))
            {
                if (message.IsUser)
                {
                    messages.Add(new UserChatMessage(message.Text));
                }
                else
                {
                    messages.Add(new AssistantChatMessage(message.Text));
                }
            }
            messages.Add(new UserChatMessage(userMessage));

            var client = new ChatClient("gpt-4o-mini", _openAiApiKey);

            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync(messages);

            string responseText = String.Empty;
            await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    string data = completionUpdate.ContentUpdate[0].Text;
                    string dataWithBr = data.Replace("\n", "<br>");
                    responseText += dataWithBr;
                    var bytes = Encoding.UTF8.GetBytes($"data:{dataWithBr}\n\n");

                    await Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    await Response.Body.FlushAsync();
                }
            }


            await messageRepository.AddMessageAsync(chat.Id, userMessage, true);
            await messageRepository.AddMessageAsync(chat.Id, responseText, false);
            await Response.WriteAsync($"chatid: {chat.Id}\n\n");
            await Response.Body.FlushAsync();
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(Guid chatId)
        {
            var chat = await chatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return NotFound();
            }
            return Ok(chat);
        }
    }

}
