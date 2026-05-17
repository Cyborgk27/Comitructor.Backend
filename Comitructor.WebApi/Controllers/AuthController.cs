using Comitructor.Application.Dtos.Auth;
using Comitructor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Comitructor.WebApi.Controllers
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
        public async Task<IActionResult> Login(LoginRequest input)
        {
            var result = await _authService.Login(input);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginRequest input)
        {
            await _authService.Register(input);
            return Ok();
        }
    }
}
