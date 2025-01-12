namespace ChatGptMiniApp.Shared.Domain.Entities
{
    public class Chat: BaseEntity
    {
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Message> Messages { get; set; }
    }
}
