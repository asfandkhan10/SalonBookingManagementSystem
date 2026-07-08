using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SalonBookingSystem.Web.Models;
using SalonBookingSystem.Web.Services;

namespace SalonBookingSystem.Web.Controllers;

/// <summary>
/// Handles Login, Register and Logout.
/// On login: calls the API, captures the Set-Cookie Identity header,
/// stores it in session so all subsequent API calls pass it as a Cookie header.
/// </summary>
public class AuthController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AuthController> _logger;

    private static readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    public AuthController(IApiService apiService, ILogger<AuthController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // ── Register ─────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var response = await _apiService.PostAsync<ApiResponseViewModel<AuthenticationResponse>>(
                "/auth/register", new
                {
                    model.Email,
                    model.Password,
                    model.ConfirmPassword,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber
                });

            if (response?.Success == true)
            {
                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction(nameof(Login));
            }

            var error = response?.Message ?? "Registration failed.";
            if (response?.Errors?.Count > 0)
                error = string.Join(", ", response.Errors);

            ModelState.AddModelError(string.Empty, error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration.");
        }

        return View(model);
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            // Use PostRawAsync so we can capture the Set-Cookie header from the API.
            // The API uses ASP.NET Core Identity cookie auth — we must forward that
            // cookie on every subsequent API call for protected endpoints to work.
            var rawResponse = await _apiService.PostRawAsync(
                "/auth/login",
                new { model.Email, model.Password });

            var body = await rawResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ApiResponseViewModel<AuthenticationResponse>>(
                body, _jsonSettings);

            if (response?.Success == true && response.Data != null)
            {
                // Capture the Identity auth cookie from the API response
                if (rawResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    // Store all cookie values as a single Cookie header string
                    var cookieHeader = string.Join("; ", cookies.Select(c =>
                    {
                        // Keep only name=value part (strip Path, HttpOnly, etc.)
                        var part = c.Split(';')[0].Trim();
                        return part;
                    }));
                    HttpContext.Session.SetString("AuthCookie", cookieHeader);
                    _logger.LogInformation("Auth cookie captured and stored in session");
                }

                // Store user info in session
                if (response.Data.CustomerId.HasValue)
                    HttpContext.Session.SetInt32("CustomerId", response.Data.CustomerId.Value);

                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("IsLoggedIn", "true");

                TempData["SuccessMessage"] = "Login successful!";

                var returnUrl = TempData["ReturnUrl"] as string;
                return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
            }

            ModelState.AddModelError(string.Empty, response?.Message ?? "Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login.");
        }

        return View(model);
    }

    // ── Logout ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Tell the API to sign out (clears server-side Identity session)
            await _apiService.PostAsync<ApiResponseViewModel<object>>("/auth/logout", new { });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API logout call failed — clearing local session anyway");
        }

        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "Logged out successfully.";
        return RedirectToAction(nameof(Login));
    }
}
