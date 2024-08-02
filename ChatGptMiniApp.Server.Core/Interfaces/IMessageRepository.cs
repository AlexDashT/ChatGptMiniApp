using ChatGptMiniApp.Shared.Domain.Entities;

namespace ChatGptMiniApp.Server.Core.Interfaces;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Guid chatId, string text, bool isUser);
}