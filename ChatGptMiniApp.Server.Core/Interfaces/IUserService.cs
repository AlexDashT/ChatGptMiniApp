using ChatGptMiniApp.Shared.Domain.Entities;

namespace ChatGptMiniApp.Server.Core.Interfaces;

public interface IUserService
{
    string GenerateCode();
    void SaveUserCode(string email, string code);
    void SendEmail(string email, string code);
    Task<string> ValidateCodeAndGenerateTokenAsync(string email, string code);
}