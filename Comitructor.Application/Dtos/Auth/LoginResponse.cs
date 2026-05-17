using System.ComponentModel;

namespace Comitructor.Application.Dtos.Auth
{
    /// <summary>
    /// Información devuelta tras un inicio de sesión exitoso.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT de acceso para autenticar peticiones posteriores.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        [DefaultValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del usuario que ha iniciado sesión.
        /// </summary>
        /// <example>admin</example>
        [DefaultValue("admin")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Rol asignado al usuario.
        /// </summary>
        /// <example>Administrator</example>
        [DefaultValue("Administrator")]
        public string Role { get; set; } = string.Empty;
    }
}