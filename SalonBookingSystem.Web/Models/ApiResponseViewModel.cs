namespace SalonBookingSystem.Web.Models;

/// <summary>
/// Maps the standard ApiResponse&lt;T&gt; envelope returned by the API.
/// </summary>
public class ApiResponseViewModel<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
