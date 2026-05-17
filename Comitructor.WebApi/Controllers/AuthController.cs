using Comitructor.Application.Dtos.Auth;
using Comitructor.Application.Interfaces;
using Comitructor.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;

namespace Comitructor.WebApi.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar la autenticación y el registro de usuarios.
    /// Proporciona los puntos de entrada para obtener tokens JWT y dar de alta nuevos operadores.
    /// </summary>
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
        /// <param name="input">Credenciales del usuario (Nombre de usuario y Contraseña).</param>
        /// <returns>Información del usuario autenticado y el Token JWT generado.</returns>
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
        /// <remarks>
        /// Por defecto, todos los usuarios registrados mediante este endpoint 
        /// se crean con el rol de 'Operator'.
        /// </remarks>
        /// <param name="input">Datos necesarios para el registro del nuevo usuario.</param>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] LoginRequest input)
        {
            await _authService.Register(input);
            return Ok(new { message = "Usuario registrado exitosamente" });
        }
    }
}