namespace Comitructor.Domain.Common
{
    /// <summary>
    /// Contrato unificado para la auditoría y trazabilidad de entidades.
    /// Incluye soporte nativo para Soft Delete y control de modificaciones.
    /// </summary>
    public interface IAuditableEntity
    {
        // --- Sección de Creación ---
        /// <summary> Identificador del usuario que creó el registro. </summary>
        public int? CreatedBy { get; set; }
        /// <summary> Fecha y hora de creación. </summary>
        public DateTime? CreatedDate { get; set; }

        // --- Sección de Modificación ---
        /// <summary> Identificador del último usuario que editó el registro. </summary>
        public int? LastModifiedBy { get; set; }
        /// <summary> Fecha y hora de la última actualización. </summary>
        public DateTime? LastModifiedDate { get; set; }

        // --- Sección de Borrado Lógico (Soft Delete) ---
        /// <summary> Identificador del usuario que eliminó el registro. </summary>
        public int? DeletedBy { get; set; }
        /// <summary> Fecha y hora de la eliminación lógica. </summary>
        public DateTime? DeletedDate { get; set; }
        /// <summary> Indica si el registro está marcado como eliminado (borrado lógico). </summary>
        public bool IsDeleted { get; set; }
    }
}