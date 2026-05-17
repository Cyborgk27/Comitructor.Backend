namespace Comitructor.Infrastructure.Common
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje informativo o descripción del error.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Los datos devueltos por la solicitud (puede ser un objeto, una lista o nulo).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Marca de tiempo en la que se generó la respuesta.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Métodos estáticos para facilitar la creación de respuestas
        public static ApiResponse<T> SuccessResult(T data, string message = "Operación exitosa")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message };
        }

        public static ApiResponse<T> FailureResult(string message)
        {
            return new ApiResponse<T> { Success = false, Data = default, Message = message };
        }
    }
}
