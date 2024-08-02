namespace ChatGptMiniApp.Shared.Domain.Entities;

public class Message:BaseEntity
{
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    public string Text { get; set; }
    public bool IsUser { get; set; }
    public DateTime SentAt { get; set; } = DateTime.Now;
}