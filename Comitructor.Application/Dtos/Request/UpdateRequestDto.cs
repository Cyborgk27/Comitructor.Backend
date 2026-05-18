using System.ComponentModel.DataAnnotations;

namespace Comitructor.Application.Dtos.Request
{
    /// <summary>
    /// Datos requeridos para actualizar una solicitud de mantenimiento existente.
    /// </summary>
    public class UpdateRequestDto
    {
        /// <summary>
        /// Identificador único de la solicitud a modificar.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Título actualizado de la solicitud. Máximo 120 caracteres.
        /// </summary>
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detalle actualizado de la falla o requerimiento.
        /// </summary>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Prioridad actualizada (Low, Medium, High, Critical).
        /// </summary>
        [Required]
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Área de mantenimiento responsable (Systems, Warehouse, Sales, etc.).
        /// </summary>
        [Required]
        public string Area { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de vencimiento opcional para medir tiempos de respuesta.
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Id del usuario asignado a la tarea
        /// </summary>
        [Required]
        public int AssignedUserId { get; set; }
    }
}