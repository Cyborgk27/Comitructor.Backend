namespace Comitructor.Domain.Enums
{
    /// <summary>
    /// Estados del ciclo de vida de la solicitud
    /// </summary>
    public enum RequestStatus
    {
        New = 1,
        InProgress = 2,
        OnHold = 3,
        Closed = 4,
        Cancelled = 5    
    }
}
