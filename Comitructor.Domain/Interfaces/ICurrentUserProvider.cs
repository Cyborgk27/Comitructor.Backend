namespace Comitructor.Domain.Interfaces
{
    /// <summary>
    /// Provee acceso a la identidad del usuario actual desde el contexto de ejecución.
    /// Esencial para la auditoría automática y el control de acceso basado en roles.
    /// </summary>
    public interface ICurrentUserProvider
    {
        /// <summary>
        /// Identificador único del usuario (extraído del Claim 'sub' o 'nameid').
        /// Se utiliza para llenar los campos de auditoría (CreatedBy, LastModifiedBy).
        /// </summary>
        int? UserId { get; }

        /// <summary>
        /// Nombre de usuario (Username) almacenado en el token JWT.
        /// </summary>
        string? Username { get; }

        /// <summary>
        /// Rol actual del usuario (Administrator u Operator).
        /// Facilita validaciones de permisos dentro de los servicios de aplicación.
        /// </summary>
        string? Role { get; }

        /// <summary>
        /// Indica si la petición actual proviene de un usuario autenticado.
        /// </summary>
        bool IsAuthenticated { get; }
    }
}