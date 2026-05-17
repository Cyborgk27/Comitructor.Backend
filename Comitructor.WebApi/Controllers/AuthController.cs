using Comitructor.Application.Dtos.Auth;
using Comitructor.Application.Interfaces;
using Comitructor.Infrastructure.Common;
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

        /// <summary>
        /// Inicia sesión en el sistema y genera un token de acceso.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest input)
        {
            var result = await _authService.Login(input);
            return Ok(result);
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] LoginRequest input)
        {
            await _authService.Register(input);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}