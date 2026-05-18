namespace Comitructor.Application.Dtos
{
    /// <summary>
    /// Representa las métricas generales para las tarjetas de resumen del dashboard.
    /// </summary>
    public class RequestSummaryDto
    {
        /// <summary> Total histórico de solicitudes creadas. </summary>
        /// <example>150</example>
        public int TotalRequests { get; set; }

        /// <summary> Solicitudes en estado 'New' o 'InProgress'. </summary>
        /// <example>45</example>
        public int OpenRequests { get; set; }

        /// <summary> Solicitudes con prioridad 'Critical'. </summary>
        /// <example>12</example>
        public int CriticalRequests { get; set; }

        /// <summary> Solicitudes cuya fecha de vencimiento es menor a la actual y no están cerradas. </summary>
        /// <example>5</example>
        public int OverdueRequests { get; set; }

        /// <summary> Solicitudes en estado 'Closed' o 'Cancelled'. </summary>
        /// <example>88</example>
        public int ClosedRequests { get; set; }
    }
}
