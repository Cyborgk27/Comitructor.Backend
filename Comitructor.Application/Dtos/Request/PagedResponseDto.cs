namespace Comitructor.Application.Dtos.Request
{
    public class PagedResponseDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
