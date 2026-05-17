using Comitructor.Domain.Common;
using Comitructor.Domain.Enums;

namespace Comitructor.Domain.Entities
{
    /// <summary>
    /// Entidad que representa a un usuario para autenticación y autorización
    /// </summary>
    public class User : BaseEntity<int>
    {
        /// <summary>
        /// Nombre de usuario para el login
        /// </summary>
        public string Username { get; set; } = null!;
        /// <summary>
        /// Contraseña cifrada del usuario
        /// </summary>
        public string Password { get; set; } = null!;
        /// <summary>
        /// Rol del usuario que determina sus permisos en el sistema
        /// </summary>
        public UserRole Role { get; set; }
    }
}
