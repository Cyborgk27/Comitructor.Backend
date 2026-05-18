using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Comitructor.Application.Dtos.Request
{
    /// <summary>
    /// Información detallada de una solicitud de mantenimiento para su visualización.
    /// </summary>
    public class RequestDto
    {
        /// <summary>Identificador único en la base de datos.</summary>
        /// <example>1</example>
        [DefaultValue(1)]
        public int Id { get; set; }

        /// <summary>Código de negocio generado (Ej: REQ-001).</summary>
        /// <example>REQ-001</example>
        [DefaultValue("REQ-001")]
        public string Code { get; set; } = string.Empty;

        /// <summary>Título descriptivo del problema.</summary>
        /// <example>Falla en aire acondicionado</example>
        [DefaultValue("Falla en aire acondicionado")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Descripción detallada de la falla.</summary>
        /// <example>El equipo del aula 204 no enciende y presenta goteo.</example>
        [DefaultValue("El equipo del aula 204 no enciende y presenta goteo.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Estado actual (New, InProgress, Closed, etc.).</summary>
        /// <example>New</example>
        [DefaultValue("New")]
        public string Status { get; set; } = string.Empty;

        /// <summary>Nivel de urgencia (Low, Medium, High, Urgent).</summary>
        /// <example>High</example>
        [DefaultValue("High")]
        public string Priority { get; set; } = string.Empty;

        /// <summary>Departamento técnico encargado (Electric, Plumbing, etc.).</summary>
        /// <example>Infrastructure</example>
        [DefaultValue("Infrastructure")]
        public string Area { get; set; } = string.Empty;

        /// <summary>Nombre del operador asignado. Puede ser nulo si no hay asignación.</summary>
        /// <example>operador_mantenimiento</example>
        [DefaultValue("operador_mantenimiento")]
        public string? AssignedUserName { get; set; }

        /// <summary>Fecha y hora de registro de la solicitud.</summary>
        /// <example>2026-05-17T14:30:00Z</example>
        [DefaultValue("2026-05-17T14:30:00Z")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Fecha de vencimiento opcional. Por defecto se asigna 48 horas desde la creación.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Id del usuario asignado a la tarea
        /// </summary>
        [Required]
        public int AssignedUserId { get; set; }
    }
}