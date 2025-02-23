using ChatGptMiniApp.Server.Core.Interfaces;
using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatGptMiniApp.Server.Infrastructure.Data.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> CreateChatAsync(Guid userId, string title, Guid chatId)
        {
            var chat = new Chat { UserId = userId,Title = title,Id = chatId};
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            chat.Messages = new List<Message>();
            return chat;
        }

        public async Task<Chat?> GetChatByIdAsync(Guid chatId)
        {
            return await _context.Chats.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _context.Chats.Include(c => c.Messages).ToListAsync();
        }
    }
}
