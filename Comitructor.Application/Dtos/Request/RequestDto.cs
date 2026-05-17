namespace Comitructor.Application.Dtos.Request
{
    public record RequestDto(
        int Id,
        string Code,
        string Title,
        string Description,
        string Status,
        string Priority,
        string Area,
        string? AssignedUserName,
        DateTime CreatedDate
    );
}
