using Comitructor.Domain.Common;
using Comitructor.Domain.Enums;
using Comitructor.Domain.Exceptions;

namespace Comitructor.Domain.Entities
{
    /// <summary>
    /// Representa una solicitud interna dentro del sistema
    /// </summary>
    public class Request : BaseEntity<int>
    {
        public string Code { get; set; } = string.Empty; // SOL-2026-0001 
        public string Title { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;

        public RequestArea Area { get; set; }
        public RequestPriority Priority { get; set; } 
        public RequestStatus Status { get; set; }

        /// <summary>
        /// ID del usuario responsable (Operador o Administrador).
        /// </summary>
        public int AssignedUserId { get; set; }

        /// <summary>
        /// Propiedad de navegación al usuario asignado.
        /// </summary>
        public virtual User? AssignedUser { get; set; }
        public DateTime? DueDate { get; set; } // FechaVencimiento 
        public DateTime? ClosedDate { get; set; } // FechaCierre

        /// <summary>
        /// Colección de cambios históricos asociados a esta solicitud.
        /// </summary>
        public virtual ICollection<RequestHistory> Histories { get; set; } = new List<RequestHistory>();

        /// <summary>
        /// Regla de Negocio
        /// </summary>
        public bool IsEditable()
        {
            return Status != RequestStatus.Closed && Status != RequestStatus.Cancelled;
        }

        /// <summary>
        /// Cambia el estado de la solicitud y dispara reglas de negocio asociadas
        /// </summary>
        public void ChangeStatus(RequestStatus newStatus, string? reason = null)
        {
            if (!IsEditable())
                throw new UserFriendlyException("No se puede cambiar el estado de una solicitud cerrada o cancelada.");

            if (newStatus == RequestStatus.Closed)
            {
                ClosedDate = DateTime.Now;
            }

            Status = newStatus;
        }

        /// <summary>
        /// Asigna formalmente la solicitud a un usuario del sistema.
        /// </summary>
        public void AssignTo(User user)
        {
            if (!IsEditable())
                throw new UserFriendlyException("No se puede asignar una solicitud cerrada.");

            AssignedUserId = user.Id;
            AssignedUser = user;
        }
    }
}