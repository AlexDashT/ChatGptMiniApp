namespace ChatGptMiniApp.Shared.Domain.Entities
{
    public class Chat: BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Message> Messages { get; set; }
        public string Title { get; set; }
    }
}
