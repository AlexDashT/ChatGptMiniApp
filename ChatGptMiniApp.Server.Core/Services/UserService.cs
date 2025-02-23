using ChatGptMiniApp.Server.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatGptMiniApp.Shared.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Collections.Concurrent;

namespace ChatGptMiniApp.Server.Core.Services
{
    public class UserService(IConfiguration config, IUserRepository userRepository) : IUserService
    {
        // In-memory store for verification codes (email -> code)
        private static readonly ConcurrentDictionary<string, string> _userVerificationCodes = new();

        public string GenerateCode()
        {
            //return "1111";
            return new Random().Next(100000, 999999).ToString();
        }

        public void SaveUserCode(string email, string code)
        {
            _userVerificationCodes[email] = code;
        }

        public void SendEmail(string email, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MyApp", "noreply@myapp.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Your Verification Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {code}"
            };

            using var client = new SmtpClient();
            client.Connect("smtp.mailtrap.io", 587, false);
            client.Authenticate("your_username", "your_password");
            client.Send(message);
            client.Disconnect(true);
        }

        public async Task<string> ValidateCodeAndGenerateTokenAsync(string email, string code)
        {
            if (!_userVerificationCodes.TryGetValue(email, out var storedCode) || storedCode != code)
            {
                return null;
            }

            _userVerificationCodes.TryRemove(email, out _);

            var user = await userRepository.GetUserByEmailAsync(email);
            user.LastLoginTime = DateTime.UtcNow;
            await userRepository.UpdateUserAsync(user);

            return GenerateJwtToken(email);
        }

        private string GenerateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"] ?? throw new InvalidOperationException());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = config["JwtSettings:Issuer"],
                Audience = config["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
