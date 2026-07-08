using Microsoft.AspNetCore.Mvc;
using SalonBookingSystem.Web.Models;
using SalonBookingSystem.Web.Services;

namespace SalonBookingSystem.Web.Controllers;

/// <summary>
/// Administrator module — manage users, barbers and services.
/// Thin controller: delegates all work to the API.
/// </summary>
public class AdminController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IApiService apiService, ILogger<AdminController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            List<AppointmentViewModel> appointments = new();
            List<BarberViewModel> barbers = new();
            List<ServiceViewModel> services = new();
            List<CustomerViewModel> customers = new();

            try
            {
                var appointmentsResponse = await _apiService.GetAsync<ApiResponseViewModel<List<AppointmentViewModel>>>("appointments");
                appointments = appointmentsResponse?.Data ?? new List<AppointmentViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load appointments for dashboard");
            }

            try
            {
                var barbersResponse = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<BarberViewModel>>>("barbers?pageSize=100");
                barbers = barbersResponse?.Data?.Data ?? new List<BarberViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load barbers for dashboard");
            }

            try
            {
                var servicesResponse = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<ServiceViewModel>>>("services?pageSize=100");
                services = servicesResponse?.Data?.Data ?? new List<ServiceViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load services for dashboard");
            }

            try
            {
                var customersResponse = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<CustomerViewModel>>>("customers?pageSize=100");
                customers = customersResponse?.Data?.Data ?? new List<CustomerViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load customers for dashboard");
            }

            var today = DateTime.Today;
            var dashboard = new AdminDashboardViewModel
            {
                TotalBookings = appointments.Count,
                TotalRevenue = appointments.Where(a => a.Status == "Completed").Sum(a => a.TotalAmount),
                UpcomingBookings = appointments.Count(a => a.AppointmentDate > today && a.Status != "Cancelled"),
                PendingBookings = appointments.Count(a => a.Status == "Pending"),
                ConfirmedBookings = appointments.Count(a => a.Status == "Confirmed"),
                CompletedBookings = appointments.Count(a => a.Status == "Completed"),
                CancelledBookings = appointments.Count(a => a.Status == "Cancelled"),
                TodayBookings = appointments.Count(a => a.AppointmentDate.Date == today),
                TotalBarbers = barbers.Count,
                ActiveBarbers = barbers.Count(b => b.IsActive),
                TotalServices = services.Count,
                TotalCustomers = customers.Count,
                RecentAppointments = appointments.OrderByDescending(a => a.AppointmentDate).Take(10).ToList()
            };

            return View(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard data");
            return View(new AdminDashboardViewModel());
        }
    }

    // ── Users ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        try
        {
            var response = await _apiService.GetAsync<ApiResponseViewModel<List<UserViewModel>>>("users");
            return View(response?.Data ?? new List<UserViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load users");
            TempData["ErrorMessage"] = "Failed to load users.";
            return View(new List<UserViewModel>());
        }
    }

    [HttpGet]
    public IActionResult CreateUser() => View(new CreateUserViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var response = await _apiService.PostAsync<ApiResponseViewModel<object>>("users", model);
            if (response?.Success == true)
            {
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Users));
            }
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create user.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create user failed");
            ModelState.AddModelError(string.Empty, "An error occurred while creating user.");
        }

        return View(model);
    }

    // ── Barbers ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Barbers()
    {
        try
        {
            var response = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<BarberViewModel>>>(
                "barbers?pageSize=100");
            return View(response?.Data?.Data ?? new List<BarberViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load barbers");
            TempData["ErrorMessage"] = "Failed to load barbers.";
            return View(new List<BarberViewModel>());
        }
    }

    [HttpGet]
    public IActionResult CreateBarber() => View(new BarberViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBarber(BarberViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var request = new
            {
                model.FirstName,
                model.LastName,
                model.PhoneNumber
            };

            var response = await _apiService.PostAsync<ApiResponseViewModel<object>>("barbers", request);
            if (response?.Success == true)
            {
                TempData["SuccessMessage"] = "Barber created successfully!";
                return RedirectToAction(nameof(Barbers));
            }
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create barber.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create barber failed");
            ModelState.AddModelError(string.Empty, "An error occurred while creating barber.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateBarber(int id)
    {
        await _apiService.PostAsync<ApiResponseViewModel<object>>($"barbers/{id}/activate", new { });
        TempData["SuccessMessage"] = "Barber activated.";
        return RedirectToAction(nameof(Barbers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateBarber(int id)
    {
        await _apiService.PostAsync<ApiResponseViewModel<object>>($"barbers/{id}/deactivate", new { });
        TempData["SuccessMessage"] = "Barber deactivated.";
        return RedirectToAction(nameof(Barbers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBarber(int id)
    {
        await _apiService.DeleteAsync<ApiResponseViewModel<object>>($"barbers/{id}");
        TempData["SuccessMessage"] = "Barber deleted.";
        return RedirectToAction(nameof(Barbers));
    }

    // ── Services ──────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Services()
    {
        try
        {
            var response = await _apiService.GetAsync<ApiResponseViewModel<PagedDataViewModel<ServiceViewModel>>>(
                "services?pageSize=100");
            return View(response?.Data?.Data ?? new List<ServiceViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load services");
            TempData["ErrorMessage"] = "Failed to load services.";
            return View(new List<ServiceViewModel>());
        }
    }

    [HttpGet]
    public IActionResult CreateService() => View(new ServiceViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateService(ServiceViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var request = new
            {
                model.Name,
                model.Description,
                model.Price,
                model.DurationMinutes
            };

            var response = await _apiService.PostAsync<ApiResponseViewModel<object>>("services", request);
            if (response?.Success == true)
            {
                TempData["SuccessMessage"] = "Service created successfully!";
                return RedirectToAction(nameof(Services));
            }
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create service.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create service failed");
            ModelState.AddModelError(string.Empty, "An error occurred while creating service.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteService(int id)
    {
        await _apiService.DeleteAsync<ApiResponseViewModel<object>>($"services/{id}");
        TempData["SuccessMessage"] = "Service deleted.";
        return RedirectToAction(nameof(Services));
    }
}
