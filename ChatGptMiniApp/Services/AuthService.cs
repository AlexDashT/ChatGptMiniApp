using ChatGptMiniApp.Shared.Domain.Dtos;

namespace ChatGptMiniApp.Services
{
    public class AuthService(HttpClient http)
    {
        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            var response = await http.PostAsJsonAsync("api/auth/login", new { Email = email });
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> VerifyCodeAsync(string email, string verificationCode)
        {
            var response = await http.PostAsJsonAsync("api/auth/verify", new { Email = email, Code = verificationCode });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResponse?.Token;
        }
    }
}
