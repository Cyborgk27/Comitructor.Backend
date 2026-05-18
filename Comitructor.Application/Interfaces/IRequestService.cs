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
        /// Actualiza los datos generales de una solicitud existente.
        /// </summary>
        /// <param name="dto">DTO que contiene el ID de la solicitud y los campos a modificar (Título, Descripción, Prioridad, Área y Fecha de Vencimiento).</param>
        /// <returns>El ID de la solicitud actualizada.</returns>
        /// <remarks>
        /// Este método no modifica el estado (Status) ni el responsable, ya que esos cambios 
        /// se gestionan a través de flujos de proceso específicos.
        /// </remarks>
        Task<int> UpdateAsync(UpdateRequestDto dto);

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

        /// <summary>
        /// Obtiene una lista simplificada de usuarios activos para alimentar componentes de selección (dropdowns).
        /// </summary>
        /// <returns>Una colección de <see cref="UserResponseDto"/> con el ID y nombre de los usuarios.</returns>
        /// <remarks>
        /// Este método se utiliza principalmente en las pantallas de administración para la asignación de responsables 
        /// a las solicitudes. Solo debe retornar usuarios que no estén marcados como eliminados y que tengan 
        /// permisos para ser asignados a tareas.
        /// </remarks>
        Task<IEnumerable<UserResponseDto>> GetUsersForSelectAsync();

    }
}
