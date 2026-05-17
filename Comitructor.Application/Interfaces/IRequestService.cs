using Comitructor.Application.Dtos.Request;

namespace Comitructor.Application.Interfaces
{
    /// <summary>
    /// Define los casos de uso para la gestión de solicitudes de mantenimiento.
    /// </summary>
    public interface IRequestService
    {
        /// <summary>
        /// Crea una nueva solicitud en estado 'New'.
        /// La auditoría debe registrar automáticamente al creador.
        /// </summary>
        /// <param name="dto">Datos de la solicitud (Título, Descripción, Prioridad, Área).</param>
        /// <returns>El ID de la solicitud creada o el DTO completo.</returns>
        Task<int> CreateAsync(CreateRequestDto dto);

        /// <summary>
        /// Obtiene todas las solicitudes con filtros aplicados (por Rol o Estado).
        /// Si el usuario es 'Operator', solo debería ver las que tiene asignadas.
        /// </summary>
        Task<IEnumerable<RequestDto>> GetAllAsync();

        /// <summary>
        /// Obtiene el detalle de una solicitud específica, incluyendo su historial.
        /// </summary>
        Task<RequestDto?> GetByIdAsync(int id);

        /// <summary>
        /// Cambia el estado de una solicitud
        /// Debe validar que un 'Operator' no cierre solicitudes que no le pertenecen.
        /// </summary>
        /// <param name="requestId">ID de la solicitud.</param>
        /// <param name="newStatus">Nuevo estado (InProgress, Closed, etc).</param>
        /// <param name="reason">Motivo del cambio para el historial.</param>
        Task UpdateStatusAsync(int requestId, string newStatus, string reason);

        /// <summary>
        /// Asigna una solicitud a un operador específico (Solo para Administrators).
        /// </summary>
        Task AssignRequestAsync(int requestId, int userId);

        /// <summary>
        /// Eliminado lógico de una solicitud (Soft Delete).
        /// </summary>
        Task DeleteAsync(int id);
    }
}
