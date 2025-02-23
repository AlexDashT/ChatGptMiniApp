using System.ComponentModel.DataAnnotations;

namespace ChatGptMiniApp.Shared.Domain.Entities;

public class User:BaseEntity
{
    public string Email { get; set; }
    public DateTime LastLoginTime { get; set; }
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}