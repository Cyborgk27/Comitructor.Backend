using Comitructor.Domain.Entities;

namespace Comitructor.Domain.Interfaces
{
    /// <summary>
    /// Define el contrato para la generación de tokens JWT (Requerimiento 2.1).
    /// </summary>
    public interface IJwtProvider
    {
        /// <summary>
        /// Genera un token de acceso basado en la identidad y el rol del usuario.
        /// </summary>
        /// <param name="user">Entidad de usuario con Role y Username.</param>
        /// <returns>Una cadena de texto con el token JWT generado.</returns>
        string Generate(User user);
    }
}
