using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiometricAuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var token = _authService.Authenticate(model.Username, model.Password);
            if (token == null)
            {
                return Unauthorized("Invalid credentials");
            }
            return Ok(new { Token = token });
        }
    }
}
