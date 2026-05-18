using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Comitructor.Application.Dtos.Request
{
    /// <summary>
    /// Datos requeridos para registrar una nueva solicitud de mantenimiento.
    /// </summary>
    public class CreateRequestDto
    {
        /// <summary>
        /// Título corto que resuma el problema.
        /// </summary>
        /// <example>Falla de iluminación en pasillo B</example>
        [Required]
        [DefaultValue("Falla de iluminación en pasillo B")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detalle completo de la falla o requerimiento.
        /// </summary>
        /// <example>Se reportan tres luminarias intermitentes en el ala norte del segundo piso.</example>
        [Required]
        [DefaultValue("Se reportan tres luminarias intermitentes en el ala norte del segundo piso.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Id del usuario asignado a la tarea
        /// </summary>
        [Required]
        public int AssignedUserId { get; set; }

        /// <summary>
        /// Prioridad de la solicitud (Low, Medium, High, Urgent).
        /// </summary>
        /// <example>Medium</example>
        [Required]
        [DefaultValue("Medium")]
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Área de mantenimiento (Electric, Plumbing, Infrastructure, etc.).
        /// </summary>
        /// <example>Electric</example>
        [Required]
        [DefaultValue("Electric")]
        public string Area { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de vencimiento opcional. Por defecto se asigna 48 horas desde la creación.
        /// </summary>
        public DateTime? DueDate { get; set; } = null;
    }
}