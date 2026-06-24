namespace SalonBookingSystem.Application.Common;

public class PagedQuery
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public string? Search { get; init; }

    public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;

    public int NormalizedPageSize => PageSize switch
    {
        < 1 => DefaultPageSize,
        > MaxPageSize => MaxPageSize,
        _ => PageSize
    };
}
