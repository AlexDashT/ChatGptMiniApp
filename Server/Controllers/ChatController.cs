using ChatGptMiniApp.Server.Core.Interfaces;
using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace ChatGptMiniApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly string? _openAiApiKey;

        public ChatController(IChatRepository chatRepository, IMessageRepository messageRepository)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChat([FromBody] string userName)
        {
            var chat = await _chatRepository.CreateChatAsync(userName);
            return Ok(chat);
        }


        [HttpPost("{chatId}/stream")]
        public async Task StreamMessage(Guid chatId, [FromBody] string userMessage)
        {
            Response.ContentType = "text/event-stream";
            Chat chat;

            if (chatId == Guid.Empty)
            {
                chat = await _chatRepository.CreateChatAsync("ali");
            }
            else
            {
                chat = await _chatRepository.GetChatByIdAsync(chatId);
            }

            var messages = new List<ChatMessage>();

            foreach (Message message in chat.Messages)
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
           

            await foreach (var update in client.CompleteChatStreamingAsync(messages))
            {
                foreach (var updatePart in update.ContentUpdate)
                {
                    await Response.WriteAsync($"data: {updatePart.Text}\n\n");
                    await Response.Body.FlushAsync();
                }
            }

            await _messageRepository.AddMessageAsync(chat.Id, userMessage, true);
            await Response.WriteAsync($"chatid: {chat.Id}\n\n");
            await Response.Body.FlushAsync();
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat(Guid chatId)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return NotFound();
            }
            return Ok(chat);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChats()
        {
            var chats = await _chatRepository.GetAllChatsAsync();
            return Ok(chats);
        }
    }

}
