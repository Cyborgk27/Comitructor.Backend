using Comitructor.Application.Dtos;

namespace Comitructor.Application.Interfaces
{
    /// <summary>
    /// Define los servicios de autenticación y gestión de usuarios del sistema.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Realiza el proceso de autenticación de un usuario.
        /// </summary>
        /// <param name="request">DTO que contiene Username y Password.</param>
        /// <returns>
        /// Una tarea que representa la operación asíncrona, 
        /// devolviendo un <see cref="LoginResponse"/> con el token JWT y datos básicos.
        /// </returns>
        /// <exception cref="UserFriendlyException">Lanzada cuando las credenciales son incorrectas.</exception>
        Task<LoginResponse> Login(LoginRequest request);

        /// <summary>
        /// Registra un nuevo usuario en la base de datos con un rol predeterminado.
        /// </summary>
        /// <param name="request">Datos del nuevo usuario.</param>
        /// <returns>Una tarea que representa la finalización del registro.</returns>
        /// <remarks>
        /// En la implementación, se debe asegurar el cifrado de la contraseña antes de persistirla.
        /// </remarks>
        Task Register(LoginRequest request);
    }
}