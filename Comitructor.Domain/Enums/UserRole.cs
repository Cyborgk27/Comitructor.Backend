namespace Comitructor.Domain.Enums
{
    /// <summary>
    /// Define los roles de acceso para la autorización basada en claims
    /// </summary>
    public enum UserRole
    {
        /// <summary> Acceso total: crear, editar, asignar y cerrar solicitudes. </summary>
        Administrator = 1,

        /// <summary> Acceso limitado: consultar y actualizar estados asignados. </summary>
        Operator = 2
    }
}
