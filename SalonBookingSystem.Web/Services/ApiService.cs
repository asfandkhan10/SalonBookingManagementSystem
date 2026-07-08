using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SalonBookingSystem.Web.Configuration;

namespace SalonBookingSystem.Web.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiService> _logger;

    // API uses System.Text.Json camelCase — match with Newtonsoft camelCase resolver
    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    public ApiService(
        HttpClient httpClient,
        IOptions<ApiSettings> apiSettings,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;

        // IMPORTANT: BaseAddress MUST end with trailing slash.
        // HttpClient strips the last path segment without it, turning
        // "https://localhost:7251/api/v1" → "https://localhost:7251/api/"
        var baseUrl = apiSettings.Value.BaseUrl.TrimEnd('/') + "/";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    // ── Auth cookie forwarding ───────────────────────────────────────────────

    /// <summary>
    /// Attaches the stored Identity auth cookie to every outbound request so the
    /// API recognises the authenticated user. The API uses cookie-based Identity auth,
    /// not JWT — the cookie captured at login must be forwarded on every call.
    /// </summary>
    private void AttachAuthCookie()
    {
        var cookie = _httpContextAccessor.HttpContext?.Session.GetString("AuthCookie");
        if (!string.IsNullOrEmpty(cookie))
        {
            _httpClient.DefaultRequestHeaders.Remove("Cookie");
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }

    /// <summary>Strips leading slash so HttpClient keeps the full base path.</summary>
    private static string Normalize(string endpoint) => endpoint.TrimStart('/');

    // ── Public API ───────────────────────────────────────────────────────────

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        AttachAuthCookie();
        var url = Normalize(endpoint);

        try
        {
            var response = await _httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("GET {Url} → {Status}", url, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GET {Url} non-success: {Status} — {Body}", url, (int)response.StatusCode, body);
                return default;
            }

            return JsonConvert.DeserializeObject<T>(body, _jsonSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET {Url} exception", url);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        AttachAuthCookie();
        var url = Normalize(endpoint);
        using var content = new StringContent(
            JsonConvert.SerializeObject(data, _jsonSettings), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("POST {Url} → {Status}", url, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("POST {Url} non-success: {Status} — {Body}", url, (int)response.StatusCode, body);

            // Always deserialize — error responses also return ApiResponse envelope with message/errors
            return JsonConvert.DeserializeObject<T>(body, _jsonSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST {Url} exception", url);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PostRawAsync(string endpoint, object data)
    {
        AttachAuthCookie();
        var url = Normalize(endpoint);
        using var content = new StringContent(
            JsonConvert.SerializeObject(data, _jsonSettings), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        _logger.LogDebug("POST(raw) {Url} → {Status}", url, (int)response.StatusCode);
        return response;
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        AttachAuthCookie();
        var url = Normalize(endpoint);
        using var content = new StringContent(
            JsonConvert.SerializeObject(data, _jsonSettings), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PutAsync(url, content);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("PUT {Url} → {Status}", url, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("PUT {Url} non-success: {Status} — {Body}", url, (int)response.StatusCode, body);

            return JsonConvert.DeserializeObject<T>(body, _jsonSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT {Url} exception", url);
            throw;
        }
    }

    public async Task<T?> DeleteAsync<T>(string endpoint)
    {
        AttachAuthCookie();
        var url = Normalize(endpoint);

        try
        {
            var response = await _httpClient.DeleteAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("DELETE {Url} → {Status}", url, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("DELETE {Url} non-success: {Status} — {Body}", url, (int)response.StatusCode, body);

            return JsonConvert.DeserializeObject<T>(body, _jsonSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE {Url} exception", url);
            throw;
        }
    }
}
