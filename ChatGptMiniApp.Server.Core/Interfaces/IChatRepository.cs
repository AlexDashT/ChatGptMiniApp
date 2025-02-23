using ChatGptMiniApp.Shared.Domain.Entities;

namespace ChatGptMiniApp.Server.Core.Interfaces
{
    public interface IChatRepository
    {
        Task<Chat> CreateChatAsync(Guid userId, string title, Guid chatId);
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task<IEnumerable<Chat>> GetAllChatsAsync();
    }
}
