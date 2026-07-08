namespace SalonBookingSystem.Web.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<T?> DeleteAsync<T>(string endpoint);

    /// <summary>
    /// Posts data and returns the raw HttpResponseMessage.
    /// Used for login to capture the Set-Cookie header from the Identity auth response.
    /// </summary>
    Task<HttpResponseMessage> PostRawAsync(string endpoint, object data);
}
