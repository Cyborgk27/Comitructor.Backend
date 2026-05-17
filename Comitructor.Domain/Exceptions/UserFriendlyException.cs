namespace Comitructor.Domain.Exceptions
{
    /// <summary>
    /// Excepción diseñada para enviar mensajes claros y seguros al usuario final.
    /// Generalmente capturada por un Middleware para retornar una respuesta JSON estructurada.
    /// </summary>
    public class UserFriendlyException : Exception
    {
        /// <summary>
        /// Código de estado HTTP que debe retornar la API.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase con un mensaje específico y un código de estado.
        /// </summary>
        /// <param name="message">Mensaje descriptivo para el usuario.</param>
        /// <param name="statusCode">Código HTTP (400, 403, 404, etc.). Por defecto es 400.</param>
        public UserFriendlyException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}