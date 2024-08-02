using ChatGptMiniApp.Server.Core.Interfaces;
using ChatGptMiniApp.Shared.Domain.Entities;

namespace ChatGptMiniApp.Server.Infrastructure.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message> AddMessageAsync(Guid chatId, string text, bool isUser)
    {
        var message = new Message { ChatId = chatId, Text = text, IsUser = isUser };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }
}