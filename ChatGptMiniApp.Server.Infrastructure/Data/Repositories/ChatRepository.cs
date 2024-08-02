using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatGptMiniApp.Server.Core.Interfaces;

namespace ChatGptMiniApp.Server.Infrastructure.Data.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> CreateChatAsync(string userName)
        {
            var chat = new Chat { UserName = userName };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            chat.Messages = new List<Message>();
            return chat;
        }

        public async Task<Chat> GetChatByIdAsync(Guid chatId)
        {
            return await _context.Chats.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _context.Chats.Include(c => c.Messages).ToListAsync();
        }
    }
}
