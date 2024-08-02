using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatGptMiniApp.Server.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
}