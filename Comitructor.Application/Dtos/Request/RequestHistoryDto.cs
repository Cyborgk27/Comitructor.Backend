namespace Comitructor.Application.Dtos.Request
{
    public class RequestHistoryDto
    {
        /// <summary> Identificador del registro de historial. </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary> Estado previo antes del cambio. </summary>
        /// <example>New</example>
        public string PreviousStatus { get; set; } = string.Empty;

        /// <summary> Nuevo estado asignado. </summary>
        /// <example>InProgress</example>
        public string NewStatus { get; set; } = string.Empty;

        /// <summary> Motivo o comentario del cambio de estado. </summary>
        /// <example>Se asignó técnico para revisión de hardware.</example>
        public string? ChangeReason { get; set; }

        /// <summary> Fecha y hora en que se registró el movimiento. </summary>
        /// <example>2026-05-18T14:30:00Z</example>
        public DateTime CreatedDate { get; set; }

        /// <summary> Nombre del usuario responsable del cambio. </summary>
        /// <example>Kevin Stalin</example>
        public string UserName { get; set; } = string.Empty;
    }
}
