using ChatGptMiniApp.Server.Core.Interfaces;
using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatGptMiniApp.Server.Infrastructure.Data.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}