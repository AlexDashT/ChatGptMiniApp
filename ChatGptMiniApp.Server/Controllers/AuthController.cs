using System.Security.Claims;
using ChatGptMiniApp.Server.Core.Interfaces;
using ChatGptMiniApp.Shared.Domain.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ChatGptMiniApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var code = _userService.GenerateCode();
            _userService.SaveUserCode(request.Email, code);
            //_userService.SendEmail(request.Email, code);
            return Ok(new { message = "Verification code sent!" + code});
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
        {
            var token = await _userService.ValidateCodeAndGenerateTokenAsync(request.Email, request.Code);
            if (token == null)
            {
                return Unauthorized("Invalid code.");
            }
            return Ok(new TokenResponse { Token = token });
        }
    }
}
