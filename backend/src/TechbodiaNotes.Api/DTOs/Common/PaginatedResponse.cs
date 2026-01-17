namespace TechbodiaNotes.Api.DTOs.Common;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }

    public static PaginatedResponse<T> Create(IEnumerable<T> data, int total, int page, int limit)
    {
        return new PaginatedResponse<T>
        {
            Data = data,
            Total = total,
            Page = page,
            Limit = limit,
            TotalPages = (int)Math.Ceiling(total / (double)limit)
        };
    }
}
