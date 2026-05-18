namespace Comitructor.Application.Dtos.Request
{
    /// <summary>
    /// DTO simplificado para representar un usuario en listas de selección o filtros.
    /// </summary>
    public class UserResponseDto
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        /// <example>10</example>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario o nombre completo para mostrar en la interfaz.
        /// </summary>
        /// <example>admin_jose</example>
        public string UserName { get; set; } = string.Empty;
    }
}