using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Comitructor.Application.Dtos.Auth
{
    /// <summary>
    /// Modelo para la solicitud de autenticación y registro.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Nombre de usuario del sistema.
        /// </summary>
        /// <example>admin</example>
        [Required]
        [DefaultValue("admin")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña en texto plano.
        /// </summary>
        /// <example>P@ssw0rd123</example>
        [Required]
        [DefaultValue("P@ssw0rd123")]
        public string Password { get; set; } = string.Empty;
    }
}