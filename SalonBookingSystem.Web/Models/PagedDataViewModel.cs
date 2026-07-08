using Newtonsoft.Json;

namespace SalonBookingSystem.Web.Models;

/// <summary>
/// Maps the PagedResult&lt;T&gt; returned by paginated API endpoints.
/// API sends: { "pageNumber": 1, "pageSize": 10, "totalRecords": 5, "totalPages": 1, "data": [...] }
/// </summary>
public class PagedDataViewModel<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }

    // API returns IReadOnlyList serialized as a JSON array — deserialize into List<T>
    public List<T> Data { get; set; } = new();
}
