using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGptMiniApp.Shared.Domain.Entities
{
    public class Chat:BaseEntity
    {
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Message> Messages { get; set; }
    }

    public class BaseEntity
    {
        public Guid Id { get; set; }
    }
}
