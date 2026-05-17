using Comitructor.Domain.Common;
using Comitructor.Domain.Enums;

namespace Comitructor.Domain.Entities
{
    /// <summary>
    /// Representa el historial de cambios de una solicitud
    /// Registra la transición de estados para fines de auditoría.
    /// </summary>
    public class RequestHistory : BaseEntity<int>
    {
        /// <summary>
        /// Identificador de la solicitud relacionada.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Propiedad de navegación hacia la solicitud.
        /// </summary>
        public virtual Request Request { get; set; } = null!;

        /// <summary>
        /// Estado que tenía la solicitud antes del cambio.
        /// </summary>
        public RequestStatus PreviousStatus { get; set; }

        /// <summary>
        /// Nuevo estado asignado a la solicitud.
        /// </summary>
        public RequestStatus NewStatus { get; set; }

        /// <summary>
        /// Comentario o motivo opcional del cambio de estado.
        /// </summary>
        public string? ChangeReason { get; set; }
    }
}