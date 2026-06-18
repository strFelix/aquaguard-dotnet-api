namespace AquaGuard.API.ViewModels.Shared;

public class PaginatedResponseViewModel<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
}