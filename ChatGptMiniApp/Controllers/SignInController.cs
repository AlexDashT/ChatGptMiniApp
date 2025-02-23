using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ChatGptMiniApp.Shared.Domain.Dtos;
using System.Net.Http.Headers;
using System.Net.Http;

namespace ChatGptMiniApp.Controllers
{
    public class SignInController : Controller
    {
        [HttpGet("signincallback")]
        public async Task<IActionResult> SignInCallback(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Redirect("/login");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email),
                new (ClaimTypes.Name, email),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                });

            return Redirect("/");
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/logout-confirmation");
        }
    }
}
